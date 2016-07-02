using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using OurGame.Scripts.Enviroment.Modifications;
using Microsoft.Xna.Framework;

namespace OurGame.Engine.Statics
{
    public class ModificationIndicator
    {
        private static Point _size = new Point(100, 100);
        private static Point _position;

        Dictionary<Modificator, Texture2D> indicators;

        private Texture2D _currentTexture;
        private bool _isActive;

        float XPos = 0.05f, YPos = 0.05f;


        public ModificationIndicator(GraphicsDevice device)
        {
            EventSystem.Instance.RegisterForEvent("ModificatorSet", SetActiveMod);
            _position = new Point((int)Math.Ceiling(device.Viewport.Width * XPos), (int)Math.Ceiling(device.Viewport.Height * YPos));
            indicators = new Dictionary<Modificator, Texture2D>()
            {
                {Modificator.BigEnemies, ContentContainer.TexColor["HpMod"] },
                {Modificator.GentelMan, ContentContainer.TexColor["DamageMod"] },
                {Modificator.Opalescent, ContentContainer.TexColor["AccelMod"] },
                {Modificator.ThugLife, ContentContainer.TexColor["ThugMod"] },
                {Modificator.Rainbow, ContentContainer.TexColor["LifeMod"]}
            };
        }

        public void Draw(SpriteBatch batch)
        {
            if (_isActive)
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                batch.Draw(_currentTexture, new Rectangle(_position, _size), Color.White);
                batch.End();
            }

        }

        private void SetActiveMod(object modifier)
        {
            if (modifier == null)
            {
                _currentTexture = null;
                _isActive = false;
            }
            else
            {
                _currentTexture = indicators[(Modificator)modifier];
                _isActive = true;
            }

        }
    }
}
