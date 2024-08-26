using System;
using System.Collections.Generic;
using System.Linq;

using AITSYS.ChangelogGenerator.Models;

using LibGit2Sharp;

namespace AITSYS.ChangelogGenerator.Services;

public sealed class GitService(string repositoryPath, string? afterCommitHash = null, string? beforeCommitHash = null)
{
	public DiffContext? GetUnifiedDiff()
	{
		using var repo = new Repository(repositoryPath);
		var afterCommit = afterCommitHash is not null
			? repo.Lookup(afterCommitHash, ObjectType.Commit).Peel<Commit>()
			: repo.Tags.LastOrDefault()?.PeeledTarget as Commit ?? repo.Head.Tip;

		var beforeCommit = beforeCommitHash is not null
			? repo.Lookup(beforeCommitHash, ObjectType.Commit).Peel<Commit>()
			: repo.Head.Tip;

		var patch = repo.Diff.Compare<Patch>(afterCommit.Tree, beforeCommit.Tree);

		Console.WriteLine("Generating diffs");
		var diffs = (from entry in patch
					where entry.Path.EndsWith(".cs", StringComparison.Ordinal)
					let oldFileContent = GetFileContentAtCommit(repo, entry.OldPath, afterCommit)
					let newFileContent = GetFileContentAtCommit(repo, entry.Path, beforeCommit)
					select new DiffContext
					{
						FilePath = entry.Path,
						OldFile = entry.OldPath,
						NewFile = entry.Path,
						OldContent = oldFileContent?.Split('\n').ToList() ?? [],
						NewContent = newFileContent?.Split('\n').ToList() ?? [],
						LinesAdded = entry.LinesAdded,
						LinesDeleted = entry.LinesDeleted
					}).ToList();

		return diffs.Any()
			? CombineDiffs(diffs)
			: null;
	}

	private static DiffContext CombineDiffs(List<DiffContext> diffs)
	{
		Console.WriteLine("Combining diffs");
		var combinedDiff = new DiffContext
		{
			FilePath = diffs.First().FilePath,
			OldFile = diffs.First().OldFile,
			NewFile = diffs.First().NewFile,
			OldContent = [],
			NewContent = []
		};

		foreach (var diff in diffs)
		{
			combinedDiff.OldContent.AddRange(diff.OldContent ?? []);
			combinedDiff.NewContent.AddRange(diff.NewContent ?? []);
		}

		return combinedDiff;
	}

	private static string GetFileContentAtCommit(Repository repo, string filePath, Commit commit)
	{
		var blob = commit[filePath]?.Target as Blob;
		return blob?.GetContentText() ?? string.Empty;
	}
}
