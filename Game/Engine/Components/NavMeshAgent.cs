using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace OurGame.Engine.Components
{
    [DataContract(IsReference = true)]
    public class NavMeshAgent : AbstractComponent
    {

    //    private Vector3 _currentPosition;
        private Vector3 _destination;
        private Vector3[] _waypoints;
        private bool isMoving = false;

        public float Speed { get; set; }
        public bool destinationReached { get; private set; }


   //     private Vector3 nearestWaypoint;

        public NavMeshAgent(GameObject parent) : base(parent)
        {
            _waypoints = Scene.FindWithTag("NavMeshTest").GetComponentOfType<NavMeshComponent>().Waypoints;
            destinationReached = true;
            Speed = 10f;
        }

        public override void Update(GameTime time)
        {
            if (isMoving)
                if (!destinationReached)
                    Move(time);
        }

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
            isMoving = true;
            destinationReached = false;
        }



        public void Stop()
        {
            isMoving = false;
        }

        public void Resume()
        {
            isMoving = true;
        }

        private void Move(GameTime gameTime)
        {

            Vector3 Direction;
            Vector3 nearestWaypoint;

            nearestWaypoint = FindNearestWaypoint();
            Direction = nearestWaypoint - Parent.Transform.Position;
            Direction.Normalize();

            Vector3 result = Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Parent.Transform.Position = Vector3.Add(Parent.Transform.Position, result);

            if ((Parent.Transform.Position - nearestWaypoint).Length() < 1f)
                destinationReached = true;
        }

        private void Turn(Vector3 direction)
        {
            //Parent.Transform.Rotation = Quaternion.CreateFromRotationMatrix(Matrix.)
        }

        private Vector3 FindNearestWaypoint()
        {
            int nearestIndex = 0;
            Vector3 minVector = new Vector3(99999, 99999, 99999);
            Vector3 vect;

            for (int i = 0; i < _waypoints.Count(); i++)
            {
                vect = _waypoints[i] - _destination;
                if (minVector.Length() > vect.Length())
                {
                    minVector = vect;
                    nearestIndex = i;
                }
            }

            return _waypoints[nearestIndex];
        }
    }
}
