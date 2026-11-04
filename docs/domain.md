# Domain

## Concepts

| Term | Type | Meaning |
|---|---|---|
| Class | `SchoolClass` | A group of students in one school year |
| Student | `Student` | A child in exactly one class, with optional general notes |
| Subject | `Subject` | A school subject with a name and short code, shared by all classes |
| Assessment result | `Assessment` | One student's result for an assessment in one subject |
| Subject note | `SubjectNote` | A dated note about a student in a subject |

### There is no class-assessment entity

A class assessment exists only as a set of `Assessment` rows, one per student, with the same title and subject. As a result:

- `CreateClassAssessmentAsync` creates rows only for students in the class at that time.
- A student added later has no row for earlier assessments.
- Editing one row does not affect the others.

## Grades

The scale runs from 1.0 (best) to 5.0 (worst). Higher values are worse.

The value 0.0 means the student did not participate. See [ADR 0004](decisions/0004-grade-scale-and-participation.md).

| Input | Stored grade | Stored `HasParticipated` | Counted in statistics |
|---|---|---|---|
| Grade 1.0 to 5.0, participated | As entered | `true` | Yes |
| Grade 0.0, any participation value | 0.0 | `false` | No |
| Grade 1.0 to 5.0, not participated | As entered | `false` | No |
| Any other grade | Rejected with `ArgumentOutOfRangeException` | | |

### Defaults for a new assessment

`ClassDetailViewModel` creates assessments with:

- grade 1.0 for every student
- the current date and time
- a weight that must parse as a whole number

Until a teacher edits them, these grades **count in all averages as a 1**. See TD-05 and TD-06 in [tech-debt.md](tech-debt.md).

### Grade editor

`AssessmentGradeEntryDto` keeps the two values in sync:

| Change | Effect |
|---|---|
| Participation turned off | Grade becomes 0.0 |
| Participation turned on, grade below 1.0 | Grade becomes 1.0 |
| Grade set to 0.0 or lower while participating | Participation turns off |
| Grade set to 1.0 or higher while not participating | Participation turns on |

The input allows steps of 0.5 from 0.0 to 5.0. See TD-04 in [tech-debt.md](tech-debt.md) for the value 0.5.

## Calculations

`AssessmentService` holds both calculations. Only results with `HasParticipated == true` and `Grade > 0.0` count.

### Average

`CalculateSubjectAverage` returns the sum of `Grade * Weight`, divided by the sum of `Weight`, rounded to two decimal places. If no result counts, it returns 0.0.

- **Subject average:** the results in that subject.
- **Overall average:** all results across subjects, each with its own weight. It is not the average of the subject averages.
- **Display:** one decimal place in the current culture, or `-` if the average is 0.0.

### Performance drop

`HasPerformanceDrop` returns `true` when both conditions are met:

1. At least three results count.
2. The unweighted average of the two latest results, minus the unweighted average of all counted results, is at least 0.75.

The calculation ignores weights. `StudentDetailViewModel` runs it per subject.

## Validation

| Entity | Rule | Exception |
|---|---|---|
| `SchoolClass` | Name not blank | `ArgumentNullException` |
| `SchoolClass` | School year matches `^\d{4}/\d{4}$` | `ArgumentException` |
| `Student` | First and last name not blank | `ArgumentNullException` |
| `Student` | Class ID not empty | `ArgumentNullException` |
| `Subject` | Name and code not blank | `ArgumentNullException` |
| `SubjectNote` | Text not blank | `ArgumentNullException` |
| `SubjectNote` | Student and subject IDs not empty | `ArgumentException` |
| `Assessment` | Title not blank | `ArgumentNullException` |
| `Assessment` | Grade valid (see above) | `ArgumentOutOfRangeException` |
| `Assessment` | Weight greater than 0 | `ArgumentOutOfRangeException` |
| `Assessment` | Student and subject IDs not empty | `ArgumentException` |

Tests rely on these exception types, so changing one is a behavior change.

### Normalization

Entities trim names, notes, feedback and note text, and convert subject codes to upper case. `Assessment.Title` is not trimmed, and the school year is not trimmed.

## Deleting

- Deleting a class deletes its students and their results and notes.
- "Removing" a student from a class permanently deletes the student and their results and notes. Students cannot move between classes in the UI.
- The UI deletes both without a confirmation dialog. See TD-08 in [tech-debt.md](tech-debt.md).
- The app has no function to delete a subject or a single assessment.
