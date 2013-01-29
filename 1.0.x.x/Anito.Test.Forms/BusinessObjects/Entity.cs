using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anito.Test.Windows.BusinessObjects
{
    public partial class Entity
    {
        #region UnboundFields
        [Anito.Data.Attributes.DataField("EntityCode")]
        public string TestField { get; set; }
        #endregion

        #region Constructor
        public Entity()
        {
            OnFieldValueChanging += new Anito.Data.Events.FieldValueChangingDelegate(Entity_OnFieldValueChanging);
        }
        #endregion

        #region Events
        void Entity_OnFieldValueChanging(object sender, Anito.Data.Events.FieldValueChangingEventArgs args)
        {
            if (args.FieldName == "Name")
            {
                args.NewValue = "Arwen";
            }
        }
        #endregion

    }
}
