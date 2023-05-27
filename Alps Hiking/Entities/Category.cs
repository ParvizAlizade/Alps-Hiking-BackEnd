namespace Alps_Hiking.Entities
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public List<Tour> Tours { get; set; }
        public Category()
        {
            Tours = new List<Tour>();
        }
    }
}
