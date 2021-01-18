# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [Unreleased]

### Changed

- Bump MathNet.Numerics from 4.13.0 to 4.14.0
- Bump CsvHelper from 19.0.0 to 20.0.0

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

[Unreleased]: https://github.com/HicServices/BadMedicine/compare/v0.1.6...develop
[0.1.6]: https://github.com/HicServices/BadMedicine/compare/v0.1.5...v0.1.6
[0.1.5]: https://github.com/HicServices/BadMedicine/compare/v0.1.4...v0.1.5
[0.1.4]: https://github.com/HicServices/BadMedicine/compare/v0.1.3...v0.1.4
[0.1.3]: https://github.com/HicServices/BadMedicine/compare/0.0.1.2...v0.1.3
