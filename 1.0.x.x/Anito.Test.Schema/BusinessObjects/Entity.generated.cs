using System;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{    
    [Source(View = "Entity", Update = "Entity")]
    public partial class Entity : DataObject
    {
        public Entity(ISession session) :
            base(session)
        {
        }
        public Entity()
        {
        }

        private int _ID;

        [DataField(FieldName = "ID"
            , MemberName = "_ID"
            , Identity = true // temporarily commented to test get methods of mongodb.. please uncomment when testing other providers
            )]
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                SetField <int>("ID", ref _ID, value);
            }
        }

        private string _EntityCode;

        [DataField(FieldName = "EntityCode"
            , MemberName = "_EntityCode"
            , PrimaryKey = true
            , Size = 50
            )]
        public string EntityCode
        {
            get
            {
                return _EntityCode;
            }
            set
            {
                SetField <string>("EntityCode", ref _EntityCode, value);
            }
        }

        private Guid? _Identifier;

        [DataField(FieldName = "Identifier"
            , MemberName = "_Identifier"
            )]
        public Guid? Identifier
        {
            get
            {
                return _Identifier;
            }
            set
            {
                SetField <Guid?>("Identifier", ref _Identifier, value);
            }
        }

        private string _Name;

        [DataField(FieldName = "Name"
            , MemberName = "_Name"
            , Size = 250
            )]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetField <string>("Name", ref _Name, value);
            }
        }

        private string _EmailAddress;

        [DataField(FieldName = "EmailAddress"
            , MemberName = "_EmailAddress"
            , Size = 250
            )]
        public string EmailAddress
        {
            get
            {
                return _EmailAddress;
            }
            set
            {
                SetField <string>("EmailAddress", ref _EmailAddress, value);
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
                SetField <string>("Type", ref _Type, value);
            }
        }

        private string _ContactNo;

        [DataField(FieldName = "ContactNo"
            , MemberName = "_ContactNo"
            , Size = 50
            )]
        public string ContactNo
        {
            get
            {
                return _ContactNo;
            }
            set
            {
                SetField <string>("ContactNo", ref _ContactNo, value);
            }
        }

        private string _FaxNo;

        [DataField(FieldName = "FaxNo"
            , MemberName = "_FaxNo"
            , Size = 50
            )]
        public string FaxNo
        {
            get
            {
                return _FaxNo;
            }
            set
            {
                SetField <string>("FaxNo", ref _FaxNo, value);
            }
        }

        private string _Address;

        [DataField(FieldName = "Address"
            , MemberName = "_Address"
            , Size = 250
            )]
        public string Address
        {
            get
            {
                return _Address;
            }
            set
            {
                SetField <string>("Address", ref _Address, value);
            }
        }

        private string _City;

        [DataField(FieldName = "City"
            , MemberName = "_City"
            , Size = 250
            )]
        public string City
        {
            get
            {
                return _City;
            }
            set
            {
                SetField <string>("City", ref _City, value);
            }
        }

        private string _State;

        [DataField(FieldName = "State"
            , MemberName = "_State"
            , Size = 250
            )]
        public string State
        {
            get
            {
                return _State;
            }
            set
            {
                SetField <string>("State", ref _State, value);
            }
        }

        private string _PostalCode;

        [DataField(FieldName = "PostalCode"
            , MemberName = "_PostalCode"
            , Size = 50
            )]
        public string PostalCode
        {
            get
            {
                return _PostalCode;
            }
            set
            {
                SetField <string>("PostalCode", ref _PostalCode, value);
            }
        }

        private string _Country;

        [DataField(FieldName = "Country"
            , MemberName = "_Country"
            , Size = 250
            )]
        public string Country
        {
            get
            {
                return _Country;
            }
            set
            {
                SetField <string>("Country", ref _Country, value);
            }
        }

        private string _Currency;

        [DataField(FieldName = "Currency"
            , MemberName = "_Currency"
            , Size = 50
            )]
        public string Currency
        {
            get
            {
                return _Currency;
            }
            set
            {
                SetField <string>("Currency", ref _Currency, value);
            }
        }

        private string _ProfileCode;
        
        [DataField(FieldName = "ProfileCode"
            , MemberName = "_ProfileCode"
            , Size = 30
            )]
        public string ProfileCode
        {
            get
            {
                return _ProfileCode;
            }
            set
            {
                SetField<string>("ProfileCode", ref _ProfileCode, value);
            }
        }        

        private decimal? _Balance;

        [DataField(FieldName = "Balance"
            , MemberName = "_Balance"
            )]
        public decimal? Balance
        {
            get
            {
                return _Balance;
            }
            set
            {
                SetField <decimal?>("Balance", ref _Balance, value);
            }
        }

        private decimal? _BalanceRate;

        [DataField(FieldName = "BalanceRate"
            , MemberName = "_BalanceRate"
            )]
        public decimal? BalanceRate
        {
            get
            {
                return _BalanceRate;
            }
            set
            {
                SetField <decimal?>("BalanceRate", ref _BalanceRate, value);
            }
        }

        private DateTime? _DateRegistered;

        [DataField(FieldName = "DateRegistered"
            , MemberName = "_DateRegistered"
            )]
        public DateTime? DateRegistered
        {
            get
            {
                return _DateRegistered;
            }
            set
            {
                SetField <DateTime?>("DateRegistered", ref _DateRegistered, value);
            }
        }

        private bool? _IsActive;

        [DataField(FieldName = "IsActive"
            , MemberName = "_IsActive"
            )]
        public bool? IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                SetField <bool?>("IsActive", ref _IsActive, value);
            }
        }

    }
}
