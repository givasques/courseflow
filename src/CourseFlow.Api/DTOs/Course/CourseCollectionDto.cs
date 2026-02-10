using CourseFlow.Api.DTOs.Course;

namespace CourseFlow.Api;

public sealed class CourseCollectionDto
{
    public List<CourseDto> Data { get; set; } = [];
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; } 
}
