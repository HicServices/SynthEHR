# BadMedicine
Library and CLI for randomly generating medical data like you might get out of an Electronic Health Records (EHR) system

## Command Line Usage:

Bad Medicine can be run from the command line:

```
dotnet BadMedicine.dll c:\temp\
```

You can change how much data is produced (e.g. 500 patients, 10000 records per dataset):

```
dotnet BadMedicine.dll c:\temp\ 500 10000
```

Or run only a single dataset:

```
dotnet ./BadMedicine.dll c:\omg 5000 200000 -l -d CarotidArteryScanReportExerciseTestData
```

## Library Usage

You can generate test data for your program yourself:

```csharp
//Seed the random generator if you want to always produce the same randomisation
var r = new Random(100);

//Create a new person
var person = new TestPerson(r);

//Create test data for that person
var a = new TestAdmission(person,person.DateOfBirth,r);

Assert.IsNotNull(a.Person.CHI);
Assert.IsNotNull(a.Person.DateOfBirth);
Assert.IsNotNull(a.Person.Address.Line1);
Assert.IsNotNull(a.Person.Address.Postcode);
Assert.IsNotNull(a.AdmissionDate);
Assert.IsNotNull(a.DischargeDate);
Assert.IsNotNull(a.Condition1);
```

## Datasets

The following synthetic datasets can be produced.

| Dataset        | Description           |  Class Name |
| ------------- |:-------------:|:------:|
| Demography      | Address and patient details as might appear in the CHI register | `DemographyExerciseTestData`|
| Biochemistry      | Lab test codes as might appear in Sci Store lab system extracts | `BiochemistryExerciseTestData`|
| Prescribing      | Prescription data of prescribed drugs | `PrescribingExerciseTestData`|
| Carotid Artery Scan      | Scan results for Carotid Artery | `CarotidArteryScanReportExerciseTestData`|
| Hospital Admissions | ICD9 and ICD10 codes for admission to hospital | `HospitalAdmissionsExerciseTestData`|

