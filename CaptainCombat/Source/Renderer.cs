using CaptainCombat.Source.Components;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            domain.ForMatchingEntities<Sprite, Transform>((entity) => {

                var sprite = entity.GetComponent<Sprite>();
                var transform = entity.GetComponent<Transform>();

                var texture = sprite.Texture.MGTexture;

                float width = (float)(sprite.Width * transform.ScaleX);
                float height = (float)(sprite.Height * transform.ScaleY);

                // Draw the texture
                spriteBatch.Draw(
                       texture,
                       new Rectangle((int)transform.X, (int)transform.Y, (int)width, (int)height),
                       null,
                       Color.White,
                       0, // Rotation
                       new Vector2(texture.Width/2.0f, texture.Width/2.0f),
                       SpriteEffects.None,
                       1
                );
            });


            spriteBatch.End();
            
        }



    }
}
