# Build Instructions

Detailed instructions for building the Idasen Desk Controller from source.

## Prerequisites

### Required Software

1. **Windows 10 or 11**
   - The project targets Windows-specific APIs (WPF, Bluetooth LE)
   - Cannot be built on Linux or macOS

2. **.NET 8.0 SDK**
   - Download from [Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Verify installation: `dotnet --version`

3. **Git**
   - Download from [git-scm.com](https://git-scm.com/)
   - Or use GitHub Desktop

### Optional Tools

- **Visual Studio 2022** (recommended for GUI development)
  - Community Edition is free
  - Install "NET desktop development" workload
  
- **VS Code** (alternative, lighter weight)
  - Install C# extension
  - Install .NET Core Test Explorer extension

## Getting the Source Code

### Clone the Repository

```bash
# Using HTTPS
git clone https://github.com/tschroedter/idasen-desk.git

# Using SSH (if you have SSH keys set up)
git clone git@github.com:tschroedter/idasen-desk.git

# Navigate to the repository
cd idasen-desk
```

### Verify Repository Structure

```bash
cd idasen-desk
dir  # Windows
ls   # Git Bash
```

You should see:
- `.github/` - CI/CD configurations
- `docs/` - Documentation
- `src/` - Source code
- `wiki/` - Wiki documentation
- `README.md`, `LICENSE`, etc.

## Building from Command Line

### 1. Navigate to Source Directory

```bash
cd src
```

### 2. Restore NuGet Packages

```bash
dotnet restore Idasen-Desk.sln
```

This downloads all required dependencies.

### 3. Build the Solution

**Debug Build:**
```bash
dotnet build Idasen-Desk.sln --configuration Debug
```

**Release Build:**
```bash
dotnet build Idasen-Desk.sln --configuration Release
```

### 4. Verify Build

Check for the executable:
```bash
# Debug
dir Idasen.SystemTray.Win11\bin\Debug\net8.0-windows10.0.19041\

# Release
dir Idasen.SystemTray.Win11\bin\Release\net8.0-windows10.0.19041\
```

### 5. Run the Application

```bash
# Debug
.\Idasen.SystemTray.Win11\bin\Debug\net8.0-windows10.0.19041\Idasen.SystemTray.Win11.exe

# Release
.\Idasen.SystemTray.Win11\bin\Release\net8.0-windows10.0.19041\Idasen.SystemTray.Win11.exe
```

## Building in Visual Studio

### 1. Open Solution

1. Launch Visual Studio 2022
2. File → Open → Project/Solution
3. Navigate to `src/Idasen-Desk.sln`
4. Click Open

### 2. Restore Packages

Visual Studio automatically restores NuGet packages on solution load. If needed:
1. Right-click solution in Solution Explorer
2. Select "Restore NuGet Packages"

### 3. Select Build Configuration

In the toolbar dropdown:
- Select "Debug" for development
- Select "Release" for production build

### 4. Build Solution

- Menu: Build → Build Solution
- Shortcut: `Ctrl + Shift + B`
- Or right-click solution → Build

### 5. Run Application

- Menu: Debug → Start Debugging (F5)
- Or: Debug → Start Without Debugging (Ctrl+F5)

## Building in VS Code

### 1. Open Folder

1. Launch VS Code
2. File → Open Folder
3. Select the `idasen-desk` directory

### 2. Install Extensions

When prompted, install recommended extensions:
- C# (Microsoft)
- .NET Core Test Explorer

### 3. Build

**Using Terminal:**
```bash
cd src
dotnet build Idasen-Desk.sln --configuration Release
```

**Using Tasks:**
1. Terminal → Run Build Task (Ctrl+Shift+B)
2. Select "build" from the list

### 4. Run

```bash
cd src
dotnet run --project Idasen.SystemTray.Win11/Idasen.SystemTray.Win11.csproj
```

## Publishing Standalone Executable

To create a single-file, self-contained executable (like the official releases):

### Full Command

```bash
cd src

dotnet publish Idasen.SystemTray.Win11/Idasen.SystemTray.Win11.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishReadyToRun=true \
  -p:IncludeNativeLibrariesForSelfExtract=true
```

### Explanation of Parameters

- `--configuration Release` - Optimized release build
- `--runtime win-x64` - Windows 64-bit target
- `--self-contained true` - Include .NET runtime
- `-p:PublishSingleFile=true` - Single executable file
- `-p:PublishReadyToRun=true` - Ahead-of-time compilation (faster startup)
- `-p:IncludeNativeLibrariesForSelfExtract=true` - Bundle native dependencies

### Output Location

Published executable is in:
```
src/Idasen.SystemTray.Win11/bin/Release/net8.0-windows10.0.19041/win-x64/publish/
```

File: `Idasen.SystemTray.Win11.exe` (self-contained, ~70-100 MB)

### Testing Published Build

```bash
cd src/Idasen.SystemTray.Win11/bin/Release/net8.0-windows10.0.19041/win-x64/publish/
.\Idasen.SystemTray.Win11.exe
```

## Running Tests

### Command Line

```bash
cd src

# Run all tests
dotnet test Idasen-Desk.sln

# With detailed output
dotnet test Idasen-Desk.sln --verbosity normal

# With code coverage
dotnet test Idasen-Desk.sln --collect:"XPlat Code Coverage"
```

### Visual Studio

1. Test → Test Explorer
2. Click "Run All Tests" (or Ctrl+R, A)
3. View results in Test Explorer

### VS Code

1. Install .NET Core Test Explorer extension
2. Test Explorer icon in sidebar
3. Click "Run All Tests"

## Build Configurations

### Debug Configuration

**Purpose:** Development and debugging

**Characteristics:**
- No optimizations
- Debug symbols included
- Easier to debug
- Larger file size
- Slower execution

**When to use:**
- During development
- When debugging issues
- When stepping through code

### Release Configuration

**Purpose:** Production deployment

**Characteristics:**
- Full optimizations
- No debug symbols (by default)
- Smaller file size
- Faster execution
- Harder to debug

**When to use:**
- Creating releases
- Performance testing
- End-user distribution

## Build Targets

The solution includes two projects:

### 1. Idasen.SystemTray.Win11

**Type:** WPF Application (.exe)

**Purpose:** Main application

**Output:**
- Executable: `Idasen.SystemTray.Win11.exe`
- Configuration files
- Dependencies

### 2. Idasen.SystemTray.Win11.Tests

**Type:** Test Project (.dll)

**Purpose:** Unit tests

**Output:**
- Test assembly: `Idasen.SystemTray.Win11.Tests.dll`
- Test results

## Troubleshooting Build Issues

### "SDK not found" Error

**Problem:** .NET 8.0 SDK not installed

**Solution:**
1. Download .NET 8.0 SDK
2. Install
3. Restart terminal/IDE
4. Verify: `dotnet --version`

### NuGet Package Restore Fails

**Problem:** Cannot download packages

**Solutions:**
1. Check internet connection
2. Clear NuGet cache:
   ```bash
   dotnet nuget locals all --clear
   ```
3. Retry restore:
   ```bash
   dotnet restore --force
   ```

### Build Errors After Pulling Updates

**Problem:** Outdated dependencies or cache

**Solutions:**
1. Clean solution:
   ```bash
   dotnet clean
   ```
2. Restore packages:
   ```bash
   dotnet restore --force
   ```
3. Rebuild:
   ```bash
   dotnet build
   ```

### "Target framework not found"

**Problem:** Wrong .NET version installed

**Solution:**
1. Ensure .NET 8.0 SDK installed (not just runtime)
2. Check installed SDKs:
   ```bash
   dotnet --list-sdks
   ```
3. Should show: `8.0.xxx`

### Visual Studio Build Fails But Command Line Works

**Problem:** Visual Studio cache issue

**Solutions:**
1. Close Visual Studio
2. Delete `.vs` folder in solution directory
3. Delete `bin` and `obj` folders
4. Reopen Visual Studio
5. Rebuild solution

### "Warnings treated as errors"

**Problem:** Code contains warnings

**Solution:**
The project treats warnings as errors for code quality. Either:
1. Fix the warnings (recommended)
2. Temporarily disable (not recommended):
   Edit `.csproj`, change `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` to `false`

## Build Performance Tips

### Parallel Builds

Use multiple CPU cores:
```bash
dotnet build -m:4  # Use 4 cores
```

### Incremental Builds

After first build, subsequent builds are faster:
- Only changed files are recompiled
- Dependencies are cached

### Avoid Clean Unless Necessary

Only clean when:
- Switching branches
- After package updates
- Build behaves unexpectedly

## Continuous Integration

The project uses GitHub Actions for CI/CD. See `.github/workflows/dotnet-ci.yml` for:
- Automated builds
- Test execution  
- Release creation
- Version stamping

To run the same checks locally:
```bash
cd src
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --verbosity normal
```

## Build Artifacts

After a successful build, you'll find:

### Debug Build
```
src/Idasen.SystemTray.Win11/bin/Debug/net8.0-windows10.0.19041/
├── Idasen.SystemTray.Win11.exe
├── Idasen.SystemTray.Win11.dll
├── Idasen.SystemTray.Win11.pdb (debug symbols)
├── [Dependencies].dll
└── Idasen.SystemTray.Win11.deps.json
```

### Release Build
```
src/Idasen.SystemTray.Win11/bin/Release/net8.0-windows10.0.19041/
├── Idasen.SystemTray.Win11.exe
├── Idasen.SystemTray.Win11.dll
├── [Dependencies].dll
└── Idasen.SystemTray.Win11.deps.json
```

### Published Build
```
src/Idasen.SystemTray.Win11/bin/Release/net8.0-windows10.0.19041/win-x64/publish/
└── Idasen.SystemTray.Win11.exe (self-contained, single file)
```

## Next Steps

After successfully building:

1. **Run Tests** - Ensure all tests pass
2. **Run Application** - Test the built executable
3. **Make Changes** - Modify code as needed
4. **Debug** - Use IDE debugging tools
5. **Contribute** - See [Developer Guide](Developer-Guide) for contribution workflow

## Additional Resources

- [.NET CLI Documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [MSBuild Reference](https://docs.microsoft.com/en-us/visualstudio/msbuild/)
- [Developer Guide](Developer-Guide) - Contributing guidelines
- [Architecture](Architecture) - Technical architecture details

---

**Navigation**: [Home](Home) | [Developer Guide](Developer-Guide) | [Build Instructions](Build-Instructions) | [Testing Guide](Testing-Guide)
