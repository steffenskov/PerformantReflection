namespace PerformantReflection.UnitTests.Model;

internal class DummyObject
{
	public string Username { get; set; }
	public string Password { get; set; }

	public DummyObject(string username, string password)
	{
		this.Username = username;
		this.Password = password;
	}
}