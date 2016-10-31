using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Fog;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Core.Utils;
using TGC.Group.Model;
using TGC.GroupoMs.Camaras;
using TGC.GrupoMs.Camara;

namespace TGC.GroupoMs.Model.efectos
{
    public class Niebla
    {
        private Effect effect;
        private TgcFog fog;

        private TgcSkyBox skyBox;

        public bool fogShader { get; set; }
        private TgcScene mapScene;

        public Niebla(GameModel gm)
        {
            effect = TgcShaders.loadEffect(gm.ShadersDir+ "TgcFogShader.fx");
            fog = new TgcFog();
            fog.Enabled = true;
            fog.StartDistance = 500f;
            fog.EndDistance = 2000f;
            fog.Density = 0.025f;
            fog.Color = Color.Gray;
            fogShader = false;
            skyBox = gm.SkyBox;
            mapScene = gm.MapScene;
            //ahora cargo todo en el efecto de directX
            
        }
        public void CargarCamara (TgcCamera camara)
        {
            ConfigurarDirectX(camara.Position);
            fogShader = true;
        }
        public void CargarCamara(TgcThirdPersonCamera camara)
        {
            ConfigurarDirectX(camara.Position);
            fogShader = true;
            Render();

        }

        public void Update(TgcCamera camara)
        {
            var camaraPosition = camara.Position;
            effect.SetValue("CameraPos", TgcParserUtils.vector3ToFloat4Array(camaraPosition));
        }

        private void ConfigurarDirectX(Vector3 camaraPosition)
        {
            effect.SetValue("ColorFog", fog.Color.ToArgb());
            effect.SetValue("CameraPos", TgcParserUtils.vector3ToFloat4Array(camaraPosition));
            effect.SetValue("StartFogDistance", fog.StartDistance);
            effect.SetValue("EndFogDistance", fog.EndDistance);
            effect.SetValue("Density", fog.Density);
        }

        /// <summary>
        /// seteo las tecnicas de render pero en realidad no rendereo nada aca.
        /// </summary>
        public void Render()
        {
            //primero el skybox
            foreach (var mesh in skyBox.Faces)
            {
                if (fogShader)
                {
                    mesh.Effect = effect;
                    mesh.Technique = "RenderScene";
                }
                else
                {
                    mesh.Effect = TgcShaders.Instance.TgcMeshShader;
                    mesh.Technique = "DIFFUSE_MAP";
                }
            }

            //ahora la ciudad
            foreach (var mesh in mapScene.Meshes)
            {
                if (fogShader)
                {
                    mesh.Effect = effect;
                    mesh.Technique = "RenderScene";
                }
                else
                {
                    mesh.Effect = TgcShaders.Instance.TgcMeshShader;
                    mesh.Technique = "DIFFUSE_MAP";
                }
                //mesh.UpdateMeshTransform();
                //mesh.render();
            }
        }
        
    }
}
