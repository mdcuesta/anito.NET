/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using Anito.Data.Common;
using Anito.Data.Extensions;

namespace Anito.Data.SqlClient
{
    public class Translator : AdoTranslatorBase
    {

        private const string SELECT = "SELECT";
        private const string FROM = "FROM";
        private const string SPACE = " ";
        private const string COMMA = ",";

        public Translator(IProvider provider)
            : base(provider)
        {
        }

        protected override Expression ProcessLambda(LambdaExpression expression)
        {
            var lambda = (LambdaExpression)StripQuotes(expression);
            ProcessExpression(lambda.Body);
            return expression;
        }


        protected override Expression ProcessMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.DeclaringType == typeof(Queryable) && expression.Method.Name == "Where")
            {
                ProcessExpression(expression.Arguments[0]);
                CommandTextBuilder.Append(" AS T WHERE ");

                var lambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
                ProcessExpression(lambda.Body);
                return expression;
            }

            if (expression.Method.DeclaringType == typeof(Queryable) && expression.Method.Name == "Single")
            {
                ProcessExpression(expression.Arguments[0]);
                CommandTextBuilder.Append(" AS T WHERE ");

                var lambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
                ProcessExpression(lambda.Body);
                return expression;
            }

            if (expression.Method.DeclaringType == typeof(Queryable) && expression.Method.Name == "Select")
            {
                ProcessExpression(expression.Arguments[0]);
                CommandTextBuilder.Append(" AS T WHERE ");
                return expression;
            }

            if (expression.Method.DeclaringType == typeof(String))
            {
                ProcessExpression(expression.Object);
                var value = expression.Arguments[0].ToString().TrimStart('"').TrimEnd('"');

                switch(expression.Method.Name.ToLower())
                {
                    case "contains":
                        CommandTextBuilder.Append(string.Format(" LIKE ('%{0}%')", value));
                        break;
                    case "startswith":
                        CommandTextBuilder.Append(string.Format(" LIKE ('{0}%')", value));
                        break;
                    case "endswith":
                        CommandTextBuilder.Append(string.Format(" LIKE ('%{0}')", value));
                        break;
                }
                return expression;
            }

            if(expression.Method.DeclaringType == typeof(SqlMethod) || expression.Method.DeclaringType == typeof(QueryExtensions))
            {
                switch (expression.Method.Name.ToLower())
                {
                    case "like":
                        ProcessExpression(expression.Arguments[0]);
                        var value = expression.Arguments[1].ToString().TrimStart('"').TrimEnd('"');
                        CommandTextBuilder.Append(string.Format(" LIKE ('{0}')", value));
                        break;
                    case "between":
                        ProcessExpression(expression.Arguments[0]);
                        CommandTextBuilder.Append(" BETWEEN ");
                        ProcessExpression(expression.Arguments[1]);
                        CommandTextBuilder.Append(" AND ");
                        ProcessExpression(expression.Arguments[2]);    
                        break;
                    case "in":
                        ProcessExpression(expression.Arguments[0]);
                        CommandTextBuilder.Append(" IN( ");
                        ProcessExpression(expression.Arguments[1]);
                        CommandTextBuilder.Append(" ) ");
                        break;
                }
                return expression;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", expression.Method.Name));
        }

        protected override Expression ProcessBinary(BinaryExpression expression)
        {

            CommandTextBuilder.Append("(");

            ProcessExpression(expression.Left);

            switch (expression.NodeType)
            {
                case ExpressionType.And:
                    CommandTextBuilder.Append(" AND ");
                    break;
                case ExpressionType.AndAlso:
                    CommandTextBuilder.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    CommandTextBuilder.Append(" OR ");
                    break;
                case ExpressionType.OrElse:
                    CommandTextBuilder.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    CommandTextBuilder.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    CommandTextBuilder.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    CommandTextBuilder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    CommandTextBuilder.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    CommandTextBuilder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    CommandTextBuilder.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", expression.NodeType));

            }

            ProcessExpression(expression.Right);

            CommandTextBuilder.Append(")");

            return expression;

        }

        protected override Expression ProcessConstant(ConstantExpression expression)
        {
            var q = expression.Value as IQueryable;
            if (q != null)
            {

                var columnBuilder = new StringBuilder();
                var schemaTable = Provider.GetSchema(q.ElementType);

                foreach (var column in schemaTable)
                {
                    if (columnBuilder.Length > 0)
                        columnBuilder.Append(COMMA + SPACE);
                    columnBuilder.Append(column.Name);
                }

                CommandTextBuilder.Append(string.Format(
                    "{0} {1} {2} {3}",
                    SELECT,
                    columnBuilder,
                    FROM,
                    schemaTable.ViewSource
                    ));

            }
            else if (expression.Value == null)
            {
                CommandTextBuilder.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(expression.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        CommandTextBuilder.Append(((bool)expression.Value) ? 1 : 0);
                        break;
                        case TypeCode.String:
                        CommandTextBuilder.Append("'");
                        CommandTextBuilder.Append(expression.Value);
                        CommandTextBuilder.Append("'");
                        break;

                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", expression.Value));
                    default:
                        CommandTextBuilder.Append(expression.Value);
                        break;
                }
            }
            return expression;
        }

