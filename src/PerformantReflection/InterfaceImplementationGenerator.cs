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
		var methods = type.GetMethods();
		if (methods.Any(m => !m.IsSpecialName))
		{
			throw new InvalidOperationException("Cannot create an implementation for an interface that contains methods");
		}

		var tb = _moduleBuilder.DefineType($"{type.Name}Implementation{GenerateStrippedGuid()}", TypeAttributes.Public);
		tb.AddInterfaceImplementation(type);
		CreateProperties(type, tb, new HashSet<Type>());

		return tb.CreateType()!;
	}

	private static void CreateProperties(Type type, TypeBuilder typeBuilder, ISet<Type> mappedTypes)
	{
		if (mappedTypes.Contains(type))
		{
			return;
		}

		mappedTypes.Add(type);
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

		foreach (var prop in properties)
		{
			CreateProperty(typeBuilder, prop);
		}

		foreach (var implementedInterfaceType in type.GetInterfaces())
		{
			CreateProperties(implementedInterfaceType, typeBuilder, mappedTypes);
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

	private static string GenerateStrippedGuid()
	{
		return Guid.NewGuid().ToString().Replace("-", string.Empty);
	}
}