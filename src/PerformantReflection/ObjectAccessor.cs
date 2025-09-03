namespace PerformantReflection;

/// <summary>
///     ObjectAccessor lets you get and set all properties of an object.
/// </summary>
public class ObjectAccessor
{
	/// <summary>
	///     Create a new ObjectAccessor which exposes the properties of the target given.
	/// </summary>
	/// <param name="target">Target object to access properties of.</param>
	/// <param name="includePrivateProperties">Whether to include private properties or not.</param>
	public ObjectAccessor(object target, bool includePrivateProperties = false)
	{
		if (target is null)
		{
			throw new ArgumentNullException(nameof(target));
		}

		var properties = TypePropertyCache.GetPropertiesOfType(target.GetType(), includePrivateProperties);
		var accessors = properties
			.Select(property => new PropertyAccessor(target, property));

		Properties = new PropertyCollection(accessors);
	}

	public PropertyCollection Properties { get; }

	public static ObjectAccessor<TObject> Create<TObject>(TObject target, bool includePrivateProperties = false)
		where TObject : notnull
	{
		return new ObjectAccessor<TObject>(target, includePrivateProperties);
	}
}

/// <summary>
///     ObjectAccessor lets you get and set all properties of an object.
/// </summary>
public class ObjectAccessor<TObject> : ObjectAccessor
	where TObject : notnull
{
	/// <summary>
	///     Create a new ObjectAccessor which exposes the properties of the target given.
	/// </summary>
	/// <param name="target">Target object to access properties of.</param>
	/// <param name="includePrivateProperties">Whether to include private properties or not.</param>
	public ObjectAccessor(TObject target, bool includePrivateProperties = false) : base(target, includePrivateProperties)
	{
	}

	/// <summary>
	///     Sets the specified property to the provided value.
	/// </summary>
	/// <typeparam name="TValue">Property value type.</typeparam>
	/// <param name="expression">Expression selecting a direct property on <typeparamref name="TObject" />.</param>
	/// <param name="value">Value to assign.</param>
	/// <exception cref="ArgumentNullException">Thrown if the expression is null.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the property has no setter.</exception>
	public void SetProperty<TValue>(Expression<Func<TObject, TValue?>> expression, TValue? value)
	{
		ArgumentNullException.ThrowIfNull(expression);
		var propertyName = ExpressionParser<TObject>.GetPropertyNameFromExpression(expression);
		Properties[propertyName].SetValue(value);
	}
}