using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;
using Anito.Data.Attributes;

namespace Anito.Test.Windows.BusinessObjects
{
    [View("Entity")]
    [UpdateSource("Entity")]
    public partial class Entity: DataObject
    {
        [DataField("ID")]
        [Identity]
        private int _iD = default(System.Int32);
        
        
        public int ID 
        {
            get
            {
                return _iD;
            }
            set
            {
                SetField<int>("ID", ref _iD, value);
            }
        }

        [DataField("EntityCode")]
        [PrimaryKey]
        [ColumnSize(30)]
        private string _entityCode = null;
        
        
        public string EntityCode
        {
            get
            {
                return _entityCode;
            }
            set
            { 
                SetField<string>("EntityCode", ref _entityCode, value);
            }
        }

        [DataField("Name")]
        [ColumnSize(250)]
        private string _name = null;
        
        
        public string Name
        { 
            get
            { 
                return _name;
            }
            set
            { 
                SetField<string>("Name", ref _name, value);
            }
        }

        [DataField("EmailAddress")]
        private string _emailAddress = null;
        
        public string EmailAddress
        { 
            get
            { 
                return _emailAddress;   
            }
            set
            {
                SetField<string>("EmailAddress", ref _emailAddress, value);
            }
        
        }

        [DataField("ContactNo")]
        private string _contactNo = null;
        
        public string ContactNo 
        {        
            get
            { 
                return _contactNo;      
            }
            set
            {
                SetField<string>("ContactNo", ref _contactNo, value);
            }
        }

        [DataField("FaxNo")]
        private string _faxNo = null;
        
        public string FaxNo 
        {
            get
            { 
                return _faxNo;   
            }
            set
            {
                SetField<string>("FaxNo", ref _faxNo, value);
            }
        
        }

        [DataField("Address")]
        private string _address = null;
        
        public string Address 
        { 
            get
            { 
                return _address;   
            }
            set
            {
                SetField<string>("Address", ref _address, value);
            }
        }

        [DataField("City")]
        private string _city = null;
        
        public string City 
        { 
            get
            { 
                return _city;   
            }
            set
            {
                SetField<string>("City", ref _city, value);
            }
        }

        [DataField("State")]
        private string _state = null;
        
        public string State 
        {
            get
            { 
                return _state;    
            }
            set
            {
                SetField<string>("State", ref _state, value);
            }
        }

        [DataField("PostalCode")]
        private string _postalCode = null;
        
        public string PostalCode 
        { 
            get
            { 
                return _postalCode;   
            }
            set
            {
                SetField<string>("PostalCode", ref _postalCode, value);
            }
        }

        [DataField("Country")]
        private string _country = null;
        
        public string Country 
        {
            get
            { 
                return _country; 
            }
            set
            {
                SetField<string>("Country", ref _country, value);
            }
        }

        [DataField("Currency")]
        private string _currency = null;
        
        public string Currency
        {
            get
            {
                return _currency;
            }
            set
            {
                SetField<string>("Currency", ref _currency, value);
            }
        }

        [DataField("Balance")]
        private decimal _balance = default(decimal);
        
        public decimal Balance 
        {
            get
            { 
                return _balance;   
            }
            set
            {
                SetField<decimal>("Balance", ref _balance, value);
            }        

        }

        [DataField("BalanceRate")]
        private decimal _balanceRate = default(decimal);
        
        public decimal BalanceRate 
        {
            get
            { 
                return _balanceRate;    
            }
            set
            {
                SetField<decimal>("BalanceRate", ref _balanceRate, value);
            }
        }

        [DataField("IsActive")]
        private bool _isActive = default(bool);
        
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                SetField<Boolean>("IsActive", ref _isActive, value);
            }
        }

        public Entity(ISession session) :
            base(session)
        { }

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(this.GetType().FullName);
            sb.Append(_entityCode);
            return sb.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.EntityCode.Equals((obj as Entity).EntityCode);
        }
    }
}
