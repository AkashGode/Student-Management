using Microsoft.Extensions.Logging;
using StudentManagement.Core.DTOs;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;

namespace StudentManagement.Infrastructure.Repositories;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly ILogger<StudentService> _logger;

    public StudentService(IStudentRepository repository, ILogger<StudentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<StudentResponseDto>>> GetAllStudentsAsync()
    {
        var students = await _repository.GetAllAsync();
        var dtos = students.Select(MapToDto);
        return ApiResponse<IEnumerable<StudentResponseDto>>.SuccessResponse(dtos, "Students retrieved successfully");
    }

    public async Task<ApiResponse<StudentResponseDto>> GetStudentByIdAsync(int id)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student is null)
        {
            _logger.LogWarning("Student not found with ID: {Id}", id);
            return ApiResponse<StudentResponseDto>.FailResponse($"Student with ID {id} not found");
        }

        return ApiResponse<StudentResponseDto>.SuccessResponse(MapToDto(student));
    }

    public async Task<ApiResponse<StudentResponseDto>> CreateStudentAsync(StudentCreateDto dto)
    {
        // Check duplicate email
        var existing = await _repository.GetByEmailAsync(dto.Email);
        if (existing is not null)
            return ApiResponse<StudentResponseDto>.FailResponse($"Email '{dto.Email}' is already registered");

        var student = new Student
        {
            Name = dto.Name.Trim(),
            Email = dto.Email.Trim().ToLower(),
            Age = dto.Age,
            Course = dto.Course.Trim()
        };

        var created = await _repository.AddAsync(student);
        return ApiResponse<StudentResponseDto>.SuccessResponse(MapToDto(created), "Student created successfully");
    }

    public async Task<ApiResponse<StudentResponseDto>> UpdateStudentAsync(int id, StudentUpdateDto dto)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student is null)
            return ApiResponse<StudentResponseDto>.FailResponse($"Student with ID {id} not found");

        // Check email conflict with another student
        var emailOwner = await _repository.GetByEmailAsync(dto.Email);
        if (emailOwner is not null && emailOwner.Id != id)
            return ApiResponse<StudentResponseDto>.FailResponse($"Email '{dto.Email}' is already in use");

        student.Name = dto.Name.Trim();
        student.Email = dto.Email.Trim().ToLower();
        student.Age = dto.Age;
        student.Course = dto.Course.Trim();

        var updated = await _repository.UpdateAsync(student);
        return ApiResponse<StudentResponseDto>.SuccessResponse(MapToDto(updated), "Student updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteStudentAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
            return ApiResponse<bool>.FailResponse($"Student with ID {id} not found");

        return ApiResponse<bool>.SuccessResponse(true, "Student deleted successfully");
    }

    private static StudentResponseDto MapToDto(Student s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Email = s.Email,
        Age = s.Age,
        Course = s.Course,
        CreatedDate = s.CreatedDate
    };
}
