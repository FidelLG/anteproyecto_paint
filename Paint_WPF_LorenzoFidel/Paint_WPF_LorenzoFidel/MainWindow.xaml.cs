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
using System.IO;

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
        System.Windows.Controls.TextBox areaText = new System.Windows.Controls.TextBox();
        System.Windows.Point start;
        System.Windows.Point end;

        public MainWindow()
        {
            InitializeComponent();
            rectColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0,0,0));
           
        }

       
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public enum Item
        {
             Line, Triangle, Ellipse, Rectangle
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
                canvasMain.DefaultDrawingAttributes.Color = System.Windows.Media.Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
            }
        }



        private void ButtonClicker(object sender, RoutedEventArgs e)
        {

            var btn = (System.Windows.Controls.Button)sender;
            rectColor.Fill = btn.Background;
            System.Windows.Media.Color x = ((SolidColorBrush)btn.Background).Color;
            canvasMain.DefaultDrawingAttributes.Color = x;
           
      
        }

       

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Imagenes|*.jpg;*.jpeg;*.png|All|*.*";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                canvasMain.EditingMode = InkCanvasEditingMode.Select;
                System.Windows.Controls.Image i = new System.Windows.Controls.Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(openFileDialog1.FileName, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                i.Source = src;
                i.Stretch = Stretch.Fill;
                i.Height = 50;
                i.Width = 50;
                this.canvasMain.Children.Add(i);
               canvasMain.Select(canvasMain.Strokes, new UIElement[] { i });

            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPEG|*.jpeg|JPG|*.jpg|PNG|*.png|All|*.*";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

                string filename = saveFileDialog1.FileName;
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)this.canvasMain.ActualWidth, (int)this.canvasMain.ActualHeight, 0, 0, PixelFormats.Pbgra32);

                rtb.Render(canvasMain);
                rtb.Render(areaText);

                using (FileStream file = new FileStream(filename, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    encoder.Save(file);
                }
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

       

        private void EraseItem_Click(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void PencilItem_Click(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.InkAndGesture;
        }

        private void LineItem_Click(object sender, RoutedEventArgs e)
        {
            currentTool = Item.Line;
        }
      

        private void drawCircleAp(object sender, RoutedEventArgs e)
        {
           
        }

        private void SelectItem_Click(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.Select;
        }

        private void TextItem_Click(object sender, RoutedEventArgs e)
        {

            areaText.BorderThickness = new Thickness(0);
            System.Windows.Media.Color mainColor = ((SolidColorBrush)rectColor.Fill).Color;

            areaText.TextWrapping = TextWrapping.Wrap;
            areaText.Background = canvasMain.Background;
            areaText.FontSize = 15;
            areaText.VerticalAlignment = VerticalAlignment.Center;
            areaText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            canvasMain.Children.Add(areaText);
            canvasMain.Select(new UIElement[] { areaText });

            areaText.Foreground = new SolidColorBrush(mainColor);

        }


     

        private void CanvasMain_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(this);
        }

        private void CanvasMain_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            switch (currentTool) {
                case Item.Line:
                    DrawLine();
                    break;
                case Item.Triangle:
                    break;
                case Item.Ellipse:
                    break;
                case Item.Rectangle:
                    break;
            }
        }
        

        private void CanvasMain_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) {
                end = e.GetPosition(this);
            }
        }

        private void DrawLine() {
            System.Windows.Media.Color mainColor = ((SolidColorBrush)rectColor.Fill).Color;
            Line newLine = new Line();
            newLine.Stroke = new SolidColorBrush(mainColor);
            newLine.X1 = start.X;
            newLine.Y1 = start.Y;
            newLine.X2 = end.X;
            newLine.Y2 = end.Y;
            canvasMain.Children.Add(newLine);

        }

      

        private void Tools_Click(object sender, RoutedEventArgs e)
        {
            Form3 f3 = new Form3();
            DialogResult res; 
            res= f3.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                canvasMain.Width = Double.Parse(f3.validateTextBox1.TextTxt);
                canvasMain.Height = Double.Parse(f3.validateTextBox2.TextTxt);

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
