using System.Runtime.Serialization;
using OurGame.OurException;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace OurGame.Engine.Components
{
    [DataContract(IsReference = true)]
    public class SimpleAnimationComponent : AbstractComponent
    {
        [DataMember]
        ModelComponent _modelComponent;
        [DataMember]
        float _changeModelInterval;
        [DataMember]
        string[] _modelsNames;
        [DataMember]
        float _changeTimer;
        [DataMember]
        short _currentModel;
        Model[] _models;

        public int CurrentModel { get { return _currentModel; } }
        public float ChangeModelInterval { get { return _changeModelInterval; } }
        public SimpleAnimationComponent(GameObject parent, string[] modelNames, float changeModelInterval) : base(parent)
        {
            _modelComponent = parent.GetComponentOfType<ModelComponent>();
            if (_modelComponent == null)
                throw new NoModelException("Game Object has no ModelComponent");
            _changeModelInterval = changeModelInterval;
            _modelsNames = modelNames;
            _currentModel = 0;
            LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _models = new Model[_modelsNames.Length];
            for (int i = 0; i < _modelsNames.Length; i++)
                ContentContainer.Models.TryGetValue(_modelsNames[i], out _models[i]);
            _modelComponent.Model = _models[0];
        }

        public override void Update(GameTime time)
        {
            _changeTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
            if (_changeTimer >= _changeModelInterval)
            {
                if (_currentModel == _models.Length - 1)
                    _currentModel = 0;
                else
                    _currentModel++;
                _modelComponent.Model = _models[_currentModel];
                _changeTimer = 0f;
            }
        }
    }
}
