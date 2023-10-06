using System;


namespace GameFrame
{
    public enum Direction { Center, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight };//代表物体运动的方向
    public class GameObject
    {
        /// <summary>
        /// 玩家所在地图
        /// </summary>
        public Map Map { get; set; } = new Map();
        /// <summary>
        /// 物体位置
        /// </summary>
        public Vector Position;
        /// <summary>
        /// 物体的方向,0为默认方向即无方向,1为上,2为下,3为左,4为右
        /// </summary>
        public Direction Direction { get; set; } = 0;
        /// <summary>
        /// 物体显示的图标
        /// </summary>
        public char Image { get; set; }

        /// <summary>
        /// 下标为0,则访问X坐标,下标为1,则访问Y坐标,若都不是则默认返回0
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[int index]
        {
            get
            {
                if (index == 0)
                    return Position.X;
                else if (index == 1)
                    return Position.Y;
                else return 0;
            }
            set
            {
                if (index == 0)
                    Position.X = value;
                else if (index == 1)
                    Position.Y = value;
            }
        }

        public GameObject(int x, int y, char image, Map map, Direction direction = Direction.Up)
        {
            Position.X = x;
            Position.Y = y;
            this.Image = image;
            this.Direction = direction;
            this.Map = map;
        }
        public GameObject(Vector pos, char image, Map map, Direction direction = Direction.Up)
        {
            Position = pos;
            Image = image;
            Map = map;
            Direction = direction;
        }

        /// <summary>
        /// 将物体信息储存进地图
        /// </summary>
        /// <param name="isNull"></param>
        public virtual void Place(bool isNull = false)
        {
            if (!IsOutMap())
            {
                if (!isNull) Map[Position] = Image;
                else Map[Position] = (char)0;
            }
        }

        public bool Touch(GameObject subObject)
        {
            return Position == subObject.Position;
        }

        /// <summary>
        /// 检测物体是否碰撞某类物体
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool Touch(char image)
        {
            return Map[Position] == image;
        }

        /// <summary>
        /// 检测物体是否出界
        /// </summary>
        /// <returns></returns>
        public bool IsOutMap()
        {
            int x = this[0], y = this[1];
            if (x < Map.Width && x > -1 && y < Map.Height && y > -1) return false;
            else return true;
        }
        /// <summary>
        /// 检测物体是否出界某地图
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool IsOutMap(Map map)
        {
            int x = this[0], y = this[1];
            if (x < map.Width && x > -1 && y < map.Height && y > -1) return false;
            else return true;
        }
        /// <summary>
        /// 是否在边界上
        /// </summary>
        /// <returns></returns>
        public bool IsOnBoundary()
        {
            int x = this[0], y = this[1];
            return x == 0 || x == Map.Width - 1 || y == 0 || y == Map.Height - 1;
        }

        /// <summary>
        /// 获取游戏物体在对应原点的相对坐标
        /// </summary>
        /// <param name="oringin"></param>
        /// <returns></returns>
        public (int X, int Y) GetRelativePos((int X, int Y) oringin)
        {
            return (Position.X - oringin.X, Position.Y - oringin.Y);
        }
        /// <summary>
        /// 获得物体当前位置与目标位置的距离
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public double GetDistance((int x, int y) pos)
        {
            var (xdis, ydis) = (pos.x - Position.X, pos.y - Position.Y);
            return Math.Sqrt(xdis * xdis + ydis * ydis);
        }
    }
}
