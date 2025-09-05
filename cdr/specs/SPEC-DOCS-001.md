---
id: SPEC-DOCS-001
title: Documentation Generation Specification
status: draft
date: 2023-11-28
---

# Documentation Generation Specification

## Overview

This specification defines the documentation generation system for CycoTui, providing comprehensive API documentation, usage examples, and developer guides similar to Ratatui's docs.rs integration.

## Scope

This specification covers:

1. XML documentation generation for all public APIs
2. DocFX integration for comprehensive documentation sites
3. API reference generation and organization
4. Cross-platform browser integration for documentation preview
5. CI/CD integration for automated documentation deployment
6. Documentation quality standards and validation

## Requirements

### README Generation and Maintenance

**Automated README Generation**:
Based on Ratatui's cargo-rdme integration (`xtask/src/commands/rdme.rs`), CycoTui requires automated README generation for all library packages:

```csharp
/// <summary>
/// Manages README.md generation and validation for CycoTui packages
/// </summary>
public class ReadmeGenerationTask
{
    /// <summary>
    /// Projects that should have auto-generated README.md files from XML documentation
    /// </summary>
    /// <remarks>
    /// The main CycoTui package is excluded as it uses a hand-crafted README
    /// </remarks>
    private static readonly string[] DocumentableProjects = new[]
    {
        "CycoTui.Core",
        "CycoTui.Backend.Crossterm", 
        "CycoTui.Backend.Windows",
        "CycoTui.Backend.Unix",
        "CycoTui.Widgets",
        "CycoTui.Macros"
    };
    
    /// <summary>
    /// Generates or validates README.md files for all documentable packages
    /// </summary>
    /// <param name="checkOnly">If true, validates existing READMEs instead of generating them</param>
    /// <returns>Task representing the async operation</returns>
    public async Task ProcessReadmeFilesAsync(bool checkOnly = false)
    {
        foreach (var project in DocumentableProjects)
        {
            await ProcessProjectReadmeAsync(project, checkOnly);
        }
    }
    
    private async Task ProcessProjectReadmeAsync(string projectName, bool checkOnly)
    {
        var projectPath = $"{projectName}/{projectName}.csproj";
        
        if (!File.Exists(projectPath))
        {
            throw new FileNotFoundException($"Project file not found: {projectPath}");
        }
        
        // Extract XML documentation and generate/validate README
        if (checkOnly)
        {
            await ValidateReadmeAsync(projectName);
        }
        else
        {
            await GenerateReadmeAsync(projectName);
        }
    }
    
    private async Task ValidateReadmeAsync(string projectName)
    {
        var readmePath = $"{projectName}/README.md";
        
        if (!File.Exists(readmePath))
        {
            throw new InvalidOperationException($"README.md not found for {projectName}");
        }
        
        // Extract current XML docs and compare with existing README
        var expectedContent = await ExtractReadmeContentFromXmlDocsAsync(projectName);
        var actualContent = await File.ReadAllTextAsync(readmePath);
        
        if (!IsReadmeUpToDate(expectedContent, actualContent))
        {
            throw new InvalidOperationException(
                $"README.md for {projectName} is out of date. Run 'dotnet run --project tools readme' to update.");
        }
    }
    
    private async Task GenerateReadmeAsync(string projectName)
    {
        var readmeContent = await ExtractReadmeContentFromXmlDocsAsync(projectName);
        var readmePath = $"{projectName}/README.md";
        
        await File.WriteAllTextAsync(readmePath, readmeContent);
        Console.WriteLine($"Generated README.md for {projectName}");
    }
    
    private async Task<string> ExtractReadmeContentFromXmlDocsAsync(string projectName)
    {
        // Build project to generate XML documentation
        await RunDotnetCommand($"build {projectName} -c Release -p:GenerateDocumentationFile=true");
        
        // Load XML documentation file
        var xmlDocPath = $"{projectName}/bin/Release/net8.0/{projectName}.xml";
        
        if (!File.Exists(xmlDocPath))
        {
            throw new FileNotFoundException($"XML documentation not found: {xmlDocPath}");
        }
        
        var xmlDoc = XDocument.Load(xmlDocPath);
        
        // Extract and format documentation for README
        return GenerateReadmeContentFromXml(xmlDoc, projectName);
    }
    
    private string GenerateReadmeContentFromXml(XDocument xmlDoc, string projectName)
    {
        var sb = new StringBuilder();
        
        // Generate header
        sb.AppendLine($"# {projectName}");
        sb.AppendLine();
        
        // Extract assembly-level documentation
        var assemblyDoc = xmlDoc.Descendants("member")
            .FirstOrDefault(m => m.Attribute("name")?.Value.StartsWith("T:") == true);
            
        if (assemblyDoc?.Element("summary") != null)
        {
            sb.AppendLine(assemblyDoc.Element("summary").Value.Trim());
            sb.AppendLine();
        }
        
        // Generate API overview
        sb.AppendLine("## API Overview");
        sb.AppendLine();
        
        var publicTypes = xmlDoc.Descendants("member")
            .Where(m => m.Attribute("name")?.Value.StartsWith("T:") == true)
            .Where(m => !m.Attribute("name").Value.Contains("+")) // Exclude nested types
            .ToList();
            
        foreach (var type in publicTypes)
        {
            var typeName = ExtractTypeName(type.Attribute("name").Value);
            var summary = type.Element("summary")?.Value.Trim();
            
            if (!string.IsNullOrEmpty(summary))
            {
                sb.AppendLine($"### {typeName}");
                sb.AppendLine(summary);
                sb.AppendLine();
            }
        }
        
        // Generate usage examples
        sb.AppendLine("## Usage");
        sb.AppendLine();
        
        var examples = xmlDoc.Descendants("example").ToList();
        foreach (var example in examples.Take(3)) // Limit to first 3 examples
        {
            var code = example.Element("code")?.Value.Trim();
            if (!string.IsNullOrEmpty(code))
            {
                sb.AppendLine("```csharp");
                sb.AppendLine(code);
                sb.AppendLine("```");
                sb.AppendLine();
            }
        }
        
        // Add package information
        sb.AppendLine("## Installation");
        sb.AppendLine();
        sb.AppendLine($"```xml");
        sb.AppendLine($"<PackageReference Include=\"{projectName}\" Version=\"1.0.0\" />");
        sb.AppendLine($"```");
        sb.AppendLine();
        
        return sb.ToString();
    }
    
    private string ExtractTypeName(string memberName)
    {
        // Convert "T:CycoTui.Core.Terminal" to "Terminal"
        return memberName.Substring(2).Split('.').Last();
    }
    
    private bool IsReadmeUpToDate(string expected, string actual)
    {
        // Compare normalized content (ignoring whitespace differences)
        var normalizedExpected = NormalizeContent(expected);
        var normalizedActual = NormalizeContent(actual);
        
        return normalizedExpected == normalizedActual;
    }
    
    private string NormalizeContent(string content)
    {
        return Regex.Replace(content, @"\s+", " ").Trim();
    }
}
```

**CLI Integration**:
```csharp
[Command("readme")]
public static async Task<int> ManageReadmeFiles(
    [Option("--check")] bool checkOnly = false,
    [Option("--project")] string specificProject = null)
{
    try
    {
        var readmeTask = new ReadmeGenerationTask();
        
        if (!string.IsNullOrEmpty(specificProject))
        {
            await readmeTask.ProcessProjectReadmeAsync(specificProject, checkOnly);
        }
        else
        {
            await readmeTask.ProcessReadmeFilesAsync(checkOnly);
        }
        
        if (checkOnly)
        {
            Console.WriteLine("All README files are up to date");
        }
        else
        {
            Console.WriteLine("README files generated successfully");
        }
        
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"README operation failed: {ex.Message}");
        return 1;
    }
}
```

**Project Exclusion Strategy**:
The main CycoTui package should be excluded from automated README generation to allow for:
- Custom marketing content
- Detailed installation instructions
- Comprehensive usage examples
- Project roadmap and contribution guidelines
- Links to documentation and community resources

**CI/CD Integration**:
```yaml
- name: Validate README files
  run: dotnet run --project tools readme --check
  
