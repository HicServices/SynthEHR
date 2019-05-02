# BadMedicine
Library and CLI for randomly generating medical data like you might get out of an Electronic Health Records (EHR) system

## Usage

Bad Medicine can be run from the command line:

```
dotnet BadMedicine.dll c:\temp\
```

## Datasets

The following synthetic datasets can be produced.

| Dataset        | Description           |  Class Name |
| ------------- |:-------------:|:------:|
| Demography      | Address and patient details as might appear in the CHI register | `DemographyExerciseTestData`|
| Biochemistry      | Lab test codes as might appear in Sci Store lab system extracts | `BiochemistryExerciseTestData`|
| Prescribing      | Prescription data of prescribed drugs | `PrescribingExerciseTestData`|
| Carotid Artery Scan      | Scan results for Carotid Artery | `CarotidArteryScanReportExerciseTestData`|
