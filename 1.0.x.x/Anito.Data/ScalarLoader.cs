using System;
using System.Linq.Expressions;

namespace Anito.Data
{
    public class ScalarLoader<TEntity, TReturnType> : SessionContainer
        where TEntity : class
    {
        private TReturnType m_value;

        private Expression<Func<TEntity, bool>> Expression { get; set; }
        private Expression<Func<TEntity, TReturnType>> Column { get; set; }

        internal ScalarLoader(ISession session)
           : base(session)
        {
        }

        public ScalarLoader(Expression<Func<TEntity, TReturnType>> column, Expression<Func<TEntity, bool>> expression)
            : base()
        {
            Expression = expression;
            Column = column;
        }

        protected override void Load()
        {
            if(Equals(Expression, null)
                throw new Exception("Expression is null");

            if(!Equals(DataSession, null))
                m_value = DataSession.ExecuteScalar(Column, Expression);
            
        }
    }
}
