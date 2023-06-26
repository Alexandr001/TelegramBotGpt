namespace Main;

public class CustomException : Exception
{
	public CustomException(string mess) : base(mess) { }
}