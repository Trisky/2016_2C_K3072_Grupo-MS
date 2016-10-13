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

namespace TGC.GroupoMs.Model
{
    public class Auto_miranda : MovingObject
    {
        //------------
       
      
        public float anguloMeshEnEjecucion;
        public Vector3 vectorOrientacion;
        public float anguloOrientacionMesh = 0 * (float)Math.PI /180;
        public float anguloOrientacion;
        public float aux;
        public float anguloRotacion;
       
        public float anguloTotal = 0f;
        public float velocidadInstantanea = 0f;
        //------------ TRANSFORMACIONES ----------------------
        public bool huboMovimiento;
        public float orientacion { get; set; }


        //public Matrix matrizTraslacion;
        //public float rotAngle;
        public float rotate;
        public Matrix matrizRot;
        public Matrix matrizTraslacion;
        public Vector3 newPosition;

        private float aceleracionVertical;
        public bool huboSalto;
        public float rotAngle;
        public bool huboRotacionRuedas;

        public float Vida { get; set; }
        public float VidaMaxima { get; set; }
        public bool EsAutoJugador { get; set; }

        public float AvanceMax { get; set; }
        public float ReversaMax { get; set; } //velocidad maxima en reversa.
        public float Aceleracion { get; set; }
        public float DireccionRuedas { get; set; } //negativo para la derecha, positivo para la izquierda


        public float Desaceleracion { get; set; }
        public List<Arma> Armas { get; set; }
        public Arma ArmaSeleccionada { get; set; }
        public GameModel GameModel { get; set; }
        public TgcThirdPersonCamera CamaraAuto { get; set; }

        public Vector3 rotation;
        public Vector3 originalMeshRot = new Vector3(0, 0, -1);
        public float posEnY;
        public float velocidadRotacion = 120f;

        //----
        public float angTotal = 0;
        //---

        public bool huboRotacion;


        public Auto_miranda(string nombre, float vida, float avanceMaximo, float reversaMax,
                    float aceleracion, float desaceleracion, List<Arma> armas, TgcMesh mesh, GameModel model,Vector3 v)
        {

            //---- para atras y adelante -------------
            
            huboMovimiento = false;
            Velocidad = 0;
            orientacion = 0;
            AvanceMax = avanceMaximo;
            ReversaMax = reversaMax;
            Desaceleracion = 5f;
            InerciaNegativa = 1f;
            DireccionRuedas = ((float) Math.PI) / 2;
            Armas = armas;
            aceleracionVertical = 0f;
            vectorOrientacion = new Vector3(0,0,0);

            Mesh = mesh;
            newPosition = mesh.Position;
            //-------------------------


            Mesh.AutoTransformEnable = false;
            Mesh.AutoUpdateBoundingBox = false;
            Mesh.Transform = Matrix.Identity;
            Mesh.Position = new Vector3(0, 0, 0);
            Mesh.Scale = new Vector3(0.7f, 0.7f, 0.7f);  //0.0f para el hummer en este mapa.
            //Mesh.Rotation = new Vector3(0, 0, -1);
            Aceleracion = 10f;
            huboSalto = false;
            
            GameModel = model;
            rotate = 0;
            Mesh.Position = v;




        }

        /// <summary>
        /// hace que la camara siga a este auto.
        /// </summary>
        /// <returns></returns>
        /// 
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
        public void chocar(Vector3 angulo)
        {
            //this.direccionFrente = + pi/2??

        }
        public TgcCamera camaraSeguirEsteAuto()
        {
            Vector3 v = Mesh.Position;
            CamaraAuto = new TgcThirdPersonCamera(v, 200, 300);
            
            return CamaraAuto;
        }

        public void Acelerar()
        {
            velocidadInstantanea = -Aceleracion * 10f * GameModel.ElapsedTime;


            Velocidad += + velocidadInstantanea;
        }
        public void Frenar() //frenar tambien representa acelerar en reversa :P
        {
            velocidadInstantanea = Desaceleracion * 10f * GameModel.ElapsedTime;
            this.Velocidad = Velocidad + Aceleracion * GameModel.ElapsedTime * 15f;
        }

