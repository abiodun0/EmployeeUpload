using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;


public partial class _Default : System.Web.UI.Page
{
    #region Private Member Variables

    #endregion
    #region Web Methods
    protected void Page_Load(object sender, EventArgs args)
    {
        if (!this.IsPostBack)
        {
           
            //Reserve a spot in Session for the UploadDetail object
            this.Session["UploadDetail"] = new UploadDetail { IsReady = false };
            LoadUploadedFiles(ref gvNewFiles);
        }
    }
    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    protected void SendMail()
    {
       


        MailMessage mail = new MailMessage();
        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
        mail.From = new MailAddress("your_email_address@gmail.com");
        mail.To.Add(YourEmail.Text);
        mail.Subject = "Attacments";
        mail.Body = "mail with attachment";

        System.Net.Mail.Attachment attachment;
        
        DirectoryInfo dir = new DirectoryInfo(MyGlobals.UPLOADFOLDER);
        
        foreach (FileInfo files in dir.GetFiles("*.*"))
        {
            // string fileName = Path.GetFileName(files.Name);
            attachment = new System.Net.Mail.Attachment(files.Name);
            mail.Attachments.Add(attachment);
        }
       
          
       
        SmtpServer.Port = 587;
        SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
        SmtpServer.EnableSsl = true;

        SmtpServer.Send(mail);
        
        // Gmail Address from where you send the mail
      /*  var fromAddress = "Gmail@gmail.com";
        // any address where the email will be sending
        var toAddress = YourEmail.Text;
        var Phonenumber = TextBox1.Text;
        //Password of your gmail address
        const string fromPassword = "Password";
        // Passing the values and make a email formate to display

        string body = "From: " + YourName.Text + "\n";
        body += "Email: " + YourEmail.Text + "\n";
        // smtp settings
        var smtp = new SmtpClient();
        {
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
            smtp.Timeout = 20000;
        }
        // Passing values to smtp object
        smtp.Send(fromAddress, toAddress, Phonenumber, body);*/
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        // SendMail();
        SaveInfo saveInfoInDB = new SaveInfo();
        saveInfoInDB.Name = YourName.Text;
        saveInfoInDB.Email = YourEmail.Text;
        saveInfoInDB.TransactionNo = MyGlobals.TransactionNumber;
        saveInfoInDB.Gender = DropDownList1.Text;
        saveInfoInDB.PhoneNo = TextBox1.Text;
        Result Result = saveInfoInDB.SaveInfoInDB();
        if (Result.IsError == false)
        {
            //File Save in Database Successfully!
        }
        else
        {
            //Error in Saving File in Database!
            //Error: Result.ErrorMessage
            //InnerException: Result.InnerException
            //StackTrace: Result.StackTrace
        }
        try
        {
            //here on button click what will done 


            YourEmail.Text = "";
            YourName.Text = "";
            TextBox1.Text = "";
            Label1.Text = "Your Transaction number is" + " " + MyGlobals.TransactionNumber;

        }
        catch (Exception) { }
    } 
    public static object GetUploadStatus()
    {
        //Get the length of the file on disk and divide that by the length of the stream
        UploadDetail info = (UploadDetail)HttpContext.Current.Session["UploadDetail"];
        if (info != null && info.IsReady)
        {
            int soFar = info.UploadedLength;
            int total = info.ContentLength;
            int percentComplete = (int)Math.Ceiling((double)soFar / (double)total * 100);
            string message = "Uploading...";
            string fileName = string.Format("{0}", info.FileName);
            string downloadBytes = string.Format("{0} of {1} Bytes", soFar, total);
            return new
            {
                percentComplete = percentComplete,
                message = message,
                fileName = fileName,
                downloadBytes = downloadBytes
            };
        }
        //Not ready yet
        return null;
    }
    #endregion

