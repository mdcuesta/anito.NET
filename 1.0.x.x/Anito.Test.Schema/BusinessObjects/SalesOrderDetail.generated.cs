using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{
    public partial class SalesOrderDetail : DataObject
    {
        public SalesOrderDetail(ISession session) :
            base(session)
        {
        }
        public SalesOrderDetail()
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

        private string _SalesOrderCode;

        [DataField(FieldName = "SalesOrderCode"
            , MemberName = "_SalesOrderCode"
            , PrimaryKey = true
            , Size = 50
            )]
        public string SalesOrderCode
        {
            get
            {
                return _SalesOrderCode;
            }
            set
            {
                SetField<string>("SalesOrderCode", ref _SalesOrderCode, value);
            }
        }

        private int _LineNum;

        [DataField(FieldName = "LineNum"
            , MemberName = "_LineNum"
            , PrimaryKey = false
            )]
        public int LineNum
        {
            get
            {
                return _LineNum;
            }
            set
            {
                SetField<int>("LineNum", ref _LineNum, value);
            }
        }

        private string _ItemCode;

        [DataField(FieldName = "ItemCode"
            , MemberName = "_ItemCode"
            , PrimaryKey = false
            , Size = 50
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

        private float _QuantityOrdered;

        [DataField(FieldName = "QuantityOrdered"
            , MemberName = "_QuantityOrdered"
            )]
        public float QuantityOrdered
        {
            get
            {
                return _QuantityOrdered;
            }
            set
            {
                SetField<float>("QuantityOrdered", ref _QuantityOrdered, value);
            }
        }

        private float _QuantityInvoiced;

        [DataField(FieldName = "QuantityInvoiced"
            , MemberName = "_QuantityInvoiced"
            )]
        public float QuantityInvoiced
        {
            get
            {
                return _QuantityInvoiced;
            }
            set
            {
                SetField<float>("QuantityInvoiced", ref _QuantityInvoiced, value);
            }
        }


        private decimal _ActualPrice;

        [DataField(FieldName = "ActualPrice"
            , MemberName = "_ActualPrice"
            )]
        public decimal ActualPrice
        {
            get
            {
                return _ActualPrice;
            }
            set
            {
                SetField<decimal>("ActualPrice", ref _ActualPrice, value);
            }
        }

        private decimal _ActualPriceRate;

        [DataField(FieldName = "ActualPriceRate"
            , MemberName = "_ActualPriceRate"
            )]
        public decimal ActualPriceRate
        {
            get
            {
                return _ActualPriceRate;
            }
            set
            {
                SetField<decimal>("ActualPriceRate", ref _ActualPriceRate, value);
            }
        }

        private decimal _Discount;

        [DataField(FieldName = "Discount"
            , MemberName = "_Discount"
            )]
        public decimal Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                SetField<decimal>("Discount", ref _Discount, value);
            }
        }

        private decimal _DiscountRate;

        [DataField(FieldName = "DiscountRate"
            , MemberName = "_DiscountRate"
            )]
        public decimal DiscountRate
        {
            get
            {
                return _DiscountRate;
            }
            set
            {
                SetField<decimal>("DiscountRate", ref _DiscountRate, value);
            }
        }

        private decimal _Tax;

        [DataField(FieldName = "Tax"
            , MemberName = "_Tax"
            )]
        public decimal Tax
        {
            get
            {
                return _Tax;
            }
            set
            {
                SetField<decimal>("Tax", ref _Tax, value);
            }
        }

        private decimal _TaxRate;

        [DataField(FieldName = "TaxRate"
            , MemberName = "_TaxRate"
            )]
        public decimal TaxRate
        {
            get
            {
                return _TaxRate;
            }
            set
            {
                SetField<decimal>("TaxRate", ref _TaxRate, value);
            }
        }

        private decimal _ExtendedPrice;

        [DataField(FieldName = "ExtendedPrice"
            , MemberName = "_ExtendedPrice"
            )]
        public decimal ExtendedPrice
        {
            get
            {
                return _ExtendedPrice;
            }
            set
            {
                SetField<decimal>("ExtendedPrice", ref _ExtendedPrice, value);
            }
        }

        private decimal _ExtendedPriceRate;

        [DataField(FieldName = "ExtendedPriceRate"
            , MemberName = "_ExtendedPriceRate"
            )]
        public decimal ExtendedPriceRate
        {
            get
            {
                return _ExtendedPriceRate;
            }
            set
            {
                SetField<decimal>("ExtendedPriceRate", ref _ExtendedPriceRate, value);
            }
        }

        private decimal _Total;

        [DataField(FieldName = "Total"
            , MemberName = "_Total"
            )]
        public decimal Total
        {
            get
            {
                return _Total;
            }
            set
            {
                SetField<decimal>("Total", ref _Total, value);
            }
        }

        private decimal _TotalRate;

        [DataField(FieldName = "TotalRate"
            , MemberName = "_TotalRate"
            )]
        public decimal TotalRate
        {
            get
            {
                return _TotalRate;
            }
            set
            {
                SetField<decimal>("TotalRate", ref _TotalRate, value);
            }
        }

        private decimal _ExtendedTotal;

        [DataField(FieldName = "ExtendedTotal"
            , MemberName = "_ExtendedTotal"
            )]
        public decimal ExtendedTotal
        {
            get
            {
                return _ExtendedTotal;
            }
            set
            {
                SetField<decimal>("ExtendedTotal", ref _ExtendedTotal, value);
            }
        }

        private decimal _ExtendedTotalRate;

        [DataField(FieldName = "ExtendedTotalRate"
            , MemberName = "_ExtendedTotalRate"
            )]
        public decimal ExtendedTotalRate
        {
            get
            {
                return _ExtendedTotalRate;
            }
            set
            {
                SetField<decimal>("ExtendedTotalRate", ref _ExtendedTotalRate, value);
            }
        }

    }
}
