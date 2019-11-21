////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	MainWindow.xaml.cs
//
// summary:	Implements the main window.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
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
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Interaction logic for MainWindow.xaml. </summary>
    ///
    /// <remarks>   Fidi, 11/16/2019. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class MainWindow : Window
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Booleana usada el manjeo del pintado de las distintas formas del enumerado de Items.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        bool canvasDraw = false;
        //bool editing = false;
        /// <summary>   Booleana usada para el manejo de la opción Deshacer. </summary>
        bool undoDone = true;
        /// <summary>   Booleana usada para el manejo de la opción Rehacer. </summary>
        bool redoDone =true;
        /// <summary>   Variable tipo enumerado para identificar el tipo de forma que se va a pintar. </summary>
        Item currentTool;
        /// <summary>   Variables tipo int para identificar las posiciones del ratón. </summary>
        int x, y, lx, ly = 0;
        /// <summary>   The custom color. </summary>
     //   System.Drawing.Color customColor = System.Drawing.Color.FromArgb(50, System.Drawing.Color.Gray);
        /// <summary>   Objeto tipo Point para la posición de comienzo del ratón. </summary>
        System.Windows.Point start;
        /// <summary>   Objeto tipo Point usada para la posición de fin del ratón. </summary>
        System.Windows.Point end;
        /// <summary>   Objeto Texto que se usa en la herramienta "Texto" del programa. </summary>
        System.Windows.Controls.TextBox areaText = new System.Windows.Controls.TextBox();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Colección StrokeCollection usada para la funcionalidad de Deshacer y Rehacer.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        System.Windows.Ink.StrokeCollection _added;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Colección StrokeCollection usada para la funcionalidad de Deshacer y Rehacer.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        System.Windows.Ink.StrokeCollection _removed;
        /// <summary>   Booleana que maneja el funcionamiento de Deshacer y Rehacer. </summary>
        private bool handle = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor del formulario. </summary>
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se ejecuta cuando el formulario cambia de tamaño. El menú y ToolBar cambian 
        ///   al mismo tamaño que el formulario.          
        ///</summary>
        ///
        /// <param name="sender">   Formulario. </param>
        /// <param name="e">        Evento <b>SizeChanged</b> del propio formulario </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            toolbar.Width = MainForm.ActualWidth-20;
            MenuMain.Width = MainForm.ActualWidth-20;
            
        }

        //Cambio del titulo con el tamaño del inkcanvas, cuando este cambia.

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se ejecuta cuando el Inkcanvas cambia de tamaño. Actualiza el tamaño del InkCanvas 
        ///    en el título         
        /// </summary>
        ///
        /// <param name="sender">   InkCanvas. </param>
        /// <param name="e">        Evento <b>SizeChanged</b> </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CanvasMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Title="Paint .Net (" + canvasMain.Width + " ," + canvasMain.Height + ")";
            areaText.MaxHeight = canvasMain.ActualHeight ;
            areaText.MaxWidth = canvasMain.ActualWidth ;
        }

        //Posicion del raton al levantar

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza al levantar el click ratón para obeter las coordenadas. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   InkCanvas. </param>
        /// <param name="e">        Evento <b>MouseUp</b> </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CanvasMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            canvasDraw = false;
            lx = e.X;
            ly = e.Y;
        }

        //Posición del ratón al bajar

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza al bajar el ratón para obtener coordenadas. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   InkCanvas. </param>
        /// <param name="e">        Evento <b>MouseDown</b> </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CanvasMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            canvasDraw = true;
            x = e.X;
            y = e.Y;
           
        }
        //enumerado formas

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Enumerado Item con los distintos tipos de formas. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum Item
        {
            /// <summary>   An enum constant representing the line option. </summary>
            Line, Triangle, Ellipse, Rectangle
        }

        //coleccion de cambios del stroke usado para deshacer y rehacer

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza cuando cambia un objeto Stroke en la colección.Se usa para el manejo de 
        ///             de Deshacer y Rehacer </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Strokes. </param>
        /// <param name="e">        Evento <b>StrokesChanged</b> </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (handle)
            {
                _added = e.Added;
                _removed = e.Removed;
                undoDone = false;
                Undo.IsEnabled = true;
                Undo.Background = Brushes.LightGray;
                redoDone = true;
                Redo.IsEnabled = false;
                Redo.Background = Brushes.Transparent;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Método genérico para abrir archivos,en este caso imágenes. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="width">    Valor para el ancho de la imagen. </param>
        /// <param name="heigth">   Valor para el alto de la imagen. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void openFiles(double width, double heigth)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "JPEG|*.jpeg|JPG|*.jpg|PNG|*.png|All|*.*";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
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
                    i.Height = heigth;
                    i.Width = width;
                    this.canvasMain.Children.Add(i);
                    canvasMain.Select(canvasMain.Strokes, new UIElement[] { i });
                }
                catch (NotSupportedException ex)
                {
                    System.Windows.Forms.MessageBox.Show("Solo se pueden abrir imágenes", "Error al abrir imagen", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Método genérico para guardar archivos, en este caso imágenes. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void saveFiles() {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "ImagenWPF";
            saveFileDialog1.DefaultExt = ".jpg";
            saveFileDialog1.Filter = "JPEG|*.jpeg|JPG|*.jpg|PNG|*.png|All|*.*";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                string filename = saveFileDialog1.FileName;

                var size = new Size(canvasMain.ActualWidth, canvasMain.ActualHeight);
                canvasMain.Margin = new Thickness(0, 0, 0, 0);
                canvasMain.Measure(size);
                canvasMain.Arrange(new Rect(size));
                var bitmapTarget = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Default);
                bitmapTarget.Render(canvasMain);

                using (FileStream file = new FileStream(filename, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapTarget));
                    encoder.Save(file);
                    canvasMain.Margin = new Thickness(0, 81, 0, 0);
                }
            }
        }

        //Se pregunta al usuario si desea guardar el progreso sin guardar, (Utilizando las colecciones Strokes y Children)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Método para detectar si el InkCanvas ha sido modificado. Se pregunta al usuario si desea
        /// guardar el progreso sin guardar,  (Utilizando las colecciones Strokes y Children)
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="option">   Valor para distinguir entre las funciones de Nuevo(true) y
        ///                         abrir(false) </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void unsavedChanges(bool option) {
           
            DialogResult res= System.Windows.Forms.MessageBox.Show("¿Desea guardar cambios?", "Paint .Net",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            switch (res) {
                case System.Windows.Forms.DialogResult.Yes:
                    saveFiles();
                    break;
                case System.Windows.Forms.DialogResult.No:
                    if (option)
                    {
                        canvasMainClear();
                    }
                    else
                    {
                        canvasMainClear();
                        openFiles(canvasMain.Width,canvasMain.Height);
                    }
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                    break;

            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Método para limpar las colecciones Strokes y Children de InkCanvas. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void canvasMainClear() {
            canvasMain.Strokes.Clear();
            canvasMain.Children.Clear();
        }

        //menu Archivo
        //Nuevo, 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza cuando usamos el menu Nuevo.Crea un InkCanvas vacio. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Submenú Nuevo. </param>
        /// <param name="e">        Evento <b>Click</b> del menú Nuevo. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.Strokes.Count > 0 || canvasMain.Children.Count > 0)
            {
                unsavedChanges(true);
            }
            else {
                canvasMainClear();
            }

            
        }
    
        //Abrir

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza cuando usamos el menú Abrir.Abre una imagen desde un directorio. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Submenú Abrir. </param>
        /// <param name="e">        Evento <b>Click</b> del menú Abrir. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.Strokes.Count > 0 || canvasMain.Children.Count > 0)
            {
                unsavedChanges(false);
            }
            else
            {
                openFiles(canvasMain.Width,canvasMain.Height);
            }
          
        }

        //Importar

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza cuando usamos el menú Importar, Importa una imagen directamente a tu
        /// trabajo actual.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Submenú Importar. </param>
        /// <param name="e">        Evento <b>Click</b> del menú Importar. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            openFiles(canvasMain.Width/3,canvasMain.Height/3);
        }

        //Guardar

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza cuando usamos el menú Guardar. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Submenú Guardar. </param>
        /// <param name="e">        Evento <b>Click</b> del menú Guardar. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            saveFiles();
        }

        //Imprimir

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza al usar el menú Imprimir. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Submenú Imprimir. </param>
        /// <param name="e">        Evento <b>Click</b> del menú Imprimir. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Print_Click(object sender, RoutedEventArgs e)
        {
           
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza al usar el menú Acerca de..., donde se especifica información sobre el programa </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Submenú Acerca de... </param>
        /// <param name="e">        Evento <b>Click</b> del menú Acerca de... </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Autor: Fidel Lorenzo García\nVersión: 1.0.7","Paint .Net Acerca de...");
            Console.WriteLine(canvasMain.Children.Count.ToString());
 
        }

        //Menu Herramientas

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza al usar el menú Herrramientas, para cambiar el tamaño del InkCanvas desde otro
        /// formulario.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   menú Herramientas. </param>
        /// <param name="e">        Evento <b>Click</b> del menú Herramientas. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Tools_Click(object sender, RoutedEventArgs e)
        {
            frmSize f3 = new frmSize();
            DialogResult res;
            res = f3.ShowDialog();


            if (res == System.Windows.Forms.DialogResult.OK)
            {
                canvasMain.Width = Double.Parse(f3.validateTextBox1.TextTxt);
                canvasMain.Height = Double.Parse(f3.validateTextBox2.TextTxt);

            }
        }

        //Bloque rehacer/deshacer

        //deshacer, solo deshace el ultimo Stoke(triángulo o trazo de lápiz)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza al usar el menú Deshacer. Sólo deshace el último Stroke(triángulo o trazo
        /// de lápiz), ya que el resto de elementos están en la colección InkCanvasChildren.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Submenú Deshacer. </param>
        /// <param name="e">        Evento <b>Click</b> del menú Deshacer. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Undo_Click(object sender, RoutedEventArgs e)
        {


            if (undoDone==false)
            {
                handle = false;
                canvasMain.Strokes.Remove(_added);
                canvasMain.Strokes.Add(_removed);
               
                handle = true;
                undoDone = true;
                Undo.IsEnabled = false;
                Undo.Background = Brushes.Transparent;
                redoDone = false;
                Redo.IsEnabled = true;
                Redo.Background = Brushes.LightGray;
            }

        }

        //rehace el último deshacer (solamente 1)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza al usar el menú Rehacer. Rehace el último deshacer, (Solamente permite hacer 1 operación) </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Submenú Rehacer. </param>
        /// <param name="e">        Evento <b>Click</b> del menú Rehacer. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Redo_Click(object sender, RoutedEventArgs e)
        {

            if (redoDone==false)
            {
                handle = false;
                canvasMain.Strokes.Add(_added);
                canvasMain.Strokes.Remove(_removed);
                handle = true;
                undoDone = false;
                Undo.IsEnabled = true;
                Undo.Background = Brushes.LightGray;
                redoDone = true;
                Redo.IsEnabled = false;
                Redo.Background = Brushes.Transparent;
            }
        }
        //Bloque Cortar,Copiar,Pegar
        //Cortar

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza al usar la herramienta Cortar(Solamente funciona con strokes)
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Herramienta Cortar. </param>
        /// <param name="e">        Evento <b>Click</b> de la herramienta Cortar. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CutItem_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.GetSelectedStrokes().Count > 0 || canvasMain.GetSelectedElements().Count > 0)
            {
                canvasMain.CutSelection();
            }
        }

        //Copiar

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza al usar la herramienta Copiar(Solamente funciona con strokes)
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Herramienta Copiar. </param>
        /// <param name="e">        Evento <b>Click</b> de la herramienta Copiar. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.GetSelectedStrokes().Count > 0)
            {
                canvasMain.CopySelection();
            }
        }

        //Pegar

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza al usar la herramienta Pegar(Solamente funciona con strokes)
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Herramienta Pegar. </param>
        /// <param name="e">        Evento <b>Click</b> de la herramienta Pegar. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PasteItem_Click(object sender, RoutedEventArgs e)
        {
            if (canvasMain.CanPaste())
                canvasMain.Paste();
        }


        //Bloque Lápiz, borrar, texto selcción
        //Lápiz

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza al usar la herramienta Lápiz, con el que puedes dibujar trazados con el
        /// ratón.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Herramienta Lápiz. </param>
        /// <param name="e">        Evento <b>Click</b> de la herramienta Lápiz. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PencilItem_Click(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.Ink;
        }

        //Borrar por puntos

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza al usar la herramienta "Borrar por puntos". </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Herramienta Borrar por puntos. </param>
        /// <param name="e">        Evento <b>Click</b> de la herramienta Borrar por puntos. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void EraseItem_Click(object sender, RoutedEventArgs e)
        {
            
            
            canvasMain.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        //Borrar trazado entero

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que lanza cuand ose usa la herramienta Borrar trazado entero. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Herramienta Borrador de trazos. </param>
        /// <param name="e">        Evento <b>Click</b> de la herramienta borrador por trazos. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void EraseStrokeItem_Click(object sender, RoutedEventArgs e)
        {
            
            canvasMain.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        //Texto

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza al usarla herramienta Texto, añade un textbox al InkCanvas.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Herramienta Texto. </param>
        /// <param name="e">        Evento <b>Click</b> de la herramienta Texto. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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
            areaText.MaxHeight = canvasMain.ActualHeight-20;
            areaText.MaxWidth = canvasMain.ActualWidth-20;
            canvasMain.Children.Add(areaText);
            canvasMain.Select(new UIElement[] { areaText });

            areaText.Foreground = new SolidColorBrush(mainColor);

        }

        //Selección

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza cuando se usa la herramienta Selección, puede seleccionar Strokes,Shapes,Textos e Imágenes
        /// objeto de mi programa.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Herramienta Selección. </param>
        /// <param name="e">        Evento <b>Click</b> de la herramienta Selección. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SelectItem_Click(object sender, RoutedEventArgs e)
        {
            
            canvasMain.EditingMode = InkCanvasEditingMode.Select;
        }

        //Bloque formas
        //Selección de línea

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza cuando se usa la forma Línea, se usa arrastrando el ratón. Se identifica
        /// que enumerado es.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Forma Línea. </param>
        /// <param name="e">        Evento <b>Click</b> de la forma Línea. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void LineItem_Click(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.None;
            currentTool = Item.Line;
            canvasDraw = true;
        }
        
        //Dibujado de línea

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Se crea el objeto Línea y se añade a la colección Children (se dibuja) </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza cuando usamos la forma trinángulo, aparece en la esquina izda superior.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Forma trinángulo. </param>
        /// <param name="e">        Evento <b>Click</b> de la forma Triángulo. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void TriangleItem_Click(object sender, RoutedEventArgs e)
        {
            currentTool = Item.Triangle;
            canvasDraw = true;
            canvasMain.EditingMode = InkCanvasEditingMode.None;
        }

        //Dibujado de trinángulo

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Se crea el triángulo añadiendolo en la colección strokes. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza cuando usamos la forma Elipse,y se identifica que enumerado es.
        /// </summary>
        ///
        /// <remarks>   <code>Fidi, 11/16/2019.</code> </remarks>
        ///
        /// <param name="sender">   Forma Elipse. </param>
        /// <param name="e">        Evento <b>Click</b> de la forma Elipse. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void drawCircleAp(object sender, RoutedEventArgs e)
        {
            canvasMain.EditingMode = InkCanvasEditingMode.None;
            currentTool = Item.Ellipse;
            canvasDraw = true;

        }

        //Dibujado de elipse

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Se crea el objeto tipo Elipse y se añade a la colección Strokes y se dibuja.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Evento que se lanza cuando se usa la Forma rectángulo. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Forma Rectángulo. </param>
        /// <param name="e">        Evento <b>Click</b> de la forma Rectángulo. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RectangleItem_Click(object sender, RoutedEventArgs e)
        {
            currentTool = Item.Rectangle;
            canvasDraw = true;
            canvasMain.EditingMode = InkCanvasEditingMode.None;
        }

        //Dibujado de rectángulo

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Se crea el objeto tipo rectángulo, con las posiciones del ratón y se añade a la colección
        /// Children.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Se obtiene la última posición del cursor al mover el ratón. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   InkCanvas. </param>
        /// <param name="e">        Evento <b>MouseMove</b> del InkCanvas. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CanvasMain_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                end = e.GetPosition(this);
            }
        }

        //Posicion al bajarse el raton

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Se obtiene la posición del cursor por primera vez al bajar el ratón. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   InkCanvas. </param>
        /// <param name="e">        Evento <b>MouseDown</b> del InkCanvas. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CanvasMain_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(this);

               
            //if (canvasMain.EditingMode!=InkCanvasEditingMode.Select && editing==false) {
            //    editing = true;
            //}
        }

        //Switch con los enumerados

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Se obtiene el tipo de enumerado que se va a dibujar. </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   InkCanvas. </param>
        /// <param name="e">        Evento <b>MouseUp</b> del InkCanvas. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza cuando se cambian los valores del slider del tamaño del lápiz.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   SliderStroke. </param>
        /// <param name="e">        Evento <b>ValueChanged</b> del SliderStrokes. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza cuando se cambian los valores del slider del tamaño de la fuente del
        /// textbox.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   SliderText. </param>
        /// <param name="e">        Evento <b>ValueChanged</b> del SliderText. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SliderText_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (areaText!=null) {
            areaText.FontSize = SliderText.Value;
            }
        }

        //Bloque Colores
        //Todos los botones de colores asigan a un rectangulo su color, que sera el color actual

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza al pinchar los botones de los colores, para seleccionar uno.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Button. </param>
        /// <param name="e">        Evento <b>Click</b> de los botones. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonClicker(object sender, RoutedEventArgs e)
        {

            var btn = (System.Windows.Controls.Button)sender;
            rectColor.Fill = btn.Background;
            System.Windows.Media.Color x = ((SolidColorBrush)btn.Background).Color;
            canvasMain.DefaultDrawingAttributes.Color = x;


        }

        //ColorDialog para una selección de colores más avanzada

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Evento que se lanza cuando se usa la herramienta Editar, para seleccionar un color desde el
        /// ColorDialog.
        /// </summary>
        ///
        /// <remarks>   Fidi, 11/16/2019. </remarks>
        ///
        /// <param name="sender">   Herramienta Editar. </param>
        /// <param name="e">        Evento <b>Click</b> de la herramienta Editar. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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
