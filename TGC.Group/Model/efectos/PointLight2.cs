using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Utils;
using TGC.Group.Model;

namespace TGC.GroupoMs.Model.efectos
{
    public class PointLight2
    {
        private GameModel gameModel;
        private Vector3 lighthPos;
        private TgcBox lightMesh;
        private Effect currentShader;

        public PointLight2(GameModel gm,Vector3 Posicion)
        {
            gameModel = gm;
            lighthPos = Posicion;
            float t = 10f;
            lightMesh = TgcBox.fromSize(lighthPos, new Vector3(t,t,t),Color.White);
            lightMesh.updateValues();
            currentShader = TgcShaders.Instance.TgcMeshPointLightShader;
        }

        public void Update()
        {
            float dir = 1f;
            float velocidad = 1f;// * gameModel.ElapsedTime;
            float posY = lighthPos.Y;

            if (posY < -500f || posY > 500f)
                dir = dir * -1f;
            
            lighthPos.Y += velocidad*dir;
            lightMesh.updateValues();
        }

        public void render(List<TgcMesh> meshes,Vector3 camPos)
        {
            //Update();
            foreach (var mesh in meshes)
            {
                //1 seteo efecto
                mesh.Effect = currentShader;
                mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);

                //2seteo valores
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularExp", 9f);

                //3 seteo variables

                mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lighthPos));
                mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camPos));
                mesh.Effect.SetValue("lightIntensity", 30f);
                mesh.Effect.SetValue("lightAttenuation", 0.2f);

                lightMesh.render();
            }
        }
    }
}
