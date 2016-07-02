using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;


using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using OurGame.Engine.Components;
using OurGame.Engine;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    public class SceneGenerator : IScript
    {
        private static int _boardSize = 10;
        [DataMember]
        private int MaxRoomCount = 5;
        [DataMember]
        private int MinRoomCount = 3;
        [DataMember]
        private int seed = 0;

        [DataMember]
        private int startingX;
        [DataMember]
        private int startingY;

        private Random _rand;

        private GameObject _parent;
        private static float _roomOffset = 3500.0f;

        public SceneGenerator()
        {
        }

        public void Initialize(GameObject parent)
        {
            _parent = parent;

            if (seed == 0)
            {
                seed = new Random().Next(10000, 100000);
            }
            _rand = new Random(seed);
            Modifications.AbstractModification.random = _rand;
            List<Room> rooms;
            Room[,] res = GetRoomsSetup(_rand, out rooms);
            GameObject[,] roomsMatrix = GenerateRoomGameObjects(res);
            SetImmovableRooms(ref roomsMatrix);
            SetupRoomScrips(ref roomsMatrix, res);

        }

        public void Update(GameTime time)
        {

        }

        public class Room
        {
            public Room(int x, int y)
            {
                X = x;
                Y = y;
                _right = _up = _left = _down = true;
                if (x == 0)
                    Left = false;
                if (x == _boardSize - 1)
                    Right = false;
                if (y == 0)
                    Down = false;
                if (y == _boardSize - 1)
                    Up = false;
            }

            private bool _left, _right, _up, _down;  //true - can be build, false - it cannot

            public bool IsStarting;
            public bool IsEnding;

            public bool Left
            {
                get { return _left; }
                set { _left = value; }
            }
            public bool Right { get { return _right; } set { _right = value; } }

            public bool Up
            {
                get { return _up; }
                set { _up = value; }
            } 
            public bool Down { get { return _down; } set { _down = value; } }

            public int X, Y;

            public Room GetNextNeighbour(ref Room[,] board, Random rand)
            {
                bool isRightChecked = false, isLeftChecked = false, isUpChecked = false, isDownChecked = false;

                while (!isRightChecked || !isLeftChecked || !isUpChecked || !isDownChecked)
                {
                    int nextDirection = rand.Next(0, 4);  //0 - right, 1 - left, 2 - up, 3 - down
                    switch (nextDirection)
                    {
                        case 0:
                            isRightChecked = true;
                            if (Right)
                            {
                                Right = false;
                                if (X + 1 >= board.GetLength(0))
                                {
                                    break;
                                }
                                else
                                {
                                    Room newRoom = new Room(X + 1, Y);
                                    newRoom.Left = false;
                                    board[X + 1, Y] = newRoom;
                                    return newRoom;
                                }
                            }
                            break;

                        case 1:
                            isLeftChecked = true;
                            if (Left)
                            {
                                Left = false;
                                if (X - 1 < 0)
                                {
                                    break;
                                }
                                else
                                {
                                    Room newRoom = new Room(X - 1, Y);
                                    newRoom.Right = false;
                                    board[X - 1, Y] = newRoom;
                                    return newRoom;
                                }
                            }
                            break;

                        case 2:
                            isUpChecked = true;
                            if (Up)
                            {
                                Up = false;
                                if (Y + 1 >= board.GetLength(0))
                                {
                                    break;
                                }
                                else
                                {
                                    Room newRoom = new Room(X, Y + 1);
                                    newRoom.Down = false;
                                    board[X, Y + 1] = newRoom;
                                    return newRoom;
                                }
                            }
                            break;

                        case 3:
                            isDownChecked = true;
                            if (Down)
                            {
                                Down = false;
                                if (Y - 1 < 0)
                                {
                                    break;
                                }
                                else
                                {
                                    Room newRoom = new Room(X, Y - 1);
                                    newRoom.Up = false;
                                    board[X, Y - 1] = newRoom;
                                    return newRoom;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                return null;
            }

            public void NotifyNeighbours(ref Room[,] board)
            {
                if (X + 1 <= _boardSize)
                {
                    if (board[X + 1, Y] != null)
                    {
                        board[X + 1, Y].Left = false;
                        Right = false;
                    }
                }
                if (X - 1 >= 0)
                {
                    if (board[X - 1, Y] != null)
                    {
                        board[X - 1, Y].Right = false;
                        Left = false;
                    }
                }
                if (Y + 1 == _boardSize)
                {
                    if (board[X, Y + 1] != null)
                    {
                        board[X, Y + 1].Down = false;
                        Up = false;
                    }
                }
                if (Y - 1 >= 0)
                {
                    if (board[X, Y - 1] != null)
                    {
                        board[X, Y - 1].Up = false;
                        Down = false;
                    }
                }

            }
        }


        private Room[,] GetRoomsSetup(Random rand, out List<Room> rooms)
        {
            rooms = new List<Room>();
            Room[,] res = new Room[_boardSize, _boardSize];
            int roomsToCreate = rand.Next(MinRoomCount, MaxRoomCount + 1);
            startingX = rand.Next(0, _boardSize);
            startingY = rand.Next(0, _boardSize);
            Room startingPoint = new Room(startingX, startingY) {IsStarting = true};
            //Scene.FindWithTag("MainCamera").Transform.Position = new Vector3(startingX * _roomOffset, 200f, startingY * _roomOffset);
            res[startingX, startingY] = startingPoint;
            rooms.Add(startingPoint);
            int counter = 1;
            while (counter <= roomsToCreate)
            {
                Room room = rooms[rand.Next(0, rooms.Count)].GetNextNeighbour(ref res, rand);
                if (room != null)
                {
                    room.NotifyNeighbours(ref res);
                    rooms.Add(room);
                    if (counter == roomsToCreate)
                        room.IsEnding = true;
                    counter++;
                }
            }
            return res;
        }

        private GameObject[,] GenerateRoomGameObjects(Room[,] rooms)
        {
            GameObject[,] result = new GameObject[_boardSize, _boardSize];
            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    if (rooms[i, j] != null)
                    {
                        GameObject go = null;
                        if (rooms[i, j].IsStarting)
                        {
                            go = PrefabManager.GetPrafabClone("RoomStarting");
                            go.GetComponentOfType<ScriptComponent>().GetScriptOfType<RoomScript>().IsFinished = true;
                        }
                        else if (rooms[i, j].IsEnding)
                        {
                            go = PrefabManager.GetPrafabClone("RoomBoss");
                        }
                        else
                        {
                            int room = _rand.Next(1, 5);
                            go = PrefabManager.GetPrafabClone("Room" + room);
                        }
                        Vector3 newPos = new Vector3(rooms[i, j].X * _roomOffset, 0f, rooms[i, j].Y * _roomOffset);
                        go.Transform.Position = newPos;
                        if (!rooms[i, j].Right)
                        {
                            var child = go.Children.Find(x => x.Tag == "Right");
                            child.Destroy = true;
                        }
                        if (!rooms[i, j].Down)
                        {
                            var child = go.Children.Find(x => x.Tag == "Down");
                            child.Destroy = true;
                        }
                        result[i, j] = go;
                        ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(go);
                    }
                }
            }

            return result;
        }

        private void SetupRoomScrips(ref GameObject[,] rooms, Room[,] map)
        {
            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    var room = rooms[i, j];
                    if (room != null)
                    {
                        var roomScript = room.GetComponentOfType<ScriptComponent>().GetScriptOfType<RoomScript>();
                        roomScript.Random = _rand;
                        roomScript.UpWall = room.Children.First(x => x.Tag == "Up");
                        roomScript.LeftWall = room.Children.First(x => x.Tag == "Left");
                        if (!map[i, j].Right)
                        {
                            if (i < _boardSize)
                                roomScript.RightWall = rooms[i + 1, j].Children.First(x => x.Tag == "Left");
                            else
                                roomScript.RightWall = room.Children.First(x => x.Tag == "Right");
                        }
                        else
                        {
                            roomScript.RightWall = room.Children.First(x => x.Tag == "Right");
                        }
                        if (!map[i, j].Down)
                        {
                            if (j < _boardSize && rooms[i, j - 1] != null)
                                roomScript.DownWall = rooms[i, j - 1].Children.First(x => x.Tag == "Up");
                            else
                                roomScript.DownWall = room.Children.First(x => x.Tag == "Down");
                        }
                        else
                        {
                            roomScript.DownWall = room.Children.First(x => x.Tag == "Down");
                        }
                        if (map[i, j].IsStarting)
                        {
                            roomScript.IsStarting = true;
                            RoomScript.StartingRoom = roomScript;
                        }
                        if(map[i,j].IsEnding)
                        {
                            roomScript.IsBoss = true;
                        }
                            
                    }
                }
            }
        }

        private void SetImmovableRooms(ref GameObject[,] rooms)
        {
            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    if (rooms[i, j] != null)
                    {
                        if (i == 0)
                            rooms[i, j].Children.First(x => x.Tag == "Left").Name = "Immovable";
                        else if (rooms[i - 1, j] == null)
                            rooms[i, j].Children.First(x => x.Tag == "Left").Name = "Immovable";

                        if (i + 1 == _boardSize)
                            rooms[i, j].Children.First(x => x.Tag == "Right").Name = "Immovable";
                        else if (rooms[i + 1, j] == null)
                            rooms[i, j].Children.First(x => x.Tag == "Right").Name = "Immovable";

                        if (j == 0)
                            rooms[i, j].Children.First(x => x.Tag == "Down").Name = "Immovable";
                        else if (rooms[i, j - 1] == null)
                            rooms[i, j].Children.First(x => x.Tag == "Down").Name = "Immovable";

                        if (j + 1 == _boardSize)
                            rooms[i, j].Children.First(x => x.Tag == "Up").Name = "Immovable";
                        else if (rooms[i, j + 1] == null)
                            rooms[i, j].Children.First(x => x.Tag == "Up").Name = "Immovable";
                    }
                }
            }
        }

        public void SetPlayerOnLevel()
        {
            Scene.FindWithTag("MainCamera").Transform.Position = new Vector3(startingX * _roomOffset, 200f, startingY * _roomOffset);
        }
    }
}
