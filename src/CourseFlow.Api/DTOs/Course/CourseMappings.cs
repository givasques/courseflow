namespace CourseFlow.Api.DTOs.Course;

public static class CourseMappings
{
    public static Entities.Course toEntity(this CreateCourseDto createCourseDto)
    {
        return new Entities.Course ()
        {
            Id = $"c_{Guid.NewGuid()}",
            Title = createCourseDto.Title,
            Description = createCourseDto.Description,
            Category = createCourseDto.Category,
            WorkloadHours = createCourseDto.WorkloadHours,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    public static CourseDto ToDto(this Entities.Course course)
    {
        return new CourseDto()
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Category = course.Category,
            WorkloadHours = course.WorkloadHours,
            CreatedAtUtc = course.CreatedAtUtc,
            UpdatedAtUtc = course.UpdatedAtUtc,
            InstructorId = course.InstructorId
        };
    }
}
