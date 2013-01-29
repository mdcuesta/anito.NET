using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{
   
    public partial class SalesOrder
    {
        DataObjectSet<List<SalesOrderDetail>, SalesOrderDetail> _SalesOrderDetail;

        [Association(Relationship = Relation.OneToMany
            , Hierarchy = RelationHierarchy.Parent
            , SourceMember = "_SalesOrderDetail"
            , SourceKey = "SalesOrderCode"
            , ReferenceKey = "SalesOrderCode"
            , ViewOnly = false)]
        public List<SalesOrderDetail> Details
        {
            get
            {
                return _SalesOrderDetail.Details;
            }
            set
            {
                _SalesOrderDetail.Details = value;
            }
        }

        DataObjectRef<Entity> _Customer;

        [Association(Relationship = Relation.OneToOne
            , SourceMember = "_Customer"
            , SourceKey = "EntityCode, Balance"
            , ReferenceKey = "EntityCode, Balance"
            , ViewOnly = false)]
        public Entity Customer
        {
            get
            {
                return _Customer.DataObject;
            }
            set
            {
                _Customer.DataObject = value;
            }
        }

    }
}
