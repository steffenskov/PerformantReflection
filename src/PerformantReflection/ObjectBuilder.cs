using System;
using System.Linq.Expressions;
using PerformantReflection.Reflection;

namespace PerformantReflection
{
	public class ObjectBuilder<T>
	where T : notnull, new()
	{
		private readonly ObjectAccessor _accessor;
		private readonly T _result;

		public ObjectBuilder()
		{
			_result = TypeInstantiator.CreateInstance<T>();
			_accessor = new ObjectAccessor(_result);
		}

		public ObjectBuilder<T> With<TValue>(Expression<Func<T, TValue?>> expression, TValue value)
		{
			var propertyName = ExpressionParser<T>.GetPropertyNameFromExpression(expression);
			_accessor.Properties[propertyName].SetValue(value);
			return this;
		}

		public T Build()
		{
			return _result;
		}
	}
}