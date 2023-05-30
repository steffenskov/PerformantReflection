using System;
using System.Linq;
using System.Linq.Expressions;

namespace PerformantReflection.Reflection
{
	internal static class ExpressionParser<T>
	{
		public static string GetPropertyNameFromExpression<TValue>(Expression<Func<T, TValue?>> expression)
		{
			var propertyName = GetMemberName(expression);
			var properties = TypePropertyCache.GetPropertiesOfType(typeof(T), false).ToDictionary(prop => prop.Name);
			if (!properties.TryGetValue(propertyName, out var propertyData))
				throw new InvalidOperationException(
					$"{typeof(T).Name} doesn't contain a property named {propertyName}.");

			return propertyData.Name;
		}

		private static string GetMemberName(Expression expression)
		{
			return expression.NodeType switch
			{
				ExpressionType.Lambda => GetMemberName(((LambdaExpression)expression).Body),
				ExpressionType.MemberAccess => ((MemberExpression)expression).Member.Name,
				ExpressionType.Convert => GetMemberName(((UnaryExpression)expression).Operand),
				_ => throw new NotSupportedException($"Unsupported node type: {expression.NodeType}")
			};
		}
	}
}