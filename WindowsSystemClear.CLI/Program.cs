namespace WindowsSystemClear.CLI;


public static class Program
{
	static void Main(string[] args)
	{
		if (args.Contains("--clear"))
		{

		}
		else if (args.Contains("--help") || args.Contains("-h"))
		{
			Console.WriteLine("Usage: WindowsSystemClear.CLI --clear [options]");
			Console.WriteLine("Options:");
			Console.WriteLine("  --clear    Clear the system cache and temporary files.");
			Console.WriteLine("  --help, -h Show this help message.");
		}
		else
		{
			Console.WriteLine("Invalid argument. Use --help for usage information.");
		}
	}
}