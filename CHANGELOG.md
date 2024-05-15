# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - Unreleased

- Rename package to SynthEHR

## [1.2.2] - 2024-05-16

-Add warning about naming deprecation, see [README](./README.md#Deprecation)

## [1.2.1] - 2024-03-18

- Bump HIC.FAnsiSql from 3.2.0 to 3.2.2

## [1.2.0] - 2024-03-06

- Now targets .Net 8.0
- Some bugfixes change random data generation, cross-version consistency not preserved
- SynthEHR itself now AOT/trim clean, but dependencies are not
- Improve BucketList performance
- Add Equ 2.3.0
- Bump HIC.FAnsiSql from 3.0.1 to 3.2.0
- Bump CvsHelper from 30.0.1 to 31.0.2
- Bump YamlDotNet from 12.0.2 to 15.1.2

## [1.1.2] - 2022-11-22

### Dependencies

- Bump CvsHelper from 30.0.0 to 30.0.1

## [1.1.1] - 2022-10-31

### Dependencies

- Bump CvsHelper from 28.0.0 to 30.0.0
- Bump HIC.FAnsiSql from 2.0.4 to 3.0.1
- Bump YamlDotNet from 11.2.1 to 12.0.2

## [1.1.0] - 2022-07-11

### Added

- Added support for creating to database
- Added the Wide and UltraWide datasets
- Added Microsoft.SourceLink.GitHub 1.1.1 (build time only) for easier debugging

### Dependencies

- Bump CsvHelper from 24.0.1 to 28.0.0
- Bump MathNet.Numerics from 4.15.0 to 5.0.0
- Build nupkg using csproj metadata instead of separate nuspec

### Fixed

- Fixed formatting and missing headers in lookup z\_chiStatus.csv
- Fixed `--help` not rendering correctly for some command line arguments

## [1.0.0] - 2021-02-16

### Changed

- Bump MathNet.Numerics from 4.13.0 to 4.14.0
- Bump CsvHelper from 19.0.0 to 24.0.1

## [0.1.6] - 2020-05-20

### Changed

- Updated CsvHelper to 15.0.5
- Updated MathNet.Numerics to 4.9.1

## [0.1.5] - 2019-07-12

### Fixed

- Fixed seed randomisation bug (caused by static initialization) when generating multiple copies of a dataset (e.g. generating Biochemistry twice within the same process)

## [0.1.4] - 2019-07-02

### Changed

- CI changes only (no changes to codebase)

## [0.1.3] - 2019-07-02

### Changed

- Patient birth dates now go from 1914 (Person.MinimumYearOfBirth) allowing for patients aged up to 100 years

[Unreleased]: https://github.com/HicServices/SynthEHR/compare/v2.0.0...main
[2.0.0]: https://github.com/HicServices/SynthEHR/compare/v1.2.2...v2.0.0
[1.2.2]: https://github.com/HicServices/SynthEHR/compare/v1.2.1...v1.2.2
[1.2.1]: https://github.com/HicServices/SynthEHR/compare/v1.2.0...v1.2.1
[1.2.0]: https://github.com/HicServices/SynthEHR/compare/v1.1.2...v1.2.0
[1.1.2]: https://github.com/HicServices/SynthEHR/compare/v1.1.1...v1.1.2
[1.1.1]: https://github.com/HicServices/SynthEHR/compare/v1.1.0...v1.1.1
[1.1.0]: https://github.com/HicServices/SynthEHR/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/HicServices/SynthEHR/compare/v0.1.6...v1.0.0
[0.1.6]: https://github.com/HicServices/SynthEHR/compare/v0.1.5...v0.1.6
[0.1.5]: https://github.com/HicServices/SynthEHR/compare/v0.1.4...v0.1.5
[0.1.4]: https://github.com/HicServices/SynthEHR/compare/v0.1.3...v0.1.4
[0.1.3]: https://github.com/HicServices/SynthEHR/compare/0.0.1.2...v0.1.3
