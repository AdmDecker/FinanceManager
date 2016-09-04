using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;


namespace Budget_Manager_Launcher
{
    public partial class Form1 : Form
    {

        string version = "1.0.0";
        public Form1()
        {
            InitializeComponent();
            textBoxPath.Text = Program.installPath;
            if(Program.versionCurrent != "")
                listView1.Items.Add("Current version: " + Program.versionCurrent);
            if(Program.versionLatest != "")
                listView1.Items.Add("Latest version: " + Program.versionLatest);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void DownloadUpdate()
        {
            backgroundWorker1.RunWorkerAsync();

        }

        private void InstallUpdate()
        {
            //Check if already installed
            if(File.Exists(Program.installPath + "/Budget Manager/Budget Manager.exe"))
            {
                //Delete old installation
                File.Delete(Program.installPath + "/Budget Manager/Budget Manager.exe");
            }
            //Move new installer to location
            try
            {
                File.Move(textBoxPath.Text + "/Budget Manager/UpdateData", textBoxPath.Text + "/Budget Manager/Budget Manager.exe");
            }
            catch
            {
                //If it fails, revert everything
                listView1.Items.Add("Install failed. Cleaning up...");
                File.Delete(textBoxPath.Text + "/Budget Manager/UpdateData");
                listView1.Items.Add("Done");
                buttonUninstall.Enabled = true;
                buttonInstall.Enabled = true;
                buttonBrowse.Enabled = true;
                return;
            }

            //Remove old version and path files
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (File.Exists(path + "/BudgetManager/version"))
            {
                try
                {
                    File.Delete(path + "/BudgetManager/version");
                    File.Delete(path + "/BudgetManager/path");
                }catch{}
            }

            //Create Appdata directory if it does not exist
            if(!Directory.Exists(path + "/BudgetManager"))
            {
                try
                {
                    Directory.CreateDirectory(path + "/BudgetManager");
                }
                catch
                {
                    listView1.Items.Add("Install failed. Could not create Appdata Directory.");
                    listView1.Items.Add("Cleaning up...");
                    File.Delete(textBoxPath.Text + "/UpdateData");
                    listView1.Items.Add("Done");
                    buttonUninstall.Enabled = true;
                    buttonInstall.Enabled = true;
                    buttonBrowse.Enabled = true;
                    return;
                }
            }

            //Edit appdata to reflect new install locations
            File.WriteAllLines(path + "/BudgetManager/version", new string[1] { Program.versionLatest });
            File.WriteAllLines(path + "/BudgetManager/path", new string[1] { textBoxPath.Text });

            //re-enable buttons
            listView1.Items.Add("Installation Complete.");
            buttonInstall.Text = "Launch";
            buttonInstall.Enabled = true;
            buttonBrowse.Enabled = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                WebClient wclient = new WebClient();
                wclient.DownloadFile("http://www.hitbash.com/projects/budgetmanager/budgetmanager.exe", textBoxPath.Text + "/Budget Manager/UpdateData");
            }
            catch
            {
                backgroundWorker1.ReportProgress(1);
                return;
            }
            backgroundWorker1.ReportProgress(100);
            return;

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(e.ProgressPercentage == 1)
            {
                listView1.Items.Add("Download Failed");
                buttonUninstall.Enabled = true;
                buttonInstall.Enabled = true;
                buttonBrowse.Enabled = true;
            }
            else if(e.ProgressPercentage == 100)
            {
                listView1.Items.Add("Download complete. Installing...");
                InstallUpdate();
            }
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            //clear display
            listView1.Items.Clear();



            if(buttonInstall.Text != "Launch")
            {
                //Check for new launcher version
                listView1.Items.Add("Checking for new launcher version...");
                WebClient wc = new WebClient();
                try
                {
                    string versionLatest = wc.DownloadString("http://www.hitbash.com/projects/budgetmanager/getlauncherversion.html");
                    if (version == versionLatest)
                    {
                        listView1.Items.Add("Launcher is up to date");
                    }
                    else
                    {
                        listView1.Items.Add("Launcher update found");
                        var click = MessageBox.Show(
                            "Launcher update found. You may download it from www.hitbash.com. Would you like to go there now? Warning: Program is not guaranteed to function properly if launcher is not updated", "Launcher Update",
                            MessageBoxButtons.YesNo);
                        if(click == DialogResult.Yes)
                        {
                            Process.Start("http://www.hitbash.com/projects/budgetmanager/budgetmanager-launcher.exe");
                            listView1.Items.Add("Installation Aborted.");
                            return;
                        }
                    }
                }
                catch
                {
                    listView1.Items.Add("WARNING: Launcher update check failed. This may affect update process.");
                }


                //Check if target directory exists
                listView1.Items.Add("Checking target directory...");
                if(!Directory.Exists(textBoxPath.Text + "/Budget Manager"))
                {
                    listView1.Items.Add("Creating folder in target directory...");
                    try
                    {
                        Directory.CreateDirectory(textBoxPath.Text + "/Budget Manager");
                        listView1.Items.Add("Folder successfully created");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        listView1.Items.Add("UPDATE FAILED. START PROGRAM IN ADMINISTRATOR MODE OR CHANGE");
                        listView1.Items.Add("INSTALL PATH");
                        MessageBox.Show("Update Failed: Please start the program in administrator mode or change the install path using the \"Browse\" button.");
                        return;
                    }
                    catch
                    {
                        listView1.Items.Add("Update failed. Could not create folder in target directory.");
                        listView1.Items.Add("Please change target directory");
                        return;
                    }

                }
                else
                {
                    listView1.Items.Add("Check successful.");
                }

                listView1.Items.Add("Contacting update server...");
                buttonInstall.Enabled = false;
                buttonBrowse.Enabled = false;
                buttonUninstall.Enabled = false;
                DownloadUpdate();
            }
            else
            {
                try
                {
                    Process.Start(textBoxPath.Text + "/Budget Manager/Budget Manager.exe");
                    Application.Exit();
                }
                catch
                {
                    listView1.Items.Add("Launch failed");
                }
            }

        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            if(fb.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = fb.SelectedPath;
            }
        }

        private void buttonUninstall_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show(
                "Are you sure you want to uninstall Finance Manager 2014? All data will be LOST.", "Uninstall", MessageBoxButtons.YesNo)
                == DialogResult.Yes)
            {
                buttonUninstall.Enabled = false;
                buttonInstall.Enabled = false;
                buttonBrowse.Enabled = false;
                listView1.Items.Clear();
                listView1.Items.Add("Uninstalling...");
                listView1.Items.Add("Removing install directory.");
                try
                {
                    Directory.Delete(Program.installPath + "/Budget Manager", true);
                }
                catch (DirectoryNotFoundException)
                {
                    listView1.Items.Add("WARNING: Install directory does not exist.");
                }
                catch (UnauthorizedAccessException)
                {
                    listView1.Items.Add("ERROR: Failed to remove install directory. Please launch as administrator.");
                    listView1.Items.Add("Uninstall aborted.");
                    return;
                }

                listView1.Items.Add("Removing appdata");
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                try
                {
                    Directory.Delete(path + "/BudgetManager", true);
                }
                catch
                {
                    listView1.Items.Add("WARNING: Failed to remove appdata directory");
                }

                listView1.Items.Add("Uninstall complete. Please remove the launcher manually.");
                buttonUninstall.Enabled = true;
                buttonInstall.Enabled = true;
                buttonBrowse.Enabled = true;
            }
        }
    }


}
