using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Sound;

namespace TGC.GroupoMs.Model
{
    public class Sonidos
    {
        public TgcMp3Player motor;
        private TgcMp3Player frenada;

        public Sonidos(string MediaDir)
        {
            //motor
            motor = new TgcMp3Player();
            motor.FileName = MediaDir + "\\motor.mp3";
            motor.play(true);


            frenada = new TgcMp3Player();
            frenada.FileName = MediaDir + "\\frenada.mp3";
        }

        public void playFrenada()
        {
            frenada.play(true);
        }
        public void stopFrenada()
        {
            frenada.play(false);
        }
        
    }
}
