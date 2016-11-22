using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.GroupoMs;
using TGC.GroupoMs.Model;
using System;
using TGC.Core.Terrain;
using TGC.GroupoMs.Camaras;
using TGC.Core;
using TGC.GroupoMs.Model.efectos;
using TGC.GrupoMs.Model.Elementos;
using TGC.GroupoMs.Model.ScreenOverlay;
using TGC.Core.Collision;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model
{
    /// <summary>
    ///     TP grupo MS
    /// </summary>
    public class GameModel : TgcExample
    {
        public TgcSkyBox SkyBox;
        //private List<TgcScene> ScenesLst;
        public List<LuzFija> LucesLst;
        public PointLight2 pointLuz;
        private AutoOponente autoOponente;
        private bool showMenu;

        public bool FinishedLoading { get; set; }

        public Niebla Niebla { get; set; }

        public TgcMesh PlayerMesh { get; set; }
        public bool GodModeOn { get; set; }

        public List<Auto> Autos { get; set; } //aca esta el auto del jugador y los autos de la AI
        public Auto AutoJugador { get; set; }
        public TgcScene MapScene { get; set; }
        //public TgcBox CajaScene { get; set; }
        public MenuCaja MenuBox { get; set; }

        private bool BoundingBox { get; set; }
        //public TgcScene BosqueScene { get; private set; }
        public Velocimetro Velocimetro { get; set; }

        //public ShadowMap shadowMap;


        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            FinishedLoading = false;

            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            GodModeOn = true; //cuando llamo a toggle lo pone en false


            CargarScenes();
            ToggleGodCamera();
            pointLuz = new PointLight2(this, new Vector3(100f, 100f, 100f));
            Vector3 posicion = new Vector3(100f, 5f, -500f);
            autoOponente = new AutoOponente(this, AutoJugador, 2f, 45f, 6f, posicion);

        }

        /// <summary>
        /// Usa el gameBuilder para crear los distintos elementos.
        /// </summary>
        /// <param name="loader"></param>
        private void AsignarPlayersConMeshes(TgcSceneLoader loader)
        {
            GameBuilder gb = new GameBuilder(MediaDir, this, loader);
            Autos = new List<Auto>();
            //BosqueScene = gb.CrearBosque();
            Velocimetro = new Velocimetro(this);
            AutoJugador = gb.CrearHummer(MapScene, Velocimetro);
            Autos.Add(AutoJugador);
            gb.CrearLuces();
            //sprites en pantalla


        }


        /// <summary>
        /// init del skybox, se lo llama en el metodo init de GameModel.cs
        /// </summary>
        private void cargarSkyBox()
        {
            //Crear SkyBox
            SkyBox = new TgcSkyBox();
            SkyBox.Center = new Vector3(0, 0, 0);
            SkyBox.Size = new Vector3(10000, 10000, 10000);

            //Configurar color
            //skyBox.Color = Color.OrangeRed;

            var texturesPath = MediaDir + "Texturas\\Quake\\SkyBox1\\";

            //Configurar las texturas para cada una de las 6 caras
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "phobos_up.jpg");
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "phobos_dn.jpg");
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "phobos_lf.jpg");
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "phobos_rt.jpg");

            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "phobos_bk.jpg");
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "phobos_ft.jpg");
            SkyBox.SkyEpsilon = 25f;
            //Inicializa todos los valores para crear el SkyBox
            SkyBox.Init();

            //Modifier para mover el skybox con la posicion de la caja con traslaciones.
            // Modifiers.addBoolean("moveWhitCamera", "Move Whit Camera", false);
        }

        /// <summary>
        /// Cargar scenes, se llama en init.
        /// </summary>
        private void CargarScenes()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            this.MapScene = loader.loadSceneFromFile(MediaDir + "Bosque\\ciudad-mod5-TgcScene.xml");
            cargarSkyBox();
            Niebla = new Niebla(this);

            AsignarPlayersConMeshes(loader);

        }

        /// <summary>
        /// Activa/desactiva la camara de modo dios, cuando la activo el juego sigue funcionando.
        /// </summary>
        private void ToggleGodCamera()
        {

            if (GodModeOn)
            {
                GodModeOn = false;
                Camara = AutoJugador.camaraSeguirEsteAuto(this);

                // apagar godmode y  volver al auto
            }
            else
            {
                GodModeOn = true;
                var cameraPosition = new Vector3(100, 100, 100);
                Camara = new GodCamera(cameraPosition, Input);
            }
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();
            SkyBoxUpdate();
            if (FinishedLoading)
                Niebla.Update(Camara);

            //le mando el input al auto del jugador parar que haga lo que tenga que hacer.
            if (GodModeOn == false)
                AutoJugador.Update(Input);
            if (GodModeOn)
            {
                if (Input.keyPressed(Key.Z))
                    AutoJugador.Acelero(1.4f);
            }

            //Capturar Input teclado
            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }
            if (Input.keyPressed(Key.L))
            {
                AutoJugador.RenderLuces = !AutoJugador.RenderLuces;
            }

            // Ir al menu?
            if (Input.keyPressed(Key.P))
            {
                //Niebla.CargarCamara(Camara);

            }

            //Capturar Input Mouse
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {

            }

            //godMode Toggle
            if (Input.keyPressed(Key.O))
            {
                ToggleGodCamera();
            }
            autoOponente.Update();
            //LucesLst[0].Update(AutoJugador.Mesh.Position, AutoJugador.Mesh.Rotation);
        }

        private void SkyBoxUpdate()
        {
            //Se cambia el valor por defecto del farplane para evitar cliping de farplane.
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * 2f);

            //Se actualiza la posicion del skybox.
            //if ((bool)Modifiers.getValue("moveWhitCamera")) tengo que comentar esto?
            SkyBox.Center = Camara.Position;
        }

        /// <summary>
        ///     main render method
        /// </summary>
        public override void Render()
        {
            BorrarPantalla();
            if (showMenu)
            {
                //TODO -> mostrar menu inicial
                return;
            }
            if (!FinishedLoading) return;
            
            preRenderPointLight(); //render 1
            //preRenderNiebla(); //render2, rompe con la luz por eso esta comentado

            IniciarScene(); //empiezo main escena


            //PreRenderPersonalizado(); //para el shadowMapFIX

            if (GodModeOn) DibujarDebug();
            var posCamara = AutoJugador.CamaraAuto.Position;
            foreach (Auto a in Autos)
            {
                foreach (LuzFija luz in LucesLst)
                {
                    luz.setValues(a.Mesh, posCamara);
                }

                a.Render();
            }

            foreach (TgcMesh mesh in MapScene.Meshes)
            {
                //rendereo solo lo que esta dentro del frustrum
                var c = TgcCollisionUtils.classifyFrustumAABB(Frustum, mesh.BoundingBox);
                if (c != TgcCollisionUtils.FrustumResult.OUTSIDE)
                {
                    mesh.render();
                }
            }
            DrawText.drawText(AutoJugador.Velocidad.ToString(), 20, 50, Color.Orange);
            //skybox render
            

            //render de menubox solo cuando es necesario.
            if (GodModeOn == true && MenuBox != null)
                MenuBox.Render();
            //Dibujo los sprites2d en pantalla
            Velocimetro.Render();

            autoOponente.render();
            SkyBox.render();
            RenderAxis();
            RenderFPS();
            TerminarScene(); //termino main scene
            ImprimirPantalla();
        }

        private void preRenderNiebla()
        {
            IniciarScene(); //empiezo escena
            Niebla.Render();
            TerminarScene(); //termino escena
        }

        private void preRenderPointLight()
        {
            //una escena para cada luz
            IniciarScene();
            pointLuz.render(MapScene.Meshes,
                           AutoJugador.CamaraAuto.Position,
                           AutoJugador.TodosLosMeshes);
            TerminarScene();
        }


        #region inicio,fin e impresion de scenes

        /// <summary>
        /// inicia un scene
        /// </summary>
        private void IniciarScene()
        {
            D3DDevice.Instance.Device.BeginScene();
        }
        /// <summary>
        /// termina un scene
        /// </summary>
        private void TerminarScene()
        {
            D3DDevice.Instance.Device.EndScene(); //termino la escena
        }

        /// <summary>
        /// Se llama al inicio de todo para borrar lo que quedó del render anterior.
        /// </summary>
        private void BorrarPantalla()
        {
            ClearTextures();
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
        }
        /// <summary>
        /// se llama al final de todo para imprimir en pantalla todos los scenes, falla si falta algun TerminarScene()
        /// </summary>
        private void ImprimirPantalla()
        {
            D3DDevice.Instance.Device.Present();
        }
        #endregion 

        

        private void DibujarDebug()
        {

            DrawText.drawText("GodMode = ON", 600, 40, Color.OrangeRed);
            DrawText.drawText("F = bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("posicion camara: " + TgcParserUtils.printVector3(Camara.Position), 0, 30, Color.OrangeRed);
            DrawText.drawText("GodMode = OFF", 0, 40, Color.White);
            DrawText.drawText("v=", 0, 50, Color.Orange);
            DrawText.drawText(AutoJugador.Velocidad.ToString(), 20, 50, Color.Orange);
            //DrawText.drawText("ruedas=", 0, 60, Color.Green);
            //DrawText.drawText(AutoJugador.DireccionRuedas.ToString(), 50, 60, Color.Green);

            DrawText.drawText("angOrientacionMesh=", 0, 70, Color.White);
            DrawText.drawText((AutoJugador.angOrientacionMesh * 180 / (float)Math.PI).ToString(), 160, 70, Color.White);

            DrawText.drawText("MeshPosition=", 0, 80, Color.White);
            DrawText.drawText(AutoJugador.Mesh.Position.ToString(), 100, 80, Color.White);



            DrawText.drawText("scale mesh = ", 0, 160, Color.White);
            DrawText.drawText(AutoJugador.Mesh.Scale.ToString(), 100, 160, Color.White);

            DrawText.drawText("obb center = ", 0, 220, Color.White);
            DrawText.drawText(AutoJugador.obb.Center.ToString(), 100, 220, Color.White);

            DrawText.drawText("bounding position = ", 0, 280, Color.White);
            DrawText.drawText(AutoJugador.Mesh.BoundingBox.ToString(), 150, 280, Color.White);

            //DrawText.drawText("collisionFound = ", 0, 340, Color.White);
            //DrawText.drawText(AutoJugador.collisionFound.ToString(), 150, 340, Color.White);

            DrawText.drawText("direccionRuedas = ", 0, 400, Color.White);
            DrawText.drawText(AutoJugador.DireccionRuedas.ToString(), 150, 400, Color.White);
            if (BoundingBox)
            {
                //Box.BoundingBox.render();
                // Mesh.BoundingBox.render();
                AutoJugador.Mesh.BoundingBox.render();
                MapScene.BoundingBox.render();
            }
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            //Dispose de la caja.
            // Box.dispose();
            //Dispose del mesh.
            // Mesh.dispose();
            SkyBox.dispose();
        }

        /// <summary>
        /// renderea un mesh
        /// </summary>
        /// <param name="T"></param>
        /// <param name="shadow"></param>
        public void RenderMesh(TgcMesh T, bool shadow)
        {
            if (shadow)
            {
                T.Technique = "RenderShadow";
            }
            else
            {
                T.Technique = "RenderScene";
            }

            T.render();
        }
    }
}