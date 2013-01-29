using System;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{
    public partial class Address : DataObject
    {
        public Address(ISession session) :
            base(session)
        {
        }
        public Address()
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

        private string _AddressCode;

        [DataField(FieldName = "AddressCode"
            , MemberName = "_AddressCode"
            , PrimaryKey = true
            , Size = 30
            )]
        public string AddressCode
        {
            get
            {
                return _AddressCode;
            }
            set
            {
                SetField<string>("AddressCode", ref _AddressCode, value);
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
                SetField<string>("Country", ref _Country, value);
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
                SetField<string>("State", ref _State, value);
            }
        }

        private string _PostalCode;

        [DataField(FieldName = "PostalCode"
            , MemberName = "_PostalCode"
            , Size = 30
            )]
        public string PostalCode
        {
            get
            {
                return _PostalCode;
            }
            set
            {
                SetField<string>("PostalCode", ref _PostalCode, value);
            }
        }

        private string _Address;

        [DataField(FieldName = "Address"
            , MemberName = "_Address"
            , Size = 250
            )]
        public string Address1
        {
            get
            {
                return _Address;
            }
            set
            {
                SetField<string>("Address", ref _Address, value);
            }
        }

        private string _Address2;

        [DataField(FieldName = "Address2"
            , MemberName = "_Address2"
            , Size = 250
            )]
        public string Address2
        {
            get
            {
                return _Address2;
            }
            set
            {
                SetField<string>("Address2", ref _Address2, value);
            }
        }
    }
}
