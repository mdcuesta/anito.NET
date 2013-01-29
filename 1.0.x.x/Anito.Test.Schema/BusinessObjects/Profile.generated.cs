using System;
using Anito.Data;
using Anito.Data.Mapping;

namespace Anito.Test.Schema.BusinessObjects
{
    public partial class Profile : DataObject
    {
        public Profile(ISession session) :
            base(session)
        {
        }
        public Profile()
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

        private string _ProfileCode;

        [DataField(FieldName = "ProfileCode"
            , MemberName = "_ProfileCode"
            , PrimaryKey = true
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

        private string _ProfileName;

        [DataField(FieldName = "ProfileName"
            , MemberName = "_ProfileName"
            , Size = 250
            )]
        public string ProfileName
        {
            get
            {
                return _ProfileName;
            }
            set
            {
                SetField<string>("ProfileName", ref _ProfileName, value);
            }
        }

        private string _AddressCode;

        [DataField(FieldName = "AddressCode"
            , MemberName = "_AddressCode"
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
    }
}