        protected override Expression ProcessMemberAccess(MemberExpression expression)
        {
            if (expression.Expression != null && expression.Expression.NodeType == ExpressionType.Parameter)
            {
                var schemaTable = Provider.GetSchema(expression.Member.DeclaringType);

                CommandTextBuilder.Append(schemaTable.GetDbColumn(expression.Member.Name));

                return expression;
            }

            if (expression.Expression != null && expression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var propertyInfo = expression.Member as PropertyInfo;

                if (Equals(propertyInfo, null))
                    throw new NullReferenceException("propertyInfo");

                var memberExpression = expression.Expression as MemberExpression;

                if (Equals(memberExpression, null))
                    throw new NullReferenceException("memberExpression");

                if(typeof(PropertyInfo).IsAssignableFrom(memberExpression.Member.GetType()))
                {
                    ProcessExpression(expression.Expression);
                    return expression;
                }

                var fieldInfo = memberExpression.Member as FieldInfo;

                if (Equals(fieldInfo, null))
                    throw new NullReferenceException("fieldInfo");

                var cExpression = memberExpression.Expression as ConstantExpression;               

                if (Equals(cExpression, null))
                    throw new NullReferenceException("cExpression");
                
                var objectValue = fieldInfo.GetValue(cExpression.Value);

                var value = propertyInfo.GetValue(objectValue, null);

                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.String:
                        CommandTextBuilder.Append(string.Format("'{0}'", value));
                        break;
                    case TypeCode.DateTime:
                        CommandTextBuilder.Append(string.Format("'{0}'", ((DateTime)value).ToString("MM/dd/yyyy H:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)));
                        break;
                    default:
                        CommandTextBuilder.Append(value);
                        break;
                }
                return expression;
            }

            if (expression.Expression != null && expression.Expression.NodeType == ExpressionType.Constant)
            {
                var fieldInfo = expression.Member as FieldInfo;

                if (Equals(fieldInfo, null))
                    throw new NullReferenceException("fieldInfo");

                var cExpression = expression.Expression as ConstantExpression;

                if (Equals(cExpression, null))
                    throw new NullReferenceException("cExpression");

                var value = fieldInfo.GetValue(cExpression.Value);

                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.String:
                        CommandTextBuilder.Append(string.Format("'{0}'", value));
                        break;
                    case TypeCode.DateTime:
                        CommandTextBuilder.Append(string.Format("'{0}'", ((DateTime)value).ToString("MM/dd/yyyy H:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)));
                        break;
                    default:
                        CommandTextBuilder.Append(value);
                        break;
                }
                return expression;
            }

            if (expression.Expression != null && expression.Expression.NodeType == ExpressionType.Call)
            {
                var propertyInfo = expression.Member as PropertyInfo;

                if (Equals(propertyInfo, null))
                    throw new NullReferenceException("propertyInfo");

                var callExpression = expression.Expression as MethodCallExpression;

                if (Equals(callExpression, null))
                    throw new NullReferenceException("callExpression");

                var methodInfo = callExpression.Method;
                var methodParams = new object[callExpression.Arguments.Count];
                for (var i = 0; i < callExpression.Arguments.Count; i++)
                {
                    var consExpression = callExpression.Arguments[i] as ConstantExpression;

                    if (Equals(consExpression, null))
                        throw new NullReferenceException("consExpression");
                    methodParams[i] = consExpression.Value;
                }

                var memberExpression = callExpression.Object as MemberExpression;

                if (Equals(memberExpression, null))
                    throw new NullReferenceException("memberExpression");

                var fieldInfo = memberExpression.Member as FieldInfo;

                if (Equals(fieldInfo, null))
                    throw new NullReferenceException("fieldInfo");

                var cExpression = memberExpression.Expression as ConstantExpression;

                if (Equals(cExpression, null))
                    throw new NullReferenceException("cExpression");

                var objectContainer = fieldInfo.GetValue(cExpression.Value);

                var objectValue = methodInfo.Invoke(objectContainer, methodParams);

                var value = propertyInfo.GetValue(objectValue, null);

                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.String:
                        CommandTextBuilder.Append(string.Format("'{0}'", value));
                        break;
                    case TypeCode.DateTime:
                        CommandTextBuilder.Append(string.Format("'{0}'", ((DateTime)value).ToString("MM/dd/yyyy H:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)));
                        break;
                    default:
                        CommandTextBuilder.Append(value);
                        break;
                }
                return expression;
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", expression.Member.Name));
        }

        protected override Expression ProcessUnary(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Not:
                    CommandTextBuilder.Append(" NOT ");
                    ProcessExpression(expression.Operand);
                    break;
                case ExpressionType.Quote:
                    var lambda = (LambdaExpression)StripQuotes(expression);
                    ProcessExpression(lambda.Body);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", expression.NodeType));
            }
            return expression;
        }
    }
}
