using System;

namespace PerformantReflection.Reflection
{
    public record PropertyInformation(string Name, bool IsPublic, Type Type, Func<object, object?>? Getter, Action<object, object?>? Setter);
}