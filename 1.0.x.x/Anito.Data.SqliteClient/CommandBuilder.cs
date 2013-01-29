/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.SQLite;
using Anito.Data.Exceptions;
using Anito.Data.Schema;
using Anito.Data.Util;
using Anito.Data.Common;

namespace Anito.Data.SqliteClient
{
    public class CommandBuilder : AdoCommandBuilderBase 
    {
        #region Variables
        private static Dictionary<string, Command> s_commandCache;
        private static Dictionary<Type, string> s_columnSelectCache;

        private const string SELECT = "SELECT";
        private const string FROM = "FROM";
        private const string WHERE = "WHERE";
        private const string INSERT = "INSERT";
        private const string INTO = "INTO";
        private const string VALUES = "VALUES";
        private const string UPDATE = "UPDATE";
        private const string DELETE = "DELETE";
        private const string SET = "SET";
        private const string COUNT = "COUNT";
        private const string AND = "AND";
        private const string CACHE_KEY = "{0}_{1}";
        private const string OPEN_PARENTHESES = "(";
        private const string CLOSE_PARENTHESES = ")";
        private const string SEMI_COLON = ";";
        private const string SHARP = "#";
        private const string DOT = ".";
        private const string PARAM_IDENTIFIER = "@";
        private const string PARAM_IDENTIFIER_ORIGINAL = "@Original_";
        private const string SPACE = " ";
        private const string COMMA = ",";
        private const string EQUALS = "=";
        private const string GET_TABLE_BY_PAGE = @"
            SELECT
                {0}
            FROM
                {1}
            LIMIT @StartRow, @EndRow 
        ";

        private const string GET_TABLE_BY_PAGE_WHERE = @"
            SELECT
                {0}
            FROM
                {1}
            WHERE
                {2}
            LIMIT @StartRow, @EndRow 
        ";

        #endregion

        #region Properties

        #region CommandCache
        private static Dictionary<string, Command> CommandCache
        {
            get
            {
                s_commandCache = s_commandCache ?? new Dictionary<string, Command>();
                return s_commandCache;
            }
        }
        #endregion

        #region ColumnSelectCache
        private static Dictionary<Type, string> ColumnSelectCache
        {
            get
            {
                s_columnSelectCache = s_columnSelectCache ?? new Dictionary<Type, string>();
                return s_columnSelectCache;
            }
        }
        #endregion

        #endregion

        #region Constructor
        public CommandBuilder(IProvider provider)
            : base(provider)
        {
        }
        #endregion

        #region Methods

        #region CreateCommands

        #region CreateGetTCommand
        public override ICommand CreateGetTCommand<T>(Expression expression)
        {
            var typeT = typeof(T);
            var schemaTable = Provider.GetSchema(typeT);

            var sqlCommand = new Command();

            var translator = new Translator(Provider);
            var whereText = translator.Translate(expression);

            sqlCommand.SqlCommand.CommandText = sqlCommand.SqlCommand.CommandText =
                    string.Format("{0} {1} {2} {3} {4} {5}",
                    SELECT, SelectColumnsStatement(typeT), FROM, schemaTable.ViewSource, WHERE, whereText);

            return sqlCommand;
        }
        #endregion

