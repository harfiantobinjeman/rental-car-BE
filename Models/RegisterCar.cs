namespace rentalcar_backend.Models
{
    public class RegisterCar
    {
        //car_id 
        public int car_id { get; set; }
        //Product name input seller
        public string product_name { get; set; }
        //Year input seller
        public int car_years { get; set; }
        //Price input seller
        public double car_rental_price { get; set; }
        //Car image input seller
        public string car_image { get; set; }
        //Type/Category input seller
        public string car_variant { get; set; }
        //Description input seller
        public string description { get; set; }
        //Tags input seller
        public string keywords  { get; set; }

        //fk_admin_id 
        public int fk_admin_id { get; set; }
    }
}
