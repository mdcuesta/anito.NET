/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Serialization;
using Anito.Data.Configuration;
using Anito.Data.Schema;
using System.Security.Permissions;

namespace Anito.Data
{
    [type: ReflectionPermission(SecurityAction.Assert)]
    public static class ProviderFactory
    {
        #region Variables
        private static readonly Dictionary<string, ProviderSetting> s_providerCache;
        private static Dictionary<Type, Delegate> s_createProviderMethodsCache;
        private static ProviderConfiguration s_configuration;

        private class ProviderSetting
        {
            public string Name { get; set; }
            public string ProviderTypeName { get; set; }
            public string ConnectionString { get; set; }
            public Type ProviderType { get; set; }
        }
        #endregion

        #region Properties
        private static Dictionary<Type, Delegate> CreateProviderMethodsCache
        {
            get
            {
                s_createProviderMethodsCache = s_createProviderMethodsCache ?? new Dictionary<Type, Delegate>();
                return s_createProviderMethodsCache;
            }
        }

        private static Dictionary<string, ProviderSetting> ProviderCache
        {
            get
            {
                return s_providerCache;
            }
        }

        private static ProviderConfiguration Configuration
        {
            get
            {
                if (s_configuration == null)
                    InitConfiguration();              
                return s_configuration;
            }
        }
        #endregion

        #region Constructor

        static ProviderFactory()
        {
            s_providerCache = new Dictionary<string, ProviderSetting>();
            InitConfiguration();
        }

        #endregion

        #region InitConfiguration
        private static void InitConfiguration()
        {
            var section = System.Configuration.ConfigurationManager.GetSection("AnitoProviderConfiguration");
            if (section == null)
                throw new Exception("Default Anito Provider Configuration Section doesn't exist (AnitoProviderConfiguration)");
            s_configuration = section as ProviderConfiguration;

            if(Equals(s_configuration, null))
                throw new NullReferenceException("s_configuration");

            foreach (ProviderConfigurationElement element in s_configuration.Providers)
            {
                var setting = new ProviderSetting{
                    Name = element.Name
                    , ConnectionString = element.ConnectionString
                    , ProviderTypeName = element.Type};

                ProviderCache.Add(element.Name, setting);

                var source = new TypeSchemaSource();
                foreach (SchemaSourceElement schemaElement in element.SchemaSourceCollection)
                {
                    if(!File.Exists(schemaElement.SourceFile))
                        throw new FileNotFoundException(schemaElement.SourceFile);
                    XmlReader reader = new XmlTextReader(schemaElement.SourceFile);
                    (source as IXmlSerializable).ReadXml(reader);
                    reader.Close();
                }
                CacheManager.SchemaCache.Add(element.Name, source);
            }    
        }
        #endregion

        #region GetProvider
        public static IProvider GetProvider()
        {
            return GetProvider(Configuration.DefaultProvider);
        }

        public static IProvider GetProvider(string providerName)
        {
            if (!ProviderCache.ContainsKey(providerName))
                throw new Exception("Unknown Provider. (See Provider Configuration)");

            ProviderSetting setting = ProviderCache[providerName];

            if (setting.ProviderType == null)
            {
                Type type = Type.GetType(setting.ProviderTypeName);
                if (type == null)
                    throw new Exception(string.Format("Unknown Type {0}", setting.ProviderTypeName));
                setting.ProviderType = type;
            }
            IProvider provider = CreateIProviderInstance(setting.ProviderType);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[setting.ConnectionString].ConnectionString;

            if (!string.IsNullOrEmpty(connectionString))
                provider.ConnectionString = connectionString;

            if (typeof(ProviderBase).IsAssignableFrom(provider.GetType()))
            {
                var providerBase = provider as ProviderBase;
                if (Equals(providerBase, null))
                    throw new NullReferenceException("providerBase");

                providerBase.SetProviderName(providerName);
            }

            return provider;
        }

        public static ISession GetSession()
        {
            return GetSession(GetProvider());
        }

        public static ISession GetSession(IProvider provider)
        {
            return new DataSession(provider);
        }

        public static ISession GetSession(string providerName)
        {
            return GetSession(GetProvider(providerName));
        }

        public static void SetProviderConfiguration(ProviderConfiguration configuration)
        {
            s_configuration = configuration;
        }

        private static IProvider CreateIProviderInstance(Type type)
        {
            if (!CreateProviderMethodsCache.ContainsKey(type))
            {
                var dm = new DynamicMethod("CreateIProviderInstance", typeof(IProvider), Type.EmptyTypes, type);
                var il = dm.GetILGenerator();
                il.DeclareLocal(type);
                var constructorInfo = type.GetConstructor(Type.EmptyTypes);
                if(Equals(constructorInfo, null))
                    throw new Exception(string.Format("There is no parameterless constructor present in type {0}", type.FullName));

                il.Emit(OpCodes.Newobj, constructorInfo);
                il.Emit(OpCodes.Ret);
                CreateProviderMethodsCache.Add(type, dm.CreateDelegate(typeof(CreateIProviderDelegate)));
            }
            var method = CreateProviderMethodsCache[type] as CreateIProviderDelegate;
            if(Equals(method, null))
                throw new Exception(string.Format("Unable to create instance of type {0}", type.FullName));
            return method.Invoke();
        }
        #endregion
    }
}
