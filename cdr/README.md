# Current Design Records (CDR)

This folder contains all active, approved design documentation for the CycoTui project.

## Purpose

Current Design Records (CDRs) are the authoritative, approved documents that guide implementation. They represent the current state of the project design and are the source of truth for development.

## Contents

- `/vision/` - Vision documents describing the "why" and "what" of the project
- `/features/` - User-facing functionality specifications
- `/roadmap/` - Development timeline and phases
- `/tasks/` - Implementation units for development
- `/specs/` - Technical specifications and requirements

## Status Conventions

CDR documents use these status indicators:
- `draft` - Document is in progress, not yet approved
- `review` - Document is complete and awaiting approval
- `approved` - Document is approved and considered authoritative
- `superseded` - Document has been replaced by a newer version

## About CycoTui

CycoTui is a C# port of the Ratatui terminal UI library, designed to provide:

1. Feature parity with Ratatui while following C# idioms and conventions
2. A powerful, immediate-mode terminal UI framework for .NET applications
3. Cross-platform terminal manipulation capabilities
4. Efficient, buffer-based rendering with minimal terminal I/O

The project aims to bring Ratatui's elegant design to the .NET ecosystem while ensuring the API feels natural to C# developers.