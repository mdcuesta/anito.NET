using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{
    public partial class Item : DataObject
    {
        public Item(ISession session) :
            base(session)
        {
        }
        public Item()
        {
        }

        private int _ID;

        [DataField(FieldName = "ID"
            , MemberName = "_ID"
            , Identity = true
            )]
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                SetField<int>("ID", ref _ID, value);
            }
        }

        private string _ItemCode;

        [DataField(FieldName = "ItemCode"
            , MemberName = "_ItemCode"
            , PrimaryKey = true
            , Size = 30
            )]
        public string ItemCode
        {
            get
            {
                return _ItemCode;
            }
            set
            {
                SetField<string>("ItemCode", ref _ItemCode, value);
            }
        }

        private string _DisplayCode;

        [DataField(FieldName = "DisplayCode"
            , MemberName = "_DisplayCode"
            , Size = 60
            )]
        public string DisplayCode
        {
            get
            {
                return _DisplayCode;
            }
            set
            {
                SetField<string>("DisplayCode", ref _DisplayCode, value);
            }
        }

        private string _ItemName;

        [DataField(FieldName = "ItemName"
            , MemberName = "_ItemName"
            , Size = 60
            )]
        public string ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                SetField<string>("ItemName", ref _ItemName, value);
            }
        }

        private string _Description;

        [DataField(FieldName = "Description"
            , MemberName = "_Description"
            , Size = 250
            )]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetField<string>("Description", ref _Description, value);
            }
        }

        private string _Type;

        [DataField(FieldName = "Type"
            , MemberName = "_Type"
            , Size = 60
            )]
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetField<string>("Type", ref _Type, value);
            }
        }

        private decimal _Price;

        [DataField(FieldName = "Price"
            , MemberName = "_Price"
            )]
        public decimal Price
        {
            get
            {
                return _Price;
            }
            set
            {
                SetField<decimal>("Price", ref _Price, value);
            }
        }

        private decimal _PriceRate;

        [DataField(FieldName = "PriceRate"
            , MemberName = "_PriceRate"
            )]
        public decimal PriceRate
        {
            get
            {
                return _PriceRate;
            }
            set
            {
                SetField<decimal>("PriceRate", ref _PriceRate, value);
            }
        }
    }
}
