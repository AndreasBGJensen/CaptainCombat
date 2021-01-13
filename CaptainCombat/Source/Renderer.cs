using CaptainCombat.Source.Components;
using CaptainCombat.Source.Utility;
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



        /// <summary>
        /// Renders all Colliders to the screen
        /// </summary>
        public static void RenderColliders(Domain domain, Camera camera) {
            spriteBatch.Begin(transformMatrix: camera.GetMatrix());

            // Render box colliders
            domain.ForMatchingEntities<Transform, BoxCollider>((e) => {
                var transform = e.GetComponent<Transform>();
                var collider = e.GetComponent<BoxCollider>();

                // Color of collider box
                var color =
                      collider.Collided ? Color.Red
                    : collider.Enabled ? Color.Yellow
                    : new Color(0.75f, 0.75f, 0.75f);

                var texture = Assets.Textures.LINE_SQUARE.GetNative<Texture2D>();

                spriteBatch.Draw(
                       texture,
                       new Vector2((float)transform.X, (float)transform.Y),
                       null,
                       color,
                       // Rotation (radians)
                       (float)((transform.Rotation + collider.Rotation) * Math.PI / 180), // Rotation

                       // Origin offset (relative to MG Texture)
                       new Vector2(texture.Width / 2.0f, texture.Height / 2.0f),

                       // Scale sprite size to the desired width and height
                       new Vector2((float)(collider.Width) / texture.Width, (float)(collider.Height) / texture.Height),

                       SpriteEffects.None,
                       1
                );
            });

            // Render circle colliders
            domain.ForMatchingEntities<Transform, CircleCollider>((e) => {

                var transform = e.GetComponent<Transform>();
                var collider = e.GetComponent<CircleCollider>();
                
                var position = new Vector2((float)transform.X, (float)transform.Y); // Draw the texture

                var color =
                      collider.Collided ? Color.Red
                    : collider.Enabled ? Color.Yellow
                    : new Color(0.75f, 0.75f, 0.75f, 1.0f);
                
                var texture = Assets.Textures.LINE_CIRCLE.GetNative<Texture2D>();

                spriteBatch.Draw(
                       texture,
                       position,
                       null,
                       color,
                       (float)((transform.Rotation) * Math.PI / 180), // Rotation

                       // Origin offset (relative to MG Texture)
                       new Vector2(texture.Width / 2.0f, texture.Height / 2.0f),

                       // Scale sprite size to the desired width and height
                       new Vector2((float)(collider.Radius*2)/texture.Width, (float)(collider.Radius*2)/texture.Height),

                       SpriteEffects.None,
                       1
                );
            });

            spriteBatch.End();
        }



    }
}
