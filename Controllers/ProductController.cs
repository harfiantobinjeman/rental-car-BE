using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

using MySqlConnector;
using System.Data;

// namespace pribadi
using rentalcar_backend.Models;
using rentalcar_backend.Method;
using System.Linq;

namespace rentalcar_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private IConfiguration _configuration;
        private string _conString = "";
        public ProductController(IConfiguration configuration)
        {

            _configuration = configuration;
            _conString = configuration["ConnectionStrings:Default"];
        }


        [HttpGet]
        [Route("HomePageGetData")]
        public IActionResult HomePageGetData(int rowPerPage)
        {
            var result = new List<RegisterCar>();

            try
            {


                using (var connection = new MySqlConnection(_conString))
                {
                    connection.Open();

                    using (var query = new MySqlCommand())
                    {
                        try
                        {
                            query.Connection = connection;

                            query.CommandText = "SELECT * FROM car ORDER BY car_id DESC LIMIT @rowPerPage";

                            query.Parameters.AddRange(new MySqlParameter[]
                            {
                                new MySqlParameter{ ParameterName="@rowPerPage", DbType= DbType.Int32, Value=rowPerPage}
                            });

                            var reader = query.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var temp = new RegisterCar()
                                    {
                                        product_name = reader["car_brand"] is DBNull ? "" : (string)reader["car_brand"],
                                        car_years = reader["car_years"] is DBNull ? 0 : (int)reader["car_years"],
                                        car_rental_price = reader["car_rental_price"] is DBNull ? 0 : (double)reader["car_rental_price"],
                                        car_variant = reader["car_variant"] is DBNull ? "" : (string)reader["car_variant"],
                                        car_image = reader["car_image"] is DBNull ? "" : (string)reader["car_image"],
                                        description = reader["description"] is DBNull ? "" : (string)reader["description"],
                                        keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"]

                                    };
                                    result.Add(temp);
                                }
                            }

                            reader.Close();


                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                            //throw new Exception(e.Message);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                // throw new Exception(e.Message);

            }


            return Ok(result);
        }


        [HttpGet]
        [Route("CategoryPageGetData")]

        public IActionResult CategoryPageGetData(string category, string? sort, double minPrice = 0, double maxPrice = 999999999)
        {
            var result = new List<RegisterCar>();

            try
            {
                using (var connection = new MySqlConnection(_conString))
                {
                    connection.Open();

                    using (var query = new MySqlCommand())
                    {
                        try
                        {
                            query.Connection = connection;

                            string queryData = "SELECT * FROM car WHERE car_variant=@category AND car_rental_price BETWEEN @MinPrice AND @MaxPrice";

                            if (sort == "Newest")
                            {
                                queryData += " ORDER BY car_id DESC";
                                // SELECT* FROM car ORDER BY car_id DESC

                            }
                            else if (sort == "Older")
                            {
                                queryData += " ORDER BY car_id";
                                //  SELECT* FROM car ORDER BY car_id

                            }
                            else if (sort == "High Price")
                            {
                                queryData += " ORDER BY car_rental_price DESC";
                                // SELECT* FROM car ORDER BY car_rental_price
                            }
                            else if (sort == "Low Price")
                            {
                                queryData += " ORDER BY car_rental_price";

                                //SELECT* FROM car ORDER BY car_rental_price DESC
                            }



                            query.CommandText = queryData;

                            query.Parameters.AddRange(new MySqlParameter[]
                             {
                                new MySqlParameter{ ParameterName="@MinPrice", DbType= DbType.Int32, Value=minPrice},
                                new MySqlParameter{ ParameterName="@MaxPrice", DbType= DbType.Int32, Value=maxPrice},
                                new MySqlParameter{ ParameterName="@category", DbType= DbType.String, Value=category},

                             });

                            var reader = query.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var temp = new RegisterCar()
                                    {
                                        product_name = reader["car_brand"] is DBNull ? "" : (string)reader["car_brand"],
                                        car_years = reader["car_years"] is DBNull ? 0 : (int)reader["car_years"],
                                        car_rental_price = reader["car_rental_price"] is DBNull ? 0 : (double)reader["car_rental_price"],
                                        car_variant = reader["car_variant"] is DBNull ? "" : (string)reader["car_variant"],
                                        car_image = reader["car_image"] is DBNull ? "" : (string)reader["car_image"],
                                        car_id = reader["car_id"] is DBNull ? 0 : (int)reader["car_id"],
                                        keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"]

                                    };
                                    result.Add(temp);
                                }
                            }
                            reader.Close();

                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }

                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }

            return Ok(result);
        }


        [HttpGet]
        [Route("CartPageGetData")]

        public IActionResult CartPageGetData()
        {
            var result = new List<object>();
            try
            {
                using (var conn = new MySqlConnection(_conString))
                {
                    conn.Open();

                    using (var query = new MySqlCommand())
                    {
                        query.Connection = conn;
                        query.CommandText = "SELECT car.car_id, car.car_brand, car.car_years, car.car_rental_price, car.car_image, cart.car_rental_days FROM car INNER JOIN cart ON cart.fk_car_id=car.car_id";
                        var reader = query.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var temp = new
                                {
                                    car_id = reader["car_id"] is DBNull ? 0 : (Int32)reader["car_id"],
                                    car_brand = reader["car_brand"] is DBNull ? "" : (string)reader["car_brand"],
                                    car_years = reader["car_years"] is DBNull ? 0 : (Int32)reader["car_years"],
                                    car_rental_price = reader["car_rental_price"] is DBNull ? 0 : (double)reader["car_rental_price"],
                                    car_rental_days = reader["car_rental_days"] is DBNull ? 0 : (Int32)reader["car_rental_days"],
                                    car_image = reader["car_image"] is DBNull ? "" : (string)reader["car_image"]


                                };
                                result.Add(temp);
                            }

                        }
                        reader.Close();


                    }

                    conn.Close();
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("SearchPageGetData")]

        public IActionResult SearchPageGetData(string? keywords, string? sort , string? category, double minPrice = 0, double maxPrice = 999999999)
        {

            var result = new List<object>();
            List<string> categories = new List<string>();
            try
            {
                using (var conn = new MySqlConnection(_conString))
                {
                    conn.Open();

                    using (var query = new MySqlCommand())
                    {

                        string queryData = "SELECT * FROM car WHERE car_rental_price BETWEEN @minPrice AND @maxPrice";

                        if (!String.IsNullOrEmpty(keywords))
                        {
                            queryData += " AND keywords LIKE @keywords";

                        }
                        if (!String.IsNullOrEmpty(category))
                        {
                       
                            // 1."bmw, honda" --> Split() #dari string di split jadi List
                            // 2.["bmw", "honda"] --> ToList().Select() #Hasil List sebelumnya
                            // 3.["'bmw'", "'honda'"] --> string.join() #Hasil List point 2 diproses ulang dengan select() untuk nambah petik 1 ''
                            // 4. "'bmw', 'honda'" --> #Dari List point 3 hasil akhir string
                           categories = (List<string>)category.Split(",").ToList().Select(item => "'" + item + "'").ToList();
            
                            queryData += " AND car_variant  IN (" + string.Join(",", categories) + ")";   //"IN ('bmw', 'honda')"
                        }



                        if (sort == "Newest")
                        {
                            queryData += " ORDER BY car_id DESC";
                            // SELECT* FROM car ORDER BY car_id DESC

                        }
                        else if (sort == "Older")
                        {
                            queryData += " ORDER BY car_id";
                            //  SELECT* FROM car ORDER BY car_id

                        }
                        else if (sort == "High Price")
                        {
                            queryData += " ORDER BY car_rental_price DESC";
                            // SELECT* FROM car ORDER BY car_rental_price
                        }
                        else if (sort == "Low Price")
                        {
                            queryData += " ORDER BY car_rental_price";

                            //SELECT* FROM car ORDER BY car_rental_price DESC
                        }

                        query.Parameters.AddRange(new MySqlParameter[]
                             {
                                new MySqlParameter{ ParameterName="@keywords", DbType= DbType.Int32, Value="%"+keywords+"%"},
                                //new MySqlParameter{ ParameterName="@category", DbType= DbType.String, Value=string.Join(",", categories)},
                                new MySqlParameter{ ParameterName="@minPrice",DbType= DbType.Double, Value=minPrice},
                                new MySqlParameter{ ParameterName="@maxPrice",DbType= DbType.Double, Value=maxPrice},

                             });



                        query.Connection = conn;
                        query.CommandText = queryData;

                        var reader = query.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var temp = new
                                {
                                    product_name = reader["car_brand"] is DBNull ? "" : (string)reader["car_brand"],
                                    car_years = reader["car_years"] is DBNull ? 0 : (int)reader["car_years"],
                                    car_rental_price = reader["car_rental_price"] is DBNull ? 0 : (double)reader["car_rental_price"],
                                    car_variant = reader["car_variant"] is DBNull ? "" : (string)reader["car_variant"],
                                    car_image = reader["car_image"] is DBNull ? "" : (string)reader["car_image"],
                                    car_id = reader["car_id"] is DBNull ? 0 : (int)reader["car_id"],
                                    description = reader["description"] is DBNull ? "" : (string)reader["description"],
                                    keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"]


                                };
                                result.Add(temp);
                            }

                        }
                        reader.Close();


                    }

                    conn.Close();
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(result);
        }


        [HttpGet]
        [Route("InvoiceGetData")]

        public IActionResult InvoiceGetData(int userid)
        {
            var result = new List<object>();

            try
            {
                using (var conn = new MySqlConnection(_conString))
                {
                    conn.Open();
                    using (var query = new MySqlCommand())
                    {
                        string queryData = "SELECT * FROM invoices WHERE fk_user_id = @userid ORDER BY invoice_id DESC";
                        query.Parameters.AddRange(new MySqlParameter[]
                        {
                        new MySqlParameter{ ParameterName="@userid", DbType= DbType.Int32, Value=userid}
                        });
                        query.Connection = conn;
                        query.CommandText = queryData;
                        var reader = query.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var temp = new
                                {
                                    invoice_id = reader["invoice_id"] is DBNull ? 0 : (int)reader["invoice_id"],
                                    fk_user_id = reader["fk_user_id"] is DBNull ? 0 : (int)reader["fk_user_id"],
                                    fk_car_id = reader["fk_car_id"] is DBNull ? 0 : (int)reader["fk_car_id"],
                                    no_invoice = reader["No_invoice"] is DBNull ? 0 : (int)reader["No_invoice"],
                                    buy_date = reader["buy_date"] is DBNull ? DateTime.Now : (DateTime)reader["buy_date"],
                                    total_item = reader["Total_item"] is DBNull ? 0 : (int)reader["Total_item"],
                                    total_price = reader["Total_price"] is DBNull ? 0 : (double)reader["Total_price"]
                                };
                                result.Add(temp);
                            }
                        }
                        reader.Close();
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(200, result);
        }

        [HttpGet]
        [Route("InvoiceGetDataAll")]

        public IActionResult InvoiceGetDataAll()
        {
            var result = new List<object>();

            try
            {
                using (var conn = new MySqlConnection(_conString))
                {
                    conn.Open();
                    using (var query = new MySqlCommand())
                    {
                        string queryData = "SELECT * FROM invoices INNER JOIN user ON invoices.fk_user_id = user.user_id INNER JOIN car ON invoices.fk_car_id = car.car_id";
                        query.Parameters.AddRange(new MySqlParameter[]
                        {
                        //new MySqlParameter{ ParameterName="@userid", DbType= DbType.Int32, Value=userid}
                        });
                        query.Connection = conn;
                        query.CommandText = queryData;
                        var reader = query.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var temp = new
                                {
                                    invoice_id = reader["invoice_id"] is DBNull ? 0 : (int)reader["invoice_id"],
                                    fk_user_id = reader["fk_user_id"] is DBNull ? 0 : (int)reader["fk_user_id"],
                                    fk_car_id = reader["fk_car_id"] is DBNull ? 0 : (int)reader["fk_car_id"],
                                    no_invoice = reader["No_invoice"] is DBNull ? 0 : (int)reader["No_invoice"],
                                    buy_date = reader["buy_date"] is DBNull ? DateTime.Now : (DateTime)reader["buy_date"],
                                    total_item = reader["Total_item"] is DBNull ? 0 : (int)reader["Total_item"],
                                    total_price = reader["Total_price"] is DBNull ? 0 : (double)reader["Total_price"],
                                    name = reader["username"] is DBNull ? "" : (string)reader["username"],
                                    car_brand = reader["car_brand"] is DBNull ? "" : (string)reader["car_brand"]
                                };
                                result.Add(temp);
                            }
                        }
                        reader.Close();
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(200, result);
        }

        [HttpGet]
        [Route("InvoiceDetailGetData")]

        public IActionResult InvoiceDetailGetData(int invoiceid)
        {
            var result = new List<object>();

            try
            {
                using (var conn = new MySqlConnection(_conString))
                {
                    conn.Open();
                    using (var query = new MySqlCommand())
                    {
                        string queryData = "SELECT * FROM invoices INNER JOIN car ON invoices.fk_car_id = car.car_id WHERE invoice_id = @invoiceid ";
                        query.Parameters.AddRange(new MySqlParameter[]
                        {
                        new MySqlParameter{ ParameterName="@invoiceid", DbType= DbType.Int32, Value=invoiceid}
                        });
                        query.Connection = conn;
                        query.CommandText = queryData;
                        var reader = query.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var temp = new
                                {
                                    invoice_id = reader["invoice_id"] is DBNull ? 0 : (int)reader["invoice_id"],
                                    fk_user_id = reader["fk_user_id"] is DBNull ? 0 : (int)reader["fk_user_id"],
                                    fk_car_id = reader["fk_car_id"] is DBNull ? 0 : (int)reader["fk_car_id"],
                                    no_invoice = reader["No_invoice"] is DBNull ? 0 : (int)reader["No_invoice"],
                                    buy_date = reader["buy_date"] is DBNull ? DateTime.Now : (DateTime)reader["buy_date"],
                                    total_item = reader["Total_item"] is DBNull ? 0 : (int)reader["Total_item"],
                                    car_rental_price = reader["car_rental_price"] is DBNull ? 0 : (double)reader["car_rental_price"],
                                    total_price = reader["Total_price"] is DBNull ? 0 : (double)reader["Total_price"],
                                    car_variant = reader["car_variant"] is DBNull ? "" : (string)reader["car_variant"],
                                    car_brand = reader["car_brand"] is DBNull ? "" : (string)reader["car_brand"],
                                    car_image = reader["car_image"] is DBNull ? "" : (string)reader["car_image"],
                                    car_years = reader["car_years"] is DBNull ? 0 : (int)reader["car_years"]
                                };
                                result.Add(temp);
                            }
                        }
                        reader.Close();
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(200, result);
        }

        [HttpGet]
        [Route("RecommendationGetData")]

        public IActionResult RecommendationGetData(int rowsPerPage)
        {
            var result = new List<RegisterCar>();

            try
            {

                using (var connection = new MySqlConnection(_conString))
                {
                    connection.Open();

                    using (var query = new MySqlCommand())
                    {
                        try
                        {
                            query.Connection = connection;

                            query.CommandText = "SELECT * FROM car ORDER BY soldCounter DESC LIMIT @rowPerPage";

                            query.Parameters.AddRange(new MySqlParameter[]
                            {
                                new MySqlParameter{ ParameterName="@rowPerPage", DbType= DbType.Int32, Value= rowsPerPage}
                            });

                            var reader = query.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var temp = new RegisterCar()
                                    {
                                        product_name = reader["car_brand"] is DBNull ? "" : (string)reader["car_brand"],
                                        car_years = reader["car_years"] is DBNull ? 0 : (int)reader["car_years"],
                                        car_rental_price = reader["car_rental_price"] is DBNull ? 0 : (double)reader["car_rental_price"],
                                        car_variant = reader["car_variant"] is DBNull ? "" : (string)reader["car_variant"],
                                        car_image = reader["car_image"] is DBNull ? "" : (string)reader["car_image"],
                                        description = reader["description"] is DBNull ? "" : (string)reader["description"],
                                        keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"]

                                    };
                                    result.Add(temp);
                                }
                            }

                            reader.Close();


                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                            //throw new Exception(e.Message);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                // throw new Exception(e.Message);

            }


            return Ok(result);
        }


        [HttpGet]
        [Route("GetDetailProduct")]

        public IActionResult GetDetailProduct(string carName, int carYears, int daysRented, bool checkStatus)
        {
            var result = new List<RegisterCar>();

            try
            {


                using (var connection = new MySqlConnection(_conString))
                {
                    connection.Open();

                    using (var query = new MySqlCommand())
                    {
                        try
                        {
                            query.Connection = connection;

                            query.CommandText = "SELECT * FROM car WHERE car_brand = @carName AND car_years = @carYears";

                            query.Parameters.AddRange(new MySqlParameter[]
                            {
                                new MySqlParameter{ ParameterName="@carName", DbType= DbType.String, Value=carName},
                                new MySqlParameter{ ParameterName="@carYears", DbType = DbType.Int32, Value=carYears}
                            });

                            var reader = query.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var temp = new RegisterCar()
                                    {
                                        car_id = reader["car_id"] is DBNull ? 0 : (int)reader["car_id"],
                                        product_name = reader["car_brand"] is DBNull ? "" : (string)reader["car_brand"],
                                        car_years = reader["car_years"] is DBNull ? 0 : (int)reader["car_years"],
                                        car_rental_price = reader["car_rental_price"] is DBNull ? 0 : (double)reader["car_rental_price"],
                                        car_variant = reader["car_variant"] is DBNull ? "" : (string)reader["car_variant"],
                                        car_image = reader["car_image"] is DBNull ? "" : (string)reader["car_image"],
                                        description = reader["description"] is DBNull ? "" : (string)reader["description"],
                                        keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"]

                                    };
                                    result.Add(temp);
                                }
                            }

                            reader.Close();


                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                            //throw new Exception(e.Message);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                // throw new Exception(e.Message);

            }


            return Ok(result);
        }

        [HttpGet]
        [Route("GetSingleProduct")]

        public IActionResult GetSingleProduct(int carid)
        {
            var result = new List<RegisterCar>();

            try
            {


                using (var connection = new MySqlConnection(_conString))
                {
                    connection.Open();

                    using (var query = new MySqlCommand())
                    {
                        try
                        {
                            query.Connection = connection;

                            query.CommandText = "SELECT * FROM car WHERE car_id = @carid";

                            query.Parameters.AddRange(new MySqlParameter[]
                            {
                                new MySqlParameter{ ParameterName="@carid", DbType= DbType.String, Value=carid}
                            });

                            var reader = query.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var temp = new RegisterCar()
                                    {
                                        car_id = reader["car_id"] is DBNull ? 0 : (int)reader["car_id"],
                                        product_name = reader["car_brand"] is DBNull ? "" : (string)reader["car_brand"],
                                        car_years = reader["car_years"] is DBNull ? 0 : (int)reader["car_years"],
                                        car_rental_price = reader["car_rental_price"] is DBNull ? 0 : (double)reader["car_rental_price"],
                                        car_variant = reader["car_variant"] is DBNull ? "" : (string)reader["car_variant"],
                                        car_image = reader["car_image"] is DBNull ? "" : (string)reader["car_image"],
                                        description = reader["description"] is DBNull ? "" : (string)reader["description"],
                                        keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"]

                                    };
                                    result.Add(temp);
                                }
                            }

                            reader.Close();


                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                            //throw new Exception(e.Message);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                // throw new Exception(e.Message);

            }


            return Ok(result);
        }





    }
}
