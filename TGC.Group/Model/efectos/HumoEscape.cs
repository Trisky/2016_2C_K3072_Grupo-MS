using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Particle;
using TGC.Group.Model;

namespace TGC.GroupoMs.Model.efectos
{
    /// <summary>
    /// Humo/fuego del canio de escape.
    /// </summary>
    public class HumoEscape
    {
        private ParticleEmitter emitter1;
        private ParticleEmitter emitter2;
        private int selectedParticleCount;
        private string texturaHumito;
        private string texturaFuego;
        private string texturePath;
        private GameModel gameModel;

        public HumoEscape(GameModel gm)
        {
            gameModel = gm;
            Init();
            
        }

        public void Init()
        {

            texturePath = Game.Default.MediaDirectory + "Particles\\";
            //Crear emisor de particulas
            texturaHumito = "pisada.png";
            texturaFuego = "fuego.png";
           
            selectedParticleCount = 10;

            //emmiter1
            emitter1 = new ParticleEmitter(texturePath + texturaHumito, selectedParticleCount);
            emitter1.Position = new Vector3(0, 15, 0);

            Vector3 speed = new Vector3(5, 5, 5);
            emitter1.MinSizeParticle = 5f;
            emitter1.MaxSizeParticle = 10f;
            emitter1.ParticleTimeToLive = 0.5f;
            emitter1.CreationFrecuency = 0.1f;
            emitter1.Dispersion = 15;
            emitter1.Speed = speed;

            //emmiter2
            selectedParticleCount = 10;
            emitter2 = new ParticleEmitter(texturePath + texturaFuego, selectedParticleCount);
            emitter2.Position = new Vector3(0, 15, 0);

            
            emitter2.MinSizeParticle = 5f;
            emitter2.MaxSizeParticle = 10f;
            emitter2.ParticleTimeToLive = 0.5f;
            emitter2.CreationFrecuency = 0.1f;
            emitter2.Dispersion = 15;
            emitter2.Speed = speed;

        }

        public void Update(Vector3 pos,Vector3 rotation)
        {
            //primero me muevo 5 unidades para atras del centro del auto.
            rotation.Normalize();
            Vector3 posEscape = pos  + new Vector3(0, 10, 0) ;
            emitter1.Position = posEscape;
            emitter2.Position = posEscape;
            
            //emitter.ro
        }

        //public void MoverHumoCanioEscapeA(Vector3 newPosicion, Vector3 rotation)
        //{

        //    //primero me muevo 5 unidades para atras del centro del auto.
        //    
        //    posicionCajita = posicionCajita + new Vector3(0, 15, 0);
        //    humoBox.move(posicionCajita);
        //    //humoBox.Rotation = -rotation;
        //}

        public void Render(bool conNitro)
        {
            //IMPORTANTE PARA PERMITIR ESTE EFECTO.
            D3DDevice.Instance.ParticlesEnabled = true;
            D3DDevice.Instance.EnableParticles();
            if (conNitro)
            {
                emitter2.render(gameModel.ElapsedTime);
            }
            else
            {
                emitter1.render(gameModel.ElapsedTime);
            }
            

        }
    }
}
