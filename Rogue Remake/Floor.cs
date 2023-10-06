using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace Rogue_Remake
{

    public class Building//游戏的地图，由多个楼层组成
    {
        public const int MapWidth = 100;
        public const int MapHeight = 60;
        readonly List<Floor> floors = new List<Floor>();//所有楼层
        public Floor this[int index]
        {
            get => floors[index];
        }
        public void CreatStair()//生成楼层
        {
            int layer = Program.layer;
            Floor newFloor = new Floor(MapWidth, MapHeight, true);
            List<Monster> temp = new List<Monster>();
            for (int i = 0; i < layer + 2; i++)//添加怪物模板
            {
                if (i < 26)
                    temp.Add(Monster.AllMonsters[i]);
            }
            newFloor.SummonMonster(layer / 5, layer / 5 + 2, temp.ToArray());//生成怪物
            floors.Add(newFloor);
        }
    }
    public class Floor : Map//楼层
    {
        #region 常数
        const int minRoomNum = 6;
        const int maxRoomNum = 16;
        const int minRoomWidth = 10;
        const int maxRoomWidth = 25;
        const int minRoomHeight = 6;
        const int maxRoomHeight = 15;
        public const char IWall = 'O';//墙
        public const char IDoor = '+';//门
        public const char IRoad = '.';//路
        public const char IStair = '>';//楼梯
        public const char IRoomFloor = '_';//地板
        #endregion
        public (int X, int Y) StartPos;//出生点，玩家到达该楼层的初始位置
        public (int X, int Y) EndPos;//结束点，玩家离开该楼层的出口
        Room playerInRoom;
        readonly List<Room> rooms = new List<Room>();//所有产生的房间
        readonly List<(int x, int y, bool hasConnected, Direction direct)> doors = new List<(int x, int y, bool hasConnected, Direction direct)>();//所有产生的门        
        public Monster[] Monsters;
        bool isOpenFog;
        public Floor(int width, int height, bool isOpenFog = true) : base(width, height) { this.isOpenFog = isOpenFog; CreatRoom(); CreatDoor(); PrintFloor(); }

        void CreatRoom()//为一个楼层创建房间
        {
            Random r = new Random();
            int numOfRoom = r.Next(minRoomNum, maxRoomNum + 1);//房间数
            int Count;//避免死循环，当地图随机生成多次任然与其他房间重叠时将放弃生成
            Room newRoom;
            for (int i = 0; i < numOfRoom; i++)
            {
                Count = 0;
                do
                {
                    Count++;
                    (int w, int h) = (r.Next(minRoomWidth, maxRoomWidth + 1), r.Next(minRoomHeight, maxRoomHeight + 1));
                    (int x, int y) = (r.Next(Building.MapWidth - w), r.Next(Building.MapHeight - h));//随机房间位置和大小     
                    newRoom = new Room(x, y, x + w, y + h);
                } while (HasInterRoom(newRoom) && Count <= 256);//检测是否冲突,如果冲突并且次数足够时则再次刷新 
                if (Count <= 256)
                {
                    rooms.Add(newRoom);//添加房间
                    if (isOpenFog) Program.System.Screen.AddFog(newRoom.Fog);
                }
            }
            newRoom = rooms[r.Next(rooms.Count)];
            StartPos = (r.Next(newRoom.Start.X + 1, newRoom.End.X), r.Next(newRoom.Start.Y + 1, newRoom.End.Y));//制作出生点
            newRoom = rooms[r.Next(rooms.Count)];
            EndPos = (r.Next(newRoom.Start.X + 1, newRoom.End.X), r.Next(newRoom.Start.Y + 1, newRoom.End.Y));//制作结束点
        }
        void CreatDoor()//为房间创建路径
        {
            Random r = new Random();
            int numOfDoor;//门的数量                
            foreach (var room in rooms)
            {
                numOfDoor = r.Next(1, 5);//一个房间可能有1-4道门 
                for (int i = 0; i < 4; i++)//按顺序为每道墙设置门
                {
                    bool hasBoundary = false;//判断对应墙的位置是否是边境
                    switch (i)
                    {
                        case 0: if (room.Start.Y == 0) hasBoundary = true; break;
                        case 1: if (room.End.X == Building.MapWidth - 1) hasBoundary = true; break;
                        case 2: if (room.End.Y == Building.MapHeight - 1) hasBoundary = true; break;
                        case 3: if (room.Start.X == 0) hasBoundary = true; break;
                    }
                    if (hasBoundary) continue;//在边界上的墙不考虑设置门
                    bool flag = true;//是否为该堵墙设置门
                    if (4 - i > numOfDoor) flag = r.Next(2) == 1;
                    if (flag)
                    {
                        int x = 0, y = 0;
                        Direction direct = default;
                        switch (i)
                        {
                            case 0: (x, y, direct) = (r.Next(room.Start.X + 1, room.End.X), room.Start.Y, Direction.Up); break; //上墙
                            case 1: (x, y, direct) = (room.End.X, r.Next(room.Start.Y + 1, room.End.Y), Direction.Right); break; //右墙
                            case 2: (x, y, direct) = (r.Next(room.Start.X + 1, room.End.X), room.End.Y, Direction.Down); break; //下墙
                            case 3: (x, y, direct) = (room.Start.X, r.Next(room.Start.Y + 1, room.End.Y), Direction.Left); break; //左墙
                        }
                        doors.Add((x, y, false, direct));
                        room.Doors[i] = (x, y, false, direct);
                    }

                }
            }
            if (doors.Count % 2 != 0) doors.RemoveAt(0);//如果门的数量不是偶数，则删除一道门
        }
        void CreatPath()
        {
            Random r = new Random();
            var door = (x: 0, y: 0, hasConnected: false, direct: Direction.Center);
            var tarDoor = door;
            for (int i = 0; i < doors.Count; i++)
            {
                door = doors[i];
                if (this[new Vector(door.x, door.y) + door.direct] == IRoad) door.hasConnected = true;
                if (!door.hasConnected)//如果门没被连接
                {
                    int index = FindDoor(door.x, door.y);
                    tarDoor = doors[index];//目标门                   
                    Pathfinder ap = new Pathfinder((tarDoor.x, tarDoor.y), new Vector(door.x, door.y) + door.direct, this, PathFindDirect.FourDirect, PathFindWay.Flat, IWall, IDoor);
                    ap.BeforeMove += () =>
                    {
                        this[ap.CurPos] = IRoad;
                        if (isOpenFog) Program.System.Screen.ReRenderChar[ap.CurPos.X, ap.CurPos.Y] = '\0';
                    };
                    ap.FindLoop(true);
                    door.hasConnected = true;
                    tarDoor.hasConnected = true;
                    doors[i] = door;
                    doors[index] = tarDoor;
                }
            }
        }
        public void SummonMonster(int minNum, int maxNum, params Monster[] sortOfMonster)
        {
            List<Monster> monsters = new List<Monster>();
            Random r = new Random();
            foreach (var room in rooms)
            {
                int numOfMonster = r.Next(minNum, maxNum + 1);
                for (int i = 0; i < numOfMonster; i++)
                {
                    var (x, y) = (r.Next(room.Start.X + 1, room.End.X), r.Next(room.Start.Y + 1, room.End.Y));
                    monsters.Add(Monster.Summon(x, y, this, sortOfMonster[r.Next(sortOfMonster.Length)]));
                }
            }
            Monsters = monsters.ToArray();
        }

        int FindDoor(int x, int y)//通过一个门的位置在已有门中寻找距离该门最近的门,返回其下标
        {
            int index = 0;
            double minDis = 0;//最远的距离
            double GetDistance(int tx, int ty) { return Math.Pow(tx - x, 2) + Math.Pow(ty - y, 2); }//计算距离的平方
            for (int i = 0; i < doors.Count; i++)
            {
                if (!doors[i].hasConnected)//门是否连接
                {
                    double dis = GetDistance(doors[i].x, doors[i].y);//计算距离
                    if (dis > minDis && (doors[i].x, doors[i].y) != (x, y)) { minDis = dis; index = i; }
                }
            }
            return index;
        }
        bool HasInterRoom(Room room)//检测某房间是否与已有房间重叠
        {
            foreach (var r in rooms)
            {
                if (r.IsIntersected(room)) return true;
            }
            return false;
        }

        public void PrintFloor()//根据房间属性绘制地图
        {
            foreach (var room in rooms)//绘制房间
            {
                for (int i = room.Start.X; i <= room.End.X; i++)
                {
                    if (i == room.Start.X)
                    {
                        for (int j = room.Start.Y; j <= room.End.Y; j++)
                        {
                            if (j == room.Start.Y || j == room.End.Y) this[i, j] = IWall;//左上左下角落
                            else this[i, j] = IWall;//左竖边
                        }
                    }
                    else if (i == room.End.X)
                    {
                        for (int j = room.Start.Y; j <= room.End.Y; j++)
                        {
                            if (j == room.Start.Y || j == room.End.Y) this[i, j] = IWall;//右上右下角落
                            else this[i, j] = IWall;//右竖边
                        }
                    }
                    else
                    {
                        this[i, room.Start.Y] = IWall;//上横边￣—
                        this[i, room.End.Y] = IWall;//下横边
                        for (int j = room.Start.Y + 1; j <= room.End.Y - 1; j++)
                        {
                            this[i, j] = IRoomFloor;//房间内․
                        }
                    }

                }
                foreach (var door in room.Doors)
                {
                    if (door != default)
                    {
                        this[door.X, door.Y] = IDoor;
                    }
                }
                room.CreatItem(this);
            }
            this[EndPos] = IStair;
            CreatPath();//绘制房间之间的路径

        }
        public void ChangeFogs(GameObject gameObject)//根据玩家位置改变迷雾
        {
            if (playerInRoom == null)//检测玩家所在房间
                foreach (var room in rooms)
                {
                    if (room.ClearFog(gameObject))//清除玩家所在地图迷雾
                    {
                        playerInRoom = room;//记录玩家所在地图
                        if (!room.HasFound)
                        {
                            room.HasFound = true;
                            room.Fog = new Fog((room.Start.X + 1, room.Start.Y + 1), (room.End.X - 1, room.End.Y - 1), IRoomFloor);//发现房间后改变房间迷雾
                            foreach (var d in room.Doors) room.Fog.ExtraFog.Add((d.X, d.Y, IDoor));
                            if (room.IsInRoom(EndPos)) room.Fog.ExtraFog.Add((EndPos.X, EndPos.Y, IStair));
                        }
                        break;
                    }
                }
            if (playerInRoom != null && playerInRoom.ResumeFog(gameObject))//玩家离开时覆盖迷雾
                playerInRoom = null;//去除迷雾
            for (Direction d = 0; (int)d <= 8; d++)
            {
                (int x, int y) = Move<GameObject>.DirectMove(gameObject.Position, d);
                if (x >= 0 && x < Building.MapWidth && y >= 0 && y < Building.MapHeight) Program.System.Screen.ReRenderChar[x, y] = null;
            }
        }
        public void ResumeRoadFog(GameObject gameObject)
        {
            for (Direction d = 0; (int)d <= 8; d++)
            {
                (int x, int y) = Move<GameObject>.DirectMove(gameObject.Position, d);
                if (x >= 0 && x < Building.MapWidth && y >= 0 && y < Building.MapHeight && this[x, y] == IRoad) Program.System.Screen.ReRenderChar[x, y] = IRoad;
            }
        }
        class Room//房间
        {
            public (int X, int Y) Start;//房间起点
            public (int X, int Y) End;//房间重点
            public Fog Fog;//迷雾
            public bool HasFound = false;//表示是否被发现
            public (int X, int Y, bool HasConnected, Direction Direct)[] Doors = new (int, int, bool, Direction)[4];//门
            public Room(int X1, int Y1, int X2, int Y2)
            {
                Start = (X1, Y1);
                End = (X2, Y2);
                Fog = new Fog(Start, End, '\0');
            }
            public bool IsIntersected(Room tRoom)//检测是否与目标房间重叠
            {
                return IsCornerInRoom(tRoom) || tRoom.IsCornerInRoom(this) || IsCross(tRoom) || tRoom.IsCross(this);
            }
            public bool ClearFog(GameObject gameObject)//检测玩家进入房间时清除迷雾
            {
                if (IsInRoom(gameObject.Position))
                {
                    Program.System.Screen.RemoveFog(Fog);
                    return true;
                }
                return false;
            }
            public bool ResumeFog(GameObject gameObject)
            {
                if (!IsInRoom(gameObject.Position))
                {
                    Program.System.Screen.AddFog(Fog);
                    return true;
                }
                return false;
            }
            bool IsInRoomLarge((int X, int Y) Pos)//检测目标点是否在房间内,范围会扩大一格
            {
                (int x, int y) = Pos;
                return x >= Start.X - 1 && x <= End.X + 1 && y >= Start.Y - 1 && y <= End.Y + 1;
            }
            public bool IsInRoom((int x, int y) Pos)//检测目标点是否在房间内,范围会缩小一个格
            {
                (int x, int y) = Pos;
                return x >= Start.X + 1 && x <= End.X - 1 && y >= Start.Y + 1 && y <= End.Y - 1;
            }
            bool IsCornerInRoom(Room tRoom)//检测目标房间四个角落是否在房间内
            {
                (int x1, int y1) = tRoom.Start;
                (int x2, int y2) = tRoom.End;
                return IsInRoomLarge((x1, y1)) || IsInRoomLarge((x1, y2)) || IsInRoomLarge((x2, y1)) || IsInRoomLarge((x2, y2));
            }
            bool IsCross(Room tRoom)//检测是否与目标房间交叉
            {
                return tRoom.Start.X >= Start.X && tRoom.End.X <= End.X && tRoom.Start.Y <= Start.Y && tRoom.End.Y >= End.Y;
            }
            public void CreatItem(Floor floor)
            {
                Random r = new Random();
                int itemOfNum = r.Next(3);//生成的物品数量
                int x, y;
                for (int i = 0; i < itemOfNum; i++)
                {
                    do
                    {
                        (x, y) = (r.Next(Start.X + 1, End.X - 1), r.Next(Start.Y + 1, End.Y - 1));
                    } while (floor[x, y] != IRoomFloor);
                    floor[x, y] = Item.ItemImages[r.Next(Item.ItemImages.Length)];
                }
            }
        }
    }
}



