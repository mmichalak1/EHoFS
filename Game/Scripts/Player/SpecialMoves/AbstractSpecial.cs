using System.Runtime.Serialization;

using Microsoft.Xna.Framework;

namespace OurGame.Scripts.Player.SpecialMoves
{
    [DataContract]
    public abstract class AbstractSpecial
    {
        protected float Duration
        {
            get;
            private set;
        }
        protected SpeciaMoveManager Parent
        {
            get;
            private set;
        }

        protected bool isActive = true;

        public AbstractSpecial(float duration, SpeciaMoveManager parent)
        {
            Duration = duration;
            Parent = parent;
        }
        public abstract void Update(GameTime gameTime);
    }
}
