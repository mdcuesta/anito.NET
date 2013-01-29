using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{
    public partial class Profile
    {

        DataObjectRef<Address> _Address;

        [Association(Relationship = Relation.OneToOne
            , SourceMember = "_Address"
            , SourceKey = "AddressCode"
            , ReferenceKey = "AddressCode"
            , ViewOnly = false)]
        public Address Address
        {
            get
            {
                return _Address.DataObject;
            }
            set
            {
                _Address.DataObject = value;
            }
        }
    }
}
