# Workflow Security

## Overview

This document describes the security measures implemented in the GitHub Actions workflows for this repository.

## Security Architecture

### 1. PR Workflow (`pr.yaml`)

**Trigger**: `pull_request` to `main`

The PR workflow validates all pull requests before merge with:
- **Secrets Scan (gitleaks)**: Detects leaked credentials in the codebase
- **Build & Validate Templates**: Builds all template projects, packs NuGet packages, and runs DevSkim security scanning

### 2. CodeQL Security Analysis

This repository uses **GitHub's default CodeQL setup** rather than a custom workflow file. CodeQL runs automatically on:
- Push to `main`
- Pull requests to `main`
- Weekly schedule

Code scanning results are enforced via branch protection rules using the `code_scanning` rule type, which allows GitHub to handle cases where CodeQL doesn't run without blocking merges.

### 3. DevSkim Security Scanning

DevSkim analyzes source code for security anti-patterns. The following rules are suppressed:
- `DS176209`: Not applicable to this repo
- `DS137138`: Insecure URL — the `$schema` references in template config files use `http://` by convention

### 4. Gitleaks Secrets Scanning

The PR workflow uses the official `gitleaks/gitleaks-action@v2` to detect secrets in the codebase. A `.gitleaks.toml` configuration file provides allowlist rules for test fixtures and test data.

### 5. Branch Protection

Branch protection rules can be configured using `scripts/Setup-BranchRuleset.ps1`. The script creates a ruleset requiring:
- Status checks to pass (Build & Validate Templates, Secrets Scan)
- CodeQL code scanning (High+ severity blocks merge)
- Conversation resolution before merging
- Force push and branch deletion prevention

### 6. Dependency Management

Dependabot is configured in `.github/dependabot.yml` to monitor NuGet packages across all project directories and create PRs for updates weekly.

## Release Workflow (`release.yaml`)

The release workflow triggers on **GitHub Release publish** and:
1. Validates the build across all template projects
2. Packs NuGet packages with the version from the release tag
3. Smoke tests template installation via `dotnet new install`
4. Generates SBOMs (CycloneDX)
5. Publishes to NuGet.org
6. Triggers DocFX documentation deployment
7. Attaches artifacts to the GitHub Release
