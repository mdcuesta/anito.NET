using Anito.Data.Mapping;

namespace Anito.Test.Data.Entities
{
    public class Address
    {
        [DataField(Identity = true, PrimaryKey = true)]
        public int ID { get; set; }

        [DataField]
        public string Country { get; set; }

        [DataField]
        public string State { get; set; }

        [DataField]
        public string Address1 { get; set; }

        [DataField]
        public string Address2 { get; set; }

        [DataField]
        public string ZipCode { get; set; }
    }
}
