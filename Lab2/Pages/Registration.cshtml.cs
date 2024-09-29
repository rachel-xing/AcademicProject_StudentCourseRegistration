using Microsoft.AspNetCore.Mvc.RazorPages;
using AcademicManagement;
using Microsoft.AspNetCore.Mvc;


namespace Lab2.Pages;

public class Registration : PageModel
{
    [BindProperty] public string? SelectedStudentName { get; set; }
    public Student? SelectedStudent { get; set; }

    public List<Student> AllStudents { get; set; } = DataAccess.GetAllStudents();
    public List<Course> Courses { get; set; } = DataAccess.GetAllCourses();
    public List<AcademicRecord> ExistingAcademicRecords { get; set; } = DataAccess.GetAllAcademicRecords();

    [BindProperty] 
    public List<string> SelectedCourseCodes { get; set; } = new List<string>();
    public AcademicRecord? NewAcademicRecord { get; set; }
    public List<AcademicRecord> SelectedStudentsRegistered = [];
    public bool IsExisted { get; set; }
    public string Message { get; set; }
    
    public IActionResult OnPostStudentSelected()
    {
        SelectedStudent = AllStudents.FirstOrDefault(student => student.StudentName == SelectedStudentName);
        Message = (SelectedStudent is null) ? "You must select a student!" : "";
        IsExisted = ExistingAcademicRecords.Any(record => record.StudentId == SelectedStudent.StudentId);

        if (SelectedStudent != null && !IsExisted)
        {
            Message = "The student hasn't registered any course. Select course(s) to register.";
        }
        else if (SelectedStudent != null && IsExisted)
        {
            Message = "The student has registered for following course(s)";
            SelectedStudentsRegistered = DataAccess.GetAcademicRecordsByStudentId(SelectedStudent.StudentId);
        }

        return Page();
    }

    public IActionResult OnPostRegister()
    {
        SelectedStudent = AllStudents.FirstOrDefault(student => student.StudentName == SelectedStudentName);
        if (SelectedCourseCodes.Count == 0 && SelectedStudent != null)
        {
            Message = "You must select at least one course!";
            return Page();
        } 
        
        if (SelectedCourseCodes.Count > 0 && SelectedStudent != null)
        {
            foreach (var courseCode in SelectedCourseCodes)
            {
                NewAcademicRecord = new AcademicRecord(SelectedStudent.StudentId, courseCode);
                try
                {
                    DataAccess.AddAcademicRecord(NewAcademicRecord);
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    return Page();
                }
            }

            Message = "The student has registered for the following course(s)!";
            SelectedStudentsRegistered = DataAccess.GetAcademicRecordsByStudentId(SelectedStudent.StudentId);
            IsExisted = ExistingAcademicRecords.Any(record => record.StudentId == SelectedStudent.StudentId);
            return Page();
        }
        return Page();
    }
}