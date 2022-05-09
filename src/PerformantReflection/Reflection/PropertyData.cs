namespace PerformantReflection.Reflection;

internal record PropertyData(string Name, bool IsPublic, Func<object, object?>? Getter, Action<object, object?>? Setter);