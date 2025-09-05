---
id: SPEC-ARCH-001
title: CycoTui Architecture Specification
status: draft
date: 2023-11-28
---

# CycoTui Architecture Specification

## Overview

This specification defines the overall architecture and modular organization of CycoTui, based on the modular approach used by Ratatui while adapted for .NET ecosystem conventions.

## Scope

This specification covers:
- Package structure and organization
- Module/namespace hierarchy
- Dependency management strategy
- API stability approach
- Platform targeting strategy

## Requirements

### Package Structure
- **Primary Package**: `CycoTui` - Complete terminal UI library for applications
- **Core Package**: `CycoTui.Core` - Stable foundation for widget library authors
- **Backend Packages**: Platform-specific implementations (optional)

### Module Organization
- **CycoTui.Core.Backend**: Terminal backend abstraction
- **CycoTui.Core.Buffer**: Rendering buffer and cell model
- **CycoTui.Core.Layout**: Layout engine and constraints
- **CycoTui.Core.Style**: Styling and color system
- **CycoTui.Core.Symbols**: Drawing symbols and characters
- **CycoTui.Core.Terminal**: Terminal management
- **CycoTui.Core.Text**: Text handling and rendering
- **CycoTui.Core.Widgets**: Widget interfaces and base classes

### API Stability Strategy
- **CycoTui.Core**: Maximum stability, breaking changes only in major versions
- **CycoTui**: Can evolve more freely, adding convenience features and widgets
- **Clear Interface Boundaries**: Core interfaces remain stable while implementations can evolve

### Platform Targeting
- **Primary Target**: .NET 8.0 for modern applications
- **Compatibility Target**: .NET Standard 2.0 for broader ecosystem compatibility
- **Package Strategy**: Multi-target packages when possible

## Technical Approach

### Namespace Hierarchy
```csharp
CycoTui.Core
├── Backend
├── Buffer
├── Layout
├── Style
├── Symbols
├── Terminal
├── Text
└── Widgets

CycoTui
├── Widgets (built-in widgets)
├── Extensions (convenience methods)
└── Applications (application helpers)
```

### Dependency Management
- CycoTui.Core: Minimal dependencies, no UI framework dependencies
- CycoTui: Can depend on CycoTui.Core and additional convenience libraries
- Clear separation prevents unnecessary dependencies in widget libraries

### Conditional Compilation
- Use conditional compilation for platform-specific code
- Feature flags for optional dependencies
- Runtime capability detection where appropriate

## Platform-Specific Details

### .NET Framework Compatibility
- .NET Standard 2.0 ensures compatibility with .NET Framework 4.6.1+
- P/Invoke declarations compatible across .NET implementations

### .NET Core/5+ Optimizations
- Use Span<T> and Memory<T> for performance-critical buffer operations
- Modern async patterns for input handling
- Native interop improvements

## See Also

- VISION-TECH-002: Technical Vision
- SPEC-BACKEND-001: Backend Abstraction
- SETUP-PROJ-STRUCTURE-001: Project Structure Task