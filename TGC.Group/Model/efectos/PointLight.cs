using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Utils;

namespace TGC.GroupoMs.Model.efectos
{
    class PointLight
    {

        Effect currentShader;
        TgcScene scene;


        public PointLight(TgcScene sc)
        {
            scene = sc;
            Init();
        }
        private void Init()
        {
            currentShader = TgcShaders.Instance.TgcMeshShader;

            Vector3 posLuz = new Vector3(100, 100, 100);

            foreach(TgcMesh mesh in scene.Meshes)
            {
                mesh.Effect = currentShader;
                mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(posLuz));
                
                mesh.Effect.SetValue("lightIntensity", 20f);
                mesh.Effect.SetValue("lightAttenuation",0.3f);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularExp", 9f);
            }

        }
        public void Update(Vector3 camPos)
        {
            foreach (TgcMesh mesh in scene.Meshes)
            {
                mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camPos));
            }

        }
    }
}
