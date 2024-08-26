using System;

using AITSYS.ChangelogGenerator.Services;

namespace AITSYS.ChangelogGenerator;

internal class Program
{
	private static void Main(string[] args)
	{
		var repositoryPath = args.Length > 0
			? args[0]
			: @"I:\Development\GitHub\DisCatSharp";
		var gitService = new GitService(repositoryPath);

		Console.WriteLine("Generating diff");
		var diffContext = gitService.GetUnifiedDiffSinceLastTag();
		Console.WriteLine("Analyzing diff");
		var changelogEntry = RoslynService.AnalyzeUnifiedDiff(diffContext);

		Console.WriteLine(changelogEntry);
	}
}
