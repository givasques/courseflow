using System.Linq.Expressions;
using System.Security.Claims;
using CourseFlow.Api.Data;
using CourseFlow.Api.DTOs.Course;
using CourseFlow.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseFlow.Api.Controllers;

[ApiController]
[Route("courses")]
public sealed class CourseController(ApplicationDbContext applicationDbContext) : ControllerBase
{
    [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
    [HttpPost]
    public async Task<IActionResult> CreateCourse(CreateCourseDto createCourseDto)
    {
        var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Instructor? instructor = await applicationDbContext.Instructors.FirstOrDefaultAsync(i => i.IdentityId == identityId);

        Course course = createCourseDto.toEntity();

        if (instructor is not null)
        {
            course.InstructorId = instructor.Id;
        }

        applicationDbContext.Courses.Add(course);
        await applicationDbContext.SaveChangesAsync();

        CourseDto dto = course.ToDto();

        return CreatedAtAction(nameof(GetCourse), new { course.Id }, dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(string id)
    {
        CourseDto? courseDto = applicationDbContext.Courses
            .Where(c => c.Id == id)
            .Select(CourseQueries.QueryToDto())
            .FirstOrDefault();

        if (courseDto is null)
        {
            return NotFound();
        }

        return Ok(courseDto);
    } 
}
