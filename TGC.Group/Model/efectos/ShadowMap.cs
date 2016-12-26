using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Group.Model;
using TGC.Core.Direct3D;
using Microsoft.DirectX;
using TGC.Core.Geometry;
using System.Drawing;
using TGC.Core.Camara;

namespace TGC.GroupoMs.Model.efectos
{
   
    public class ShadowMap
    {
        private Effect effect;
        private GameModel gameModel;
        private TgcScene mapScene;

        private Vector3 g_LightDir; // direccion de la luz actual
        private Vector3 g_LightPos; // posicion de la luz actual (la que estoy analizando)
        private Matrix g_LightView; // matriz de view del light
        private Matrix g_mShadowProj; // Projection matrix for shadow map
        private Surface g_pDSShadow; // Depth-stencil buffer for rendering to shadow map
        private TgcArrow arrow; //de donde sale la luz

        private readonly int SHADOWMAP_SIZE = 1024;
        private readonly float far_plane = 1500f;
        private readonly float near_plane = 2f;

        
        private Texture g_pShadowMap; // Texture to which the shadow map is rendered
        private Vector3 lightLookAt;
        private Vector3 lightLookFrom;
        private TgcCamera Camara;
        private Surface pOldRT;
        private Surface pOldDS;

        public ShadowMap(GameModel gm)
        {
            effect = TgcShaders.loadEffect(gm.ShadersDir + "ShadowMap.fx");
            gameModel = gm;
            mapScene = gm.MapScene;
            Camara = gm.Camara;

            foreach (var m in mapScene.Meshes)
            {
                m.Effect = effect;
                m.Technique = "RenderShadow";
            }

            //--------------------------------------------------------------------------------------
            // Creo el shadowmap.
            // Format.R32F
            // Format.X8R8G8B8
            g_pShadowMap = new Texture(D3DDevice.Instance.Device, SHADOWMAP_SIZE, SHADOWMAP_SIZE,
                1, Usage.RenderTarget, Format.R32F,
                Pool.Default);

            // tengo que crear un stencilbuffer para el shadowmap manualmente
            // para asegurarme que tenga la el mismo tamano que el shadowmap, y que no tenga
            // multisample, etc etc.
            g_pDSShadow = D3DDevice.Instance.Device.CreateDepthStencilSurface(SHADOWMAP_SIZE,
                SHADOWMAP_SIZE,
                DepthFormat.D24S8,
                MultiSampleType.None,
                0,
                true);
            // por ultimo necesito una matriz de proyeccion para el shadowmap, ya
            // que voy a dibujar desde el pto de vista de la luz.
            // El angulo tiene que ser mayor a 45 para que la sombra no falle en los extremos del cono de luz
            // de hecho, un valor mayor a 90 todavia es mejor, porque hasta con 90 grados es muy dificil
            // lograr que los objetos del borde generen sombras
            var aspectRatio = D3DDevice.Instance.AspectRatio;
            g_mShadowProj = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(80), aspectRatio, 50, 5000);
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), aspectRatio, near_plane, far_plane);

            arrow = new TgcArrow();
            arrow.Thickness = 1f;
            arrow.HeadSize = new Vector2(2f, 2f);
            arrow.BodyColor = Color.Blue;

            //donde esta y a donde apunta la luz
            lightLookFrom = new Vector3(80, 120, 0);
            g_LightPos = lightLookFrom;
            lightLookAt = new Vector3(0, 0, 0);
            g_LightDir = lightLookAt;

            

            float K = 300;
        }

        public void Update(Vector3 lookDir,Vector3 pos)
        {
            arrow.PStart = g_LightPos;
            arrow.PEnd = g_LightPos + g_LightDir * 20;

        }

        public void Render(TgcScene scene)
        {
            D3DDevice.Instance.Device.BeginScene(); //BEGIN1================================================================
            // Calculo la matriz de view de la luz
            effect.SetValue("g_vLightPos", new Vector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            effect.SetValue("g_vLightDir", new Vector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
            g_LightView = Matrix.LookAtLH(g_LightPos, g_LightPos + g_LightDir, new Vector3(0, 0, 1));

            arrow.PStart = g_LightPos;
            arrow.PEnd = g_LightPos + g_LightDir * 20f;
            arrow.updateValues();
            D3DDevice.Instance.Device.EndScene(); //END1================================================================
            // inicializacion standard:
            effect.SetValue("g_mProjLight", g_mShadowProj);
            effect.SetValue("g_mViewLightProj", g_LightView * g_mShadowProj);

            // Primero genero el shadow map, para ello dibujo desde el pto de vista de luz
            // a una textura, con el VS y PS que generan un mapa de profundidades.
             pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            var pShadowSurf = g_pShadowMap.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, pShadowSurf);
             pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
            D3DDevice.Instance.Device.DepthStencilSurface = g_pDSShadow;
            //D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene(); //BEGIN2 ================================================================

            // Hago el render de la escena pp dicha
            effect.SetValue("g_txShadow", g_pShadowMap);

            foreach(TgcMesh M in mapScene.Meshes)
            {
                RenderMesh(M,true);

            }
            

            // Termino
            //D3DDevice.Instance.Device.EndScene();

            //TextureLoader.Save("shadowmap.bmp", ImageFileFormat.Bmp, g_pShadowMap);

            // restuaro el render target y el stencil
            //D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            //D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
            //arrow.render();
        }

        

        public void RestaurarStencil()
        {
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
        }

        public void RenderMesh(TgcMesh T, bool shadow)
        {
            if (shadow)
            {
                T.Technique = "RenderShadow";
            }
            else
            {
                T.Technique = "RenderScene";
            }

            T.render();
        }
    }
}
