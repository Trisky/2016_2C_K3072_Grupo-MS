using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.GroupoMs.Model
{
    class Arma
    {
        public string nombre { get; set; }
        public int danio { get; set; }
        public int ammo { get; set; } //ammo ilimitado: -1

        public bool tieneAmmoIlimitada() {
            return this.ammo == -1;
        }

        public int daniar(Auto victima) {
            victima.recibirDanio(this.danio);
            return 0;
        
        }
        
    }
}
