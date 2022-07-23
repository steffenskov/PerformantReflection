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
		private static LockedConcurrentDictionary<Type, IReadOnlyCollection<PropertyData>> _typeProperties = new();

		/// <summary>
		/// Gets a dictionary of PropertyMethods for all properties on the target.
		/// </summary>
		/// <param name="target">Target to get properties for</param>
		/// <param name="includePrivateProperties">Whether to include private properties or not</param>
		/// <returns>Dictionary with PropertyMethods keyed by name</returns>
		public static IReadOnlyCollection<PropertyData> GetPropertiesOfType(Type type, bool includePrivateProperties)
		{
			if (type is null)
				throw new ArgumentNullException(nameof(type));
			var result = _typeProperties.GetOrAdd(type, CreatePropertyDatas);
			if (!includePrivateProperties)
				return result.Where(property => property.IsPublic).ToList().AsReadOnly();
			else
				return result;
		}

		private static IReadOnlyCollection<PropertyData> CreatePropertyDatas(Type type)
		{
			var result = new List<PropertyData>();
			foreach (var property in GetPropertyMethods(type, true))
			{
				result.Add(new PropertyData(property.Name, true, property.Getter, property.Setter));
			}
			foreach (var property in GetPropertyMethods(type, false))
			{
				result.Add(new PropertyData(property.Name, false, property.Getter, property.Setter));
			}

			return result.AsReadOnly();
		}

		private static IEnumerable<(string Name, Func<object, object?>? Getter, Action<object, object?>? Setter)> GetPropertyMethods(Type type, bool @public)
		{
			var flag = @public ? System.Reflection.BindingFlags.Public : System.Reflection.BindingFlags.NonPublic;
			foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Instance | flag))
			{
				var getterMethod = CreateGetter(property);
				var setterMethod = CreateSetter(property);
				yield return (property.Name, getterMethod, setterMethod);
			}
		}

		private static Func<object, object?>? CreateGetter(PropertyInfo property)
		{
			var method = property.GetGetMethod(true);
			if (method == null)
				return null;

			var dm = new DynamicMethod($"__get_{property.Name}", typeof(object), new Type[] { typeof(object) }, property.DeclaringType!);
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, property.DeclaringType!);
			il.Emit(OpCodes.Callvirt, method);
			if (property.PropertyType.IsValueType)
			{
				il.Emit(OpCodes.Box, property.PropertyType);
			}
			il.Emit(OpCodes.Ret);
			return (Func<object, object?>)dm.CreateDelegate(typeof(Func<object, object?>));
		}

		private static Action<object, object?>? CreateSetter(PropertyInfo property)
		{
			var method = property.GetSetMethod(true);
			if (method == null)
				return null;

			var dm = new DynamicMethod($"__set_{property.Name}", null, new Type[] { typeof(object), typeof(object) }, property.DeclaringType!);
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, property.DeclaringType!);
			il.Emit(OpCodes.Ldarg_1);
			if (property.PropertyType.IsValueType)
			{
				il.Emit(OpCodes.Unbox_Any, property.PropertyType);
			}
			else
			{
				il.Emit(OpCodes.Castclass, property.PropertyType);
			}
			il.Emit(OpCodes.Callvirt, method!);
			il.Emit(OpCodes.Ret);
			return (Action<object, object?>)dm.CreateDelegate(typeof(Action<object, object?>));
		}
	}
}