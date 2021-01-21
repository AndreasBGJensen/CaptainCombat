
using System;
using System.Collections.Generic;
using System.Linq;
using CaptainCombat.Common.Components;
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common {

    // Temporary (probably) utility functions
    // for constructing some simple entities
    public static class EntityUtility {

        public static Entity CreateBoundary(Domain domain, double x, double y, double width, double height) {

            var entity = new Entity(domain);
            entity.AddComponent(new Transform(x, y));

            entity.AddComponent(new Sprite(Assets.Textures.SQUARE, width, height, Settings.BOUNDARY_COLOR));

            entity.AddComponent(new BoxCollider(Assets.ColliderTags.ROCK, width, height));

            entity.SetSyncMode(Component.SynchronizationMode.CREATE);

            return entity;
        }


        public static Entity CreateRock(Domain domain, double x, double y, double scale = 0, double rotation = 0) {
            var entity = new Entity(domain);

            var transform = entity.AddComponent(new Transform());
            transform.Position = new Vector(x, y);
            transform.ScaleX = scale;
            transform.ScaleY = scale;
            transform.Rotation = rotation;

            var sprite = entity.AddComponent(new Sprite(Assets.Textures.ROCK, 100, 100));
            sprite.Depth = 0.75f;

            var collider = entity.AddComponent(new BoxCollider());
            collider.Width = 60 * scale;
            collider.Height = 45 * scale;
            collider.Rotation = 10;
            collider.Tag = Assets.ColliderTags.ROCK;

            entity.SetSyncMode(Component.SynchronizationMode.CREATE);

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

            var sprite = cannonBall.AddComponent(new Sprite(Assets.Textures.CANNON_BALL, 15, 15));
            sprite.Depth = 0.20f;


            cannonBall.AddComponent(new CircleCollider(Assets.ColliderTags.PROJECTILE, 10));

            cannonBall.SetSyncMode(Component.SynchronizationMode.CREATE);
            projectile.SyncMode = Component.SynchronizationMode.NONE;

            return null;
        }

        public static Entity MenuArrow(Domain domain, bool left)
        {

            var entity = new Entity(domain);
            entity.AddComponent(new Transform());
            int width = 50;
            int height = 25; 
            if (left)
            {
                entity.AddComponent(new Sprite(Assets.Textures.Arrow_left, width, height));
            }
            else
            {
                entity.AddComponent(new Sprite(Assets.Textures.Arrow_right, width, height));
            }
            return entity;
        }



        public static Entity CreateMessage(Domain domain, string message, double x, double y, int size, TextOrigin origin = TextOrigin.Center)
        {
            var entity = new Entity(domain);

            var transform = entity.AddComponent(new Transform());
            transform.Position.X = x;
            transform.Position.Y = y;

            entity.AddComponent(new Text(getFont(size), message, origin));

            return entity;
        }

        public static Entity CreateInput(Domain domain, string message, double x, double y, int size)
        {
            var entity = new Entity(domain);

            var transform = entity.AddComponent(new Transform());
            transform.Position.X = x;
            transform.Position.Y = y;

            entity.AddComponent(new Input(getFont(size), message));

            return entity;
        }

        
        private static Font getFont(int size)
        {
            List<int> fontSizes = new List<int> { 12, 14, 16, 18, 20, 22, 30, 40 };
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
            move.RotationResistance = 0.80;
            move.ForwardVelocity = true;

            var health = entity.AddComponent(new ShipHealth());
            health.Max = 100;
            health.Current = health.Max;

            var collider = entity.AddComponent(new BoxCollider());
            collider.Width = 40;
            collider.Height = 80;
            collider.Tag = Assets.ColliderTags.SHIP;

            var sprite = AddSpriteToShip(entity, clientId, damageLevel);
            sprite.Depth = 0.25f;
            

            return entity;
        }


        public static Sprite AddSpriteToShip(Entity entity, int clientId, int damageLevel)
        {
            int shipWidth = 66;
            int shipHeight = 113;

            switch (clientId)
            {
                case 1:
                    {
                        string assetTag = "blue_ship_" + damageLevel;
                        return entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                case 2:
                    {
                        string assetTag = "yellow_ship_" + damageLevel;
                        return entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                case 3:
                    {
                        string assetTag = "green_ship_" + damageLevel;
                        return entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                case 4:
                    {
                        string assetTag = "red_ship_" + damageLevel;
                        return entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                case 5:
                    {
                        string assetTag = "white_ship_" + damageLevel;
                        return entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
                default:
                    {
                        string assetTag = "black_ship_" + damageLevel;
                        return entity.AddComponent(new Sprite((Texture)Asset.GetAsset(assetTag), shipWidth, shipHeight));
                    }
            }

        }



    }




}
