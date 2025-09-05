# Development Tools and Build System

## Overview

Implement a development tooling system for CycoTui similar to Ratatui's xtask system, providing automated build, test, lint, and CI capabilities for .NET development.

## Implementation Approach

Create a set of development tools that provide:

1. **Build Commands**: Comprehensive building with different configurations
2. **Testing Suite**: Multi-layered testing (unit, integration, cross-platform)
3. **Linting Pipeline**: Code quality checks (formatting, static analysis, documentation)
4. **Code Formatting**: Automated code formatting for C# and configuration files
5. **CI Integration**: Automated CI pipeline with early failure detection

### .NET Tooling Strategy

- Use **dotnet CLI** as the primary build tool (equivalent to cargo)
- Implement custom tooling using **System.CommandLine** for command parsing
- Use **MSBuild** targets for complex build scenarios
- Integrate **GitHub Actions** or **Azure DevOps** for CI
- Use **dotnet format** for C# code formatting (equivalent to rustfmt)
- Use external tools for configuration file formatting (equivalent to taplo for TOML)

### Command Structure

Implement commands similar to xtask structure:
- `build` - Build all targets and configurations
- `test` - Run comprehensive test suite
- `lint` - Run all code quality checks
- `format` - Code formatting checks/fixes for C# and configuration files
- `coverage` - Generate code coverage reports
- `docs` - Documentation generation and validation

### Code Formatting Implementation

Based on analysis of `xtask/src/commands/format.rs`:

```csharp
[Command("format")]
public class FormatCommand : ICommand
{
    [Option("--check", Description = "Check formatting without making changes")]
    public bool CheckOnly { get; set; } = false;
    
    public async Task<int> ExecuteAsync()
    {
        var exitCode = 0;
        
        // Format C# code using dotnet format
        exitCode = Math.Max(exitCode, await FormatCSharpAsync());
        
        // Format configuration files (JSON, XML, etc.)
        exitCode = Math.Max(exitCode, await FormatConfigurationFilesAsync());
        
        return exitCode;
    }
    
    private async Task<int> FormatCSharpAsync()
    {
        var args = new List<string> { "format", "--verbosity", "minimal" };
        
        if (CheckOnly)
        {
            args.Add("--verify-no-changes");
        }
        
        return await ProcessRunner.RunAsync("dotnet", args);
    }
    
    private async Task<int> FormatConfigurationFilesAsync()
    {
        // Handle JSON, XML, and other configuration files
        // Equivalent to taplo formatting in Rust version
        var configFiles = GetConfigurationFiles();
        
        foreach (var file in configFiles)
        {
            await FormatFileAsync(file);
        }
        
        return 0;
    }
}

## Key Challenges

1. **Cross-Platform Testing**: Ensure tools work on Windows, macOS, and Linux
2. **External Dependencies**: Manage dependencies on external tools (analyzers, formatters)
3. **Performance**: Optimize build and test execution times
4. **Maintainability**: Keep tooling scripts simple and maintainable

## Related Components

- Project structure and build configuration
- CI/CD pipeline configuration
- Testing infrastructure
- Documentation generation system

## Integration Points

- MSBuild project files and Directory.Build.props
- GitHub Actions or Azure DevOps workflows  
- NuGet package generation and publishing
- Code coverage and quality reporting tools

## Testing Approach

- Test development tools themselves for reliability
- Validate tools work across different environments
- Ensure tools integrate properly with IDE workflows
- Test CI pipeline configuration and execution

## Acceptance Criteria

- [ ] Development tooling provides equivalent functionality to Ratatui's xtask system
- [ ] Commands support common development workflows (build, test, lint, format)
- [ ] Tools work consistently across Windows, macOS, and Linux
- [ ] CI pipeline integrates with chosen platform (GitHub Actions/Azure DevOps)
- [ ] Documentation includes setup and usage instructions for development tools
- [ ] Performance is comparable to or better than manual execution of individual tools

## See Also

- SETUP-PROJ-STRUCTURE-001: Project organization and build configuration
- SETUP-CI-PIPELINE-001: Continuous integration setup (to be created)
- Source analysis: xtask/src/commands.rs - Build command orchestration pattern