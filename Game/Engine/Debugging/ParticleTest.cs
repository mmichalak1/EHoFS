using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Engine.Debugging
{
    public static class ParticleTest
    {
        //public static SpriteBatch spriteBatch;
        //public static BasicEffect basicEffect;
        //public static GraphicsDevice graphicDevice;
        //public static Texture2D particleTexture;

        public static void Initialize()
        {
            //graphicDevice = OurGame.Game.GraphicsDevice;
            //spriteBatch = new SpriteBatch(graphicDevice);

            //basicEffect = new BasicEffect(graphicDevice)
            //{
            //    TextureEnabled = true,
            //    VertexColorEnabled = true,
            //};
        }

        public static void Draw(Effect efect, Texture2D texture)
        {
            Vector3 particlePosition = new Vector3(500, 500, 500);

            efect.Parameters["World"].SetValue(Matrix.CreateConstrainedBillboard(particlePosition, particlePosition - CameraComponent.Main.Parent.Transform.Orientation, Vector3.Down, null, null));
            Matrix matrix = Matrix.CreateConstrainedBillboard(particlePosition, particlePosition - CameraComponent.Main.Parent.Transform.Orientation, Vector3.Down, null, null);
            efect.Parameters["View"].SetValue(CameraComponent.Main.View);
            //efect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, (float)16 / 9, 1f, 100000f));
            efect.Parameters["Projection"].SetValue(CameraComponent.Main.Projection);
            efect.Parameters["shaderTexture"].SetValue(texture);

            //basicEffect.World = Matrix.CreateConstrainedBillboard(textPosition, textPosition - CameraComponent.Main.Parent.Transform.Orientation, Vector3.Down, null, null);
            //basicEffect.View = CameraComponent.Main.View;
            //basicEffect.Projection = CameraComponent.Main.View;

            //const string message = "hello, world!";
            //Vector2 textOrigin = Debug.debugFont.MeasureString(message) / 2;
            //const float textSize = 5.25f;

            //spriteBatch.Begin(0, null, null, DepthStencilState.DepthRead, RasterizerState.CullClockwise, basicEffect);
            //spriteBatch.DrawString(Debug.debugFont, message, Vector2.Zero, Color.White, 0, textOrigin, textSize, 0, 0);
            //spriteBatch.End();
        }
    }
}
