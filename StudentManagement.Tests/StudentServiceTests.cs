using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StudentManagement.Core.DTOs;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Infrastructure.Repositories;
using Xunit;

namespace StudentManagement.Tests;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _repoMock;
    private readonly Mock<ILogger<StudentService>> _loggerMock;
    private readonly StudentService _service;

    public StudentServiceTests()
    {
        _repoMock = new Mock<IStudentRepository>();
        _loggerMock = new Mock<ILogger<StudentService>>();
        _service = new StudentService(_repoMock.Object, _loggerMock.Object);
    }

    // ── GetAll ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllStudentsAsync_ReturnsAllStudents()
    {
        var students = new List<Student>
        {
            new() { Id = 1, Name = "Alice", Email = "alice@test.com", Age = 20, Course = "CS" },
            new() { Id = 2, Name = "Bob",   Email = "bob@test.com",   Age = 22, Course = "IT" }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(students);

        var result = await _service.GetAllStudentsAsync();

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllStudentsAsync_EmptyList_ReturnsSuccessWithEmpty()
    {
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Student>());

        var result = await _service.GetAllStudentsAsync();

        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    // ── GetById ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetStudentByIdAsync_ExistingId_ReturnsStudent()
    {
        var student = new Student { Id = 1, Name = "Alice", Email = "alice@test.com", Age = 20, Course = "CS" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(student);

        var result = await _service.GetStudentByIdAsync(1);

        result.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Alice");
    }

    [Fact]
    public async Task GetStudentByIdAsync_NonExistingId_ReturnsFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Student?)null);

        var result = await _service.GetStudentByIdAsync(999);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateStudentAsync_ValidData_ReturnsCreatedStudent()
    {
        var dto = new StudentCreateDto { Name = "Charlie", Email = "charlie@test.com", Age = 21, Course = "Math" };
        var created = new Student { Id = 3, Name = "Charlie", Email = "charlie@test.com", Age = 21, Course = "Math" };

        _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Student?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Student>())).ReturnsAsync(created);

        var result = await _service.CreateStudentAsync(dto);

        result.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(3);
        result.Data.Email.Should().Be("charlie@test.com");
    }

    [Fact]
    public async Task CreateStudentAsync_DuplicateEmail_ReturnsFailure()
    {
        var dto = new StudentCreateDto { Name = "Charlie", Email = "existing@test.com", Age = 21, Course = "Math" };
        var existing = new Student { Id = 1, Email = "existing@test.com" };

        _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existing);

        var result = await _service.CreateStudentAsync(dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already registered");
    }

    // ── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStudentAsync_ValidData_ReturnsUpdatedStudent()
    {
        var dto = new StudentUpdateDto { Name = "Alice Updated", Email = "alice@test.com", Age = 25, Course = "CS" };
        var existing = new Student { Id = 1, Name = "Alice", Email = "alice@test.com", Age = 20, Course = "CS" };
        var updated = new Student { Id = 1, Name = "Alice Updated", Email = "alice@test.com", Age = 25, Course = "CS" };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existing); // same student
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Student>())).ReturnsAsync(updated);

        var result = await _service.UpdateStudentAsync(1, dto);

        result.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Alice Updated");
    }

    [Fact]
    public async Task UpdateStudentAsync_NotFound_ReturnsFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Student?)null);

        var result = await _service.UpdateStudentAsync(999, new StudentUpdateDto());

        result.Success.Should().BeFalse();
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteStudentAsync_ExistingId_ReturnsSuccess()
    {
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteStudentAsync(1);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteStudentAsync_NotFound_ReturnsFailure()
    {
        _repoMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        var result = await _service.DeleteStudentAsync(999);

        result.Success.Should().BeFalse();
    }
}
