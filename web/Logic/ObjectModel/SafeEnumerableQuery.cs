using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Logic.ObjectModel
{

    public static class DefaultGenerator
    {
        public static object DefaultValue(this Type parameter)
        {
            var defaultGeneratorType =
              typeof(DefaultGenerator<>).MakeGenericType(parameter);

            return defaultGeneratorType.InvokeMember(
              "GetDefault",
              BindingFlags.Static |
              BindingFlags.Public |
              BindingFlags.InvokeMethod,
              null, null, new object[0]);
        }
    }

    public class DefaultGenerator<Type>
    {
        public static Type GetDefault()
        {
            return default(Type);
        }
    }

    public class SafeEnumerableQuery<T> : IOrderedQueryable<T>, IQueryable<T>, IOrderedQueryable, IQueryable, IQueryProvider, IEnumerable<T>, IEnumerable
    {
        internal interface IExpressionProxy { };

        internal class ExpressionDecoder : ExpressionVisitor
        {
        }

        internal class ExpressionCoder : ExpressionVisitor
        {

            static Expression Null = Expression.Constant(null);

            public static Expression Proxy(Expression baseNode)
            {
                return (Expression)Activator.CreateInstance(typeof(ExpressionProxy<>).MakeGenericType(typeof(T), baseNode.GetType()), baseNode);
            }

            private bool CanConvertToNullable(Expression node)
            {
                return node.Type.IsValueType && !(node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(Nullable<>));
            }

            private Expression ConvertToNullable(Expression node)
            {
                return (Expression)Expression.Convert(node, typeof(Nullable<>).MakeGenericType(node.Type));
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression == null && node.Type == typeof(DateTime)
                    || (node.Expression is MemberExpression && (node.Expression as MemberExpression).Expression.NodeType == ExpressionType.Constant)
                    || (node.Expression.Type.IsGenericType && node.Expression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    || (node.Expression.Type == typeof(DateTime))
                    )
                    return node;
                if (typeof(IBusinessObject).IsAssignableFrom(node.Expression.Type) &&
                    (node.Member is PropertyInfo && (!((PropertyInfo)node.Member).CanWrite ||
                    node.Member.GetCustomAttribute<NotMappedAttribute>() != null)))
                    throw new NotSupportedException("Property " + node.Member.Name + " not supported");
                var res = node.Expression.NodeType == ExpressionType.MemberAccess
                    ? Expression.Condition(
                        Expression.NotEqual(this.Visit(node.Expression), Null),
                            CanConvertToNullable(node)
                                ? ConvertToNullable(node)
                                : node,
                        CanConvertToNullable(node)
                            ? Expression.Convert(Null, typeof(Nullable<>).MakeGenericType(node.Type))
                            : Expression.Convert(Null, node.Type))
                    : base.VisitMember(node);
                return res;
            }

            private bool IsNull(Expression exp)
            {
                return exp is ConstantExpression && (exp as ConstantExpression).Value == null;
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);
                ConvertToNullable(ref right, ref left);
                var lifted = right.Type.IsGenericType && right.Type.GetGenericTypeDefinition() == typeof(Nullable<>) && !IsNull(right) && !IsNull(left) ? true : node.IsLiftedToNull;
                var nullExpr = Expression.MakeBinary(node.NodeType, left, right, lifted, node.Method);
                return node.Type == typeof(bool) && nullExpr.Type.Equals(typeof(bool?))
                    ? Expression.Coalesce(nullExpr, Expression.Constant((bool?)false))
                    : nullExpr;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(Enumerable) || node.Method.DeclaringType == typeof(Queryable)
                    || node.Method.DeclaringType == typeof(string) || node.Method.DeclaringType == typeof(TimeSpan) || node.Method.DeclaringType == typeof(DateTime)
                    || typeof(IEnumerable).IsAssignableFrom(node.Method.DeclaringType))
                {
                    var left = Expression.Call(node.Object, node.Method, node.Arguments.Select(arg => ConvertType(arg)));
                    if ((node.Method.DeclaringType == typeof(Enumerable) || node.Method.DeclaringType == typeof(Queryable)) && node.Type.IsValueType)
                    {
                        //                       var right = Expression.Constant(node.Type.DefaultValue(), left.Type);
                        var nullleft = ConvertToNullable(left);
                        var nul = Expression.Constant(null, nullleft.Type);
                        return Expression.Condition(Expression.NotEqual(left.Arguments[0], Null), nullleft, nul);
                    }
                    return left;
                }
                throw new NotSupportedException("Method not supported");
            }

            protected override Expression VisitLambda<LambdaType>(Expression<LambdaType> node)
            {
                var body = Visit(node.Body);
                var types = typeof(LambdaType).GetGenericArguments();
                body = node.Body.Type == typeof(bool) && body.Type.Equals(typeof(bool?))
                    ? Expression.Coalesce(body, Expression.Constant((bool?)false))
                    : body;
                var newType = typeof(Func<,>).MakeGenericType(types[0], body.Type);
                return Expression.Lambda(newType, body, node.Parameters);
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                var op = Visit(node.Operand);
                if (node.NodeType == ExpressionType.Convert && op.Type == node.Type)
                    return op;
                var type = (op.Type != node.Operand.Type && !node.Type.IsGenericType && op.Type.IsGenericType
                    && op.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    ? typeof(Nullable<>).MakeGenericType(node.Type)
                    : node.Type;
                return Expression.MakeUnary(node.NodeType, op, type);
            }

            protected override Expression VisitNew(NewExpression node)
            {
                return node.Constructor != null
                    ? Expression.New(node.Constructor, node.Arguments.Select(a => ConvertType(a)))
                    : base.VisitNew(node);
            }

            private Expression ConvertType(Expression expr)
            {
                var visited = Visit(expr);
                if (expr.Type != visited.Type)
                {
                    var type = expr as LambdaExpression != null ? (expr as LambdaExpression).ReturnType : expr.Type;
                    if (type.IsValueType)
                    {
                        Expression def = ConvertToNullable(Expression.Constant(type.DefaultValue()));
                        visited = expr is LambdaExpression ? (visited as LambdaExpression).Body : visited;
                        visited = Expression.Convert(Expression.Coalesce(visited, def), type);
                        visited = expr is LambdaExpression ? Expression.Lambda(visited, (expr as LambdaExpression).Parameters) : visited;
                    }
                }
                return visited;
            }

            public override Expression Visit(Expression node)
            {
                var res = node is IExpressionProxy
                    ? node
                    : base.Visit(node);
                return res;
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                var bindings = new List<MemberAssignment>();
                foreach (var b in node.Bindings.Cast<MemberAssignment>())
                {
                    Expression bb = Visit(b.Expression);
                    if (bb.Type != b.Expression.Type)
                        bb = Expression.Convert(bb, b.Expression.Type);
                    bindings.Add(Expression.Bind(b.Member, bb));
                }
                return Expression.MemberInit(node.NewExpression, bindings);
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                var tr = Visit(node.IfTrue);
                var fa = Visit(node.IfFalse);
                ConvertToNullable(ref tr, ref fa);
                var exp = Visit(node.Test);
                if (exp.Type == typeof(bool?))
                    exp = Expression.Coalesce(exp, Expression.Constant(false));
                return Expression.Condition(exp, tr, fa);
            }

            private void ConvertToNullable(ref Expression right, ref Expression left)
            {
                if (right.Type != left.Type)
                {
                    if (CanConvertToNullable(right))
                        right = ConvertToNullable(right);
                    if (CanConvertToNullable(left))
                        left = ConvertToNullable(left);
                }
            }

            internal class ExpressionProxy<ExpType> : Expression, IExpressionProxy where ExpType : Expression
            {
                ExpType expr;

                public ExpressionProxy(ExpType expr)
                {
                    this.expr = expr;
                }

                public override string ToString()
                {
                    return "Proxy(" + expr.ToString() + ")";
                }

                public override ExpressionType NodeType
                {
                    get
                    {
                        return expr.NodeType;
                    }
                }

                public override Type Type
                {
                    get
                    {
                        return expr.Type;
                    }
                }

                public override bool CanReduce
                {
                    get
                    {
                        return true;
                    }
                }

                public override Expression Reduce()
                {
                    return expr;
                }
            }
        }

        EnumerableQuery<T> query;
        public SafeEnumerableQuery(IEnumerable<T> list)
        {
            query = new EnumerableQuery<T>(list);
        }

        public SafeEnumerableQuery(EnumerableQuery<T> query)
        {
            this.query = query;
        }

        public SafeEnumerableQuery(Expression expression)
        {
            query = new EnumerableQuery<T>(expression);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var decodedQuery = new EnumerableQuery<T>(new ExpressionDecoder().Visit(((IQueryable)query).Expression));
            return ((IEnumerable<T>)decodedQuery).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType
        {
            get { return ((IQueryable)query).ElementType; }
        }

        public Expression Expression
        {
            get { return ((IQueryable)query).Expression; }
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            expression = ExpressionCoder.Proxy(new ExpressionCoder().Visit(expression));
            return new SafeEnumerableQuery<TElement>((EnumerableQuery<TElement>)((IQueryProvider)query).CreateQuery(expression));
        }

        public IQueryable CreateQuery(Expression expression)
        {
            expression = ExpressionCoder.Proxy(new ExpressionCoder().Visit(expression));
            return new SafeEnumerableQuery<T>((EnumerableQuery<T>)((IQueryProvider)query).CreateQuery(expression));
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        public object Execute(Expression expression)
        {
            expression = new ExpressionDecoder().Visit(new ExpressionCoder().Visit(expression));
            return ((IQueryProvider)query).Execute(expression);
        }
    }
}
