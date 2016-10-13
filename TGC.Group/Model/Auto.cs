﻿using Microsoft.DirectX;
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
//using TGC.Core.SceneLoader;

namespace TGC.GroupoMs.Model
{
    public class Auto : MovingObject
    {


        public bool collisionFound;
        //public bool collisionResult;

        public bool EsAutoJugador { get; set; }
        public float DireccionRuedas { get; set; } //negativo para la derecha, positivo para la izquierda


        public List<Arma> Armas { get; set; }
        public Arma ArmaSeleccionada { get; set; }


        public TgcMesh RuedaMainMesh { get; set; }
        //public int MyProperty { get; set; }

        public Ruedas RuedasTraseras { get; set; }
        public Ruedas RuedasDelanteras { get; set; }

        //----------- Ariel -------------------
        public bool huboMovimiento;
        public bool huboRotacion;
        public Vector3 pivoteMesh;
        public Vector3 newPosicion;
        public float angOrientacionMesh = 270 * (float)Math.PI / 180;
        public float velocidadInstantanea;
        public Matrix matrixRotacion;
        public float anguloFinal = 0;


        //--------------- salto -----------------
        public float impulsoSalto = 20f;
        public bool huboSalto = false;
        public float velocidadInstantaneaVertical = 0;
        public float VelocidadVertical = 0;
        public float longitudSalto = 10;
        public float posY = 5;
        public float time = 0;

        //----------- Boundig box --------------
        public TgcBoundingOrientedBox obb;
        TgcScene escenario;


        public Auto(string nombre, float vida, float avanceMaximo, float reversaMax,
                    float aceleracion, float desaceleracion, List<Arma> armas,
                    TgcMesh mesh, GameModel model, Ruedas ruedasAdelante, Ruedas ruedasAtras, TgcMesh ruedaMainMesh, TgcScene scena)
        {
            AvanceMax = avanceMaximo;
            ReversaMax = reversaMax;
            Desaceleracion = desaceleracion;
            InerciaNegativa = 1f;
            DireccionRuedas = 0f;
            Armas = armas;
            aceleracionVertical = 0f;

            Mesh = mesh;
            Aceleracion = aceleracion;

            GameModel = model;

            RuedasTraseras = ruedasAtras;
            RuedasDelanteras = ruedasAdelante;
            RuedaMainMesh = ruedaMainMesh;

            //------------Ariel---------------
            Mesh.AutoTransformEnable = false;

            /*
            hummerMesh.move(0, 5, 0); // la posicion inicial del hummer
            hummerMesh.AutoTransformEnable = true;
            hummerMesh.AutoUpdateBoundingBox = true;
            hummerMesh.Scale = new Vector3(0.7f, 0.7f, 0.7f);*/

            //----------- Colisiones ------------
            /*
            pMax = new Vector3(Mesh.BoundingBox.PMax.X * 0.7f, Mesh.BoundingBox.PMax.Y * 0.7f, Mesh.BoundingBox.PMax.Z * 0.7f);
            pMin = new Vector3(Mesh.BoundingBox.PMin.X * 0.7f, Mesh.BoundingBox.PMin.Y * 0.7f, Mesh.BoundingBox.PMin.Z * 0.7f);
            */


            obb = TgcBoundingOrientedBox.computeFromAABB(Mesh.BoundingBox);
            //obb.Extents.
            escenario = scena;

            Mesh.AutoUpdateBoundingBox = false;


        }
        /// <summary>
        /// hace que la camara siga a este auto.
        /// </summary>
        /// <returns></returns>

        public void Acelero()
        {
            velocidadInstantanea = Aceleracion / 3 * GameModel.ElapsedTime;

            Velocidad += velocidadInstantanea;
            if (AvanceMax < Velocidad)
                Velocidad = AvanceMax;
        }

        public void Freno()
        {
            velocidadInstantanea = -Desaceleracion / 2 * GameModel.ElapsedTime;
            Velocidad += velocidadInstantanea;
            if (ReversaMax < Velocidad)
                Velocidad = ReversaMax;
        }




