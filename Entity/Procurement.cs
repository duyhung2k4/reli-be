namespace _2reli_api.Entity
{
    public class Procurement
    {public int Id { get; set; }    
        public string Product_name { get; set; }    
        public string Product_descr { get; set;}
        public int Product_quantity { get; set; }
        public string Product_status { get; set; }
        public string Product_price { get; set; }   
        public int Turn_way { get; set; }
        public int User_id { get; set; }
        public int Product_handle { get; set; }
    }
}
