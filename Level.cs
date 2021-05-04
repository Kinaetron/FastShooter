using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

using PolyOne.LevelProcessor;
using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Components;

using FastShooter.Platforms;


namespace FastShooter
{
    public enum GameTags
    {
        None = 0,
        Player1 = 1,
        Player2 = 2,
        Solid = 3,
        Bullet = 4,
        Bullet2 = 5,
        Bullet3 = 6
    }

    public class Level : Scene, IDisposable
    {
     
        LevelTiles tiles;
        LevelData levelData = new LevelData();
        LevelTiler tile = new LevelTiler();
        List<Player> players = new List<Player>();

        CameraShooter camera = new CameraShooter();

        private const float distanceFromBullet = 200.0f;
        private const float bulletExistTime = 400.0f;

        private const float distanceFromBullet2 = 150.0f;
        private const float bulletExistTime2 = 500.0f;

        private const float distanceFromBullet3 = 120.0f;
        private const float bulletExistTime3 = 600.0f;

        private const float bulletOffSet = 12.0f;

        private List<Vector2> playerPositions = new List<Vector2>();

        private Song backgroundSong;

        private CounterSet<string> counters = new CounterSet<string>();
        private bool musicBool = false;

        public Level()
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            levelData = Engine.Instance.Content.Load<LevelData>("Arena");
            tile.LoadContent(levelData);

            bool[,] collisionInfo = LevelTiler.TileConverison(tile.CollisionLayer, 65);
            tiles = new LevelTiles(collisionInfo);
            this.Add(tiles);

            for (int i = 0; i < tile.PlayerPosition.Count; i++) {

                if (i == 0) {
                    players.Add(new Player1(tile.PlayerPosition[i], i, Color.Blue, GameTags.Player1, 
                                new Vector2(tile.MapWidthInPixels, tile.MapHeightInPixels)));
                }

                if(i == 1) {
                    players.Add(new Player2(tile.PlayerPosition[i], i, Color.Green, GameTags.Player2, 
                                new Vector2(tile.MapWidthInPixels, tile.MapHeightInPixels)));
                }
            }

            this.HelperEntity.Add(counters);
            this.Add(players);

            foreach (var player in players) {
                player.Added(this);
            }

            initalizeMusic();
        }

        private void initalizeMusic()
        {
            if (tile.BackgroundSong != null)
            {
                backgroundSong = Engine.Instance.Content.Load<Song>(tile.BackgroundSong);
                if (MediaPlayer.State == MediaState.Paused) {
                    MediaPlayer.Stop();
                }
                MediaPlayer.Play(backgroundSong);
            }
        }

        public override void UnloadContent()
        {
        }

        public override void Update()
        {
            MusicManagement();
            CameraManagement();
            base.Update();
            BulletCollision();
            BulletCollisionEnemy();
        }

        private void CameraManagement()
        {
            camera.Update();
        }

        private void MusicManagement()
        {
            if(MediaPlayer.State == MediaState.Stopped)
            {
                if(musicBool == false) {
                    counters["musicCounter"] = 3000.0f;
                    musicBool = true;
                }

                if (counters.Check("musicCounter") == false) {
                    MediaPlayer.Play(backgroundSong);
                    musicBool = false;
                }
            }
        }

