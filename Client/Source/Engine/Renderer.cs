using CaptainCombat.Common.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using CaptainCombat.Common;

using MGColor = Microsoft.Xna.Framework.Color;

namespace CaptainCombat.Client {

    public static class Renderer {

        private static GraphicsDevice graphics;
        private static SpriteBatch spriteBatch;

        public static void Initialize(GraphicsDevice graphics) {
            Renderer.graphics = graphics;
            spriteBatch = new SpriteBatch(graphics);
        }


        public static void RenderSprites(Domain domain, Camera camera) {
            spriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: camera.GetMatrix().ToMGMatrix());

            // Submit all entities which have a Sprite and Transform component
            // to the sprite batch (for drawing)
            domain.ForMatchingEntities<Sprite, Transform>((entity) => {

                var sprite = entity.GetComponent<Sprite>();
                var transform = entity.GetComponent<Transform>();

                if (!sprite.Enabled) return;

                var texture = sprite.Texture.GetNative<Texture2D>();

                float width = (float)(sprite.Width * transform.ScaleX);
                float height = (float)(sprite.Height * transform.ScaleY);

                // Draw the texture
                spriteBatch.Draw(
                       texture,

                       // Render position and size
                       transform.Position.ToMGVector(),

                       // "Texture Coordinates" (null for full texture)
                       null,

                       // Tint (not implemented in Sprite component yet
                       sprite.Color.ToMGColor(),

                       // Rotation (radians)
                       (float)((transform.Rotation + sprite.Texture.Rotation) * Math.PI / 180), // Rotation

                       // Origin offset (relative to MG Texture)
                       new Vector2(texture.Width / 2.0f, texture.Height / 2.0f),

                       // Scale sprite size to the desired width and height
                       new Vector2(width / texture.Width, height / texture.Height),

                       SpriteEffects.None,

                       sprite.Depth
                );
            });

            spriteBatch.End();
            
        }

        public static void RenderText(Domain domain, Camera camera)
        {
            spriteBatch.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: camera.GetMatrix().ToMGMatrix());

            // Submit all entities which have a Sprite and Transform component
            // to the sprite batch (for drawing)
            domain.ForMatchingEntities<Text, Transform>((entity) => {

                var text = entity.GetComponent<Text>();             
                var transform = entity.GetComponent<Transform>();
                var font = text.FONT.GetNative<SpriteFont>();

                var textWidth = font.MeasureString(text.Message).X;

                // Adjust drawing origin
                float offset = 0;
                if (text.Origin == TextOrigin.Left)  offset = 0;
                if (text.Origin == TextOrigin.Right) offset = -textWidth;
                if (text.Origin == TextOrigin.Center) offset = -textWidth/2.0f;

                spriteBatch.DrawString(
                    // Font 
                    font,
                    // Text 
                    text.Message,
                    // Position 
                    transform.Position.ToMGVector() + new Vector2(offset, 0),
                     // Color 
                     MGColor.Black);
            });
            spriteBatch.End();
        }

        public static void RenderInput(Domain domain, Camera camera)
        {
            spriteBatch.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: camera.GetMatrix().ToMGMatrix());
            //spriteBatch.Begin();

            // Submit all entities which have a Sprite and Transform component
            // to the sprite batch (for drawing)
            domain.ForMatchingEntities<Input, Transform>((entity) => {

                var input = entity.GetComponent<Input>();
                var transform = entity.GetComponent<Transform>();
                var font = input.FONT.GetNative<SpriteFont>();

                spriteBatch.DrawString(
                    // Font 
                    font,
                    // Text 
                    input.Message,
                    // Position 
                    transform.Position.ToMGVector(),
                    // Color 
                    MGColor.Black
                    );
            });
            spriteBatch.End();
        }

        /// <summary>
        /// Renders all Colliders to the screen
        /// </summary>
        public static void RenderColliders(Domain domain, Camera camera) {


            spriteBatch.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: camera.GetMatrix().ToMGMatrix());

            // Render box colliders
            domain.ForMatchingEntities<Transform, BoxCollider>((e) => {
                var transform = e.GetComponent<Transform>();
                var collider = e.GetComponent<BoxCollider>();

                // Color of collider box
                var color =
                      collider.Collided ? MGColor.Red
                    : collider.Enabled ? MGColor.Yellow
                    : new MGColor(0.75f, 0.75f, 0.75f);

                var texture = Assets.Textures.LINE_SQUARE.GetNative<Texture2D>();

                spriteBatch.Draw(
                       texture,
                       transform.Position.ToMGVector(),
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
                
                var position = transform.Position.ToMGVector(); // Draw the texture

                var color =
                      collider.Collided ? MGColor.Red
                    : collider.Enabled ? MGColor.Yellow
                    : new MGColor(0.75f, 0.75f, 0.75f, 1.0f);
                
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
