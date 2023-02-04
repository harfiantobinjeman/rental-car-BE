using MySqlConnector;
using System.Data;
using rentalcar_backend.Models;

namespace WebApi.Method
{
    public static class CRUD
    {
        public static IConfiguration _configuration;
        private static string _conString;

        // get appsettings.json config in Program.cs
        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration;
            _conString = configuration["ConnectionStrings:Default"];
        }

        // SPECIFIC METHOD TO ADD DATA TO PRODUCT TABLE
        //public static bool AddProduct(string conString, Product product)
        //{
        //    try
        //    {
        //        // prepare connection
        //        using (var conn = new MySqlConnection(conString))
        //        {
        //            conn.Open();

        //            // prepare query command
        //            using (var cmd = new MySqlCommand())
        //            {
        //                cmd.Connection = conn;
        //                cmd.Transaction = conn.BeginTransaction();
        //                cmd.CommandText = "INSERT INTO Product(title, `desc`, price, fk_category_id, keywords, image_content, image_name) " +
        //                        "VALUES (@title, @desc, @price, @fk_category_id, @keywords, @image_content, @image_name);";

        //                var param = new MySqlParameter[]
        //                {
        //                    new MySqlParameter { ParameterName="@title", DbType = DbType.String, Value = String.IsNullOrEmpty(product.title) ? "" : product.title },
        //                    new MySqlParameter { ParameterName="@desc", DbType = DbType.String, Value = String.IsNullOrEmpty(product.desc) ? "" : product.desc },
        //                    new MySqlParameter { ParameterName="@price", DbType = DbType.Int32, Value = product.price },
        //                    new MySqlParameter { ParameterName="@fk_category_id", DbType = DbType.Int32, Value = product.fk_category_id},
        //                    new MySqlParameter { ParameterName="@keywords", DbType = DbType.String, Value = String.IsNullOrEmpty(product.keywords) ? "" : product.keywords },
        //                    new MySqlParameter { ParameterName="@image_content", DbType = DbType.String, Value = String.IsNullOrEmpty(product.image_content) ? "" : product.image_content },
        //                    new MySqlParameter { ParameterName="@image_name", DbType = DbType.String, Value = String.IsNullOrEmpty(product.image_name) ? "" : product.image_name }
        //                };

        //                cmd.Parameters.AddRange(param);

        //                try
        //                {
        //                    int res = cmd.ExecuteNonQuery();
        //                    if (res == -1)
        //                    {
        //                        cmd.Transaction.Rollback();
        //                    }
        //                    else
        //                    {
        //                        cmd.Transaction.Commit();
        //                    }
        //                }
        //                catch
        //                {
        //                    cmd.Transaction.Rollback();
        //                    throw;
        //                }
        //            }

        //            conn.Close();
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        // SPECIFIC METHOD TO ADD BLOB DATA TO PRODUCT TABLE
        //public static bool AddProductBlob(string conString, Product product)
        //{
        //    try
        //    {
        //        string imageName = product.image_content_blob.FileName;

        //        convert IFormFile ke byte[] untuk disimpan di BLOB(binary large object)
        //        byte[] imageContentBlob;

        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            product.image_content_blob.CopyTo(stream);
        //            imageContentBlob = stream.ToArray();
        //        }

        //        prepare connection
        //        using (var conn = new MySqlConnection(conString))
        //        {
        //            conn.Open();

        //            prepare query command
        //            using (var cmd = new MySqlCommand())
        //            {
        //                cmd.Connection = conn;
        //                cmd.Transaction = conn.BeginTransaction();
        //                cmd.CommandText = "INSERT INTO Product(title, `desc`, price, fk_category_id, keywords, image_content, image_name, image_content_blob) " +
        //                        "VALUES (@title, @desc, @price, @fk_category_id, @keywords, @image_content, @image_name, @image_content_blob);";

        //                var param = new MySqlParameter[]
        //                {
        //                    new MySqlParameter { ParameterName="@title", DbType = DbType.String, Value = String.IsNullOrEmpty(product.title) ? "" : product.title },
        //                    new MySqlParameter { ParameterName="@desc", DbType = DbType.String, Value = String.IsNullOrEmpty(product.desc) ? "" : product.desc },
        //                    new MySqlParameter { ParameterName="@price", DbType = DbType.Int32, Value = product.price },
        //                    new MySqlParameter { ParameterName="@fk_category_id", DbType = DbType.Int32, Value = product.fk_category_id},
        //                    new MySqlParameter { ParameterName="@keywords", DbType = DbType.String, Value = String.IsNullOrEmpty(product.keywords) ? "" : product.keywords },
        //                    new MySqlParameter { ParameterName="@image_content", DbType = DbType.String, Value = String.IsNullOrEmpty(product.image_content) ? "" : product.image_content },
        //                    new MySqlParameter { ParameterName="@image_name", DbType = DbType.String, Value = String.IsNullOrEmpty(imageName) ? "" : imageName },
        //                    new MySqlParameter { ParameterName="@image_content_blob", DbType = DbType.Binary, Value = imageContentBlob },
        //                };

