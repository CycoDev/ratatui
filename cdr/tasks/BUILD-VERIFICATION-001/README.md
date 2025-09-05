# Build Verification System

## Overview

Implement a comprehensive build verification system that ensures CycoTui works correctly across all supported platforms and backend configurations, based on Ratatui's feature matrix testing approach.

## Implementation Approach

### Core Requirements

Based on analysis of `xtask/src/commands/check.rs`, implement:

1. **Feature Matrix Testing**: Test all backend combinations with various feature flags
2. **Platform-Specific Verification**: Exclude incompatible backends on each platform  
3. **Sequential and Parallel Testing**: Support both approaches for different scenarios
4. **Error Reporting**: Clear feedback when builds fail with specific configurations

### Development CLI Tool

Create a CycoTui development CLI similar to Ratatui's xtask, including linting capabilities:

```csharp
[Command("check")]
public class CheckCommand
{
    [Option("--all-features", Description = "Check all feature combinations")]
    public bool AllFeatures { get; set; }
    
    [Option("--backend", Description = "Specific backend to check")]
    public TerminalBackendType? Backend { get; set; }
    
    [Option("--fix", Description = "Apply automatic code fixes")]
    public bool Fix { get; set; }
    
    public async Task<int> ExecuteAsync()
    {
        // Run code quality checks first
        var qualityResult = await RunCodeQualityChecks();
        if (qualityResult != 0) return qualityResult;
        
        if (AllFeatures)
        {
            return await CheckAllFeatureCombinations();
        }
        else if (Backend.HasValue)
        {
            return await CheckSpecificBackend(Backend.Value);
        }
        else
        {
            return await CheckDefault();
        }
    }
    
    private async Task<int> RunCodeQualityChecks()
    {
        // Run typo checking first
        var typoResult = await RunTypoCheck();
        if (typoResult != 0) return typoResult;
        
        // Run formatting checks
        var formatResult = await RunFormattingCheck();
        if (formatResult != 0) return formatResult;
        
        // Run linting/analyzers
        return await RunLinting();
    }
    
    private async Task<int> RunTypoCheck()
    {
        // Check for typos in documentation and comments
        var typoChecker = new TypoCheckingTask();
        try
        {
            await typoChecker.RunAsync(Fix);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Typo checking failed: {ex.Message}");
            return 1;
        }
    }
    
    private async Task<int> RunLinting()
    {
        // Equivalent to clippy with complex feature handling
        var linter = new CodeQualityRunner();
        
        // Run analyzers on backend packages with specific features
        foreach (var backend in SupportedBackends)
        {
            var features = GetBackendFeatures(backend);
            var result = await linter.RunAnalyzers($"CycoTui.{backend}", features, Fix);
            if (result != 0) return result;
        }
        
        // Run analyzers on all other packages with all features
        return await linter.RunAnalyzers("workspace", allFeatures: true, Fix);
    }
    
    private async Task<int> RunFormattingCheck()
    {
        // Check code formatting (equivalent to rustfmt check)
        var formatCommand = new FormatCommand { CheckOnly = !Fix };
        return await formatCommand.ExecuteAsync();
    }
}

[Command("lint")]
public class LintCommand
{
    [Option("--fix", Description = "Apply automatic fixes")]
    public bool Fix { get; set; }
    
    public async Task<int> ExecuteAsync()
    {
        var runner = new CodeQualityRunner();
        return await runner.RunComprehensiveLinting(Fix);
    }
}
```

### MSBuild Integration

Integrate verification into the build process:

```xml
<Target Name="VerifyBackendCompatibility" BeforeTargets="Build">
  <PropertyGroup>
    <SelectedBackend Condition="'$(TerminalBackend)' == ''">Crossterm</SelectedBackend>
    <SelectedBackend Condition="'$(TerminalBackend)' != ''">$(TerminalBackend)</SelectedBackend>
  </PropertyGroup>
  
  <!-- Platform validation -->
  <Error Text="Unix backend cannot be used on Windows" 
         Condition="'$(SelectedBackend)' == 'Unix' AND '$(OS)' == 'Windows_NT'" />
  <Error Text="Windows backend can only be used on Windows" 
         Condition="'$(SelectedBackend)' == 'Windows' AND '$(OS)' != 'Windows_NT'" />
         
  <Message Text="Building with $(SelectedBackend) backend" Importance="high" />
</Target>
```

