using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MyGlobals
/// </summary>
public static class MyGlobals
{
    public static string TransactionNumber = DateTime.Now.ToString("yyyyMMddHHmmssffff");
    public static string UPLOADFOLDER = "Uploads" + "/" + TransactionNumber;
 

}