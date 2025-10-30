# Wiki Setup Instructions for Maintainers

This document provides instructions for maintainers on how to publish the wiki files to GitHub Wiki.

## What Was Created

A comprehensive Wiki has been created in the `wiki/` directory with the following pages:

### User Documentation (7 pages)
1. **Home.md** - Landing page with navigation
2. **Getting-Started.md** - Installation and setup guide
3. **User-Guide.md** - Complete usage instructions
4. **Troubleshooting.md** - Common issues and solutions
5. **FAQ.md** - Frequently asked questions
6. **Configuration.md** - Advanced configuration options
7. **Release-Notes.md** - Version history and changelog

### Developer Documentation (6 pages)
1. **Developer-Guide.md** - Contributing guidelines
2. **Build-Instructions.md** - Building from source
3. **Architecture.md** - Technical architecture overview
4. **Testing-Guide.md** - Testing guidelines and best practices
5. **API-Reference.md** - Internal API documentation
6. **README.md** - Wiki maintenance guide

## Publishing to GitHub Wiki

### Option 1: Manual Sync (Recommended for First Time)

1. **Clone the Wiki Repository**
   ```bash
   git clone https://github.com/tschroedter/idasen-desk.wiki.git
   cd idasen-desk.wiki
   ```

2. **Copy Wiki Files**
   ```bash
   # From the main repository directory
   cp ../idasen-desk/wiki/*.md .
   
   # Don't copy README.md to wiki (it's for repo use only)
   rm README.md
   ```

3. **Commit and Push**
   ```bash
   git add .
   git commit -m "Initial wiki creation with comprehensive documentation"
   git push origin master
   ```

4. **Verify**
   - Visit: https://github.com/tschroedter/idasen-desk/wiki
   - Check all pages are present and render correctly
   - Test internal links

### Option 2: Automated Sync with GitHub Actions

For future updates, you can create a GitHub Action to automatically sync wiki files.

Create `.github/workflows/sync-wiki.yml`:

```yaml
name: Sync Wiki

on:
  push:
    branches:
      - main
    paths:
      - 'wiki/**'
  workflow_dispatch:

jobs:
  sync:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout main repo
        uses: actions/checkout@v4

      - name: Checkout wiki repo
        uses: actions/checkout@v4
        with:
          repository: ${{ github.repository }}.wiki
          path: wiki-repo

      - name: Copy wiki files
        run: |
          cp wiki/*.md wiki-repo/
          rm wiki-repo/README.md  # Don't sync the maintenance guide
          cd wiki-repo

      - name: Commit and push
        run: |
          cd wiki-repo
          git config user.name "github-actions[bot]"
          git config user.email "github-actions[bot]@users.noreply.github.com"
          git add .
          git diff --quiet && git diff --staged --quiet || (git commit -m "Sync wiki from main repository" && git push)
```

## Updating the Wiki

### After Merging This PR

1. Manually sync the wiki using Option 1 above
2. Optionally set up automated sync using Option 2
3. Review all pages on GitHub Wiki
4. Test all internal links
5. Check formatting and images

### For Future Updates

**If using manual sync:**
1. Update wiki files in `wiki/` directory
2. Create PR and merge
3. Manually sync to GitHub Wiki

**If using automated sync:**
1. Update wiki files in `wiki/` directory
2. Create PR and merge
3. GitHub Action automatically syncs to Wiki

## Wiki Structure

```
wiki/
├── Home.md                      # Landing page
├── Getting-Started.md           # Installation guide
├── User-Guide.md                # Usage documentation
├── Troubleshooting.md           # Problem solving
├── FAQ.md                       # Common questions
├── Configuration.md             # Advanced config
├── Release-Notes.md             # Version history
├── Developer-Guide.md           # Contributing
├── Build-Instructions.md        # Building
├── Architecture.md              # Technical design
├── Testing-Guide.md             # Testing
├── API-Reference.md             # API docs
└── README.md                    # This maintenance guide
```

## Navigation Structure

The wiki uses a consistent navigation footer on each page:

```markdown
**Navigation**: [Home](Home) | [Page1](Page1) | [Page2](Page2)
```

This creates a breadcrumb-style navigation for easy movement between pages.

## Internal Linking

Links between wiki pages use GitHub Wiki format:
- `[Page Title](Page-Name)` - Links to another wiki page
- `[External](https://example.com)` - Links to external URL
- `[File](../docs/file.md)` - Links to repository file (use full GitHub URL)

## Images

Images are stored in `docs/images/` in the main repository and referenced using full GitHub URLs:

```markdown
![Alt Text](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/image.png)
```

## Maintaining the Wiki

### Regular Maintenance

- **Review quarterly** - Check for outdated information
- **After releases** - Update version-specific content
- **After major changes** - Update affected documentation
- **Check links** - Ensure no broken links
- **Update screenshots** - Keep visuals current

### Adding New Pages

1. Create new `.md` file in `wiki/` directory
2. Follow existing page structure
3. Add to Home.md navigation
4. Add navigation footer
5. Test locally
6. Create PR
7. Sync to GitHub Wiki after merge

### Updating Existing Pages

1. Edit the file in `wiki/` directory
2. Maintain consistent formatting
3. Update "Last Updated" date if present
4. Test locally
5. Create PR
6. Sync to GitHub Wiki after merge

## Benefits of This Approach

✅ **Version Control** - Wiki content is version controlled in main repo
✅ **PR Review** - Changes reviewed before publishing
✅ **Single Source** - Wiki files are source of truth
✅ **Easy Updates** - Update in main repo, sync to wiki
✅ **Backup** - Wiki backed up in main repository
✅ **Consistency** - Same process as code changes

## Testing Before Publishing

Before syncing to GitHub Wiki:

1. **Check Markdown Rendering**
   - Use GitHub preview or markdown viewer
   - Verify all formatting is correct
   - Check tables, code blocks, lists

2. **Test Internal Links**
   - Ensure links point to correct pages
   - Use correct GitHub Wiki link format
   - Test on published wiki after first sync

3. **Verify Images**
   - All images load correctly
   - Use correct GitHub raw URLs
   - Images are appropriately sized

4. **Review Content**
   - Check for typos and grammar
   - Ensure technical accuracy
   - Verify code examples work

## Troubleshooting

### Links Not Working on Wiki

- GitHub Wiki uses different link format than repository
- Use `[Text](Page-Name)` not `[Text](Page-Name.md)`
- Test on published wiki to verify

### Images Not Displaying

- Ensure using full GitHub URLs
- Use `main` branch in URL, not `master`
- Check image file exists in repository

### Formatting Issues

- Check markdown syntax
- GitHub Wiki may render slightly differently than repository
- Test on published wiki to verify

## Next Steps

1. ✅ PR with wiki files has been merged
2. ⏳ Manually sync wiki files to GitHub Wiki
3. ⏳ Review published wiki pages
4. ⏳ Test all links and images
5. ⏳ Optionally set up automated sync
6. ⏳ Announce wiki availability to users

## Resources

- [GitHub Wiki Documentation](https://docs.github.com/en/communities/documenting-your-project-with-wikis)
- [Markdown Guide](https://www.markdownguide.org/)
- [Wiki Maintenance Guide](wiki/README.md) - In-repo wiki documentation

---

**Created:** October 2024
**Status:** Ready for publishing
