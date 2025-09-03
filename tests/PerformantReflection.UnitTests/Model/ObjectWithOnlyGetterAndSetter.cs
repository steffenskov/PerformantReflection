namespace PerformantReflection.UnitTests.Model;

public class ObjectWithOnlyGetterAndSetter
{
	public int Getter { get; }

	public string Setter
	{
		set
		{
		}
	}
}