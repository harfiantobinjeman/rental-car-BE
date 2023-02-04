using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySqlConnector;

//Entity Class
using rentalcar_backend.Models;
using rentalcar_backend.Method;

//Hashing, cryptography, and Authentication
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Security.Claims;

// DOWNLOAD AUTHHH
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

// GOOGLE SIGN IN
using Google.Apis.Auth;

//Cors
using Microsoft.AspNetCore.Cors;

namespace rentalcar_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : Controller
    {
        private string conString = "";
        //Auth token
        private string jwtkey = "";
        public string jwtKeyEmail = "letsrental key for email authentication";

        //Constructor 
        public string homebaseURL = "";
        public RegisterController(IConfiguration configuration)
        {
            conString = configuration["ConnectionStrings:Default"];
            jwtkey = configuration["jwt:key"];
            homebaseURL = configuration["homebaseURL"];
        }

        [HttpPost]
        [Route("register")]
        public ActionResult Register([FromBody] RegisterUser body)
        {
            try
            {
                //Check if username and password is valid or not
                if (Util.isValidEmail(body.username) == false)
                {
                    throw new Exception("USERNAME/EMAIL NOT VALID");
                }
                if (Util.isValidAlphanumeric(body.password) == false)
                {
                    throw new Exception("PASSWORD NOT VALID");
                }

                //Open Connection
                using (var ConnectionString = new MySqlConnection(conString))
                {
                    ConnectionString.Open();

                    using (var query = new MySqlCommand())
                    {
            

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

                            // BEGIN CONNECTION
                            using (var conn = new MySqlConnection(conString))
                            {
                                conn.Open();

                                // PREPARE QUERY COMMAND
                                using (var cmd = new MySqlCommand())
                                {
                                    cmd.Connection = conn;

                                    // CHECK USERNAME ALREADY EXIST IN DB OR NOT
                                    cmd.CommandText = "SELECT * FROM User WHERE username=@username";
                                    cmd.Parameters.Add(new MySqlParameter { ParameterName = "@username", DbType = DbType.String, Value = body.username });

                                    MySqlDataReader reader = cmd.ExecuteReader();
                                    if (reader.HasRows)
                                    {
                                        throw new Exception("USERNAME ALREADY EXIST");
                                    }
                                    reader.Close();

                                    // INSERT USER TO DB
                                    cmd.CommandText = "INSERT INTO User (username, passwordHash, passwordSalt, `name`,`Role`, `active`) VALUES (@username, @hash, @salt, @name,@role, @active);";
                                    cmd.Transaction = conn.BeginTransaction();

                                    // HASH AND SALT GENERATED HERE
                                    //string passwordDecrypted = AES.DecryptStringAES(body.password).ToString();
                                    byte[] salt;
                                    byte[] hash;
                                    using (var hmac = new HMACSHA512())
                                    {
                                        salt = hmac.Key;
                                        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body.password));
                                    }

                                    cmd.Parameters.AddRange(new MySqlParameter[]
                                    {
                                        new MySqlParameter { ParameterName="@hash", DbType=DbType.Binary, Value = hash },
                                        new MySqlParameter { ParameterName="@salt", DbType=DbType.Binary, Value = salt },
                                        new MySqlParameter {ParameterName="@name",DbType=DbType.String,Value= String.IsNullOrEmpty(body.name) ? "" : body.name},
                                        new MySqlParameter { ParameterName="@role", DbType=DbType.String, Value = String.IsNullOrEmpty(body.role) ? "buyer" : body.role },
                                        new MySqlParameter { ParameterName="@active", MySqlDbType=MySqlDbType.Bool, Value = false },
                                    });

                                    // execute query and validate it
                                    try
                                    {
                                        int executeOk = cmd.ExecuteNonQuery();
                                    }
                                    catch
                                    {
                                        cmd.Transaction.Rollback();
                                        throw new Exception("REGISTER DATA FAILED");
                                    }

                                    // CREATE TOKEN FOR EMAIL
                                    List<Claim> claims = new List<Claim>
                                    {
                                        new Claim("@email", body.username),
                                        new Claim("@request_activation", "true"),
                                        new Claim("@role", "buyer"),
                                    };
                                    string verifToken = Token.CreateJwtToken(claims, jwtKeyEmail);

                                    // SEND EMAIL
                                    string targetUrl = homebaseURL + "verification/" + verifToken;
                                    string emailBody = @$"<div>
                                             <h4 style='color:blue;'>Thanks for joining our application</h4>
                                             <p>Click this <a href={targetUrl}>{targetUrl}</a> to active your account</P>
                                         </div>";

                                    try
                                    {
                                        Email.SendEmail(body.username, "VERIFY YOUR EMAIL", emailBody);
                                    }
                                    catch(Exception e)
                                    {
                                        cmd.Transaction.Rollback();
                                        throw new Exception(e.Message);
                                    }

                                    // commit if no error happen
                                    cmd.Transaction.Commit();
                                }
                                conn.Close();
                            }

                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }




                        query.ExecuteNonQuery();

                    }

                    ConnectionString.Close();

                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

       

        }


        [HttpPost]
        [Route("register_seller")]

        public ActionResult registerseller([FromBody] RegisterAdmin body)
        {
            try
            {
                if (Util.isValidEmail(body.username) == false)
                {
                    throw new Exception("USERNAME/EMAIL NOT VALID");
                }
                if (Util.isValidAlphanumeric(body.password) == false)
                {
                    throw new Exception("PASSWORD NOT VALID");
                }

                // BEGIN CONNECTION
                using (var conn = new MySqlConnection(conString))
                {
                    conn.Open();

                    // PREPARE QUERY COMMAND
                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;

                        // CHECK USERNAME ALREADY EXIST IN DB OR NOT
                        cmd.CommandText = "SELECT * FROM `Admin` WHERE username_admin=@username";
                        cmd.Parameters.Add(new MySqlParameter { ParameterName = "@username", DbType = DbType.String, Value = body.username });

                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            throw new Exception("USERNAME ALREADY EXIST");
                        }
                        reader.Close();

                        // INSERT USER TO DB
                        cmd.CommandText = "INSERT INTO `Admin` (username_admin, Admin_passwordHash, Admin_passwordSalt, Admin_name,`Role`, `active`) VALUES (@username, @hash, @salt, @name,@role, @active);";
                        cmd.Transaction = conn.BeginTransaction();

                        // HASH AND SALT GENERATED HERE
                        //string passwordDecrypted = AES.DecryptStringAES(body.password).ToString();
                        byte[] salt;
                        byte[] hash;
                        using (var hmac = new HMACSHA512())
                        {
                            salt = hmac.Key;
                            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body.password));
                        }

                        cmd.Parameters.AddRange(new MySqlParameter[]
                        {
                                        new MySqlParameter { ParameterName="@hash", DbType=DbType.Binary, Value = hash },
                                        new MySqlParameter { ParameterName="@salt", DbType=DbType.Binary, Value = salt },
                                        new MySqlParameter {ParameterName="@name",DbType=DbType.String,Value= String.IsNullOrEmpty(body.name) ? "" : body.name},
                                        new MySqlParameter { ParameterName="@role", DbType=DbType.String, Value = String.IsNullOrEmpty(body.role) ? "renter" : body.role},
                                        new MySqlParameter { ParameterName="@active", MySqlDbType=MySqlDbType.Bool, Value = false },
                        });

                        // execute query and validate it
                        try
                        {
                            int executeOk = cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            cmd.Transaction.Rollback();
                            throw new Exception("REGISTER DATA FAILED");
                        }

                        // CREATE TOKEN FOR EMAIL
                        List<Claim> claims = new List<Claim>
                                    {
                                        new Claim("@email", body.username),
                                        new Claim("@request_activation", "true"),
                                        new Claim("@role", "renter"),
                                    };
                        string verifToken = Token.CreateJwtToken(claims, jwtKeyEmail);

                        // SEND EMAIL
                        string targetUrl = "http://localhost:3000/seller/verification/" + verifToken;
                        string emailBody = @$"<div>
                                             <h4 style='color:blue;'>Thanks for becoming our renter!</h4>
                                             <p>Click this <a href={targetUrl}>{targetUrl}</a> to active your account</P>
                                         </div>";

                        try
                        {
                            Email.SendEmail(body.username, "VERIFY YOUR EMAIL", emailBody);
                        }
                        catch (Exception e)
                        {
                            cmd.Transaction.Rollback();
                            throw new Exception(e.Message);
                        }

                        // commit if no error happen
                        cmd.Transaction.Commit();
                    }
                    conn.Close();
                }



                return Ok();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
          
        }
      


        [HttpPost]
        [Route("EmailTokenVerification")]
        [DisableCors]
        public ActionResult VerifyEmailToken([FromBody] TokenVerification verifToken)
        {
            try
            {
                // VERIFY CLAIMS
                List<Claim> claims = Token.ParseJwtTokenClaim(verifToken.Token, jwtKeyEmail);

                bool isActivation = false;
                string username = "";
            

                foreach (Claim claim in claims)
                {
                    if (claim.Type == "@request_activation")
                    {
                        isActivation = true;
                    }
                    if (claim.Type == "@email")
                    {
                        username = claim.Value;
                    }
           
                }

                if(verifToken.role == "buyer")
                {
                    // CLAIM ALREADY VERIFIED
                    if (isActivation == true && String.IsNullOrEmpty(username) == false)
                    {
                        using (var conn = new MySqlConnection(conString))
                        {
                            conn.Open();

                            using (var cmd = new MySqlCommand())
                            {
                                // ACTIVATE USER
                                cmd.Connection = conn;
                                cmd.CommandText = "UPDATE User SET `active` = true WHERE username=@username";
                                cmd.Parameters.Add(new MySqlParameter { ParameterName = "@username", MySqlDbType = MySqlDbType.VarString, Value = username });
                                cmd.Transaction = conn.BeginTransaction();

                                int executeOk = cmd.ExecuteNonQuery();
                                if (executeOk == -1)
                                {
                                    cmd.Transaction.Rollback();
                                    throw new Exception("USER ACTIVATION FAILED");
                                }

                                cmd.Transaction.Commit();
                            }

                            conn.Close();
                        }
                    }
                    else
                    {
                        throw new Exception("EMAIL TOKEN VERIFICATION FAILED");
                    }
                }
                else
                {
                    throw new Exception("ROLE NOT A BUYER!");
                }

            

                return Ok("email verified");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("EmailTokenVerificationAdmin")]
        [DisableCors]

        public ActionResult VerifyEmailTokenAdmin([FromBody] TokenVerification verifToken)
        {
            try
            {
                // VERIFY CLAIMS
                List<Claim> claims = Token.ParseJwtTokenClaim(verifToken.Token, jwtKeyEmail);

                bool isActivation = false;
                string username = "";

                foreach (Claim claim in claims)
                {
                    if (claim.Type == "@request_activation")
                    {
                        isActivation = true;
                    }
                    if (claim.Type == "@email")
                    {
                        username = claim.Value;
                    }
                }

                if(verifToken.role == "renter")
                {
                    // CLAIM ALREADY VERIFIED
                    if (isActivation == true && String.IsNullOrEmpty(username) == false)
                    {
                        using (var conn = new MySqlConnection(conString))
                        {
                            conn.Open();

                            using (var cmd = new MySqlCommand())
                            {
                                // ACTIVATE USER
                                cmd.Connection = conn;
                                cmd.CommandText = "UPDATE `Admin` SET `active` = true WHERE username_admin=@username";
                                cmd.Parameters.Add(new MySqlParameter { ParameterName = "@username", MySqlDbType = MySqlDbType.VarString, Value = username });
                                cmd.Transaction = conn.BeginTransaction();

                                int executeOk = cmd.ExecuteNonQuery();
                                if (executeOk == -1)
                                {
                                    cmd.Transaction.Rollback();
                                    throw new Exception("RENTER ACTIVATION FAILED");
                                }

                                cmd.Transaction.Commit();
                            }

                            conn.Close();
                        }
                    }
                    else
                    {
                        throw new Exception("EMAIL TOKEN VERIFICATION FAILED");
                    }
                }else
                {
                    throw new Exception("ROLE IS NOT RENTER!");
                }

            

                return Ok("email verified");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }


}


