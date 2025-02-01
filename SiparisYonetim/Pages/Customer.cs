namespace SiparisYonetim.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public decimal Budget { get; set; }
        public string CustomerType { get; set; }
        public decimal TotalSpent { get; set; }
    }
}