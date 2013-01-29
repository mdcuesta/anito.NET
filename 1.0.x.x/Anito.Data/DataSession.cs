/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Collections.Generic;
using Anito.Data.Query;
using Anito.Data.Exceptions;
using System.Collections;
using System.Linq.Expressions;
using System.Data.Common;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Permissions;
using Anito.Data.Schema;
using Anito.Data.Mapping;

namespace Anito.Data
{

    [type : ReflectionPermission(SecurityAction.Assert)]
    public class DataSession : ISession
    {
        #region Variables

        private ITransaction CurrentTransaction { get; set; }

        #endregion

        #region Properties

        #region Provider

        public IProvider Provider { get; private set; }

        #endregion

        #region IsTransactionInitiated

        public bool IsTransactionInitiated { get; private set; }

        #endregion

        #endregion

        #region Methods

        #region Constructor
        public DataSession(IProvider provider)
        {
            Provider = provider;
        }
        #endregion

        #region Save

        /// <summary>
        /// Saves the item to the database
        /// </summary>
        /// <param name="item">The item to save</param>        
        public void Save(object item)
        {
            if(Equals(item, null))
                throw new ArgumentNullException("item");

            var transaction = IsTransactionInitiated 
                ? CurrentTransaction
                : Provider.CommandExecutor.InitiateTransaction();

            try
            {
                Save(item, transaction);
                if (!IsTransactionInitiated) 
                    Provider.CommandExecutor.CommitTransaction(transaction);
            }
            catch
            {
                if (IsTransactionInitiated)
                    RollBackTransaction();
                else if (transaction != null)
                    Provider.CommandExecutor.RollbackTransaction(transaction);
                throw;
            } 
        }
        /// <summary>
        /// Saves the item to the database within the given transaction, if it has been modified.
        /// </summary>
        /// <param name="item">The item to save</param>
        /// <param name="transaction">The object representing the transaction</param>
        protected virtual void Save(object item, ITransaction transaction)
        {
            ICommand command = null;

            var itemStatus = Provider.GetEntityStatus(item);

            switch (itemStatus)
            {
                case EntityStatus.Insert:
                    command = Provider.CommandBuilder.CreateInsertCommand(item);
                    
                    if (Equals(command, null))
                        throw new UnableToCreateCommandException(Provider.ToString());

                    Provider.CommandBuilder.SupplyInsertCommandParameters(ref command, item);
                    break;
                case EntityStatus.Update:

                    command = Provider.CommandBuilder.CreateUpdateCommand(item);

                    if (Equals(command, null))
                        throw new UnableToCreateCommandException(Provider.ToString());

                    Provider.CommandBuilder.SupplyUpdateCommandParameters(ref command, item);
                    Provider.CommandBuilder.SupplyUpdateCommandWhereParameters(ref command, item);
                    break;
            }

            

            var itemType = item.GetType();
            var reader = Provider.CommandExecutor.ExecuteReader(command, transaction);
            if (reader.Read())
            {
                item = (typeof(IDataObject).IsAssignableFrom(itemType)) 
                    ? Provider.Mapper.GetDataObjectMappingMethod(item.GetType())(reader, this) 
                    : Provider.Mapper.GetObjectMappingMethod(itemType)(reader);
            }
            else
            {
                if (typeof(IDataObject).IsAssignableFrom(itemType))
                    if ((item as IDataObject) != null) 
                        (item as IDataObject).AcceptChanges();
            }

            reader.Close();

            // Save children if they exist
            var schema = Provider.GetSchema(item.GetType());
            foreach (var relation in schema.Associations)
            {
                if (relation.GetFieldValue(item) == null || relation.Hierarchy != RelationHierarchy.Parent)
                {
                    continue;
                }

                var child = relation.GetFieldValue(item);

                if (typeof(ISingleChild).IsAssignableFrom(child.GetType()))
                {
                    var singleChild = child as ISingleChild;
                    if (singleChild != null)
                        Save(singleChild.Child, transaction);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(child.GetType()))
                {
                    var enumerable = child as IEnumerable;
                    if (enumerable != null)
                        foreach (var obj in enumerable)
                            Save(obj, transaction);
                }
            }
        }        
        #endregion

        #region Delete

