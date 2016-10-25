using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.GrupoMs.Camara;
using Microsoft.DirectX.DirectInput;
using TGC.Group.Model;
using TGC.Core.Utils;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;
using TGC.Core.Collision;
using System.Drawing;
//using TGC.Core.SceneLoader;

namespace TGC.GroupoMs.Model
{
    public class Auto : MovingObject
    {



        public bool pintarObb;
        //public bool collisionResult;
        public float obbPosY = 0;
        public bool EsAutoJugador { get; set; }
        public float DireccionRuedas { get; set; } //negativo para la derecha, positivo para la izquierda

        public List<Arma> Armas { get; set; }
        public Arma ArmaSeleccionada { get; set; }


        public TgcMesh RuedaMainMesh { get; set; }
        //public int MyProperty { get; set; }

        public Ruedas RuedasTraseras { get; set; }
        public Ruedas RuedasDelanteras { get; set; }



        //----------- Boundig box --------------
        public TgcBoundingOrientedBox obb;
        public TgcScene escenario;


        public Auto(string nombre, float vida, float avanceMaximo, float reversaMax,
                    float aceleracion, float desaceleracion,
                    TgcMesh mesh, GameModel model, Ruedas ruedasAdelante, Ruedas ruedasAtras, TgcMesh ruedaMainMesh)
        {
            AvanceMax = avanceMaximo;
            ReversaMax = -reversaMax;
            Desaceleracion = desaceleracion;
            InerciaNegativa = 1f;
            DireccionRuedas = 0f;
            Armas = new List <Arma>();
            aceleracionVertical = 0f;

            Mesh = mesh;
            Aceleracion = aceleracion;

            GameModel = model;

            RuedasTraseras = ruedasAtras;
            RuedasDelanteras = ruedasAdelante;
            RuedaMainMesh = ruedaMainMesh;

            //------------Ariel---------------
            Mesh.AutoTransformEnable = false;
            Mesh.AutoUpdateBoundingBox = false;

            obb = TgcBoundingOrientedBox.computeFromAABB(Mesh.BoundingBox);
            var yMin = Mesh.BoundingBox.PMin.Y;
            var yMax = Mesh.BoundingBox.PMax.Y;
            obbPosY = (yMax + yMin) / 2 + yMin;

            escenario = model.MapScene;
        }

        [Obsolete]
        public void recibirDanio(int cant)
        {
            if (this.VidaMaxima < 0)
            {
                return;
            }
            this.Vida = -cant;
            if (this.Vida <= 0)
            {
                //TODO this.morir; //explota?
            }
            if (this.EsAutoJugador)
            {
                /*TODO mostrar en pantalla danio recibido,con 200ms seria suficiente, 
                podria ir sumandose para saber cuanto le sacamos en esa "rafaga" de disparos.*/
            }
            else
            {
                //TODO mostrar en pantalla danio dado
            }
        }

        [Obsolete]
        public void Chocar(Vector3 angulo)
        {
            Velocidad = -Velocidad * 0.2f;
            PosicionRollback();
        }

        /// <summary>
        /// Llamado una vez por frame
        /// </summary>
        /// <param name="input"></param>
        public void Update(TgcD3dInput input)
        {
            huboMovimiento = huboRotacion = false;
            bool collisionFound = huboSalto = false;
            //pintarObb = false;

            PosicionAnterior = Mesh.Position;
            RotacionAnterior = Mesh.Rotation;

            //1 acelerar
            if (input.keyDown(Key.W))
            {
                Acelero(1);
                huboMovimiento = true;

                if (input.keyDown(Key.LeftShift))
                    Acelero(0.5f); //turbo!
            }

            //2 frenar, reversa
            if (input.keyDown(Key.S))
            {
                Freno();
                huboMovimiento = true;
            }

            //3 izquierda TODO//girar ruedas
            if (input.keyDown(Key.A))
            {
                DoblarRuedas(1);
            }

            //4 derecha TODO //girar ruedas
            if (input.keyDown(Key.D))
            {
                DoblarRuedas(-1);
            }

            if (input.keyDown(Key.F))
            {
                pintarObb = !pintarObb;
            }

            //5 disparar TODO -- 
            if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //disparar 
                //this.ArmaSeleccionada.ammo--;
                //TODO falta evento de fin de ammo.
            }

            //6 cambiar de arma TODO
            if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {

            }

