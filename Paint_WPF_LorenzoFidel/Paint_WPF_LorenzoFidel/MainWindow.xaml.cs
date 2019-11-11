using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Paint_WPF_LorenzoFidel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        bool canvasDraw = false;
        bool editing = false;
        System.Drawing.Color colorFromMenu;
        Item currentTool;
        int x, y, lx, ly = 0;
        System.Drawing.Color customColor = System.Drawing.Color.FromArgb(50, System.Drawing.Color.Gray);
        
        System.Windows.Point start;
        System.Windows.Point end;
        System.Windows.Controls.TextBox areaText;

        System.Windows.Ink.StrokeCollection _added;
        System.Windows.Ink.StrokeCollection _removed;
        private bool handle = true;

        public MainWindow()
        {
            InitializeComponent();
            rectColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            canvasMain.EditingMode = InkCanvasEditingMode.None;
            canvasMain.Strokes.StrokesChanged += Strokes_StrokesChanged;
            canvasMain.Background = Brushes.White;
            this.Title = "Paint .Net (" + canvasMain.Width + " ," + canvasMain.Height + ")";
        }

        //Cambio de tamaño del toolbar y menu con el grid
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            toolbar.Width = MainForm.ActualWidth;
            MenuMain.Width = MainForm.ActualWidth;
        }

        //Cambio del titulo con el tamaño del inkcanvas, cuando este cambia.
        private void CanvasMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Title="Paint .Net (" + canvasMain.Width + " ," + canvasMain.Height + ")";
        }

        //Posicion delk raton al levantar
        private void CanvasMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            canvasDraw = false;
            lx = e.X;
            ly = e.Y;
        }

        //Posición del ratón al bajar
        private void CanvasMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            canvasDraw = true;
            x = e.X;
            y = e.Y;
           
        }
        //enumerado formas
        public enum Item
        {
            Line, Triangle, Ellipse, Rectangle
        }

        //coleccion de cambios del stroke usado para deshacer y rehacer
        private void Strokes_StrokesChanged(object sender, System.Windows.Ink.StrokeCollectionChangedEventArgs e)
        {
            if (handle)
            {
                _added = e.Added;
                _removed = e.Removed;
            }
        }


        //menu Archivo
        //Nuevo
        private void New_Click(object sender, RoutedEventArgs e)
        {
            
            canvasMain.Children.Clear();
            canvasMain.Strokes.Clear();
        }
        
        //Abrir
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

        //Guardar
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPEG|*.jpeg|JPG|*.jpg|PNG|*.png|All|*.*";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                string filename = saveFileDialog1.FileName;
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)this.canvasMain.RenderSize.Width, (int)this.canvasMain.RenderSize.Height,0, 0, PixelFormats.Pbgra32);

                rtb.Render(canvasMain);
               // rtb.Render(areaText);

                using (FileStream file = new FileStream(filename, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    encoder.Save(file);
                }
            }
        }

        //Imprimir
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

        //Acerca de..
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // System.Windows.MessageBox.Show("Autor: Fidel Lorenzo García\nVersión: 1.0","Paint .Net Acerca de...");
            System.Windows.MessageBox.Show(_added.Count.ToString(), "Paint .Net Acerca de...");

        }

        //Menu Herramientas (Tamaño Lienzo)
        private void Tools_Click(object sender, RoutedEventArgs e)
        {
            frmTamaño f3 = new frmTamaño();
            DialogResult res;
            res = f3.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                canvasMain.Width = Double.Parse(f3.validateTextBox1.TextTxt);
                canvasMain.Height = Double.Parse(f3.validateTextBox2.TextTxt);

            }
        }

        //Bloque rehacer/deshacer

        //deshacer
        private void Undo_Click(object sender, RoutedEventArgs e)
        {


            if (_added != null && _added.Count > 0)
            {
                handle = false;
                canvasMain.Strokes.Remove(_added);
                canvasMain.Strokes.Add(_removed);
                handle = true;
            }

        }

        //rehacer
        private void Redo_Click(object sender, RoutedEventArgs e)
        {

                handle = false;
            if (_added!=null)
            {
                canvasMain.Strokes.Add(_added);
                canvasMain.Strokes.Remove(_removed);
                handle = true;
            }
        }
        //Bloque Cortar,Copiar,Pegar
        //Cortar
        private void CutItem_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.GetSelectedStrokes().Count > 0 || canvasMain.GetSelectedElements().Count > 0)
            {
                canvasMain.CutSelection();
            }
        }

        //Copiar
        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.GetSelectedStrokes().Count > 0)
            {
                canvasMain.CopySelection();
            }
        }

        //Pegar
        private void PasteItem_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.CanPaste())
                canvasMain.Paste();
        }


        //Bloque Lápiz, borrar, texto selcción
        //Lápiz
        private void PencilItem_Click(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.Ink;
        }

        //Borrar por puntos
        private void EraseItem_Click(object sender, RoutedEventArgs e)
        {
            
            
            canvasMain.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        //Borrar trazado entero
        private void EraseStrokeItem_Click(object sender, RoutedEventArgs e)
        {
            
            canvasMain.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        //Texto
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

        //Selección
        private void SelectItem_Click(object sender, RoutedEventArgs e)
        {
            
            canvasMain.EditingMode = InkCanvasEditingMode.Select;
        }

        //Bloque formas
        //Selección de línea
        private void LineItem_Click(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.None;
            currentTool = Item.Line;
            canvasDraw = true;
        }
        
        //Dibujado de línea
        private void DrawLine()
        {
            System.Windows.Media.Color mainColor = ((SolidColorBrush)rectColor.Fill).Color;
            Line newLine = new Line();
            newLine.Stroke = new SolidColorBrush(mainColor);
            newLine.X1 = start.X;
            newLine.Y1 = start.Y - 50;
            newLine.X2 = end.X;
            newLine.Y2 = end.Y - 50;
            canvasMain.Children.Add(newLine);

        }

        //Selección de triángulo
        private void TriangleItem_Click(object sender, RoutedEventArgs e)
        {
            currentTool = Item.Triangle;
            canvasDraw = true;
            canvasMain.EditingMode = InkCanvasEditingMode.None;
        }

        //Dibujado de trinángulo
        private void DrawTriangle()
        {
            System.Windows.Media.Color mainColor = ((SolidColorBrush)rectColor.Fill).Color;
            StylusPointCollection stroke1Points = new StylusPointCollection();
            stroke1Points.Add(new StylusPoint(50, 10));
            stroke1Points.Add(new StylusPoint(90, 50));
            stroke1Points.Add(new StylusPoint(10, 50));
            stroke1Points.Add(new StylusPoint(50, 10));

            Stroke stroke1 = new Stroke(stroke1Points);
            stroke1.DrawingAttributes.Color = mainColor;
            canvasMain.Strokes.Add(stroke1);
            
        }

        //Selección de elipse
        private void drawCircleAp(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.None;
            currentTool = Item.Ellipse;
            canvasDraw = true;

        }

        //Dibujado de elipse
        private void DrawEllipse()
        {
            Ellipse newEllipse = new Ellipse();
            newEllipse.Stroke = Brushes.Black;
            newEllipse.Fill = (SolidColorBrush)rectColor.Fill;
            newEllipse.Height = 10;
            newEllipse.Width = 10;
            if (end.X >= start.X)
            {
                newEllipse.SetValue(InkCanvas.LeftProperty, start.X);
                newEllipse.Width = end.X - start.X;
            }
            else
            {
                newEllipse.SetValue(InkCanvas.LeftProperty, end.X);
                newEllipse.Width = start.X - end.X;
            }

            if (end.Y >= start.Y)
            {
                newEllipse.SetValue(InkCanvas.TopProperty, start.Y - 50);
                newEllipse.Height = end.Y - start.Y;
            }
            else
            {
                newEllipse.SetValue(InkCanvas.TopProperty, end.Y - 50);
                newEllipse.Height = start.Y - end.Y;
            }
            canvasMain.Children.Add(newEllipse);
        }

        //Selección de rectángulo
        private void RectangleItem_Click(object sender, RoutedEventArgs e)
        {
            currentTool = Item.Rectangle;
            canvasDraw = true;
            canvasMain.EditingMode = InkCanvasEditingMode.None;
        }

        //Dibujado de rectángulo
        private void DrawRectangle()
        {
            System.Windows.Shapes.Rectangle newRectangle = new System.Windows.Shapes.Rectangle();
            newRectangle.Stroke = Brushes.Black;
            newRectangle.Fill = (SolidColorBrush)rectColor.Fill;
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
        //LLamada a las distintas formas dependiendo del enumerado seleccionado
        //Posicion del raton al moverse
        private void CanvasMain_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                end = e.GetPosition(this);
            }
        }

        //Posicion al bajarse el raton
        private void CanvasMain_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(this);
        }

        //Switch con los enumerados
        private void CanvasMain_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            
            if (canvasDraw)
            {
                switch (currentTool)
                {
                    case Item.Line:
                        DrawLine();
                        break;
                    case Item.Triangle:
                        DrawTriangle();
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

        //Bloque tamaños
        //Slider para cambiar el tamaño del lápiz
        private void SliderStroke_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderStroke.Value == 0)
            {
                SliderStroke.Minimum = 1;
            }
            else
            {
                canvasMain.DefaultDrawingAttributes.Height = (SliderStroke.Value * 3);
                canvasMain.DefaultDrawingAttributes.Width = (SliderStroke.Value * 3);

            }
        }

        //slider para cambiar el tamaño de la fuente del textBox
        private void SliderText_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            areaText.FontSize = SliderText.Value;
        }

        //Bloque Colores
        //Todos los botones de colores asigan a un rectangulo su color, que sera el color actual
        private void ButtonClicker(object sender, RoutedEventArgs e)
        {

            var btn = (System.Windows.Controls.Button)sender;
            rectColor.Fill = btn.Background;
            System.Windows.Media.Color x = ((SolidColorBrush)btn.Background).Color;
            canvasMain.DefaultDrawingAttributes.Color = x;


        }

        //ColorDialog para una selección de colores más avanzada
        private void PaletaItem_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rectColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B));
                canvasMain.DefaultDrawingAttributes.Color = System.Windows.Media.Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
            }
        }




        


    }

   
}