- name: Generate README files (if needed)
  if: failure()
  run: |
    dotnet run --project tools readme
    git add .
    git commit -m "Update auto-generated README files"
```

### XML Documentation Standards

**Required Documentation Elements**:
```csharp
/// <summary>
/// Brief description of the class, method, or property
/// </summary>
/// <param name="parameterName">Description of parameter purpose and constraints</param>
/// <returns>Description of return value and possible states</returns>
/// <exception cref="ExceptionType">Conditions under which this exception is thrown</exception>
/// <example>
/// <code>
/// // Example usage demonstrating typical scenarios
/// var terminal = new Terminal();
/// terminal.Clear();
/// </code>
/// </example>
/// <remarks>
/// Additional notes, performance considerations, or usage guidelines
/// </remarks>
```

**Documentation Coverage Requirements**:
- All public classes, interfaces, and enums must have summary documentation
- All public methods and properties must have summary documentation
- All parameters must be documented with purpose and constraints
- All return values must be documented
- Complex types should include usage examples
- Platform-specific behavior must be documented in remarks

### DocFX Site Generation

**Site Structure**:
```
docs-site/
├── index.html                 # Main landing page
├── api/                       # Generated API reference
│   ├── CycoTui.Core.html     # Core API documentation
│   ├── CycoTui.Widgets.html  # Widget API documentation
│   └── ...
├── articles/                  # Hand-written documentation
│   ├── getting-started.html
│   ├── tutorials/
│   └── guides/
└── examples/                  # Code examples and demos
    ├── hello-world.html
    └── ...
