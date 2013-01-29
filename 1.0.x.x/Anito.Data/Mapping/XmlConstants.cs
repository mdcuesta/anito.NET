namespace Anito.Data.Mapping
{
    internal static class XmlConstants
    {
        public const string SCHEMA = "Schema";
        public const string NAMESPACE = "Namespace";
        public const string DATA_OBJECT = "DataObject";

        public const string ASSOCIATION = "Association";
        public const string ASSOCIATIONS = "Associations";

        public static class Namespace
        {
            public const string ID = "ID";
            public const string ASSEMBLY = "Assembly";
        }

        public static class TypeRelation
        {
            public const string REFERENCE = "Reference";
            public const string SOURCE_KEY = "SourceKey";
            public const string REFERENCE_KEY = "ReferenceKey";
            public const string RELATION = "Relation";
            public const string ONE_TO_ONE = "OneToOne";
            public const string ONE_TO_MANY = "OneToMany";
            public const string HIERARCHY = "Hierarchy";
            public const string STORAGE = "Storage";
            public const string VIEW_ONLY = "ViewOnly";
        }
    }
}
