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

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        private TgcSkyBox SkyBox;
        private List<TgcScene> ScenesLst;
        public TgcMesh PlayerMesh { get; set; }
        public bool GodModeOn { get; set; }

        public List<Auto> Autos { get; set; } //aca esta el auto del jugador y los autos de la AI
        public Auto AutoJugador { get; set; }
        public TgcScene MapScene { get; set; }
        
        
        //Caja que se muestra en el ejemplo.
        private TgcBox Box { get; set; }
        //Mesh de TgcLogo.
        private TgcMesh Mesh { get; set; }
        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

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
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var texture = TgcTexture.createTexture(pathTexturaCaja);

            //Creamos una caja 3D ubicada de dimensiones (5, 10, 5) y la textura como color.
            var size = new Vector3(5, 10, 5);
            //Construimos una caja según los parámetros, por defecto la misma se crea con centro en el origen y se recomienda así para facilitar las transformaciones.
            Box = TgcBox.fromSize(size, texture);
            //Posición donde quiero que este la caja, es común que se utilicen estructuras internas para las transformaciones.
            //Entonces actualizamos la posición lógica, luego podemos utilizar esto en render para posicionar donde corresponda con transformaciones.
            Box.Position = new Vector3(-25, 0, 0);

            //Cargo el unico mesh que tiene la escena.
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + "LogoTGC-TgcScene.xml").Meshes[0];
            //Defino una escala en el modelo logico del mesh que es muy grande.
            Mesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            
            //Camara.
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            //Camara.SetCamera(cameraPosition, lookAt);
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.


            
            GodModeOn = false; //cuando llamo a toggle lo pone en false
            ToggleGodCamera();
            cargarSkyBox();
            CargarScenes();
        }

        private void AsignarPlayersConMeshes(TgcSceneLoader loader)
        {
            Autos = new List<Auto>();

            TgcScene hummerScene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml");
            TgcMesh hummerMesh = hummerScene.Meshes[0];
            PlayerMesh = hummerMesh;
            PlayerMesh.move(0, 5, 0); //muevo el mesh un poco para arriba

            AutoJugador = new Auto("hummer", 100, 100, 50, 10, 7, new List<Arma>(), PlayerMesh);
            Autos.Add(this.AutoJugador);
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
        private void CargarScenes() {
            TgcSceneLoader loader = new TgcSceneLoader();
            this.ScenesLst = new List<TgcScene>();
            this.MapScene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Scenes\\Ciudad\\Ciudad-TgcScene.xml");

            AsignarPlayersConMeshes(loader);
            //this.scenesLst.Add();

        }

        /// <summary>
        /// Activa/desactiva la camara de modo dios, cuando la activo el juego sigue funcionando.
        /// </summary>
        /// <param name="ubicacion"></param>
        /// <param name="lookAt"></param>
        private void ToggleGodCamera() {
            if (GodModeOn)
            {
                GodModeOn = false;
                Camara = this.AutoJugador.camaraSeguirEsteAuto();
                // apagar godmode y  volver al auto
            }
            else {
                GodModeOn = true;
                var cameraPosition = new Vector3(23, 39, 113);
                Camara = new TgcFpsCamera(cameraPosition, Input);
                //Camara.SetCamera(cameraPosition, Vector3.Empty); solo es para camara estatica
                
            }
            //this.Render();
            
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();
            this.SkyBoxUpdate();

            //le mando el input al auto del jugador parar que haga lo que tenga que hacer.
            AutoJugador.Update(Input);
           
            //Capturar Input teclado
            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            //Capturar Input Mouse
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
               
            }

            //godMode Toggle
            if (Input.keyPressed(Key.O))
            {
                this.ToggleGodCamera();
            }

            //PlayerMesh.render();
            //rendereo todos los autos.
            
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
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
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
            }

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            Box.Transform = Matrix.Scaling(Box.Scale) *
                            Matrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) *
                            Matrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            Box.render();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Mesh.UpdateMeshTransform();
            //Render del mesh
            Mesh.render();

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                Box.BoundingBox.render();
                Mesh.BoundingBox.render();
                AutoJugador.Mesh.BoundingBox.render();
            }


            //rendereo el mapa.
            MapScene.renderAll();

            //rendereo el mesh del jugador
            //PlayerMesh.render();


            //render de cada auto.
            foreach (Auto a in Autos)
            {
                a.Render();
            }

            //skybox render
            SkyBox.render();



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
            Box.dispose();
            //Dispose del mesh.
            Mesh.dispose();
            SkyBox.dispose();
        }
    }
}