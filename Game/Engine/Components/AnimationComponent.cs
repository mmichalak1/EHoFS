using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SkinnedModel;
using Microsoft.Xna.Framework.Graphics;
using OurGame.Engine.Utilities;
using OurGame.Engine.Serialization;

namespace OurGame.Engine.Components
{
    [DataContract(IsReference = true)]
    public class AnimationComponent : AbstractComponent
    {
        [DataMember]
        private string _modelName;

        private Model _model;
        private AnimationPlayer _animatedModel;
        private Dictionary<string, AnimationClip> _clips;
        private SkinningData _skinningData;

        [DataMember]
        private bool _usesDefaultNormals;
        [DataMember]
        private string _effectName = "RenderBuffer";
        [DataMember]
        private string _materialName;
        [DataMember]
        private string _idle = "IdleWithoutWeapon";
        private bool _isPlaying = false;
        private bool boolTmp = false;

        private Material _material;
        private Effect _effect;

        private Texture2D _diffuseMap, _specularMap, _normalMap, _emissiveMap;
        private int deltaTime = 0;
        private Quaternion _rotOffset = Quaternion.CreateFromYawPitchRoll(0, 0, 0);

        public Quaternion RotOffset
        {
            get { return _rotOffset; }
            set { _rotOffset = value; }
        }

        public Texture2D DiffuseMap
        {
            get { return _diffuseMap; }
            set { _diffuseMap = value; }
        }

        public AnimationPlayer AnimatedModel
        {
            get { return _animatedModel; }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        public string Idle
        {
            get { return _idle; }
            set { _idle = value; }
        }

        public AnimationComponent(GameObject parent, string modelName, string material, bool usesDefaultNormals) : base(parent)
        {
            _modelName = modelName;
            _usesDefaultNormals = usesDefaultNormals;
            _materialName = material;
            Initialize();
        }

        protected override void LoadContent()
        {
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
            ContentContainer.Shaders.TryGetValue(_effectName, out _effect);


            ContentContainer.AnimatedModels.TryGetValue(_modelName, out _model);

            _skinningData = _model.Tag as SkinningData;
            if (_skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");



            ProcessClips();

            _animatedModel = new AnimationPlayer(_skinningData);

            _animatedModel.StartClip(_clips[_idle]);
        }

        public override void Update(GameTime time)
        {
            _animatedModel.Update(TimeSpan.Zero, true, CreateWorldMatrix());
            if (_animatedModel.CurrentClip != null)
            {
                if (_animatedModel.CurrentClip == _clips[_idle])
                {
                    _isPlaying = false;
                    _animatedModel.Update(time.ElapsedGameTime, true, CreateWorldMatrix());
                }
                else
                {
                    if (new TimeSpan(0, 0, 0, 0, deltaTime) < _animatedModel.CurrentClip.Duration)
                    {
                        _isPlaying = true;
                        _animatedModel.Update(time.ElapsedGameTime, true, CreateWorldMatrix());
                        deltaTime += (int)time.ElapsedGameTime.Milliseconds;
                        if (new TimeSpan(0, 0, 0, 0, deltaTime) > _animatedModel.CurrentClip.Duration)
                        {
                            deltaTime = 0;
                            _isPlaying = false;
                            _animatedModel.StartClip(_clips[_idle]);
                            _animatedModel.Update(TimeSpan.Zero, true, CreateWorldMatrix());
                            boolTmp = true;
                        }
                    }
                    else
                    {
                        _isPlaying = false;
                        deltaTime = 0;
                    }
                }

            }
        }

        public override void Draw(GameTime time)
        {
            if(Gun.Instance.gunActive == false)
            {
                if (boolTmp)
                    boolTmp = false;
                Matrix[] bones = _animatedModel.GetSkinTransforms();


                foreach (ModelMesh mesh in _model.Meshes)
                {

                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = _effect;
                        part.Effect.Parameters["Bones"].SetValue(bones);
                        part.Effect.Parameters["emissionPower"].SetValue(_material.EmissionPower);
                        part.Effect.Parameters["specularIntensity"].SetValue(_material.Shininess);
                        part.Effect.Parameters["specularPower"].SetValue(_material.ShiningPower);
                        part.Effect.Parameters["View"].SetValue(CameraComponent.Main.View);
                        part.Effect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, (float)16 / 9, 1f, 100000f));
                        part.Effect.Parameters["ColorMap"].SetValue(_diffuseMap);
                        part.Effect.Parameters["SpecularMap"].SetValue(_specularMap);
                        part.Effect.Parameters["NormalMap"].SetValue(_normalMap);
                        part.Effect.Parameters["EmissionMap"].SetValue(_emissiveMap);
                        part.Effect.CurrentTechnique = part.Effect.Techniques[1];
                    }
                    mesh.Draw();
                }
            }
        }

        private Matrix CreateWorldMatrix()
        {
            Quaternion rotation = Parent.Transform.Rotation;
            Matrix quatRot = Matrix.CreateFromQuaternion(rotation);
            rotation = Quaternion.CreateFromRotationMatrix(quatRot);
            Matrix world =
                   Matrix.CreateScale(
                        Parent.Transform.Scale.X,
                        Parent.Transform.Scale.Y,
                        Parent.Transform.Scale.Z) *
                   Matrix.CreateFromQuaternion(Parent.Transform.Rotation * _rotOffset) *
                   Matrix.CreateTranslation(Parent.Transform.Position + new Vector3(0, 0, 0));
            return world;
        }


        private void ProcessClips()
        {
            _clips = new Dictionary<string, AnimationClip>();
            AnimationClip animationClip = _skinningData.AnimationClips.First().Value;
            Clip[] clips;
            ContentContainer.AnimationProperties.TryGetValue(_modelName, out clips);
            List<List<Keyframe>> list = new List<List<Keyframe>>();
            for (int i = 0; i < clips.Length; i++)
            {
                TimeSpan separator = new TimeSpan(0, 0, 0, 0, clips[i].Separator);
                TimeSpan duration = new TimeSpan(0, 0, 0, 0, clips[i].Duration);
                List<Keyframe> keyframesBuffer = new List<Keyframe>();

                foreach (Keyframe x in animationClip.Keyframes)
                {
                    if (x.Time >= separator && x.Time < separator + duration)
                    {
                        keyframesBuffer.Add(x);
                    }
                }

                list.Add(keyframesBuffer);
            }

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    list[i][j].Time -= new TimeSpan(0, 0, 0, 0, clips[i].Separator);
                }
                _clips.Add(clips[i].ClipName, new AnimationClip(new TimeSpan(0, 0, 0, 0, clips[i].Duration), list[i]));
            }
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        public void ChangeClip(string name)
        {
            if (!_isPlaying)
            {
                _animatedModel.Update(TimeSpan.Zero, true, CreateWorldMatrix());
                _isPlaying = true;
                deltaTime = 0;
                _animatedModel.StartClip(_clips[name]);
                _animatedModel.Update(TimeSpan.Zero, true, CreateWorldMatrix());
            }
        }

    }
}
