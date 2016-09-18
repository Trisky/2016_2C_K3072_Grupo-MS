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

namespace TGC.GroupoMs.Model
{
    public class Auto : MovingObject
    {
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

        public int MyProperty { get; set; }

        public Auto(string nombre, float vida, float avanceMaximo, float reversaMax,
                    float aceleracion,float desaceleracion,List<Arma> armas,TgcMesh mesh,GameModel model)
        {
            AvanceMax = avanceMaximo;
            ReversaMax = reversaMax;
            Desaceleracion = desaceleracion;
            InerciaNegativa = 1f;
            DireccionRuedas = 0f;
            Armas = armas;

            Mesh = mesh;
            Mesh.AutoTransformEnable = true;
            Mesh.AutoUpdateBoundingBox = true;
            Mesh.Scale = new Vector3(0.7f, 0.7f, 0.7f);  //0.0f para el hummer en este mapa.
            //Mesh.Rotation = new Vector3(0, 0, -1);
            Aceleracion = aceleracion;

            GameModel = model;

            
        }
        
        /// <summary>
        /// hace que la camara siga a este auto.
        /// </summary>
        /// <returns></returns>
        public TgcCamera camaraSeguirEsteAuto() {
            Vector3 v = Mesh.Position;
            CamaraAuto = new TgcThirdPersonCamera(v, 200, 300);
            return CamaraAuto;
        }

        public void recibirDanio(int cant)
        {
            if (this.VidaMaxima < 0) {
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
        public void chocar(Vector3 angulo) {
            //this.direccionFrente = + pi/2??
            
        }
        public void Acelerar() {
            if (AvanceMax > Velocidad)
            {
                Velocidad += Aceleracion * GameModel.ElapsedTime; //uso elapsed time para que sea time-bound
            }
        }

        public void Frenar() //frenar tambien representa acelerar en reversa :P
        {
            if(-ReversaMax < Velocidad)
            {
            Velocidad -= Desaceleracion * GameModel.ElapsedTime;
            }
        }

        /// <summary>
        /// Llamado una vez por frame
        /// </summary>
        /// <param name="input"></param>
        public void Update(TgcD3dInput input)
        {
            if(!input.keyDown(Key.W) && !input.keyDown(Key.S))
                ProcesarInercia();
            if (!input.keyDown(Key.A) && !input.keyDown(Key.D))
                EnderezarRuedas();
            
            
            //1 acelerar
            if (input.keyDown(Key.W))
            {
                Acelerar();
            }

            //2 frenar, reversa
            if (input.keyDown(Key.S))
            {
                Frenar();
            }

            //3 izquierda TODO//girar ruedas
            if (input.keyDown(Key.A))
            {
                Doblar(0);
            }

            //4 derecha TODO //girar ruedas
            if (input.keyDown(Key.D))
            {
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

            MoverMesh(); 
        }

        private void Doblar(int lado)
        {
            if(lado == 0) //doblo iz
            {
                DireccionRuedas -= 3f * GameModel.ElapsedTime;
            }
            if(lado == 1) //doblo der
            {
                DireccionRuedas += 3f * GameModel.ElapsedTime;
            }
        }

        private void EnderezarRuedas()
        {
            if (DireccionRuedas < 0.01 || DireccionRuedas > 0.01)
                DireccionRuedas = 0f;
            if (DireccionRuedas > 0)
                DireccionRuedas -= 6f * GameModel.ElapsedTime;
            if(DireccionRuedas<0)
                DireccionRuedas += 6f * GameModel.ElapsedTime;

        }

        /// <summary>
        /// Indica al mesh su nueva posicion y/o rotacion -- TODO
        /// </summary>
        private void MoverMesh()
        {
            
            //1- rotacion
            //Mesh.Rotation

            //2- traslacion
            Vector3 rotation = Mesh.Rotation; //obtengo la orientacion actual del mesh
            if (rotation == Vector3.Empty)
                rotation = new Vector3(0, 0, -1);
            rotation.Multiply(Velocidad); //ahora tengo lo que me tengo q mover en el vector este
            Mesh.move(rotation);

            if(CamaraAuto != null) //actualizo la posicion de la camara respecto de la del mesh
            CamaraAuto.Target = Mesh.Position;
        }

        /// <summary>
        /// lleva la velocidad del vehiculo lentamente a cero.
        /// </summary>
        private void ProcesarInercia()
        {
            
            if (Velocidad < 0.01f || Velocidad<0.01f)
            {
                Velocidad = 0f;
                return;
            }
            //falta hacer que dismunuya la velocidad cada X tiempo y no por frame.
            //if (Velocidad < -inerciaNegativa)
            //{
            //    Velocidad++;
            //    return;
            //}
            //if (Velocidad > inerciaNegativa)
            //{
            //    Velocidad--;
            //    return;
            //}

            if (Velocidad > 0)
            {
                Velocidad -=InerciaNegativa * GameModel.ElapsedTime;
                return;
            }
            if(Velocidad < 0)
            {
                Velocidad +=InerciaNegativa * GameModel.ElapsedTime;
                return;
            }
           
            //efecto gravedad? -> TODO

        }

        /// <summary>
        /// Muevo el mesh y lo rendereo.
        /// </summary>
        public void Render()
        {
            Mesh.render();
        }
    }
}
