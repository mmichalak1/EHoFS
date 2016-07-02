using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using System.Runtime.Serialization;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    public class TargetScript : IScript
    {
        private GameObject parent;
        private ColliderComponent collider;
        public bool IsActive;
        private bool isRotating;
        private float rotationTime;

        public bool IsRotating { get { return isRotating; } set { isRotating = value; } }

        public void Initialize(GameObject parent)
        {
            this.parent = parent;
            isRotating = false;
            IsActive = false;
            collider = parent.GetComponentOfType<ColliderComponent>();
            parent.Transform.Position = new Vector3(parent.Parent.Transform.Position.X,600,parent.Parent.Transform.Position.Z);
            parent.Parent.GetComponentOfType<ModelComponent>().DiffuseMap = ContentContainer.TexColor["thing_red"];
            rotationTime = 1000;
        }

        public void Update(GameTime gameTime)
        {
            var bullet = collider.getActualListOfColidingObjects().Where(x => x.Tag == "Bullet").FirstOrDefault();
            if (bullet != null)
            {
                bullet.Destroy = true;
                IsActive = true;
                isRotating = true;
            }

            if (IsActive)
            {
                if (isRotating)
                {
                    parent.Parent.GetComponentOfType<ModelComponent>().DiffuseMap = ContentContainer.TexColor["thing"];
                    rotationTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    parent.Transform.Rotation *= Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver2 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                    if (rotationTime < 0)
                    {
                        isRotating = false;
                        rotationTime = 1000;
                    }
                }
            }
            else
            {
                if(isRotating)
                {
                    parent.Parent.GetComponentOfType<ModelComponent>().DiffuseMap = ContentContainer.TexColor["thing_red"];
                    rotationTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    parent.Transform.Rotation *= Quaternion.CreateFromYawPitchRoll(0, MathHelper.PiOver2 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                    if (rotationTime < 0)
                    {
                        isRotating = false;
                        rotationTime = 1000;
                    }
                }
            }
        }
    }
}
