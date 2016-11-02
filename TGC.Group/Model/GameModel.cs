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

namespace TGC.Group.Model
{
    /// <summary>
    ///     TP grupo MS
    /// </summary>
    public class GameModel : TgcExample
    {
        public TgcSkyBox SkyBox;
        private List<TgcScene> ScenesLst;
        public List<LuzFija> LucesLst;

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
            
            cargarSkyBox();
            CargarScenes();
            ToggleGodCamera();
            Niebla = new Niebla(this);
            Niebla.CargarCamara(Camara);


            
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
            AutoJugador = gb.CrearHummer(MapScene);
            Autos.Add(AutoJugador);
            gb.CrearLuces();
            //sprites en pantalla
            Velocimetro = new Velocimetro(this);
            
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
            this.MapScene = loader.loadSceneFromFile(MediaDir + "Bosque\\ciudad-mod4-TgcScene.xml");

            AsignarPlayersConMeshes(loader);


            //CrearBosque(loader);
            //this.scenesLst.Add();
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
                var cameraPosition = new Vector3(100,100,100);
                Camara = new GodCamera(cameraPosition, Input);
                //Camara.SetCamera(cameraPosition, new Vector3 (0,0,0));
                //Niebla.CargarCamara(Camara);
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
                
                
                //if (MenuBox != null)
                //{
                //    GodModeOn = false;
                //    Camara = AutoJugador.camaraSeguirEsteAuto(this);
                //}
                //else
                //{
                //    GodModeOn = true; //para q el auto no se mueva
                //    MenuBox = new MenuCaja(Input);
                //    Camara = MenuBox.CrearCamaraCaja();

                //}
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
            if (!FinishedLoading) return;
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();
            if(FinishedLoading) Niebla.Render();
            //Dibuja un texto por pantalla
            DrawText.drawText("F = bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("posicion camara: " + TgcParserUtils.printVector3(Camara.Position), 0, 30, Color.OrangeRed);
            if (GodModeOn)
            {
                DrawText.drawText("GodMode = ON", 0, 40, Color.OrangeRed);
            }
            
            else
            {
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
            }

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            //Box.Transform = Matrix.Scaling(Box.Scale) *
            //                Matrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) *
            //                Matrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            //Box.render();
            //CajaScene.render();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            // Mesh.UpdateMeshTransform();
            //Render del mesh
            // Mesh.render();

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                //Box.BoundingBox.render();
                // Mesh.BoundingBox.render();
                AutoJugador.Mesh.BoundingBox.render();
                MapScene.BoundingBox.render();
            }

            //rendereo el mapa.
            

            //render de cada auto.
            
            foreach (Auto a in Autos)
            {
                foreach (LuzFija luz in LucesLst)
                {
                    luz.setValues(a.Mesh, a.CamaraAuto.Position);
                }
                
                a.Render();
            }
            
            foreach(TgcMesh mesh in MapScene.Meshes)
            {
                var c = TgcCollisionUtils.classifyFrustumAABB(Frustum, mesh.BoundingBox);
                if(c != TgcCollisionUtils.FrustumResult.OUTSIDE)
                {
                    foreach (LuzFija luz in LucesLst)
                    {
                        luz.setValues(mesh, AutoJugador.CamaraAuto.Position);
                    }
                    
                    mesh.render();
                }
                
            }
            //MapScene.renderAll();
            //if(BosqueScene!=null)BosqueScene.renderAll();
            //skybox render
            SkyBox.render();


            //render de menubox solo cuando es necesario.
            if (GodModeOn == true && MenuBox != null)
                MenuBox.Render();

            //Dibujo los sprites2d en pantalla
            Velocimetro.Render();

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
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
    }
}