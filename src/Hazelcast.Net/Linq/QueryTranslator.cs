// Copyright (c) 2008-2022, Hazelcast, Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Hazelcast.Linq
{
    internal class QueryTranslator : ExpressionVisitor
    {
        private StringBuilder _sb;
        private string _mapName;

        public QueryTranslator(string mapName)
        {
            _mapName = mapName;
        }

        internal Expression TranslateAsExpression(Expression expression)
        {
            _sb = new StringBuilder();
            return Visit(expression);
        }

        internal string Translate(Expression expression)
        {
            _sb = new StringBuilder();
            Visit(expression);
            return _sb.ToString();
        }

        private static Expression StripQuotes(Expression node)
        {
            while (node.NodeType == ExpressionType.Quote)
            {
                node = ((UnaryExpression)node).Operand;
            }

            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _sb.Append('(');

            this.Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                    _sb.Append(" OR");
                    break;

                case ExpressionType.Equal:
                    _sb.Append(" = ");
                    break;

                case ExpressionType.NotEqual:
                    _sb.Append(" <> ");
                    break;

                case ExpressionType.LessThan:
                    _sb.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    _sb.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    _sb.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    _sb.Append(" >= ");
                    break;

                default:
                    throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported");
            }

            this.Visit(node.Right);

            _sb.Append(')');

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    _sb.Append(" NOT ");
                    this.Visit(node.Operand);
                    break;

                default:
                    throw new NotSupportedException($"The unary operator '{ node.NodeType}' is not supported");
            }

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            IAsyncQueryable q = node.Value as IAsyncQueryable;

            if (q != null)
            {
                // assume constant nodes w/ IQueryables are table references
                _sb.Append("SELECT * FROM ");
                _sb.Append(_mapName);
            }
            else if (node.Value == null)
            {
                _sb.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(node.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        _sb.Append(((bool)node.Value) ? 1 : 0);
                        break;

                    case TypeCode.String:
                        _sb.Append("'");
                        _sb.Append(node.Value);
                        _sb.Append("'");
                        break;

                    case TypeCode.Object:
                        throw new NotSupportedException($"The constant for '{node.Value}' is not supported");

                    default:
                        _sb.Append(node.Value);
                        break;
                }
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                //TODO: re-think, what if custom types contain KeyValuePair??Check depth??
                if (node.Member.ReflectedType.Name == typeof(KeyValuePair<,>).Name && node.Member.Name == "Key")
                {
                    _sb.Append("__key");
                }
                else
                {
                    //Todo: if value is primative then use 'this'
                    _sb.Append(node.Member.Name);
                }

                return node;
            }

            throw new NotSupportedException($"The member '{node.Member.Name}' is not supported");
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(AsyncQueryable) && node.Method.Name == "Where")
            {
                _sb.Append("SELECT * FROM (");
                this.Visit(node.Arguments[0]);
                _sb.Append(") AS T WHERE ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                this.Visit(lambda.Body);
                return node;
            }

            throw new NotSupportedException($"The method '{node.Method.Name}' is not supported");
        }
    }
}
