using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        private int number = 0;
        private int maxNumber = 100;
        private double oldAngle;
        private int backNumber = 0;
        private int maxBackNumber = 100;
        private TileType oldTileType;
        private TileType veryOldTileType;
        private double maxSpeed = 15;

        public void Move(Car self, World world, Game game, Move move)
        {
            double nextWaypointX = (self.NextWaypointX + 0.5D) * game.TrackTileSize;
            double nextWaypointY = (self.NextWaypointY + 0.5D) * game.TrackTileSize;

            double angleToWaypoint = self.GetAngleTo(nextWaypointX, nextWaypointY);

            if (backNumber < maxBackNumber)
            {
                backNumber++;
                move.WheelTurn = -oldAngle;
                move.EnginePower = -1;
                number = -100;
                return;
            }
            double speedModule = Math.Sqrt(self.SpeedX * self.SpeedX + self.SpeedY * self.SpeedY);
            if (Math.Abs(speedModule) < .05)
                number++;
            if (number > maxNumber)
            {
                backNumber = 0;
                number = 0;
                oldAngle = angleToWaypoint * 32.0D / Math.PI;
            }

            move.EnginePower = 1;
            var maxSpeed = 18;

            double max = .35D;
            double delta = speedModule / maxSpeed;
            double cornerTileOffset = max * delta * game.TrackTileSize;
            if (Math.Abs(nextWaypointX - self.X) < 55 && Math.Abs(nextWaypointY - self.Y) < 55)
            {
                angleToWaypoint = AngelToWayPoint(self, world, game, move);
            }
            move.WheelTurn = angleToWaypoint * 32.0D / Math.PI;
            Console.WriteLine(String.Format("{0} - {1}", self.SpeedX, self.SpeedY));

            move.IsUseNitro = IsUseNitro(self, world, game, move);
            move.IsThrowProjectile = IsThrow(self, world, game, move);
            move.IsSpillOil = IsSpillOil(self, world, game, move);

            veryOldTileType = oldTileType;
            oldTileType = world.TilesXY[self.NextWaypointX][self.NextWaypointY];
            Console.WriteLine(speedModule);

        }


        private bool IsThrow(Car self, World world, Game game, Move move)
        {
            int maxShootDistance = 10;
            if (self.ProjectileCount <= 0)
            {
                return false;
            }

            foreach (Car c in world.Cars)
            {
                if (c.IsTeammate)
                {
                    continue;
                }

                double enemyCarAngel = self.GetAngleTo(c.X, c.Y);
                Console.WriteLine(self.GetDistanceTo(c));
                if (self.GetDistanceTo(c) > maxShootDistance)
                {
                    continue;
                }

                if (Math.Abs(enemyCarAngel) < 0.1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSpillOil(Car self, World world, Game game, Move move)
        {
            int currentTileX = Convert.ToInt32(self.X / game.TrackTileSize);
            int currentTileY = Convert.ToInt32(self.Y / game.TrackTileSize);

            switch (world.TilesXY[currentTileX][currentTileY])
            {
                case TileType.LeftBottomCorner:
                case TileType.LeftTopCorner:
                case TileType.RightBottomCorner:
                case TileType.RightTopCorner:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsUseNitro(Car self, World world, Game game, Move move)
        {
            int currentTileX = Convert.ToInt32(self.X / game.TrackTileSize);
            int currentTileY = Convert.ToInt32(self.Y / game.TrackTileSize);

            switch (world.TilesXY[currentTileX][currentTileY])
            {
                case TileType.Crossroads:
                case TileType.Horizontal:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsUseBreak(Car self, World world, Game game, Move move)
        {
            double nextWaypointX = (self.NextWaypointX + 0.5D) * game.TrackTileSize;
            double nextWaypointY = (self.NextWaypointY + 0.5D) * game.TrackTileSize;

            switch (world.TilesXY[self.NextWaypointX][self.NextWaypointY])
            {
                case TileType.LeftBottomCorner:
                case TileType.LeftTopCorner:
                case TileType.RightBottomCorner:
                case TileType.RightTopCorner:
                    return true;
                default:
                    return false;
            }
        }

        private double AngelToWayPoint(Car self, World world, Game game, Move move)
        {
            double nextWaypointX = (self.NextWaypointX + 0.5D) * game.TrackTileSize;
            double nextWaypointY = (self.NextWaypointY + 0.5D) * game.TrackTileSize;

            double cornerTileOffset = 0.37D * game.TrackTileSize;

            switch (world.TilesXY[self.NextWaypointX][self.NextWaypointY])
            {
                case TileType.LeftTopCorner:
                    move.EnginePower = 1.0D;
                    nextWaypointX += cornerTileOffset;
                    nextWaypointY += cornerTileOffset;
                    break;
                case TileType.RightTopCorner:
                    move.EnginePower = 1.0D;
                    nextWaypointX -= cornerTileOffset;
                    nextWaypointY += cornerTileOffset;
                    break;
                case TileType.LeftBottomCorner:
                    move.EnginePower = 1.0D;
                    nextWaypointX += cornerTileOffset;
                    nextWaypointY -= cornerTileOffset;
                    break;
                case TileType.RightBottomCorner:
                    move.EnginePower = 1.0D;
                    nextWaypointX -= cornerTileOffset;
                    nextWaypointY -= cornerTileOffset;
                    break;
            }

            return self.GetAngleTo(nextWaypointX, nextWaypointY);
        }
    }
}
