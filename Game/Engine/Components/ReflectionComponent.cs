using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OurGame.Engine.Components
{
    [DataContract]
    public class ReflectionComponent : AbstractComponent
    {
        RenderTargetCube target;
        TextureCube CUBE;
        Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)16 / 9, 10f, 10000f);
        Matrix[] viewMatricies = new Matrix[6];
        RenderTarget2D[] renderTargets = new RenderTarget2D[6];
        Effect skyboxEffect;
        BasicEffect effect = new BasicEffect(OurGame.Game.GraphicsDevice);
        Model myModel;
        [DataMember]
        string modelName;
        int size = 1024;

        public ReflectionComponent(GameObject parent, string colorTextureName, string modelName) : base(parent)
        {
            target = new RenderTargetCube(OurGame.Game.GraphicsDevice, 256, false, SurfaceFormat.Color, DepthFormat.None);
            this.modelName = modelName;
            Initialize();
        }

        public override void Initialize()
        {
            Vector3 direction = new Vector3(0f, 0f, -10f);
            var pos = Vector3.Transform(Parent.Transform.Position, Parent.Transform.Rotation);
            CUBE = new TextureCube(OurGame.Game.GraphicsDevice, size, false, SurfaceFormat.Color);
            viewMatricies[4] = Matrix.CreateLookAt(Parent.Transform.Position, direction, Vector3.Up);
            viewMatricies[0] = Matrix.CreateLookAt(Parent.Transform.Position, Vector3.Transform(pos, Matrix.CreateFromYawPitchRoll(90, 0, 0)), Vector3.Up);
            viewMatricies[5] = Matrix.CreateLookAt(Parent.Transform.Position, Vector3.Transform(pos, Matrix.CreateFromYawPitchRoll(180, 0, 0)), Vector3.Up);
            viewMatricies[1] = Matrix.CreateLookAt(Parent.Transform.Position, Vector3.Transform(pos, Matrix.CreateFromYawPitchRoll(-90, 0, 0)), Vector3.Up);
            viewMatricies[2] = Matrix.CreateLookAt(Parent.Transform.Position, Vector3.Transform(pos, Matrix.CreateFromYawPitchRoll(0, 90, 0)), Vector3.Up);
            viewMatricies[3] = Matrix.CreateLookAt(Parent.Transform.Position, Vector3.Transform(pos, Matrix.CreateFromYawPitchRoll(0, -90, 0)), Vector3.Up);
            for (int i = 0; i < renderTargets.Length; i++)
                renderTargets[i] = new RenderTarget2D(OurGame.Game.GraphicsDevice, size, size, false, SurfaceFormat.Color, DepthFormat.None);
            LoadContent();
        }

        public override void Update(GameTime time)
        {

        }

        protected override void LoadContent()
        {
            ContentContainer.Shaders.TryGetValue("SkyboxEffect", out skyboxEffect);
            ContentContainer.Models.TryGetValue("DeathStar", out myModel);
            ContentContainer.TexCube.TryGetValue("Skybox", out CUBE);
            base.LoadContent();
        }


        public override void Draw(GameTime time)
        {
            //Color[] temp = new Color[size * size];
            //renderTargets[0].GetData(temp);
            //CUBE.SetData(CubeMapFace.NegativeZ, temp);
            //renderTargets[1].GetData(temp);
            //CUBE.SetData(CubeMapFace.NegativeX, temp);
            //renderTargets[2].GetData(temp);
            //CUBE.SetData(CubeMapFace.PositiveZ, temp);
            //renderTargets[3].GetData(temp);
            //CUBE.SetData(CubeMapFace.PositiveX, temp);
            //renderTargets[4].GetData(temp);
            //CUBE.SetData(CubeMapFace.NegativeY, temp);
            //renderTargets[5].GetData(temp);
            //CUBE.SetData(CubeMapFace.PositiveY, temp);
            OurGame.Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            skyboxEffect.Parameters["World"].SetValue(
                Matrix.CreateScale(1f,1,1f) *
                Matrix.CreateFromQuaternion(Parent.Transform.Rotation) *
                Matrix.CreateTranslation(Parent.Transform.Position));
            skyboxEffect.Parameters["View"].SetValue(CameraComponent.Main.View);
            skyboxEffect.Parameters["Projection"].SetValue(CameraComponent.Main.Projection);
            skyboxEffect.Parameters["CameraPosition"].SetValue(CameraComponent.Main.Parent.Transform.Position);
            skyboxEffect.Parameters["SkyboxTexture"].SetValue(target);
            skyboxEffect.Parameters["isReflecting"].SetValue(true);
            //effect.World = Matrix.CreateScale(50, 50, 50) *
            //    Matrix.CreateFromYawPitchRoll(0, 0, 0) *
            //    Matrix.CreateTranslation(Parent.Transform.Position);
            //effect.View = CameraComponent.Main.View;
            //effect.Projection = CameraComponent.Main.Projection;
            //effect.TextureEnabled = true;
            //effect.Texture = renderTargets[0] as Texture2D;
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = skyboxEffect;
                mesh.Draw();
            }
            base.Draw(time);
            OurGame.Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        public override void DrawReflective(GraphicsDevice device)
        {

            for (int i = 0; i < 6; i++)
            {
                CubeMapFace face = (CubeMapFace)i;
                OurGame.Game.GraphicsDevice.SetRenderTarget(target, (CubeMapFace)i);
                OurGame.Game.GraphicsDevice.Clear(Color.CornflowerBlue);
                ScreenManager.Instance.CurrentScreen.DrawColor(viewMatricies[i], projectionMatrix);
                //using (System.IO.Stream stream = new System.IO.FileStream("pic" + i + ".png", System.IO.FileMode.CreateNew))
                //{
                //    renderTargets[i].SaveAsPng(stream, size, size);
                //}
            }
            device.SetRenderTarget(null);
        }
    }
}
