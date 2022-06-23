using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PerformantReflection
{

	/// <summary>
	/// Exposes all the properties of a single object.
	/// </summary>
	public class PropertyCollection : IEnumerable<PropertyAccessor>, IReadOnlyCollection<PropertyAccessor>
	{
		private readonly IDictionary<string, PropertyAccessor> _accessors;

		public int Count => _accessors.Count;

		public PropertyAccessor this[string index] => _accessors[index];

		internal PropertyCollection(IEnumerable<PropertyAccessor> accessors)
		{
			_accessors = accessors.ToDictionary(accessor => accessor.Name);
		}

		public bool TryGetValue(string propertyName, out PropertyAccessor? accessor)
		{
			return _accessors.TryGetValue(propertyName, out accessor);
		}

		public IEnumerator<PropertyAccessor> GetEnumerator()
		{
			return _accessors.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _accessors.Values.GetEnumerator();
		}
	}
}