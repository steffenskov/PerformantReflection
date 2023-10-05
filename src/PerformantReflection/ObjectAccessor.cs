using System;
using System.Linq;
using PerformantReflection.Reflection;

namespace PerformantReflection
{
	/// <summary>
	///     ObjectAccessor let's you get and set all properties of an object.
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
				throw new ArgumentNullException(nameof(target));

			var properties = TypePropertyCache.GetPropertiesOfType(target.GetType(), includePrivateProperties);
			var accessors = properties
				.Select(property => new PropertyAccessor(target, property));

			Properties = new PropertyCollection(accessors);
		}

		public PropertyCollection Properties { get; }
	}
}