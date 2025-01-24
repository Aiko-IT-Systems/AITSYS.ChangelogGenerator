using System;
using System.Collections.Generic;
using System.Linq;

using AITSYS.ChangelogGenerator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AITSYS.ChangelogGenerator.Services;

public sealed class RoslynService
{
	public static ChangelogEntry AnalyzeUnifiedDiff(DiffContext diffContext)
	{
		var oldTree = CSharpSyntaxTree.ParseText(string.Join("\n", diffContext.OldContent));
		var newTree = CSharpSyntaxTree.ParseText(string.Join("\n", diffContext.NewContent));

		var oldRoot = oldTree.GetRoot();
		var newRoot = newTree.GetRoot();

		var changelogEntry = new ChangelogEntry();

		Console.WriteLine("Analyzing changes..");
		AnalyzeSyntaxChanges(oldRoot, newRoot, changelogEntry);
		Console.WriteLine("Done analyzing changes");

		return changelogEntry;
	}

	private static void AnalyzeSyntaxChanges(SyntaxNode oldRoot, SyntaxNode newRoot, ChangelogEntry changelogEntry)
	{
		Console.WriteLine("Analyzing namespace changes");
		AnalyzeNamespaceChanges(oldRoot, newRoot, changelogEntry);
		Console.WriteLine("Analyzing using changes");
		AnalyzeUsingChanges(oldRoot, newRoot, changelogEntry);
		Console.WriteLine("Analyzing type changes");
		AnalyzeTypeChanges(oldRoot, newRoot, changelogEntry);
		Console.WriteLine("Analyzing method changes");
		AnalyzeMethodChanges(oldRoot, newRoot, changelogEntry);
		Console.WriteLine("Analyzing property changes");
		AnalyzePropertyChanges(oldRoot, newRoot, changelogEntry);
		Console.WriteLine("Analyzing field changes");
		AnalyzeFieldChanges(oldRoot, newRoot, changelogEntry);
		Console.WriteLine("Analyzing event changes");
		AnalyzeEventChanges(oldRoot, newRoot, changelogEntry);
		Console.WriteLine("Analyzing enum changes");
		AnalyzeEnumChanges(oldRoot, newRoot, changelogEntry);
	}

	private static void AnalyzeNamespaceChanges(SyntaxNode oldRoot, SyntaxNode newRoot, ChangelogEntry changelogEntry)
	{
		var oldNamespaces = oldRoot.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();
		var newNamespaces = newRoot.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();

		foreach (var ns in newNamespaces)
		{
			var oldNamespace = oldNamespaces.FirstOrDefault(n => n.Name.ToString() == ns.Name.ToString());

			if (oldNamespace == null)
				changelogEntry.AddedNamespaces.Add($"{ns.Name}");
			else if (!ns.IsEquivalentTo(oldNamespace))
				changelogEntry.ModifiedNamespaces.Add(($"{oldNamespace.Name}", $"{ns.Name}"));
		}

		foreach (var ns in oldNamespaces.Where(ns => newNamespaces.All(n => n.Name.ToString() != ns.Name.ToString())))
			changelogEntry.RemovedNamespaces.Add($"{ns.Name}");
	}

	private static void AnalyzeUsingChanges(SyntaxNode oldRoot, SyntaxNode newRoot, ChangelogEntry changelogEntry)
	{
		var oldUsings = oldRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().Select(u => u.Name.ToString()).ToList();
		var newUsings = newRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().Select(u => u.Name.ToString()).ToList();

		foreach (var @using in newUsings.Except(oldUsings))
			changelogEntry.AddedUsings.Add(@using);

		foreach (var @using in oldUsings.Except(newUsings))
			changelogEntry.RemovedUsings.Add(@using);
	}

	private static void AnalyzeTypeChanges(SyntaxNode oldRoot, SyntaxNode newRoot, ChangelogEntry changelogEntry)
	{
		var oldTypes = oldRoot.DescendantNodes().OfType<TypeDeclarationSyntax>().ToList();
		var newTypes = newRoot.DescendantNodes().OfType<TypeDeclarationSyntax>().ToList();

		foreach (var type in newTypes)
		{
			var oldType = oldTypes.FirstOrDefault(t => t.Identifier.Text == type.Identifier.Text);

			if (oldType == null)
				changelogEntry.AddedTypes.Add($"{GetFullTypeName(type)}");
			else if (!type.IsEquivalentTo(oldType) && !ElementMoved(oldType, type))
				changelogEntry.ModifiedTypes.Add(($"{GetFullTypeName(oldType)}", $"{GetFullTypeName(type)}"));
		}

		foreach (var type in oldTypes.Where(type => newTypes.All(t => t.Identifier.Text != type.Identifier.Text)))
			changelogEntry.RemovedTypes.Add($"{GetFullTypeName(type)}");
	}

