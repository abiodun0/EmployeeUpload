using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SaveInfo
/// </summary>
public class SaveInfo
{
    public string Name { get; set; }
    public string Email { get; set; }

    public string PhoneNo { get; set; }

    public string Gender { get; set; }

    public string TransactionNo { get; set; }
	public Result SaveInfoInDB()
	{
        Result Result = new Result();
        string strConnection = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\EmployeeDatabase.mdf;Integrated Security=True";
        //"Data Source=Imdadhusen\\SQLEXPRESS;Initial Catalog=SaveFileExampleDB;Integrated Security=True;Pooling=False"
        using (SqlConnection conn = new SqlConnection(strConnection))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SaveInfoInDB";
            cmd.Connection = conn;

            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@PhoneNo", PhoneNo);
            cmd.Parameters.AddWithValue("@Gender", Gender);
            cmd.Parameters.AddWithValue("@TransactionNo", TransactionNo);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                Result.IsError = false;
            }
     catch (Exception ex)
            {
                Result.IsError = true;
                Result.ErrorMessage = ex.Message;
                Result.InnerException = ex.InnerException.ToString();
                Result.StackTrace = ex.StackTrace.ToString();
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
                conn.Dispose();
            }
        }
        return Result;
	}
}