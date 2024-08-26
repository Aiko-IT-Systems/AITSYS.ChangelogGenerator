using System;

using AITSYS.ChangelogGenerator.Services;

namespace AITSYS.ChangelogGenerator;

internal class Program
{
	private static void Main(string[]? args = null)
	{
		string? repositoryPath = null;
		string? afterSha = null;
		string? beforeSha = null;

		for (var i = 0; i < args.Length; i++)
			switch (args[i])
			{
				case "--help":
				case "-h":
					PrintHelp();
					return;
				case "--path":
				case "-r":
					if (i + 1 < args.Length)
						repositoryPath = args[++i];
					else
					{
						Console.WriteLine("Error: --path requires a value.");
						return;
					}

					break;
				case "--after":
				case "-a":
					if (i + 1 < args.Length)
						afterSha = args[++i];
					else
					{
						Console.WriteLine("Error: --after requires a SHA value.");
						return;
					}

					break;
				case "--before":
				case "-b":
					if (i + 1 < args.Length)
						beforeSha = args[++i];
					else
					{
						Console.WriteLine("Error: --before requires a SHA value.");
						return;
					}

					break;
				default:
					Console.WriteLine($"Unknown argument: {args[i]}");
					PrintHelp();
					return;
			}

		if (string.IsNullOrEmpty(repositoryPath))
		{
			Console.WriteLine("Error: Repository path is required.");
			PrintHelp();
			return;
		}

		var gitService = new GitService(repositoryPath, afterSha, beforeSha);

		Console.WriteLine("Generating diff...");
		var diffContext = gitService.GetUnifiedDiff();

		if (diffContext is null)
		{
			Console.WriteLine("No usable diffs found, aborting");
			Console.ReadKey();
			return;
		}

		Console.WriteLine("Analyzing diff...");
		var changelogEntry = RoslynService.AnalyzeUnifiedDiff(diffContext);

		Console.WriteLine(changelogEntry);
		Console.WriteLine("Done");
		Console.ReadKey();
	}

	private static void PrintHelp()
	{
		Console.WriteLine("Usage: AITSYS.ChangelogGenerator.exe [options]");
		Console.WriteLine("Options:");
		Console.WriteLine("  --help, -h               Show this help message.");
		Console.WriteLine("  --path, -r PATH          Path to the repository.");
		Console.WriteLine("  --after, -a SHA          Commits after this SHA (optional).");
		Console.WriteLine("  --before, -b SHA         Commits before this SHA (optional).");
	}
}
