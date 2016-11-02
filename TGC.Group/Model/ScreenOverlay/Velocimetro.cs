using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Group.Model;
using TGC.GroupoMs.Model.ScreenOverlay;
using System.Drawing;
using TGC.Core.Utils;

namespace TGC.GroupoMs.Model
{
    public class Velocimetro
    {
        private CustomSprite spriteVelocimetro;
        private CustomSprite spriteAguja;
        private GameModel gameModel;
        private Drawer2D drawer2D;
        public float acum = 22;
        

        public Velocimetro(GameModel gm)
        {
            gameModel = gm;
            spriteVelocimetro = new CustomSprite();
            spriteVelocimetro.Bitmap = new CustomBitmap(gm.MediaDir + "\\speedometer.png", D3DDevice.Instance.Device);
            spriteVelocimetro.Scaling = new Vector2(0.8f, 0.8f);
            var textureSize = spriteVelocimetro.Bitmap.Size;
            spriteVelocimetro.Position =  new Vector2(FastMath.Max(D3DDevice.Instance.Width - textureSize.Width, 0),
                FastMath.Max(D3DDevice.Instance.Height- textureSize.Height, 0));

            //falta crear la aguja y ponerla en su lugar.
            spriteAguja = new CustomSprite();
            spriteAguja.Bitmap = new CustomBitmap(gm.MediaDir + "\\aguja.png", D3DDevice.Instance.Device);
            spriteAguja.Scaling = new Vector2(0.1f, 0.2f);
            //spriteAguja.Position = new Vector2(spriteVelocimetro.Position.X + textureSize.Width/2, spriteVelocimetro.Position.Y + textureSize.Height/2);
            spriteAguja.Position = new Vector2(spriteVelocimetro.Position.X +(textureSize.Width / 2.6f), spriteVelocimetro.Position.Y + (textureSize.Height / 2.6f));
            //spriteAguja.Rotation = FastMath.PI / 4;
            drawer2D = new Drawer2D();

            


        }

        public void Update(float velocidad, bool huboMarchaAtras)
        {
            //cuando tenga la aguja la muevo segun la velocidad :P
            //if()

            if (velocidad < 0)
                spriteAguja.Rotation = (FastMath.PI / 4 - velocidad);
            else
                spriteAguja.Rotation = FastMath.PI / 4 + velocidad;
            if (velocidad < 0 && huboMarchaAtras)
                spriteAguja.Rotation = FastMath.PI / 4;
            
           

        }

        public void Render()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(spriteVelocimetro);
            drawer2D.DrawSprite(spriteAguja);
            drawer2D.EndDrawSprite();
            
        }

        
    }
}
