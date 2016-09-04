using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Finance_Manager
{
    public partial class NewAccount : Form
    {
        Account oldAccount;
        bool editing;
        public NewAccount()
        {
            InitializeComponent();
            editing = false;
        }

        public NewAccount(Account old)
        {
            InitializeComponent();
            textBox1.Text = old.name;
            this.oldAccount = old;
            this.editing = true;
        }

        private void NewAccount_Load(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if(editing)
                {
                    oldAccount.name = textBox1.Text;
                }
                else
                {
                    Account account = new Account(textBox1.Text);
                    Data.accounts.Add(account);
                    Data.isCancelled = false;
                }
                this.Close();
            }
        }
    }
}
