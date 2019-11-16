# Proyecto Paint .Net por Fidel Lorenzo García
| Fecha| Modificación |
|-------|-------------|
|03/10/2019|- Creación WPF.|
|04/10/2019|- Creación de la mayoría de botones del ToolBar, y la carpeta con las imágenes en Resources.|
|07/10/2019|- Implementación del resto de herramientas y menús básicos, a falta de detalles.|
|14/10/2019|- Implementación de los botones hacer/deshacer y la modificación de algunos iconos.|
||- Añadidos 2 Windwos forms para la selección del tamaño y tipos de pinceles.|
||- Añadidas también algunas funciones como open y savedialogs.|
|21/10/2019|- Menú Herramientas añadadido, para modificar el tamaño del canvas.|
||- Cambiado el Canvas por un InkCanvas sobre el cual se pintará.|
||- Añadida la opción de Imprimir en el menú Archivo.|
||- Añadidas las imágenes de los submenús y sus respectivos atajos de teclado (Crtl + N) etc...|
||- Trabajando en las funciones para pintar, de momento solo funciona el lápiz.|
||- Añadidos algunos Tooltips, estarán terminados para la próxima revisión.|
|28/11/2019|- Elimnada la opción de "Cubo pintura" por incompatiblidad con InkCanvas (Se intentará agregar la forma "Polígono" para compensar). |
||- Menú "Guardar" funcionando (Comprobar si se realizaron cambios).|
||- Menú "Abrir" funcionando (Comprobar si se realizaron cambios).|
||- Herramienta "Lápiz" funcionando.|
||- Herramienta "Borrar" funcionando.|
||- Herramienta "Texto" funcionando (Límite de 1 TextBox,faltan más comprobaciones).|
||- Herramienta "Seleccionar" funcionando.|
||- Herramienta "Línea" funcionando, falta pulirla.|
||- Los botones de los colores tanto como el ColorDialog ya se aplican a las herramientas de dibujar.|
||- Creado un nuevo componente para la validación de TextBox (Usados en el formulario para cambiar el tamaño del InkCanvas, también funcionando).|
|04/11/2019|- Herramienta "Cortar" funcionando.|
||- Herramienta "Copiar" funcionando.|
||- Herramienta "Pegar" funcionando.|
||- Menú "Imprimir" funcionando.|
||- Cambiado el botón de "Tamaño" por un slider, para elegir el tamaño del pincel y funcionando.|
||- Añadido otro slider para el tamaño del texto, funcionando.|
||- Herramienta "Elipse" funcionando.|
||- Herramienta "Rectángulo" funcionando.|
||- Añadida la herramienta pra borrar los trazos enteros de un click.|
|11/11/2019|- Herramienta "Triángulo" funcionando.|
||- Eliminada la opción de "Pinceles", no encontre manera ninguna (seguiré buscando información).|
||- Añadir al título el tamaño del lienzo actual, siempre que cambia de tamaño se actualiza.|
|Problemas encontrados|- Deshacer y rehacer solo guardan un cambio, si se prueba a deshacer 2 seguidos falla.|
||- Cortar, copiar, pegar, borrar sólo funcionan con el láapiz y triángulo, ya que son los unicos que se añaden a la colección Strokes, no he sido capaz de que funcionen con la línea, elipse y rectángulo, que los he tratado como shapes, y van a InkCanvas.Children|
||- Cuando usas los menús nuevo, abrir y guardar, no se como detectar el cambio del InkCanvas para poder preguntar al usuario si quiere guardar el progreso actual.|
||- Al guardar en formato imagen, aparece una barra de color negro en la parte superior.|
|16/11/2019|- Añadaido el submenú "Importar"|
||- Se ha modularizado el código en su medida|
||- Documentación realizada|
|Problemas solventados|- Deshacer y rehacer ya no falla, pero estan limitados a 1 deshacer y 1 rehacer, siempre el último Stroke|
||- Al guardar en formato imagen, ya no guarda el area negra, guarda la imagen tal y como debería|
||- Cuando usas los menús nuevo, abrir, ya piden al usuario si quieren guardar el progreso sin guardar|
||- Arreglado un problema con el formulario secundario, donde cambiamos el tamaño del InkCanvas. No estaban bien escritas las expresiones del Regex.IsMatch|



