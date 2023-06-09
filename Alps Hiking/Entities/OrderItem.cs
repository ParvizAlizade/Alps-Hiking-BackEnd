namespace Alps_Hiking.Entities
{
    public class OrderItem:BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public decimal UnitPrice { get; set; }
        public int SaleQuantity { get; set; }
        public int TourDateId { get; set; }
        public TourDate TourDate { get; set; }
    }
}
