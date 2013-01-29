using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{
 
    public partial class SalesOrderDetail
    {
        private DataObjectRef<Item> _Item;

        [Association(Relationship = Relation.OneToOne
            , SourceMember = "_Item"
            , SourceKey = "ItemCode"
            , ReferenceKey = "ItemCode"
            , ViewOnly = false)]
        public Item Item
        {
            get
            {
                return _Item.DataObject;
            }
            set
            {
                _Item.DataObject = value;
            }
        }
    }
}