        #region CreateGetObjectByKeyCommand
        public override ICommand CreateGetObjectByKeyCommand<T>()
        {
            var typeT = typeof(T);
            const string uniqueId = "4F9C6FBE-2971-46A8-A367-B28AB5F65895";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, typeT.FullName)))
            {
                var schemaTable = Provider.GetSchema(typeT);
                var sqlCommand = new Command();

                if ((from column in schemaTable where column.IsPrimaryKey select column).Count() < 1)
                    throw new TypeNoKeyException(typeof(T));

                var paramBuilder = new StringBuilder();
                foreach (var column in (from column in schemaTable where column.IsPrimaryKey select column))
                {
                    if (paramBuilder.Length > 0)
                        paramBuilder.Append(string.Format("{0}{1}{2}", SPACE, AND, SPACE));
                    paramBuilder.Append(string.Format("{0} = {1}{2}", column.Name, PARAM_IDENTIFIER, column.Name));
                    sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name));
                }

                sqlCommand.SqlCommand.CommandText =
                    string.Format("{0} {1} {2} {3} {4} {5}",
                    SELECT, SelectColumnsStatement(typeT), FROM, schemaTable.ViewSource, WHERE, paramBuilder);
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }
        #endregion

        #region CreateGetListCommand
        public override ICommand CreateGetListCommand<T>()
        {
            var typeT = typeof(T);
            const string uniqueId = "232FDED0-6C97-40B4-930F-FC190124D181";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId , typeT.FullName)))
            {
                var schemaTable = Provider.GetSchema(typeT);

                var sqlCommand = new Command();

                sqlCommand.SqlCommand.CommandText = string.Format(
                    "{0} {1} {2} {3}",
                    SELECT, SelectColumnsStatement(typeT), FROM, schemaTable.ViewSource
                    );
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }

        public override ICommand CreateGetListCommand<T>(IFilterCriteria criteria)
        {
            var typeT = typeof(T);
            const string uniqueId = "232FDED0-6C97-40B4-930F-FC190124D181";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, typeT.FullName)))
            {
                var schemaTable = Provider.GetSchema(typeT);

                var sqlCommand = new Command();

                sqlCommand.SqlCommand.CommandText = string.Format(
                    "{0} {1} {2} {3}",
                    SELECT, SelectColumnsStatement(typeT), FROM, schemaTable.ViewSource
                    );
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }

        public override ICommand CreateGetListCommand<T>(Expression expression)
        {
            var typeT = typeof(T);            
            var schemaTable = Provider.GetSchema(typeT);

            var translator = new Translator(Provider);
            string whereText = translator.Translate(expression);

            var sqlCommand = new Command();

            sqlCommand.SqlCommand.CommandText = string.Format(
                "{0} {1} {2} {3} {4} {5}",
                SELECT, SelectColumnsStatement(typeT), FROM, schemaTable.ViewSource, WHERE, whereText
                );
            return sqlCommand;
        }
        #endregion

        #region CreateGetListByPageCommand
        public override ICommand CreateGetListByPageCommand<T>()
        {  
            var typeT = typeof(T);
            const string uniqueId = "D89646AE-F93B-4D59-9A0C-7AF5A6B79F5D";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, typeT.FullName)))
            {
                var schemaTable = Provider.GetSchema(typeT);

                var sqlCommand = new Command();

                var tempTable = SHARP + "D89646AE" + schemaTable.ViewSource;

                var columnBuilder = new StringBuilder();
                var keyBuilder = new StringBuilder();
                var keyCompareBuilder = new StringBuilder();
                
                foreach (var column in schemaTable)
                {
                    if (columnBuilder.Length > 0)
                        columnBuilder.Append(COMMA + SPACE);
                    columnBuilder.Append(schemaTable.ViewSource + DOT + column.Name);

                    if (!column.IsPrimaryKey)
                        continue;


                    if (keyBuilder.Length > 0)
                        keyBuilder.Append(COMMA + SPACE);
                    keyBuilder.Append(column.Name);

                    if (keyCompareBuilder.Length > 0)
                        keyCompareBuilder.Append(SPACE + AND + SPACE);
                    keyCompareBuilder.Append(tempTable + DOT + column.Name);
                    keyCompareBuilder.Append(SPACE + EQUALS + SPACE);
                    keyCompareBuilder.Append(schemaTable.ViewSource + DOT + column.Name);
                }

                sqlCommand.SqlCommand.CommandText = string.Format(GET_TABLE_BY_PAGE, 
                    columnBuilder, schemaTable.ViewSource);

                sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER, "StartRow"));
                sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER, "EndRow"));

                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }

        public override ICommand CreateGetListByPageCommand<T>(Expression expression)
        {
            var typeT = typeof(T);
            var schemaTable = Provider.GetSchema(typeT);

            var sqlCommand = new Command();

            var tempTable = SHARP + "D89646AM" + schemaTable.ViewSource;

            var columnBuilder = new StringBuilder();
            var keyBuilder = new StringBuilder();
            var keyCompareBuilder = new StringBuilder();

            foreach (var column in schemaTable)
            {
                if (columnBuilder.Length > 0)
                    columnBuilder.Append(COMMA + SPACE);
                columnBuilder.Append(schemaTable.ViewSource + DOT + column.Name);

                if (!column.IsPrimaryKey)
                    continue;

                if (keyBuilder.Length > 0)
                    keyBuilder.Append(COMMA + SPACE);
                keyBuilder.Append(column.Name);

                if (keyCompareBuilder.Length > 0)
                    keyCompareBuilder.Append(SPACE + AND + SPACE);
                keyCompareBuilder.Append(tempTable + DOT + column.Name);
                keyCompareBuilder.Append(SPACE + EQUALS + SPACE);
                keyCompareBuilder.Append(schemaTable.ViewSource + DOT + column.Name);
            }

            var translator = new Translator(Provider);
            var whereStatement = translator.Translate(expression);

            sqlCommand.SqlCommand.CommandText = string.Format(GET_TABLE_BY_PAGE_WHERE,
                columnBuilder, schemaTable.ViewSource, whereStatement);

            sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER, "StartRow"));
            sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER, "EndRow"));

            return sqlCommand;
        }
        #endregion

        #region CreateGetChildrenCommand
        public override ICommand CreateGetChildrenCommand<T>(TypeRelation relation, params object[] referenceValue)
        {
            var typeT = typeof(T);

            var schemaTable = Provider.GetSchema(typeT);

            var whereText = string.Empty;

            var idx = 0;
            foreach (var o in referenceValue)
            {
                var column = relation.SourceColumns[idx];
                var colName = schemaTable.GetDbColumn(column.ColumnHolder);

                string val;
                if (Equals(o, null))
                    val = "NULL";
                else if (o is DateTime)
                    val = string.Format("'{0}'", ((DateTime)o).ToString("MM/dd/yyyy H:mm:ss.fff", CultureInfo.InvariantCulture));
                else
                    val = (Misc.IsNumericType(o.GetType()))
                       ? Convert.ToString(o, CultureInfo.InvariantCulture)
                       : string.Format("'{0}'", o);

                var equation = string.Format("{0} = {1}", colName, val);

                whereText +=
                    string.IsNullOrEmpty(whereText)
                    ? equation
                    : string.Format(" AND {0}", equation);

                idx++;
            }

            var sqlCommand = new Command();

            sqlCommand.SqlCommand.CommandText = string.Format(
                "{0} {1} {2} {3} {4} {5}",
                SELECT, SelectColumnsStatement(typeT), FROM, schemaTable.ViewSource, WHERE, whereText
                );
            return sqlCommand;
        }
        #endregion

        #region CreateCommandFromProcedure
        public override ICommand CreateCommandFromProcedure(Procedure procedure)
        {          
            var command = new SQLiteCommand(procedure.ProcedureName){CommandType = CommandType.StoredProcedure};
            
            foreach (var procedureParam in procedure.Parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = PARAM_IDENTIFIER + procedureParam.Name;
                parameter.Direction = GetParameterDirection(procedureParam.Direction);
                parameter.Value = procedureParam.Value;
                parameter.DbType = GetSqlDbType(procedureParam.Type);
                parameter.Size = procedureParam.Size;
                command.Parameters.Add(parameter);
            }

            var sqlCommand = new Command(command);
            return sqlCommand;
        }
        #endregion

        #region CreateExecuteScalarCommand
        public override ICommand CreateExecuteScalarCommand<T>(Expression column, Expression expression)
        {
            var typeT = typeof(T);
            var schemaTable = Provider.GetSchema(typeT);

            var sqlCommand = new Command();

            var translator = new Translator(Provider);
            var columnName = translator.Translate(column);
            var whereText = translator.Translate(expression);

            sqlCommand.SqlCommand.CommandText = sqlCommand.SqlCommand.CommandText =
                    string.Format("{0} {1} {2} {3} {4} {5}",
                    SELECT, columnName, FROM, schemaTable.ViewSource, WHERE, whereText);

            return sqlCommand;
        }
        #endregion

        #region Read
        public override ICommand CreateReadCommand<T>(Expression expression)
        {
            var translator = new Translator(Provider);
            var commandText = translator.Translate(expression);
            var command = new SQLiteCommand(commandText);
            return new Command(command);
        }
        #endregion

        #region Insert

        #region CreateInsertCommand
        public override ICommand CreateInsertCommand(object data)
        {
            var typeT = data.GetType();
            const string uniqueId = "26ABBE10-3D2A-40be-ABAC-F92EF9040608";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, typeT.FullName)))
            {
                var schemaTable = Provider.GetSchema(typeT);

                var sqlCommand = new Command();

                var commandTextBuilder = new StringBuilder();
                var paramBuilder = new StringBuilder();
                var columnBuilder = new StringBuilder();

                TypeColumn identityColumn = null;

                foreach (var column in (from col in schemaTable where !col.ViewOnly select col))
                {
                    if (column.IsIdentity)
                    {
                        identityColumn = column;
                        continue;
                    }

                    if (sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name)))
                        continue;

                    if (columnBuilder.Length > 0 && paramBuilder.Length > 0)
                    {
                        columnBuilder.Append(COMMA);
                        paramBuilder.Append(COMMA);
                    }
                    columnBuilder.Append(column.Name);
                    paramBuilder.Append(PARAM_IDENTIFIER + column.Name);
                    if(!sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name)))
                        sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name));
                }
                commandTextBuilder.Append(string.Format("{0} {1} {2}",
                    INSERT, INTO, schemaTable.UpdateSource));
                commandTextBuilder.Append(OPEN_PARENTHESES);
                commandTextBuilder.Append(columnBuilder.ToString());
                commandTextBuilder.Append(CLOSE_PARENTHESES);
                commandTextBuilder.Append(SPACE);
                commandTextBuilder.Append(VALUES + OPEN_PARENTHESES);
                commandTextBuilder.Append(paramBuilder.ToString());
                commandTextBuilder.Append(CLOSE_PARENTHESES + SEMI_COLON);

                if (identityColumn != null)
                    commandTextBuilder.Append(string.Format("SELECT {0} FROM {1} WHERE {2} = (SELECT LAST_INSERT_ROWID());"
                        , schemaTable.ColumnList
                        , schemaTable.ViewSource
                        , identityColumn.Name));

                sqlCommand.SqlCommand.CommandText = commandTextBuilder.ToString();
                
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }
        #endregion

        #endregion

        #region Update

        #region CreateUpdateCommand
        public override ICommand CreateUpdateCommand(object data)
        {
            var typeT = data.GetType();
            var schemaTable = Provider.GetSchema(typeT);

            if (schemaTable.HasIdentity)
                return CreateUpdateByIdCommand(data, typeT);
            return (schemaTable.HasKey)
                ? CreateUpdateByKeyCommand(data, typeT)
                : null;
        }

        public override ICommand CreateUpdateCommand<T>(Expression expression)
        {
            var typeT = typeof(T);

            var schemaTable = Provider.GetSchema(typeT);

            var sqlCommand = new Command();

            var commandTextBuilder = new StringBuilder();
            var paramBuilder = new StringBuilder();

            foreach (var column in 
                (from col in schemaTable 
                 where !col.ViewOnly && !col.IsIdentity
                 && !sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER, col.Name))
                 select col))
            {
                if (paramBuilder.Length > 0)
                    paramBuilder.Append(COMMA + SPACE);
                paramBuilder.Append(column.Name + SPACE + EQUALS + SPACE);
                paramBuilder.Append(PARAM_IDENTIFIER + column.Name);
                if (!sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name)))
                    sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name));
            }

            var translator = new Translator(Provider);
            var whereClause = translator.Translate(expression);

            commandTextBuilder.Append(string.Format("{0} {1} {2} ",
                UPDATE, schemaTable.UpdateSource, SET
                )
                );
            commandTextBuilder.Append(paramBuilder.ToString());
            commandTextBuilder.Append(SPACE + WHERE + SPACE);
            commandTextBuilder.Append(whereClause);

            sqlCommand.SqlCommand.CommandText = commandTextBuilder.ToString();
            return sqlCommand;
        }
        #endregion

        #region CreateUpdateByKeyCommand
        public override ICommand CreateUpdateByKeyCommand(object data)
        {
            var typeT = data.GetType();
            const string uniqueId = "34FD7357-8114-4f36-825E-77C3F819B39F";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, typeT.FullName)))
            {
                var sqlCommand = _CreateUpdateByKeyCommand(typeT);
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }

        public override ICommand CreateUpdateByKeyCommand(object data, Type type)
        {
            const string uniqueId = "34FD7357-8114-4f36-825E-77C3F819B39F";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, type.FullName)))
            {
                var sqlCommand = _CreateUpdateByKeyCommand(type);
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, type.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, type.FullName)];
        }

        private Command _CreateUpdateByKeyCommand(Type type)
        {
            var schemaTable = Provider.GetSchema(type);

            var sqlCommand = new Command();

            var commandTextBuilder = new StringBuilder();
            var paramBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();

            foreach (var column in 
                (from col in schemaTable 
                 where !col.ViewOnly
                 && !(col.IsIdentity || sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER, col.Name)))
                 select col))
            {
                if (paramBuilder.Length > 0)
                    paramBuilder.Append(COMMA + SPACE);
                paramBuilder.Append(column.Name + SPACE + EQUALS + SPACE);
                paramBuilder.Append(PARAM_IDENTIFIER + column.Name);
                if (!sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name)))
                    sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name));

                if (column.IsPrimaryKey)
                {
                    if (whereBuilder.Length > 0)
                        whereBuilder.Append(SPACE + AND + SPACE);
                    whereBuilder.Append(column.Name);
                    whereBuilder.Append(SPACE);
                    whereBuilder.Append(EQUALS);
                    whereBuilder.Append(SPACE);
                    whereBuilder.Append(PARAM_IDENTIFIER_ORIGINAL + column.Name);
                    if (!sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER_ORIGINAL, column.Name)))
                        sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER_ORIGINAL, column.Name));
                }
            }
            commandTextBuilder.Append(string.Format("{0} {1} {2}",
                UPDATE, schemaTable.UpdateSource, SET));
            commandTextBuilder.Append(SPACE);
            commandTextBuilder.Append(paramBuilder.ToString());
            commandTextBuilder.Append(SPACE);
            commandTextBuilder.Append(WHERE);
            commandTextBuilder.Append(SPACE);
            commandTextBuilder.Append(whereBuilder.ToString());
            commandTextBuilder.Append(SEMI_COLON);

            sqlCommand.SqlCommand.CommandText = commandTextBuilder.ToString();
            return sqlCommand;
        }
        #endregion

        #region CreateUpdateByIdCommand
        public override ICommand CreateUpdateByIdCommand(object data)
        { 
            var typeT = data.GetType();
            const string uniqueId = "80065C07-24CD-40E8-9642-A46085892480";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, typeT.FullName)))
            {
                var sqlCommand = _CreateUpdateByIdCommand(typeT);
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }

        public override ICommand CreateUpdateByIdCommand(object data, Type type)
        {
            const string uniqueId = "80065C07-24CD-40E8-9642-A46085892480";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, type.FullName)))
            {
                var sqlCommand = _CreateUpdateByIdCommand(type);
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, type.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, type.FullName)];
        }
        private Command _CreateUpdateByIdCommand(Type type)
        {
            var schemaTable = Provider.GetSchema(type);

            var sqlCommand = new Command();

            var commandTextBuilder = new StringBuilder();
            var paramBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();

            foreach (var column in 
                (from col in schemaTable 
                 where !col.ViewOnly
                 && !sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER, col.Name))
                 select col))
            {

                if (!column.IsIdentity)
                {
                    if (paramBuilder.Length > 0)
                        paramBuilder.Append(COMMA + SPACE);
                    paramBuilder.Append(column.Name + SPACE + EQUALS + SPACE);
                    paramBuilder.Append(PARAM_IDENTIFIER + column.Name);
                    if (!sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name)))
                        sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER, column.Name));
                }
                else if (column.IsIdentity)
                {
                    if (whereBuilder.Length > 0)
                        whereBuilder.Append(SPACE + AND + SPACE);
                    whereBuilder.Append(column.Name);
                    whereBuilder.Append(SPACE);
                    whereBuilder.Append(EQUALS);
                    whereBuilder.Append(SPACE);
                    whereBuilder.Append(PARAM_IDENTIFIER_ORIGINAL + column.Name);
                    if (!sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER_ORIGINAL, column.Name)))
                        sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER_ORIGINAL, column.Name));
                }
            }
            commandTextBuilder.Append(string.Format("{0} {1} {2}",
                UPDATE, schemaTable.UpdateSource, SET));
            commandTextBuilder.Append(SPACE);
            commandTextBuilder.Append(paramBuilder.ToString());
            commandTextBuilder.Append(SPACE);
            commandTextBuilder.Append(WHERE);
            commandTextBuilder.Append(SPACE);
            commandTextBuilder.Append(whereBuilder.ToString());
            commandTextBuilder.Append(SEMI_COLON);

            sqlCommand.SqlCommand.CommandText = commandTextBuilder.ToString();
            return sqlCommand;
        }
        #endregion

        #endregion

        #region Delete

        #region CreateDeleteCommand
        public override ICommand CreateDeleteCommand(object data)
        {
            Type typeT = data.GetType();
            TypeTable schemaTable = Provider.GetSchema(typeT);

            if (schemaTable.HasIdentity)
                return CreateDeleteByIdCommand(data, typeT);
            return (schemaTable.HasKey)
                ? CreateDeleteByKeyCommand(data, typeT)
                : null;
        }

        public override ICommand CreateDeleteCommand<T>(Expression expression)
        {
            var typeT = typeof(T);
            var schemaTable = Provider.GetSchema(typeT);
            var translator = new Translator(Provider);
            var whereClause = translator.Translate(expression);
            var commandText = string.Format("{0} {1} {2} {3} {4}",
                DELETE,
                FROM,
                schemaTable.UpdateSource,
                WHERE,
                whereClause);
            var sqlCommand = new SQLiteCommand(commandText);
            return new Command(sqlCommand);
        }
        #endregion

        #region CreateDeleteByIdCommand
        public override ICommand CreateDeleteByIdCommand(object data)
        {
            var typeT = data.GetType();
            const string uniqueId = "F58B3FAE-6E43-4B39-9850-8E2D4EC63419";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, typeT.FullName)))
            {
                var schemaTable = Provider.GetSchema(typeT);
                var sqlCommand = _CreateDeletebyIdCommand(schemaTable);
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }

        public override ICommand CreateDeleteByIdCommand(object data, Type type)
        {
            const string uniqueId = "F58B3FAE-6E43-4B39-9850-8E2D4EC63419";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, type.FullName)))
            {
                var schemaTable = Provider.GetSchema(type);

                var sqlCommand = _CreateDeletebyIdCommand(schemaTable);
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, type.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, type.FullName)];
        }

        private Command _CreateDeletebyIdCommand(TypeTable schemaTable)
        {
            var sqlCommand = new Command();
            var commandTextBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();

            foreach (var column in (from col in schemaTable where col.IsIdentity && !col.ViewOnly select col))
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(SPACE + AND + SPACE);
                whereBuilder.Append(column.Name);
                whereBuilder.Append(SPACE);
                whereBuilder.Append(EQUALS);
                whereBuilder.Append(SPACE);
                whereBuilder.Append(PARAM_IDENTIFIER_ORIGINAL + column.Name);
                if (!sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER_ORIGINAL, column.Name)))
                    sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER_ORIGINAL, column.Name));
            }

            commandTextBuilder.Append(string.Format("{0} {1} {2}",
                DELETE, schemaTable.UpdateSource, WHERE));
            commandTextBuilder.Append(SPACE);
            commandTextBuilder.Append(whereBuilder.ToString());
            commandTextBuilder.Append(SEMI_COLON);

            sqlCommand.SqlCommand.CommandText = commandTextBuilder.ToString();
            return sqlCommand;
        }
        #endregion

        #region CreateDeleteByKeyCommand
        public override ICommand CreateDeleteByKeyCommand(object data)
        {
            var typeT = data.GetType();
            const string uniqueId = "92C92A43-D467-4931-8C12-F26AA936E7D7";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, typeT.FullName)))
            {
                var schemaTable = Provider.GetSchema(typeT);

                Command sqlCommand = _CreateDeletebyKeyCommand(schemaTable);
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }

        public override ICommand CreateDeleteByKeyCommand(object data, Type type)
        {
            const string uniqueId = "92C92A43-D467-4931-8C12-F26AA936E7D7";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, type.FullName)))
            {
                var schemaTable = Provider.GetSchema(type);

                var sqlCommand = _CreateDeletebyKeyCommand(schemaTable);
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, type.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, type.FullName)];
        }

        private Command _CreateDeletebyKeyCommand(TypeTable schemaTable)
        {
            var sqlCommand = new Command();
            var commandTextBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();

            foreach (var column in (from col in schemaTable where col.IsPrimaryKey && !col.ViewOnly select col))
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(SPACE + AND + SPACE);
                whereBuilder.Append(column.Name);
                whereBuilder.Append(SPACE);
                whereBuilder.Append(EQUALS);
                whereBuilder.Append(SPACE);
                whereBuilder.Append(PARAM_IDENTIFIER_ORIGINAL + column.Name);
                if (!sqlCommand.Parameters.ContainsKey(string.Format("{0}{1}", PARAM_IDENTIFIER_ORIGINAL, column.Name)))
                    sqlCommand.AddParam(string.Format("{0}{1}", PARAM_IDENTIFIER_ORIGINAL, column.Name));
            }

            commandTextBuilder.Append(string.Format("{0} {1} {2}",
                DELETE, schemaTable.UpdateSource, WHERE));
            commandTextBuilder.Append(SPACE);
            commandTextBuilder.Append(whereBuilder.ToString());
            commandTextBuilder.Append(SEMI_COLON);

            sqlCommand.SqlCommand.CommandText = commandTextBuilder.ToString();
            return sqlCommand;
        }
        #endregion

        #endregion

        #region Count
        public override ICommand CreateCountCommand(string source)
        {            
            var sqlCommand = new Command();
            sqlCommand.SqlCommand.CommandText = string.Format("{0} {1}(1) {2} {3}", SELECT, COUNT, FROM, source);
            return sqlCommand;
        }

        public override ICommand CreateCountCommand<T>()
        {
            var typeT = typeof(T);
            const string uniqueId = "1801F51F-A4EC-4A78-8AE8-BCB30402E4B5";
            if (!CommandCache.ContainsKey(string.Format(CACHE_KEY, uniqueId, typeT.FullName)))
            {
                var schemaTable = Provider.GetSchema(typeT);
                var sqlCommand = (Command) CreateCountCommand(schemaTable.ViewSource);                
                CommandCache.Add(string.Format(CACHE_KEY, uniqueId, typeT.FullName), sqlCommand);
            }
            return CommandCache[string.Format(CACHE_KEY, uniqueId, typeT.FullName)];
        }

        public override ICommand CreateCountCommand<T>(Expression expression)
        {
            var translator = new Translator(Provider);
            var typeT = typeof(T);
            
            var schemaTable = Provider.GetSchema(typeT);

            string commandText = string.Format("{0} {1}(1) {2} {3} {4} {5}", SELECT, 
                COUNT,
                FROM,
                schemaTable.ViewSource,
                WHERE,
                translator.Translate(expression));
            var command = new SQLiteCommand(commandText);
            return new Command(command);
        }
        #endregion

        #endregion

        #region SupplyParameters

        #region SupplyGetListByPageCommandParameters
        public override void SupplyGetListByPageCommandParameters(ref ICommand command, int page, int pageSize)
        {
            page = (page * pageSize) - pageSize;
            command.Parameters["@StartRow"].Value = page;
            command.Parameters["@EndRow"].Value = pageSize;
        }
        #endregion
        

        #endregion

        #region Misc

        private string SelectColumnsStatement(Type type)
        {
            if (!ColumnSelectCache.ContainsKey(type))
            {
                var schemaTable = Provider.GetSchema(type);
                var columnSb = new StringBuilder();
                foreach (var column in schemaTable)
                {
                    if (columnSb.Length > 0) columnSb.Append(", ");

                    if (column.IsIdentity)
                    {
                        columnSb.Append(string.Format("{0}.{1} AS {2}", schemaTable.ViewSource, column.Name, column.Name));
                    }
                    else
                        switch (column.Type.Name.ToUpper())
                        {
                            case "INT16":
                            case "INT32":
                            case "INT64":
                            case "SINGLE":
                            case "DATETIME":
                            case "DECIMAL":
                            case "DOUBLE":
                            case "BOOLEAN":
                                columnSb.Append(string.Format("IFNULL({0}.{1},{2}) AS {3}", schemaTable.ViewSource, column.Name, 0, column.Name));
                                break;
                            case "STRING":
                            case "BYTE[]":
                            case "BYTE":
                                columnSb.Append(string.Format("{0}.{1} AS {2}", schemaTable.ViewSource, column.Name, column.Name));
                                break;
                            case "GUID":
                                columnSb.Append(string.Format("IFNULL({0}.{1},'{2}') AS {3}", schemaTable.ViewSource, column.Name, default(Guid), column.Name));
                                break;
                        }
                }
                ColumnSelectCache.Add(type, columnSb.ToString());
            }
            return ColumnSelectCache[type];
        }  

        #endregion

        #endregion
    }
}
