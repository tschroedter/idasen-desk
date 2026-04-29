# .NET Version Upgrade to .NET 10.0

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: .NET 10.0 (LTS)

## Upgrade Options
**Source**: .github/upgrades/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

## Strategy
**Selected**: All-at-Once
**Rationale**: 2 projects, both already on .NET 10.0, all packages compatible, package update only

### Execution Constraints
- Single atomic operation — all packages updated together
- Validate full solution build after package updates
- Fix all compilation errors and warnings in a single pass
- Run full test suite after build succeeds

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-to-net10
- **Commit Strategy**: Single Commit at End
