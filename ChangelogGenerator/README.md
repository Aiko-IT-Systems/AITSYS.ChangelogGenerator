# AITSYS.ChangelogGenerator

A simple automatic changelog generator

```powershell
# Install Locally
dotnet tool install --global --add-source ChangelogGenerator/bin/Release/ AITSYS.ChangelogGenerator

# Update Locally
dotnet tool update --global --add-source ChangelogGenerator/bin/Release/ AITSYS.ChangelogGenerator

# Install
dotnet tool install --global AITSYS.ChangelogGenerator --version 1.0.1

# Update
dotnet tool update --global AITSYS.ChangelogGenerator --version 1.0.1

# Uninstall
dotnet tool uninstall --global AITSYS.ChangelogGenerator

# Publish
Get-ChildItem ./ChangelogGenerator/bin/Release/*.nupkg | ForEach-Object { dotnet nuget push $_.FullName -k $env:NUGET -s https://api.nuget.org/v3/index.json --skip-duplicate }
Get-ChildItem ./ChangelogGenerator/bin/Release/*.nupkg | ForEach-Object { dotnet nuget push $_.FullName -k $env:NUGET_GITHUB -s github --skip-duplicate }
```
