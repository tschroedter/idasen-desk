# Changelog Automation Workflow

This diagram illustrates how the automated changelog system works in the idasen-desk repository.

## Workflow Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         CONTRIBUTOR WORKFLOW                             │
└─────────────────────────────────────────────────────────────────────────┘

  1. Create PR                 2. Add Labels              3. Merge PR
  ┌──────────┐                ┌──────────┐               ┌──────────┐
  │  Create  │───────────────>│   Add    │──────────────>│  Merge   │
  │    PR    │                │  Labels  │               │  to main │
  │          │                │          │               │          │
  └──────────┘                └──────────┘               └──────────┘
       │                            │                          │
       │                            │                          │
       v                            v                          v
  [feature]                   [Select from:]              [Automatic]
  [bugfix]                    • feature                   [CI runs]
  [docs]                      • fix/bugfix/bug
                              • chore/maintenance
                              • docs/documentation
                              • security
                              • major/minor/patch

                                        │
                                        │
                                        v
┌─────────────────────────────────────────────────────────────────────────┐
│                    RELEASE DRAFTER WORKFLOW                              │
│                   (.github/workflows/release-drafter.yml)                │
└─────────────────────────────────────────────────────────────────────────┘

  Triggers:                        Process:                    Output:
  ┌──────────┐                    ┌──────────┐               ┌──────────┐
  │ Push to  │                    │ Generate │               │  Draft   │
  │   main   │───────────────────>│ Release  │──────────────>│ Release  │
  │          │                    │  Notes   │               │  Notes   │
  └──────────┘                    └──────────┘               └──────────┘
  ┌──────────┐                         │                          │
  │ PR Open/ │                         │                          │
  │  Update  │                         v                          v
  └──────────┘               [Categorize by label]      [Update GitHub Release]
  ┌──────────┐               [Group changes]            [Show in Releases tab]
  │ Manual   │               [Calculate version]
  │ Trigger  │
  └──────────┘

                                        │
                                        │
                                        v
┌─────────────────────────────────────────────────────────────────────────┐
│                         MAINTAINER ACTION                                │
└─────────────────────────────────────────────────────────────────────────┘

  4. Review Draft              5. Edit (Optional)         6. Publish Release
  ┌──────────┐                ┌──────────┐               ┌──────────┐
  │  Review  │───────────────>│   Edit   │──────────────>│ Publish  │
  │  Draft   │                │  Notes   │               │ Release  │
  │          │                │          │               │          │
  └──────────┘                └──────────┘               └──────────┘
       │                            │                          │
       v                            v                          v
  [Check content]            [Enhance/Fix]             [Make public]
  [Verify version]           [Add details]             [Create tag]
  [Review changes]           [Format]                  [Notify users]

                                        │
                                        │
                                        v
┌─────────────────────────────────────────────────────────────────────────┐
│                   UPDATE CHANGELOG WORKFLOW                              │
│                  (.github/workflows/update-changelog.yml)                │
└─────────────────────────────────────────────────────────────────────────┘

  Trigger:                         Process:                   Output:
  ┌──────────┐                    ┌──────────┐               ┌──────────┐
  │ Release  │                    │  Update  │               │ Updated  │
  │Published │───────────────────>│CHANGELOG │──────────────>│CHANGELOG │
  │          │                    │   .md    │               │   .md    │
  └──────────┘                    └──────────┘               └──────────┘
  ┌──────────┐                         │                          │
  │ Manual   │                         │                          │
  │ Trigger  │                         v                          v
  └──────────┘               [Extract release notes]    [Commit to main]
                             [Format for changelog]      [Push changes]
                             [Add to CHANGELOG.md]        [Skip CI]

                                        │
                                        │
                                        v
┌─────────────────────────────────────────────────────────────────────────┐
│                              RESULT                                      │
└─────────────────────────────────────────────────────────────────────────┘

  ┌──────────────────────────────────────────────────────────────┐
  │                     CHANGELOG.md                              │
  │  ┌────────────────────────────────────────────────────────┐  │
  │  │ # Changelog                                            │  │
  │  │                                                         │  │
  │  │ ## [Unreleased]                                        │  │
  │  │                                                         │  │
  │  │ ## [1.0.0] - 2024-10-07                                │  │
  │  │                                                         │  │
  │  │ ### 🚀 Features                                        │  │
  │  │ - Add new feature X @contributor (#123)                │  │
  │  │                                                         │  │
  │  │ ### 🐛 Bug Fixes                                       │  │
  │  │ - Fix issue with Y @developer (#124)                   │  │
  │  │                                                         │  │
  │  │ ### 📚 Documentation                                   │  │
  │  │ - Update README @maintainer (#125)                     │  │
  │  └────────────────────────────────────────────────────────┘  │
  └──────────────────────────────────────────────────────────────┘

  ┌──────────────────────────────────────────────────────────────┐
  │                  GitHub Releases Page                         │
  │  ┌────────────────────────────────────────────────────────┐  │
  │  │ V1.0.0 - Latest Release                                │  │
  │  │ Published on Oct 7, 2024                               │  │
  │  │                                                         │  │
  │  │ Changes:                                               │  │
  │  │ - Add new feature X @contributor (#123)                │  │
  │  │ - Fix issue with Y @developer (#124)                   │  │
  │  │ - Update README @maintainer (#125)                     │  │
  │  │                                                         │  │
  │  │ [Download Assets]                                      │  │
  │  └────────────────────────────────────────────────────────┘  │
  └──────────────────────────────────────────────────────────────┘
```

## Key Benefits

1. **Automated** - No manual changelog updates needed
2. **Consistent** - Same format for all releases
3. **Traceable** - Every change linked to its PR and author
4. **Categorized** - Changes grouped by type (features, fixes, etc.)
5. **Version-aware** - Automatic version bumping based on labels

## Label Categories

| Category | Labels | Icon |
|----------|--------|------|
| Features | `feature`, `enhancement` | 🚀 |
| Bug Fixes | `fix`, `bugfix`, `bug` | 🐛 |
| Maintenance | `chore`, `maintenance` | 🧰 |
| Documentation | `documentation`, `docs` | 📚 |
| Security | `security` | 🔒 |

## Version Bumping

| Label | Version Change | Example |
|-------|----------------|---------|
| `major` | Breaking changes | 1.0.0 → 2.0.0 |
| `minor` | New features | 1.0.0 → 1.1.0 |
| `patch` | Bug fixes (default) | 1.0.0 → 1.0.1 |

## References

- Full documentation: [docs/CHANGELOG_AUTOMATION.md](CHANGELOG_AUTOMATION.md)
- Setup guide: [docs/IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)
- Label creation script: [scripts/create-labels.sh](../scripts/create-labels.sh)
- PR template: [.github/pull_request_template.md](../.github/pull_request_template.md)
