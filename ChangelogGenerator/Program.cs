using System;
using System.IO;

using AITSYS.ChangelogGenerator.Services;

string? repositoryPath = null;
string? afterSha = null;
string? beforeSha = null;
string? outputPath = null;

if (args is null || args.Length is 0)
{
	repositoryPath = @"I:\\Development\\GitHub\\DisCatSharp";
	afterSha = "98ebc5d11ad754293c15ad3fc4e2d5896b55c531";
	beforeSha = null;
}

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
		case "--output":
		case "-o":
			if (i + 1 < args.Length)
				outputPath = args[++i];
			else
			{
				Console.WriteLine("Error: --output requires a value.");
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
	_ = Console.ReadKey();
	return;
}

Console.WriteLine("Analyzing diff...");
var changelogEntry = RoslynService.AnalyzeUnifiedDiff(diffContext);

if (outputPath is not null)
{
	var text = File.CreateText(outputPath);
	await text.WriteAsync(changelogEntry.ToString());
}
else
	Console.WriteLine(changelogEntry);

Console.WriteLine("Done");
_ = Console.ReadKey();
return;

static void PrintHelp()
{
	Console.WriteLine("Usage: AITSYS.ChangelogGenerator.exe [options]");
	Console.WriteLine("Options:");
	Console.WriteLine("  --help, -h               Show this help message.");
	Console.WriteLine("  --path, -r PATH          Path to the repository.");
	Console.WriteLine("  --after, -a SHA          Commits after this SHA (optional).");
	Console.WriteLine("  --before, -b SHA         Commits before this SHA (optional).");
	Console.WriteLine("  --output, -o PATH        Write changes to file (optional).");
}
