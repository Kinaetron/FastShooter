using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne.Engine;
using PolyOne.ScreenManager;
using PolyOne.Utility;



namespace FastShooter.Screens
{
    class BackgroundScreen : GameScreen
    {
        Texture2D backgroundTexture;

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            backgroundTexture = Engine.MenuContentManager.Load<Texture2D>("MenuAssets/background");
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                     bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
             Rectangle fullscreen = new Rectangle(0, 0, Engine.ActualWidth, Engine.ActualHeight);

            Engine.Begin();

            Engine.SpriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            Engine.End();
        }
    }
}