	private static void AnalyzeMethodChanges(SyntaxNode oldRoot, SyntaxNode newRoot, ChangelogEntry changelogEntry)
	{
		var oldMethods = oldRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
		var newMethods = newRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

		foreach (var method in newMethods)
		{
			var oldMethod = oldMethods.FirstOrDefault(m => m.Identifier.Text == method.Identifier.Text);

			if (oldMethod == null)
				changelogEntry.AddedMethods.Add($"{GetMethodSignatureWithContext(method)}");
			else if (!method.IsEquivalentTo(oldMethod) && !ElementMoved(oldMethod, method))
				changelogEntry.ModifiedMethods.Add(($"{GetMethodSignatureWithContext(oldMethod)}", $"{GetMethodSignatureWithContext(method)}"));
		}

		foreach (var method in oldMethods.Where(method => newMethods.All(m => m.Identifier.Text != method.Identifier.Text)))
			changelogEntry.RemovedMethods.Add($"{GetMethodSignatureWithContext(method)}");
	}

	private static void AnalyzePropertyChanges(SyntaxNode oldRoot, SyntaxNode newRoot, ChangelogEntry changelogEntry)
	{
		var oldProperties = oldRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();
		var newProperties = newRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

		foreach (var property in newProperties)
		{
			var oldProperty = oldProperties.FirstOrDefault(p => p.Identifier.Text == property.Identifier.Text);

			if (oldProperty == null)
				changelogEntry.AddedProperties.Add($"{GetPropertySignatureWithContext(property)}");
			else if (!property.IsEquivalentTo(oldProperty) && !ElementMoved(oldProperty, property))
				changelogEntry.ModifiedProperties.Add(($"{GetPropertySignatureWithContext(oldProperty)}", $"{GetPropertySignatureWithContext(property)}"));
		}

		foreach (var property in oldProperties.Where(property => newProperties.All(p => p.Identifier.Text != property.Identifier.Text)))
			changelogEntry.RemovedProperties.Add($"{GetPropertySignatureWithContext(property)}");
	}

	private static void AnalyzeFieldChanges(SyntaxNode oldRoot, SyntaxNode newRoot, ChangelogEntry changelogEntry)
	{
		var oldFields = oldRoot.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
		var newFields = newRoot.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();

		foreach (var field in newFields)
		foreach (var variable in field.Declaration.Variables)
		{
			var oldField = oldFields.FirstOrDefault(f =>
				f.Declaration.Variables.Any(v => v.Identifier.Text == variable.Identifier.Text));

			if (oldField == null)
				changelogEntry.AddedFields.Add($"{GetFullFieldSignatureWithContext(field, variable.Identifier.Text)}");
			else if (!field.IsEquivalentTo(oldField) && !ElementMoved(oldField, field))
				changelogEntry.ModifiedFields.Add(($"{GetFullFieldSignatureWithContext(oldField, variable.Identifier.Text)}", $"{GetFullFieldSignatureWithContext(field, variable.Identifier.Text)}"));
		}

		foreach (var field in oldFields)
		foreach (var variable in field.Declaration.Variables.Where(variable => newFields.All(f =>
			f.Declaration.Variables.All(v => v.Identifier.Text != variable.Identifier.Text))))
			changelogEntry.RemovedFields.Add($"{GetFullFieldSignatureWithContext(field, variable.Identifier.Text)}");
	}

	private static void AnalyzeEventChanges(SyntaxNode oldRoot, SyntaxNode newRoot, ChangelogEntry changelogEntry)
	{
		var oldEventFields = oldRoot.DescendantNodes().OfType<EventFieldDeclarationSyntax>().ToList();
		var newEventFields = newRoot.DescendantNodes().OfType<EventFieldDeclarationSyntax>().ToList();

		var oldEventDeclarations = oldRoot.DescendantNodes().OfType<EventDeclarationSyntax>().ToList();
		var newEventDeclarations = newRoot.DescendantNodes().OfType<EventDeclarationSyntax>().ToList();

		var oldProperties = oldRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();
		var newProperties = newRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

		AnalyzeEventFields(oldEventFields, newEventFields, changelogEntry);
		AnalyzeEventDeclarations(oldEventDeclarations, newEventDeclarations, changelogEntry);
		AnalyzePropertyEvents(oldProperties, newProperties, changelogEntry);
	}

