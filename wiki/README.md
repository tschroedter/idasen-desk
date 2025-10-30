# Idasen Desk Controller Wiki

This directory contains the source files for the project's GitHub Wiki documentation.

## About This Directory

The `wiki/` directory contains markdown files that serve as the source for the project's GitHub Wiki. These files are maintained in the repository for version control and easy updates.

## Wiki Structure

### User Documentation
- **[Home.md](Home.md)** - Wiki landing page and navigation
- **[Getting-Started.md](Getting-Started.md)** - Installation and setup guide
- **[User-Guide.md](User-Guide.md)** - Complete usage instructions
- **[Troubleshooting.md](Troubleshooting.md)** - Common issues and solutions
- **[FAQ.md](FAQ.md)** - Frequently asked questions
- **[Configuration.md](Configuration.md)** - Advanced configuration options
- **[Release-Notes.md](Release-Notes.md)** - Version history

### Developer Documentation
- **[Developer-Guide.md](Developer-Guide.md)** - Contributing guidelines
- **[Build-Instructions.md](Build-Instructions.md)** - How to build from source
- **[Architecture.md](Architecture.md)** - Technical architecture (TODO)
- **[Testing-Guide.md](Testing-Guide.md)** - Testing guidelines (TODO)
- **[API-Reference.md](API-Reference.md)** - API documentation (TODO)

## Viewing the Wiki

### On GitHub

The wiki is available at:
**https://github.com/tschroedter/idasen-desk/wiki**

### Locally

You can read the markdown files directly in this directory using any markdown viewer or text editor.

## Contributing to the Wiki

### Making Changes

1. **Edit Files in This Directory**
   - Modify the markdown files in `wiki/`
   - Follow the existing formatting style
   - Update links if changing page titles

2. **Submit Pull Request**
   - Commit your changes
   - Push to your fork
   - Create a pull request
   - Add `documentation` or `docs` label

3. **Wiki Sync** (Maintainers Only)
   - After PR merge, maintainers sync to GitHub Wiki
   - Changes appear on the wiki

### Style Guidelines

1. **Formatting**
   - Use standard markdown syntax
   - Include navigation footer on each page
   - Add table of contents for long pages

2. **Links**
   - Use relative links between wiki pages: `[Page](Page-Name)`
   - Use full URLs for external links
   - Use absolute GitHub URLs for repository files

3. **Images**
   - Store images in `docs/images/`
   - Reference with full GitHub URLs
   - Include alt text for accessibility

4. **Code Blocks**
   - Use language-specific syntax highlighting
   - Include comments for clarity
   - Show full commands with context

## Wiki Pages Overview

### Home.md
- Entry point for all wiki users
- Quick navigation to all sections
- Project overview and status
- Getting help resources

### Getting-Started.md
- Installation methods (direct download, Scoop)
- Prerequisites and requirements
- Pairing desk with Windows
- First launch configuration
- Quick configuration guide

### User-Guide.md
- System tray interface
- Context menu options
- Settings window (all tabs)
- Controlling the desk
- Hotkeys and best practices
- Complete feature documentation

### Troubleshooting.md
- Connection issues
- Pairing problems
- Movement issues
- Application issues
- Performance problems
- Log file access
- Getting help section

### FAQ.md
- General questions
- Compatibility questions
- Installation questions
- Usage questions
- Feature questions
- Technical questions
- Contributing questions
- License questions

### Configuration.md
- Configuration file location
- All configuration options
- Example configurations
- Advanced scenarios
- Troubleshooting config

### Release-Notes.md
- Latest release highlights
- Recent release summaries
- Version history links
- Upgrade guide
- Breaking changes
- Deprecation notices

### Developer-Guide.md
- Getting started with development
- Project structure
- Building and testing
- Code style guidelines
- Contributing workflow
- Pull request process
- Common development tasks

### Build-Instructions.md
- Prerequisites
- Getting source code
- Command line builds
- IDE builds (Visual Studio, VS Code)
- Publishing executables
- Running tests
- Troubleshooting builds

## Maintenance

### Regular Updates

Wiki should be updated when:
- ✅ New features are added
- ✅ Configuration options change
- ✅ New releases are published
- ✅ Installation process changes
- ✅ Common issues are identified

### Review Schedule

- Review quarterly for accuracy
- Update after major releases
- Check links for broken references
- Ensure screenshots are current

### Outdated Content

If you find outdated information:
1. Create an issue describing the problem
2. Or submit a PR with corrections
3. Tag with `documentation` label

## Syncing to GitHub Wiki

### For Maintainers

To sync repository wiki files to GitHub Wiki:

```bash
# Clone wiki repository
git clone https://github.com/tschroedter/idasen-desk.wiki.git

# Copy files from main repo
cp wiki/*.md idasen-desk.wiki/

# Commit and push
cd idasen-desk.wiki
git add .
git commit -m "Update wiki from main repository"
git push
```

### Automation (Future)

Consider automating wiki sync with GitHub Actions:
- Trigger on push to `wiki/` directory
- Automatically sync to GitHub Wiki
- Maintain single source of truth

## File Naming Conventions

- Use `PascalCase` for file names: `Getting-Started.md`
- Match GitHub Wiki URL format: `Getting-Started` → `Getting-Started.md`
- Hyphens for multi-word pages
- `.md` extension for all files

## Template for New Pages

```markdown
# Page Title

Brief description of what this page covers.

## Table of Contents

- [Section 1](#section-1)
- [Section 2](#section-2)

## Section 1

Content here...

## Section 2

Content here...

---

**Navigation**: [Home](Home) | [Related-Page](Related-Page) | [Another-Page](Another-Page)
```

## Resources

### Markdown
- [GitHub Markdown Guide](https://guides.github.com/features/mastering-markdown/)
- [Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet)

### Documentation
- [Write the Docs](https://www.writethedocs.org/)
- [Documentation Guide](https://www.writethedocs.org/guide/)

### Project Docs
- [Main README](../README.md)
- [Contributing Guidelines](../.github/pull_request_template.md)
- [Copilot Instructions](../.github/copilot-instructions.md)

## Questions?

- Check [FAQ](FAQ.md)
- Ask in [Discussions](https://github.com/tschroedter/idasen-desk/discussions)
- Open an [Issue](https://github.com/tschroedter/idasen-desk/issues)

---

**Last Updated:** October 2024
