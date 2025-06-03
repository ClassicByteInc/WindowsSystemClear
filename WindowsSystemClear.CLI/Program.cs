namespace WindowsSystemClear.CLI;
public static class Program
{
	static void Main(string[] args)
	{
		Console.WriteLine("Windows System Clear CLI Tool");
		Console.WriteLine("");
		if (args.Contains("--clear"))
		{
			Clear();
		}
		else if (args.Contains("--help") || args.Contains("-h"))
		{
			Console.WriteLine("用法:wscc.exe [option] [argument]");

			Console.WriteLine("选项:");
			Console.WriteLine("--clear [targetFileName]\t\t清除系统缓存和临时文件(默认使用自带清理文件)");
			Console.WriteLine("--help, -h\t显示帮助信息");
			Console.WriteLine("--analysis \t分析系统状态并生成报告(在后面添加 '-t' 选项以通过 target 文件来分析,或者 '-a'直接分析整个系统分区.");
			Console.WriteLine("--force, -f\t强制执行");
		}
		else if (args.Contains("--analysis"))
		{
			Analysis();
		}

		//
		void Clear()
		{

			Dictionary<string, string> targets;
			if (args.Length > 1)
			{
				targets = TargetList.Load(args[1]).Targets;
			}
			else
			{

				targets = TargetList.Load("targets.xml").Targets;
			}

		}

		void Analysis()
		{

		}
	}
}