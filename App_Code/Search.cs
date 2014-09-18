using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Search
/// </summary>
public class Search
{
    public string Email { get; set; }

    public string TransactionNo { get; set; }
    public bool SearchEmail()
	{
        bool result = false;
        Result Result = new Result();
		string strConnection = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\EmployeeDatabase.mdf;Integrated Security=True";
        //"Data Source=Imdadhusen\\SQLEXPRESS;Initial Catalog=SaveFileExampleDB;Integrated Security=True;Pooling=False"
        using (SqlConnection conn = new SqlConnection(strConnection))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SearchEmail";
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@Email", Email);

            try
            {
                conn.Open();
                double amountOfEmails = Convert.ToDouble(cmd.ExecuteScalar());
                if (amountOfEmails >= 1)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            finally
            {
                conn.Close();
            }
        }
        return result;
	}
    public bool Transaction()
    {
        bool result = false;
        Result Result = new Result();
        string strConnection = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\EmployeeDatabase.mdf;Integrated Security=True";
        //"Data Source=Imdadhusen\\SQLEXPRESS;Initial Catalog=SaveFileExampleDB;Integrated Security=True;Pooling=False"
        using (SqlConnection conn = new SqlConnection(strConnection))
        {
           SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TransactionSearch";
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@TransactionNO", TransactionNo);

            try
            {
                conn.Open();
                double amountOfTransactions = Convert.ToDouble(cmd.ExecuteScalar());
                if (amountOfTransactions >= 1)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            finally
            {
                conn.Close();
            }
        }
        return result;
	}
       
}