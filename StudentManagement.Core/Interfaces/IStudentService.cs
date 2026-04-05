using StudentManagement.Core.DTOs;

namespace StudentManagement.Core.Interfaces;

public interface IStudentService
{
    Task<ApiResponse<IEnumerable<StudentResponseDto>>> GetAllStudentsAsync();
    Task<ApiResponse<StudentResponseDto>> GetStudentByIdAsync(int id);
    Task<ApiResponse<StudentResponseDto>> CreateStudentAsync(StudentCreateDto dto);
    Task<ApiResponse<StudentResponseDto>> UpdateStudentAsync(int id, StudentUpdateDto dto);
    Task<ApiResponse<bool>> DeleteStudentAsync(int id);
}
