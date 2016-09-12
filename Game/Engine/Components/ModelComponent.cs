using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using OurGame.Engine.Utilities;
using OurGame.Scripts.Player;

namespace OurGame.Engine.Components
{
    [DataContract(IsReference = true)]
    public class ModelComponent : AbstractComponent
    {

        [DataMember]
        private bool _usesDefaultNormals;
        [DataMember]
        private string _modelName;
        [DataMember]
        private string _effectName = "RenderBuffer";
        [DataMember]
        private string _materialName;
        [DataMember]
        private Vector4 _emissiveColorChanger = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        [DataMember]
        private Vector4 _diffuseColorChanger = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        private Material _material;
        private Effect _effect;
        private Model _model;
        BasicEffect effect;

        private Texture2D _diffuseMap, _specularMap, _normalMap, _emissiveMap;

        public Texture2D DiffuseMap
        {
            get { return _diffuseMap; }
            set { _diffuseMap = value; }
        }
        public Texture2D EmissiveMap
        {
            get { return _emissiveMap; }
            set
            {
                _emissiveMap = null;
                _emissiveMap = value;
            }
        }

        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }

        public Model Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public Vector4 EmissiveColorChanger
        {
            get { return _emissiveColorChanger; }
            set { _emissiveColorChanger = value; }
        }

        public Vector4 DiffuseColorChanger
        {
            get { return _diffuseColorChanger; }
            set { _diffuseColorChanger = value; }
        }


        public ModelComponent(GameObject parent, string modelName, string material, bool usesDefaultNormals) : base(parent)
        {
            _modelName = modelName;
            _usesDefaultNormals = usesDefaultNormals;
            _materialName = material;
            LoadContent();
        }

        public override void Remove()
        {
            _model = null;
            _diffuseMap = null;
            _emissiveMap = null;
            _effect = null;
            _material = null;
            _specularMap = null;
            _normalMap = null;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            effect = new BasicEffect(OurGame.Game.GraphicsDevice);
            ContentContainer.Models.TryGetValue(_modelName, out _model);
            if (_diffuseMap == null)
                ContentContainer.TexColor.TryGetValue(_modelName, out _diffuseMap);

            if (_usesDefaultNormals)
            {

                ContentContainer.TexSpecular.TryGetValue("null", out _specularMap);
                ContentContainer.TexNormal.TryGetValue("null", out _normalMap);
            }
            else

            {
                ContentContainer.TexSpecular.TryGetValue(_modelName, out _specularMap);
                ContentContainer.TexNormal.TryGetValue(_modelName, out _normalMap);
            }

            if (_materialName == null)
                ContentContainer.Materials.TryGetValue("Default", out _material);
            else
                ContentContainer.Materials.TryGetValue(_materialName, out _material);

            if (_material.IsEmissive)
                ContentContainer.TexEmissive.TryGetValue(_modelName, out _emissiveMap);

            if (_material.IsReflective)
                _effect = ContentContainer.Shaders["ForwardRender"];
            if (!Material.IsReflective)
                ContentContainer.Shaders.TryGetValue(_effectName, out _effect);
            effect = new BasicEffect(OurGame.Game.GraphicsDevice);
        }

        public override void Update(GameTime time)
        {

        }

        public override void Draw(GameTime time)
        {
            if (!Material.IsReflective)
                DrawToRender();
        }

        private void DrawToRender()
        {
            if (!Material.IsReflective)
            {
                foreach (ModelMesh mesh in _model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = _effect;
                        part.Effect.Parameters["emissionPower"].SetValue(_material.EmissionPower);
                        part.Effect.Parameters["specularIntensity"].SetValue(_material.Shininess);
                        part.Effect.Parameters["specularPower"].SetValue(_material.ShiningPower);
                        part.Effect.Parameters["View"].SetValue(CameraComponent.Main.View);
                        part.Effect.Parameters["FarFrustrum"].SetValue(CameraComponent.Main.FarFrustrum);
                        if (Parent.Tag == "Weapon" && CameraComponent.Main.Parent.GetComponentOfType<ScriptComponent>().GetScriptOfType<Wielding>().Weapon != null)
                            part.Effect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, (float)16 / 9, 1f, 100000f));
                        else
                            part.Effect.Parameters["Projection"].SetValue(CameraComponent.Main.Projection);
                        part.Effect.Parameters["World"].SetValue(
                           Matrix.CreateScale(
                                Parent.Transform.Scale.X,
                                Parent.Transform.Scale.Y,
                                Parent.Transform.Scale.Z) *
                           Matrix.CreateFromQuaternion(Parent.Transform.Rotation) *
                           Matrix.CreateTranslation(Parent.Transform.Position));
                        part.Effect.Parameters["ColorMap"].SetValue(_diffuseMap);
                        part.Effect.Parameters["SpecularMap"].SetValue(_specularMap);
                        part.Effect.Parameters["NormalMap"].SetValue(_normalMap);
                        part.Effect.Parameters["EmissionMap"].SetValue(_emissiveMap);
                        part.Effect.Parameters["EmissiveColorChanger"].SetValue(_emissiveColorChanger);
                        part.Effect.Parameters["DiffuseColorChanger"].SetValue(_diffuseColorChanger);
                        part.Effect.CurrentTechnique = _effect.Techniques[0];
                    }
                    mesh.Draw();
                }
            }
        }

        public override void DrawTransparent()
        {
            if (Material.IsReflective)
            {
                foreach (ModelMesh mesh in _model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = _effect;
                        part.Effect.Parameters["World"].SetValue(Matrix.CreateScale(
                                Parent.Transform.Scale.X,
                                Parent.Transform.Scale.Y,
                                Parent.Transform.Scale.Z) *
                           Matrix.CreateFromQuaternion(Parent.Transform.Rotation) *
                           Matrix.CreateTranslation(Parent.Transform.Position));
                        part.Effect.Parameters["FarFrustrum"].SetValue(CameraComponent.Main.FarFrustrum);
                        part.Effect.Parameters["View"].SetValue(CameraComponent.Main.View);
                        part.Effect.Parameters["Projection"].SetValue(CameraComponent.Main.Projection);
                        part.Effect.Parameters["ShaderTexture"].SetValue(_diffuseMap);
                    }
                    mesh.Draw();
                }
            }
        }

        public override void DrawColor(Matrix view, Matrix projection)
        {
            foreach (var mesh in Model.Meshes)
            {

                effect.World =
                       Matrix.CreateScale(
                            Parent.Transform.Scale.X,
                            Parent.Transform.Scale.Y,
                            Parent.Transform.Scale.Z) *
                       Matrix.CreateFromQuaternion(Parent.Transform.Rotation) *
                       Matrix.CreateTranslation(Parent.Transform.Position);
                effect.View = view;
                effect.Projection = projection;
                effect.TextureEnabled = true;
                effect.Texture = _diffuseMap;
                mesh.Effects[0].CurrentTechnique = effect.CurrentTechnique;
                effect.CurrentTechnique.Passes[0].Apply();
                mesh.Draw();
            }

        }
    }
}
