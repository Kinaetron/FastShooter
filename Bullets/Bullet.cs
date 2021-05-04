using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne.Collision;

using PolyOne;
using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Animation;

namespace FastShooter
{
    public class Bullet : Entity
    {
        Texture2D bullet;
        Texture2D muzzle;

        float speed;

        public Vector2 Direction;
        public float Distance;
        public float Timer;

        Vector2 actualDirection;

        private AnimationPlayer sprite;
        private AnimationData bulletAnimation;
        private AnimationData muzzleAnimation;

        protected Vector2 origin;

        private float rotation;
        private SpriteEffects flip;

        private GameTags playerTag;

        public static float BulletDamage { get; private set; }

        public Vector2 CurrentPosition;

        public Bullet(Vector2 position, float speed, GameTags playerTag)
            :base(position)
        {
            BulletDamage = 3;
            this.playerTag = playerTag;

            if (this.playerTag == GameTags.Player1)
            {
                bullet = Engine.Instance.Content.Load<Texture2D>("Bullet");
                bulletAnimation = new AnimationData(bullet, 100, 21, false);

                muzzle = Engine.Instance.Content.Load<Texture2D>("Muzzle");
                muzzleAnimation = new AnimationData(muzzle, 20, 32, false);
            }
            else if(this.playerTag == GameTags.Player2)
            {
                bullet = Engine.Instance.Content.Load<Texture2D>("Bullet2");
                bulletAnimation = new AnimationData(bullet, 100, 21, false);

                muzzle = Engine.Instance.Content.Load<Texture2D>("Muzzle2");
                muzzleAnimation = new AnimationData(muzzle, 20, 32, false);
            }

            this.Tag((int)GameTags.Bullet);
            this.Collider = new Hitbox(5, 5, 0.0f, 0.0f);

            this.speed = speed;
            this.Visible = true;

            sprite = new AnimationPlayer();
            sprite.PlayAnimation(muzzleAnimation);
            flip = SpriteEffects.None;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }
        public override void Update()
        {
            base.Update();

            if (muzzleAnimation.AnimationFinished == true)
            {
                sprite.PlayAnimation(bulletAnimation);
                Timer += Engine.DeltaTime;

                if (Direction.Y == 0.0f)
                {
                    Position.X += speed;
                    rotation = 0.0f;
                }
                else
                {
                    Direction.Normalize();

                    actualDirection += Direction * 0.6f;
                    actualDirection.Normalize();
                    rotation = (float)Math.Atan2(actualDirection.Y, actualDirection.X);

                    Position += actualDirection * Math.Abs(speed);

                    flip = SpriteEffects.None;
                }
            }
            else if(muzzleAnimation.AnimationFinished == false) {
                this.Position = CurrentPosition;
            }
        }

        public override void Draw()
        {
           if (actualDirection == Vector2.Zero)
            {
                if (speed > 0.0f) {
                    flip = SpriteEffects.None;
                }
                else {
                    flip = SpriteEffects.FlipHorizontally;
                }
            }

            sprite.Draw(this.Position, rotation, flip);

            base.Draw();
        }
    }
}