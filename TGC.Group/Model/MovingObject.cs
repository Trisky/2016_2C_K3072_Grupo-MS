using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.GroupoMs.Model
{
    public class MovingObject
    {
        public string nombre { get; set; }
        public int velocidad { get; set; } //velocidad actual, cantidad que se mueve por frame.
        public TgcMesh Mesh { get; set; }
        public Microsoft.DirectX.Vector3 direccion { get; set; } //direccion en la que apunta el frente
        public int inerciaNegativa { get; set; } // cte de cada uno
        public Vector3 direccionMovimiento { get; set; } //hacia donde apunta.
    }
}
