using System.Linq.Expressions;

namespace CourseFlow.Api.DTOs.Course;

public static class CourseQueries
{
    public static Expression<Func<Entities.Course, CourseDto>> QueryToDto()
    {
        return c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Category = c.Category,
            WorkloadHours = c.WorkloadHours,
            CreatedAtUtc = c.CreatedAtUtc,
            UpdatedAtUtc = c.UpdatedAtUtc,
            InstructorId = c.InstructorId
        };
    }
}