        //                cmd.Parameters.AddRange(param);

        //                try
        //                {
        //                    int res = cmd.ExecuteNonQuery();
        //                    if (res == -1)
        //                    {
        //                        cmd.Transaction.Rollback();
        //                    }
        //                    else
        //                    {
        //                        cmd.Transaction.Commit();
        //                    }
        //                }
        //                catch
        //                {
        //                    cmd.Transaction.Rollback();
        //                    throw;
        //                }
        //            }

        //            conn.Close();
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        // ------------------------------ INI YANG DIPAKAI listing-webapp-image-axios ------------------------------
        // GENERAL FUNCTION TO ADD DATA TO ANY TABLE
        public static int AddSingleData(string tableName, MySqlParameter[] sqlParams, string conString = "")
        {
            // default value
            if (String.IsNullOrEmpty(conString)) conString = _conString;

            // main logic
            try
            {
                int result = -1;

                // Validation
                if (String.IsNullOrEmpty(conString))
                {
                    throw new Exception("conString cannot empty");
                }
                if (String.IsNullOrEmpty(tableName))
                {
                    throw new Exception("tableName cannot empty");
                }
                if (sqlParams.Length < 1)
                {
                    throw new Exception("sqlParams cannot empty");
                }

                // prepare connection
                using (var conn = new MySqlConnection(conString))
                {
                    conn.Open();

                    // prepare query command
                    using (var cmd = new MySqlCommand())
                    {
                        // mapping table column and values
                        string tableColumn = "";
                        string tableValues = "";
                        for (int idx = 0; idx < sqlParams.Length; idx++)
                        {
                            MySqlParameter sqlParam = sqlParams[idx];

                            // give comma separator
                            if (idx > 0)
                            {
                                tableColumn += ", ";
                                tableValues += ", ";
                            }

                            // check prefix @ to fill column and values (assumption: ParameterName is the same as column name)
                            if (sqlParam.ParameterName[0] == '@')
                            {
                                tableColumn += "`" + sqlParam.ParameterName.Substring(1) + "`";
                                tableValues += sqlParam.ParameterName;
                            }
                            else
                            {
                                tableColumn += "`" + sqlParam.ParameterName + "`";
                                tableValues += "@" + sqlParam.ParameterName;
                            }
                        }

                        // fill command
                        cmd.Connection = conn;
                        cmd.Transaction = conn.BeginTransaction();
                        cmd.CommandText = $"INSERT INTO {tableName}({tableColumn}) VALUES ({tableValues});";
                        cmd.Parameters.AddRange(sqlParams);

                        // execute command
                        try
                        {
                            result = cmd.ExecuteNonQuery();
                            if (result == -1)
                            {
                                cmd.Transaction.Rollback();
                            }
                            else
                            {
                                cmd.Transaction.Commit();
                            }
                        }
                        catch
                        {
                            cmd.Transaction.Rollback();
                            conn.Close();
                            throw;
                        }
                    }

                    conn.Close();
                }

                return result; // either return -1 or if something when wrong (from ExecuteNonQuery)
            }
            catch
            {
                throw;
            }
        }

        // GENERAL SELECT
        public static DataTable SelectData(string query, MySqlParameter[]? sqlParams = null, string conString = "")
        {
            // default value
            if (String.IsNullOrEmpty(conString)) conString = _conString;

            try
            {
                DataTable result = new DataTable();

                // Validation
                if (String.IsNullOrEmpty(conString))
                {
                    throw new Exception("conString cannot empty");
                }
                if (String.IsNullOrEmpty(query))
                {
                    throw new Exception("query cannot empty");
                }

                // connect to database
                using (var conn = new MySqlConnection(conString))
                {
                    conn.Open();

                    // prepare query command
                    using (var cmd = new MySqlCommand())
                    {
                        // fill command
                        cmd.CommandText = query;
                        cmd.Connection = conn;
                        if (sqlParams != null && sqlParams.Length > 1)
                        {
                            cmd.Parameters.AddRange(sqlParams);
                        }

                        // execute command and fill it to DataTable
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(result);
                    }

                    conn.Close();
                }

                return result;
            }
            catch
            {
                throw;
            }

        }
    }
}
