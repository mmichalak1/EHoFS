using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OurGame.Engine.Components
{
    [DataContract]
    class SkyboxRenderer : AbstractComponent
    {
        [DataMember]
        private string _skyboxModelName = "Skybox";
        [DataMember]
        private string _skyboxEffectName = "SkyboxEffect";

        private TextureCube skyboxTexture;
        private Effect skyboxEffect;
        private Model skyboxModel;

        public SkyboxRenderer(GameObject parent, string modelName, string material, bool usesDefaultNormals) : base(parent)
        {
            LoadContent();
        }

        public override void Initialize()
        {
            LoadContent();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentContainer.TexCube.TryGetValue(_skyboxModelName, out skyboxTexture);
            ContentContainer.Shaders.TryGetValue(_skyboxEffectName, out skyboxEffect);
            ContentContainer.Models.TryGetValue(_skyboxModelName, out skyboxModel);
        }

        public override void Draw(GameTime time)
        {
            skyboxEffect.Parameters["World"].SetValue(
                Matrix.CreateScale(10000f, 10000f, 10000f) *
                Matrix.CreateFromYawPitchRoll(0, 0, 0) *
                Matrix.CreateTranslation(Parent.Transform.Position));
            skyboxEffect.Parameters["View"].SetValue(CameraComponent.Main.View);
            skyboxEffect.Parameters["Projection"].SetValue(CameraComponent.Main.Projection);
            skyboxEffect.Parameters["CameraPosition"].SetValue(CameraComponent.Main.Parent.Transform.Position);
            skyboxEffect.Parameters["SkyboxTexture"].SetValue(skyboxTexture);
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = skyboxEffect;
                mesh.Draw();
            }
        }

        public override void Update(GameTime time)
        {
            
        }

    }
}
