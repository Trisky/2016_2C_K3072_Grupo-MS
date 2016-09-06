using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Examples.Camara;

namespace TGC.GroupoMs.Model
{
    public class Auto : MovingObject
    {
        public int vida { get; set; }
        public int vidaMaxima { get; set; }
        public bool EsAutoJugador { get; set; }

        public int avanceMax { get; set; }
        public int reversaMax { get; set; } //velocidad maxima en reversa.
        public int aceleracion { get; set; }
        
        public int desaceleracion { get; set; } 
        public List<Arma> armas { get; set; }
        
        
        
        //hace que la velocidad se acerque a cero constantemente si no se esta acelerando o frenando.
        public Arma ArmaSeleccionada { get; set; }

        public Auto(string nombre, int vida, int avanceMaximo, int reversaMax,
                    int aceleracion,int desaceleracion,List<Arma> armas,TgcMesh mesh)
        {
            //TODO
        }
        public Auto()
        {
            this.vida = -1;
            this.nombre = "testCar";
            this.avanceMax = 100;
            this.reversaMax = 100;
            this.aceleracion = 100;
            this.velocidad = 0;
            this.desaceleracion = 50;
            this.inerciaNegativa = 20;
            this.armas = new List<Arma>();
            //this.direccionFrente = new Microsoft.DirectX.Vector3()

        }

        /// <summary>
        /// hace que la camara siga a este auto.
        /// </summary>
        /// <returns></returns>
        public TgcCamera camaraSeguirEsteAuto() {
            return new TgcThirdPersonCamera(Mesh.Position, 200, 300);
        }

        public void recibirDanio(int cant)
        {
            if (this.vidaMaxima < 0) {
                return;
            }
            this.vida = -cant;
            if (this.vida <= 0)
            {
                //TODO this.morir; //explota?
            }
            if (this.EsAutoJugador)
            {
                //mostrar en pantalla danio recibido
            }
            else
            {
                //mostrar en pantalla danio dado
            }

        }
        public void chocar(Microsoft.DirectX.UnsafeNativeMethods.Vector3 angulo) {
            //this.direccionFrente = + pi/2??
            
        }
        public void acelerar() {
            if (this.avanceMax > this.velocidad)
            {
                this.velocidad = +this.aceleracion;
            }

        }
        public void frenar() //frenar tambien representa acelerar en reversa :P
        {
            if(this.reversaMax < this.velocidad)
            {
                this.velocidad = -this.desaceleracion;
            }
        }

        internal void Update(TgcD3dInput input)
        {
            this.procesarInercia();
            
            //1 acelerar
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.W))
            {
                this.acelerar();
            }

            //2 frenar, reversa
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.S))
            {
                this.frenar();
            }

            //3 izquierda TODO
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.A))
            {
                //cambiar la direccion del mesh en X grados hacia la izquierda
            }

            //4 derecha TOOD
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.D))
            {
                //cambiar la direccion del mesh en X grados hacia la derecha
            }

            //5 disparar TODO -- 
            //if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            //{
            //    //disparar 
            //    //this.ArmaSeleccionada.ammo--;
            //    //TODO falta evento de fin de ammo.
            //}

            //6 cambiar de arma TODO
            if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            { 

            }
            
            //7 freno de mano? maybe TODO
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.Space))
            {

            }


            //this.Mesh.


        }

        /// <summary>
        /// lleva la velocidad del vehiculo lentamente a cero.
        /// </summary>
        private void procesarInercia()
        {
            if (this.velocidad > 0)
            {
                this.velocidad = -this.inerciaNegativa;
            }
            else
            {
                this.velocidad = +this.inerciaNegativa;
            }
        }
    }
}
