using Microsoft.Xna.Framework;

using PolyOne.Engine;
using PolyOne.Utility;

using FastShooter.Screens;


namespace FastShooter
{
    public class FastShooter : Engine
    {

        static readonly string[] preloadAssets =
        {
            "MenuAssets/gradient",
        };

        public FastShooter()
            : base(640, 360, "Speedbuster", 2.0f, false)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            TileInformation.TileDiemensions(32, 32);

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
            ExitOnEscapeKeyPress = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            foreach (string asset in preloadAssets)
            {
                Engine.Instance.Content.Load<object>(asset);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }

    static class Program
    {
       static void Main(string[] args)
        {
            using (FastShooter game = new FastShooter())
            {
                game.Run();
            }
        }
    }
}