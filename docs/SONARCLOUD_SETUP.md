# SonarCloud Static Code Analysis Setup

This document describes the SonarCloud integration for the idasen-desk project.

## Overview

The project includes a dedicated GitHub Actions workflow (`.github/workflows/sonarcloud.yml`) that performs static code analysis using SonarCloud. This workflow runs automatically on:
- Push to `main` branch
- Pull requests targeting `main` branch
- Manual trigger via workflow dispatch

## Prerequisites

Before the SonarCloud workflow can run successfully, the following setup is required:

### 1. SonarCloud Project Setup

1. Go to [SonarCloud.io](https://sonarcloud.io/)
2. Sign in with your GitHub account
3. Import the `tschroedter/idasen-desk` repository
4. Note the following values from your SonarCloud project:
   - **Project Key**: `tschroedter_idasen-desk` (already configured in the workflow)
   - **Organization**: `tschroedter` (already configured in the workflow)

### 2. GitHub Repository Secret

Add the `SONAR_TOKEN` secret to your GitHub repository:

1. In SonarCloud, go to your project settings
2. Navigate to "Security" or "Analysis Method"
3. Generate a token (or copy the existing one)
4. In your GitHub repository, go to Settings → Secrets and variables → Actions
5. Create a new repository secret:
   - Name: `SONAR_TOKEN`
   - Value: (paste the token from SonarCloud)

## Workflow Details

### What the Workflow Does

1. **Checkout Code**: Uses full git history for better analysis
2. **Setup Environment**: Installs .NET 8.x SDK
3. **Cache Management**: Caches both SonarCloud scanner and NuGet packages
4. **Install Scanner**: Installs the SonarScanner for .NET as a dotnet tool
5. **Restore Dependencies**: Restores NuGet packages
6. **Analyze Code**:
   - Begins SonarCloud analysis
   - Builds the solution in Release mode
   - Runs tests with code coverage collection (OpenCover format)
   - Ends SonarCloud analysis and uploads results

### Code Coverage

The workflow collects code coverage using XPlat Code Coverage with OpenCover format. The coverage data is automatically uploaded to SonarCloud and displayed in the analysis results.

## Badges

The README.md includes two SonarCloud badges:

1. **Workflow Status Badge**: Shows if the SonarCloud analysis workflow is passing
2. **Quality Gate Badge**: Shows the overall code quality status from SonarCloud

## Configuration

The workflow is configured with the following key parameters:

- **Project Key**: `tschroedter_idasen-desk`
- **Organization**: `tschroedter`
- **SonarCloud URL**: `https://sonarcloud.io`
- **Coverage Format**: OpenCover XML

### Customization

If you need to customize the analysis, you can modify the workflow file to add additional SonarCloud properties. Common properties include:

```powershell
/d:sonar.exclusions="**/obj/**,**/bin/**"
/d:sonar.coverage.exclusions="**/*Tests.cs"
/d:sonar.cpd.exclusions="**/Program.cs"
```

Refer to the [SonarCloud documentation](https://docs.sonarcloud.io/advanced-setup/analysis-parameters/) for all available parameters.

## Viewing Results

After the workflow runs successfully:

1. Check the workflow run in the GitHub Actions tab
2. View detailed analysis results on [SonarCloud.io](https://sonarcloud.io/dashboard?id=tschroedter_idasen-desk)
3. Review the badges in the README.md for at-a-glance status

## Troubleshooting

### Workflow Fails with Authentication Error

- Ensure the `SONAR_TOKEN` secret is correctly set in GitHub repository secrets
- Verify the token hasn't expired in SonarCloud

### No Code Coverage Data

- Ensure the test project has the necessary code coverage package installed
- Verify the OpenCover format is being generated in the TestResults directory

### Build Failures

- The workflow runs on Windows (required for WPF projects)
- Ensure all NuGet packages can be restored
- Check that the project builds successfully on Windows

## Maintenance

- Periodically update the GitHub Actions versions in the workflow
- Keep the SonarScanner for .NET up to date
- Review and address issues flagged by SonarCloud regularly

---

**Last Updated**: October 2024
**Maintainer**: tschroedter