```

**Configuration Requirements**:
```json
{
  "metadata": {
    "includePrivateMembers": false,
    "disableGitFeatures": false,
    "namespaceLayout": "flattened",
    "memberLayout": "samePage",
    "allowCompilationErrors": false
  },
  "build": {
    "template": ["default", "modern"],
    "markdownEngineName": "markdig",
    "globalMetadata": {
      "_appTitle": "CycoTui Documentation",
      "_appFooter": "CycoTui - Terminal UI Library for .NET",
      "_enableSearch": true
    }
  }
}
```

### API Reference Quality

**Required Content Standards**:
- Every public API must have clear, concise summary
- Complex APIs must include usage examples
- Performance characteristics documented where relevant
- Thread safety behavior explicitly stated
- Platform-specific behavior clearly marked
- Breaking change history maintained

**Code Example Standards**:
```csharp
/// <example>
/// This example demonstrates basic terminal usage:
/// <code>
/// using CycoTui;
/// 
/// // Create and configure terminal
/// using var terminal = new Terminal();
/// 
/// // Render content
/// terminal.Draw(frame => {
///     var paragraph = new Paragraph("Hello, World!")
///         .Alignment(Alignment.Center)
///         .Style(Style.Default.Foreground(Color.Blue));
///     frame.RenderWidget(paragraph, frame.Size);
/// });
/// </code>
/// </example>
```

### Cross-Platform Browser Integration

**Platform-Specific Launch Mechanisms**:
```csharp
public static async Task OpenDocumentationAsync(string path)
{
    var fullPath = Path.GetFullPath(path);
    var uri = new Uri(fullPath).ToString();
    
    try
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            await LaunchWindowsBrowserAsync(uri);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            await LaunchMacBrowserAsync(uri);
        }
        else
        {
            await LaunchLinuxBrowserAsync(uri);
        }
    }
    catch (Exception ex)
    {
        // Fallback: Display URL for manual opening
        Console.WriteLine($"Unable to open browser automatically: {ex.Message}");
        Console.WriteLine($"Please open: {uri}");
    }
}

private static async Task LaunchWindowsBrowserAsync(string uri)
{
    var processInfo = new ProcessStartInfo(uri)
    {
        UseShellExecute = true,
        Verb = "open"
    };
    Process.Start(processInfo);
}

private static async Task LaunchMacBrowserAsync(string uri)
{
    var processInfo = new ProcessStartInfo("open", uri)
    {
        UseShellExecute = false
    };
    await Process.Start(processInfo).WaitForExitAsync();
}

private static async Task LaunchLinuxBrowserAsync(string uri)
{
    var processInfo = new ProcessStartInfo("xdg-open", uri)
    {
        UseShellExecute = false
    };
    await Process.Start(processInfo).WaitForExitAsync();
}
```

## Technical Approach

### Documentation Pipeline

**Multi-Stage Generation Process**:
1. **Validation Stage**: Ensure all public APIs have required documentation
2. **XML Generation Stage**: Build projects with XML documentation enabled
3. **Processing Stage**: Process XML files and generate metadata
4. **Site Generation Stage**: Use DocFX to generate comprehensive site
5. **Verification Stage**: Validate generated documentation and check links

### Integration with Build System

**MSBuild Integration**:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <DocumentationFile>bin\$(Configuration)\$(TargetFramework)\$(AssemblyName).xml</DocumentationFile>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <WarningsAsErrors />
    <WarningsNotAsErrors>CS1591</WarningsNotAsErrors> <!-- Missing XML documentation -->
</PropertyGroup>

<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
    <GenerateDocumentationFile>false</GenerateDocumentationFile>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
</PropertyGroup>
```

**CI/CD Integration**:
```yaml
# GitHub Actions example
- name: Generate Documentation
  run: dotnet run --project tools/CycoTui.DevTools -- docs
  
- name: Deploy Documentation
  if: github.ref == 'refs/heads/main'
  uses: peaceiris/actions-gh-pages@v3
  with:
    github_token: ${{ secrets.GITHUB_TOKEN }}
    publish_dir: ./docs-site
```

### Quality Assurance

**Documentation Validation**:
- Automated checks for missing XML documentation
- Link validation within generated documentation
- Example code compilation verification
- Accessibility compliance checking

**Continuous Integration**:
- Documentation generation must succeed in CI
- Generated documentation deployed automatically on releases
- Breaking changes in API must update documentation
- Documentation coverage reporting

## Performance Targets

- Documentation generation should complete within 5 minutes for full site
- Generated site should load quickly with proper caching headers
- Search functionality should provide responsive results
- Browser opening should occur within 2 seconds of command completion

## See Also

- [SPEC-BUILD-001.md](SPEC-BUILD-001.md): Build system integration
- [BUILD-DOCS-GENERATION-001](../tasks/BUILD-DOCS-GENERATION-001/README.md): Implementation task