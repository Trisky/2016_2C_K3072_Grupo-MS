using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Group.Model;
using TGC.GroupoMs.Model.ScreenOverlay;

namespace TGC.GroupoMs.Model
{
    public class Velocimetro
    {
        private CustomSprite spriteVelocimetro;
        private GameModel gameModel;
        private Drawer2D drawer2D;

        public Velocimetro(GameModel gm)
        {
            gameModel = gm;
            spriteVelocimetro = new CustomSprite();
            spriteVelocimetro.Bitmap = new CustomBitmap(gm.MediaDir + "\\speedometer.png", D3DDevice.Instance.Device);
            spriteVelocimetro.Scaling = new Vector2(0.5f, 0.5f);
            spriteVelocimetro.Position = new Vector2(0, 0);
            drawer2D = new Drawer2D();

            //falta crear la aguja y ponerla en su lugar.
        }

        public void Update(float Velocidad)
        {
            //cuando tenga la aguja la muevo segun la velocidad :P
        }

        public void Render()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(spriteVelocimetro);
            drawer2D.EndDrawSprite();
        }

        
    }
}
