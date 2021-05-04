using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

using PolyOne.ScreenManager;
using PolyOne.Components;
using System.Collections;
using PolyOne.Engine;

namespace FastShooter.Screens
{
    class GameplayScreen : GameScreen
    {
        float pauseAlpha;
        int index;

        Level level = new Level();

        Coroutine coroutine;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            ScreenManager.Game.ResetElapsedTime();
            level.LoadContent();

            coroutine = new Coroutine(MatchEnd());
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive == true)
            {
                if (MediaPlayer.State == MediaState.Paused) {
                    MediaPlayer.Resume();
                }

                level.Update();
            }
            else
            {

                if (MediaPlayer.State == MediaState.Playing) {
                    MediaPlayer.Pause();
                }
            }
        }

        public override void HandleInput(InputMenuState input)
        {
            if (input.IsPauseGame(ControllingPlayer)) {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            if (level.MatchFinished(out index) == true) {

                if (coroutine.Finished == false) {
                    coroutine.Update();
                }
            }
        }

        IEnumerator MatchEnd()
        {
            yield return 200;
            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new VictoryScreen(index), ControllingPlayer);
        }



        public override void Draw(GameTime gameTime)
        {
            level.Draw();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
