using Finances.DTOs;
using Finances.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finances.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

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
    
    [HttpGet]
    public async Task<IEnumerable<CategoryGetDto>> Get()
    {
        var categories = await _context.Categories
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

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryEntity>> GetById(Guid id)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        return category == null ? NotFound() : Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryCreateDto>> Put(Guid id, CategoryCreateDto categoryDto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return BadRequest();
        category.Title = categoryDto.Title;
        category.Description = categoryDto.Description;
        category.Icon = categoryDto.Icon;
        category.ParentCategoryId = categoryDto.ParentCategoryId;
        await _context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryEntity>> Post(CategoryCreateDto categoryDto)
    {
        var user = new CategoryEntity()
        {
            Title = categoryDto.Title,
            Description = categoryDto.Description,
            Icon = categoryDto.Icon,
            ParentCategoryId = categoryDto.ParentCategoryId
        };
        await _context.Categories.AddAsync(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Post), new { id = user.Id }, user);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if(category == null) return NotFound();
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}