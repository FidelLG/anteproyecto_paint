using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using System.Drawing;

namespace Paint_WPF_LorenzoFidel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        bool canvasDraw = false;
        bool aux = false;
        System.Drawing.Color colorFromMenu;
        Item currentTool;
        int x, y ,lx, ly=0;
        System.Drawing.Color customColor = System.Drawing.Color.FromArgb(50, System.Drawing.Color.Gray);


        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public enum Item
        {
            Pencil, Eraser, PaintBucket, Text, Brush, Line, Triangle, Ellipse, Rectangle
        }


        private void PasteItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void PaletaItem_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rectColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B));
            }
        }



        private void ButtonClicker(object sender, RoutedEventArgs e)
        {

            var btn = (System.Windows.Controls.Button)sender;
            rectColor.Fill = btn.Background;
      
        }

       

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Imagenes|*.jpg;*.jpeg;*.png|All|*.*";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {



            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Imagenes|*.jpg;*.jpeg;*.png|All|*.*";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Autor: Fidel Lorenzo García\nVersión: 1.0","Paint .Net Acerca de...");
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            toolbar.Width = MainForm.ActualWidth;
            MenuMain.Width = MainForm.ActualWidth;
           
        }

        private void SizeLineItem_Click(object sender, RoutedEventArgs e)
        {
           Form1 w1 = new Form1();
            w1.ShowDialog();
            
        }

        private void BrushItem_Click(object sender, RoutedEventArgs e)
        {
            Form2 f2 = new Form2();
            f2.ShowDialog();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("IMPRIMIR");
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("UNDO");
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("REDO");
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("NEW");
        }

        private void CanvasMain_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (canvasDraw)
            {
                
                switch (currentTool)
                {
                   
                    case Item.Pencil:
                      
                        break;
                    
                }
               
            }
        }

        private void Tools_Click(object sender, RoutedEventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
            if (f3.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                canvasMain.Width = int.Parse(f3.textBox1.Text);
                canvasMain.Height = int.Parse(f3.textBox2.Text);
                
            }
        }

        private void CanvasMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            canvasDraw = false;
            lx = e.X;
            ly = e.Y;
        }

        private void CanvasMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            canvasDraw = true;
            x = e.X;
            y = e.Y;
        }

    }
   
}
