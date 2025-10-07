# SonarCloud Static Code Analysis Setup

## Overview

This repository includes a dedicated GitHub Actions workflow for running SonarCloud static code analysis on the C# codebase. The workflow is configured to run automatically on pushes to the main branch and on pull requests.

SonarCloud is a cloud-based code quality and security service that is free for open-source projects and provides excellent integration with GitHub.

## Workflow Details

- **Workflow File**: `.github/workflows/sonarqube.yml`
- **Trigger Events**:
  - Push to `main` branch
  - Pull request events (opened, synchronize, reopened)
  - Manual trigger via `workflow_dispatch`
- **Platform**: Windows latest (required for .NET 8.0 Windows-specific projects)

## Setup Requirements

To enable SonarCloud analysis, you need to configure the following repository secrets:

### Required Secrets

1. **SONAR_TOKEN**
   - Description: Authentication token for SonarCloud
   - Where to get it: Go to [SonarCloud](https://sonarcloud.io/) → My Account → Security → Generate Token

2. **SONAR_ORGANIZATION**
   - Description: Your SonarCloud organization key
   - Where to get it: Available in your SonarCloud organization settings or when you create a project

### Adding Secrets to GitHub

1. Go to your repository on GitHub
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add both `SONAR_TOKEN` and `SONAR_ORGANIZATION`

## SonarCloud Setup

SonarCloud is free for open-source projects and provides excellent integration with GitHub:

1. **Sign up for SonarCloud**
   - Go to [SonarCloud](https://sonarcloud.io/)
   - Sign in with your GitHub account
   - Import your organization (e.g., `tschroedter`)

2. **Create a new project**
   - Click **Analyze new project**
   - Select `tschroedter/idasen-desk`
   - Choose **GitHub Actions** as the analysis method

3. **Get your credentials**
   - Project key: `idasen-desk` (or as configured in SonarCloud)
   - Organization key: Your SonarCloud organization (e.g., `tschroedter`)
   - Token: Generate under My Account → Security → Generate Token

4. **Verify workflow configuration**
   - The workflow is already configured with project key `idasen-desk`
   - The workflow uses the hardcoded URL `https://sonarcloud.io`
   - If your project key is different, update the `/k:` parameter in `.github/workflows/sonarqube.yml`

5. **Add secrets to GitHub**
   - Add `SONAR_TOKEN` with the generated token
   - Add `SONAR_ORGANIZATION` with your organization key (e.g., `tschroedter`)

## Self-Hosted SonarQube Setup (Alternative)

If you prefer a self-hosted SonarQube instance instead of SonarCloud, you would need to modify the workflow file to use `SONAR_HOST_URL` instead of the hardcoded `https://sonarcloud.io` URL. However, this workflow is specifically configured for SonarCloud.

For self-hosted SonarQube setup:
1. Modify `.github/workflows/sonarqube.yml` to replace `https://sonarcloud.io` with `${{ secrets.SONAR_HOST_URL }}`
2. Replace the `SONAR_ORGANIZATION` secret with `SONAR_HOST_URL`
3. Update the organization parameter `/o:` to use your self-hosted configuration

## Workflow Behavior

### With Secrets Configured

When `SONAR_TOKEN` and `SONAR_ORGANIZATION` are properly configured:
1. The workflow restores NuGet packages
2. Begins SonarCloud analysis
3. Builds the solution
4. Completes SonarCloud analysis and uploads results to https://sonarcloud.io

### Without Secrets Configured

If secrets are not configured:
- The workflow will skip the analysis gracefully
- A warning message will be displayed
- The workflow will not fail (exit code 0)
- This allows the workflow to be committed without requiring immediate setup

## Customization

### Changing the Project Key

To change the SonarCloud project key, edit `.github/workflows/sonarqube.yml`:

```yaml
dotnet sonarscanner begin /k:"your-project-key" /o:"${env:SONAR_ORGANIZATION}" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.token="${env:SONAR_TOKEN}"
```

### Adding SonarCloud Parameters

You can add additional SonarCloud parameters to the `begin` command:

```yaml
dotnet sonarscanner begin /k:"idasen-desk" \
  /o:"${env:SONAR_ORGANIZATION}" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.token="${env:SONAR_TOKEN}" \
  /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" \
  /d:sonar.coverage.exclusions="**Tests*.cs"
```

### Adding Code Coverage

To include code coverage in SonarCloud analysis:

1. Generate coverage reports during the test step
2. Configure SonarCloud to read the coverage files
3. Update the workflow to include coverage parameters

## Viewing Results

### SonarCloud

- Go to [SonarCloud](https://sonarcloud.io/)
- Navigate to your organization (e.g., `tschroedter`)
- Select the `idasen-desk` project
- View analysis results, code smells, bugs, vulnerabilities, and code coverage

### Quality Gate Badge

The README includes a workflow status badge that shows if the SonarCloud analysis is passing:

```markdown
[![SonarCloud Analysis](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarqube.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarqube.yml)
```

For a SonarCloud quality gate badge, add this to your README after setup:

```markdown
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=idasen-desk&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=idasen-desk)
```

## Troubleshooting

### Workflow Fails with "Authentication error"

- Verify `SONAR_TOKEN` is correct and not expired
- Check that the token has the necessary permissions in SonarCloud

### Workflow Fails with "Project not found"

- Verify the project key matches between SonarCloud and the workflow file
- Ensure the project exists in your SonarCloud organization
- Check that the `SONAR_ORGANIZATION` secret is set correctly

### Workflow Fails with "Organization not found"

- Verify the `SONAR_ORGANIZATION` secret matches your SonarCloud organization key
- Check that your token has access to the organization

### Build Fails During Analysis

- Check that the solution builds successfully locally
- Review the workflow logs for specific build errors
- Ensure all NuGet packages are restored correctly

## Best Practices

1. **Run on Pull Requests**: The workflow analyzes PRs to catch issues before merging
2. **Quality Gates**: Configure quality gates in SonarCloud to enforce code quality standards
3. **Regular Analysis**: The workflow runs on every push to main for continuous monitoring
4. **Review Results**: Regularly review SonarCloud reports and address code quality issues

## Resources

- [SonarCloud Documentation](https://docs.sonarcloud.io/)
- [SonarScanner for .NET](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner-for-msbuild/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

---

**Last Updated**: October 2024
**Workflow Status**: [![SonarCloud Analysis](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarqube.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarqube.yml)
