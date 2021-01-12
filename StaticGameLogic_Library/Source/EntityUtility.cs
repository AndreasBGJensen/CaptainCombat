﻿



using Source.ECS;
using StaticGameLogic_Library.Components;
using taticGameLogic_Library.Source;
using static Source.ECS.Domain;

namespace Source.EntityUtility
{
    
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

            return entity;            
        }

    }




}
