using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Text;
using System.Drawing;
using TGC.Core.Utils;
using TGC.Core.Direct3D;

namespace TGC.GroupoMs.Model
{
    public class Cronometro
    {
        public TgcText2D text2d;
        public float time = 0;
        
        public void render(float elapseElapsedTime)
        {
            if (elapseElapsedTime < 200)
                time += elapseElapsedTime;

            var seg = Math.Truncate(time % 60);
            var min = Math.Truncate(time / 60);

            var segString = "";
            var minString = "";


            if (min < 10)
                minString = "0";

            if (seg < 10)
                segString = "0";
            segString = segString + seg.ToString();
            minString = minString + min.ToString();
            text2d = new TgcText2D();
            text2d.Text = minString + " : " + segString + " ";
            //text2d.Text = time.ToString();
            text2d.Color = Color.WhiteSmoke;
            text2d.Align = TgcText2D.TextAlign.LEFT;
            text2d.Position = new Point(D3DDevice.Instance.Width - 210, 650);
            text2d.Size = new Size(300, 100);
            text2d.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold | FontStyle.Italic));
            text2d.render();
        }
    }
}
