# Changelog Automation

This repository uses automated changelog generation via GitHub Actions to maintain a consistent and up-to-date record of all changes.

## How It Works

The changelog automation consists of two workflows:

### 1. Release Drafter (`release-drafter.yml`)

This workflow automatically:
- Creates and updates draft releases
- Generates release notes based on merged Pull Requests
- Categorizes changes using PR labels
- Runs on every push to `main` and when PRs are opened/updated

### 2. Update Changelog (`update-changelog.yml`)

This workflow automatically:
- Updates the `CHANGELOG.md` file when a release is published
- Adds the release notes to the changelog in Keep a Changelog format
- Commits the updated changelog back to the repository

## Using PR Labels

To ensure your changes are properly categorized in the changelog, add one of these labels to your Pull Requests:

| Label | Category | Description |
|-------|----------|-------------|
| `feature`, `enhancement` | 🚀 Features | New features and enhancements |
| `fix`, `bugfix`, `bug` | 🐛 Bug Fixes | Bug fixes |
| `chore`, `maintenance` | 🧰 Maintenance | Maintenance tasks, dependencies |
| `documentation`, `docs` | 📚 Documentation | Documentation changes |
| `security` | 🔒 Security | Security-related changes |

### Version Bumping

You can also control the version bump by adding these labels:

- `major` - Bumps major version (e.g., 1.0.0 → 2.0.0)
- `minor` - Bumps minor version (e.g., 1.0.0 → 1.1.0)
- `patch` - Bumps patch version (e.g., 1.0.0 → 1.0.1) - **default**

## Workflow

1. **Create a Pull Request** with your changes
2. **Add appropriate labels** to categorize your changes
3. **Merge the PR** to main
4. The Release Drafter workflow automatically updates the draft release
5. When ready, **publish the release** from GitHub Releases
6. The Update Changelog workflow automatically updates `CHANGELOG.md`

## Manual Triggers

Both workflows can be manually triggered from the Actions tab if needed:
1. Go to Actions → Select the workflow
2. Click "Run workflow" → Select branch → Run workflow

## Viewing the Changelog

The changelog is available in multiple places:
- **CHANGELOG.md** in the repository root
- **GitHub Releases** page with formatted release notes
- **Draft releases** (visible to maintainers) before publication

## Configuration

The changelog generation is configured in `.github/release-drafter.yml`. You can customize:
- Categories and their labels
- Template for release notes
- Version resolution strategy
- Change entry format

## Best Practices

1. **Write clear PR titles** - They become the changelog entries
2. **Always add labels** - Helps categorize changes correctly
3. **Keep PRs focused** - One feature/fix per PR makes tracking easier
4. **Review draft releases** - Check generated notes before publishing
5. **Edit release notes** - Feel free to enhance auto-generated notes before publishing