        /// <summary>
        /// Delete the given data object from the database.
        /// </summary>
        /// <param name="item">The object to delete</param>
        public void Delete(object item)
        {
            var transaction = (IsTransactionInitiated) ? CurrentTransaction
                    : Provider.CommandExecutor.InitiateTransaction();
            try
            {
                Delete(item, transaction);
                if(!IsTransactionInitiated) Provider.CommandExecutor.CommitTransaction(transaction);
            }
            catch
            {
                if(IsTransactionInitiated) 
                    RollBackTransaction();
                else if(transaction != null)
                    Provider.CommandExecutor.RollbackTransaction(transaction);
                throw;
            } 
        }

        /// <summary>
        /// Delete the given data object from the database using the given transaction.
        /// </summary>
        /// <param name="item">The object to delete</param>
        /// <param name="transaction">The object representing the transaction</param>
        protected virtual void Delete (object item, ITransaction transaction )
        {
            var command = Provider.CommandBuilder.CreateDeleteCommand(item);
            Provider.CommandBuilder.SupplyDeleteCommandParameters(ref command, item);
            if (command == null) throw new Exception();
            Provider.CommandExecutor.ExecuteNonQuery(command, transaction);
        }
        #endregion

        #region BeginTransaction

        /// <summary>
        /// Starts a new transaction within this session.
        /// </summary>
        public void BeginTransaction()
        {
            if (IsTransactionInitiated) 
                throw new TransactionInstantiatedAlreadyException();
            CurrentTransaction = Provider.CommandExecutor.InitiateTransaction();
            IsTransactionInitiated = true;
        }
        #endregion

        #region RollBackTransaction

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        public void RollBackTransaction()
        {
            if (!IsTransactionInitiated) 
                throw new TransactionNotInstantiatedException();
            Provider.CommandExecutor.RollbackTransaction(CurrentTransaction);
            CurrentTransaction = null;
            IsTransactionInitiated = false;
        }
        #endregion

        #region CommitTransaction

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        public void CommitTransaction()
        {
            if (!IsTransactionInitiated) 
                throw new TransactionNotInstantiatedException();
            Provider.CommandExecutor.CommitTransaction(CurrentTransaction);
            CurrentTransaction = null;
            IsTransactionInitiated = false;

        }
        #endregion     

        #region GetT

        /// <summary>
        /// Fetches the first record from the table represented by TEntity that match the given expression.
        /// </summary>
        /// <typeparam name="TEntity">The class representing the table to search</typeparam>
        /// <param name="expression">A predicate indicating which records to return</param>
        /// <returns></returns>
        public TEntity GetT<TEntity>(Expression<Func<TEntity, bool>> expression)
            where TEntity : class
        {
            return GetT<TEntity>((Expression)expression);
        }

        /// <summary>
        /// Fetches the first record from the table represented by TEntity that match the given expression.
        /// </summary>
        /// <typeparam name="TEntity">The class representing the table to search</typeparam>
        /// <param name="expression">A predicate indicating which records to return</param>
        /// <returns></returns>
        public TEntity GetT<TEntity> ( Expression expression )
            where TEntity : class
        {
            if(Equals(expression, null))
                throw new ArgumentNullException("expression");

            DbDataReader reader = null;
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateGetTCommand<TEntity>(expression);

                if(Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                reader = GetReader(command);
                
                var newT = default(TEntity);
                if (reader.Read())
                {   
                    newT = (typeof(IDataObject).IsAssignableFrom(typeof(TEntity))) 
                        ? Provider.Mapper.GetDataObjectMappingMethod<TEntity>()(reader, this) 
                        : Provider.Mapper.GetTMappingMethod<TEntity>()(reader);
                    LoadAssociations(ref newT);                    
                }
                return newT;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);                
            }
        }


