namespace _2reli_api
{
    public class SellProduct
    {
        public int id { get; set; }
        public string Product_name {get;set;}
        public string Product_descr { get;set;}
        public string Product_status { get;set;}
        public string ?Product_type { get;set;}
        public string ?Product_size { get;set;}
        public string Product_quantity { get; set; }
        public string Product_price { get;set;} 
        public int User_id { get; set; }
        public int Sell_status { get; set; }    
    }
}
