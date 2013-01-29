/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System.Linq.Expressions;
using System.Text;

namespace Anito.Data.Common
{
    public abstract class AdoTranslatorBase
    {
        protected virtual StringBuilder CommandTextBuilder { get; set; }

        protected IProvider Provider { get; set; }

        protected AdoTranslatorBase(IProvider provider)
        {
            Provider = provider;
        }

        public virtual string Translate(Expression expression)
        {
            CommandTextBuilder = new StringBuilder();
            ProcessExpression(expression);
            return CommandTextBuilder.ToString();
        }

        protected virtual void ProcessExpression(Expression expression)
        {
            var type = expression.GetType();

            if (type == typeof(MethodCallExpression)
                || typeof(MethodCallExpression).IsAssignableFrom(type))
                ProcessMethodCall(expression as MethodCallExpression);
            else if (type == typeof(BinaryExpression)
                || typeof(BinaryExpression).IsAssignableFrom(type))
                ProcessBinary(expression as BinaryExpression);
            else if (type == typeof(ConstantExpression)
                || typeof(ConstantExpression).IsAssignableFrom(type))
                ProcessConstant(expression as ConstantExpression);
            else if (type == typeof(MemberExpression)
                || typeof(MemberExpression).IsAssignableFrom(type))
                ProcessMemberAccess(expression as MemberExpression);
            else if (type == typeof(UnaryExpression)
                || typeof(UnaryExpression).IsAssignableFrom(type))
                ProcessUnary(expression as UnaryExpression);
            else if (type == typeof(LambdaExpression)
                || typeof(LambdaExpression).IsAssignableFrom(type))
                ProcessLambda(expression as LambdaExpression);
            else if (type == typeof(NewArrayExpression))
                ProcessNewArrayExpression(expression as NewArrayExpression);
            else if (type.BaseType == typeof(LambdaExpression)
                || typeof(LambdaExpression).IsAssignableFrom(type.BaseType))
                ProcessLambda(expression as LambdaExpression);
 
        }

        protected abstract Expression ProcessLambda(LambdaExpression expression);

        protected abstract Expression ProcessMethodCall(MethodCallExpression expression);

        protected abstract Expression ProcessBinary(BinaryExpression expression);

        protected abstract Expression ProcessConstant(ConstantExpression expression);

        protected abstract Expression ProcessMemberAccess(MemberExpression expression);

        protected abstract Expression ProcessUnary(UnaryExpression expression);

        protected virtual Expression ProcessNewArrayExpression(NewArrayExpression expression)
        {
            var notFirst = false;
            foreach (var member in expression.Expressions)
            {
                if (notFirst)
                    CommandTextBuilder.Append(",");
                
                ProcessExpression(member);
                notFirst = true;
            }
            return expression;
        }

        public virtual Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression)expression).Operand;
            return expression;
        }
    }
}
