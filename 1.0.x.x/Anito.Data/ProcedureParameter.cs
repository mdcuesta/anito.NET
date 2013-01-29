/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

namespace Anito.Data
{
    public class ProcedureParameter
    {
        private string m_name = string.Empty;
        private ParameterDirection m_direction = ParameterDirection.Input;

        public string Name 
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        
        }

        public ParameterType Type { get; set; }

        public object Value { get; set; }

        public int Size { get; set; }

        public ParameterDirection Direction
        {
            get
            {
                return m_direction;
            }
            set
            {
                m_direction = value;
            }
        }

        public ProcedureParameter(string name)
        {
            Name = name;
        }

        public ProcedureParameter(string name, ParameterType type)
        {
            Name = name;
            Type = type;
        }

        public ProcedureParameter(string name, ParameterType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public ProcedureParameter(string name, ParameterType type, object value, int size)
        {
            Name = name;
            Type = type;
            Value = value;
            Size = size;
        }

        public ProcedureParameter(string name, ParameterType type, ParameterDirection direction)
        {
            Name = name;
            Type = type;
            Direction = direction;
        }

        public ProcedureParameter(string name, ParameterType type, ParameterDirection direction, object value)
        {
            Name = name;
            Type = type;
            Direction = direction;
            Value = value;
        }

        public ProcedureParameter(string name, ParameterType type, ParameterDirection direction, object value, int size)
        {
            Name = name;
            Type = type;
            Direction = direction;
            Value = value;
            Size = size;
        }
    }
}
