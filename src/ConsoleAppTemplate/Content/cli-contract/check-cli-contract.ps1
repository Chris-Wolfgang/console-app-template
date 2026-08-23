<#
.SYNOPSIS
    Verifies the app's CLI surface has not drifted from the committed baseline.

.DESCRIPTION
    Runs the 'cli-surface' subcommand to capture the current CLI manifest and diffs it
    against cli-contract/cli-surface.baseline.json. Exits non-zero on any difference -
    a renamed/removed option or subcommand is a breaking change for scripts and pipelines
    that call this app. Run with -Update to accept the current surface as the new baseline
    (do this deliberately, and note breaking changes in your release notes).

.EXAMPLE
    pwsh cli-contract/check-cli-contract.ps1
    pwsh cli-contract/check-cli-contract.ps1 -Update
#>
param(
    [switch]$Update
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$baseline = Join-Path $PSScriptRoot 'cli-surface.baseline.json'
$current = Join-Path ([System.IO.Path]::GetTempPath()) "cli-surface.$([System.Guid]::NewGuid().ToString('N')).json"

try
{
    dotnet run --project $root -c Release -- cli-surface --output $current | Out-Null
    if ($LASTEXITCODE -ne 0)
    {
        throw "cli-surface command failed with exit code $LASTEXITCODE."
    }

    if ($Update)
    {
        Copy-Item $current $baseline -Force
        Write-Host "Baseline updated: $baseline"
        exit 0
    }

    if (-not (Test-Path $baseline))
    {
        Write-Error "No baseline found. Create one with: pwsh cli-contract/check-cli-contract.ps1 -Update"
        exit 1
    }

    # -SyncWindow 0 keeps the comparison sequence-aware so a pure reordering of lines
    # is still reported as drift (the default window is order-insensitive).
    $diff = Compare-Object (Get-Content $baseline) (Get-Content $current) -SyncWindow 0
    if ($null -ne $diff)
    {
        Write-Host "CLI surface has changed from the committed baseline:" -ForegroundColor Yellow
        $diff | Format-Table -AutoSize | Out-String | Write-Host
        Write-Error "CLI contract drift detected. If this change is intentional, re-run with -Update and document it."
        exit 1
    }

    Write-Host "CLI surface matches the baseline." -ForegroundColor Green
}
finally
{
    if (Test-Path $current) { Remove-Item $current -Force }
}
