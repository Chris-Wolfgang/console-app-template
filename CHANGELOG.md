# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

- **BREAKING:** the `--response-file` template parameter's choice values were shortened —
  `ParseArgsAsLineSeparated` → `LineSeparated`, `ParseArgsAsSpaceSeparated` → `SpaceSeparated`
  (default is now `LineSeparated`). Scripts/CI that pin the old values must update. See the
  [Migration Guide](docs/MIGRATION.md).

### Deprecated

### Removed

### Fixed

### Security
