﻿
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
            Font defaultFont = Assets.Fonts.PIRATE_FONT_16; 
            string assetTag = "PIRATE_FONT_"+ closest;
            return (Font)Asset.GetAsset(assetTag); 
        }

        public static Entity CreateIcon(Domain domain, int clientId)
        {
            var entity = new Entity(domain);
            entity.AddComponent(new Transform());
            int iconsize = 25; 



            switch (clientId)
            {
                case 1:
                    entity.AddComponent(new Sprite(Assets.Textures.blue_icon, iconsize, iconsize));
                    break;
                case 2:
                    entity.AddComponent(new Sprite(Assets.Textures.yellow_icon, iconsize, iconsize));
                    break;
                case 3:
                    entity.AddComponent(new Sprite(Assets.Textures.green_icon, iconsize, iconsize));
                    break;
                case 4:
                    entity.AddComponent(new Sprite(Assets.Textures.red_icon, iconsize, iconsize));
                    break;
                case 5:
                    entity.AddComponent(new Sprite(Assets.Textures.white_icon, iconsize, iconsize));
                    break;
                default:
                    entity.AddComponent(new Sprite(Assets.Textures.black_icon, iconsize, iconsize));
                    break;
            }
            return entity;
        }

        public static Entity CreateShip(Domain domain, int clientId, int damage)
        {
            List<int> levels = new List<int> { 1, 2, 3, 4 };
            int damageLevel = levels.OrderBy(level => Math.Abs(damage - level)).First();

            var entity = new Entity(domain);
            entity.AddComponent(new Transform());

            var move = entity.AddComponent(new Move());
            move.Resistance = 0.25;
            move.RotationResistance = 0.75;
            move.ForwardVelocity = true;

            AddSpriteToShip(entity, clientId, damageLevel); 

            return entity;
        }

        public static void AddSpriteToShip(Entity entity, int clientId, int damageLevel)
        {
            int shipWidth = 66;
            int shipHeight = 113;

            switch (clientId)
            {
                case 1:
                    {
                        string assetTag = "blue_ship_" + damageLevel;
                        entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                    break;
                case 2:
                    {
                        string assetTag = "yellow_ship_" + damageLevel;
                        entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                    break;
                case 3:
                    {
                        string assetTag = "green_ship_" + damageLevel;
                        entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                    break;
                case 4:
                    {
                        string assetTag = "red_ship_" + damageLevel;
                        entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                    break;
                case 5:
                    {
                        string assetTag = "white_ship_" + damageLevel;
                        entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                    break;
                default:
                    {
                        string assetTag = "black_ship_" + damageLevel;
                        entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                    break;
            }

        }



    }




}
