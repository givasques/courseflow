using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using CourseFlow.Api.Controllers.Extensions;
using CourseFlow.Api.Data;
using CourseFlow.Api.DTOs.Course;
using CourseFlow.Api.Entities;
using CourseFlow.Api.Services.Application;
using CourseFlow.Api.Services.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseFlow.Api.Controllers;

[ApiController]
[Route("courses")]
public sealed class CourseController(CourseService courseService) : ControllerBase
{
    [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
    [HttpPost]
    public async Task<IActionResult> CreateCourse(CreateCourseDto createCourseDto)
    {
        ServiceResult<CourseDto> result = await courseService.CreateCourse(createCourseDto);
        
        if (!result.Success)
            return this.FromServiceResult(result);

        return CreatedAtAction(nameof(GetCourse), new { result.Data!.Id }, result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses([FromQuery] CourseQueryParamsDto query)
    {
        ServiceResult<CourseCollectionDto> result = await courseService.GetCourses(query);
        return this.FromServiceResult(result);
    } 

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(string id)
    {
        ServiceResult<CourseDto> result = await courseService.GetCourse(id);
        return this.FromServiceResult(result);
    } 

    [Authorize(Roles = $"{Roles.Admin},{Roles.Instructor}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(string id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        ServiceResult<CourseDto> result = await courseService.UpdateCourse(id, updateCourseDto);
        return this.FromServiceResult(result);
    } 
    
    [Authorize(Roles=Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(string id)
    {
        ServiceResult<CourseDto> result = await courseService.DeleteCourse(id);
        return this.FromServiceResult(result);
    }
}
