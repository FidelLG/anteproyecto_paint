using Paint_WPF_LorenzoFidel.Properties;
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
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            object O = Resources.ResourceManager.GetObject("size1");
            object I = Resources.ResourceManager.GetObject("size2");
            object L = Resources.ResourceManager.GetObject("size3");
            button1.Image = (Image)O;
            button2.Image = (Image)I;
            button3.Image = (Image)L;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
