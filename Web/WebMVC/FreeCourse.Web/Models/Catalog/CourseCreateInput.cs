using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Catalog;

public class CourseCreateInput
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    public decimal Price { get; set; }

    public string? UserId { get; set; }

    public string? Picture { get; set; }

    public FeatureViewModel? Feature { get; set; }

    public string? CategoryId { get; set; }
}