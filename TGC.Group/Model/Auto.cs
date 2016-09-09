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

namespace TGC.GroupoMs.Model
{
    public class Auto : MovingObject
    {
        public int Vida { get; set; }
        public int VidaMaxima { get; set; }
        public bool EsAutoJugador { get; set; }

        public int AvanceMax { get; set; }
        public int ReversaMax { get; set; } //velocidad maxima en reversa.
        public int Aceleracion { get; set; }
        
        public int Desaceleracion { get; set; } 
        public List<Arma> Armas { get; set; }
        public Arma ArmaSeleccionada { get; set; }

        public Auto(string nombre, int vida, int avanceMaximo, int reversaMax,
                    int aceleracion,int desaceleracion,List<Arma> armas,TgcMesh mesh)
        {
            AvanceMax = avanceMaximo;
            ReversaMax = reversaMax;
            Desaceleracion = desaceleracion;
            Armas = armas;
            Mesh = mesh;
            Aceleracion = aceleracion;
        }
        public Auto()
        {
            this.Vida = -1;
            this.nombre = "testCar";
            this.AvanceMax = 100;
            this.ReversaMax = 100;
            this.Aceleracion = 100;
            this.Velocidad = 0;
            this.Desaceleracion = 50;
            this.inerciaNegativa = 20;
            this.Armas = new List<Arma>();
            //this.direccionFrente = new Microsoft.DirectX.Vector3()
        }

        /// <summary>
        /// hace que la camara siga a este auto.
        /// </summary>
        /// <returns></returns>
        public TgcCamera camaraSeguirEsteAuto() {
            Vector3 v = Mesh.Position;
            return new TgcThirdPersonCamera(v, 200, 300);
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
            if (this.AvanceMax > this.Velocidad)
            {
                this.Velocidad = +this.Aceleracion;
            }
        }

        public void Frenar() //frenar tambien representa acelerar en reversa :P
        {
            if(this.ReversaMax < this.Velocidad)
            {
                this.Velocidad = -this.Desaceleracion;
            }
        }

        public void Update(TgcD3dInput input)
        {
            //ProcesarInercia();
            
            //1 acelerar
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.W))
            {
                Acelerar();
            }

            //2 frenar, reversa
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.S))
            {
                Frenar();
            }

            //3 izquierda TODO
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.A))
            {
                //cambiar la direccion del mesh en X grados hacia la izquierda
            }

            //4 derecha TODO
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.D))
            {
                //cambiar la direccion del mesh en X grados hacia la derecha
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

        private void MoverMesh()
        {
            Vector3 rotation = Mesh.Rotation; //obtengo la orientacion actual del mesh
            rotation.Multiply(Velocidad); //ahora tengo lo que me tengo q mover en el vector este
            Mesh.move(rotation);
        }

        /// <summary>
        /// lleva la velocidad del vehiculo lentamente a cero.
        /// </summary>
        private void ProcesarInercia()
        {
            if (Velocidad == 0)
            {
                return;
            }
            //falta hacer que dismunuya la velocidad cada X tiempo y no por frame.
            if (Velocidad < -inerciaNegativa)
            {
                Velocidad++;
                return;
            }
            if (Velocidad > inerciaNegativa)
            {
                Velocidad--;
                return;
            }

            if (Velocidad > 0)
            {
                Velocidad = Velocidad-inerciaNegativa;
                return;
            }
            if(Velocidad < 0)
            {
                Velocidad = Velocidad+inerciaNegativa;
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
