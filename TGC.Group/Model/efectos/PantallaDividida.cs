using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;
using TGC.Group.Model;
using TGC.Core.Shaders;

//using TGC.Examples.Camara;
//using TGC.Examples.Example;

namespace TGC.GroupoMs.Model.efectos
{
    public class PantallaDividida
    {
        public Effect effect;
        public Surface g_pDepthStencil; // Depth-stencil buffer
        public Texture g_pRenderTarget;
        public VertexBuffer g_pVBV3D;
        public List<TgcMesh> meshes;
        public string MyShaderDir;
        public float time;
        public GameModel gm;
        //-------------------------------

        public Device device;
        public bool activar_efecto;
        public Surface pOldRT;
        public Surface pSurf;
        public Surface pOldDS;

        public PantallaDividida(GameModel gm)
        {
            var d3dDevice = D3DDevice.Instance.Device;
            //effect = TgcShaders.loadEffect(gm.ShadersDir + "ShadowMap.fx");
            //Cargar Shader personalizado
            string compilationErrors;
            effect = Effect.FromFile(d3dDevice, gm.ShadersDir + "PantallaDividida.fx", null, null, ShaderFlags.PreferFlowControl,
                null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }

            effect.Technique = "DefaultTechnique";

            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth,
               d3dDevice.PresentationParameters.BackBufferHeight,
               DepthFormat.D24S8, MultiSampleType.None, 0, true);

            // inicializo el render target
            g_pRenderTarget = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8,
                Pool.Default);

            effect.SetValue("g_RenderTarget", g_pRenderTarget);

            // Resolucion de pantalla
            effect.SetValue("screen_dx", d3dDevice.PresentationParameters.BackBufferWidth);
            effect.SetValue("screen_dy", d3dDevice.PresentationParameters.BackBufferHeight);

            CustomVertex.PositionTextured[] vertices =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };
            //vertex buffer de los triangulos
            g_pVBV3D = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                CustomVertex.PositionTextured.Format, Pool.Default);
            g_pVBV3D.SetData(vertices, 0, LockFlags.None);
        }
        /*
        public override void Init()
        {
            time = 0;

            var d3dDevice = D3DDevice.Instance.Device;
            MyShaderDir = ShadersDir + "WorkshopShaders\\";

            //Cargamos un escenario
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Scenes\\Deposito\\Deposito-TgcScene.xml");
            meshes = scene.Meshes;

            //Cargar Shader personalizado
            string compilationErrors;
            effect = Effect.FromFile(d3dDevice, MyShaderDir + "FullQuad.fx", null, null, ShaderFlags.PreferFlowControl,
                null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }
            //Configurar Technique dentro del shader
            effect.Technique = "DefaultTechnique";

            //Camara en primera persona
            Camara = new TgcFpsCamera(new Vector3(250, 160, -570), Input);

            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth,
                d3dDevice.PresentationParameters.BackBufferHeight,
                DepthFormat.D24S8, MultiSampleType.None, 0, true);

            // inicializo el render target
            g_pRenderTarget = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8,
                Pool.Default);

            effect.SetValue("g_RenderTarget", g_pRenderTarget);

            // Resolucion de pantalla
            effect.SetValue("screen_dx", d3dDevice.PresentationParameters.BackBufferWidth);
            effect.SetValue("screen_dy", d3dDevice.PresentationParameters.BackBufferHeight);

            CustomVertex.PositionTextured[] vertices =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };
            //vertex buffer de los triangulos
            g_pVBV3D = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                CustomVertex.PositionTextured.Format, Pool.Default);
            g_pVBV3D.SetData(vertices, 0, LockFlags.None);

            Modifiers.addBoolean("activar_efecto", "Activar efecto", true);
        }
        
        public override void Update()
        {
            PreUpdate();
        }
        */
        public void Render1()
        {
            //ClearTextures();

            device = D3DDevice.Instance.Device;

            //time += gm.ElapsedTime;
            activar_efecto = true;

            //Cargar variables de shader

            // dibujo la escena una textura
            effect.Technique = "DefaultTechnique";
            // guardo el Render target anterior y seteo la textura como render target
            pOldRT = device.GetRenderTarget(0);
            pSurf = g_pRenderTarget.GetSurfaceLevel(0);
            if (activar_efecto)
                device.SetRenderTarget(0, pSurf);
            // hago lo mismo con el depthbuffer, necesito el que no tiene multisampling
            pOldDS = device.DepthStencilSurface;
            // Probar de comentar esta linea, para ver como se produce el fallo en el ztest
            // por no soportar usualmente el multisampling en el render to texture.
            if (activar_efecto)
                device.DepthStencilSurface = g_pDepthStencil;

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);


            device.BeginScene();


            //Dibujamos todos los meshes del escenario
            /*
            foreach (var m in meshes)
            {
                m.render();
            }*/

            device.EndScene();

            pSurf.Dispose();

            if (activar_efecto)
            {
                // restuaro el render target y el stencil
                device.DepthStencilSurface = pOldDS;
                device.SetRenderTarget(0, pOldRT);

                // dibujo el quad pp dicho :
                device.BeginScene();

                effect.Technique = "PostProcess";
                effect.SetValue("time", time);
                device.VertexFormat = CustomVertex.PositionTextured.Format;
                device.SetStreamSource(0, g_pVBV3D, 0);
                effect.SetValue("g_RenderTarget", g_pRenderTarget);

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                effect.Begin(FX.None);
                effect.BeginPass(0);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                effect.EndPass();
                effect.End();

                device.EndScene();
            }

            device.BeginScene();

            device.EndScene();
            device.Present();
        }

        public void Dispose()
        {

            effect.Dispose();
            g_pRenderTarget.Dispose();
            g_pVBV3D.Dispose();
            g_pDepthStencil.Dispose();
        }

    }
}
