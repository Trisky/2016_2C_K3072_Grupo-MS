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


        //----------- Ariel -------------------
        public bool huboMovimiento;
        public bool huboRotacion;
        public Vector3 pivoteMesh;
        public Vector3 newPosicion;
        public float angOrientacionMesh = 270 * (float)Math.PI / 180;

        public Matrix matrixRotacion;
        public float anguloFinal = 0;


        //--------------- salto -----------------
        public float impulsoSalto = 20f;
        public bool huboSalto = false;
        public float velocidadInstantaneaVertical = 0;
        public float VelocidadVertical = 0;
        public float longitudSalto = 10;
        public float posY = 5;
        public float time = 0;


        public MotionBlur motionBlur;

        public void PosicionRollback()
        {
            Mesh.Position = PosicionAnterior;
            Mesh.Rotation = RotacionAnterior;
            Velocidad = -Velocidad * 0.1f;
        }

        public TgcCamera camaraSeguirEsteAuto(GameModel model)
        {
            Vector3 v = Mesh.Position;
            CamaraAuto = new TgcThirdPersonCamera(v, 200, 300);
            //motionBlur = new MotionBlur(CamaraAuto, model);
            return CamaraAuto;
        }

        public void Saltar()
        {
            aceleracionVertical += 10f * GameModel.ElapsedTime;
        }

        public void Acelero(float valor)
        {
            Velocidad += valor*Aceleracion / 3 * GameModel.ElapsedTime; ;
            if (AvanceMax < Velocidad)
                Velocidad = AvanceMax;
        }

        public void Freno()
        {
            Velocidad += -Desaceleracion / 2 * GameModel.ElapsedTime;
            if (ReversaMax > Velocidad)
                Velocidad = ReversaMax;
        }

        public void ProcesarInercia()
        {

            if ((Velocidad < 0.01f && Velocidad > 0f) || (Velocidad > -0.01f && Velocidad < 0f)) //si velocidad esta entre +-0.01f
            {
                Velocidad = 0f;
                return;
            }

            if (Velocidad > 0)
            {
                Velocidad -= InerciaNegativa * GameModel.ElapsedTime;
                return;
            }
            if (Velocidad < 0)
            {
                Velocidad += 0.8f*InerciaNegativa * GameModel.ElapsedTime;
                return;
            }

            //efecto gravedad? -> TODO

        }
        public float calcularDX()
        {
            //return velocidadInstantanea * (float)Math.Cos(angOrientacionMesh);
            return Velocidad * (float)Math.Cos(angOrientacionMesh);
        }
        public float calcularDZ()
        {
            //return velocidadInstantanea * (float)Math.Sin(angOrientacionMesh);
            return Velocidad * (float)Math.Sin(angOrientacionMesh);
        }
    }
}
