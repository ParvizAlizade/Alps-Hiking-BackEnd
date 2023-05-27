using System.ComponentModel.DataAnnotations.Schema;

namespace Alps_Hiking.Entities
{
    public class Partner:BaseEntity
    {
        public string ImagePath { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }
    }
}
