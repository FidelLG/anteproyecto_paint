using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint_WPF_LorenzoFidel
{
    public partial class frmTamaño : Form
    {
        public int num1, num2;
        public frmTamaño()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {


            if (System.Text.RegularExpressions.Regex.IsMatch(validateTextBox1.TextTxt, "[^0-9]") &&
                System.Text.RegularExpressions.Regex.IsMatch(validateTextBox2.TextTxt, "[^0-9]"))
            {
                label3.Text = "Inserte valores numéricos";
            }
            else
            {
                DialogResult = DialogResult.OK;


                label3.Text = "";

            }
        }


    }
}
