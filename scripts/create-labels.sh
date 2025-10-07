#!/bin/bash

# Script to create PR labels for changelog automation
# Usage: ./create-labels.sh [owner/repo]
# Example: ./create-labels.sh tschroedter/idasen-desk
#
# Note: Requires GitHub CLI (gh) to be installed and authenticated
# Install: https://cli.github.com/

set -e

REPO="${1:-tschroedter/idasen-desk}"

echo "Creating PR labels for changelog automation in $REPO..."
echo

# Check if gh CLI is installed
if ! command -v gh &> /dev/null; then
    echo "Error: GitHub CLI (gh) is not installed."
    echo "Please install it from https://cli.github.com/"
    exit 1
fi

# Check if authenticated
if ! gh auth status &> /dev/null; then
    echo "Error: Not authenticated with GitHub CLI."
    echo "Please run: gh auth login"
    exit 1
fi

# Feature labels
echo "Creating feature labels..."
gh label create "feature" --description "New feature" --color "0E8A16" --repo "$REPO" 2>/dev/null || echo "  - 'feature' already exists"
gh label create "enhancement" --description "Enhancement to existing feature" --color "A2EEEF" --repo "$REPO" 2>/dev/null || echo "  - 'enhancement' already exists"

# Bug fix labels
echo "Creating bug fix labels..."
gh label create "fix" --description "Bug fix" --color "D73A4A" --repo "$REPO" 2>/dev/null || echo "  - 'fix' already exists"
gh label create "bugfix" --description "Bug fix" --color "D73A4A" --repo "$REPO" 2>/dev/null || echo "  - 'bugfix' already exists"
gh label create "bug" --description "Bug" --color "D73A4A" --repo "$REPO" 2>/dev/null || echo "  - 'bug' already exists"

# Maintenance labels
echo "Creating maintenance labels..."
gh label create "chore" --description "Maintenance task" --color "FEF2C0" --repo "$REPO" 2>/dev/null || echo "  - 'chore' already exists"
gh label create "maintenance" --description "Maintenance" --color "FEF2C0" --repo "$REPO" 2>/dev/null || echo "  - 'maintenance' already exists"

# Documentation labels
echo "Creating documentation labels..."
gh label create "documentation" --description "Documentation" --color "0075CA" --repo "$REPO" 2>/dev/null || echo "  - 'documentation' already exists"
gh label create "docs" --description "Documentation" --color "0075CA" --repo "$REPO" 2>/dev/null || echo "  - 'docs' already exists"

# Security labels
echo "Creating security labels..."
gh label create "security" --description "Security" --color "EE0701" --repo "$REPO" 2>/dev/null || echo "  - 'security' already exists"

# Version bump labels
echo "Creating version bump labels..."
gh label create "major" --description "Major version bump" --color "B60205" --repo "$REPO" 2>/dev/null || echo "  - 'major' already exists"
gh label create "minor" --description "Minor version bump" --color "FBCA04" --repo "$REPO" 2>/dev/null || echo "  - 'minor' already exists"
gh label create "patch" --description "Patch version bump" --color "BFD4F2" --repo "$REPO" 2>/dev/null || echo "  - 'patch' already exists"

echo
echo "✅ Label creation complete!"
echo
echo "You can view all labels at: https://github.com/$REPO/labels"
