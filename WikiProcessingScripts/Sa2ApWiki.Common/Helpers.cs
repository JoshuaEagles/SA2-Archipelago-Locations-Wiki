namespace Sa2ApWiki.Common;

public class Helpers
{
	public static string ZeroPadNumber(int number, int length)
	{
		return number.ToString().PadLeft(length, '0');
	}
}