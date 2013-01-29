/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

namespace Anito.Data
{

    public class DataObjectRef<TEntity> : SessionContainer
        ,ISingleChild
        where TEntity : class, new()
    {
        private TEntity m_dataObject;


        public TEntity DataObject
        { 
            get
            {
                if(m_dataObject == null)
                    Load();
                return m_dataObject;
            }
            set
            {
                m_dataObject = value;
            }
        }

        internal DataObjectRef(ISession session)
           : base(session)
        {
             
        }

        protected override void Load()
        {
            if (DataSession == null || KeyValues.Count < 1)
                m_dataObject = null;
            else
            {
                m_dataObject = DataSession.GetChild<TEntity>(Relation, KeyValues.ToArray());
            }
        }

        object ISingleChild.Child
        {
            get
            {
                return DataObject;
            }
        }
    }
}
