using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne;
using PolyOne.Scenes;
using PolyOne.Collision;
using PolyOne.Engine;
using PolyOne.Components;
using PolyOne.Animation;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace FastShooter
{
    public class Player : Entity
    {
        protected bool xLast;
        protected bool yLast;

        protected float signX;
        protected float signY;

        protected float boast;

        public Color PlayerColour;
        protected int playerNo;
        protected Color assignColour;

        protected Texture2D healthBarImageEmpty;
        protected Texture2D healthBarImage1;
        protected Texture2D healthBarImage2;

        protected Vector2 healthBarInitPosition;
        protected Vector2 healthBarPosition;

        public Rectangle HealthRectangle
        {
            get { return healthRectangle; }
            set { healthRectangle = value; }
        }
        protected Rectangle healthRectangle;

        public float PlayerHealth
        {
            get { return playerHealth; }
            set { playerHealth = value; }
        }
        protected float playerHealth;

        protected const float runAccel = 1.0f;
        protected const float turnMul = 1.0f;
        protected const float normMaxHorizSpeed = 10.0f;

        protected bool engineOn;
        protected const float airAccel = 0.5f;
        protected const float airturnMul = 0.25f;
        protected const float airMaxHorizSpeed = 8.0f;

        protected const float boastAcc = -1.0f;
        protected const float gravity = 0.1f;
        protected const float fallMaxSpeed = 0.25f;
        protected const float boastMaxSpeed = 6.0f;

        protected bool dodgeStart;
        protected bool dodgeCoolBool;
        protected const float dodgeCoolTime = 250.2f;
        protected const float dodgeTime = 133.7f;
        protected Color dodgeColour = Color.White;
        protected const float dodgeAccel = 6.0f;
        protected const float dodgeSpeed = 30.0f;

        protected AnimationPlayer sprite;
        protected AnimationPlayer spriteEngine;
        protected AnimationPlayer chargeAni;

        protected AnimationData idleAnimationTop;
        protected AnimationData hitAnimationTop;
        protected AnimationData shootingAnimationTop;
        protected AnimationData dodgeAnimation;
        protected AnimationData engineAnimation;
        protected AnimationData charge1Animation;
        protected AnimationData charge2Animation;

        protected Texture2D bottomPlayerIdle;
        protected Texture2D bottomPlayerHit;
        protected Texture2D bottomPlayerRight;
        protected Texture2D bottomPlayerLeft;

        protected SpriteEffects flip;

        public bool Dodge
        {
           get { return counters.Check("dodgeTimer"); }
        }

        protected Vector2 remainder;


        public Vector2 Velocity;

        protected bool isShooting;
        protected bool timerSet;

        protected float lastSignX;
        protected float lastSignY;


        public List<Bullet> Bullets
        {
            get { return bullets; }
            set { bullets = value; }
        }
        private List<Bullet> bullets = new List<Bullet>();

        public List<Bullet2> Bullets2
        {
            get { return bullets2; }
            set { bullets2 = value; }
        }
        private List<Bullet2> bullets2 = new List<Bullet2>();

        public List<Bullet3> Bullets3
        {
            get { return bullets3; }
            set { bullets3 = value; }
        }
        private List<Bullet3> bullets3 = new List<Bullet3>();

        protected const float bulletSpeed = 16.0f;
        protected const float bulletSpeed2 = 14.5f;
        protected const float bulletSpeed3 = 12.0f;
        protected bool charged1 = false;
        protected bool charged2 = false;
        protected const float bulletDistanceLeft = 15.0f;
        protected const float bulletDistanceRight = 50.0f;
        protected const float bulletCoolDown = 180.0f;
        protected const int bulletPositionOffSet = 12;

        protected CounterSet<string> counters = new CounterSet<string>();
        protected bool cooldown = false;

        protected Vector2 levelSize;

        protected bool isOnGround = false;

        protected GameTags playerTag;

        protected Vector2 currentBulletPos;

        public bool PlayerHit = false;

        protected SoundEffect engineSound;
        protected SoundEffectInstance engineInstance;

        protected SoundEffect shootLevel1;
        protected SoundEffectInstance shootInstance1;

        protected SoundEffect shootLevel2;
        protected SoundEffectInstance shootInstance2;

        protected SoundEffect shootLevel3;
        protected SoundEffectInstance shootInstance3;

        protected SoundEffect chargeSound;
        protected SoundEffectInstance chargeInstance;

        protected SoundEffect chargeSound2;
        protected SoundEffectInstance chargeInstance2;

        protected SoundEffect dodgeSound;
        protected SoundEffectInstance dodgeInstance;

        protected SoundEffect hurtSound;
        protected SoundEffectInstance hurtInstance;

        public Player(Vector2 position, int playerNo, Color playerColour, GameTags gameTag, Vector2 levelSize)
            :base(position)
        {

            sprite = new AnimationPlayer();
            spriteEngine = new AnimationPlayer();
            chargeAni = new AnimationPlayer();

            this.levelSize = levelSize;
            this.playerNo = playerNo;
            this.PlayerColour = playerColour;
            assignColour = playerColour;

            Tag((int)gameTag);
            playerTag = gameTag;
            Collider = new Hitbox((float)32.0f, (float)33.0f, 0.0f, 0.0f);
            this.Visible = true;

            healthBarImageEmpty = Engine.Instance.Content.Load<Texture2D>("GUI/EmptyHealthBar");
            healthBarImage1 = Engine.Instance.Content.Load<Texture2D>("GUI/HealthBar1");
            healthBarImage2 = Engine.Instance.Content.Load<Texture2D>("GUI/HealthBar2");

            hitAnimationTop = new AnimationData(Engine.Instance.Content.Load<Texture2D>("Player/HitFrameTop"), 200, 30, true);
            bottomPlayerHit = Engine.Instance.Content.Load<Texture2D>("Player/HitLegs");

            engineSound = Engine.Instance.Content.Load<SoundEffect>("EngineSound");
            engineInstance = engineSound.CreateInstance();
            engineInstance.IsLooped = true;

            shootLevel1 = Engine.Instance.Content.Load<SoundEffect>("ShootLevel1");
            shootInstance1 = shootLevel1.CreateInstance();

            shootLevel2 = Engine.Instance.Content.Load<SoundEffect>("ShootLevel2");
            shootInstance2 = shootLevel2.CreateInstance();

            shootLevel3 = Engine.Instance.Content.Load<SoundEffect>("ShootLevel3");
            shootInstance3 = shootLevel3.CreateInstance();

            chargeSound = Engine.Instance.Content.Load<SoundEffect>("chargeSound");
            chargeInstance = chargeSound.CreateInstance();
            chargeInstance.IsLooped = true;

            chargeSound2 = Engine.Instance.Content.Load<SoundEffect>("chargeSound2");
            chargeInstance2 = chargeSound2.CreateInstance();
            chargeInstance2.IsLooped = true;

            dodgeSound = Engine.Instance.Content.Load<SoundEffect>("dodgeSound");
            dodgeInstance = dodgeSound.CreateInstance();

            hurtSound = Engine.Instance.Content.Load<SoundEffect>("hurtsound");
            hurtInstance = hurtSound.CreateInstance();


            this.Add(counters);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Update()
        {
            Movement();
            Shooting();
            BulletPosition();
            PlayerHurt();

            base.Update();
        }

        protected virtual void Movement()
        {

            if (counters.Check("dodgeTimer") == false) {

                if(boast != 0) {
                    if (engineOn == false) {
                        spriteEngine.ResetAnimation();
                    }
                    engineOn = true;
                    engineInstance.Play();
                }
                else {
                    engineOn = false;
                    engineInstance.Stop();
                }

                Velocity.Y += boastAcc * boast;

                Velocity.Y += gravity;
                Velocity.Y = MathHelper.Clamp(Velocity.Y, -boastMaxSpeed, fallMaxSpeed);
            }


            if (counters.Check("dodgeTimer") == true)
            {
                if(dodgeStart == true) {
                    dodgeStart = false;

                    if (signX != 0 || xLast == true) {
                        Velocity.X = 0;
                    }
                    else if (signY != 0 || yLast == true) {
                        Velocity.Y = 0;
                    }
                }

                if (signX != 0 && signY != 0)
                {
                    Vector2 movement = new Vector2(signX, signY);
                    movement.Normalize();
                    Velocity += dodgeAccel * movement;

                    yLast = false;
                    xLast = false;
                }


                if (signY != 0 && signX == 0) {
                    Velocity.Y += dodgeAccel * signY;
                }
                else if(yLast == true) {
                    Velocity.Y += dodgeAccel * lastSignY;
                }

                if (signX != 0 && signY == 0) {
                    Velocity.X += dodgeAccel * signX;
                }
                else if(xLast == true) {
                    Velocity.X += dodgeAccel * lastSignX;
                }

                Velocity.X = MathHelper.Clamp(Velocity.X, -dodgeSpeed, dodgeSpeed);
                Velocity.Y = MathHelper.Clamp(Velocity.Y, -dodgeSpeed, dodgeSpeed);
            }
            else
            {
                if (isOnGround == true) {
                    Velocity.X += runAccel * signX;
                    Velocity.X = MathHelper.Clamp(Velocity.X, -normMaxHorizSpeed, normMaxHorizSpeed);
                }
                else {
                    Velocity.X += airAccel * signX;
                    Velocity.X = MathHelper.Clamp(Velocity.X, -airMaxHorizSpeed, airMaxHorizSpeed);
                }
            }

            float currentSign = Math.Sign(Velocity.X);

            if (currentSign != 0 && currentSign != signX && isOnGround == true) {
                Velocity.X *= turnMul;
            }
            else if (currentSign != 0 && currentSign != signX && 
                     isOnGround == false && counters.Check("dodgeTimer") == false)
            {
                Velocity.X *= airturnMul;
            }

            MovementHorizontal(Velocity.X);
            MovementVerical(Velocity.Y);

            if (signX != 0)
            {
                xLast = true;
                yLast = false;

                lastSignX = signX;
            }
            if (signY != 0)
            {
                xLast = false;
                yLast = true;

                lastSignY = signY;
            }
        }

        protected virtual void PlayerHurt()
        {
            if (PlayerHit == true)
            {
                PlayerHit = false;
                counters["hitTimer"] = 100.0f;
            }

            if(counters.Check("hitTimer") == true) {
                sprite.PlayAnimation(hitAnimationTop);
                hurtInstance.Play();
            }
        }

        protected virtual void Shooting()
        {
            
        }

        public virtual void ShotVibration(float strength, float time)
        {

        }

        private void BulletPosition()
        {
            if (lastSignX > 0) {
                currentBulletPos = new Vector2(Position.X + bulletDistanceRight, Position.Y + bulletPositionOffSet);
            }
            else if (lastSignX < 0) {
                currentBulletPos = new Vector2(Position.X - bulletDistanceLeft, Position.Y + bulletPositionOffSet);
            }

            foreach (Bullet bullet in bullets) {
                bullet.CurrentPosition = currentBulletPos;
            }

            foreach (Bullet2 bullet in bullets2) {
                bullet.CurrentPosition = currentBulletPos;
            }

            foreach (Bullet3 bullet in bullets3) {
                bullet.CurrentPosition = currentBulletPos;
            }

        }

        protected virtual void BulletCreation()
        {
            if (counters.Check("bulletCooldown") == false) {
                cooldown = false;
            }

            if (cooldown == true)
                return;

            if (lastSignX > 0)
            {
                bullets.Add(new Bullet(new Vector2(Position.X + bulletDistanceRight, Position.Y + bulletPositionOffSet), bulletSpeed, playerTag));
                this.Scene.Add(bullets[bullets.Count - 1]);
                bullets[bullets.Count - 1].Added(this.Scene);
                shootInstance1.Play();
            }
            else if (lastSignX < 0)
            {
                bullets.Add(new Bullet(new Vector2(Position.X - bulletDistanceLeft, Position.Y + bulletPositionOffSet), -bulletSpeed, playerTag));
                this.Scene.Add(bullets[bullets.Count - 1]);
                bullets[bullets.Count - 1].Added(this.Scene);
                shootInstance1.Play();
            }

            if (bullets.Count > 0 && cooldown == false) {
                counters["bulletCooldown"] = bulletCoolDown;
                cooldown = true;
            }

        }

        protected virtual void BulletCreationLevel2()
        {
            if (lastSignX > 0)
            {
                if (charged1 == true)
                {
                    bullets2.Add(new Bullet2(new Vector2(Position.X + bulletDistanceRight, Position.Y + bulletPositionOffSet), bulletSpeed2, playerTag));
                    this.Scene.Add(bullets2[bullets2.Count - 1]);
                    bullets2[bullets2.Count - 1].Added(this.Scene);
                    shootInstance2.Play();
                }
                else if (charged2 == true)
                {
                    bullets3.Add(new Bullet3(new Vector2(Position.X + bulletDistanceRight, Position.Y + bulletPositionOffSet), bulletSpeed3, playerTag));
                    this.Scene.Add(bullets3[bullets3.Count - 1]);
                    bullets3[bullets3.Count - 1].Added(this.Scene);
                    shootInstance3.Play();
                }
            }
            else if (lastSignX < 0)
            {
                if (charged1 == true)
                {
                    bullets2.Add(new Bullet2(new Vector2(Position.X - bulletDistanceLeft, Position.Y + bulletPositionOffSet), -bulletSpeed2, playerTag));
                    this.Scene.Add(bullets2[bullets2.Count - 1]);
                    bullets2[bullets2.Count - 1].Added(this.Scene);
                    shootInstance2.Play();
                }
                else if (charged2 == true)
                {
                    bullets3.Add(new Bullet3(new Vector2(Position.X - bulletDistanceLeft, Position.Y + bulletPositionOffSet), -bulletSpeed2, playerTag));
                    this.Scene.Add(bullets3[bullets3.Count - 1]);
                    bullets3[bullets3.Count - 1].Added(this.Scene);
                    shootInstance3.Play();
                }
            }

            charged1 = false;
            charged2 = false;
        }

        private void MovementHorizontal(float amount)
        {
            remainder.X += amount;
            int move = (int)Math.Round((double)remainder.X);

            if (move != 0)
            {
                remainder.X -= move;
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(sign, 0);

                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null) {
                        remainder.X = 0;
                        break;
                    }

                    if (Position.X > levelSize.X) {
                        Position.X = 32.0f;
                    }
                    if (Position.X < 0.0f) {
                        Position.X = levelSize.X - 32.0f;
                    }

                    Position.X += sign;
                    move -= sign;
                }
            }
        }

        private void MovementVerical(float amount)
        {
            remainder.Y += amount;
            int move = (int)Math.Round((double)remainder.Y);

            if (move < 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, -1.0f);
                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null) {
                        remainder.Y = 0;
                        break;
                    }

                    if (Position.Y < 0.0f) {
                        Position.Y = levelSize.Y - 32.0f;
                    }

                    Position.Y += -1.0f;
                    move -= -1;
                }
            }
            else if (move > 0)
            {
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, 1.0f);
                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null) {
                        isOnGround = true;
                        remainder.Y = 0;
                        break;
                    }
                    else {
                        isOnGround = false;
                    }

                    if (Position.Y > levelSize.Y) {
                        Position.Y = 32.0f;
                    }

                    Position.Y += 1.0f;
                    move -= 1;
                }
            }
        }

        public virtual void HealthBarDrawing()
        {
            Engine.SpriteBatch.Draw(healthBarImageEmpty, healthBarInitPosition, new Rectangle(0, 0, 103, 32) ,Color.White);
        }

        public override void Draw()
        {
            if (engineOn == true) {
                spriteEngine.PlayAnimation(engineAnimation);
            }

            if (counters.Check("hitTimer") == false)
            {
                if (counters.Check("dodgeTimer") == true)
                {
                    sprite.PlayAnimation(dodgeAnimation);
                }
                else if (isShooting == true)
                {
                    sprite.PlayAnimation(shootingAnimationTop);
                }
                else
                {
                    sprite.PlayAnimation(idleAnimationTop);
                }
            }

            if(signX < 0)  {
                flip = SpriteEffects.FlipHorizontally;
            }
            else if(signX > 0) {
                flip = SpriteEffects.None;
            }
            else if(lastSignX < 0) {
                flip = SpriteEffects.FlipHorizontally;
            }
            else if(lastSignX > 0) {
                flip = SpriteEffects.None;
            }


            if (engineOn == true) {

                if (flip == SpriteEffects.None) {
                    spriteEngine.Draw(new Vector2(this.Position.X + 5, this.Position.Y + 20), 0.0f, flip);
                }
                else {
                    spriteEngine.Draw(new Vector2(this.Position.X + 30, this.Position.Y + 20), 0.0f, flip);
                }
            }

            if (counters.Check("dodgeTimer") == false)
            {
                 if(counters.Check("hitTimer") == true) {
                    Engine.SpriteBatch.Draw(bottomPlayerHit, new Vector2(this.Position.X, Position.Y + 16.0f), Color.White);
                }
                else if (Velocity.X > 0) {
                    Engine.SpriteBatch.Draw(bottomPlayerRight, new Vector2(this.Position.X, Position.Y + 16.0f), Color.White);
                }
                else if (Velocity.X < 0) {
                    Engine.SpriteBatch.Draw(bottomPlayerLeft, new Vector2(this.Position.X, Position.Y + 16.0f), Color.White);
                }
                else{
                    Engine.SpriteBatch.Draw(bottomPlayerIdle, new Vector2(this.Position.X, Position.Y + 16.0f), Color.White);
                }
            }

            float rotation = 0.0f;
           
            if (counters.Check("dodgeTimer") == true) {

                Vector2 direction = Velocity;
                direction.Normalize();

                if (Math.Abs(direction.X) > 0 && Math.Abs(direction.Y) < 0.07f) {
                    rotation = 0.0f;
                }
                else {
                    rotation = (float)Math.Atan2(direction.Y, direction.X);
                }
            }
            base.Draw();

            if (counters.Check("dodgeTimer") == true) {
                sprite.Draw(new Vector2(Position.X + 17, Position.Y + 13), rotation, flip);
            }
            else if (isShooting == true)
            {
                if (flip == SpriteEffects.None) {
                    sprite.Draw(new Vector2(Position.X + 20, Position.Y + 10), rotation, flip);
                }
                else {
                    sprite.Draw(new Vector2(Position.X + 12, Position.Y + 10), rotation, flip);
                }
            }
            else {
                sprite.Draw(new Vector2(Position.X + 16, Position.Y + 10), rotation, flip);
            }

            if (charged1 == true | charged2 == true) {

                if (flip == SpriteEffects.None) {
                    chargeAni.Draw(new Vector2(Position.X + 24, Position.Y + 16), 0.0f, SpriteEffects.None);
                }
                else{
                    chargeAni.Draw(new Vector2(Position.X + 7, Position.Y + 16), 0.0f, SpriteEffects.None);
                }
            }
        }
    }
}