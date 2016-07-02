using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OurGame.Engine.Lights;
using OurGame.Engine.Utilities;
using OurGame.Engine.Components;

namespace OurGame.Engine.Statics
{
    public class DeferredRenderer
    {
        //Singleton stuff
        private static DeferredRenderer _instance;
        public static DeferredRenderer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DeferredRenderer();
                return _instance;
            }
        }
        private DeferredRenderer()
        {

        }

        #region fields
        private Vector4 _ambientColor = Color.White.ToVector4();
        private float _ambientIntensity = 0.33f;
        private QuadRender _renderer;
        private SpriteBatch _spriteBatch;
        private ModificationIndicator _indicator;

        private RenderTarget2D _color;
        private RenderTarget2D _normal;
        private RenderTarget2D _lights;
        private RenderTarget2D _depth;
        private RenderTarget2D _glowMap;
        private RenderTarget2D _output;
        private RenderTarget2D _postEffect1;
        private RenderTarget2D _postEffect2;
        private RenderTarget2D _forwardColor;
        private RenderTarget2D _forwardDepth;
        private RenderTarget2D _small;
        private RenderTarget2D _fxaa;

        private GraphicsDevice _device;

        private Effect _clearRenderer;
        private Effect _directionalLight;
        private Effect _finalEffect;
        private Effect _blurEffect;
        private Effect _bloomEffect;
        private Effect _transparentEffect;
        //private Effect _fxaaEffect;

        #region BlurStuff
        Vector2[] HorizontalOffset;
        Vector2[] VerticalOffset;
        float[] weights;
        #endregion
        #endregion

        #region Preset Functions
        public void Initialize(GraphicsDevice device)
        {
            _device = device;
            _renderer = new QuadRender(device);
            _spriteBatch = new SpriteBatch(device);
            _color = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _normal = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _lights = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _depth = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _glowMap = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _output = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _postEffect1 = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _postEffect2 = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _forwardColor = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _forwardDepth = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _small = new RenderTarget2D(device, device.Viewport.Width/2, device.Viewport.Height/2, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _fxaa = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false,
                SurfaceFormat.Color, DepthFormat.Depth24);

            PrepareBlurFunction();
        }
        public void LoadContent(ContentManager content)
        {
            _clearRenderer = ContentContainer.Shaders["ClearBuffer"];
            _directionalLight = ContentContainer.Shaders["DirectionalLight"];
            _finalEffect = ContentContainer.Shaders["CombineFinal"];
            _blurEffect = ContentContainer.Shaders["BlurEffect"];
            _bloomEffect = ContentContainer.Shaders["BloomEffect"];
            _transparentEffect = ContentContainer.Shaders["MixTransparent"];
            //_fxaaEffect = ContentContainer.Shaders["FxaaEffect"];

            _directionalLight.Parameters["AmbientColor"].SetValue(_ambientColor);
            _directionalLight.Parameters["AmbientIntensity"].SetValue(_ambientIntensity);

            _blurEffect.Parameters["SampleWeights"].SetValue(weights);
            ParticleSystem.ParticleEmiter.Instance.Initialize(_device);
            _indicator = new ModificationIndicator(_device);

        }
        private void PrepareBlurFunction()
        {
            int radius = 7;
            weights = new float[15]; //2x radius + 1
            float sigma = radius / 0.0001f;
            float twoSigmaSquare = 2.0f * sigma * sigma;
            float sigmaRoot = (float)Math.Sqrt(twoSigmaSquare * Math.PI);
            float total = 0.0f;
            float distance = 0.0f;
            int index = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                distance = i * i;
                index = i + radius;
                weights[index] = (float)Math.Exp(-distance / twoSigmaSquare) / sigmaRoot;
                total += weights[index];
            }

            for (int i = 0; i < weights.Length; ++i)
                weights[i] /= total;

            HorizontalOffset = new Vector2[15];
            VerticalOffset = new Vector2[15];

            float xOff = 1.0f / _device.Viewport.Width;
            float yOff = 1.0f / _device.Viewport.Height;

            for (int i = -radius; i <= radius; ++i)
            {
                index = i + radius;
                HorizontalOffset[index] = new Vector2(i * xOff, 0.0f);
                VerticalOffset[index] = new Vector2(0.0f, i * yOff);
            }
        }

        #endregion

        #region Buffer Helpers
        public void SetBuffer()
        {
            _device.SetRenderTargets(_color, _normal, _depth, _glowMap);
        }
        public void ResolveBuffer()
        {
            _device.SetRenderTargets(null);
        }
        public void ClearBuffer()
        {
            _device.SetRenderTargets(null);
            _device.SetRenderTargets(_color, _normal, _depth, _glowMap);
            _device.BlendState = BlendState.Opaque;
            _device.DepthStencilState = DepthStencilState.Default;
            _device.RasterizerState = RasterizerState.CullCounterClockwise;
            _clearRenderer.Techniques[0].Passes[0].Apply();
            _renderer.Draw(_device);
            _device.SetRenderTargets(_postEffect1, _postEffect2, _forwardDepth, _output);
            _renderer.Draw(_device);
            _device.SetRenderTargets(null);
        }
        #endregion

        public void Draw(SpriteBatch batch, GameTime time)
        {
            ClearBuffer();
            SetBuffer();
            ScreenManager.Instance.CurrentScreen.Draw(time);
            ResolveBuffer();
            int halfWidth = _device.Viewport.Width / 2;
            int halfHeight = _device.Viewport.Height / 2;
            DrawLights(ScreenManager.Instance.CurrentScreen.PointLights, ScreenManager.Instance.CurrentScreen.DirectionalLights);

            //ClearTarget(_fxaa);
            //_device.SetRenderTarget(_fxaa);
            //_fxaaEffect.Parameters["texture"].SetValue(_color);
            //_fxaaEffect.CurrentTechnique.Passes[0].Apply();
            //_renderer.Draw(_device);
            //_device.SetRenderTarget(null);

            ApplyFinalEffect(_lights, _postEffect1);

            ApplyPostEffect();

            _device.SetRenderTargets(_forwardColor, _forwardDepth);
            _device.Clear(Color.Transparent);
            _device.BlendState = BlendState.Opaque;
            _device.DepthStencilState = DepthStencilState.Default;
            foreach (var item in ScreenManager.Instance.CurrentScreen.GameObjectList)
            {
                item.DrawTransparent();
            }
            _device.SetRenderTarget(null);
            _transparentEffect.Parameters["FinalMap"].SetValue(_color);
            _transparentEffect.Parameters["TransparentMap"].SetValue(_forwardColor);
            _transparentEffect.Parameters["SourceDepth"].SetValue(_depth);
            _transparentEffect.Parameters["TransparentDepth"].SetValue(_forwardDepth);
            _transparentEffect.CurrentTechnique.Passes[0].Apply();
            _renderer.Draw(_device);

            _device.DepthStencilState = DepthStencilState.None;
            _device.RasterizerState = RasterizerState.CullNone;
            _device.BlendState=BlendState.AlphaBlend;
            ParticleSystem.ParticleEmiter.Instance.Draw(CameraComponent.Main, _device);
            _device.RasterizerState = RasterizerState.CullCounterClockwise;


            //batch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);
            //batch.Draw(_color, new Rectangle(0, 0, halfWidth, halfHeight), Color.White); //top left
            //batch.Draw(_forwardDepth, new Rectangle(0, halfHeight, halfWidth, halfHeight), Color.White); //bottom left
            //batch.Draw(_forwardColor, new Rectangle(halfWidth, 0, halfWidth, halfHeight), Color.White); //top right
            //batch.Draw(_fxaa, new Rectangle(halfWidth, halfHeight, halfWidth, halfHeight), Color.White); //bottom right
            //batch.End();
            DrawMessage(time);
            DrawDot(time);
            DrawBossHealthBar(time);
            if(Gun.Instance.gunActive)
            {
                Gun.Instance.Update(time);
                DrawGun(time);
            }
            _indicator.Draw(_spriteBatch);
        }

        public void DrawDot(GameTime time)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(ContentContainer.TexColor["crosshair"], new Rectangle((_device.Viewport.Width / 2) - 25, (_device.Viewport.Height / 2) - 15, 25, 25), Color.White);
            _spriteBatch.End();
        }
        public void DrawMessage(GameTime time)
        {
            OnScreenMessage.Instance.Draw(time);
        }

        public void DrawBossHealthBar(GameTime time)
        {
            HealthBar.Instance.Draw(time);
        }

        public void DrawGun(GameTime time)
        {
            Gun.Instance.Draw(time);
        }

        private void DrawLights(List<PointLight> pLights, List<Lights.DirectionalLight> dLights)
        {
            _device.SetRenderTarget(_lights);
            _device.BlendState = BlendState.Opaque;
            _device.Clear(Color.Transparent);
            foreach (Lights.DirectionalLight dLight in dLights)
                DrawDirectional(dLight.Direction, dLight.Color);
            _device.SetRenderTarget(null);
        }

        #region Lights
        private void DrawDirectional(Vector3 direction, Color color)
        {
            _directionalLight.Parameters["lightDirection"].SetValue(direction);
            _directionalLight.Parameters["Color"].SetValue(color.ToVector3());
            _directionalLight.Parameters["cameraPosition"].SetValue(CameraComponent.Main.Parent.Transform.Position);
            _directionalLight.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(
                CameraComponent.Main.View * CameraComponent.Main.Projection));
            _directionalLight.Parameters["ColorMap"].SetValue(_color);
            _directionalLight.Parameters["NormalMap"].SetValue(_normal);
            _directionalLight.Parameters["DepthMap"].SetValue(_depth);
            _directionalLight.CurrentTechnique.Passes[0].Apply();
            _renderer.Draw(_device);
        }

        #endregion

        private void ApplyPostEffect()
        {

            ApplyBlurEffect(_glowMap, _small, VerticalOffset);
            ApplyBlurEffect(_glowMap, _small, HorizontalOffset);
            ClearTarget(_glowMap);
            ApplyBlurEffect(_small, _glowMap, VerticalOffset);
            ApplyBlurEffect(_small, _glowMap, HorizontalOffset);
            ApplyBloomEffect(_glowMap, _color);

        }
        private void ApplyBlurEffect(RenderTarget2D source, RenderTarget2D target, Vector2[] offsets)
        {
            _device.SetRenderTarget(target);
            _blurEffect.Parameters["SampleOffsets"].SetValue(offsets);
            _blurEffect.Parameters["shaderTexture"].SetValue(source);
            _blurEffect.CurrentTechnique.Passes[0].Apply();
            _renderer.Draw(_device);
            _device.SetRenderTarget(null);
        }

        private void ApplyBloomEffect(RenderTarget2D source, RenderTarget2D target)
        {
            ClearTarget(target);
            _device.SetRenderTarget(target);
            _bloomEffect.Parameters["GlowMap"].SetValue(source);
            _bloomEffect.Parameters["LightMap"].SetValue(_postEffect1);
            _bloomEffect.CurrentTechnique.Passes[0].Apply();
            _renderer.Draw(_device);
            _device.SetRenderTarget(null);
        }

        private void ApplyFinalEffect(RenderTarget2D source, RenderTarget2D target)
        {
            _device.SetRenderTarget(target);
            _device.BlendState = BlendState.Opaque;
            _finalEffect.Parameters["ColorMap"].SetValue(_color);
            _finalEffect.Parameters["LightMap"].SetValue(source);
            _finalEffect.Techniques[0].Passes[0].Apply();
            _renderer.Draw(_device);
            _device.SetRenderTarget(null);
        }

        private void ClearTarget(RenderTarget2D target)
        {
            _device.BlendState = BlendState.Opaque;
            _device.DepthStencilState = DepthStencilState.Default;
            _device.SetRenderTarget(target);
            _clearRenderer.CurrentTechnique.Passes[0].Apply();
            _renderer.Draw(_device);
            _device.SetRenderTarget(null);
        }



    }
}
