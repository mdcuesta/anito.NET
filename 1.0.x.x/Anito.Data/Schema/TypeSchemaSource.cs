/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Security.Permissions;
using Anito.Data.Mapping;

namespace Anito.Data.Schema
{
    [type: ReflectionPermission(SecurityAction.Assert)]
    [XmlRoot(XmlConstants.SCHEMA, IsNullable = false)]
    public class TypeSchemaSource : Dictionary<Type, TypeTable>, IXmlSerializable
    {

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            //TODO write namespace here
            foreach (var entry in this)
            {
                writer.WriteStartElement(XmlConstants.DATA_OBJECT);
                (entry.Value as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            string currentNamespace = null, currentNamespaceAssembly = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == XmlConstants.NAMESPACE)
                {
                    currentNamespace = reader.GetAttribute(XmlConstants.Namespace.ID);
                    currentNamespaceAssembly = reader.GetAttribute(XmlConstants.Namespace.ASSEMBLY);
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlConstants.NAMESPACE)
                {
                    currentNamespace = null;
                    currentNamespaceAssembly = null;
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == XmlConstants.DATA_OBJECT)
                {
                    var table = new TypeTable(currentNamespace, currentNamespaceAssembly);
                    (table as IXmlSerializable).ReadXml(reader);
                    if(!ContainsKey(table.MappedObjectType))
                        Add(table.MappedObjectType, table);
                }
                
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlConstants.SCHEMA)
                {
                    break;
                }
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }
    }
}
