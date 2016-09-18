using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Textures;
using TGC.GroupoMs.Camaras;

namespace TGC.GroupoMs.Model
{
    /// <summary>
    /// Aca va a estar el menu que va a ser del tipo point and click con una
    /// camara centrada en el origen del cubo.
    /// El texto va a estar flotante y si el framework no lo soporta 
    /// sera una imagen en uno de los lados del cubo.
    /// </summary>
    public class MenuCaja
    {
        public TgcD3dInput Input { get; set; }


        public TgcBox MenuBox { get; set; }
        public TgcRotationalCamera CamaraRotacional { get; set; }


        

        /// <summary>
        /// Crea el menu-caja, recibe el input :P
        /// </summary>
        /// <param name="input"></param>
        public MenuCaja(TgcD3dInput input)
        {
            Input = input;
            
            var size = new Vector3(100, 100, 100);
            MenuBox = TgcBox.fromSize(size,Color.Black);
            MenuBox.AutoTransformEnable = true;
            MenuBox.Position = new Vector3(0,2000, 0);
        }

        public TgcRotationalCamera CrearCamaraCaja()
        {
            
            Vector3 pos = new Vector3(0, 2000, 0);
            Vector3 target = new Vector3(10, 2000, 0);
            CamaraRotacional = new TgcRotationalCamera(pos, target, Input);
            return CamaraRotacional;
        }

        public void Render()
        {
                MenuBox.render();
        }



    }

}
