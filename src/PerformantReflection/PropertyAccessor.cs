using System;
using PerformantReflection.Reflection;

namespace PerformantReflection
{

	/// <summary>
	/// PropertyAccessor let's you get and set any property of an object.
	/// </summary>
	public class PropertyAccessor
	{
		private readonly Func<object, object?>? _getter;
		private readonly Action<object, object?>? _setter;

		private readonly object _target;

		public string Name { get; }


		/// <summary>
		/// Whether or not the property has a getter, if no getter exists the GetValue method will throw an InvalidOperationException if invoked.
		/// </summary>
		public bool HasGetter => _getter is not null;

		/// <summary>
		/// Whether or not the property has a setter, if no setter exists the SetValue method will throw an InvalidOperationException if invoked.
		/// </summary>
		public bool HasSetter => _setter is not null;

		internal PropertyAccessor(object target, PropertyData property)
		{
			if (target is null)
				throw new ArgumentNullException(nameof(target));
			if (property is null)
				throw new ArgumentNullException(nameof(property));
			if (property.Getter is null && property.Setter is null)
				throw new ArgumentNullException(nameof(property), $"Both getter and setter were null for property {property.Name}");

			_target = target;
			_getter = property.Getter;
			_setter = property.Setter;
			Name = property.Name;
		}

		/// <summary>
		/// Gets the property's value.
		/// </summary>
		/// <returns>Current value of the property</returns>
		public object? GetValue()
		{
			if (_getter is null)
				throw new InvalidOperationException($"Property {Name} has no getter");

			return _getter(_target);
		}

		/// <summary>
		/// Sets the property's value. The caller has to ensure the value has the proper type or an InvalidCastException will be thrown.
		/// </summary>
		/// <param name="value">New value to set to the property</param>
		public void SetValue(object? value)
		{
			if (_setter is null)
				throw new InvalidOperationException($"Property {Name} has no setter");

			_setter(_target, value);
		}
	}
}