    #region Web Callbacks
    protected void gvNewFiles_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "eventMouseOver(this)");
            e.Row.Attributes.Add("onmouseout", "eventMouseOut(this)");
        }
    }
    protected void gvNewFiles_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "deleteFile":
                DeleteFile(e.CommandArgument.ToString());
                LoadUploadedFiles(ref gvNewFiles);
                break;
            case "downloadFile":
                string strFolder = MyGlobals.TransactionNumber;
                string filePath = Path.Combine(strFolder, e.CommandArgument.ToString());
                DownloadFile(filePath);
                break;
        }
    }
    protected void hdRefereshGrid_ValueChanged(object sender, EventArgs e)
    {
        LoadUploadedFiles(ref gvNewFiles);
    }
    #endregion

    #region Support Methods
    public void LoadUploadedFiles(ref GridView gv)
    {
        DataTable dtFiles = GetFilesInDirectory(HttpContext.Current.Server.MapPath(MyGlobals.UPLOADFOLDER));
        gv.DataSource = dtFiles;
        gv.DataBind();
        if (dtFiles != null && dtFiles.Rows.Count > 0)
        {
            double totalSize = Convert.ToDouble(dtFiles.Compute("SUM(Size)", ""));
            if (totalSize > 0) lblTotalSize.Text = CalculateFileSize(totalSize);
        }
    }
    public DataTable GetFilesInDirectory(string sourcePath)
    {
        bool exists = System.IO.Directory.Exists((sourcePath));
        if (!exists)
            System.IO.Directory.CreateDirectory((sourcePath));
        System.Data.DataTable dtFiles = new System.Data.DataTable();
        if ((Directory.Exists(sourcePath)))
        {
            dtFiles.Columns.Add(new System.Data.DataColumn("Name"));
            dtFiles.Columns.Add(new System.Data.DataColumn("Size"));
            dtFiles.Columns["Size"].DataType = typeof(double);
            dtFiles.Columns.Add(new System.Data.DataColumn("ConvertedSize"));
            DirectoryInfo dir = new DirectoryInfo(sourcePath);
            foreach (FileInfo files in dir.GetFiles("*.*"))
            {
                System.Data.DataRow drFile = dtFiles.NewRow();
                drFile["Name"] = files.Name;
                drFile["Size"] = files.Length;
                drFile["ConvertedSize"] = CalculateFileSize(files.Length);
                dtFiles.Rows.Add(drFile);
            }
        }
        return dtFiles;
    }
    public void DownloadFile(string filePath)
    {
        if (File.Exists(Server.MapPath(filePath)))
        {
            string strFileName = Path.GetFileName(filePath).Replace(" ", "%20");
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName);
            Response.Clear();
            Response.WriteFile(Server.MapPath(filePath));
            Response.End();
        }
    }
    public string DeleteFile(string FileName)
    {
        string strMessage = "";
        try
        {
            string strPath = Path.Combine(MyGlobals.UPLOADFOLDER, FileName);
            if (File.Exists(Server.MapPath(strPath)) == true)
            {
                File.Delete(Server.MapPath(strPath));
                strMessage = "File Deleted";
            }
            else
                strMessage = "File Not Found";
        }
        catch (Exception ex)
        {
            strMessage = ex.Message;
        }
        return strMessage;
    }
    public string CalculateFileSize(double FileInBytes)
    {
        string strSize = "00";
        if (FileInBytes < 1024)
            strSize = FileInBytes + " B";//Byte
        else if (FileInBytes > 1024 & FileInBytes < 1048576)
            strSize = Math.Round((FileInBytes / 1024), 2) + " KB";//Kilobyte
        else if (FileInBytes > 1048576 & FileInBytes < 107341824)
            strSize = Math.Round((FileInBytes / 1024) / 1024, 2) + " MB";//Megabyte
        else if (FileInBytes > 107341824 & FileInBytes < 1099511627776)
            strSize = Math.Round(((FileInBytes / 1024) / 1024) / 1024, 2) + " GB";//Gigabyte
        else
            strSize = Math.Round((((FileInBytes / 1024) / 1024) / 1024) / 1024, 2) + " TB";//Terabyte
        return strSize;
    }
    #endregion


}
