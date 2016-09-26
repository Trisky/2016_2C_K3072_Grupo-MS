using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Example;
using TGC.GrupoMs.Camara;
using TGC.Core.Camara;
using TGC.Group.Model;

namespace TGC.GroupoMs.Model
{
    public class MovingObject 
    {
        public GameModel GameModel { get; set; }
        public string nombre { get; set; }
        public float Velocidad { get; set; } //velocidad actual, cantidad que se mueve por frame.
        public TgcMesh Mesh { get; set; }
        public float InerciaNegativa { get; set; } // cte de cada uno
        public Vector3 direccionMovimiento { get; set; } //hacia donde apunta.

        public float aceleracionVertical;
        public float Desaceleracion { get; set; }

        public float AvanceMax { get; set; }
        public float ReversaMax { get; set; } //velocidad maxima en reversa.
        public float Aceleracion { get; set; }

        public float Vida { get; set; }
        public float VidaMaxima { get; set; }

        public Vector3 PosicionAnterior { get; set; }
        public Vector3 RotacionAnterior { get; set; }

        public TgcThirdPersonCamera CamaraAuto { get; set; }

        public void PosicionRollback()
        {
            Mesh.Position = PosicionAnterior;
            Mesh.Rotation = RotacionAnterior;
        }

        public TgcCamera camaraSeguirEsteAuto()
        {
            Vector3 v = Mesh.Position;
            CamaraAuto = new TgcThirdPersonCamera(v, 200, 300);
            return CamaraAuto;
        }

        public void Saltar()
        {
            aceleracionVertical += 10f * GameModel.ElapsedTime;
        }

        public void Acelerar()
        {
            if (AvanceMax > Velocidad)
            {
                Velocidad += Aceleracion * GameModel.ElapsedTime; //uso elapsed time para que sea time-bound
            }
        }

        public void Frenar() //frenar tambien representa acelerar en reversa :P
        {
            if (-ReversaMax < Velocidad)
            {
                Velocidad -= Desaceleracion * GameModel.ElapsedTime;
            }
        }
    }
}
