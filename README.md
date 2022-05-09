# PerformantReflection
A super small library for making Reflection FAST.

The current version only exposes Properties, allowing you to read their values as well as write new values with basically no performance penalty compared to when not using Reflection.

This is archived by Reflecting types just once, and emitting IL to create a new dynamic method matching the getter and setter. Finally a delegate is cached for these dynamic methods, and invoking these delegates has basically no performance penalty.

# Installation
I recommend using the NuGet package: [PerformantReflection](https://www.nuget.org/packages/PerformantReflection) however feel free to clone the source instead if that suits your needs better.

# Usage
Simply instantiate an `ObjectAccessor` and feed it the object you want to access.
Then you can loop over the `Properties`, index into them with `["PropertyName"]` or use `TryGetValue` if you're unsure whether the property even exists:

```
var myObject = new {
	Username ="Old username"
};

var accessor = new ObjectAccessor(myObject);

var existingUsername = accessor.Properties["Username"].GetValue();

accessor.Properties["Username"].SetValue("New username set");
```