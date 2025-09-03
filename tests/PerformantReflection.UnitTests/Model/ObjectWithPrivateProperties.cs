namespace PerformantReflection.UnitTests.Model;

public class ObjectWithPrivateProperties
{
	public int Age { get; set; }

	private Guid Private { get; set; }

	private double PrivateTwo { get; set; }
}