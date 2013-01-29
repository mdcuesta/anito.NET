using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;

namespace Anito.Test.Schema
{
    public class StoredProcedure
    {

        public static Procedure GetEntityById(int? ID)
        {
            Procedure procedure = new Procedure("GetEntityById");
            procedure.AddParameter("ID", ParameterType.Int32, 0, ID);
            procedure.AddParameter("EntityCode", ParameterType.String, ParameterDirection.Output, null, 30);
            return procedure;
        }

        public static Procedure GetEntityByEntityCode(string EntityCode)
        {
            Procedure procedure = new Procedure("GetEntityByEntityCode");
            procedure.AddParameter("EntityCode", ParameterType.String, 30, EntityCode);
            return procedure;
        }

        public static Procedure GetEntityByState(string State)
        {
            Procedure procedure = new Procedure("GetEntityByState");
            procedure.AddParameter("State", ParameterType.String, 30, State);
            return procedure;
        }
    }
}
