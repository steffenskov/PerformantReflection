namespace PerformantReflection.Json;

/// <summary>
///     Factory for creating JsonSerializerOptions to support deserialization of properties with private setters.
/// </summary>
public static class JsonSerializerOptionsFactory
{
	/// <summary>
	///     Creates an otherwise empty instance of JsonSerializerOptions with the TypeInfoResolver configured to support
	///     deserialization of properties with private setters.
	/// </summary>
	public static JsonSerializerOptions CreateOptionsWithPrivateDeserializationSupport()
	{
		return new JsonSerializerOptions().AddPrivateDeserializationSupport();
	}

	/// <summary>
	///     Configures the TypeInfoResolver to support deserialization of properties with private setters.
	/// </summary>
	public static JsonSerializerOptions AddPrivateDeserializationSupport(this JsonSerializerOptions options)
	{
		options.TypeInfoResolver = new DefaultJsonTypeInfoResolver()
			.WithAddedModifier(typeInfo =>
			{
				var type = typeInfo.Type;
				var jsonProperties = typeInfo.Properties;
				var typeProperties = TypePropertyCache.GetPropertiesOfType(type, false)
					.ToDictionary(prop => prop.Name);
				foreach (var jsonProp in jsonProperties)
				{
					jsonProp.Set ??= typeProperties[jsonProp.Name].Setter;
				}
			});

		return options;
	}
}