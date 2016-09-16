using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Example;

namespace TGC.GroupoMs.Model
{
    public class MovingObject 
    {
        public string nombre { get; set; }
        public float Velocidad { get; set; } //velocidad actual, cantidad que se mueve por frame.
        public TgcMesh Mesh { get; set; }
        public float InerciaNegativa { get; set; } // cte de cada uno
        public Vector3 direccionMovimiento { get; set; } //hacia donde apunta.
    }
}
