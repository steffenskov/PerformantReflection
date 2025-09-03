namespace PerformantReflection.Reflection;

static internal class ExpressionParser<T>
{
	public static string GetPropertyNameFromExpression<TValue>(Expression<Func<T, TValue?>> expression)
	{
		var propertyName = GetMemberName(expression);
		var properties = TypePropertyCache.GetPropertiesOfType(typeof(T), false).ToDictionary(prop => prop.Name);
		if (!properties.TryGetValue(propertyName, out var propertyInformation))
		{
			throw new InvalidOperationException(
				$"{typeof(T).Name} doesn't contain a property named {propertyName}.");
		}

		return propertyInformation.Name;
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