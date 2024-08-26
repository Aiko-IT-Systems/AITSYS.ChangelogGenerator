using System.Collections.Generic;
using System.Text;

namespace AITSYS.ChangelogGenerator.Models;

public sealed class ChangelogEntry
{
	public List<string> AddedTypes { get; set; } = [];
	public List<(string Before, string After)> ModifiedTypes { get; set; } = [];
	public List<string> RemovedTypes { get; set; } = [];

	public List<string> AddedMethods { get; set; } = [];
	public List<(string Before, string After)> ModifiedMethods { get; set; } = [];
	public List<string> RemovedMethods { get; set; } = [];

	public List<string> AddedEnums { get; set; } = [];
	public List<(string Before, string After)> ModifiedEnums { get; set; } = [];
	public List<string> RemovedEnums { get; set; } = [];

	public List<string> AddedEnumValues { get; set; } = [];
	public List<(string Before, string After)> ModifiedEnumValues { get; set; } = [];
	public List<string> RemovedEnumValues { get; set; } = [];

	public List<string> AddedProperties { get; set; } = [];
	public List<(string Before, string After)> ModifiedProperties { get; set; } = [];
	public List<string> RemovedProperties { get; set; } = [];

	public List<string> AddedFields { get; set; } = [];
	public List<(string Before, string After)> ModifiedFields { get; set; } = [];
	public List<string> RemovedFields { get; set; } = [];

	public List<string> AddedEvents { get; set; } = [];
	public List<(string Before, string After)> ModifiedEvents { get; set; } = [];
	public List<string> RemovedEvents { get; set; } = [];

	public override string ToString()
	{
		var sb = new StringBuilder();

		if (this.AddedTypes.Count > 0)
		{
			sb.AppendLine("### New Types:");
			foreach (var type in this.AddedTypes)
				sb.AppendLine($"- {type}");
			sb.AppendLine();
		}

		if (this.ModifiedTypes.Count > 0)
		{
			sb.AppendLine("### Modified Types:");
			foreach (var (before, after) in this.ModifiedTypes)
				sb.AppendLine($"- Before: {before}\n  After: {after}");
			sb.AppendLine();
		}

		if (this.RemovedTypes.Count > 0)
		{
			sb.AppendLine("### Removed Types:");
			foreach (var type in this.RemovedTypes)
				sb.AppendLine($"- {type}");
			sb.AppendLine();
		}

		if (this.AddedEnums.Count > 0)
		{
			sb.AppendLine("### New Enums:");
			foreach (var type in this.AddedEnums)
				sb.AppendLine($"- {type}");
			sb.AppendLine();
		}

		if (this.AddedEnumValues.Count > 0)
		{
			sb.AppendLine("### New Enum Values:");
			foreach (var value in this.AddedEnumValues)
				sb.AppendLine($"- {value}");
			sb.AppendLine();
		}

		if (this.ModifiedEnumValues.Count > 0)
		{
			sb.AppendLine("### Modified Enum Values:");
			foreach (var (before, after) in this.ModifiedEnumValues)
				sb.AppendLine($"- Before: {before}\n  After: {after}");
			sb.AppendLine();
		}

		if (this.RemovedEnumValues.Count > 0)
		{
			sb.AppendLine("### Removed Enum Values:");
			foreach (var value in this.RemovedEnumValues)
				sb.AppendLine($"- {value}");
			sb.AppendLine();
		}

		/*if (this.ModifiedEnums.Count > 0)
		{
			sb.AppendLine("### Modified Enums:");
			foreach (var (before, after) in this.ModifiedEnums)
				sb.AppendLine($"- Before: {before}\n  After: {after}");
			sb.AppendLine();
		}*/

		if (this.RemovedEnums.Count > 0)
		{
			sb.AppendLine("### Removed Enums:");
			foreach (var type in this.RemovedEnums)
				sb.AppendLine($"- {type}");
			sb.AppendLine();
		}

		if (this.AddedMethods.Count > 0)
		{
			sb.AppendLine("### Added Methods:");
			foreach (var method in this.AddedMethods)
				sb.AppendLine($"- {method}");
			sb.AppendLine();
		}

		if (this.ModifiedMethods.Count > 0)
		{
			sb.AppendLine("### Modified Methods:");
			foreach (var (before, after) in this.ModifiedMethods)
				sb.AppendLine($"- Before: {before}\n  After: {after}");
			sb.AppendLine();
		}

		if (this.RemovedMethods.Count > 0)
		{
			sb.AppendLine("### Removed Methods:");
			foreach (var method in this.RemovedMethods)
				sb.AppendLine($"- {method}");
			sb.AppendLine();
		}

		if (this.AddedProperties.Count > 0)
		{
			sb.AppendLine("### Added Properties:");
			foreach (var property in this.AddedProperties)
				sb.AppendLine($"- {property}");
			sb.AppendLine();
		}

		if (this.ModifiedProperties.Count > 0)
		{
			sb.AppendLine("### Modified Properties:");
			foreach (var (before, after) in this.ModifiedProperties)
				sb.AppendLine($"- Before: {before}\n  After: {after}");
			sb.AppendLine();
		}

		if (this.RemovedProperties.Count > 0)
		{
			sb.AppendLine("### Removed Properties:");
			foreach (var property in this.RemovedProperties)
				sb.AppendLine($"- {property}");
			sb.AppendLine();
		}

		if (this.AddedFields.Count > 0)
		{
			sb.AppendLine("### Added Fields:");
			foreach (var field in this.AddedFields)
				sb.AppendLine($"- {field}");
			sb.AppendLine();
		}

		if (this.ModifiedFields.Count > 0)
		{
			sb.AppendLine("### Modified Fields:");
			foreach (var (before, after) in this.ModifiedFields)
				sb.AppendLine($"- Before: {before}\n  After: {after}");
			sb.AppendLine();
		}

		if (this.RemovedFields.Count > 0)
		{
			sb.AppendLine("### Removed Fields:");
			foreach (var field in this.RemovedFields)
				sb.AppendLine($"- {field}");
			sb.AppendLine();
		}

		if (this.AddedEvents.Count > 0)
		{
			sb.AppendLine("### Added Events:");
			foreach (var @event in this.AddedEvents)
				sb.AppendLine($"- {@event}");
			sb.AppendLine();
		}

		if (this.ModifiedEvents.Count > 0)
		{
			sb.AppendLine("### Modified Events:");
			foreach (var (before, after) in this.ModifiedEvents)
				sb.AppendLine($"- Before: {before}\n  After: {after}");
			sb.AppendLine();
		}

		if (this.RemovedEvents.Count > 0)
		{
			sb.AppendLine("### Removed Events:");
			foreach (var @event in this.RemovedEvents)
				sb.AppendLine($"- {@event}");
			sb.AppendLine();
		}

		return sb.ToString().Replace("Global.", "");
	}
}
