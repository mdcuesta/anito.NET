/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Anito.Data.Mapping;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Security.Permissions;

namespace Anito.Data.Schema
{
    [type: ReflectionPermission(SecurityAction.Assert)]
    public class TypeRelation : IXmlSerializable
    {
        private List<TypeColumn> m_sourceColumns;
        private MemberInfo m_memberInfo;

        public Relation Relation { get; private set;}
        public RelationHierarchy? Hierarchy { get; private set; }
        public Type ObjectType { get; private set; }
        public string MemberName { get; private set; }
        public bool ViewOnly { get; private set; }

        public List<TypeColumn> SourceColumns
        {
            get 
            { 
                m_sourceColumns = m_sourceColumns ?? new List<TypeColumn>();
                return m_sourceColumns;
            }
        }

        public TypeTable Table { get; set; }

        public TypeRelation()
        { 
        
        }
        
        internal TypeRelation(Relation relation,
            RelationHierarchy hierarchy,
            Type objectType, 
            string memberName,
            MemberInfo memberInfo)
        {
            Relation = relation;
            Hierarchy = hierarchy;
            ObjectType = objectType;
            MemberName = memberName;
            m_memberInfo = memberInfo;
        }

        public void SetFieldValue(object item, object value)
        {
            switch (m_memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fInfo = m_memberInfo as FieldInfo;
                    if(fInfo == null) break;
                    fInfo.SetValue(item, value);
                    break;
                case MemberTypes.Property:
                    var pInfo = m_memberInfo as PropertyInfo;
                    if(pInfo == null) break;
                    pInfo.SetValue(item, value, null);
                    break;
            } 
        }

        public object GetFieldValue(object item)
        {
            switch (m_memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fInfo = m_memberInfo as FieldInfo;
                    if (fInfo == null) break;
                    return fInfo.GetValue(item);
                case MemberTypes.Property:
                    var pInfo = m_memberInfo as PropertyInfo;
                    if (pInfo == null) break;
                    return pInfo.GetValue(item, null);
            }
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var relation = Relation.OneToOne;
            var storage = string.Empty;
            var viewOnly = false;
            RelationHierarchy? relationHierarchy = null;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case XmlConstants.TypeRelation.RELATION:
                        relation = (reader.Value == XmlConstants.TypeRelation.ONE_TO_ONE) ? Relation.OneToOne : Relation.OneToMany;
                        break;
                    case XmlConstants.TypeRelation.HIERARCHY:
                        relationHierarchy = (RelationHierarchy)Enum.Parse(typeof(RelationHierarchy), reader.Value, true);
                        break;
                    case XmlConstants.TypeRelation.STORAGE:
                        storage = reader.Value;
                        break;
                    case XmlConstants.TypeRelation.VIEW_ONLY:
                        viewOnly = bool.Parse(reader.Value);
                        break;
                } 
            }
            Relation = relation;
            Hierarchy = relationHierarchy;
            MemberName = storage;
            ViewOnly = viewOnly;

            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == XmlConstants.TypeRelation.REFERENCE)
                {
                    if (Equals(Table, null)) continue;

                    var source = reader.GetAttribute("SourceKey");
                    
                    var parentColumn = Table.FirstOrDefault(col => col.Name == source);

                    if (Equals(parentColumn, null))
                        throw new Exception(string.Format("Property {0} not found in type {1}", source, Table.MappedObjectType.FullName));

                    SourceColumns.Add(parentColumn);
                }

                if ((reader.NodeType == XmlNodeType.EndElement && (reader.Name == XmlConstants.ASSOCIATION
                   || reader.Name == XmlConstants.ASSOCIATIONS)) || reader.NodeType == XmlNodeType.Element && reader.Name == XmlConstants.ASSOCIATION)
                    break;

            }
        }

        internal void ReadXml(XmlReader reader, Type type, TypeTable table)
        {
            var relation = Relation.OneToOne;
            var storage = string.Empty;
            var viewOnly = false;
            RelationHierarchy? relationHierarchy = null;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case XmlConstants.TypeRelation.RELATION:
                        relation = (reader.Value == XmlConstants.TypeRelation.ONE_TO_ONE) ? Relation.OneToOne : Relation.OneToMany;
                        break;
                    case XmlConstants.TypeRelation.HIERARCHY:
                        relationHierarchy = (RelationHierarchy)Enum.Parse(typeof(RelationHierarchy), reader.Value, true);
                        break;
                    case XmlConstants.TypeRelation.STORAGE:
                        storage = reader.Value;
                        break;
                    case XmlConstants.TypeRelation.VIEW_ONLY:
                        viewOnly = bool.Parse(reader.Value);
                        break;
                }  
            }

            var memberInfo = GetMemberInfo(type, storage);

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    var pInfo = memberInfo as PropertyInfo;
                    if(pInfo == null) break;
                    ObjectType = pInfo.PropertyType;
                    break;
                case MemberTypes.Field:
                    var fInfo = memberInfo as FieldInfo;
                    if(fInfo == null) break;
                    ObjectType = fInfo.FieldType;
                    break;
            }

            Relation = relation;
            Hierarchy = relationHierarchy;
            MemberName = storage;
            ViewOnly = viewOnly;
            m_memberInfo = memberInfo;
            
            if(reader.IsEmptyElement)
                return;
            
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == XmlConstants.TypeRelation.REFERENCE)
                {
                    var source = reader.GetAttribute(XmlConstants.TypeRelation.SOURCE_KEY);
                    //var reference = reader.GetAttribute(Constants.TypeRelation.REFERENCE_KEY);

                    var parentColumn = table.FirstOrDefault(col => col.Name == source);

                    if(Equals(parentColumn, null))
                        throw new Exception(string.Format("Property {0} not found in type {1}", source, table.MappedObjectType.FullName));

                    SourceColumns.Add(parentColumn);
                }

                if ((reader.NodeType == XmlNodeType.EndElement && (reader.Name == XmlConstants.ASSOCIATION
                   || reader.Name == XmlConstants.ASSOCIATIONS)) || reader.NodeType == XmlNodeType.Element && reader.Name == XmlConstants.ASSOCIATION)
                    break;

            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(XmlConstants.ASSOCIATION);

            writer.WriteStartAttribute(XmlConstants.TypeRelation.RELATION);
            writer.WriteValue((Relation == Relation.OneToOne) ? XmlConstants.TypeRelation.ONE_TO_ONE : XmlConstants.TypeRelation.ONE_TO_MANY);
            writer.WriteEndAttribute();

            if (Hierarchy != null)
            {
                writer.WriteStartAttribute(XmlConstants.TypeRelation.HIERARCHY);
                writer.WriteValue(Hierarchy.ToString());
                writer.WriteEndAttribute();
            }

            writer.WriteStartAttribute(XmlConstants.TypeRelation.STORAGE);
            writer.WriteValue(MemberName);
            writer.WriteEndAttribute();

            if (ViewOnly)
            {
                writer.WriteStartAttribute(XmlConstants.TypeRelation.VIEW_ONLY);
                writer.WriteValue(ViewOnly);
                writer.WriteEndAttribute();
            }

            writer.WriteEndElement();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        private static MemberInfo GetMemberInfo(Type type, string memberName)
        {
            var typeInfos = type.GetMember(memberName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            if (typeInfos.Count() > 0) return typeInfos[0];
            return (type.BaseType != null) ? GetMemberInfo(type.BaseType, memberName)
                : null;
        }
    }
}
