using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;
using OurGame.Engine.Components;
using OurGame.Engine.ExtensionMethods;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    class EnemyDeathScript : IScript
    {
        private bool flag = false;
        private float timeCounter = 0;
        [DataMember]
        private GameObject _parent;
        private List<GameObject> listOfDeathBox;
        private GameObject healthBox;
        public void Initialize(GameObject parent)
        {
            Random rand = new Random();
            _parent = parent;
            listOfDeathBox = new List<GameObject>();
            foreach (GameObject children in _parent.Children)
            {
                if (children.Name == "DeathBox")
                {
                    listOfDeathBox.Add(children);
                }
                else
                {
                    healthBox = children;
                }

            }

            foreach (GameObject children in listOfDeathBox)
            {
                DeathBoxRandomizationTMPScript script = children.GetComponentOfType<ScriptComponent>().GetScriptOfType<DeathBoxRandomizationTMPScript>();

                script.Y = rand.Next(10) - 5;
                script.X = rand.Next(10) - 5;
                script.Z = rand.Next(10) - 5;
            }
        }

        public void Update(GameTime gameTime)
        {
            //KeyboardState newState = Keyboard.GetState();

            if (flag == false) //&& newState.IsKeyDown(Keys.F))
            {
                flag = true;
                Random rand = new Random();
                foreach (GameObject children in listOfDeathBox)
                {
                    children.GetComponentOfType<RigidBodyComponent>().Mass = 1;
                    children.GetComponentOfType<RigidBodyComponent>().AffectedByGravity = true;
                    Vector3 direction = -_parent.Transform.Orientation;
                    direction += ExtensionMethods.RandomDirection(rand);
                    direction.Y += 0.5f;
                    children.GetComponentOfType<RigidBodyComponent>().AddAffectingForce(direction * 300f, 1f);
                }
                healthBox.GetComponentOfType<RigidBodyComponent>().Mass = 1;
                healthBox.GetComponentOfType<RigidBodyComponent>().AffectedByGravity = true;
                healthBox.GetComponentOfType<RigidBodyComponent>().AddAffectingForce(Vector3.Up * 200f, 0.5f);

            }
            if (flag == true)
            {
                timeCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                float eps = 0.1f;
                foreach (GameObject deathBoxes in listOfDeathBox)
                {
                    DeathBoxRandomizationTMPScript script = deathBoxes.GetComponentOfType<ScriptComponent>().GetScriptOfType<DeathBoxRandomizationTMPScript>();
                    deathBoxes.Transform.Rotation =
                        Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(timeCounter * script.X * 10)) *
                        Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(timeCounter * script.Z * 10)) *
                        Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(timeCounter * script.Y * 10));
                    if ((2 - timeCounter) < eps)
                    {
                        if (_parent.Children.Contains(healthBox))
                            _parent.Children.Remove(healthBox);
                        _parent.Destroy = true;
                    }
                    else
                    {
                        deathBoxes.Transform.Scale = new Scale(
                            deathBoxes.Transform.Scale.X - MathHelper.Lerp(0, deathBoxes.Transform.Scale.X, timeCounter * 0.1f),
                            deathBoxes.Transform.Scale.Y - MathHelper.Lerp(0, deathBoxes.Transform.Scale.Y, timeCounter * 0.1f),
                            deathBoxes.Transform.Scale.Z - MathHelper.Lerp(0, deathBoxes.Transform.Scale.Z, timeCounter * 0.1f));
                    }

                }
            }
        }

        public void setUpPosition()
        {
            listOfDeathBox[0].Transform.Rotation = _parent.Transform.Rotation;
            listOfDeathBox[1].Transform.Rotation = _parent.Transform.Rotation;
            listOfDeathBox[2].Transform.Rotation = _parent.Transform.Rotation;
            listOfDeathBox[3].Transform.Rotation = _parent.Transform.Rotation;
            listOfDeathBox[4].Transform.Rotation = _parent.Transform.Rotation;
            listOfDeathBox[5].Transform.Rotation = _parent.Transform.Rotation;
            listOfDeathBox[6].Transform.Rotation = _parent.Transform.Rotation;
            listOfDeathBox[7].Transform.Rotation = _parent.Transform.Rotation;
            listOfDeathBox[8].Transform.Rotation = _parent.Transform.Rotation;
            listOfDeathBox[9].Transform.Rotation = _parent.Transform.Rotation;

            listOfDeathBox[0].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(10, 75, 0), _parent.Transform);
            listOfDeathBox[1].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(-15, 75, 0), _parent.Transform);
            listOfDeathBox[2].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(-15, 50, 0), _parent.Transform);
            listOfDeathBox[3].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(10, 50, 0), _parent.Transform);
            listOfDeathBox[4].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(35, 50, 0), _parent.Transform);
            listOfDeathBox[5].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(-40, 50, 0), _parent.Transform);
            listOfDeathBox[6].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(10, 25, 0), _parent.Transform);
            listOfDeathBox[7].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(-15, 25, 0), _parent.Transform);
            listOfDeathBox[8].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(35, 25, 0), _parent.Transform);
            listOfDeathBox[9].Transform.Position = ExtensionMethods.PointToWorld(new Vector3(-40, 25, 0), _parent.Transform);

            healthBox.Transform.Position = _parent.Transform.Position + (new Vector3(0, 25, -20));
            healthBox.Transform.Rotation = _parent.Transform.Rotation;
        }
    }
}
