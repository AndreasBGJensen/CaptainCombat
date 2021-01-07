﻿using CaptainCombat.Source.Components;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CaptainCombat.Source {



    public static class Renderer {

        private static GraphicsDevice graphics;
        private static SpriteBatch spriteBatch;


        public static void Initialize(GraphicsDevice graphics) {
            Renderer.graphics = graphics;
            spriteBatch = new SpriteBatch(graphics);
        }


        public static void RenderSprites(Domain domain) {

            spriteBatch.Begin();          

            // Submit all entities which have a Sprite and Transform component
            // to the sprite batch (for drawing)
            domain.ForMatchingEntities<Sprite, Transform>((entity) => {

                var sprite = entity.GetComponent<Sprite>();
                var transform = entity.GetComponent<Transform>();

                var texture = sprite.Texture.MGTexture;

                float width = (float)(sprite.Width * transform.ScaleX);
                float height = (float)(sprite.Height * transform.ScaleY);

                // Draw the texture
                spriteBatch.Draw(
                       texture,

                       // Render position and size
                       new Rectangle((int)transform.X, (int)transform.Y, (int)width, (int)height),
                       null,

                       // Tint (not implemented in Sprite component yet
                       Color.White, 

                       // Rotation (radians)
                       (float)(transform.Rotation * Math.PI/180), // Rotation

                       // Origin offset (relative to MG Texture)
                       new Vector2(texture.Width/2.0f, texture.Width/2.0f),

                       SpriteEffects.None,
                       1
                );
            });


            spriteBatch.End();
            
        }



    }
}
