using System.Collections.Generic;

namespace AITSYS.ChangelogGenerator.Models;

public sealed class DiffContext
{
	public string FilePath { get; set; }
	public string OldFile { get; set; }
	public string NewFile { get; set; }
	public List<string> OldContent { get; set; }
	public List<string> NewContent { get; set; }
	public int LinesAdded { get; set; }
	public int LinesDeleted { get; set; }
}
