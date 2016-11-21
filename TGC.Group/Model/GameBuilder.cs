using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Model;
using TGC.GroupoMs.Model.efectos;

namespace TGC.GroupoMs.Model
{
    public class GameBuilder
    {
        public string MediaDir { get; set; }
        public GameModel Gm { get; set; }
        public TgcSceneLoader Loader { get; set; }


        public GameBuilder(string md, GameModel gm, TgcSceneLoader ld)
        {
            MediaDir = md;
            Gm = gm;
            Loader = ld;
        }


        public Auto CrearHummer(TgcScene MapScene, Velocimetro velocimetro)
        {
            float scaleRuedas = 1f;
            TgcMesh rueda = CrearRueda(1f);
            rueda.move(0, -25, 0);
            TgcScene hummerScene = Loader.loadSceneFromFile(MediaDir + "Hummer\\Hummer-TgcScene.xml");
            TgcMesh hummerMesh = hummerScene.Meshes[0];
            hummerMesh.Scale = new Vector3(0.4f,0.4f,0.4f);

            //para las ruedas el offset es (ancho,altura,largo)
            float y = 13f;
            Ruedas  ruedasAtras   = new Ruedas(rueda, new Vector3(40, y, 54), new Vector3(-40, y, 54), true, scaleRuedas);
            Ruedas ruedasAdelante = new Ruedas(rueda, new Vector3(-40, y, -62), new Vector3(40, y, -62), false, scaleRuedas);

            return new Auto("hummer", 100f, 5f, 3f, 5f, 2f,
                            hummerMesh, Gm,
                            ruedasAdelante, ruedasAtras, rueda, velocimetro);
        }

        public TgcMesh CrearRueda(float escala)
        {
            //TgcMesh rueda = Loader.loadSceneFromFile(MediaDir + "ModelosX\\rueda.x").Meshes[0];
            TgcMesh rueda = Loader.loadSceneFromFile(MediaDir + "Rueda\\Rueda-TgcScene.xml").Meshes[0];
            rueda.Scale = new Vector3(escala, escala, escala);
            //rueda.AutoTransformEnable = false;
            return rueda;
        }

        //public TgcScene CrearBosque()
        //{
        //    var BosqueScene = Loader.loadSceneFromFile(MediaDir + "Bosque\\bosque2-TgcScene.xml");
        //    foreach (TgcMesh m in BosqueScene.Meshes)
        //    {
        //        m.move(new Vector3(1100, -1, -1000));
        //        m.Scale = new Vector3(1.8f, 2, 2);
        //    }
        //    return BosqueScene;
        //}

        public void CrearLuces()
        {
            Gm.LucesLst = new List<LuzFija>();
            Vector3 abajo = new Vector3(0, -1, 0);
            string shaderDir = Gm.ShadersDir;

            Gm.LucesLst.Add(new LuzFija(new Vector3(0, 100, 0),abajo,3000f,shaderDir )) ;

            //luces lado1
            //float lado1a = 128.08f;
            //float lado1b = 852.2f;
            //float intensidad = 300f;
            //Gm.LucesLst.Add(new LuzFija(new Vector3(-460.44f, lado1a, lado1b), abajo, intensidad, shaderDir));
            //Gm.LucesLst.Add(new LuzFija(new Vector3(-769f, lado1a, lado1b), abajo, intensidad, shaderDir));
            //Gm.LucesLst.Add(new LuzFija(new Vector3(-15f, lado1a, lado1b), abajo, intensidad, shaderDir));
            //Gm.LucesLst.Add(new LuzFija(new Vector3(293f, lado1a, lado1b), abajo, intensidad, shaderDir));

        }
    }
}
