using Microsoft.Xna.Framework;

namespace OurGame.Engine.Utilities
{
    public class BoxData
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Matrix View, Projection; 

        public void GenerateMatrixes()
        {
            Vector3 reference = Vector3.Transform(Position, Rotation) + Vector3.UnitZ;
            View = Matrix.CreateLookAt(Position, reference, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)16 / 9, 1f, 100000f);
        }

    }
}
