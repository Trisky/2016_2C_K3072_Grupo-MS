using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.GroupoMs.Model
{
    public class Arma : MovingObject
    {
        
        public int danio { get; set; }
        public int ammo { get; set; } //ammo ilimitado: -1

        public bool tieneAmmoIlimitada() {
            return this.ammo == -1;
        }

        public int daniar(Auto victima) {
            victima.recibirDanio(this.danio);
            return 0;
        
        }

        public Arma(string nombre,int danio,int ammo)
        {
            this.nombre = nombre;
            this.danio = danio;
            this.ammo = ammo;
        }
        
    }
}
