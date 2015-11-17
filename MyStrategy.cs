using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {
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
                backNumber ++;
                move.WheelTurn = -oldAngle;
                move.EnginePower = -1;
                number = -100;
                return;
            }
            double speedModule = Math.Sqrt(self.SpeedX * self.SpeedX + self.SpeedY * self.SpeedY);
            if (Math.Abs(speedModule) < .05)
                number ++;
            if (number > maxNumber)
            {
                backNumber = 0;
                number = 0;
                oldAngle = angleToWaypoint * 32.0D / Math.PI;
            }

            move.EnginePower = 1;
            var maxSpeed = 18;

            double max = .35D;
            double delta = speedModule/maxSpeed;
            double cornerTileOffset = max *delta * game.TrackTileSize;
            if (Math.Abs(nextWaypointX - self.X) < 55 && Math.Abs(nextWaypointY - self.Y) < 55)
            {
                switch (world.TilesXY[self.NextWaypointX][self.NextWaypointY])
                {
                    case TileType.LeftTopCorner:
                        nextWaypointX += cornerTileOffset;
                        nextWaypointY += cornerTileOffset;
                        if (speedModule > maxSpeed)
                            move.IsBrake = true;
                        move.IsUseNitro = true;
                        break;
                    case TileType.RightTopCorner:
                        nextWaypointX -= cornerTileOffset;
                        nextWaypointY += cornerTileOffset;
                        if (speedModule > maxSpeed)
                            move.IsBrake = true;
                        move.IsUseNitro = true;
                        break;
                    case TileType.LeftBottomCorner:
                        nextWaypointX += cornerTileOffset;
                        nextWaypointY -= cornerTileOffset;
                        if (speedModule > maxSpeed)
                            move.IsBrake = true;
                        move.IsUseNitro = true;
                        break;
                    case TileType.RightBottomCorner:
                        nextWaypointX -= cornerTileOffset;
                        nextWaypointY -= cornerTileOffset;
                        if (speedModule > maxSpeed)
                            move.IsBrake = true;
                        move.IsUseNitro = true;
                        break;
                }
            }
            angleToWaypoint = self.GetAngleTo(nextWaypointX, nextWaypointY);
            move.WheelTurn = angleToWaypoint * 32.0D / Math.PI;
            Console.WriteLine(String.Format("{0} - {1}", self.SpeedX, self.SpeedY));

            switch (world.TilesXY[self.NextWaypointX][self.NextWaypointY])
            {
                case TileType.LeftTopCorner:
                case TileType.RightTopCorner:
                case TileType.LeftBottomCorner:
                case TileType.RightBottomCorner:
                    move.IsUseNitro = true;
                    break;
                default:
                    move.IsUseNitro = false;
                    break;
            }

            veryOldTileType = oldTileType;
            oldTileType = world.TilesXY[self.NextWaypointX][self.NextWaypointY];
            Console.WriteLine(speedModule);

        }
    }
}