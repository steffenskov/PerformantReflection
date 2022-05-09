namespace PerformantReflection.UnitTests.Model;

internal class SimpleObject
{
	public string Username { get; set; }
	public string Password { get; set; }

	public SimpleObject(string username, string password)
	{
		this.Username = username;
		this.Password = password;
	}
}