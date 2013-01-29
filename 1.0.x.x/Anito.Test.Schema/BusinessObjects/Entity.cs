using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{
    public partial class Entity
    {
        private DataObjectRef<Profile> _Profile;

        [Association(Relationship = Relation.OneToOne
            , SourceMember = "_Profile"
            , SourceKey = "ProfileCode"
            , ReferenceKey = "ProfileCode"
            , ViewOnly = false)]
        public Profile Profile
        {
            get
            {
                return _Profile.DataObject;
            }
            set
            {
                _Profile.DataObject = value;
            }
        }

        private DataObjectSet<List<SalesOrder>, SalesOrder> _SalesOrder;

        [Association(Relationship = Relation.OneToMany
            , SourceMember = "_SalesOrder"
            , SourceKey = "EntityCode"
            , ReferenceKey = "EntityCode"
            , ViewOnly = false)]
        public List<SalesOrder> Orders
        {
            get
            {
                return _SalesOrder.Details;
            }
            set
            {
                _SalesOrder.Details = value;
            }
        }
    }
}
