using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using PolyOne.Engine;
using PolyOne.Utility;

namespace FastShooter
{
    public class CameraShooter : Camera
    {
        private float priorCameraX = 0.0f;
        private float priorCameraY = 0.0f;

        private float shakeTimer = 0.0f;
        private float shakeFrames = 0.0f;

        private bool shakeCamera = false;

        private Random random = new Random();

        public void ShakeCamera(float shakeDuration = 5.0f)
        {
           if(shakeCamera == false)
            {
                priorCameraX = Position.X;
                priorCameraY = Position.Y;

                shakeCamera = true;
                shakeFrames = shakeDuration;
            }
        }

        public void Update()
        {
            if(shakeCamera == true)
            {
                Position.X = priorCameraX + (2 * (float)random.NextDouble());
                Position.Y = priorCameraY + (2 * (float)random.NextDouble());

                shakeTimer++;
                if(shakeTimer >= shakeFrames)
                {
                    shakeCamera = false;
                    shakeTimer = 0.0f;
                    shakeFrames = 0.0f;

                    Position.X = priorCameraX;
                    Position.Y = priorCameraY;
                }
            }
        }

        /*

        steeds crappy screen shaaaaake


        
            
        public function shake(x:int, y:int, shakeSource:Point = null):void {
			if(!refresh) return;
			// sourced shakes drop off in intensity by distance
			// it stops the player feeling like they're in a cocktail shaker
			if(shakeSource){
				var dist:Number = Math.abs(game.level.data.player.x - shakeSource.x) + Math.abs(game.level.data.player.x - shakeSource.y);
				if(dist >= SHAKE_DIST_MAX) return;
				x = x* (SHAKE_DIST_MAX - dist) * INV_SHAKE_DIST_MAX;
				y = y* (SHAKE_DIST_MAX - dist) * INV_SHAKE_DIST_MAX;
				if(x == 0 && y == 0) return;
			}
			// ignore lesser shakes
			if(Math.abs(x) < Math.abs(shakeOffset.x)) return;
			if(Math.abs(y) < Math.abs(shakeOffset.y)) return;
			shakeOffset.x = x;
			shakeOffset.y = y;
			shakeDirX = x > 0 ? 1 : -1;
			shakeDirY = y > 0 ? 1 : -1;
		}


private function updateShaker():void {
			// shake first
			if(shakeOffset.y != 0){
				shakeOffset.y = -shakeOffset.y;
				if(shakeDirY == 1 && shakeOffset.y > 0) shakeOffset.y--;
				if(shakeDirY == -1 && shakeOffset.y< 0) shakeOffset.y++;
			}
			if(shakeOffset.x != 0){
				shakeOffset.x = -shakeOffset.x;
				if(shakeDirX == 1 && shakeOffset.x > 0) shakeOffset.x--;
				if(shakeDirX == -1 && shakeOffset.x< 0) shakeOffset.x++;
			}
		}


        */
    }
}
