using System.ComponentModel.DataAnnotations.Schema;

namespace Alps_Hiking.Entities
{
    public class Slider :BaseEntity
    {
        public string? ImagePath { get; set; }
        public string Title { get; set; }
        public string Button { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
    }
}