        private void BulletCollision()
        {
            foreach (Player player in players)
            {
                for (int i = 0; i < player.Bullets.Count; i++)
                {

                    if (player.Bullets[i].X > tile.MapWidthInPixels) {
                        player.Bullets[i].X = 0.0f;
                    }
                    if (player.Bullets[i].X < 0.0f) {
                        player.Bullets[i].X = tile.MapWidthInPixels;
                    }

                    if (player.Bullets[i].Timer > bulletExistTime)
                    {
                        player.Bullets[i].RemoveSelf();
                        player.Bullets.RemoveAt(i);
                    }
                    else if(player.Bullets[i].CollideFirst((int)GameTags.Solid, player.Bullets[i].Position) != null)
                    {
                        player.Bullets[i].Visible = false;
                        player.Bullets[i].RemoveSelf();
                        player.Bullets.RemoveAt(i);
                    }
                }

                for (int i = 0; i < player.Bullets2.Count; i++)
                {
                    if (player.Bullets2[i].X > tile.MapWidthInPixels) {
                        player.Bullets2[i].X = 0.0f;
                    }
                    if (player.Bullets2[i].X < 0.0f) {
                        player.Bullets2[i].X = tile.MapWidthInPixels;
                    }

                    if (player.Bullets2[i].Timer > bulletExistTime2)
                    {
                        player.Bullets2[i].RemoveSelf();
                        player.Bullets2.RemoveAt(i);
                    }
                    else if (player.Bullets2[i].CollideFirst((int)GameTags.Solid, player.Bullets2[i].Position) != null)
                    {
                        player.Bullets2[i].Visible = false;
                        player.Bullets2[i].RemoveSelf();
                        player.Bullets2.RemoveAt(i);
                    }
                }

                for (int i = 0; i < player.Bullets3.Count; i++)
                {
                    if (player.Bullets3[i].X > tile.MapWidthInPixels) {
                        player.Bullets3[i].X = 0.0f;
                    }
                    if (player.Bullets3[i].X < 0.0f) {
                        player.Bullets3[i].X = tile.MapWidthInPixels;
                    }


                    if (player.Bullets3[i].Timer > bulletExistTime3)
                    {
                        player.Bullets3[i].RemoveSelf();
                        player.Bullets3.RemoveAt(i);
                    }
                    else if (player.Bullets3[i].CollideFirst((int)GameTags.Solid, player.Bullets3[i].Position) != null)
                    {
                        player.Bullets3[i].Visible = false;
                        player.Bullets3[i].RemoveSelf();
                        player.Bullets3.RemoveAt(i);
                    }
                }
            }
        }

