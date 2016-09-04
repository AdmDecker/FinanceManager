using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace Budget_Manager_Launcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public static string installPath = "C:\\Program Files\\Budget Manager";
        public static string versionLatest = "";
        public static string versionCurrent = "";
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Get Appdata Location
            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if(File.Exists(appdataPath + "/BudgetManager/path"))
            {
                //Get path where program was originally installed
                var fileData = File.ReadLines(appdataPath + "/BudgetManager/path");
                installPath = fileData.ElementAt(0);
            }




            //Find current version
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (File.Exists(path + "/BudgetManager/version"))
            {
                var readData = File.ReadLines(path + "/BudgetManager/version");
                versionCurrent = readData.ElementAt(0);
            }

            WebClient wclient = new WebClient();

            //Get latest version number from site
            versionLatest = "";
            try
            {
                versionLatest = wclient.DownloadString("http://www.hitbash.com/projects/budgetmanager/getversion.html");
                versionLatest = versionLatest.Trim();
            }
            catch
            {
                
            }


            if (versionCurrent == "" || versionLatest == "")
            {
                
            }
            else if (versionLatest.CompareTo(versionCurrent) != 0)
            {
                MessageBox.Show("Update Found! Update manager will now launch.");
                Form1 form = new Form1();
                Application.Run(form);
                return;
            }



            //Check if application exists
            if (File.Exists(appdataPath + "/BudgetManager/path"))
            {
                //Check if path is valid
                if(Directory.Exists(@installPath.Trim()))
                {
                    //Start the program!
                    try
                    {
                        Process.Start(installPath + "\\Budget Manager\\Budget Manager.exe");
                    }
                    catch
                    {
                        //If Starting the program failed, boot the launcher UI
                        MessageBox.Show("Failed to start Budget Manager. Please reinstall the application.");
                        Form1 form = new Form1();
                        Application.Run(form);
                        return;
                    }

                }
                else
                {
                    //Show user error and ask to reinstall
                    MessageBox.Show("Install path not valid. Please reinstall the application");
                    Form1 form = new Form1();
                    Application.Run(form);
                    return;
                }
            }
            else
            {
                Form1 form = new Form1();
                Application.Run(form);
                return;
            }


            /*
 
            Form1 form = new Form1();
            Application.Run(form);
             */
        }
    }
}
