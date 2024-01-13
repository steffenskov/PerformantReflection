using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PerformantReflection
{
	/// <summary>
	///     Exposes all the properties of a single object.
	/// </summary>
	public class PropertyCollection : IReadOnlyCollection<PropertyAccessor>
	{
		private readonly IDictionary<string, PropertyAccessor> _accessors;

		internal PropertyCollection(IEnumerable<PropertyAccessor> accessors)
		{
			_accessors = accessors.ToDictionary(accessor => accessor.Name);
		}

		public PropertyAccessor this[string index] => _accessors[index];

		public int Count => _accessors.Count;

		public IEnumerator<PropertyAccessor> GetEnumerator()
		{
			return _accessors.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _accessors.Values.GetEnumerator();
		}

		public bool TryGetValue(string propertyName, [MaybeNullWhen(returnValue:false)] out PropertyAccessor accessor)
		{
			return _accessors.TryGetValue(propertyName, out accessor);
		}
	}
}