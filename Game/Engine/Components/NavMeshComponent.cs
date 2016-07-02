using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine.Utilities.Geometrics;
using OurGame.Engine.Utilities;
using OurGame.Engine.Navigation.Draw;
using System.Runtime.Serialization;

namespace OurGame.Engine.Components
{
    [DataContract(IsReference = true)]
    public class NavMeshComponent : AbstractComponent
    {
        private List<Triangle3> _triangles;
        private NavMeshDraw navDraw;
        
        public Vector3[] Waypoints { get; private set; }

        public NavMeshComponent(GameObject parent, string fileName) : base(parent)
        {
            ObjModel obj = new ObjModel("./" + fileName);
            _triangles = obj.Tris;
            Waypoints = new Vector3[_triangles.Count];
            for (int i = 0; i < Waypoints.Count(); i++)
                Waypoints[i] = _triangles[i].Centroid + Parent.Transform.Position;
            navDraw = new NavMeshDraw(_triangles);
            navDraw.SetUpVertieces();
        }

        public override void Initialize()
        {

        }

        public override void Update(GameTime time)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            if (ScreenManager.IsNavMeshVisible)
                navDraw.Draw(gameTime, Parent.Transform);

            base.Draw(gameTime);
        }
    }
}
