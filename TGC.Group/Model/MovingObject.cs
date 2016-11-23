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
using TGC.GroupoMs.Model.efectos;
using TGC.Core.Geometry;
using System.Drawing;
using TGC.Core.Collision;
using TGC.Core.Utils;
using TGC.Core.BoundingVolumes;

namespace TGC.GroupoMs.Model
{
    public class MovingObject
    {
        public bool fixEjecutado;
        public GameModel GameModel { get; set; }
        public string nombre { get; set; }
        public float Velocidad { get; set; } //velocidad actual, cantidad que se mueve por frame.
        public TgcMesh Mesh { get; set; }
        public float InerciaNegativa { get; set; } // cte de cada uno
        public Vector3 direccionMovimiento { get; set; } //hacia donde apunta.
        public float DireccionRuedas { get; set; } //negativo para la derecha, positivo para la izquierda

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
        public TgcBox humoBox;
        public HumoEscape humoEscape;

        public void PosicionRollback()
        {
            TirarHumoChoque();
            Mesh.Position = PosicionAnterior;
            Mesh.Rotation = RotacionAnterior;
            Velocidad = -Velocidad * 0.45f;


            //GameModel.AutoJugador.Doblar(40);
            if (Velocidad < 1f)
            {
                DeformationConstant = 2.3f;
                GameModel.AutoJugador.Doblar(40);
            }
        }
        public void RecobrarForma()
        {
            if(DeformationConstant > 1f)
            {
                DeformationConstant = DeformationConstant - 0.3f*GameModel.ElapsedTime;
            }
        }

        public float tInicioHumo=1f; //para el choque
        public static float tDuracionHumo =10f;
        public float tFinHumo=2f;

        private void TirarHumoChoque()
        {
            tInicioHumo = GameModel.ElapsedTime;
            tFinHumo = tInicioHumo + tDuracionHumo;
        }

        public static  float camaraOffsetDefaulForward = 300f;
        public TgcCamera camaraSeguirEsteAuto(GameModel model)
        {
            Vector3 v = Mesh.Position;
            CamaraAuto = new TgcThirdPersonCamera(v, 150, 300f);
            //motionBlur = new MotionBlur(CamaraAuto, model);
            GameModel.Niebla.CargarCamara(CamaraAuto);
            return CamaraAuto;
        }

        public void Saltar()
        {
            aceleracionVertical += 10f * GameModel.ElapsedTime;
        }

        public void Acelero(float valor)
        {
            Velocidad += valor * Aceleracion / 3 * GameModel.ElapsedTime; ;
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
                Velocidad += 0.8f * InerciaNegativa * GameModel.ElapsedTime;
                return;
            }
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

        public void CrearHumoCanioDeEscape(GameModel model)
        {
            humoBox = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(10, 10, 10), Color.Blue);
            humoEscape = new HumoEscape(model);
        }

        

        public float calcularDY()
        {
            if (huboSalto || Mesh.Position.Y > 5)
            {
                //velocidadInstantaneaVertical = 5 + 100f * GameModel.ElapsedTime - 400 * GameModel.ElapsedTime * GameModel.ElapsedTime;
                //return Mesh.Position.Y + velocidadInstantaneaVertical;
                time += GameModel.ElapsedTime;
                return posY += 30f * time - 45f * time * time;
            }
            else
            {
                time = 0;
                posY = 5;
                return Mesh.Position.Y - Mesh.Position.Y + 5;
            }
        }
        public  void EnderezarRuedas()
        {
            //if (DireccionRuedas < 0.01 || DireccionRuedas > 0.01)
            //    DireccionRuedas = 0f;
            if (DireccionRuedas > 0)
                DireccionRuedas -= 0.02f * GameModel.ElapsedTime;
            if (DireccionRuedas < 0)
                DireccionRuedas += 0.02f * GameModel.ElapsedTime;
            //if (DireccionRuedas > 1f)
            //    DireccionRuedas = 1f;
            //if (DireccionRuedas < 1f)
            //    DireccionRuedas = -1f;
        }

