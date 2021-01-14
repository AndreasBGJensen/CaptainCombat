
using CaptainCombat.Source.Components;
using CaptainCombat.Source.Utility;
using ECS;
using Microsoft.Xna.Framework;
using static ECS.Domain;

namespace CaptainCombat.Source {
    
    // Temporary (probably) utility functions
    // for constructing some simple entities
    public static class EntityUtility {
        

        public static Entity CreateRock(Domain domain, double x, double y, double scale = 0, double rotation = 0) {
            var entity = new Entity(domain);

            var transform = entity.AddComponent(new Transform());
            transform.X = x;
            transform.Y = y;
            transform.ScaleX = scale;
            transform.ScaleY = scale;
            transform.Rotation = rotation;

            entity.AddComponent(new Sprite(Assets.Textures.ROCK, 100, 100));

            var collider = entity.AddComponent(new BoxCollider());
            collider.Width = 60 * scale;
            collider.Height = 45 * scale;
            collider.Rotation = 10;
            collider.Tag = Assets.ColliderTags.ROCK;

            return entity;            
        }


        public static Entity FireCannonBall(Entity player) {

            var playerTransform = player.GetComponent<Transform>();

            var cannonBall = new Entity(player.Domain);

            var transform = cannonBall.AddComponent(new Transform());

            var playerPosition = new Vector2((float)playerTransform.X, (float)playerTransform.Y);
            var normPosition = playerPosition;
            normPosition.Normalize();
            var position = playerPosition + new Vector2(30.0f, 0.0f).WithDirection((float)playerTransform.Rotation);

            transform.X = position.X;
            transform.Y = position.Y;
            transform.Rotation = playerTransform.Rotation + 90;
            
            var move = cannonBall.AddComponent(new Move());
            move.ForwardVelocity = true;
            move.Velocity = new Vector2(500, 0);

            cannonBall.AddComponent(new Sprite(Assets.Textures.CANNON_BALL, 15, 15));

            cannonBall.AddComponent(new CircleCollider(Assets.ColliderTags.PROJECTILE, 10));

            cannonBall.SetSyncMode(Component.SynchronizationMode.CREATE);

            return null;
        }

    }




}
