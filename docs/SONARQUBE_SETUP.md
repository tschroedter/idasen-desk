# SonarQube Static Code Analysis Setup

## Overview

This repository includes a dedicated GitHub Actions workflow for running SonarQube static code analysis on the C# codebase. The workflow is configured to run automatically on pushes to the main branch and on pull requests.

## Workflow Details

- **Workflow File**: `.github/workflows/sonarqube.yml`
- **Trigger Events**:
  - Push to `main` branch
  - Pull request events (opened, synchronize, reopened)
  - Manual trigger via `workflow_dispatch`
- **Platform**: Windows latest (required for .NET 8.0 Windows-specific projects)

## Setup Requirements

To enable SonarQube analysis, you need to configure the following repository secrets:

### Required Secrets

1. **SONAR_TOKEN**
   - Description: Authentication token for SonarQube/SonarCloud
   - Where to get it:
     - **SonarCloud**: Go to [SonarCloud](https://sonarcloud.io/) → My Account → Security → Generate Token
     - **Self-hosted SonarQube**: Go to your SonarQube instance → User → My Account → Security → Generate Token

2. **SONAR_HOST_URL**
   - Description: URL of your SonarQube server
   - Examples:
     - SonarCloud: `https://sonarcloud.io`
     - Self-hosted: `https://sonarqube.yourdomain.com`

### Adding Secrets to GitHub

1. Go to your repository on GitHub
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add both `SONAR_TOKEN` and `SONAR_HOST_URL`

## SonarCloud Setup (Recommended for Open Source)

SonarCloud is free for open-source projects and provides excellent integration with GitHub:

1. **Sign up for SonarCloud**
   - Go to [SonarCloud](https://sonarcloud.io/)
   - Sign in with your GitHub account
   - Import your organization

2. **Create a new project**
   - Click **Analyze new project**
   - Select `tschroedter/idasen-desk`
   - Choose **GitHub Actions** as the analysis method

3. **Get your tokens**
   - SonarCloud will provide you with:
     - Project key (default: `idasen-desk`)
     - Organization key
     - Token (generate under Security)

4. **Configure the workflow**
   - The workflow is already configured with project key `idasen-desk`
   - If your project key is different, update the `/k:` parameter in `.github/workflows/sonarqube.yml`

5. **Add secrets to GitHub**
   - Add `SONAR_TOKEN` with the generated token
   - Add `SONAR_HOST_URL` as `https://sonarcloud.io`

## Self-Hosted SonarQube Setup

If you're using a self-hosted SonarQube instance:

1. **Create a project** in your SonarQube instance
   - Project key: `idasen-desk` (or update the workflow file)

2. **Generate a token**
   - Go to User → My Account → Security
   - Generate a new token with appropriate permissions

3. **Add secrets to GitHub**
   - Add `SONAR_TOKEN` with the generated token
   - Add `SONAR_HOST_URL` with your SonarQube server URL

## Workflow Behavior

### With Secrets Configured

When `SONAR_TOKEN` and `SONAR_HOST_URL` are properly configured:
1. The workflow restores NuGet packages
2. Begins SonarQube analysis
3. Builds the solution
4. Completes SonarQube analysis and uploads results

### Without Secrets Configured

If secrets are not configured:
- The workflow will skip the analysis gracefully
- A warning message will be displayed
- The workflow will not fail (exit code 0)
- This allows the workflow to be committed without requiring immediate setup

## Customization

### Changing the Project Key

To change the SonarQube project key, edit `.github/workflows/sonarqube.yml`:

```yaml
dotnet sonarscanner begin /k:"your-project-key" /d:sonar.host.url="${env:SONAR_HOST_URL}" /d:sonar.login="${env:SONAR_TOKEN}"
```

### Adding SonarQube Parameters

You can add additional SonarQube parameters to the `begin` command:

```yaml
dotnet sonarscanner begin /k:"idasen-desk" \
  /d:sonar.host.url="${env:SONAR_HOST_URL}" \
  /d:sonar.login="${env:SONAR_TOKEN}" \
  /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" \
  /d:sonar.coverage.exclusions="**Tests*.cs"
```

### Adding Code Coverage

To include code coverage in SonarQube analysis:

1. Generate coverage reports during the test step
2. Configure SonarQube to read the coverage files
3. Update the workflow to include coverage parameters

## Viewing Results

### SonarCloud

- Go to [SonarCloud](https://sonarcloud.io/)
- Navigate to your project
- View analysis results, code smells, bugs, vulnerabilities, and code coverage

### Self-Hosted

- Go to your SonarQube instance
- Navigate to the `idasen-desk` project
- View the analysis dashboard

## Badge

The README includes a workflow status badge that shows if the SonarQube analysis is passing:

```markdown
[![SonarQube Analysis](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarqube.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarqube.yml)
```

For a SonarCloud quality gate badge, add this to your README after setup:

```markdown
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=idasen-desk&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=idasen-desk)
```

## Troubleshooting

### Workflow Fails with "Authentication error"

- Verify `SONAR_TOKEN` is correct and not expired
- Check that the token has the necessary permissions

### Workflow Fails with "Project not found"

- Verify the project key matches between SonarQube and the workflow file
- Ensure the project exists in your SonarQube/SonarCloud instance

### Build Fails During Analysis

- Check that the solution builds successfully locally
- Review the workflow logs for specific build errors
- Ensure all NuGet packages are restored correctly

## Best Practices

1. **Run on Pull Requests**: The workflow analyzes PRs to catch issues before merging
2. **Quality Gates**: Configure quality gates in SonarQube to enforce code quality standards
3. **Regular Analysis**: The workflow runs on every push to main for continuous monitoring
4. **Review Results**: Regularly review SonarQube reports and address code quality issues

## Resources

- [SonarCloud Documentation](https://docs.sonarcloud.io/)
- [SonarQube Documentation](https://docs.sonarqube.org/)
- [SonarScanner for .NET](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner-for-msbuild/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

---

**Last Updated**: October 2024
**Workflow Status**: [![SonarQube Analysis](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarqube.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarqube.yml)