        public void ManejarColisionCamara()
        {
            //Actualizar valores de camara segun modifiers
            //COPIADO DE EJEMPLO COLISIONES CAMARA del tgc viewer

            //Pedirle a la camara cual va a ser su proxima posicion
            Vector3 position;
            Vector3 target;
            CamaraAuto.CalculatePositionTarget(out position, out target);

            //Detectar colisiones entre el segmento de recta camara-personaje y todos los objetos del escenario
            Vector3 q;
            var minDistSq = FastMath.Pow2(CamaraAuto.OffsetForward);
            foreach (var obstaculo in ciudadScene.Meshes)
            {
                //Hay colision del segmento camara-personaje y el objeto
                if (TgcCollisionUtils.intersectSegmentAABB(target, position, obstaculo.BoundingBox, out q))
                {
                    //Si hay colision, guardar la que tenga menor distancia
                    var distSq = Vector3.Subtract(q, target).LengthSq();
                    //Hay dos casos singulares, puede que tengamos mas de una colision hay que quedarse con el menor offset.
                    //Si no dividimos la distancia por 2 se acerca mucho al target.
                    minDistSq = FastMath.Min(distSq / 2, minDistSq);
                }
            }

            //Acercar la camara hasta la minima distancia de colision encontrada (pero ponemos un umbral maximo de cercania)
            var newOffsetForward = -FastMath.Sqrt(minDistSq);

            if (FastMath.Abs(newOffsetForward) < 10)
            {
                newOffsetForward = -10;
            }
            if(camaraOffsetDefaulForward > CamaraAuto.OffsetForward)
            {
                CamaraAuto.OffsetForward = (newOffsetForward - 72f * GameModel.ElapsedTime)*(-1f); //enderezo lentamente
                
            }
            else
            {
                CamaraAuto.OffsetForward = -newOffsetForward;
            }
            
            //CamaraAuto.OffsetForward = newOffsetForward;

            //Asignar la ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
            CamaraAuto.CalculatePositionTarget(out position, out target);
            CamaraAuto.SetCamera(position, target);

        }



        //public void ManejarColisionCamara()
        //{
        //    float distanciaDefault = 100f;
        //    bool choca = false;


        //    foreach (var sceneMesh in ciudadScene.Meshes)
        //    {
        //        if (MeshEstaCerca(CamaraAuto.Position, sceneMesh, distanciaDefault))
        //        {
        //            CamaraAuto.OffsetForward = CamaraAuto.OffsetForward - 12f*GameModel.ElapsedTime;
        //            choca = true;
        //            break;
        //        }
        //    }
        //    if (!choca)
        //        CamaraAuto.OffsetForward = camaraOffsetDefaulForward +12f* GameModel.ElapsedTime;
        //}

        public float DeformationConstant { get; set; }
        //OPTIMIZACION
        public TgcScene ciudadScene { get; set; }
        

        //public TgcScene bosqueScene { get; set; }
        public List<TgcMesh> MeshesCercanos;

        public void SeleccionarMeshesCercanos(float distanciaDefault)
        {
            //para la ciudad
            foreach (var sceneMesh in ciudadScene.Meshes)
            {
                if (MeshEstaCerca(Mesh.Position,sceneMesh,distanciaDefault))
                    MeshesCercanos.Add(sceneMesh);
            }
            //para el bosque
            //foreach (var sceneMesh in bosqueScene.Meshes)
            //{
            //    if (MeshEstaCerca(Mesh.Position,sceneMesh,distanciaDefault))
            //        MeshesCercanos.Add(sceneMesh);
            //}
        }
        //los meshes que estan mas lejos que esto no los evaluo en las colisiones.
        public bool MeshEstaCerca(Vector3 pos,TgcMesh sceneMesh,float distanciaDefault)
        {
            float d = TgcCollisionUtils.sqDistPointAABB(pos, sceneMesh.BoundingBox);

            if (d < distanciaDefault) return true;
            else return false;
        }
        //FIN OPTIMIZACION


        //----------------------------------------
        public bool intersectRayAABB(TgcRay.RayStruct ray, TgcBoundingAxisAlignBox aabb,out float tmin,
            out Vector3 q)
            //, out float tmin, out Vector3 q)
        {
            var aabbMin = TgcCollisionUtils.toArray(aabb.PMin);
            var aabbMax = TgcCollisionUtils.toArray(aabb.PMax);
            var p = TgcCollisionUtils.toArray(ray.origin);
            var d = TgcCollisionUtils.toArray(ray.direction);

            tmin = 0.0f; // set to -FLT_MAX to get first hit on line
            var tmax = float.MaxValue; // set to max distance ray can travel (for segment)
            q = Vector3.Empty;

            // For all three slabs
            for (var i = 0; i < 3; i++)
            {
                if (FastMath.Abs(d[i]) < float.Epsilon)
                {
                    // Ray is parallel to slab. No hit if origin not within slab
                    if (p[i] < aabbMin[i] || p[i] > aabbMax[i]) return false;
                }
                else
                {
                    // Compute intersection t value of ray with near and far plane of slab
                    var ood = 1.0f / d[i];
                    var t1 = (aabbMin[i] - p[i]) * ood;
                    var t2 = (aabbMax[i] - p[i]) * ood;
                    // Make t1 be intersection with near plane, t2 with far plane
                    if (t1 > t2) TgcCollisionUtils.swap(ref t1, ref t2);
                    // Compute the intersection of slab intersection intervals
                    tmin = TgcCollisionUtils.max(tmin, t1);
                    tmax = TgcCollisionUtils.min(tmax, t2);
                    // Exit with no collision as soon as slab intersection becomes empty
                    if (tmin > tmax) return false;
                }
            }
            // Ray intersects all 3 slabs. Return point (q) and intersection t value (tmin)
            q = ray.origin + ray.direction * tmin;
            return true;
        }
    }
}
