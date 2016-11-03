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

namespace TGC.GroupoMs.Model.efectos
{
    public class LuzFija
    {
        public TgcThirdPersonCamera Camara;
        public TgcScene Scene;
        public bool lightEnable;
        public Vector3 lightPos;
        public Color lightColor;
        public float lightIntensity;
        public float specularEx;
        public float spotAngle;
        public float spotExponent;
        public Vector3 lightDir;
        public Vector3 CamaraPos;

        public LuzFija(Vector3 pos, Vector3 dir)
        {
            lightEnable = true;
            lightPos = pos;
            lightDir = dir;
            lightDir.Normalize();
            lightColor = Color.White;
            lightIntensity = 35f;
            specularEx = 9f;
            spotAngle = 36f;
            spotExponent = 7f;
        }

        public void setCamara(TgcThirdPersonCamera cam)
        {
            CamaraPos = cam.Position;
        }

        public void setValues(TgcMesh mesh,Vector3 posicionCamara)
        {
            Effect currentShader;
            
            currentShader = TgcShaders.Instance.TgcMeshSpotLightShader;
            mesh.Effect = currentShader;
            var direccionLuz = lightDir;

            mesh.Technique = "DIFFUSE_MAP";//TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
            mesh.Effect = TgcShaders.Instance.TgcMeshSpotLightShader;
            //El Technique depende del tipo RenderType del mesh
            mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
            mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.Violet));
            mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lightPos));
            mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(posicionCamara));
            mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat3Array(direccionLuz));
            mesh.Effect.SetValue("lightIntensity", lightIntensity);
            mesh.Effect.SetValue("lightAttenuation", 0.3f);
            mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(45f));
            mesh.Effect.SetValue("spotLightExponent", 20f);
            mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Gray));
            mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            mesh.Effect.SetValue("materialSpecularExp", 10f);
        }
        //public void AplicarEfecto(Vector3 posicionCamara)
        //{
        //    Effect currentShader;
        //    currentShader = TgcShaders.Instance.TgcMeshSpotLightShader;
        //    foreach (var mesh in Scene.Meshes)
        //    {
        //        mesh.Effect = currentShader;
                

        //        //El Technique depende del tipo RenderType del mesh

        //    }
        //    lightMesh.Position = lightPos;
            
        //}

        
    }
}
