# PerformantReflection
A super small library for making Reflection FAST.

The current version exposes Properties, allowing you to read their values as well as write new values with basically no performance penalty compared to when not using Reflection.
It also has a sped-up version of the .Net built-in `Activator.CreateInstance`: `TypeInstantiator.CreateInstance`.

This is archived by reflecting types just once, and emitting IL to create new dynamic methods. Finally a delegate is cached for these dynamic methods, and invoking these delegates has basically no performance penalty.

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