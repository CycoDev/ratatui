# Documentation Generation System

## Overview

Implement a comprehensive documentation generation system for CycoTui that mirrors Ratatui's docs command functionality. This includes XML documentation generation, DocFX site generation, API reference creation, and cross-platform browser integration.

## Implementation Approach

**Multi-Phase Documentation Pipeline**:
1. **XML Documentation Generation**: Generate XML docs for all projects with proper warning handling
2. **Documentation Testing**: Test documentation across all packages with feature matrix validation
3. **DocFX Site Generation**: Create comprehensive documentation site with API reference
4. **API Reference Generation**: Generate detailed API documentation similar to docs.rs
5. **README Generation and Validation**: Auto-generate README.md files from XML documentation for library packages
6. **Browser Integration**: Cross-platform browser opening for immediate preview

**Key Components**:
- `DocumentationGenerationTask` class for orchestrating the entire process
- `DocumentationTestingTask` class for validating documentation across feature matrices
- `ReadmeGenerationTask` class for managing README.md files from XML documentation
- CLI command integration for developer workflow
- DocFX configuration for professional documentation site
- Cross-platform browser launching with proper fallbacks

## Key Challenges

**Feature Matrix Documentation Testing**:
- .NET lacks a direct equivalent to Rust's cargo-hack for testing documentation with different feature combinations
- Need to implement custom logic for testing backend-specific packages with various feature sets
- Must handle platform-specific testing (Windows vs Unix backends) appropriately
- Documentation testing should validate both XML doc generation and actual doc content

**Cross-Platform Documentation Consistency**:
- Ensure documentation works correctly across Windows, Linux, and macOS
- Handle platform-specific API documentation differences
- Validate that examples in documentation work on target platforms

**Cross-Platform Browser Opening**:
- Windows: Use `UseShellExecute = true` with Process.Start
- macOS: Use `open` command
- Linux: Use `xdg-open` command
- Provide graceful fallback with manual URL display

**Multi-Project Coordination**:
- Coordinate documentation generation across multiple packages
- Handle build dependencies and ensure proper project order
- Manage XML documentation warnings without breaking builds

**Documentation Tool Integration**:
- Integrate DocFX for comprehensive site generation
- Consider dotnet-api-docs or similar tools for API reference
- Implement XML-to-README generation similar to cargo-rdme functionality
- Ensure compatibility with CI/CD environments

**README Generation Strategy**:
- Extract XML documentation and convert to markdown format
- Maintain list of packages that should have auto-generated READMEs
- Exclude main package to allow hand-crafted marketing content
- Support both generation and validation modes for CI/CD

## Related Components

- `SPEC-BUILD-001.md`: Overall build system specification
- CLI tooling infrastructure for development commands
- MSBuild integration for documentation properties
- Package organization system for multi-project documentation

## Integration Points

**CLI Integration**:
```csharp
[Command("docs")]
public static async Task<int> GenerateDocumentation(
    [Option("--open")] bool openInBrowser = false)

[Command("readme")]
public static async Task<int> ManageReadmeFiles(
    [Option("--check")] bool checkOnly = false,
    [Option("--project")] string specificProject = null)
```

**MSBuild Integration**:
```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <DocumentationFile>bin\$(Configuration)\$(TargetFramework)\$(AssemblyName).xml</DocumentationFile>
</PropertyGroup>
```

**Configuration Files**:
- `docfx.json` for DocFX configuration
- `.editorconfig` for documentation style guidelines
- MSBuild targets for documentation generation

## Acceptance Criteria

- [ ] XML documentation generated for all public APIs
- [ ] Documentation testing validates XML docs across all packages and feature combinations
- [ ] Backend-specific documentation tested appropriately for each platform
- [ ] DocFX site generated with proper navigation and styling
- [ ] API reference documentation accessible and comprehensive
- [ ] README.md files auto-generated for library packages from XML documentation
- [ ] README generation validation mode for CI/CD integration
- [ ] Main package README excluded from auto-generation to allow custom content
- [ ] Cross-platform browser opening works on Windows, macOS, and Linux
- [ ] CLI command `docs` generates documentation successfully
- [ ] CLI command `docs --open` opens browser with generated documentation
- [ ] CLI command `test-docs` validates documentation across feature matrices
- [ ] CLI command `readme` generates README files from XML docs
- [ ] CLI command `readme --check` validates README files are up-to-date
- [ ] Documentation generation works in CI/CD environments
- [ ] Documentation testing integrated into CI/CD pipeline
- [ ] Warning handling allows development builds while maintaining quality
- [ ] Documentation includes examples and usage guidance
- [ ] Generated documentation is accessible and professional-looking

## See Also

- [SPEC-BUILD-001.md](../../specs/SPEC-BUILD-001.md): Build system specification with documentation requirements
- [SETUP-DEV-TOOLS-001](../SETUP-DEV-TOOLS-001/README.md): Development tools setup including documentation tools