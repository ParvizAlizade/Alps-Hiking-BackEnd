namespace Alps_Hiking.Entities
{
    public class Profession:BaseEntity
    {
        public string Name { get; set;}
        public List<Team> Teams  { get; set; }
    }
}
