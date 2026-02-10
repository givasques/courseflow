using CourseFlow.Api.Data;
using CourseFlow.Api.DTOs.Course;
using CourseFlow.Api.Entities;
using CourseFlow.Api.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace CourseFlow.Api.Services.Application;

public sealed class CourseService(
    ApplicationDbContext applicationDbContext,
    CurrentUserService currentUserService)
{
    public async Task<ServiceResult<CourseDto>> CreateCourse(CreateCourseDto createCourseDto)
    {
        var identityId = currentUserService.IdentityId;
        Instructor? instructor = await applicationDbContext.Instructors
            .FirstOrDefaultAsync(i => i.IdentityId == identityId);

        Course course = createCourseDto.ToEntity();

        if (instructor is not null)
        {
            course.InstructorId = instructor.Id;
        } 
        else 
        {
            bool instructorExists = await applicationDbContext.Instructors
            .AnyAsync(i => i.Id == createCourseDto.InstructorId);

            if (course.InstructorId is null || !instructorExists)
            {
                return ServiceResult<CourseDto>.BadRequest(
                    "Instructor is required", 
                    "Admin must provide a valid InstructorId when creating a course.");
            }
        }

        applicationDbContext.Courses.Add(course);
        await applicationDbContext.SaveChangesAsync();

        CourseDto dto = course.ToDto();

        return ServiceResult<CourseDto>.Ok(dto);
    }

    public async Task<ServiceResult<CourseCollectionDto>> GetCourses(CourseQueryParamsDto query)
    {
        string? lowerQuerySearch = query.Search is null ? null : query.Search.ToLower();
        IQueryable<Course> coursesQuery = applicationDbContext.Courses
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAtUtc)
            .Where(c => string.IsNullOrWhiteSpace(lowerQuerySearch) 
            || c.Title.ToLower().Contains(lowerQuerySearch))
            .Where(c => query.Category == null || query.Category == c.Category);

        int totalCount = await coursesQuery.CountAsync();

        var totalPages = Math.Max(1, (int)Math.Ceiling((double)totalCount / query.PageSize));
        query.Page = query.Page > totalPages ? totalPages : query.Page;

        List<CourseDto> coursesDto = await coursesQuery
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

        return ServiceResult<CourseCollectionDto>.Ok(response);
    }

    public async Task<ServiceResult<CourseDto>> GetCourse(string id)
    {
        CourseDto? courseDto = await applicationDbContext.Courses
            .Where(c => c.Id == id)
            .Select(CourseQueries.QueryToDto())
            .FirstOrDefaultAsync();

        if (courseDto is null)
        {
            return ServiceResult<CourseDto>.NotFound();
        }

        return ServiceResult<CourseDto>.Ok(courseDto);
    }

    public async Task<ServiceResult<CourseDto>> UpdateCourse(string id, UpdateCourseDto updateCourseDto)
    {
        Course? course = await applicationDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
        {
            return ServiceResult<CourseDto>.NotFound();
        }

        var identityId = currentUserService.IdentityId;
        Instructor? instructor = await applicationDbContext.Instructors
            .FirstOrDefaultAsync(i => i.IdentityId == identityId);

        if (instructor is not null && instructor.Id != course.InstructorId)
        {
            return ServiceResult<CourseDto>.Forbidden(
                "Forbidden", 
                "You do not have permission to modify this course.");
        }

        course.UpdateCourse(updateCourseDto);
        await applicationDbContext.SaveChangesAsync();

        return ServiceResult<CourseDto>.NoContent();
    }

    public async Task<ServiceResult<CourseDto>> DeleteCourse(string id)
    {
        Course? course = await applicationDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
        {
            return ServiceResult<CourseDto>.NotFound();
        }

        applicationDbContext.Courses.Remove(course);
        await applicationDbContext.SaveChangesAsync();

        return ServiceResult<CourseDto>.NoContent();
    }
}