	private static void AnalyzeEventFields(List<EventFieldDeclarationSyntax> oldEvents, List<EventFieldDeclarationSyntax> newEvents, ChangelogEntry changelogEntry)
	{
		foreach (var @event in newEvents)
		{
			var oldEvent = oldEvents.FirstOrDefault(e => GetEventFieldSignatureWithContext(e) == GetEventFieldSignatureWithContext(@event));

			if (oldEvent == null)
				changelogEntry.AddedEvents.Add($"{GetEventFieldSignatureWithContext(@event)}");
			else if (!@event.IsEquivalentTo(oldEvent))
			{
				if (ElementMoved(oldEvent, @event))
					continue;

				changelogEntry.ModifiedEvents.Add(($"{GetEventFieldSignatureWithContext(oldEvent)}", $"{GetEventFieldSignatureWithContext(@event)}"));
			}
		}

		foreach (var @event in oldEvents.Where(e => newEvents.All(ne => GetEventFieldSignatureWithContext(ne) != GetEventFieldSignatureWithContext(e))))
			changelogEntry.RemovedEvents.Add($"{GetEventFieldSignatureWithContext(@event)}");
	}

	private static void AnalyzeEventDeclarations(List<EventDeclarationSyntax> oldEvents, List<EventDeclarationSyntax> newEvents, ChangelogEntry changelogEntry)
	{
		foreach (var @event in newEvents)
		{
			var oldEvent = oldEvents.FirstOrDefault(e => GetEventDeclarationSignatureWithContext(e) == GetEventDeclarationSignatureWithContext(@event));

			if (oldEvent == null)
				changelogEntry.AddedEvents.Add($"{GetEventDeclarationSignatureWithContext(@event)}");
			else if (!@event.IsEquivalentTo(oldEvent))
			{
				if (ElementMoved(oldEvent, @event))
					continue;

				changelogEntry.ModifiedEvents.Add(($"{GetEventDeclarationSignatureWithContext(oldEvent)}", $"{GetEventDeclarationSignatureWithContext(@event)}"));
			}
		}

		foreach (var @event in oldEvents.Where(e => newEvents.All(ne => GetEventDeclarationSignatureWithContext(ne) != GetEventDeclarationSignatureWithContext(e))))
			changelogEntry.RemovedEvents.Add($"{GetEventDeclarationSignatureWithContext(@event)}");
	}

	private static void AnalyzePropertyEvents(List<PropertyDeclarationSyntax> oldProperties, List<PropertyDeclarationSyntax> newProperties, ChangelogEntry changelogEntry)
	{
		foreach (var property in newProperties)
		{
			var oldProperty = oldProperties.FirstOrDefault(p => GetPropertySignatureWithContext(p) == GetPropertySignatureWithContext(property));

			if (oldProperty == null && IsEventProperty(property))
				changelogEntry.AddedEvents.Add($"{GetPropertySignatureWithContext(property)}");
			else if (oldProperty != null && !property.IsEquivalentTo(oldProperty) && IsEventProperty(property))
			{
				if (ElementMoved(oldProperty, property))
					continue;

				changelogEntry.ModifiedEvents.Add(($"{GetPropertySignatureWithContext(oldProperty)}", $"{GetPropertySignatureWithContext(property)}"));
			}
		}

		foreach (var property in oldProperties.Where(p => newProperties.All(np => GetPropertySignatureWithContext(np) != GetPropertySignatureWithContext(p))))
			if (IsEventProperty(property))
				changelogEntry.RemovedEvents.Add($"{GetPropertySignatureWithContext(property)}");
	}

