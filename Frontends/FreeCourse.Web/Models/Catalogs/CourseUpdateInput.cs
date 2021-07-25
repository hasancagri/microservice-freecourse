using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Catalogs
{
    public class CourseUpdateInput
    {
        public string Id { get; set; }

        [Display(Name = "Kurs ismi")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Kurs fiyat")]
        [Required]
        public decimal Price { get; set; }

        [Display(Name = "Kurs açıklama")]
        [Required]
        public string Description { get; set; }

        public string UserId { get; set; }
        public string Picture { get; set; }
        public FeatureViewModel Feature { get; set; }
        public string CategoryId { get; set; }

        [Display(Name = "Kurs Resim")]
        public IFormFile PhotoFormFile { get; set; }
    }
}
