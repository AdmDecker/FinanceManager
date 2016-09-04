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
    public partial class NewBudget : Form
    {
        bool editing;
        Budget originalBudget;


        public NewBudget()
        {
            InitializeComponent();
        }

        public NewBudget(Budget originalBudget)
        {
            InitializeComponent();
            this.editing = true;
            this.originalBudget = originalBudget;
            textBox1.Text = originalBudget.name;
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                //Add new budget with name in textbox
                if(!editing)
                {
                    Data.getActiveBList().Add(new Budget(textBox1.Text));
                }
                else
                {
                    originalBudget.name = textBox1.Text;
                }

                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
