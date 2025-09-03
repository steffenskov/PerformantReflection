namespace PerformantReflection;

/// <summary>
///     Used for generating and instantiating implementation classes for interfaces.
///     The interface in question must only contain properties and must be public.
/// </summary>
public static class InterfaceImplementationGenerator
{
	private static readonly ModuleBuilder _moduleBuilder;
	private static readonly LockedConcurrentDictionary<Type, Type> _implementationTypeMap = new();

	static InterfaceImplementationGenerator()
	{
		var aName = new AssemblyName($"InterfaceImplementationGeneratorTypes{GenerateStrippedGuid()}");
		var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

		// The module name is usually the same as the assembly name.
		_moduleBuilder = ab.DefineDynamicModule(aName.Name!);
	}

	/// <summary>
	///     Generates an implementation class for the interface of type T and returns an instance of it.
	/// </summary>
	/// <typeparam name="T">Interface type to generation implementation for</typeparam>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Type T is invalid</exception>
	public static T CreateInstance<T>()
	{
		return (T)CreateInstance(typeof(T));
	}

	/// <summary>
	///     Generates an implementation class for the interface of the type given and returns an instance of it.
	/// </summary>
	/// <exception cref="InvalidOperationException">Type is invalid</exception>
	public static object CreateInstance(Type type)
	{
		var implementationType = GenerateImplementationType(type);
		return TypeInstantiator.CreateInstance(implementationType);
	}

	/// <summary>
	///     Generates an implementation class for the interface of type T and returns its type.
	/// </summary>
	/// <typeparam name="T">Interface type to generation implementation for</typeparam>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Type T is invalid</exception>
	public static Type GenerateImplementationType<T>()
	{
		return GenerateImplementationType(typeof(T));
	}

	/// <summary>
	///     Generates an implementation class for the interface of the type given returns its type.
	/// </summary>
	/// <exception cref="InvalidOperationException">Type is invalid</exception>
	public static Type GenerateImplementationType(Type type)
	{
		if (!type.IsInterface)
		{
			throw new InvalidOperationException("Type T must be an interface");
		}

		if (!type.IsVisible)
		{
			throw new InvalidOperationException("Type T must be public");
		}

		return _implementationTypeMap.GetOrAdd(type, CreateImplementationType);
	}


	private static Type CreateImplementationType(Type type)
	{
		var typeBuilder = _moduleBuilder.DefineType($"{type.Name}Implementation{GenerateStrippedGuid()}", TypeAttributes.Public);
		typeBuilder.AddInterfaceImplementation(type);
		CreateProperties(type, typeBuilder, new HashSet<Type>(), new HashSet<string>());
		CreateMethods(type, typeBuilder);

		return typeBuilder.CreateType();
	}

	private static void CreateProperties(Type type, TypeBuilder typeBuilder, ISet<Type> mappedTypes, ISet<string> excludedProperties)
	{
		if (!mappedTypes.Add(type))
		{
			return;
		}

		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

		foreach (var prop in properties)
		{
			if (prop is { CanWrite: false, GetMethod.IsAbstract: false }) // Get only property with explicit implementation, skip those
			{
				continue;
			}

			var fullPropertyName = $"{prop.DeclaringType?.FullName}.{prop.Name}";
			if (excludedProperties.Contains(fullPropertyName))
			{
				continue;
			}

			CreateProperty(typeBuilder, prop);
		}

		var explicitProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
		foreach (var prop in explicitProperties)
		{
			excludedProperties.Add(prop.Name);
		}

		foreach (var implementedInterfaceType in type.GetInterfaces())
		{
			CreateProperties(implementedInterfaceType, typeBuilder, mappedTypes, excludedProperties);
		}
	}

	private static void CreateProperty(TypeBuilder typeBuilder, PropertyInfo prop)
	{
		var propertyName = prop.Name;
		var propertyType = prop.PropertyType;
		var fieldBuilder =
			typeBuilder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);
		var propertyBuilder = typeBuilder.DefineProperty($"{propertyName}",
			PropertyAttributes.HasDefault,
			propertyType,
			null);

		var propVisibility = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;

		// Define the "get" accessor method for CustomerName.
		var getMethod = typeBuilder.DefineMethod($"get_{propertyName}",
			propVisibility,
			propertyType,
			Type.EmptyTypes);

		var getMethodIlGenerator = getMethod.GetILGenerator();