        private void BulletCollisionEnemy()
        {
            for (int i = 0; i < players[1].Bullets.Count; i++)
            {
                Vector2 distanceVector = new Vector2(players[0].Position.X, players[0].Position.Y + bulletOffSet) - players[1].Bullets[i].Position;
                float distance = distanceVector.Length();

                if (distance < distanceFromBullet) {
                    players[1].Bullets[i].Distance = distance;
                    players[1].Bullets[i].Direction = distanceVector;
                }

                if (players[1].Bullets[i].CollideFirst((int)GameTags.Player1, players[1].Bullets[i].Position) != null &&
                    players[0].Dodge == false)
                {
                    camera.ShakeCamera();
                    players[0].PlayerHealth -= Bullet.BulletDamage;
                    players[0].PlayerHit = true;

                    players[1].Bullets[i].Visible = false;
                    players[1].Bullets[i].RemoveSelf();
                    players[1].Bullets.RemoveAt(i);
                }
            }

            for (int i = 0; i < players[0].Bullets.Count; i++)
            {
                Vector2 distanceVector = new Vector2(players[1].Position.X, players[1].Position.Y + bulletOffSet) - players[0].Bullets[i].Position;
                float distance = distanceVector.Length();

                if(distance < distanceFromBullet) {
                    players[0].Bullets[i].Distance = distance;
                    players[0].Bullets[i].Direction =  distanceVector;
                }

                if (players[0].Bullets[i].CollideFirst((int)GameTags.Player2, players[0].Bullets[i].Position) != null &&
                    players[1].Dodge == false)
                {
                    camera.ShakeCamera();
                    players[1].PlayerHealth -= Bullet.BulletDamage;
                    players[1].PlayerHit = true;

                    players[0].Bullets[i].Visible = false;
                    players[0].Bullets[i].RemoveSelf();
                    players[0].Bullets.RemoveAt(i);
                }
            }

            for (int i = 0; i < players[1].Bullets2.Count; i++)
            {
                Vector2 distanceVector = new Vector2(players[0].Position.X, players[0].Position.Y + bulletOffSet) - players[1].Bullets2[i].Position;
                float distance = distanceVector.Length();

                if (distance < distanceFromBullet2) {
                    players[1].Bullets2[i].Distance = distance;
                    players[1].Bullets2[i].Direction = distanceVector;
                }

                if (players[1].Bullets2[i].CollideFirst((int)GameTags.Player1, players[1].Bullets2[i].Position) != null &&
                    players[0].Dodge == false)
                {
                    camera.ShakeCamera(10.0f);
                    players[0].PlayerHealth -= Bullet2.BulletDamage;
                    players[0].PlayerHit = true;

                    players[1].Bullets2[i].Visible = false;
                    players[1].Bullets2[i].RemoveSelf();
                    players[1].Bullets2.RemoveAt(i);
                }
            }

            for (int i = 0; i < players[0].Bullets2.Count; i++)
            {
                Vector2 distanceVector = new Vector2(players[1].Position.X, players[1].Position.Y + bulletOffSet) - players[0].Bullets2[i].Position;
                float distance = distanceVector.Length();

                if (distance < distanceFromBullet2) {
                    players[0].Bullets2[i].Direction = distanceVector;
                    players[0].Bullets2[i].Distance = distance;
                }

                if (players[0].Bullets2[i].CollideFirst((int)GameTags.Player2, players[0].Bullets2[i].Position) != null &&
                    players[1].Dodge == false)
                {
                    camera.ShakeCamera(10.0f);
                    players[1].PlayerHealth -= Bullet2.BulletDamage;
                    players[1].PlayerHit = true;

                    players[0].Bullets2[i].Visible = false;
                    players[0].Bullets2[i].RemoveSelf();
                    players[0].Bullets2.RemoveAt(i);
                }
            }

            for (int i = 0; i < players[1].Bullets3.Count; i++)
            {
                Vector2 distanceVector = new Vector2(players[0].Position.X, players[0].Position.Y + bulletOffSet) - players[1].Bullets3[i].Position;
                float distance = distanceVector.Length();

                if (distance < distanceFromBullet3) {
                    players[1].Bullets3[i].Direction = distanceVector;
                    players[1].Bullets3[i].Distance = distance;
                }

                if (players[1].Bullets3[i].CollideFirst((int)GameTags.Player1, players[1].Bullets3[i].Position) != null &&
                    players[0].Dodge == false)
                {
                    camera.ShakeCamera(15.0f);
                    players[0].PlayerHealth -= Bullet3.BulletDamage;
                    players[0].PlayerHit = true;

                    players[1].Bullets3[i].Visible = false;
                    players[1].Bullets3[i].RemoveSelf();
                    players[1].Bullets3.RemoveAt(i);
                }
            }

            for (int i = 0; i < players[0].Bullets3.Count; i++)
            {
                Vector2 distanceVector = new Vector2(players[1].Position.X, players[1].Position.Y + bulletOffSet) - players[0].Bullets3[i].Position;
                float distance = distanceVector.Length();

                if (distance < distanceFromBullet3) {
                    players[0].Bullets3[i].Direction = distanceVector;
                    players[0].Bullets3[i].Distance = distance;
                }

                if (players[0].Bullets3[i].CollideFirst((int)GameTags.Player2, players[0].Bullets3[i].Position) != null &&
                    players[1].Dodge == false)
                {
                    camera.ShakeCamera(15.0f);
                    players[1].PlayerHealth -= Bullet3.BulletDamage;
                    players[1].PlayerHit = true;

                    players[0].Bullets3[i].Visible = false;
                    players[0].Bullets3[i].RemoveSelf();
                    players[0].Bullets3.RemoveAt(i);
                }
            }
        }

        public override void Draw()
        {
            Engine.Begin(camera.TransformMatrix);
            tile.DrawBackground();

            foreach (Player player in players) {
                player.HealthBarDrawing();
            }
            base.Draw();
            Engine.End();
        }

        public bool MatchFinished(out int index)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].PlayerHealth < 2 && i == 0)
                {
                    index = i + 2;
                    Engine.Instance.Content.Unload();
                    return true;
                }
                else if (players[i].PlayerHealth < 2 && i == 1)
                {
                    index = i;
                    Engine.Instance.Content.Unload();
                    return true;
                }
            }
            index = 0;
            return false;
        }

        public void Dispose()
        {
        }
    }
}