        public TEntity GetT<TEntity>(params object[] keyValues)
            where TEntity : class
        {
            if(Equals(keyValues, null))
                throw new ArgumentNullException("keyValues");
            
                
            DbDataReader reader = null;
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateGetObjectByKeyCommand<TEntity>();

                if (Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                Provider.CommandBuilder.SupplyGetObjectByKeyCommandParameters(ref command, keyValues);

                reader = GetReader(command);

                var newT = default(TEntity);
                if (reader.Read())
                {
                    newT = (typeof(IDataObject).IsAssignableFrom(typeof(TEntity))) ? Provider.Mapper.GetDataObjectMappingMethod<TEntity>()(reader, this) : Provider.Mapper.GetTMappingMethod<TEntity>()(reader);
                    LoadAssociations(ref newT);
                }               
                return newT;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public TEntity GetT<TEntity>(Procedure procedure)
            where TEntity : class
        {
            if(Equals(procedure, null))
                throw new ArgumentNullException("procedure");

            DbDataReader reader = null;
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateCommandFromProcedure(procedure);

                if (Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                reader = GetReader(command);

                var newT = default(TEntity);
                if (reader.Read())
                {
                    newT = GetTEntity<TEntity>(reader);
                    LoadAssociations(ref newT);
                }
                return newT;
            }
            finally
            {
                if (!Equals(reader, null))
                {
                    reader.Close();
                    Provider.CommandExecutor.FinalizeProcedureParameters(procedure, command);
                }
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }
        #endregion

        #region GetList

        public TList GetList<TList, TEntity>(params object[] keyValues)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            if(Equals(keyValues, null))
                throw new ArgumentNullException("keyValues");

            DbDataReader reader = null;
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateGetObjectByKeyCommand<TEntity>();

                if (Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                Provider.CommandBuilder.SupplyGetObjectByKeyCommandParameters(ref command, keyValues);

                reader = GetReader(command);

                var list = new TList();
                if (typeof(IDataObject).IsAssignableFrom(typeof(TEntity)))
                {
                    var mapper = Provider.Mapper.GetDataObjectMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader, this);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }
                }
                else
                {
                    var mapper = Provider.Mapper.GetTMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }

                }
                return list;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public TList GetList<TList, TEntity>()
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            DbDataReader reader = null;
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateGetListCommand<TEntity>();

                if (Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                reader = GetReader(command);

                var list = new TList();

                if (typeof(IDataObject).IsAssignableFrom(typeof(TEntity)))
                {
                    var mapper = Provider.Mapper.GetDataObjectMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader, this);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }
                }
                else
                {
                    var mapper = Provider.Mapper.GetTMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }
 
                }
                return list;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public TList GetList<TList, TEntity>(Expression<Func<TEntity, bool>> expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            return GetList<TList, TEntity>((Expression)expression);
        }

        public TList GetList<TList, TEntity>(Expression expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            if(Equals(expression, null))
                throw new ArgumentNullException("expression");

            DbDataReader reader = null;
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateGetListCommand<TEntity>(expression);

                if (Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                reader = GetReader(command);

                var list = new TList();

                if (typeof(IDataObject).IsAssignableFrom(typeof(TEntity)))
                {
                    var mapper = Provider.Mapper.GetDataObjectMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader, this);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }
                }
                else
                {
                    var mapper = Provider.Mapper.GetTMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }                        
                }
                return list;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public TList GetList<TList, TEntity>(Procedure procedure)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            if(Equals(procedure, null))
                throw new ArgumentNullException("procedure");

            DbDataReader reader = null;
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateCommandFromProcedure(procedure);

                if (Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                reader = GetReader(command);

                var list = new TList();

                if (typeof(IDataObject).IsAssignableFrom(typeof(TEntity)))
                {
                    var mapper = Provider.Mapper.GetDataObjectMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader, this);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }                        
                }
                else
                {
                    var mapper = Provider.Mapper.GetTMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }                        
                }
                return list;
            }
            finally
            {
                if (!Equals(reader, null))
                {
                    reader.Close();
                    Provider.CommandExecutor.FinalizeProcedureParameters(procedure, command);
                }
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }
        #endregion

        #region GetPagedList
        public TList GetPagedList<TList, TEntity>(int pageSize, int page)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            DbDataReader reader = null;
            ICommand command = null;
            try
            {

                command = Provider.CommandBuilder.CreateGetListByPageCommand<TEntity>();

                if (Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                Provider.CommandBuilder.SupplyGetListByPageCommandParameters(ref command, page, pageSize);

                reader = GetReader(command);

                var list = new TList();

                if (typeof(IDataObject).IsAssignableFrom(typeof(TEntity)))
                {
                    var mapper = Provider.Mapper.GetDataObjectMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader, this);
                        LoadAssociations(ref newT);
                        list.Add(newT);                        
                    }                        
                }
                else
                {
                    var mapper = Provider.Mapper.GetTMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }                        
                }
                return list;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public TList GetPagedList<TList, TEntity>(int pageSize, int page, Expression<Func<TEntity, bool>> expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            return GetPagedList<TList, TEntity>(pageSize, page, (Expression)expression);
        }

        public TList GetPagedList<TList, TEntity>(int pageSize, int page, Expression expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            if(Equals(expression, null))
                throw new ArgumentNullException("expression");

            DbDataReader reader = null;
            ICommand command = null;

            try
            {
                command = Provider.CommandBuilder.CreateGetListByPageCommand<TEntity>(expression);

                if (Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                Provider.CommandBuilder.SupplyGetListByPageCommandParameters(ref command, page, pageSize);

                reader = GetReader(command);

                var list = new TList();

                if (typeof(IDataObject).IsAssignableFrom(typeof(TEntity)))
                {
                    var mapper = Provider.Mapper.GetDataObjectMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader, this);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }                    
                }
                else
                {
                    var mapper = Provider.Mapper.GetTMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }                    
                }
                return list;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public TList GetPagedList<TList, TEntity>(int pageSize, Page page)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            if (page == Page.First)
                return GetPagedList<TList, TEntity>(pageSize, 1);

            var rowCount = Count<TEntity>();
            if (rowCount == pageSize)
                return GetPagedList<TList, TEntity>(pageSize, 1);

            var i = rowCount / pageSize;
            var pageIndex = Math.Ceiling((double)i);

            return GetPagedList<TList, TEntity>(pageSize, (int)pageIndex + 1);
            
        }

        public TList GetPagedList<TList, TEntity>(int pageSize, Page page, Expression<Func<TEntity, bool>> expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            return GetPagedList<TList, TEntity>(pageSize, page, (Expression)expression);
        }

        public TList GetPagedList<TList, TEntity>(int pageSize, Page page, Expression expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            if (Equals(expression, null))
                throw new ArgumentNullException("expression");

            if (page == Page.First)
                return GetPagedList<TList, TEntity>(pageSize, 1, expression);

            var rowCount = Count<TEntity>(expression);
            if (rowCount == pageSize)
                return GetPagedList<TList, TEntity>(pageSize, 1, expression);

            var i = rowCount / pageSize;
            var pageIndex = Math.Ceiling((double)i);
            
            return GetPagedList<TList, TEntity>(pageSize, (int)pageIndex + 1, expression);
            
        }
        #endregion

        #region GetEnumerable
        public IEnumerable<TEntity> GetEnumerable<TEntity>()
            where TEntity : class
        {
            DbDataReader reader = null;
            var command = Provider.CommandBuilder.CreateGetListCommand<TEntity>();
            try
            {

                reader = GetReader(command);

                if (typeof(IDataObject).IsAssignableFrom(typeof(TEntity)))
                {
                    var mapper = Provider.Mapper.GetDataObjectMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var entity = mapper(reader, this);
                        LoadAssociations(ref entity);
                        yield return entity;
                    }
                }
                else
                {
                    var mapper = Provider.Mapper.GetTMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var entity = mapper(reader);
                        LoadAssociations(ref entity);
                        yield return entity;
                    }
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }
        #endregion

        #region Count
        public int Count<TEntity>()
        {
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateCountCommand<TEntity>();
                if (IsTransactionInitiated)
                    return Provider.CommandExecutor.ExecuteCount(command, CurrentTransaction);
                return Provider.CommandExecutor.ExecuteCount(command);
            }
            finally
            {
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public int Count<TEntity>(Expression expression)
        {
            if (Equals(expression, null))
                throw new ArgumentNullException("expression");

            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateCountCommand<TEntity>(expression);

                if (Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                return IsTransactionInitiated ? Provider.CommandExecutor.ExecuteCount(command, CurrentTransaction)
                    : Provider.CommandExecutor.ExecuteCount(command);
            }
            finally
            {
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public int Count<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            if(Equals(expression, null))
                throw new ArgumentNullException("expression");

            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateCountCommand<TEntity>(expression.Body);

                if(Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                return IsTransactionInitiated ? Provider.CommandExecutor.ExecuteCount(command, CurrentTransaction) :
                    Provider.CommandExecutor.ExecuteCount(command);
            }
            finally
            {
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }
        #endregion

        #region Execute
        public int ExecuteProcedure(Procedure procedure)
        {
            if (Equals(procedure, null))
                throw new ArgumentNullException("procedure");

            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateCommandFromProcedure(procedure);

                return Provider.CommandExecutor.ExecuteNonQuery(command);
            }
            finally
            {
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public int ExecuteProcedure(string procedureName)
        {
            if(string.IsNullOrEmpty(procedureName))
                throw new Exception("Procedure Name could never be null or empty");
            var procedure = new Procedure(procedureName);
            return ExecuteProcedure(procedure);
        }

        public TResult ExecuteScalar<TResult>(Procedure procedure)
        {
            if (Equals(procedure, null))
                throw new ArgumentNullException("procedure");

            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateCommandFromProcedure(procedure);

                var result = Provider.CommandExecutor.ExecuteScalar(command);

                if(!typeof(TResult).IsAssignableFrom(result.GetType()))
                    throw new InvalidCastException("Unabe to cast query restult to type TResult");

                return (TResult) result;
            }
            finally
            {
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public TResult ExecuteScalar<TResult>(string procedureName)
        {
            if (string.IsNullOrEmpty(procedureName))
                throw new Exception("Procedure Name should not be null or empty");
            var procedure = new Procedure(procedureName);
            return ExecuteScalar<TResult>(procedure);
        }

        public TResult ExecuteScalar<TEntity, TResult>(Expression<Func<TEntity, TResult>> column, Expression<Func<TEntity, bool>> expression)
            where TEntity : class 
        {
            if(Equals(expression, null))
                throw new ArgumentNullException("expression");

            if(Equals(column, null))
                throw new ArgumentNullException("column");
            
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateExecuteScalarCommand<TEntity>(column, expression);
                
                if(Equals(command, null))
                    throw new UnableToCreateCommandException(Provider.ToString());

                var result = Provider.CommandExecutor.ExecuteScalar(command);

                return (TResult) result;
            }
            finally
            {
                if(!Equals(command, null))
                    Provider.CommandExecutor.FinalizeCommand(command);
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            Provider = null;
            CurrentTransaction = null;
        }
        #endregion

        #endregion

        #region Insert
        public void Insert<TEntity>(ref TEntity item) where TEntity : class
        {
            ICommand command = null;
            DbDataReader reader = null;
            try
            {                
                command = Provider.CommandBuilder.CreateInsertCommand(item);
                Provider.CommandBuilder.SupplyInsertCommandParameters(ref command, item);               

                reader = (IsTransactionInitiated) ? Provider.CommandExecutor.ExecuteReader(command, CurrentTransaction) : Provider.CommandExecutor.ExecuteReader(command);
                if (reader.Read())
                {
                    item = GetTEntity<TEntity>(reader);
                    LoadAssociations(ref item);
                }               
            }
            finally
            {
                if(reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }
        #endregion

        #region Update
        public void Update<TEntity>(TEntity item, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            ICommand command = null;
            try
            {                
                command = Provider.CommandBuilder.CreateUpdateCommand<TEntity>(expression);
                Provider.CommandBuilder.SupplyUpdateCommandParameters(ref command, item);
                if (IsTransactionInitiated)
                    Provider.CommandExecutor.ExecuteNonQuery(command, CurrentTransaction);
                else
                    Provider.CommandExecutor.ExecuteNonQuery(command);
            }
            finally
            {
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }
        #endregion

        #region Delete
        public void Delete<TEntity>(object item) where TEntity : class
        {
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateDeleteCommand(item);
                Provider.CommandBuilder.SupplyDeleteCommandParameters(ref command, item);

                if (IsTransactionInitiated)
                    Provider.CommandExecutor.ExecuteNonQuery(command, CurrentTransaction);
                else
                    Provider.CommandExecutor.ExecuteNonQuery(command);
            }
            finally
            {
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            ICommand command = null;
            try
            {                
                command = Provider.CommandBuilder.CreateDeleteCommand<TEntity>(expression);
                if (IsTransactionInitiated)
                    Provider.CommandExecutor.ExecuteNonQuery(command, CurrentTransaction);
                else
                    Provider.CommandExecutor.ExecuteNonQuery(command);
            }
            finally
            {
                Provider.CommandExecutor.FinalizeCommand(command);
            }

        }
        #endregion

        #region Associations and Relations

        private static Dictionary<Type, Delegate> s_createMethods;

        private static Dictionary<Type, Delegate> CreateMethodsCache
        {
            get
            {
                s_createMethods = s_createMethods ?? new Dictionary<Type, Delegate>();
                return s_createMethods;
            }
        }

        private void LoadAssociations<TEntity>(ref TEntity item)
            where TEntity : class
        {
            var typeT = typeof(TEntity);
            var schema = Provider.GetSchema(typeT);
            if (schema == null) 
                throw new Exception("Schema for the Entity not found");
                
            
            foreach (var relation in schema.Associations)
            {
                var instance = CreateDataObjectRefInstance(relation.ObjectType, this);

                var dataObjectRef = instance as ISessionContainer;

                if (dataObjectRef == null) continue;

                dataObjectRef.Relation = relation;

                foreach (var column in relation.SourceColumns)
                {
                    var keyValue = column.GetFieldValue(item);
                    dataObjectRef.AddReferenceValue(keyValue);
                }
                relation.SetFieldValue(item, dataObjectRef);
            }
        }

        

        private static object CreateDataObjectRefInstance(Type type, ISession session)
        {
            var createMethod = GetCreateISessionContainerMethod(type);
            return createMethod(session);
        }

        private static CreateISessionContainerDelegate GetCreateISessionContainerMethod(Type type)
        {
            if (!CreateMethodsCache.ContainsKey(type))
            {
                Type[] methodArgs = { typeof(ISession) };
                var dm = new DynamicMethod("CreateISessionContainer", typeof(object), methodArgs, type);
                var il = dm.GetILGenerator();
                il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Newobj, type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, methodArgs, null));
                il.Emit(OpCodes.Ret);
                CreateMethodsCache.Add(type, dm.CreateDelegate(typeof(CreateISessionContainerDelegate)));
            }
            return CreateMethodsCache[type] as CreateISessionContainerDelegate;
        }
        #endregion

        #region Misc
        private DbDataReader GetReader(ICommand command)
        {
            return (IsTransactionInitiated) ? Provider.CommandExecutor.ExecuteReader(command, CurrentTransaction) : Provider.CommandExecutor.ExecuteReader(command);
        }

        private TEntity GetTEntity<TEntity>(DbDataReader reader)
        {
            return (typeof(IDataObject).IsAssignableFrom(typeof(TEntity))) ? Provider.Mapper.GetDataObjectMappingMethod<TEntity>()(reader, this) :
                Provider.Mapper.GetTMappingMethod<TEntity>()(reader); 
        }
        #endregion
        
        #region Select
        public ISelect<TEntity> CreateSelect<TEntity>()
            where TEntity : class
        {
            return new Select<TEntity>(this);
        }
        #endregion

        public TList GetChildren<TList, TEntity>(TypeRelation relation, params object[] referenceValue)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new()
        {
            DbDataReader reader = null;
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateGetChildrenCommand<TEntity>(relation, referenceValue);

                reader = GetReader(command);

                var list = new TList();

                if (typeof(IDataObject).IsAssignableFrom(typeof(TEntity)))
                {
                    var mapper = Provider.Mapper.GetDataObjectMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader, this);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }
                }
                else
                {
                    var mapper = Provider.Mapper.GetTMappingMethod<TEntity>();
                    while (reader.Read())
                    {
                        var newT = mapper(reader);
                        LoadAssociations(ref newT);
                        list.Add(newT);
                    }

                }
                return list;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }

        public TEntity GetChild<TEntity>(TypeRelation relation, params object[] referenceValue)
                where TEntity : class, new()
        {
            DbDataReader reader = null;
            ICommand command = null;
            try
            {
                command = Provider.CommandBuilder.CreateGetChildrenCommand<TEntity>(relation, referenceValue);

                reader = GetReader(command);

                var newT = default(TEntity);
                if (reader.Read())
                {
                    newT = (typeof(IDataObject).IsAssignableFrom(typeof(TEntity)))
                        ? Provider.Mapper.GetDataObjectMappingMethod<TEntity>()(reader, this)
                        : Provider.Mapper.GetTMappingMethod<TEntity>()(reader);
                    LoadAssociations(ref newT);
                }
                return newT;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Provider.CommandExecutor.FinalizeCommand(command);
            }
        }
    }
}
