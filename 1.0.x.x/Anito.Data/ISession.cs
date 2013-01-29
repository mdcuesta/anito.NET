/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections;
using Anito.Data.Query;
using Anito.Data.Schema;

namespace Anito.Data
{
    public interface ISession : IDisposable
    {
        //Transaction
        void BeginTransaction();
        void CommitTransaction();
        void RollBackTransaction();

        //Create, Update, Delete
        void Save(object item);
        void Delete(object item);

        void Insert<TEntity>(ref TEntity item) where TEntity : class;
        void Update<TEntity>(TEntity item, Expression<Func<TEntity, bool>> expression) where TEntity : class;
        void Delete<TEntity>(object item) where TEntity : class;
        void Delete<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;

        //Properties
        bool IsTransactionInitiated { get; }
        IProvider Provider { get; }

        //Read
        TEntity GetT<TEntity>(Expression<Func<TEntity, bool>> expression)
            where TEntity : class;
        TEntity GetT<TEntity>(Expression expression)
            where TEntity : class;
        TEntity GetT<TEntity>(params object[] keyValues)
            where TEntity : class;
        TEntity GetT<TEntity>(Procedure procedure)
            where TEntity : class;
        

        TList GetList<TList, TEntity>()
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();
        TList GetList<TList, TEntity>(Expression<Func<TEntity, bool>> expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();
        TList GetList<TList, TEntity>(Expression expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();
        TList GetList<TList, TEntity>(Procedure procedure)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();
        TList GetList<TList, TEntity>(params object[] keyValues)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();

        TList GetPagedList<TList, TEntity>(int pageSize, int page)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();
        TList GetPagedList<TList, TEntity>(int pageSize, int page, Expression<Func<TEntity, bool>> expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();
        TList GetPagedList<TList, TEntity>(int pageSize, int page, Expression expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();
        TList GetPagedList<TList, TEntity>(int pageSize, Page page)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();
        TList GetPagedList<TList, TEntity>(int pageSize, Page page, Expression<Func<TEntity, bool>> expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();
        TList GetPagedList<TList, TEntity>(int pageSize, Page page, Expression expression)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();

        //Association
        TList GetChildren<TList, TEntity>(TypeRelation relation, params object[] referenceValue)
            where TList : IList<TEntity>, IEnumerable, new()
            where TEntity : class, new();

        TEntity GetChild<TEntity>(TypeRelation relation, params object[] referenceValue)
                where TEntity : class, new();

        //Count
        int Count<TEntity>();
        int Count<TEntity>(Expression<Func<TEntity, bool>> expression);
        int Count<TEntity>(Expression expression);

        //Execute
        int ExecuteProcedure(Procedure procedure);
        int ExecuteProcedure(string procedureName);
        TResult ExecuteScalar<TResult>(Procedure procedure);
        TResult ExecuteScalar<TResult>(string procedureName);
        TResult ExecuteScalar<TEntity, TResult>(Expression<Func<TEntity, TResult>> column, Expression<Func<TEntity, bool>> expression)
            where TEntity : class;

        //Select
        ISelect<TEntity> CreateSelect<TEntity>() where TEntity : class;

        IEnumerable<TEntity> GetEnumerable<TEntity>() where TEntity : class;
    }
}
