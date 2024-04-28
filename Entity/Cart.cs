namespace _2reli_api
{
    public class Cart
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public int Product_id { get; set; }
        public int Quantity { get; set; }
        public string ?Product_price { get; set; }
        public int Product_status { get; set; }
        public string ?Product_type { get; set; }
        public string Product_name { get; set; }
        public string Product_seller { get; set; }

    }
}
