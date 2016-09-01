using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.GroupoMs.Model
{
    class Auto
    {
        public int vida { get; set; }
        public string nombre { get; set; }
        public int avanceMax { get; set; }
        public int reversaMax { get; set; } //velocidad maxima en reversa.
        public int aceleracion { get; set; }
        public int velocidad { get; set; } //velocidad actual
        public int desaceleracion { get; set; }
        public List<Arma> armas { get; set; }
        public float direccionFrente { get; set; } //direccion en la que apunta el frente del auto.

        public int inerciaNegativa { get; set; } // cte de cada Auto,
        //hace que la velocidad se acerque a cero constantemente si no se esta acelerando o frenando.

        public Auto(string nombre, int vida, int avanceMaximo, int reversaMax,
                    int aceleracion,int desaceleracion,List<Arma> armas)
        {
           
        }

        public void recibirDanio(int cant)
        {
            this.vida = -cant;
            if (this.vida <= 0)
            {
                //TODO this.morir; //explota?
            }
            
        }
        public int chocar(float angulo) {
            //this.direccionFrente = + pi/2 
            return 0;
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

        




       


    }
}
