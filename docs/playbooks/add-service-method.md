# Add a service method

1. Add the method to the interface (`IClassService` or `IAssessmentService`), with the full XML contract.
2. Implement it in the service, with `/// <inheritdoc/>`.
   - Put domain rules in the entity, not in the service.
   - Sort lists explicitly, as the existing queries do.
   - Remember that `Student`, `Assessment` and `SubjectNote` load their navigations automatically. See [data.md](../data.md#automatic-loading).
3. Add the method to the matching stub in `Tests/Stubs/`.
4. Add tests to `Tests/Services/`. Use SQLite if the behavior depends on the database. See [testing.md](../testing.md#choosing-a-test-database).
5. Call the method from the view model through the interface.
6. Run the checks in the [definition of done](../../AGENTS.md#definition-of-done).
