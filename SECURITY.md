# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability, please follow these steps:

1. **Do not** create a public issue on this repository.
2. In the top navigation of this repository, click the **Security** tab.
3. In the top right, click the **Report a vulnerability** button.
4. Fill out the provided form with:
   - A description of the vulnerability
   - Steps to reproduce the issue
   - Potential impact
   - Suggested fix (if you have one)

## Response Timeline

We will acknowledge your report within 48 hours and provide an estimated timeline for a fix.

## Thank You

Your help is greatly appreciated!
Responsible disclosure of security vulnerabilities helps protect our entire community.

## Release path & compromise scope

Facts a maintainer would need at 2am if the release identity is compromised. Generic incident-response steps (rotating credentials, revoking OAuth apps, publishing advisories, unlisting NuGet packages) are not duplicated here — GitHub's and NuGet's own docs update faster than a checked-in runbook.

- **Release path**: OIDC / NuGet Trusted Publishing via `NuGet/login@v1` in `.github/workflows/release.yaml`. The workflow mints an ephemeral push token per run via OIDC — the release path does not depend on a long-lived API key stored in GitHub secrets or on the NuGet account. During an incident, check the NuGet account for any long-lived API keys anyway (they can be created outside of CI) and delete anything you don't recognize.
- **Fallback**: none. If Trusted Publishing is compromised, the incident is at the GitHub-account level (the OIDC identity is `Chris-Wolfgang/console-app-template`).
- **Owner**: @Chris-Wolfgang.
- **Downstream consumers**: none internal — this is a `dotnet new` template pack, not a code dependency, so no Wolfgang.* repo consumes it. Consumers are developers who install the templates and generate projects; unknown external consumers may exist on nuget.org.
- **Package coordinates for unlisting**: this repo ships four packages —
  - `Wolfgang.Template.Console` — https://www.nuget.org/packages/Wolfgang.Template.Console/
  - `Wolfgang.Template.Console.Subcommand` — https://www.nuget.org/packages/Wolfgang.Template.Console.Subcommand/
  - `Wolfgang.Template.Console.ETL-SubCommand` — https://www.nuget.org/packages/Wolfgang.Template.Console.ETL-SubCommand/
  - `Wolfgang.Template.Console.Aot` — https://www.nuget.org/packages/Wolfgang.Template.Console.Aot/
