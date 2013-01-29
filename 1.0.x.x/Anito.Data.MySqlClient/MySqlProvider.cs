/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

namespace Anito.Data.MySqlClient
{
    public class MySqlProvider: ProviderBase
    {
        #region Variables
        private readonly Mapper m_mapper;
        private readonly CommandBuilder m_builder;
        private readonly CommandExecutor m_commandExecutor;
        #endregion

        #region Constants
        private const string PROVIDER_NAME = "Anito.SqlClient";
        #endregion

        #region Properties

        #region ConnectionString
        public override string ConnectionString
        {
            get
            {
                return m_commandExecutor.ConnectionString;
            }
            set
            {
                m_commandExecutor.ConnectionString = value;
            }
        }
        #endregion

        #region CommandBuilder
        public override ICommandBuilder CommandBuilder 
        {
            get
            {
                return m_builder;
            }
        }
        #endregion

        #region Mapper
        public override IMapper Mapper 
        {
            get
            {
                return m_mapper;
            }
        }
        #endregion

        #region CommandExecutor
        public override ICommandExecutor CommandExecutor
        {
            get
            {
                return m_commandExecutor;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Constructor
        public MySqlProvider()
            : base(PROVIDER_NAME)
        {
            m_mapper = new Mapper(this);
            m_builder = new CommandBuilder(this);
            m_commandExecutor = new CommandExecutor();
        }
        #endregion

        #endregion
    }
}
