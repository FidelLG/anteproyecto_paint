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
        
        System.Windows.Point start;
        System.Windows.Point end;
        System.Windows.Controls.TextBox areaText;

        public MainWindow()
        {
            InitializeComponent();
            rectColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0,0,0));
            canvasMain.EditingMode = InkCanvasEditingMode.None; 
           
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
            if (canvasMain.CanPaste())
                canvasMain.Paste();
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
        
        }

        private void BrushItem_Click(object sender, RoutedEventArgs e)
        {
            Form2 f2 = new Form2();
            f2.ShowDialog();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("IMPRIMIR");
            System.Windows.Controls.PrintDialog prnt = new System.Windows.Controls.PrintDialog();
            if (prnt.ShowDialog() == true)
            {
                System.Windows.Size pageSize = new System.Windows.Size(prnt.PrintableAreaWidth, prnt.PrintableAreaHeight);
                canvasMain.Measure(pageSize);
                canvasMain.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));

                if (prnt.ShowDialog() == true)
                {
                    prnt.PrintVisual(canvasMain, "Printing Canvas");
                }
            }
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
            canvasMain.Children.Clear();
            canvasMain.Strokes.Clear();
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
            canvasDraw = true;
        }
      

        private void drawCircleAp(object sender, RoutedEventArgs e)
        {
            currentTool = Item.Ellipse;
            canvasDraw = true;
        }

        private void SelectItem_Click(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.Select;
        }

        private void TextItem_Click(object sender, RoutedEventArgs e)
        {
            areaText = new System.Windows.Controls.TextBox();
            areaText.BorderThickness = new Thickness(0);
            System.Windows.Media.Color mainColor = ((SolidColorBrush)rectColor.Fill).Color;

            areaText.TextWrapping = TextWrapping.Wrap;
            areaText.Background = canvasMain.Background;
            areaText.FontSize = 15;
            areaText.Background = System.Windows.Media.Brushes.Transparent;
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
            if (canvasDraw) {
                switch (currentTool) {
                    case Item.Line:
                        DrawLine();
                        break;
                    case Item.Triangle:
                        break;
                    case Item.Ellipse:
                        DrawEllipse();
                        break;
                    case Item.Rectangle:
                        DrawRectangle();
                        break;
                }
            }
            canvasDraw = false;
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
            newLine.Y1 = start.Y-50;
            newLine.X2 = end.X;
            newLine.Y2 = end.Y-50;
            canvasMain.Children.Add(newLine);

        }
        private void DrawEllipse() {
            Ellipse newEllipse = new Ellipse();
            newEllipse.Fill = System.Windows.Media.Brushes.Gray;
            newEllipse.Height = 10;
            newEllipse.Width = 10;
            if (end.X >= start.X)
            {
                newEllipse.SetValue(InkCanvas.LeftProperty, start.X);
                newEllipse.Width = end.X - start.X;
            }
            else {
                newEllipse.SetValue(InkCanvas.LeftProperty, end.X);
                newEllipse.Width = start.X - end.X;
            }

            if (end.Y >= start.Y)
            {
                newEllipse.SetValue(InkCanvas.TopProperty, start.Y - 50);
                newEllipse.Height = end.Y - start.Y;
            }
            else {
                newEllipse.SetValue(InkCanvas.TopProperty, end.Y - 50);
                newEllipse.Height = start.Y - end.Y;
            }
            canvasMain.Children.Add(newEllipse);
        }

        private void DrawRectangle() {
            System.Windows.Shapes.Rectangle newRectangle = new System.Windows.Shapes.Rectangle();
            newRectangle.Fill = System.Windows.Media.Brushes.DarkBlue;
            if (end.X >= start.X)
            {
                newRectangle.SetValue(InkCanvas.LeftProperty, start.X);
                newRectangle.Width = end.X - start.X;
            }
            else
            {
                newRectangle.SetValue(InkCanvas.LeftProperty, end.X);
               newRectangle.Width = start.X - end.X;
            }

            if (end.Y >= start.Y)
            {
                newRectangle.SetValue(InkCanvas.TopProperty, start.Y - 50);
                newRectangle.Height = end.Y - start.Y;
            }
            else
            {
                newRectangle.SetValue(InkCanvas.TopProperty, end.Y - 50);
                newRectangle.Height = start.Y - end.Y;
            }
            canvasMain.Children.Add(newRectangle);
        }
        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.GetSelectedStrokes().Count > 0)
            {
                canvasMain.CopySelection();
            }
        }

        private void CutItem_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.GetSelectedStrokes().Count > 0 || canvasMain.GetSelectedElements().Count > 0)
            {
                canvasMain.CutSelection();
            }
        }

        private void SliderStroke_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderStroke.Value == 0)
            {
                SliderStroke.Minimum = 1;
            }
            else {
                canvasMain.DefaultDrawingAttributes.Height = (SliderStroke.Value*3);
                canvasMain.DefaultDrawingAttributes.Width = (SliderStroke.Value*3);
        
            }
        }

        private void SliderText_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            areaText.FontSize = SliderText.Value;
        }

        private void RectangleItem_Click(object sender, RoutedEventArgs e)
        {
            currentTool = Item.Rectangle;
            canvasDraw = true;
        }

        private void EraseStrokeItem_Click(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        private void Undo_Click_1(object sender, RoutedEventArgs e)
        {
           
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