        /// <summary>
        /// Llamado una vez por frame
        /// </summary>
        /// <param name="input"></param>
        public void Update(TgcD3dInput input)
        {

            huboRotacion = false;
            huboMovimiento = false;
            
            //1 acelerar
            if (input.keyDown(Key.W))
            {
                Acelerar();
                huboMovimiento = true;
            }

            //2 frenar, reversa
            if (input.keyDown(Key.S))
            {
                Frenar();
                huboMovimiento = true;
            }

            //3 izquierda TODO//girar ruedas
            if (input.keyDown(Key.A))
            {
                rotate = -velocidadRotacion;
                Doblar(-1);
                //-----------
                
              
              
            }

            //4 derecha TODO //girar ruedas
            if (input.keyDown(Key.D))
            {
                rotate = velocidadRotacion;
                Doblar(1);
                
              
                
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

            //------------ SALTAR-------------------------------------------
            
            if (input.keyDown(Key.Space) && (Mesh.Position.Y == 5.0))
            {

                huboSalto = true;
                Saltar();
            }
            if (huboSalto)
            {
                gravedad();

                //aceleracionVertical = -1; descomentar cuando el auto choque con el suelo.
            }
            //---------------------------------------------
            
            MoverMesh();
        }

        private void Saltar()
        {
            aceleracionVertical += 5000f * GameModel.ElapsedTime;

        }

        private void Doblar(float lado)
        {
            huboRotacion = true;
            anguloRotacion = -lado * 10 * GameModel.ElapsedTime;
            DireccionRuedas -= 0.16f * GameModel.ElapsedTime;
            

            anguloOrientacion = -lado* 15 * Velocidad* (float)Math.PI / 180 * GameModel.ElapsedTime;
            angTotal = angTotal + anguloOrientacion;
        } 
           
        

        

        private void EnderezarRuedas()
        {
            /* if (DireccionRuedas < 0.01 || DireccionRuedas > 0.01)
                 DireccionRuedas = 0f;
             if (DireccionRuedas > 0)
                 DireccionRuedas -= 6f * GameModel.ElapsedTime;
             if (DireccionRuedas < 0)
                 DireccionRuedas += 6f * GameModel.ElapsedTime;*/

            DireccionRuedas = 0f;

        }

        private void gravedad()
        {

            if (Mesh.Position.Y < 4.9999)
            {
                aceleracionVertical = 0f;
                huboSalto = false;
                
                //rotation.Y = 5;
            }
            else
                aceleracionVertical -= 70f * GameModel.ElapsedTime;



        }



        /// <summary>
        /// Indica al mesh su nueva posicion y/o rotacion -- TODO
        /// </summary>
        private void MoverMesh()
        {
            if (huboRotacion)
                anguloOrientacionMesh = anguloOrientacionMesh - anguloOrientacion;

            if (aceleracionVertical < 4.9999)
            {
                aceleracionVertical = 0f;
                huboSalto = false;

                aux = 5;
                
            }
            else
            {
                aux = aceleracionVertical;
                
            }

            //---------------------------- Calculo el nuevo punto ---------------------


            newPosition.X = calcularDesplazamientoX();
            newPosition.Y = aux;
            newPosition.Z = calcularDesplazamientoZ();


            // variable qe sirve para ver el angulo del movimiento en tiempo de ejecucion
            //anguloMeshEnEjecucion = (float)Math.Atan2(newPosition.X, newPosition.Z) *180 / (float)Math.PI;

            vectorOrientacion = newPosition - Mesh.Position;

            // -------------------------- Transformacion final del mesh

            /*supuestamente la logica de esto seria:
             * 1) Traslado el centro del mesh al origen
             * 2) Roto
             * 3) Traslado el mesh rota a la posicion que estaba
             * 4) Traslado el mesh a su posicion final
             */
            Mesh.Transform = 
                 Matrix.Translation(-Mesh.Position.X, -Mesh.Position.Y, -Mesh.Position.Z) * 
                 Matrix.RotationY(anguloOrientacionMesh) *
                 Matrix.Translation(Mesh.Position.X, Mesh.Position.Y, Mesh.Position.Z) *
                 Matrix.Translation(newPosition);
            
            // actualizo la posicion del mesh
            Mesh.Position = newPosition;

            if (huboRotacion)
            {
                CamaraAuto.RotationY = anguloOrientacionMesh;
                
            }



            CamaraAuto.Target = Mesh.Position;
            
            CamaraAuto.UpdateCamera(GameModel.ElapsedTime);
       

        }

        /// <summary>
        /// lleva la velocidad del vehiculo lentamente a cero.
        /// </summary>
        /// 
        

        private float ProcesarInercia(float Velocidad)
        {

            if (Velocidad < 0.01f || Velocidad < 0.01f)
                Velocidad = 0f;



            if (Velocidad > 0)
                Velocidad -= InerciaNegativa * GameModel.ElapsedTime;


            if (Velocidad < 0)
                Velocidad += InerciaNegativa * GameModel.ElapsedTime;

            return Velocidad;



            //efecto gravedad? -> TODO

        }


        public float calcularDesplazamientoX()
        {
            return Velocidad * (float)Math.Sin(anguloOrientacionMesh);// - (90 * Math.PI / 180) );
        }

        public float calcularDesplazamientoZ()
        {       
            return  Velocidad * (float)Math.Cos(anguloOrientacionMesh);// - (90 * Math.PI / 180)  );
        }



        /// <summary>
        /// Muevo el mesh y lo rendereo.
        /// </summary>
        public void Render()
        {
            
            Mesh.render();
            Mesh.Transform = Matrix.Identity; // seteo matriz
        }
    }
}
