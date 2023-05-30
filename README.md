# PerformantReflection
A super small library for making Reflection FAST.

The current version exposes Properties, allowing you to read their values as well as write new values with basically no performance penalty compared to when not using Reflection.

It also has a sped-up version of the .Net built-in `Activator.CreateInstance`: `TypeInstantiator.CreateInstance`.
This is archived by reflecting types just once, and emitting IL to create new dynamic methods. Finally a delegate is cached for these dynamic methods, and invoking these delegates has basically no performance penalty.

Finally it has an `InstanceBuilder` for instantiating classes and setting properties with private setters to some custom value. I personally use this for Fakes in integration tests, when Mocks don't cut it.

# Installation
I recommend using the NuGet package: [PerformantReflection](https://www.nuget.org/packages/PerformantReflection) however feel free to clone the source instead if that suits your needs better.

# Usage

## For properties
Simply instantiate an `ObjectAccessor` and feed it the object you want to access.
Then you can loop over the `Properties`, index into them with `["PropertyName"]` or use `TryGetValue` if you're unsure whether the property even exists:

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
Simply call `TypeInstantiator.CreateInstance(someType)` similar to how you would with `Activator.CreateInstance` - it's that simple :-)

Or if you need to populate properties with public getters but private setters, use the `InstanceBuilder` like this:
```
var instance = new InstanceBuilder<YourType>()
                .With(inst => inst.Id, Guid.NewGuid()) // The Id property will be set to Guid.NewGuid()
                .With(inst => inst.Name, "Some name") // Likewise Name will be set to "Some name"
                .Build();
```