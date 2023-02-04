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

// DOWNLOAD AUTHHH
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

// GOOGLE SIGN IN
using Google.Apis.Auth;


namespace rentalcar_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        public string conString = "";
        public string jwtKey = "";
        public string jwtKeyEmail = "KEY UNTUK JWT SEND EMAIL";

        public LoginController(IConfiguration configuration)
        {
            conString = configuration["ConnectionStrings:Default"];
            jwtKey = configuration["Jwt:Key"];
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult Login([FromBody] LoginUser body)
        {
            int user_id = 0;
            try
            {
                // VALIDATION
                if (Util.isValidEmail(body.username) == false)
                {
                    throw new Exception("USERNAME/EMAIL NOT VALID");
                }
                if (Util.isValidAlphanumeric(body.password) == false)
                {
                    throw new Exception("PASSWORD NOT VALID");
                }

                string jwtToken = "";

                // BEGIN CONNECTION
                using (var conn = new MySqlConnection(conString))
                {
                    conn.Open();

                    // PREPARE QUERY COMMAND
                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT * FROM User WHERE username = @username;";
                        cmd.Parameters.Add(new MySqlParameter { ParameterName = "@username", DbType = DbType.String, Value = body.username });

                        // execute query
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // VERIFIKASI USER AKTIF
                                bool active = (bool)Util.NullSafe(reader["active"], false);
                                if (active == false)
                                {
                                    throw new Exception("USER IS INACTIVE, PLEASE CONTANT US TO RE-ACTIVE IT");
                                }

                                // VERIFIKASI PASSWORD: HASH DB VS HASH PASSWORD YANG DIINPUT USER
                                byte[] userHash = (byte[])reader["passwordHash"];
                                byte[] userSalt = (byte[])reader["passwordSalt"];
                                var hex = BitConverter.ToString(userHash);

                                
                                bool isTrueUser = false;
                                using (var hmac = new HMACSHA512(userSalt))
                                {
                                    byte[] checkHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body.password));
                                    var hex2 = BitConverter.ToString(checkHash);
                                    isTrueUser = checkHash.SequenceEqual(userHash);
                                }
                                if (isTrueUser == false)
                                {
                                    throw new Exception("PASSWORD SALAH");
                                }

                                // CREATE TOKEN
                                if (isTrueUser)
                                {
                                    user_id = (int)Util.NullSafe(reader["user_id"], 0);
                                    string role = (string)Util.NullSafe(reader["role"], "");

                                    List<Claim> claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Name, body.username),
                                        new Claim(ClaimTypes.Role, role)
                                    };

                                    jwtToken = Token.CreateJwtToken(claims);
                                }

                            }
                        }
                        else
                        {
                            throw new Exception("Username Salah");
                        }

                        reader.Close();
                    }
                    conn.Close();
                }


                return Ok(new
                {
                    jwtToken = jwtToken,
                    user_id = user_id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("SellerLogin")]

        public ActionResult SellerLogin([FromBody] LoginSeller body)
        {

            
            int admin_id = 0;
            string email = "";



            try
            {
                // VALIDATION
                if (Util.isValidEmail(body.username) == false)
                {
                    throw new Exception("USERNAME/EMAIL NOT VALID");
                }
                if (Util.isValidAlphanumeric(body.password) == false)
                {
                    throw new Exception("PASSWORD NOT VALID");
                }

                string jwtToken = "";

                // BEGIN CONNECTION
                using (var conn = new MySqlConnection(conString))
                {
                    conn.Open();

                    // PREPARE QUERY COMMAND
                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT * FROM Admin WHERE username_admin = @username;";
                        cmd.Parameters.Add(new MySqlParameter { ParameterName = "@username", DbType = DbType.String, Value = body.username });

                        // execute query
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // VERIFIKASI USER AKTIF
                                bool active = (bool)Util.NullSafe(reader["active"], false);
                                if (active == false)
                                {
                                    throw new Exception("USER IS INACTIVE, PLEASE CONTANT US TO RE-ACTIVE IT");
                                }

                                // VERIFIKASI PASSWORD: HASH DB VS HASH PASSWORD YANG DIINPUT USER
                                byte[] userHash = (byte[])reader["Admin_passwordHash"];
                                byte[] userSalt = (byte[])reader["Admin_passwordSalt"];
                                var hex = BitConverter.ToString(userHash);


                                bool isTrueUser = false;
                                using (var hmac = new HMACSHA512(userSalt))
                                {
                                    byte[] checkHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body.password));
                                    var hex2 = BitConverter.ToString(checkHash);
                                    isTrueUser = checkHash.SequenceEqual(userHash);
                                }
                                if (isTrueUser == false)
                                {
                                    throw new Exception("PASSWORD SALAH");
                                }

                                // CREATE TOKEN
                                if (isTrueUser)
                                {
                                    admin_id = (int)Util.NullSafe(reader["admin_id"], 0);
                                    email = (string)Util.NullSafe(reader["username_admin"], "");
                                    string role = (string)Util.NullSafe(reader["role"], "");

                                    List<Claim> claims = new List<Claim>
                                    {
                                    
                                        new Claim(ClaimTypes.Name, body.username),
                                        new Claim(ClaimTypes.Role, role)
                                    };

                                    jwtToken = Token.CreateJwtToken(claims);
                                }

                            }
                        }
                        else
                        {
                            throw new Exception("Username Salah");
                        }

                        reader.Close();
                    }
                    conn.Close();
                }


                return Ok(new {
                    jwtToken,
                    admin_id,
                    email,
                    role = "renter"          
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
}
