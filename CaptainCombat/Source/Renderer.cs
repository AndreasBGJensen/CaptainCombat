using CaptainCombat.Source.Components;
using StaticGameLogic_Library.Source.Components;
using Source.ECS;
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


        public static void RenderSprites(Domain domain, Camera camera) {
            spriteBatch.Begin(transformMatrix: camera.GetMatrix());


            // Submit all entities which have a Sprite and Transform component
            // to the sprite batch (for drawing)
            domain.ForMatchingEntities<Sprite, Transform>((entity) => {

                var sprite = entity.GetComponent<Sprite>();
                var transform = entity.GetComponent<Transform>();

                var texture = sprite.Texture.GetNative<Texture2D>();

                float width = (float)(sprite.Width * transform.ScaleX);
                float height = (float)(sprite.Height * transform.ScaleY);
          
                // Draw the texture
                spriteBatch.Draw(
                       texture,

                       // Render position and size
                        new Vector2((float)transform.X, (float)transform.Y),
                       //new Rectangle((int)transform.X, (int)transform.Y, (int)width, (int)height),

                       // "Texture Coordinates" (null for full texture)
                       null,

                       // Tint (not implemented in Sprite component yet
                       Color.White,

                       // Rotation (radians)
                       (float)((transform.Rotation+ sprite.Texture.Rotation) * Math.PI / 180), // Rotation

                       // Origin offset (relative to MG Texture)
                       new Vector2(texture.Width / 2.0f, texture.Height / 2.0f),

                       // Scale sprite size to the desired width and height
                       new Vector2(width/texture.Width, height/texture.Height),

                       SpriteEffects.None,
                       1
                );
            });

            spriteBatch.End();
            
        }



    }
}
