using Finances.DTOs;
using Finances.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finances.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController(AppDbContext context) : ControllerBase
{
    private static CategoryGetDto CategoryToDto(CategoryEntity category) =>
        new CategoryGetDto()
        {
            Id = category.Id,
            Title = category.Title,
            ParentId = category.ParentCategoryId,
            SubCategories = category.SubCategories
                .Select(CategoryToDto)
                .ToList()
        };
    
    /// <summary>
    /// Get all existing categories
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<CategoryGetDto>> Get()
    {
        var categories = await context.Categories
            .Include(c => c.SubCategories)
            .ToListAsync();

        var categoryDict = categories.ToDictionary(c => c.Id);
        foreach (var category in categories)
        {
            category.SubCategories = categories
                .Where(c => c.ParentCategoryId == category.Id)
                .ToList();
        }

        var rootCategories = categories
            .Where(c => c.ParentCategoryId == null)
            .ToList();

        return rootCategories.Select(CategoryToDto).ToList();
    }

    /// <summary>
    /// Provide category id to see detail info about this category
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryEntity>> GetById(Guid id)
    {
        var category = await context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        return category == null ? NotFound() : Ok(category);
    }

    /// <summary>
    /// Provide category id to update the information about the category.
    /// </summary>
    /// <param name="id"></param>
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryCreateDto>> Put(Guid id, CategoryCreateDto categoryDto)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null) return BadRequest();
        category.Title = categoryDto.Title;
        category.Description = categoryDto.Description;
        category.Icon = categoryDto.Icon;
        category.ParentCategoryId = categoryDto.ParentCategoryId;
        await context.SaveChangesAsync();
        return Ok(category);
    }

    /// <summary>
    /// Create category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoryEntity>> Post(CategoryCreateDto categoryDto)
    {
        var category = new CategoryEntity()
        {
            Title = categoryDto.Title,
            Description = categoryDto.Description,
            Icon = categoryDto.Icon,
            ParentCategoryId = categoryDto.ParentCategoryId
        };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(Post), new { id = category.Id }, category);
    }

    /// <summary>
    /// Deleting a category by its id
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await context.Categories.FindAsync(id);
        if(category == null) return NotFound();
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return NoContent();
    }
}