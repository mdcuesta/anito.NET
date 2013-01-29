using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Data.Entities
{
    public class Customer
    {
        [DataField(Identity = true, PrimaryKey = true)]
        public int ID { get; set; }

        [DataField]
        public string Name { get; set; }

        [DataField]
        public int AddressID { get; set; }

        private DataObjectRef<Address> m_address;
        [Association(
            Relationship = Relation.OneToOne, 
            Hierarchy = RelationHierarchy.Child,
            SourceKey = "AddressID", 
            ReferenceKey = "ID", 
            SourceMember = "m_address")]
        public Address Address
        {
            get { return m_address.DataObject; }
        }


    }
    
}
