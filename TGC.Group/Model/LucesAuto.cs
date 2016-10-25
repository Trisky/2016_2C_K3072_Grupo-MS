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
using TGC.GrupoMs.Camara;

namespace TGC.GroupoMs.Model
{
    public class LucesAuto
    {
        private TgcBox lightMesh;
        private Effect currentShader;

        private List<TgcMesh> lstMeshes;
        private TgcThirdPersonCamera camara;


        public LucesAuto(TgcScene scene, TgcMesh auto, Ruedas r1, Ruedas r2, TgcThirdPersonCamera cam)
        {
            lightMesh = TgcBox.fromSize(new Vector3(10, 10, 10), Color.Red);
            lstMeshes = new List<TgcMesh>();
            foreach(var mesh in scene.Meshes)
            {
                lstMeshes.Add(mesh);
            }
            lstMeshes.Add(auto);

            lstMeshes.Add(r1.RuedaMeshDer);
            lstMeshes.Add(r1.RuedaMeshIzq);
            lstMeshes.Add(r2.RuedaMeshIzq);
            lstMeshes.Add(r2.RuedaMeshDer);


            camara = cam;


            currentShader = TgcShaders.Instance.TgcMeshSpotLightShader;


        }
        public void Update()
        {
            foreach (var mesh in lstMeshes)
            {
                mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
                
            }
            ProcesarLuces();
        }

        /// <summary>
        /// Este render debe realizarse despues de renderizar el mapScene!
        /// </summary>
        public void Render()
        {
            lightMesh.render();
        }
        
        public void ProcesarLuces()
        
        {
            if (camara == null) return;
            var lightDir = new Vector3(20, 0, 0);
            lightDir.Normalize();
            foreach (var mesh in lstMeshes)
            {
                mesh.Effect.SetValue("lightColor", ColorValue.FromColor((Color.White)));
                mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToVector4(new Vector3(0,30,0)));
                mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camara.Position));
                mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat3Array(lightDir));
                mesh.Effect.SetValue("lightIntensity", 200f);
                mesh.Effect.SetValue("lightAttenuation", 0.1f);
                mesh.Effect.SetValue("spotLightAngleCos", 55);
                mesh.Effect.SetValue("spotLightExponent", 7f);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor((Color.White)));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor((Color.White)));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor((Color.White)));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor((Color.White)));
                mesh.Effect.SetValue("materialSpecularExp",1f);

                
            }
            
        }
    }
}
