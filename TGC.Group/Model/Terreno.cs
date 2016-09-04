using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using static TGC.Core.Geometry.TgcPlane;

namespace TGC.GrupoMs.Model
{
    class Terreno 
    {
        private TgcPlane plane;
        public Terreno() {

            //asigno textura

            //TODO hacer bien lo del path
            var text = "C:\\Users\\Trisky\\github\\tgc-viewer\\TGC.Viewer\\bin\\Debug\\Media\\Texturas\\tierra.jpg";
            var currentTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, text);
            
            //creo plano
            this.plane = new TgcPlane();
            this.plane.setTexture(currentTexture);
            this.plane.Orientation = Orientations.XYplane; //set de orientacion del plano
            this.plane.Size = new Microsoft.DirectX.Vector3(500, 500, 500);
            this.plane.Origin = new Microsoft.DirectX.Vector3(0, 0, 0);
            this.plane.UTile = 1;
            this.plane.VTile = 1;
            this.plane.AutoAdjustUv = true;

            

        }
        public void Render()
        {
            this.plane.render();
        }
    }
}
