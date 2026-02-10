using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using CourseFlow.Api.Data;
using CourseFlow.Api.DTOs.Course;
using CourseFlow.Api.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        Instructor? instructor = await applicationDbContext.Instructors
            .FirstOrDefaultAsync(i => i.IdentityId == identityId);

        Course course = createCourseDto.ToEntity();

        if (instructor is not null)
        {
            course.InstructorId = instructor.Id;
        }

        applicationDbContext.Courses.Add(course);
        await applicationDbContext.SaveChangesAsync();

        CourseDto dto = course.ToDto();

        return CreatedAtAction(nameof(GetCourse), new { course.Id }, dto);
    }

    [HttpGet]
    public async Task<ActionResult<CourseCollectionDto>> GetCourses([FromQuery] CourseQueryParamsDto query)
    {
        string? lowerQuerySearch = query.Search is null ? null : query.Search.ToLower();
        IQueryable<Course>? coursesQuery = applicationDbContext.Courses
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAtUtc)
            .Where(c => string.IsNullOrWhiteSpace(lowerQuerySearch) 
            || c.Title.ToLower().Contains(lowerQuerySearch))
            .Where(c => query.Category == null || query.Category == c.Category);

        int totalCount = await coursesQuery.CountAsync();

        var totalPages = Math.Max(1, (int)Math.Ceiling((double)totalCount / query.PageSize));
        query.Page = query.Page > totalPages ? totalPages : query.Page;

        List<CourseDto>? coursesDto = await coursesQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(CourseQueries.QueryToDto())
            .ToListAsync();

        var response = new CourseCollectionDto
        {
            Data = coursesDto,
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = query.Page,
            HasPreviousPage = query.Page > 1
        };
        response.HasNextPage = query.Page < response.TotalPages;

        return Ok(response);
    } 

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(string id)
    {
        CourseDto? courseDto = await applicationDbContext.Courses
            .Where(c => c.Id == id)
            .Select(CourseQueries.QueryToDto())
            .FirstOrDefaultAsync();

        if (courseDto is null)
        {
            return NotFound();
        }

        return Ok(courseDto);
    } 

    [Authorize(Roles = $"{Roles.Admin},{Roles.Instructor}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(string id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        Course? course = await applicationDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
        {
            return NotFound();
        }

        var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Instructor? instructor = await applicationDbContext.Instructors
            .FirstOrDefaultAsync(i => i.IdentityId == identityId);

        if (instructor is not null && instructor.Id != course.InstructorId)
        {
            return Forbid();
        }

        course.UpdateCourse(updateCourseDto);
        await applicationDbContext.SaveChangesAsync();

        return NoContent();
    } 
    
    [Authorize(Roles=Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(string id)
    {
        Course? course = await applicationDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
        {
            return NotFound();
        }

        applicationDbContext.Courses.Remove(course);
        await applicationDbContext.SaveChangesAsync();

        return NoContent();
    }
}
