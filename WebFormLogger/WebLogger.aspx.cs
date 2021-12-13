using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebFormLogger
{
    public partial class WebLogger : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Page.IsPostBack)
            //{
            //    WriteLog("Welcome");
            //}
        }

        /// <summary>
        /// Before using this logger, create an entry named "ErrorLogLocation" in the app settings section
        /// of the config file and pass the absolute full path of the app in wwwroot folder application into it.         
        /// followed by a backward slash
        /// </summary>
        /// <param name="message"></param>
        public void WriteLog(string message)
        {  
            //string appPath = Server.MapPath("");
            string appPath = System.Configuration.ConfigurationManager.AppSettings["ErrorLogLocation"];

            //string value = Environment.CurrentDirectory;
            //C:\Users\xxx\Documents\

            //since the folder does not exist, no file will be found in it
            string filePath = appPath + @"\Log\Log" + DateTime.Now.Date.ToString("ddMMyyyy") + ".txt";

            string errorMsg = "@" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss ttt") + " : " + message;

            //check if folder called "Log" exists.  If it does not, create one
            string[] folderNames = null;

            //this will check if the file location has been configured in the config file
            //of the consuming application
            bool isValidFileLocation = false;

            try
            {
                folderNames = Directory.GetDirectories(appPath);

                isValidFileLocation = true;
            }
            catch (Exception eX)
            {
                isValidFileLocation = false;
            }

            if (!isValidFileLocation)
            {
                //use the default file location
                appPath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
                filePath = appPath + @"\Log\Log" + DateTime.Now.Date.ToString("ddMMyyyy") + ".txt";

                folderNames = Directory.GetDirectories(appPath);
            }

            bool isFolderPresent = false;

            foreach (string s in folderNames)
            {
                if (s == appPath + @"\Log")
                {
                    isFolderPresent = true;
                }
            }

            if (!isFolderPresent)
            {
                Directory.CreateDirectory(appPath + @"\Log");

                //create the text file for the first time ever and write into it
                //File.AppendAllLines(filePath, errorMsg);
                using (FileStream fs = File.Create(filePath))
                {
                    using (TextWriter tw = new StreamWriter(fs))
                    {
                        tw.WriteLine(errorMsg);
                        tw.WriteLine();
                        //tw.WriteLine();                            
                    }

                }
            }
            else
            {
                //here, the folder exists

                string[] files = Directory.GetFiles(appPath + @"\Log");

                if (files.Length > 0)
                {
                    bool isFileFound = false;

                    //here, the folder contains at least one file
                    foreach (string s in files)
                    {
                        if (Path.GetFileName(s).ToUpper() == Path.GetFileName(filePath).ToUpper())
                        {
                            isFileFound = true;
                        }
                    }

                    if (isFileFound)
                    {
                        //open the file and append it with text
                        using (StreamWriter sw = File.AppendText(filePath))
                        {
                            sw.WriteLine(errorMsg);
                            sw.WriteLine();
                            //sw.WriteLine();
                            //sw.NewLine = "\n\n";
                            //sw.NewLine = "\n\n";
                        }
                    }
                    else
                    {
                        //create the text file and write into it
                        File.AppendAllText(filePath, errorMsg);
                        using (StreamWriter sw = File.AppendText(filePath))
                        {
                            sw.WriteLine();
                            //sw.WriteLine();
                        }
                    }
                }
                else
                {
                    //the folder does not contain any file
                    //create the text file and write into it
                    File.AppendAllText(filePath, errorMsg);
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine();
                        //sw.WriteLine();
                    }
                }

            }
        }       
    }
}