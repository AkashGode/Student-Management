using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Infrastructure.Data;

namespace StudentManagement.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<StudentRepository> _logger;

    public StudentRepository(AppDbContext context, ILogger<StudentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all students from database");
        return await _context.Students.AsNoTracking().OrderByDescending(s => s.CreatedDate).ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching student with ID: {Id}", id);
        return await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _context.Students.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
    }

    public async Task<Student> AddAsync(Student student)
    {
        student.CreatedDate = DateTime.UtcNow;
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Student created with ID: {Id}", student.Id);
        return student;
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Student updated with ID: {Id}", student.Id);
        return student;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student is null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Student deleted with ID: {Id}", id);
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Students.AnyAsync(s => s.Id == id);
    }
}
