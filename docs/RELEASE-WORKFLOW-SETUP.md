# Release Workflow Setup Guide

This guide explains how to configure the repository for the `release.yaml` workflow.

## Overview

The release workflow triggers when you **publish a GitHub Release** and:
- Validates all template projects build successfully
- Packs NuGet template packages with the version from the release tag
- Smoke tests template installation via `dotnet new install`
- Generates SBOMs (CycloneDX) for each project
- Publishes all packages to NuGet.org
- Triggers DocFX documentation deployment (when configured)
- Attaches NuGet packages and SBOMs to the GitHub Release

## Required Configuration

### Add NuGet API Key Secret

**Location:** Settings > Secrets and variables > Actions > New repository secret

1. Click **"New repository secret"**
2. **Name:** `NUGET_API_KEY`
3. **Value:** Your NuGet.org API key
   - Get your key from [NuGet.org Account > API Keys](https://www.nuget.org/account/apikeys)
   - Recommended scopes: **Push new packages and package versions**
   - Set package glob pattern to `Wolfgang.Template.Console*`
   - Set expiration date (recommended: 1 year)
4. Click **"Add secret"**

The workflow validates this secret exists before attempting to publish.

## Creating a Release

1. Update `<PackageVersion>` in all three pack `.csproj` files to match the intended version
2. Merge all changes to `main`
3. Go to **Releases > Draft a new release**
4. Create a new tag matching the version (e.g., `v0.4.0`)
5. Set the title (e.g., `v0.4.0`)
6. Write release notes describing the changes
7. Click **Publish release**

The workflow will automatically:
- Build and validate all projects
- Pack with the version from the tag (overrides csproj `PackageVersion` via `/p:PackageVersion`)
- Publish to NuGet.org
- Attach `.nupkg` and `.bom.json` files to the release

## Template Packages Published

| Package | NuGet ID |
|---|---|
| Console App | `Wolfgang.Template.Console` |
| Subcommand | `Wolfgang.Template.Console.Subcommand` |
| ETL Subcommand | `Wolfgang.Template.Console.ETL-SubCommand` |

## Troubleshooting

### NUGET_API_KEY not configured
The workflow will fail with a clear error message. Add the secret as described above.

### Duplicate package version
The workflow uses `--skip-duplicate` so publishing an already-existing version will succeed without error.

### Build failures
Check the "Validate Release Build" job logs. All projects must compile before packing proceeds.
