using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{
    public partial class SalesOrder : DataObject
    {
        public SalesOrder(ISession session) :
            base(session)
        {
        }
        public SalesOrder()
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

        private string _EntityCode;

        [DataField(FieldName = "EntityCode"
            , MemberName = "_EntityCode"
            , Size = 30
            )]
        public string EntityCode
        {
            get
            {
                return _EntityCode;
            }
            set
            {
                SetField<string>("EntityCode", ref _EntityCode, value);
            }
        }

        private DateTime _OrderDate;

        [DataField(FieldName = "OrderDate"
            , MemberName = "_OrderDate"
            )]
        public DateTime OrderDate
        {
            get
            {
                return _OrderDate;
            }
            set
            {
                SetField<DateTime>("OrderDate", ref _OrderDate, value);
            }
        }

        private string _CouponCode;

        [DataField(FieldName = "CouponCode"
            , MemberName = "_CouponCode"
            , Size = 50
            )]
        public string CouponCode
        {
            get
            {
                return _CouponCode;
            }
            set
            {
                SetField<string>("CouponCode", ref _CouponCode, value);
            }
        }

        private string _TaxCode;

        [DataField(FieldName = "TaxCode"
            , MemberName = "_TaxCode"
            , Size = 50
            )]
        public string TaxCode
        {
            get
            {
                return _TaxCode;
            }
            set
            {
                SetField<string>("TaxCode", ref _TaxCode, value);
            }
        }

        private string _Currency;

        [DataField(FieldName = "Currency"
            , MemberName = "_Currency"
            , Size = 30
            )]
        public string Currency
        {
            get
            {
                return _Currency;
            }
            set
            {
                SetField<string>("Currency", ref _Currency, value);
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

        private decimal _Paid;

        [DataField(FieldName = "Paid"
            , MemberName = "_Paid"
            )]
        public decimal Paid
        {
            get
            {
                return _Paid;
            }
            set
            {
                SetField<decimal>("Paid", ref _Paid, value);
            }
        }

        private decimal _PaidRate;

        [DataField(FieldName = "PaidRate"
            , MemberName = "_PaidRate"
            )]
        public decimal PaidRate
        {
            get
            {
                return _PaidRate;
            }
            set
            {
                SetField<decimal>("PaidRate", ref _PaidRate, value);
            }
        }

        private decimal _Balance;

        [DataField(FieldName = "Balance"
            , MemberName = "_Balance"
            )]
        public decimal Balance
        {
            get
            {
                return _Balance;
            }
            set
            {
                SetField<decimal>("Balance", ref _Balance, value);
            }
        }

        private decimal _BalanceRate;

        [DataField(FieldName = "BalanceRate"
            , MemberName = "_BalanceRate"
            )]
        public decimal BalanceRate
        {
            get
            {
                return _BalanceRate;
            }
            set
            {
                SetField<decimal>("BalanceRate", ref _BalanceRate, value);
            }
        }

        private string _Status;

        [DataField(FieldName = "Status"
            , MemberName = "_Status"
            , Size = 60
            )]
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                SetField<string>("Status", ref _Status, value);
            }
        }
    }
}