	private static void AnalyzeEnumChanges(SyntaxNode oldRoot, SyntaxNode newRoot, ChangelogEntry changelogEntry)
	{
		var oldEnums = oldRoot.DescendantNodes().OfType<EnumDeclarationSyntax>().ToList();
		var newEnums = newRoot.DescendantNodes().OfType<EnumDeclarationSyntax>().ToList();

		foreach (var newEnum in newEnums)
		{
			var oldEnum = oldEnums.FirstOrDefault(e => e.Identifier.Text == newEnum.Identifier.Text);

			if (oldEnum == null)
				changelogEntry.AddedEnums.Add($"{GetEnumSignatureWithContext(newEnum)}");
			else
			{
				if (!newEnum.IsEquivalentTo(oldEnum))
				{
					var oldValues = oldEnum.Members.Select(m => m.Identifier.Text).ToList();
					var newValues = newEnum.Members.Select(m => m.Identifier.Text).ToList();

					var addedValues = newValues.Except(oldValues).ToList();
					var removedValues = oldValues.Except(newValues).ToList();

					if (addedValues.Count > 0 || removedValues.Count > 0)
					{
						changelogEntry.ModifiedEnums.Add(($"{GetEnumSignatureWithContext(oldEnum)}", $"{GetEnumSignatureWithContext(newEnum)}"));

						foreach (var addedValue in addedValues)
							changelogEntry.AddedEnumValues.Add($"{GetEnumSignatureWithContext(newEnum)}.{addedValue}");

						foreach (var removedValue in removedValues)
							changelogEntry.RemovedEnumValues.Add($"{GetEnumSignatureWithContext(oldEnum)}.{removedValue}");
					}
				}
			}
		}

		foreach (var oldEnum in oldEnums.Where(e => newEnums.All(ne => ne.Identifier.Text != e.Identifier.Text)))
			changelogEntry.RemovedEnums.Add($"{GetEnumSignatureWithContext(oldEnum)}");
	}

	private static string GetEventFieldSignatureWithContext(EventFieldDeclarationSyntax @event)
	{
		var className = GetClassName(@event);
		var namespaceName = GetNamespace(@event);
		var identifier = @event.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
		return $"{namespaceName}.{className}.{identifier}";
	}

	private static string GetEventDeclarationSignatureWithContext(EventDeclarationSyntax @event)
	{
		var className = GetClassName(@event);
		var namespaceName = GetNamespace(@event);
		var identifier = @event.Identifier.Text;
		return $"{namespaceName}.{className}.{identifier}";
	}

	private static string GetClassName(SyntaxNode node)
	{
		var typeDeclaration = node.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().FirstOrDefault();

		if (typeDeclaration != null)
			return typeDeclaration.Identifier.Text;

		var namespaceDeclaration = node.AncestorsAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
		if (namespaceDeclaration != null)
			return namespaceDeclaration.Name.ToString();

		return "Global";
	}

	private static string GetEnumSignatureWithContext(EnumDeclarationSyntax enumDeclaration)
	{
		var namespaceName = GetNamespace(enumDeclaration);
		var className = GetClassName(enumDeclaration);

		return $"{namespaceName}.{className}.{enumDeclaration.Identifier.Text}";
	}

	private static bool IsEventProperty(PropertyDeclarationSyntax property)
		=> property.Type.ToString().Contains("AsyncEvent") || property.Type.ToString().Contains("EventHandler") || property.Type.ToString().Contains("AsyncEventHandler");

	private static bool ElementMoved(SyntaxNode oldElement, SyntaxNode newElement) =>
		oldElement.GetLocation().SourceSpan != newElement.GetLocation().SourceSpan;

	private static string GetMethodSignatureWithContext(MethodDeclarationSyntax method)
	{
		var className = GetClassName(method);
		var namespaceName = GetNamespace(method);
		return $"{method.ReturnType} {namespaceName}.{className}.{method.Identifier.Text}({string.Join(", ", method.ParameterList.Parameters.Select(p => $"{p.Type} {p.Identifier}"))})";
	}

	private static string GetPropertySignatureWithContext(PropertyDeclarationSyntax property)
	{
		var className = GetClassName(property);
		var namespaceName = GetNamespace(property);
		return $"{property.Type} {namespaceName}.{className}.{property.Identifier.Text} {{ get; set; }}";
	}

	private static string GetFullFieldSignatureWithContext(FieldDeclarationSyntax field, string variableName)
	{
		var className = GetClassName(field);
		var namespaceName = GetNamespace(field);
		return $"{field.Declaration.Type} {namespaceName}.{className}.{variableName};";
	}

	private static string GetFullTypeName(TypeDeclarationSyntax type)
	{
		var namespaceName = GetNamespace(type);
		return $"{namespaceName}.{type.Identifier.Text}";
	}

	private static string GetNamespace(SyntaxNode node)
	{
		var namespaceDeclaration = node.AncestorsAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
		return namespaceDeclaration?.Name.ToString() ?? "Global";
	}
}