        private void ProcesarInercia()
        {

            if (Velocidad < 0.01f || Velocidad < 0.01f)
            {
                Velocidad = 0f;
                return;
            }

            if (Velocidad > 0)
            {
                Velocidad -= InerciaNegativa * GameModel.ElapsedTime;
                return;
            }
            if (Velocidad < 0)
            {
                Velocidad += InerciaNegativa * GameModel.ElapsedTime;
                return;
            }

            //efecto gravedad? -> TODO

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
            collisionFound = huboSalto = false;

            PosicionAnterior = Mesh.Position;
            RotacionAnterior = Mesh.Rotation;

            //1 acelerar
            if (input.keyDown(Key.W))
            {
                Acelero();
                huboMovimiento = true;
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
                Doblar(1);
            }

            //4 derecha TODO //girar ruedas
            if (input.keyDown(Key.D))
            {
                Doblar(-1);
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

        private void Doblar(int lado)
        {
            if (Velocidad < 0) lado = lado * -1; //para que doble bien en reversa
            // doblar a la derecha es antihorario

            angOrientacionMesh += lado * 1f * GameModel.ElapsedTime;

            CamaraAuto.rotateY(-lado * 1f * GameModel.ElapsedTime);
            anguloFinal = anguloFinal - lado * 1f * GameModel.ElapsedTime;
            matrixRotacion = Matrix.RotationY(anguloFinal);
            //obb.rotate(new Vector3(3, -lado * 1f * GameModel.ElapsedTime, 3));

        }

        private void EnderezarRuedas()
        {
            if (DireccionRuedas < 0.01 || DireccionRuedas > 0.01)
                DireccionRuedas = 0f;
            if (DireccionRuedas > 0)
                DireccionRuedas -= 6f * GameModel.ElapsedTime;
            if (DireccionRuedas < 0)
                DireccionRuedas += 6f * GameModel.ElapsedTime;

        }

        /// <summary>
        /// Indica al mesh su nueva posicion y/o rotacion -- TODO
        /// </summary>
        /// 
        public float calcularDX()
        {
            //return velocidadInstantanea * (float)Math.Cos(angOrientacionMesh);
            return Velocidad * (float)Math.Cos(angOrientacionMesh);
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

        public float calcularDZ()
        {
            //return velocidadInstantanea * (float)Math.Sin(angOrientacionMesh);
            return Velocidad * (float)Math.Sin(angOrientacionMesh);
        }


        private void MoverMesh()
        {
            newPosicion = new Vector3(Mesh.Position.X + calcularDX(), calcularDY(), Mesh.Position.Z + calcularDZ());
            //4- las ruedas
            RuedasDelanteras.Update(Mesh.Position, Velocidad, DireccionRuedas);
            RuedasTraseras.Update(Mesh.Position, Velocidad, DireccionRuedas);
            //---- colisiones---
            //obb.Center = new Vector3(Mesh.Position.X, centro.Y / 2 + 5 + calcularDY(), Mesh.Position.Z);
            //Mesh.AutoUpdateBoundingBox = false;

            collisionFound = false;

            foreach (var mesh in escenario.Meshes)
            {
                //Los dos BoundingBox que vamos a testear
                var mainMeshBoundingBox = Mesh.BoundingBox;
                var sceneMeshBoundingBox = mesh.BoundingBox;
                //mesh.BoundingBox.render();
                //Ejecutar algoritmo de detección de colisiones
                var collisionResult = TgcCollisionUtils.classifyBoxBox(mainMeshBoundingBox, sceneMeshBoundingBox);

                //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                if (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera)
                {
                    collisionFound = true;
                    break;
                }
            }
            //obb.
            //obb.Center = 10f;

            if (collisionFound)
            {
                // Mesh.Position = originalPos;
                var m = matrixRotacion *
                Matrix.Translation(newPosicion);

                Mesh.Transform = m;

                Mesh.BoundingBox.transform(m);

                RuedasDelanteras.Update2(matrixRotacion, Matrix.Translation(newPosicion), newPosicion);
                RuedasTraseras.Update2(matrixRotacion, Matrix.Translation(newPosicion), newPosicion);
            }

            Mesh.Position = newPosicion;

            //camera update
            if (CamaraAuto != null) //actualizo la posicion de la camara respecto de la del mesh
                CamaraAuto.Target = Mesh.Position;
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
            obb.render();
            escenario.BoundingBox.render();
            foreach (var mesh in escenario.Meshes)
            {

                mesh.BoundingBox.render();
            }
        }
    }
}