## Key Challenges

### Platform Detection and Exclusion

- **Challenge**: Automatically exclude incompatible backends during build
- **Solution**: Use runtime information and MSBuild conditions
- **Implementation**: Mirror Ratatui's conditional compilation approach

### Feature Combination Testing

- **Challenge**: Test all valid combinations without running invalid ones
- **Solution**: Create a compatibility matrix and iterate through valid combinations
- **Implementation**: Use parameterized tests with platform guards

## Related Components

- **SPEC-BUILD-001.md**: Main build system specification  
- **SPEC-BACKEND-001.md**: Backend interface definitions
- **BUILD-BACKEND-SELECTION-001**: Backend selection mechanisms

## Integration Points

### CI/CD Integration

```yaml
# GitHub Actions example
- name: Lint Code Quality
  run: dotnet run --project CycoTui.DevCli -- lint
  
- name: Check All Features
  run: dotnet run --project CycoTui.DevCli -- check --all-features
  
- name: Check Specific Backend  
  run: dotnet run --project CycoTui.DevCli -- check --backend Crossterm

# Quality gates similar to clippy's warning-as-errors approach
- name: Quality Gate
  run: |
    dotnet run --project CycoTui.DevCli -- lint
    if [ $? -ne 0 ]; then
      echo "Code quality issues detected. Build failed."
      exit 1
    fi
```

### Development Workflow

1. **Pre-commit Checks**: Run basic verification before commits
2. **CI Verification**: Comprehensive feature matrix testing in CI
3. **Release Validation**: Full platform and feature testing before releases

## Testing Approach

### Unit Tests for Verification Logic

```csharp
[TestFixture]
public class BuildVerificationTests
{
    [Test]
    public void ShouldExcludeIncompatibleBackends()
    {
        var verifier = new BuildVerifier();
        var configs = verifier.GetValidConfigurations();
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.That(configs.Any(c => c.Backend == TerminalBackendType.Unix), Is.False);
        }
        else
        {
            Assert.That(configs.Any(c => c.Backend == TerminalBackendType.Windows), Is.False);
        }
    }
}
```

### Integration Tests

```csharp
[TestFixture]
public class BuildIntegrationTests
{
    [Test]
    [TestCase(TerminalBackendType.Crossterm)]
    [TestCase(TerminalBackendType.Windows, Platform = "Win")]
    [TestCase(TerminalBackendType.Unix, Platform = "Unix")]
    public async Task ShouldBuildWithBackend(TerminalBackendType backend)
    {
        if (!BackendCompatibility.IsSupported(backend))
        {
            Assert.Ignore($"Backend {backend} not supported on this platform");
        }
        
        var result = await BuildWithBackend(backend);
        Assert.That(result.ExitCode, Is.EqualTo(0));
    }
}
```

## Acceptance Criteria

1. **CLI Tool Created**: Development CLI with check command functionality
2. **Feature Matrix Testing**: Ability to test all valid backend/feature combinations  
3. **Platform Validation**: Automatic exclusion of incompatible backends
4. **Error Reporting**: Clear error messages for build failures
5. **CI Integration**: GitHub Actions workflow using the verification system
6. **MSBuild Integration**: Build-time validation and backend selection
7. **Documentation**: Clear instructions for using the verification system

## See Also

- [SPEC-BUILD-001.md](../specs/SPEC-BUILD-001.md): Build system specification
- [BUILD-BACKEND-SELECTION-001](BUILD-BACKEND-SELECTION-001/README.md): Backend selection task
- [SETUP-DEV-TOOLS-001](SETUP-DEV-TOOLS-001/README.md): Development tooling setup