using System.Runtime.Serialization;


namespace OurGame.Engine
{
    [DataContract(IsReference = true)]
    public class Material
    {
        [DataMember]
        private float _shininess, _shiningPower, _emissionPower;
        [DataMember]
        private string _name;
        [DataMember]
        private bool _isEmissive;
        [DataMember]
        private bool _isReflective;

        public bool IsEmissive
        {
            get { return _isEmissive; }
            set { _isEmissive = value; }
        }

        public bool IsReflective
        {
            get { return _isReflective; }
            set { _isReflective = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public float Shininess
        {
            get { return _shininess; }
            set { _shininess = value; }
        }

        public float ShiningPower
        {
            get { return _shiningPower; }
            set { _shiningPower = value; }
        }

        public float EmissionPower
        {
            get { return _emissionPower; }
            set { _emissionPower = value; }
        }

        public Material(string name, float shininess, float shiningPower, bool isEmissive, bool isReflective, float emissionPower)
        {
            _name = name;
            _shininess = shininess;
            _shiningPower = shiningPower;
            _isEmissive = isEmissive;
            _isReflective = isReflective;
            _emissionPower = emissionPower;
        }

        private Material()
        {

        }
    }
}
