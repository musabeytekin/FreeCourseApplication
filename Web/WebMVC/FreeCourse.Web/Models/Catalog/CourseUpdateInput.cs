using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Catalog;

public class CourseUpdateInput
{
    
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public string UserId { get; set; }

    public string Picture { get; set; }

    public FeatureViewModel Feature { get; set; }

    public string CategoryId { get; set; }

    public IFormFile PhotoFormFile { get; set; }
}