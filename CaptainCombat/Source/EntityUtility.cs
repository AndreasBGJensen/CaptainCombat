
using CaptainCombat.Source.Components;
using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
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

            return entity;            
        }

        public static Entity CreateMessage(Domain domain, string message, double x, double y, int size)
        {
            var entity = new Entity(domain);

            var transform = entity.AddComponent(new Transform());
            transform.X = x;
            transform.Y = y;

            entity.AddComponent(new Text(getFont(size), message));

            return entity;
        }

        public static Entity CreateInput(Domain domain, string message, double x, double y, int size)
        {
            var entity = new Entity(domain);

            var transform = entity.AddComponent(new Transform());
            transform.X = x;
            transform.Y = y;

            entity.AddComponent(new Input(getFont(size), message));

            return entity;
        }

        
        private static Font getFont(int size)
        {
            List<int> fontSizes = new List<int> { 12, 14, 16, 18, 20 };
            int closest = fontSizes.OrderBy(fontSize => Math.Abs(size - fontSize)).First();
            string assetTag = "PIRATE_FONT_"+ closest;
            return Assets.Fonts.PIRATE_FONT_16;
        }

    }




}
