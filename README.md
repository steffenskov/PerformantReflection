# PerformantReflection

A super small library for making Reflection FAST.

The current version exposes Properties, allowing you to read their values as well as write new values with basically no
performance penalty compared to when not using Reflection.

It has a sped-up version of the .Net built-in `Activator.CreateInstance`: `TypeInstantiator.CreateInstance`.
This is archived by reflecting types just once, and emitting IL to create new dynamic methods. Finally a delegate is
cached for these dynamic methods, and invoking these delegates
has basically no performance penalty.

It also has an `ObjectBuilder`/`InterfaceObjectBuilder` for instantiating classes/interfaces and setting properties with
private setters to some custom value. I personally use this
for Fakes in integration tests, when Mocks don't cut it.

Finally there's the `InterfaceImplementationGenerator` which allows you to create implementation classes of interfaces
on-the-fly. This can be quite useful for exchanging e.g. JSON
based messages.

# Installation

I recommend using the NuGet package: [PerformantReflection](https://www.nuget.org/packages/PerformantReflection) however
feel free to clone the source instead if that suits your
needs better.

# Usage

## For properties

Simply instantiate an `ObjectAccessor` and feed it the object you want to access.
Then you can loop over the `Properties`, index into them with `["PropertyName"]` or use `TryGetValue` if you're unsure
whether the property even exists:

### Examples:

```
class User {
	public string Username { get;set; }
}

var user = new User {
	Username = "Old username"
};

var accessor = new ObjectAccessor(user);

var existingUsername = accessor.Properties["Username"].GetValue();

accessor.Properties["Username"].SetValue("New username");
```

## For instantiating types

Simply call `TypeInstantiator.CreateInstance(someType)` similar to how you would with `Activator.CreateInstance` - it's
that simple :-)

Or if you need to populate properties with public getters but private setters, use the `ObjectBuilder` like this:

```
var instance = new ObjectBuilder<YourType>()
                .With(inst => inst.Id, Guid.NewGuid()) // The Id property will be set to Guid.NewGuid()
                .With(inst => inst.Name, "Some name") // Likewise Name will be set to "Some name"
                .Build();
```

## For iterating over properties of types

Use the `TypePropertyCache` to get a collection of properties for any given type. As the name implies this information
is cached for rapid repeated lookup.
Furthermore the resulting type `PropertyInformation` holds high-performance getter and setter methods (assuming the
property in question has a getter or setter)

## For generating implementation types based on interfaces

Simply call `InterfaceImplementationGenerator.GenerateImplementationType<T>` or `CreateInstance<T>` depending on your
needs.
This will generate an implementation type for the interface (and instantiate an instance if
calling `CreateInstance<T>`).

This can be quite useful when sending e.g. JSON serialized messages where you don't want to share the implementation
class, but rather just the interface.
It can also be useful for situations where your class has private setters and you want to maintain that.
Simply specify an interface with only the getters, and use `GenerateImplementationType<T>` to implement a class with a
public setter that allows you to deserialize the JSON, yet
doesn't allow the caller to mutate the resulting instance.

Furthermore if you want to build an instance of your interface, with predefined values (quite useful for unit testing),
use the `InterfaceObjectBuilder` in the same fashion as
the "ordinary" `ObjectBuilder`:

```
var instance = new InterfaceObjectBuilder<IYourInterface>()
                .With(inst => inst.Id, Guid.NewGuid()) // The Id property will be set to Guid.NewGuid()
                .With(inst => inst.Name, "Some name") // Likewise Name will be set to "Some name"
                .Build();
```

## For Json deserialization of properties with private setters

Either use the `JsonSerializerOptionsFactory` class to create a new `JsonSerializerOptions` instance with the necessary
configuration, or use the extension method `AddPrivateDeserializationSupport` of  `JsonSerializerOptions` to configure
an existing options instance.
Both reside in the `PerformantReflection.Json` namespace.

An example:

```
public class User
{
    public Guid Id { get; private set; }
    
    public User()
    {
        Id = Guid.NewGuid();
    }
}

var user = new User();
var json = JsonSerializer.Serialize(user);

var options = JsonSerializerOptionsFactory.CreateOptionsWithPrivateDeserializationSupport();
var deserialized = JsonSerializer.Deserialize<User>(json, options);

// deserialized will have the Id deserialized as well now
```

# Documentation
Auto generated documentation via [DocFx](https://github.com/dotnet/docfx) is available here: https://steffenskov.github.io/PerformantReflection/