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
    public partial class frmSize : Form
    {
       
        public frmSize()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
           

            if ((System.Text.RegularExpressions.Regex.IsMatch(validateTextBox1.TextTxt, "^[0-9]*$") &&
                System.Text.RegularExpressions.Regex.IsMatch(validateTextBox2.TextTxt, "^[0-9]*$")) && 
                !String.IsNullOrWhiteSpace(validateTextBox1.TextTxt) && !String.IsNullOrWhiteSpace(validateTextBox2.TextTxt) &&
                Int32.Parse(validateTextBox1.TextTxt)>= 20 && Int32.Parse(validateTextBox2.TextTxt) >= 20)
             
            {
                DialogResult = DialogResult.OK;
                label3.Text = "";
            }
            else
            {
                label3.Text = "Valor numérico mínimo: 20";
                
            }
        }


    }
}
