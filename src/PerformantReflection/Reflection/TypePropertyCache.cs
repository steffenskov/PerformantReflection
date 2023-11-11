using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PerformantReflection.Collections;

namespace PerformantReflection.Reflection
{
	internal static class TypePropertyCache
	{
		private static readonly LockedConcurrentDictionary<Type, IReadOnlyCollection<PropertyInformation>> _typeProperties = new();

		/// <summary>
		///     Gets a dictionary of PropertyMethods for all properties on the target.
		/// </summary>
		/// <param name="type">Type to get properties for</param>
		/// <param name="includePrivateProperties">Whether to include private properties or not</param>
		/// <returns>Dictionary with PropertyMethods keyed by name</returns>
		public static IReadOnlyCollection<PropertyInformation> GetPropertiesOfType(Type type, bool includePrivateProperties)
		{
			if (type is null) throw new ArgumentNullException(nameof(type));

			var result = _typeProperties.GetOrAdd(type, tp => CreatePropertyInformationCollection(tp, new HashSet<Type>()));
			if (!includePrivateProperties)
				return result.Where(property => property.IsPublic).ToList().AsReadOnly();
			return result;
		}

		private static IReadOnlyCollection<PropertyInformation> CreatePropertyInformationCollection(Type type, ISet<Type> mappedTypes, bool typeIsNestedInterface = false)
		{
			var result = new List<PropertyInformation>();
			if (mappedTypes.Contains(type))
				return result;

			foreach (var property in GetPropertyMethods(type, true)) result.Add(new PropertyInformation(property.Name, true, property.Type, property.Getter, property.Setter));
			foreach (var property in GetPropertyMethods(type, false)) result.Add(new PropertyInformation(property.Name, false, property.Type, property.Getter, property.Setter));
			if (type.IsInterface && !typeIsNestedInterface)
				foreach (var implementedInterfaceType in type.GetInterfaces())
					result.AddRange(CreatePropertyInformationCollection(implementedInterfaceType, mappedTypes, true));

			return result.AsReadOnly();
		}

		private static IEnumerable<(string Name, Type Type, Func<object, object?>? Getter, Action<object, object?>? Setter)> GetPropertyMethods(Type type, bool @public)
		{
			var flag = @public ? BindingFlags.Public : BindingFlags.NonPublic;
			foreach (var property in type.GetProperties(BindingFlags.Instance | flag))
			{
				var getterMethod = type.IsInterface ? null : CreateGetter(property);
				var setterMethod = type.IsInterface ? null : CreateSetter(property);
				yield return (property.Name, property.PropertyType, getterMethod, setterMethod);
			}
		}

		private static Func<object, object?>? CreateGetter(PropertyInfo property)
		{
			var method = property.GetGetMethod(true);
			if (method == null) return null;
			if (property.DeclaringType is null)
				return null;

			var dm = new DynamicMethod($"__get_{property.Name}", typeof(object), new[] { typeof(object) }, property.DeclaringType);
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(property.DeclaringType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, property.DeclaringType);
			il.Emit(OpCodes.Callvirt, method);
			if (property.PropertyType.IsValueType) il.Emit(OpCodes.Box, property.PropertyType);
			il.Emit(OpCodes.Ret);
			return (Func<object, object?>)dm.CreateDelegate(typeof(Func<object, object?>));
		}

		private static Action<object, object?>? CreateSetter(PropertyInfo property)
		{
			var method = property.GetSetMethod(true);
			if (method == null) return null;
			if (property.DeclaringType is null)
				return null;

			if (property.DeclaringType.IsValueType)
				return null;
			
			var dm = new DynamicMethod($"__set_{property.Name}", null, new[] { typeof(object), typeof(object) }, property.DeclaringType);
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(property.DeclaringType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, property.DeclaringType);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(property.PropertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, property.PropertyType);
			il.Emit(OpCodes.Callvirt, method!);
			il.Emit(OpCodes.Ret);
			return (Action<object, object?>)dm.CreateDelegate(typeof(Action<object, object?>));
		}
	}
}