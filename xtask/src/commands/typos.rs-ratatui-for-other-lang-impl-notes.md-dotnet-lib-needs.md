## xtask\src\commands\typos.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: xtask command to run/fix typos via external `typos` CLI (developer tooling, not core TUI).
- Run external commands: use System.Diagnostics.Process or CliWrap (NuGet) for robust cross-platform execution/quoting.
- CLI args: replace clap with System.CommandLine (Microsoft) or CommandLineParser.
- Error reports: use Serilog for structured logs and Spectre.Console for rich terminal error output.
- Spellchecking: no native "typos" in .NET — options: call the Rust `typos` CLI, use Hunspell bindings (NHunspell, Hunspell.NetCore) or NetSpell for in-process checks.
- Packaging xtask: implement as a dotnet global tool or simple dotnet project invoked from scripts.
- CI integration: use dotnet-format, Roslyn analyzers, and GitHub Actions to mirror Rust CI checks.
- Cross-platform notes: handle PATH, shell differences, and quoting; CliWrap and System.CommandLine help abstract these.
- Dev tooling parity: wire up formatters, linters, and fixers as dotnet tools or scripts callable from CI.
- Command interface: define a common ICommand/IRunnable interface like Run trait for consistent tooling commands.

