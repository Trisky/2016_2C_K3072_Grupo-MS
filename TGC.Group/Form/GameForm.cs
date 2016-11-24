using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Textures;
using TGC.Group.Model;
using TGC.GroupoMs;

namespace TGC.Group.Form
{
    /// <summary>
    ///     GameForm es el formulario de entrada, el mismo invocara a nuestro modelo  que extiende TgcExample, e inicia el
    ///     render loop.
    /// </summary>
    public partial class GameForm : System.Windows.Forms.Form
    {
        private bool hayQueReiniciar = false;

        /// <summary>
        ///     Constructor de la ventana.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
            lblGanaPierde.Visible = false;

        }

        /// <summary>
        ///     Ejemplo del juego a correr
        /// </summary>
        private TgcExample Modelo { get; set; }

        /// <summary>
        ///     Obtener o parar el estado del RenderLoop.
        /// </summary>
        private bool ApplicationRunning { get; set; }

        /// <summary>
        ///     Permite manejar el sonido.
        /// </summary>
        private TgcDirectSound DirectSound { get; set; }

        /// <summary>
        ///     Permite manejar los inputs de la computadora.
        /// </summary>
        private TgcD3dInput Input { get; set; }

        private void GameForm_Load(object sender, EventArgs e)
        {
            
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ApplicationRunning)
            {
                ShutDown();
            }
        }

        /// <summary>
        ///     Inicio todos los objetos necesarios para cargar el ejemplo y directx.
        /// </summary>
        public void InitGraphics()
        {
            //Se inicio la aplicación
            ApplicationRunning = true;

            //Inicio Device
            D3DDevice.Instance.InitializeD3DDevice(panel3D);

            //Inicio inputs
            Input = new TgcD3dInput();
            Input.Initialize(this, panel3D);

            //Inicio sonido
            DirectSound = new TgcDirectSound();
            DirectSound.InitializeD3DDevice(panel3D);

            //Directorio actual de ejecución
            var currentDirectory = Environment.CurrentDirectory + "\\";

            //Cargar shaders del framework
            TgcShaders.Instance.loadCommonShaders(currentDirectory + Game.Default.ShadersDirectory);
            float tiempo = Convert.ToSingle(numericUpDown1.Value);
            //Juego a ejecutar, si quisiéramos tener diferentes modelos aquí podemos cambiar la instancia e invocar a otra clase.
            Modelo = new GameModel(currentDirectory + Game.Default.MediaDirectory,
                currentDirectory + Game.Default.ShadersDirectory,tiempo,this);

            //Cargar juego.
            ExecuteModel();
        }

        /// <summary>
        ///     Comienzo el loop del juego.
        /// </summary>
        public void InitRenderLoop()
        {
            while (ApplicationRunning)
            {
                //Renderizo si es que hay un ejemplo activo.
                if (Modelo != null)
                {
                    //Solo renderizamos si la aplicacion tiene foco, para no consumir recursos innecesarios.
                    if (ApplicationActive())
                    {
                        Modelo.Update();
                        Modelo.Render();
                    }
                    else
                    {
                        //Si no tenemos el foco, dormir cada tanto para no consumir gran cantidad de CPU.
                        Thread.Sleep(100);
                    }
                }
                // Process application messages.
                Application.DoEvents();
            }
        }

        /// <summary>
        ///     Indica si la aplicacion esta activa.
        ///     Busca si la ventana principal tiene foco o si alguna de sus hijas tiene.
        /// </summary>
        public bool ApplicationActive()
        {
            if (ContainsFocus)
            {
                return true;
            }

            foreach (var form in OwnedForms)
            {
                if (form.ContainsFocus)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Arranca a ejecutar un ejemplo.
        ///     Para el ejemplo anterior, si hay alguno.
        /// </summary>
        public void ExecuteModel()
        {
            //Ejecutar Init
            try
            {
                Modelo.ResetDefaultConfig();
                Modelo.DirectSound = DirectSound;
                Modelo.Input = Input;
                Modelo.Init();
                panel3D.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error en Init() del juego", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Deja de ejecutar el ejemplo actual
        /// </summary>
        public void StopCurrentExample()
        {
            if (Modelo != null)
            {
                Modelo.Dispose();
                Modelo = null;
            }
        }

        /// <summary>
        ///     Finalizar aplicacion
        /// </summary>
        public void ShutDown()
        {
            ApplicationRunning = false;

            StopCurrentExample();

            //Liberar Device al finalizar la aplicacion
            D3DDevice.Instance.Dispose();
            TexturesPool.Instance.clearAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel3D.Visible = true;
            if (hayQueReiniciar)
            {
                ShutDown();
                InitGraphics();
                hayQueReiniciar = false;
            }
            toggleVerBotones();
            if(Modelo == null)
            InitGraphics();

            //Titulo de la ventana principal.
            Text = Modelo.Name + " - " + Modelo.Description;
            lblGanaPierde.Visible = false;
            //Focus panel3D.
            panel3D.Focus();

            //Inicio el ciclo de Render.

            InitRenderLoop();
            
        }

        private void toggleVerBotones()
        {
            button1.Visible = !button1.Visible;
            pictureBox1.Visible= !pictureBox1.Visible;
            labelElijeTIempo.Visible = !labelElijeTIempo.Visible;
            numericUpDown1.Visible = !numericUpDown1.Visible;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        public void TerminarJuego(bool ganaste)
        {
            ApplicationRunning = false;
            lblGanaPierde.Visible = true;
            //panel3D.Visible = false;
            if (ganaste)
                lblGanaPierde.Text = "Ganaste!";
            else
                lblGanaPierde.Text = "Perdiste!";
            //toggleVerBotones();
            lblGanaPierde.Visible = true;
            hayQueReiniciar = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}