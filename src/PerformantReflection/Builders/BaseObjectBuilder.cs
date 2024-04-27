namespace PerformantReflection.Builders;

public abstract class BaseObjectBuilder<TSelf, TObject>
	where TSelf : BaseObjectBuilder<TSelf, TObject>
	where TObject : notnull
{
	private readonly ObjectAccessor _accessor;
	private readonly TObject _result;

	protected BaseObjectBuilder((TObject EmptyResult, ObjectAccessor Accessor) values)
	{
		_result = values.EmptyResult;
		_accessor = values.Accessor;
	}

	public TSelf With<TValue>(Expression<Func<TObject, TValue?>> expression, TValue value)
	{
		var propertyName = ExpressionParser<TObject>.GetPropertyNameFromExpression(expression);
		_accessor.Properties[propertyName].SetValue(value);
		return (TSelf)this;
	}

	public TObject Build()
	{
		return _result;
	}
}