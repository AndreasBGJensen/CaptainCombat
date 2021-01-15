﻿
using CaptainCombat.Source.Components;
using ECS;
using static ECS.Domain;

namespace CaptainCombat.Source {
    
    // Temporary (probably) utility functions
    // for constructing some simple entities
    public static class EntityUtility {
        

        public static Entity CreateRock(Domain domain, double x, double y, double scale = 0, double rotation = 0) {
            var entity = new Entity(domain);

            var transform = entity.AddComponent(new Transform());
            transform.Position = new Vector(x, y);
            transform.ScaleX = scale;
            transform.ScaleY = scale;
            transform.Rotation = rotation;

            entity.AddComponent(new Sprite(Assets.Textures.ROCK, 100, 100));

            var collider = entity.AddComponent(new BoxCollider());
            collider.Width = 60 * scale;
            collider.Height = 45 * scale;
            collider.Rotation = 10;
            collider.Tag = Assets.ColliderTags.ROCK;

            entity.SetSyncMode(Component.SynchronizationMode.NONE);

            return entity;            
        }


        public static Entity FireCannonBall(Entity player) {

            var playerTransform = player.GetComponent<Transform>();

            var cannonBall = new Entity(player.Domain);

            var transform = cannonBall.AddComponent(new Transform());

            var playerPosition = playerTransform.Position;
            transform.Position = playerPosition + Vector.CreateDirection(playerTransform.Rotation) * 30;
            transform.Rotation = playerTransform.Rotation + 90;
            
            var move = cannonBall.AddComponent(new Move());
            move.ForwardVelocity = true;
            move.Velocity = new Vector(500, 0);

            var projectile = cannonBall.AddComponent(new Projectile());

            cannonBall.AddComponent(new Sprite(Assets.Textures.CANNON_BALL, 15, 15));

            cannonBall.AddComponent(new CircleCollider(Assets.ColliderTags.PROJECTILE, 10));

            cannonBall.SetSyncMode(Component.SynchronizationMode.CREATE);
            projectile.SyncMode = Component.SynchronizationMode.NONE;

            return null;
        }

    }




}
