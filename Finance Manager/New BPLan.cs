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
    public partial class New_BPLan : Form
    {
        BudgetPlan budgetPlan = new BudgetPlan();
        bool editing = false;


        public New_BPLan()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(Data.getActiveBList().ToArray());
        }

        public New_BPLan(BudgetPlan oldPlan)
        {
            InitializeComponent();
            budgetPlan = oldPlan;
            comboBox1.Items.AddRange(Data.getActiveBList().ToArray());
            Update_BudgeterList();
            textBox1.Text = oldPlan.name;
            CalculateMinimum();
            this.editing = true;
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                if(budgetPlan.list.Count != 0)
                {
                    CalculateMinimum();
                    budgetPlan.name = textBox1.Text;
                    if(this.editing == false)
                    {
                        Data.getActiveAccount().planList.Add(budgetPlan);
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please add budgets to the plan.");
                }

            }
            else
            {
                MessageBox.Show("Please enter a name for the budget.");
                return;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 0 && comboBox1.SelectedIndex >= 0)
            {
                Budgeter budgeter = new Budgeter();
                if (rbDollar.Checked)
                {
                    budgeter.type = 0;
                    budgeter.value = numericUpDown1.Value;
                    budgeter.budget = Data.getActiveBList().ElementAt(comboBox1.SelectedIndex);
                    budgetPlan.list.Add(budgeter);
                }
                else if (rbPercent.Checked)
                {
                    budgeter.type = 1;
                    //
                    //Make sure that the percent won't go over 100
                    //

                    //Calculate current percent value
                    decimal percentSum = 0;
                    for (int x = 0; x < this.budgetPlan.list.Count; x++)
                    {
                        Budgeter budgeteer = this.budgetPlan.list.ElementAt(x);

                        // Add percentages if it's percent
                        if (budgeteer.type == 1)
                        {
                            percentSum += budgeteer.value;
                        }
                    }
                    if(percentSum + numericUpDown1.Value >= 100)
                    {
                        MessageBox.Show("Error: This action will cause the total percent allocation to meet or exceed 100%.  Please define a smaller amount or remove a budget.");
                        return;
                    }

                    //
                    //
                    //


                    budgeter.value = numericUpDown1.Value;
                    budgeter.budget = Data.getActiveBList().ElementAt(comboBox1.SelectedIndex);
                    budgetPlan.list.Add(budgeter);
                }

                CalculateMinimum();
                Update_BudgeterList();
            }
        }

        private void Update_BudgeterList()
        {
            listView1.Items.Clear();
            //Cycle through budgeters in budgeterList
            for (int x = 0; x < budgetPlan.list.Count; x++)
            {
                string money = string.Format("{0:0.00}", budgetPlan.list.ElementAt(x).value);
                if (budgetPlan.list.ElementAt(x).type == 0)
                    money = '$' + money;
                else if (budgetPlan.list.ElementAt(x).type == 1)
                    money = money + '%';
                ListViewItem item = new ListViewItem(new[] { budgetPlan.list.ElementAt(x).ToString(), money });
                listView1.Items.Add(item);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CalculateMinimum()
        {
            decimal moneySum = 0;
            decimal percentSum = 0;
            for (int x = 0; x < this.budgetPlan.list.Count; x++)
            {
                Budgeter budgeter = this.budgetPlan.list.ElementAt(x);
                // Add money if it's dollars
                if (budgeter.type == 0)
                {
                    moneySum += budgeter.value;
                }
                // Add percentages if it's percent
                if (budgeter.type == 1)
                {
                    percentSum += budgeter.value;
                }
            }
            //Calculate the minimum based on my beautiful formula :)
            decimal minimum = moneySum / (1 - (percentSum / 100));
            //Check if there are more than 2 decimals
            if(Math.Truncate(minimum * 100) != minimum * 100)
                minimum = (Math.Truncate(minimum * 100) + 1) / 100;
            budgetPlan.minimumDeposit = minimum;
            tbMinimumDeposit.Text = '$' + minimum.ToString();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedIndices.Count > 0)
            {
                budgetPlan.list.RemoveAt(listView1.SelectedIndices[0]);
                listView1.Items.Clear();
                Update_BudgeterList();
            }
            CalculateMinimum();
        }

    }
}