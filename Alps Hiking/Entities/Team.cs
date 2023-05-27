using System.ComponentModel.DataAnnotations.Schema;

namespace Alps_Hiking.Entities
{
    public class Team:BaseEntity
    {
        public string Name { get; set; }
        public int ProfessionId { get; set; }
        public Profession Profession { get; set; }
        public string About { get; set; }
        public string ImagePath { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

    }
}
