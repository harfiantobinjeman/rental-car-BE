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
    public class UserController : Controller
    {
        private IConfiguration _configuration;
        private string _conString = "";
        public UserController(IConfiguration configuration)
        {

            _configuration = configuration;
            _conString = configuration["ConnectionStrings:Default"];
        }

        [HttpPost]
        [Route("CartPagePostData")]
        public IActionResult CartPagePostdata([FromBody] CartList body)
        {
            List<Cart> cartlist = body.listData; 


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
                            query.Transaction = connection.BeginTransaction();
                            foreach (Cart data in cartlist)
                            {
                                query.CommandText = "INSERT INTO invoices (Buy_Date,Total_Item,Total_Price,fk_user_id,fk_car_id) VALUES (@buyDate,@total_item,@total_price,@fk_user_id,@fk_car_id)";
                                //query.Parameters.Remove()
                                query.Parameters.AddRange(new MySqlParameter[]
                                {
                                    //new MySqlParameter{ ParameterName="@cart_id",DbType=DbType.Int32,Value = body.cart_id},
                                    //new MySqlParameter{ ParameterName="@no_invoice",DbType=DbType.Int32,Value = body},
                                    new MySqlParameter{ ParameterName="@buyDate",DbType=DbType.DateTime,Value =DateTime.Now },
                                    new MySqlParameter{ ParameterName="@total_item",DbType=DbType.Int32,Value =data.car_rental_days},
                                    new MySqlParameter{ ParameterName="@total_price",DbType=DbType.Double,Value =data.car_rental_price * data.car_rental_days},
                                    new MySqlParameter{ ParameterName="@fk_user_id",DbType=DbType.Int32,Value =data.fk_user_id},
                                    new MySqlParameter{ ParameterName="@fk_car_id",DbType=DbType.Int32,Value =data.fk_car_id}

                                });         
                        
                                query.ExecuteNonQuery();
                                query.Parameters.Clear();

                            }
                            query.Transaction.Commit();


                        }
                        catch (Exception e)
                        {
                            query.Transaction.Rollback();

                            throw new Exception(e.Message);
                        }
                    }

                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return StatusCode(200,"Oke");

        }



        [HttpPost]
        [Route("CartPageDeleteData")]

        public IActionResult CartPageDeleteData(int car_id)
        {

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

                            query.CommandText = "DELETE FROM cart WHERE car_id = @car_id";
                            query.Transaction = connection.BeginTransaction();

                            query.Parameters.AddRange(new MySqlParameter[]
                            {
                                    //new MySqlParameter{ ParameterName="@cart_id",DbType=DbType.Int32,Value = body.cart_id},
                                    new MySqlParameter{ ParameterName="@fk_car_id",DbType=DbType.Int32,Value = car_id},
                                    //new MySqlParameter{ ParameterName="@car_rental_days",DbType=DbType.Int32,Value = rentedDays},
                                    //new MySqlParameter{ ParameterName="@fk_user_id",DbType=DbType.Int32,Value =user_id},


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


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return StatusCode(200, "Oke");

        }
    }
}
