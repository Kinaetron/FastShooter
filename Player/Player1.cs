using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using PolyOne.Scenes;
using PolyOne.Input;
using PolyOne.Animation;
using PolyOne.Engine;


namespace FastShooter
{
    public class Player1 : Player
    {
        private bool controllerMode;
        private bool keyboardMode;

        private List<Keys> keyList = new List<Keys>(new Keys[] { Keys.W, Keys.A, Keys.S ,Keys.D, Keys.C, Keys.V });

        public Player1(Vector2 position, int playerNo, Color playerColour, GameTags gameTag, Vector2 levelSize)
            :base(position, playerNo, playerColour, gameTag, levelSize)
            { 
                lastSignX = 1;
                playerHealth = 100;

                idleAnimationTop = new AnimationData(Engine.Instance.Content.Load<Texture2D>("Player/IdleFrameTop1"), 200, 30, true);
                shootingAnimationTop = new AnimationData(Engine.Instance.Content.Load<Texture2D>("Player/ShootFrameTop1"), 200, 34, true);
                dodgeAnimation = new AnimationData(Engine.Instance.Content.Load<Texture2D>("Player/Dodge1"), 50, 48, true);
                engineAnimation = new AnimationData(Engine.Instance.Content.Load<Texture2D>("Engine"), 60, 32, true);

                charge1Animation = new AnimationData(Engine.Instance.Content.Load<Texture2D>("Engine"), 100, 32, true);
                charge2Animation = new AnimationData(Engine.Instance.Content.Load<Texture2D>("Engine"), 50, 32, true);


                bottomPlayerIdle = Engine.Instance.Content.Load<Texture2D>("Player/IdleLegs1");
                bottomPlayerRight = Engine.Instance.Content.Load<Texture2D>("Player/RightLegs1");
                bottomPlayerLeft = Engine.Instance.Content.Load<Texture2D>("Player/LeftLegs1");

                flip = SpriteEffects.None;

                sprite.PlayAnimation(idleAnimationTop);
                spriteEngine.PlayAnimation(engineAnimation);

                lastSignX = 1;
                xLast = true;

                healthRectangle = new Rectangle(0, 0, (int)playerHealth, 32);
                healthBarPosition = new Vector2(20, 25);
                healthBarInitPosition = healthBarPosition;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Update()
        {
            base.Update();
            InputMode();
           
        }

        private void InputMode()
        {
            foreach (Keys key in keyList)
            {
                if (PolyInput.Keyboard.Check(key) == true) {
                    controllerMode = false;
                    keyboardMode = true;
                }
            }
            if (PolyInput.GamePads[playerNo].ButtonCheck() == true)
            {
                controllerMode = true;
                keyboardMode = false;
            }

            if (controllerMode == false && keyboardMode == false) {
                keyboardMode = true;
            }
        }

        protected override void Movement()
        {
            if (controllerMode == true)
            {
                signX = 0;
                signY = 0;

                boast = 0;

                if (PolyInput.GamePads[playerNo].Check(Buttons.A) == true ||
                    PolyInput.GamePads[playerNo].LeftTriggerCheck(0.2f) == true || 
                    PolyInput.GamePads[playerNo].Check(Buttons.LeftShoulder) == true) {
                    boast = 1;
                }
                else if (Velocity.Y < 0 && counters.Check("dodgeTimer") == false) {
                    Velocity.Y = 0.0f;
                }

                if(PolyInput.GamePads[playerNo].RightTriggerPressed(0.2f) == true ||
                   PolyInput.GamePads[playerNo].Pressed(Buttons.RightShoulder) == true)
                {
                    if (counters.Check("dodgeCoolTimer") == false)
                    {
                        dodgeStart = true;
                        dodgeCoolBool = true;
                        counters["dodgeTimer"] = dodgeTime;
                        dodgeInstance.Play();
                    }
                } 
                else if(PolyInput.GamePads[playerNo].RightTriggerReleased(0.2f) == true ||
                        PolyInput.GamePads[playerNo].Released(Buttons.RightShoulder) == true)
                {
                    counters["dodgeTimer"] = 0.0f;
             
                }

                if(counters.Check("dodgeTimer") == false && 
                   counters.Check("dodgeCoolTimer") == false && 
                   dodgeCoolBool == true)
                {
                    dodgeCoolBool = false;
                    PlayerColour = assignColour;
                    counters["dodgeCoolTimer"] = dodgeCoolTime;
                    dodgeInstance.Stop();
                }

                if (PolyInput.GamePads[playerNo].LeftStickVertical(0.3f) > 0.1f) {
                    signY = -1;
                }
                else if (PolyInput.GamePads[playerNo].LeftStickVertical(0.3f) < -0.1f) {
                    signY = 1;
                }

                if (PolyInput.GamePads[playerNo].LeftStickHorizontal(0.3f) > 0.1f) {
                    signX = 1;
                }
                else if (PolyInput.GamePads[playerNo].LeftStickHorizontal(0.3f) < -0.1f) {
                    signX = -1;
                }
                else if(counters.Check("dodgeTimer") == false) {
                    Velocity.X = 0;
                }

                base.Movement();
            }

            if (keyboardMode == true)
            {
                signX = 0;
                signY = 0;

                boast = 0;
                if (PolyInput.Keyboard.Check(Keys.W) == true) {
                    boast = 1;
                }
                else if (Velocity.Y < 0 && counters.Check("dodgeTimer") == false) {
                    Velocity.Y = 0.0f;
                }

                if (PolyInput.Keyboard.Pressed(Keys.V))
                {
                    if (counters.Check("dodgeCoolTimer") == false)
                    {
                        dodgeStart = true;
                        dodgeCoolBool = true;
                        counters["dodgeTimer"] = dodgeTime;
                        PlayerColour = dodgeColour;
                        dodgeInstance.Play();
                    }
                }
                else if (PolyInput.Keyboard.Released(Keys.V))
                {
                    counters["dodgeTimer"] = 0.0f;
                }

                if (counters.Check("dodgeTimer") == false &&
                   counters.Check("dodgeCoolTimer") == false &&
                   dodgeCoolBool == true)
                {
                    dodgeCoolBool = false;
                    PlayerColour = assignColour;
                    counters["dodgeCoolTimer"] = dodgeCoolTime;
                    dodgeInstance.Stop();
                }

                if (PolyInput.Keyboard.Check(Keys.W) == true) {
                    signY = -1;
                }
                else if (PolyInput.Keyboard.Check(Keys.S) == true) {
                    signY = 1;
                }

                if (PolyInput.Keyboard.Check(Keys.D) == true) {
                    signX = 1;
                }
                else if (PolyInput.Keyboard.Check(Keys.A) == true) {
                    signX = -1;
                }
                else if (counters.Check("dodgeTimer") == false) {
                    Velocity.X = 0;
                }

                base.Movement();
            }
        }

        protected override void Shooting()
        {
            if (counters.Check("resetAnimation") == false) {
                isShooting = false;
            }

            if (controllerMode == true)
            {
                if (PolyInput.GamePads[playerNo].Pressed(Buttons.X) == true)
                {
                    timerSet = true;
                    isShooting = true;
                    counters["resetAnimation"] = 100.0f;
                    counters["chargeTimer1"] = 600.0f;
                    counters["chargeTimer2"] = 1200.0f;

                    BulletCreation();
                }
                else if (PolyInput.GamePads[playerNo].Check(Buttons.X) == true && timerSet == true)
                {
                    isShooting = true;
                    counters["resetAnimation"] = 100.0f;

                    if (counters.Check("chargeTimer1") == false) {
                        charged1 = true;

                        if (charged2 == false)
                        {
                            chargeInstance.Play();
                            chargeAni.PlayAnimation(charge1Animation);
                        }
                    }

                    if (counters.Check("chargeTimer2") == false) {
                        charged1 = false;
                        charged2 = true;

                        chargeInstance.Stop();
                        chargeInstance2.Play();

                        chargeAni.PlayAnimation(charge2Animation);
                    }
                }

                if (PolyInput.GamePads[playerNo].Released(Buttons.X) == true && timerSet == true) {
                    timerSet = false;
                    counters["resetAnimation"] = 100.0f;
                    BulletCreationLevel2();
                    chargeInstance.Stop();
                    chargeInstance2.Stop();
                }

                base.Shooting();
            }

            if (keyboardMode == true)
            {
                if (PolyInput.Keyboard.Pressed(Keys.C) == true)
                {
                    timerSet = true;
                    isShooting = true;
                    counters["resetAnimation"] = 100.0f;
                    counters["chargeTimer1"] = 600.0f;
                    counters["chargeTimer2"] = 1200.0f;

                    BulletCreation();
                }
                else if (PolyInput.Keyboard.Check(Keys.C) == true && timerSet == true)
                {
                    isShooting = true;
                    counters["resetAnimation"] = 100.0f;

                    if (counters.Check("chargeTimer1") == false) {
                        charged1 = true;

                        if (charged2 == false)
                        {
                            chargeInstance.Play();
                            chargeAni.PlayAnimation(charge1Animation);
                        }
                    }

                    if (counters.Check("chargeTimer2") == false) {
                        charged1 = false;
                        charged2 = true;

                        chargeInstance.Stop();
                        chargeInstance2.Play();

                        chargeAni.PlayAnimation(charge2Animation);
                    }
                }

                if (PolyInput.Keyboard.Released(Keys.C) == true) {
                    timerSet = false;
                    counters["resetAnimation"] = 100.0f;
                    BulletCreationLevel2();
                    chargeInstance.Stop();
                    chargeInstance2.Stop();
                }

                base.Shooting();
            }
        }

        public override void ShotVibration(float strength, float time)
        {
            if (controllerMode == true) {
                PolyInput.GamePads[playerNo].SetVibration(time, strength, strength);
            }
        }

        public override void HealthBarDrawing()
        {

            healthRectangle = new Rectangle(0, 0, (int)playerHealth, healthRectangle.Height);

            if (healthRectangle.Width < 0) {
                healthRectangle.Width = 0;
            }

            base.HealthBarDrawing();

            Engine.SpriteBatch.Draw(healthBarImage1, healthBarPosition, healthRectangle, Color.White);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}