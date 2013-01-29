/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using Anito.Data.Schema;

namespace Anito.Data
{
    public abstract class SessionContainer : ISessionContainer
    {
        private ISession m_session;
        private List<object> m_keyValues;
 
        internal SessionContainer(ISession session)
        {
            m_session = session;
        }

        internal SessionContainer()
        {
            
        }

        ISession ISessionContainer.DataSession
        {
            get
            {
                return m_session;
            }
            set { m_session = value; }
        }

        protected virtual ISession DataSession
        {
            get
            {
                return m_session;
            }
        }

        protected virtual List<object> KeyValues
        {
            get 
            { 
                m_keyValues = m_keyValues ?? new List<object>();
                return m_keyValues;
            }
        }

        public TypeRelation Relation { get; set; }

        public void AddReferenceValue(object value)
        {
            KeyValues.Add(value);
        }

        protected abstract void Load();
    }
}
