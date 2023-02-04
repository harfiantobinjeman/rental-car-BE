using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

// namespace pribadi
using rentalcar_backend.Models;
using rentalcar_backend.Method;

// CRYPTO AND AUTHHH
using System.Security.Cryptography;
using System.Text;
using System.Data;

// AUTHHH
using System.Security.Claims;

namespace rentalcar_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private IConfiguration _configuration;
        private string _conString = "";
        public AdminController(IConfiguration configuration) {

            _configuration = configuration;
            _conString = configuration["ConnectionStrings:Default"];
        }

        [HttpPost]
        [Route("SellerProductPost")]

        public IActionResult SellerProductPost([FromBody] ButtonType product)
        {
            try
            {
                if (product.type == "add")
                {

                    using (var connection = new MySqlConnection(_conString))
                    {
                        connection.Open();

                        using (var query = new MySqlCommand())
                        {
                            try
                            {
                                query.Connection = connection;

                                query.CommandText = "INSERT INTO car (car_brand,car_years,car_rental_price,car_image,car_variant,description,keywords,fk_admin_id) VALUES (@product_name,@car_years,@car_rental_price,@car_image,@car_variant,@description,@keywords, @fk_admin_id)";
                                query.Transaction = connection.BeginTransaction();

                                query.Parameters.AddRange(new MySqlParameter[]
                                {
                                    new MySqlParameter{ ParameterName="@product_name",DbType=DbType.String,Value =String.IsNullOrEmpty(product.product_name) ? "" : product.product_name},
                                    new MySqlParameter{ ParameterName="@car_years",DbType=DbType.Int32,Value = product.car_years},
                                    new MySqlParameter{ ParameterName="@car_rental_price",DbType=DbType.Double,Value = product.car_rental_price},
                                    new MySqlParameter{ ParameterName="@car_image",DbType=DbType.String,Value =String.IsNullOrEmpty(product.car_image) ? "" : product.car_image},
                                    new MySqlParameter{ ParameterName="@car_variant",DbType=DbType.String,Value =String.IsNullOrEmpty(product.car_variant) ? "" : product.car_variant},
                                    new MySqlParameter{ ParameterName="@description",DbType=DbType.String,Value =String.IsNullOrEmpty(product.description) ? "" : product.description},
                                    new MySqlParameter{ ParameterName="@keywords",DbType=DbType.String,Value =String.IsNullOrEmpty(product.keywords) ? "" : product.keywords},
                                    new MySqlParameter{ ParameterName="@fk_admin_id",DbType=DbType.Int32,Value =product.fk_admin_id},


                                });

                                try
                                {
                                    var result = query.ExecuteNonQuery();
                                    if (result == -1)
                                    {
                                        query.Transaction.Rollback();
                                    }
                                    else
                                    {
                                        query.Transaction.Commit();
                                    }



                                }
                                catch (Exception e)
                                {
                                    query.Transaction.Rollback();

                                    connection.Close();
                                    throw new Exception(e.Message);
                                }




                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message);
                            }

                        }

                    }

                }else if(product.type == "edit")
                {
                    try
                    {
                        var result = new List<RegisterCar>();

                        using (var connection = new MySqlConnection(_conString))
                        {
                            connection.Open();

                            using (var query = new MySqlCommand())
                            {

                                try
                                {

                                    query.Connection = connection;

                                    query.CommandText = "UPDATE car SET car_brand = @product_name, car_image = @car_image, car_rental_price = @car_rental_price, car_variant = @car_variant, car_years = @car_years, keywords = @keywords, description = @description WHERE car_id = @car_id";

                                    query.Parameters.AddRange(new MySqlParameter[]
                                    {
                                    new MySqlParameter{ ParameterName="@product_name",DbType=DbType.Int32 ,Value = product.product_name},
                                    new MySqlParameter{ ParameterName="@car_image",DbType=DbType.Int32,Value = product.car_image},
                                    new MySqlParameter{ ParameterName="@car_rental_price",DbType=DbType.Int32 ,Value = product.car_rental_price},
                                    new MySqlParameter{ ParameterName="@car_variant",DbType=DbType.Int32 ,Value = product.car_variant},
                                    new MySqlParameter{ ParameterName="@car_years",DbType=DbType.Int32 ,Value = product.car_years},
                                    new MySqlParameter{ ParameterName="@keywords",DbType=DbType.Int32 ,Value = product.keywords},
                                    new MySqlParameter{ ParameterName="@description",DbType=DbType.Int32 ,Value = product.description},
                                    new MySqlParameter{ ParameterName="@car_id" ,DbType=DbType.Int32,Value=product.car_id}

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
                                                keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"],



                                            };
                                            result.Add(temp);


                                        }
                                    }

                                    reader.Close();

                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                }

                            }

                            connection.Close();
                        }
                        return Ok(result);
                    }


                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }else
                {
                    throw new Exception("button type is neither add or edit!");
                }

            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }   
            return StatusCode(200, "Oke");
        }

        [HttpGet]
        [Route("SellerProductGet")]

        public IActionResult SellerProductGet(int rowPerPage,int page)
        {
            try
            {
                var result = new List<RegisterCar>();

                    using (var connection = new MySqlConnection(_conString))
                    {
                        connection.Open();

                        using (var query = new MySqlCommand())
                        {
                         
                            try
                            {   
                                 
                                query.Connection = connection;

                                query.CommandText = "SELECT * FROM car ORDER BY car_id DESC LIMIT @rowPerPage OFFSET @offset";

                                query.Parameters.AddRange(new MySqlParameter[]
                                {
                                    new MySqlParameter{ ParameterName="@rowPerPage",DbType=DbType.Int32 ,Value = rowPerPage},
                                    new MySqlParameter{ ParameterName="@offset",DbType=DbType.Int32,Value = (page-1) * rowPerPage},
                                    
                                });
                               var reader =  query.ExecuteReader();
                            if(reader.HasRows)
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
                                        keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"],


                                     
                                    };
                                    result.Add(temp);


                                }    
                            }

                            reader.Close();

                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message);
                            }

                        }

                        connection.Close();
                    }
                return Ok(result) ;
                }

            
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
           
        }

        [HttpGet]
        [Route("SellerEditProduct")]

        //belum selesai
        public IActionResult SellerEditProduct(string carbrand, string carimage, double carrentalprice, string type, int years, string tags, string desc)
        {
            try
            {
                var result = new List<RegisterCar>();

                using (var connection = new MySqlConnection(_conString))
                {
                    connection.Open();

                    using (var query = new MySqlCommand())
                    {

                        try
                        {

                            query.Connection = connection;

                            query.CommandText = "UPDATE car SET car_brand = @carbrand, car_image = @carimage, car_rental_price = @carRentalPrice, car_variant = @carvariant, car_years = @caryear, keywords = @keywords, description = @description WHERE car_brand = @carbrand";

                            query.Parameters.AddRange(new MySqlParameter[]
                            {
                                    new MySqlParameter{ ParameterName="@carbrand",DbType=DbType.Int32 ,Value = carbrand},
                                    new MySqlParameter{ ParameterName="@carimage",DbType=DbType.Int32,Value = carimage},
                                    new MySqlParameter{ ParameterName="@carRentalPrice",DbType=DbType.Int32 ,Value = carrentalprice},
                                    new MySqlParameter{ ParameterName="@carvariant",DbType=DbType.Int32 ,Value = type},
                                    new MySqlParameter{ ParameterName="@caryear",DbType=DbType.Int32 ,Value = years},
                                    new MySqlParameter{ ParameterName="@keywords",DbType=DbType.Int32 ,Value = tags},
                                    new MySqlParameter{ ParameterName="@description",DbType=DbType.Int32 ,Value = desc},

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
                                        keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"],



                                    };
                                    result.Add(temp);


                                }
                            }

                            reader.Close();

                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }

                    }

                    connection.Close();
                }
                return Ok(result);
            }


            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("SellerDeleteProduct")]

        public IActionResult SellerDeleteProduct(string productname)
        {
            try
            {
                var result = new List<RegisterCar>();

                using (var connection = new MySqlConnection(_conString))
                {
                    connection.Open();

                    using (var query = new MySqlCommand())
                    {

                        try
                        {

                            query.Connection = connection;

                            query.CommandText = "DELETE FROM car WHERE car_brand = @productname";

                            query.Parameters.AddRange(new MySqlParameter[]
                            {
                                    new MySqlParameter{ ParameterName="@productname",DbType=DbType.Int32 ,Value = productname},
                                   
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
                                        keywords = reader["keywords"] is DBNull ? "" : (string)reader["keywords"],



                                    };
                                    result.Add(temp);


                                }
                            }

                            reader.Close();

                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }

                    }

                    connection.Close();
                }
                return Ok(result);
            }


            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }







    }
}