            //7 freno de mano? maybe TODO
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.Space))
            {

            }
            //8 saltar
            if (input.keyDown(Key.Space) && (Mesh.Position.Y == 5))
            {
                huboSalto = true;
            }

            else
            {
                //aceleracionVertical = -1; descomentar cuando el auto choque con el suelo.
            }

            //RuedasDelanteras.Update(Mesh.Position,Velocidad,DireccionRuedas);
            //RuedasTraseras.Update(Mesh.Position, Velocidad, DireccionRuedas);
            ProcesarInercia();
            MoverMesh();

        }

        private void DoblarRuedas(int lado)
        {  
            DireccionRuedas = +lado * 0.4f;
            Doblar();
        }
        private void Doblar()
        {
            float lado = DireccionRuedas;
            lado = lado * Velocidad;
            angOrientacionMesh += lado * 1f * GameModel.ElapsedTime;
            //ahora ya tengo para el lado en el que voy a girar y la intensidad del giro, entonces giro el auto.

            CamaraAuto.rotateY(-lado * 1f * GameModel.ElapsedTime);
            anguloFinal = anguloFinal - lado * 1f * GameModel.ElapsedTime;
            matrixRotacion = Matrix.RotationY(anguloFinal);
            obb.rotate(new Vector3(0, -lado * 1f * GameModel.ElapsedTime, 0));
        }

        private void EnderezarRuedas()
        {
            if (DireccionRuedas < 0.01 || DireccionRuedas > 0.01)
                DireccionRuedas = 0f;
            if (DireccionRuedas > 0)
                DireccionRuedas -= 2f * GameModel.ElapsedTime;
            if (DireccionRuedas < 0)
                DireccionRuedas += 2f * GameModel.ElapsedTime;
        }

        public float calcularDY()
        {
            if (huboSalto || Mesh.Position.Y > 5)
            {
                //velocidadInstantaneaVertical = 5 + 100f * GameModel.ElapsedTime - 400 * GameModel.ElapsedTime * GameModel.ElapsedTime;
                //return Mesh.Position.Y + velocidadInstantaneaVertical;
                time += GameModel.ElapsedTime;
                return posY += 30f * time - 45f * time * time;
            }
            else
            {
                time = 0;
                posY = 5;
                return Mesh.Position.Y - Mesh.Position.Y + 5;
            }
        }

        

        private void MoverMesh()
        {
            newPosicion = new Vector3(Mesh.Position.X + calcularDX(), calcularDY(), Mesh.Position.Z + calcularDZ());
            
            //4- las ruedas
            RuedasDelanteras.Update(Mesh.Position, Velocidad, DireccionRuedas);
            RuedasTraseras.Update(Mesh.Position, Velocidad, DireccionRuedas);

            //??
            obb.Center = new Vector3(Mesh.Position.X + calcularDX(), obbPosY + 0 + calcularDY(), Mesh.Position.Z + calcularDZ());

            //5 ---- colisiones---
            ProcesarColisiones();
           
            //6 - me muevo
            var m = matrixRotacion *
                Matrix.Translation(newPosicion);

            Mesh.Transform = m; //Mesh.BoundingBox.transform(m);

            RuedasDelanteras.Update3(Mesh.Position, matrixRotacion, angOrientacionMesh, Velocidad);
            RuedasTraseras.Update3(Mesh.Position, matrixRotacion, angOrientacionMesh, Velocidad);

            Mesh.Position = newPosicion;

            //camera update
            if (CamaraAuto != null) //actualizo la posicion de la camara respecto de la del mesh
                CamaraAuto.Target = Mesh.Position;
        }

        private void ProcesarColisiones()
        {
            bool collisionFound = false;
            var autoBB = Mesh.BoundingBox;
            foreach (var sceneMesh in escenario.Meshes)
            {
                var sceneMeshBoundingBox = sceneMesh.BoundingBox;
                //var collisionResult = TgcCollisionUtils.testObbAABB(obb, sceneMeshBoundingBox);

                //1- me fijo si el bb de este mesh choca con el del auto
                var collisionResult = TgcCollisionUtils.classifyBoxBox(autoBB, sceneMeshBoundingBox);

                //2 -si lo hizo, salgo del foreach.
                if (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera)
                {
                    collisionFound = true;
                    break;
                }
            }
            //3 - si chocó, pongo el bounding box en rojo (apretar F para ver el bb).
            if (collisionFound)
            {
                obb.setRenderColor(Color.Red);
                //PosicionRollback();
            }
            else
            {
                obb.setRenderColor(Color.Yellow);
            }
        }

        /// <summary>
        /// rendereo.
        /// </summary>
        public void Render()
        {
            Mesh.render();
            RuedasDelanteras.Render();
            RuedasTraseras.Render();
            RuedaMainMesh.render();
           
            //escenario.BoundingBox.render();

            if(pintarObb)
                obb.render();

            foreach (var mesh in escenario.Meshes)
            {
                mesh.BoundingBox.render();
            }
        }
    }
}