		getMethodIlGenerator.Emit(OpCodes.Ldarg_0);
		getMethodIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
		getMethodIlGenerator.Emit(OpCodes.Ret);

		// Define the "set" accessor method for CustomerName.
		var setMethod = typeBuilder.DefineMethod($"set_{propertyName}",
			propVisibility,
			null,
			new[]
			{
				propertyType
			});

		var setMethodIlGenerator = setMethod.GetILGenerator();

		setMethodIlGenerator.Emit(OpCodes.Ldarg_0);
		setMethodIlGenerator.Emit(OpCodes.Ldarg_1);
		setMethodIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);
		setMethodIlGenerator.Emit(OpCodes.Ret);

		// Last, we must map the two methods created above to our PropertyBuilder to
		// their corresponding behaviors, "get" and "set" respectively.
		propertyBuilder.SetGetMethod(getMethod);
		propertyBuilder.SetSetMethod(setMethod);


		// Associate the interface property accessors with the implementation methods
		var interfaceMethods = prop.GetAccessors();
		var interfaceGetterMethod = interfaceMethods.FirstOrDefault(method => method.ReturnType != typeof(void));
		var interfaceSetterMethod = interfaceMethods.FirstOrDefault(method => method.ReturnType == typeof(void));

		if (interfaceGetterMethod is not null)
		{
			typeBuilder.DefineMethodOverride(getMethod, interfaceGetterMethod);
		}

		if (interfaceSetterMethod is not null)
		{
			typeBuilder.DefineMethodOverride(setMethod, interfaceSetterMethod);
		}
	}

	private static void CreateMethods(Type type, TypeBuilder typeBuilder)
	{
		var methods = type.GetMethods()
			.Where(method => !method.IsSpecialName); // IsSpecialName == implementation of properties etc. so skip those
		foreach (var method in methods)
		{
			CreateMethod(typeBuilder, method);
		}
	}

	private static void CreateMethod(TypeBuilder typeBuilder, MethodInfo method)
	{
		var parameterTypes = method.GetParameters()
			.Select(param => param.ParameterType)
			.ToArray();

		var methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, method.ReturnType, parameterTypes);

		if (method.IsGenericMethodDefinition)
		{
			var genericParamNames = method.GetGenericArguments()
				.Select(genericParam => genericParam.Name)
				.ToArray();
			methodBuilder.DefineGenericParameters(genericParamNames);
		}

		var ilGenerator = methodBuilder.GetILGenerator();
		if (method.ReturnType != typeof(void))
		{
			if (method.ReturnType.IsClass)
			{
				ilGenerator.Emit(OpCodes.Ldnull);
			}
			else
			{
				EmitValueTypeDefault(ilGenerator, method.ReturnType);
			}
		}

		ilGenerator.Emit(OpCodes.Ret);

		typeBuilder.DefineMethodOverride(methodBuilder, method);
	}

	private static void EmitValueTypeDefault(ILGenerator ilGenerator, Type type)
	{
		if (type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte) ||
		    type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint))
		{
			ilGenerator.Emit(OpCodes.Ldc_I4_0);
		}
		else if (type == typeof(long) || type == typeof(ulong))
		{
			ilGenerator.Emit(OpCodes.Ldc_I8, 0L);
		}
		else if (type == typeof(float))
		{
			ilGenerator.Emit(OpCodes.Ldc_R4, 0.0f);
		}
		else if (type == typeof(double))
		{
			ilGenerator.Emit(OpCodes.Ldc_R8, 0.0d);
		}
		else if (type == typeof(IntPtr) || type == typeof(UIntPtr))
		{
			ilGenerator.Emit(OpCodes.Ldc_I4_0);
			ilGenerator.Emit(OpCodes.Conv_I);
		}
		else if (type == typeof(char))
		{
			ilGenerator.Emit(OpCodes.Ldc_I4_0);
		}
		else
		{
			// For structs, enums, and other value types, use initobj
			var local = ilGenerator.DeclareLocal(type);
			ilGenerator.Emit(OpCodes.Ldloca_S, local);
			ilGenerator.Emit(OpCodes.Initobj, type);
			ilGenerator.Emit(OpCodes.Ldloc, local);
		}
	}

	private static string GenerateStrippedGuid()
	{
		return Guid.NewGuid().ToString().Replace("-", string.Empty);
	}
}