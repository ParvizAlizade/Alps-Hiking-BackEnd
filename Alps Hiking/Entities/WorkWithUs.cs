using System.ComponentModel.DataAnnotations.Schema;

namespace Alps_Hiking.Entities
{
    public class WorkWithUs:BaseEntity
    {
        public string Desc { get; set; }
        public List<AboutUs> AboutUs { get; set; }
        public string? ImagePath { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }

    }
}
