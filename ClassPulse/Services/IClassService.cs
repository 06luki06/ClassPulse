using At.luki0606.ClassPulse.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    /// <summary>
    /// Manages school classes, students and subjects.
    /// </summary>
    public interface IClassService
    {
        /// <summary>
        /// Gets all classes with their students, sorted by name.
        /// </summary>
        /// <returns>The classes.</returns>
        Task<List<SchoolClass>> GetAllClassesAsync();
        /// <summary>
        /// Creates a class.
        /// </summary>
        /// <param name="name">The class name. It must not be blank.</param>
        /// <param name="schoolYear">The school year in the format <c>YYYY/YYYY</c>.</param>
        /// <returns>The created class.</returns>
        Task<SchoolClass> CreateClassAsync(string name, string schoolYear);
        /// <summary>
        /// Deletes a class together with its students and their results and notes.
        /// </summary>
        /// <param name="classId">The identifier of the class.</param>
        /// <returns>The deleted class, or <see langword="null"/> if it does not exist.</returns>
        Task<SchoolClass?> DeleteClassAsync(Guid classId);
        /// <summary>
        /// Adds a new student to a class.
        /// </summary>
        /// <param name="schoolClassId">The identifier of the class.</param>
        /// <param name="firstName">The first name. It must not be blank.</param>
        /// <param name="lastName">The last name. It must not be blank.</param>
        /// <param name="generalNotes">The optional general notes.</param>
        /// <returns>The created student.</returns>
        Task<Student> AddStudentToSchoolClassAsync(Guid schoolClassId, string firstName, string lastName, string? generalNotes = null);
        /// <summary>
        /// Searches first and last names for a text fragment.
        /// </summary>
        /// <param name="searchTerm">The text to find. It is trimmed.</param>
        /// <returns>The matches sorted by name, or an empty list for a blank search term.</returns>
        Task<List<Student>> SearchStudentAsync(string searchTerm);
        /// <summary>
        /// Gets a student with class, results and notes.
        /// </summary>
        /// <param name="studentId">The identifier of the student.</param>
        /// <returns>The student, or <see langword="null"/> if it does not exist.</returns>
        Task<Student?> GetStudentDetailsAsync(Guid studentId);
        /// <summary>
        /// Gets the students of a class, sorted by name.
        /// </summary>
        /// <param name="classId">The identifier of the class.</param>
        /// <returns>The students.</returns>
        Task<List<Student>> GetStudentsByClassIdAsync(Guid classId);
        /// <summary>
        /// Gets all subjects, sorted by name.
        /// </summary>
        /// <returns>The subjects.</returns>
        Task<List<Subject>> GetAllSubjectsAsync();
        /// <summary>
        /// Creates a subject.
        /// </summary>
        /// <param name="name">The name. It must not be blank.</param>
        /// <param name="code">The short code. It must not be blank.</param>
        /// <returns>The created subject.</returns>
        Task<Subject> CreateSubjectAsync(string name, string code);
        /// <summary>
        /// Permanently deletes a student together with the student's results and notes.
        /// </summary>
        /// <param name="schoolClassId">The identifier of the class the student must belong to.</param>
        /// <param name="studentId">The identifier of the student.</param>
        /// <returns>The deleted student, or <see langword="null"/> if no such student is in the class.</returns>
        Task<Student?> RemoveStudentFromClassAsync(Guid schoolClassId, Guid studentId);
        /// <summary>
        /// Replaces a student's general notes.
        /// </summary>
        /// <param name="studentId">The identifier of the student.</param>
        /// <param name="generalNotes">The new notes, or <see langword="null"/> to clear them.</param>
        /// <returns>The updated student, or <see langword="null"/> if it does not exist.</returns>
        Task<Student?> UpdateStudentGeneralNotesAsync(Guid studentId, string? generalNotes);
    }
}
