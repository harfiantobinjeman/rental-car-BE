namespace rentalcar_backend.Models
{
    public class Cart
    {
        public int fk_car_id { get; set; }
        public int car_rental_days { get; set; }    
        public int fk_user_id { get; set; } 
        public string car_image { get; set; }
        public string car_name { get; set; } 
        public int car_years { get; set; }
        public double car_rental_price { get; set; }

    }

    public class CartList { 
        public List<Cart> listData { get; set; }
    }

}
