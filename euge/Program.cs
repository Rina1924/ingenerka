using System;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;

namespace pacman
{
    internal class Program
    {
        private const int ScreenWidth = 150;
        private const int ScreenHeight = 50;

        private const int MapWidth = 32;
        private const int MapHeight = 32;

        private static double _playerX;
        private static double _playerY;
        private static double _playerA = 0;

        private const double Fov = Math.PI / 2;
        private const double Depth = 16; 
        

        private static string _map = "";

        private static readonly char[] Screen = new char[ScreenWidth * ScreenHeight];

        static void Main(string[] args)
        {
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.SetBufferSize(ScreenWidth, ScreenHeight);

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            _map += "################################";
            _map += "#..............p...............#";
            _map += "#..............................#";
            _map += "#..............$...............#";
            _map += "#.......................$......#";
            _map += "#..............................#";
            _map += "#...............$..............#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "################################";

            InitializePlayerPosition();
            InitilizeMoney();

            DateTime dateTimeFrom = DateTime.Now;

            char c = ' ';

            while (true)
            {
                DateTime dateTimeTo = DateTime.Now;
                double elapsedTime = (dateTimeTo - dateTimeFrom).TotalSeconds;
                dateTimeFrom = DateTime.Now;

                if (Console.KeyAvailable)
                {
                    var ConsoleKey = Console.ReadKey(true).Key;

                    switch(ConsoleKey)
                    {
                        case ConsoleKey.LeftArrow:
                            _playerA += 30 * elapsedTime;
                            break; 
                        case ConsoleKey.RightArrow:
                            _playerA -= 30 * elapsedTime;
                            break;
                        case ConsoleKey.D:
                            MovePlayer(-Math.Cos(_playerA) * 50 * elapsedTime, Math.Sin(_playerA) * 50 * elapsedTime );
                            break;
                        case ConsoleKey.A:
                            MovePlayer(Math.Cos(_playerA) * 50 * elapsedTime, -Math.Sin(_playerA) * 50 * elapsedTime);
                            break;
                        case ConsoleKey.W:
                            MovePlayer(Math.Sin(_playerA) * 70 * elapsedTime, Math.Cos(_playerA) * 70 * elapsedTime);
                            break;
                        case ConsoleKey.S:
                            MovePlayer(-Math.Sin(_playerA) * 70 * elapsedTime, -Math.Cos(_playerA) * 70 * elapsedTime);
                            break;
                    }
                }

                RenderFrame();
            }
        }

        private static void InitilizeMoney()
        {
            
        }

        private static void InitializePlayerPosition()
        {
            for (int y = 0; y < MapHeight; y++)
            {
                for(int x = 0; x < MapWidth; x++)
                {
                    if (_map[y* MapWidth + x] == 'p')
                    {
                        _playerX = x;
                        _playerY = y;
                        return;
                    }
                }
            }
        }

        private static void MovePlayer(double deltaX, double deltaY)
        {
            double newX = _playerX + deltaX;
            double newY = _playerY + deltaY;

            if (_map[(int)newY * MapWidth + (int)newX] != '#')
            {
                _playerX = newX;
                _playerY = newY;
            }
            if (_map[(int)newY * MapWidth + (int)newX] != '$')
            {
                _playerX = newX;
                _playerY = newY;
            }
        }
        private static void RenderFrame()
        {
            for (int x = 0; x < ScreenWidth; x++)
            {
                double rayAngle = _playerA + Fov / 2 - x * Fov / ScreenWidth; 

                double rayX = Math.Sin(rayAngle);
                double rayY = Math.Cos(rayAngle);

                double distanceToMoney = 0;
                bool hitMoney = false;

                while (!hitMoney && distanceToMoney < Depth)
                {
                    distanceToMoney += 0.1;

                    int testX = (int)(_playerX + rayX * distanceToMoney);
                    int testY = (int)(_playerY + rayY * distanceToMoney);

                    if (testX < 0 || testY >= MapWidth || testY < 0 || testY >= MapHeight)
                    {
                        hitMoney = true;
                        distanceToMoney = Depth;
                    }
                    else
                    {
                        char testCell = _map[testY * MapWidth + testX];
                        if (testCell == '$')
                        {
                            hitMoney = false;
                        }
                    }
                }

                double distanceToWall = 0;
                bool hitWall = false;

                while (!hitWall && distanceToWall < Depth)
                {
                    distanceToWall += 0.1;
                    
                    int testX = (int)(_playerX + rayX * distanceToWall);
                    int testY = (int)(_playerY + rayY * distanceToWall);

                    if (testX < 0 || testY >= MapWidth || testY < 0 || testY >= MapHeight)
                    {
                        hitWall = true;
                        distanceToWall = Depth;
                    }
                    else
                    {
                        char testCell = _map[testY * MapWidth + testX];
                        if (testCell == '#')
                        {
                            hitWall = true;
                        }
                    }
                }
                int ceiling = (int)(ScreenHeight / 2d - ScreenHeight * Fov / distanceToWall);
                int floor = ScreenHeight - ceiling;

                char wallshade;

                if(distanceToWall < Depth / 4d)
                {
                    wallshade = '\u2588'; //закрашенный символ 
                }
                else if (distanceToWall < Depth / 3d)
                {
                    wallshade = '\u2593';
                }
                else if(distanceToWall < Depth / 2d)
                {
                    wallshade = '\u2592';
                }
                else if(distanceToWall < Depth)
                {
                    wallshade = '\u2591';
                }
                else
                {
                    wallshade = ' ';
                }
                for (int y = 0; y < ScreenHeight; y++)
                {
                    if (y <= ceiling)
                    {
                        Screen[y * ScreenWidth + x] = ' ';
                    }
                    else if (y > ceiling && y <= floor)
                    {
                        Screen[y * ScreenWidth + x] = wallshade;
                    }
                    else
                    {
                        Screen[y * ScreenWidth + x] = '.';
                    }
                }
            }
            Console.SetCursorPosition(0, 0);
            Console.Write(Screen);
            
        }
    }
   
} 
