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
        private Vector3 offsetEscape;

        public HumoEscape(GameModel gm)
        {
            gameModel = gm;
            offsetEscape = new Vector3(10, 5, 42);
            Init();
        }

        public HumoEscape(GameModel gm,bool a)
        {
            gameModel = gm;
            if (!a) return; //solo para cuando quiero que sea de choque
            Init();
        }

        private void setEmmiter(ParticleEmitter emitter)
        {
            emitter.MinSizeParticle = 0.7f;
            emitter.MaxSizeParticle = 1.2f;
            emitter.ParticleTimeToLive = 1f;
            emitter.CreationFrecuency = 0.04f;
            emitter.Speed = new Vector3(1, 5, 100);
        }

        public void Init()
        {

            texturePath = Game.Default.MediaDirectory + "Particles\\";
            //Crear emisor de particulas
            texturaHumito = "pisada.png";
            texturaFuego = "fuego.png";
           
            selectedParticleCount = 10;

            //emmiter1 humo
            emitter1 = new ParticleEmitter(texturePath + texturaHumito, selectedParticleCount);
            emitter1.Position = new Vector3(0, 15, 0);

            Vector3 speed = new Vector3(5, 5, 5);
            setEmmiter(emitter1);

            //emmiter fuego
            selectedParticleCount = 10;
            emitter2 = new ParticleEmitter(texturePath + texturaFuego, selectedParticleCount);
            emitter2.Position = new Vector3(0, 15, 0);

            setEmmiter(emitter2);

        }

        public void Update(Vector3 pos,float rotation)
        {
            //los dos escapes van en el mismo lugar
            emitter1.Position = calcularMatrizRotacion(pos,rotation);
            emitter2.Position = emitter1.Position;
            
        }

        private Vector3 calcularMatrizRotacion(Vector3 pos, float rotation)
        {
            
            
            var matrix = Matrix.Translation(offsetEscape) * Matrix.RotationY(rotation)
                         * Matrix.Translation(pos);
            return new Vector3(matrix.M41, matrix.M42, matrix.M43); ;
        }

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
