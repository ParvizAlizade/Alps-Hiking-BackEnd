using System.ComponentModel.DataAnnotations.Schema;

namespace Alps_Hiking.Entities
{
    public class Destiantion:BaseEntity
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }
        public List<Tour> Tours { get; set; }

    }
}
