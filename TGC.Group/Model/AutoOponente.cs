using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;
using TGC.Group.Form;
using TGC.Group.Model;

namespace TGC.GroupoMs.Model
{

    class AutoOponente
    {
        private float desvioChoque;
        private Vector3 posTarget; //la pos a la que voy.
        private float velocidad;
        public TgcMesh Mesh;
        public TgcBoundingOrientedBox obb;
        private TgcMesh meshTarget;
        private GameModel gameModel;

        public float anguloFinal = 0; //indica el giro del auto en grados
        private float elapsedTime;
        private Matrix matrixRotacion;
        private Vector3 newPosicion;
        private Vector3 Scale;
        private float FixedWaitingTime;

        /// <summary>
        /// para crear un oponente. Tiempo de espera es para que no ande doblando todo el tiempo.
        /// </summary>
        /// <param name="gm"></param>
        /// <param name="AutoPlayer"></param>
        /// <param name="velocidadd"></param>
        /// <param name="desvioChoquee"></param>
        /// <param name="tiempoEspera"></param> 
        public AutoOponente(GameModel gm, Auto AutoPlayer, float velocidadd, float desvioChoquee, float tiempoEspera, Vector3 posicion)
        {
            posTarget = AutoPlayer.Mesh.Position;
            autoPlayer = AutoPlayer;
            gameModel = gm;
            elapsedTime = gm.ElapsedTime;
            meshTarget = AutoPlayer.Mesh;
            velocidad = velocidadd * 0.1f;
            desvioChoque = desvioChoquee;
            FixedWaitingTime = tiempoEspera; //el tiempo que espera hasta volver a girar.
            tiempoEspera = 0f; //este el contador del tiempo de espera.
            anguloFinal = 0f;

            //creo el mesh
            TgcSceneLoader Loader = new TgcSceneLoader();
            TgcScene sc = Loader.loadSceneFromFile(gameModel.MediaDir + "AerodeslizadorFuturista\\AerodeslizadorFuturista-TgcScene.xml");
            Mesh = sc.Meshes[0];
            float scale = 0.45f;

            Scale = new Vector3(scale, scale, scale);
            Mesh.AutoTransformEnable = false;
            Mesh.Transform = Matrix.Scaling(Scale) * Matrix.Translation(posicion);
            Mesh.Position = posicion;
            computarBoundingBox();
        }

        private float obbPosY = 0;
        private void computarBoundingBox()
        {
            obb = TgcBoundingOrientedBox.computeFromAABB(Mesh.BoundingBox);
            var yMin = Mesh.BoundingBox.PMin.Y;
            var yMax = Mesh.BoundingBox.PMax.Y;
            obbPosY = (yMax + yMin) / 2 + yMin;
            obb.Extents = new Vector3(obb.Extents.X, obb.Extents.Y, obb.Extents.Z * -1);
        }

        public void Update(Vector3 PosicionTarget)
        {

            var mapScene = gameModel.MapScene;
            bool collisionFound = false;

            Mover();


            //1 chocar contra target?

            if (TgcCollisionUtils.testObbObb(obb, autoPlayer.obb))
            {
                velocidad = 0f;
                Doblar(45f);
                DialogResult dialogResult = MessageBox.Show("Pediste!", "Juego Finalizdo", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                if (dialogResult == DialogResult.OK)
                {
                    Application.Run(new GameForm());
                    gameModel.Dispose();
                }
                obb.setRenderColor(Color.Blue);
                return;
            }
            //2 choca contra mapa?
            if (testChoqueContraMapa(mapScene, collisionFound))
            {
                velocidad = 2f;
                Doblar(45f);
                obb.setRenderColor(Color.Red);
                return;
            }
            obb.setRenderColor(Color.Yellow);
            //3 Dobla para apuntar al target?
            //ApuntarAlTarget(PosicionTarget);
        }


        float tiempoEspera;
        private Auto autoPlayer;

        #region rotacion
        private void ApuntarAlTarget(Vector3 PosicionTarget)
        {
            //1- bajo el contador que se fija cuando fue la ultima vez que apunte.
            tiempoEspera -= elapsedTime;
            // 2 - si todavia no puedo doblar, return!
            if (tiempoEspera > 0f) return;

            tiempoEspera = FixedWaitingTime; //3- Vuelvo la constante a valor inicial para que 
                                             //espere para volver a apuntar la proxima vez.

            //4 checkeo la cantidad de grados a girar y quedar apuntando hacia el target
            var posicionMesh = Mesh.Position;

            float a = Vector3.Dot(posicionMesh, PosicionTarget);
            double anguloDouble = Math.Acos(Convert.ToDouble(a));
            anguloFinal = Convert.ToSingle(anguloDouble);

            //ahora pongo al mesh mirando hacia el target.
            matrixRotacion = Matrix.RotationY(anguloFinal);
            obb.rotate(new Vector3(0, anguloFinal, 0));

        }


        private void Doblar(float v)
        {
            anguloFinal += v * elapsedTime;
            matrixRotacion = Matrix.RotationY(anguloFinal);
            obb.rotate(new Vector3(0, v * 1f * elapsedTime, 0));
        }

        private bool testChoqueContraMapa(TgcScene mapScene, bool collisionFound)
        {
            foreach (var sceneMesh in mapScene.Meshes)
            {
                var escenaAABB = sceneMesh.BoundingBox;
                var collisionResult = TgcCollisionUtils.testObbAABB(obb, escenaAABB);

                //2 -si lo hizo, salgo del foreach.
                if (collisionResult)
                {
                    collisionFound = true;
                    obb.setRenderColor(Color.Red);
                    break;
                }
            }
            return collisionFound;
        }
        #endregion




        #region traslacion
        private void Mover()
        {
            newPosicion = new Vector3(Mesh.Position.X + calcularDX(),
                                        5f,
                                        Mesh.Position.Z + calcularDZ());
            obb.Center = newPosicion + new Vector3(0, obbPosY, 0);
            Mesh.Position = newPosicion;
            var m = Matrix.Scaling(Scale) *
                    matrixRotacion *
                    Matrix.Translation(newPosicion);
            Mesh.Transform = m;
        }

        //public float angOrientacionMesh = 270 * (float)Math.PI / 180;
        public float calcularDX()
        {
            //return velocidadInstantanea * (float)Math.Cos(angOrientacionMesh);
            return velocidad * (float)Math.Cos(anguloFinal);
        }

        public float calcularDZ()
        {
            //return velocidadInstantanea * (float)Math.Sin(angOrientacionMesh);
            return velocidad * (float)Math.Sin(anguloFinal);
        }
        #endregion

        public void render()
        {
            Mesh.render();

            obb.render();
        }
    }
}
