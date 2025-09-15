# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is the Ratatui repository - a Rust library for building terminal user interfaces (TUIs). The repository also contains a `cdr/` directory with comprehensive documentation for CycoTui, a planned C# port of Ratatui.

## Repository Structure

### Ratatui (Rust Library)
The project uses a modular workspace structure introduced in v0.30.0:

- `ratatui/` - Main crate that re-exports all functionality
- `ratatui-core/` - Core types and traits (stable API for widget libraries)
- `ratatui-widgets/` - Built-in widget implementations
- `ratatui-crossterm/` - Crossterm backend implementation
- `ratatui-termion/` - Termion backend (Unix only)
- `ratatui-termwiz/` - Termwiz backend
- `ratatui-macros/` - Declarative macros for common patterns
- `xtask/` - Build automation and development tools
- `examples/` - Example applications demonstrating library usage

### CycoTui Documentation (C# Port Planning)
- `cdr/` - Current Design Records for the CycoTui C# port
  - `vision/` - High-level vision and goals
  - `features/` - User-facing functionality specifications
  - `roadmap/` - Development phases and timeline
  - `tasks/` - Implementation task definitions
  - `specs/` - Technical specifications
  - `file-analyses/` - Systematic analysis of Ratatui source files

## Common Development Commands

### Building and Testing
```bash
# Build the project (requires Rust toolchain)
cargo build

# Run tests via xtask automation
cargo xtask test          # Run all tests
cargo xtask test-libs     # Run library tests only
cargo xtask test-docs     # Run doc tests

# Check code without building
cargo xtask check
cargo xtask check --all-features

# Backend-specific checks
cargo xtask check-backend crossterm
cargo xtask check-backend termion
cargo xtask check-backend termwiz
```

### Code Quality
```bash
# Run linting and formatting
cargo xtask lint          # Run all lints
cargo xtask format        # Check formatting
cargo xtask clippy        # Run clippy lints
cargo xtask typos         # Check for typos

# Documentation
cargo xtask docs          # Build documentation
cargo xtask docs --open   # Build and open docs

# CI pipeline (runs all checks)
cargo xtask ci
```

### Testing Individual Components
```bash
# Run tests for specific crates
cargo test -p ratatui-core
cargo test -p ratatui-widgets
cargo test -p ratatui-crossterm

# Run specific test
cargo test test_name

# Run tests with specific backend
cargo test --no-default-features --features crossterm
```

## Architecture

### Modular Design (v0.30.0+)
Ratatui uses a modular architecture for better compilation times and API stability:

1. **ratatui-core**: Stable foundation containing:
   - Widget traits (`Widget`, `StatefulWidget`)
   - Buffer and cell management
   - Layout engine and constraints
   - Style and color system
   - Text rendering primitives

2. **ratatui-widgets**: Built-in widgets like:
   - Block, Paragraph, List, Table
   - Chart, Sparkline, Gauge
   - Canvas for custom drawing
   - Tabs, Calendar, and more

3. **Backend System**: Pluggable terminal backends:
   - Abstractions allow different terminal libraries
   - Each backend implements the `Backend` trait
   - Support for Crossterm, Termion, and Termwiz

### Key Design Patterns

- **Immediate-mode rendering**: Widgets are functions that render to a buffer
- **Double buffering**: Efficient updates by computing diffs between frames
- **Constraint-based layout**: Flexible positioning using the `Layout` system
- **Builder pattern**: Fluent APIs for constructing widgets
- **Zero-cost abstractions**: Performance-focused design

## CycoTui (C# Port) Context

The `cdr/` directory contains comprehensive documentation for porting Ratatui to C#:

- **Goal**: Create a feature-complete C# port following .NET idioms
- **Approach**: Systematic analysis of Ratatui source to ensure complete coverage
- **Architecture**: Two-tier structure with CycoTui.Core (stable) and CycoTui (applications)
- **Target**: .NET 8.0 primary, .NET Standard 2.0 for compatibility

Key implementation phases:
1. SETUP: Project structure and build system
2. CORE: Buffer, backend, and rendering infrastructure
3. LAYOUT: Layout engine and constraints
4. WIDGET: Widget implementations
5. INPUT: Event handling system
6. EXTEND: Advanced features and optimizations

## Important Notes

- The project forbids unsafe code (`unsafe_code = "forbid"` in Cargo.toml)
- Minimum Rust version: 1.85.0
- Uses conventional commits for version management
- Comprehensive test coverage expected for all changes
- The repository includes extensive documentation and examples