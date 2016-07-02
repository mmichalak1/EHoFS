using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OurGame.Engine.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static Vector3 GetVector(this Quaternion quaternion)
        {
            return new Vector3(quaternion.X, quaternion.Y, quaternion.Z);
        }

        /// <summary>
        /// Returns amgle between two vectors in radians.
        /// </summary>
        /// <param name="vect1"></param>
        /// <param name="vect2"></param>
        /// <returns></returns>
        public static double GetAngleFromVectors(this Vector3 vect1, Vector3 vect2)
        {
            double u, v;
            u = Math.Sqrt(vect1.X * vect1.X + vect1.Y * vect1.Y + vect1.Z * vect1.Z);
            v = Math.Sqrt(vect2.X * vect2.X + vect2.Y * vect2.Y + vect2.Z * vect2.Z);
            return Math.Acos(Vector3.Dot(vect1, vect2) / (u * v));
        }

        public static Vector3 Copy(this Vector3 vect)
        {
            return new Vector3(vect.X, vect.Y, vect.Z);
        }

        public static Quaternion Copy(this Quaternion quat)
        {
            return new Quaternion(
                quat.X,
                quat.Y,
                quat.Z,
                quat.W);
        }

        public static Quaternion TurnTowardsTheVector(this Quaternion rotation, Vector3 vect, float lerp)
        {
            double theta = Vector3.Forward.GetAngleFromVectors(vect);

            if (vect.X > 0)
                theta = -theta;

            return Quaternion.Lerp(rotation, Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), (float)theta), lerp);
        }

        public static Vector3 Truncate(this Vector3 vect, float max)
        {
            if (vect.Length() > max)
            {
                vect.Normalize();

                vect *= max;
            }

            return vect;
        }

        public static Vector3 PerpendicularVector3YAxisConstrained(this Vector3 vect)
        {
            return new Vector3(vect.Z, vect.Y, -vect.X);
        }

        public static float Min(this float value1, float value2)
        {
            if (value1 < value2)
                return value1;
            else
                return value2;
        }

        /// <summary>
        /// Returns random float from -1.0 to 1.0
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static float NextFloat(this Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            return (float)mantissa;
        }

        public static Vector3 RotateVector(this Vector3 vect, float angle)
        {
            Vector3 result = vect;
            float x, z;
            x = result.X * (float)Math.Cos(angle) - result.Z * (float)Math.Sin(angle);
            z = result.X * (float)Math.Sin(angle) + result.Z * (float)Math.Cos(angle);

            if (angle < 0)
            {
                result.X = x;
                result.Z = z;
            }
            else
            {
                result.X = x;
                result.Z = z;
            }

            return result;
        }

        public static Vector3 PointToWorld(Vector3 localPoint, Transform parentObejct)
        {
            Matrix World = Matrix.CreateScale(
                    parentObejct.Scale.X,
                    parentObejct.Scale.Y,
                    parentObejct.Scale.Z) *
                Matrix.CreateFromQuaternion(-parentObejct.Rotation) *
                Matrix.CreateTranslation(parentObejct.Position);
            localPoint *= -1;
            localPoint.Y *= -1;

            Vector3 result = Vector3.Transform(localPoint, World);

            return Vector3.Transform(localPoint, World);
        }

        public static void TurnToTarget(ref GameObject parent, Vector3 targetPosition)
        {
            if (targetPosition != Vector3.Zero || targetPosition != null)
                parent.Transform.Rotation = parent.Transform.Rotation.TurnTowardsTheVector(targetPosition, 0.05f);
        }
        public static bool FindTile(GameObject gameObject)
        {
            if (gameObject.Name == "floorTile")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            Random rand = new Random();
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static Vector3 RandomDirection(Random rand)
        {
            float x = rand.Next(2000) - 1000;
            float y = rand.Next(2000) - 1000;
            float z = rand.Next(2000) - 1000;
            Vector3 vector = new Vector3(x, y, z);
            vector.Normalize();
            return vector;
        }
    }
}
