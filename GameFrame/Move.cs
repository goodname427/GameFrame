using System;
using System.Collections.Generic;
using System.Text;

namespace GameFrame
{
    public delegate void PressEvent(ConsoleKey? button, GameObject gameObject = null);//修补调用Move方法时所导致的按键冲突
    public delegate void PressEvent<T>(ConsoleKey? button, T gameObject = null) where T : GameObject;//修补调用Move方法时所导致的按键冲突
    public delegate void TouchEvent(char? barrier, (int x, int y) Pos);//碰撞到障碍时调用
    public abstract class Move<T> where T : GameObject
    {
        public static event PressEvent<T> ManualMoveP;
        public static event PressEvent<T> TurnDireP;
        public static event PressEvent<T> ForeManualMoveP;
        public static event PressEvent<T> ForeTurnDireP;
        public static event TouchEvent OnTouch;

        static List<char> Barriers = new List<char>();

        public static Screen Screen { get; set; }//是否检查屏幕上的障碍物
        public static ConsoleKey?[] Keys = new ConsoleKey?[] { ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D, null, null, null, null };//键盘映射方向

        public static void MoveInit()//初始化
        {
            ManualMoveP = null;
            TurnDireP = null;
            ForeTurnDireP = null;
            ForeTurnDireP = null;
            Barriers = new List<char>();
        }

        public static void AddBarrier(params char[] barriers)//添加障碍
        {
            Barriers.AddRange(barriers);
        }
        public static void RemoveBarrier(params char[] barriers)//清除伤害
        {
            foreach (var b in barriers)
                Barriers.Remove(b);
        }
        protected static bool DetectBarrier(T gameObject)//检查某方向是否有障碍
        {
            foreach (var b in Barriers)
            {
                if (Screen == null && gameObject.Map[ObjTryMove(gameObject)] == b) { OnTouch?.Invoke(b, ObjTryMove(gameObject)); return true; }
                if (Screen != null && Screen[Screen.GetRelativePos(ObjTryMove(gameObject))] == b) { OnTouch?.Invoke(b, ObjTryMove(gameObject)); return true; }
                if (Screen != null && Screen[Screen.GetRelativePos(ObjTryMove(gameObject))] == b) { OnTouch?.Invoke(b, ObjTryMove(gameObject)); return true; }
            }
            return false;
        }

        public static void SetKey(int index, ConsoleKey key)
        {
            Keys[index] = key;
        }

        public static void AutoMove(T gameObject, bool isClear = true, bool isPlace = true, bool isCanOutMap = false)//根据物体方向自动移动
        {
            if (isClear) gameObject.Place(true);
            if (!DetectBarrier(gameObject) && (!gameObject.Map.IsOut(ObjTryMove(gameObject)) || isCanOutMap))
                ObjDirectMove(gameObject);
            if (isPlace) gameObject.Place();
        }
        public static void AutoMoveS(T gameObject, bool isCanOutMap = false)//手动放置物体
        {
            if (!DetectBarrier(gameObject) && (!gameObject.Map.IsOut(ObjTryMove(gameObject)) || isCanOutMap))
                ObjDirectMove(gameObject);
        }

        public static void ManualMove(T gameObject, ConsoleKey? button = null, bool isClear = true, bool isPlace = true, bool isCanOutMap = false)//手动移动
        {
            if (button == null) button = ISystem.KeyPress();

            if (button != null)
            {
                if (isClear) gameObject.Place(true);
                ForeManualMoveP?.Invoke(button, gameObject);
                if (TrytoDire(gameObject, button.Value) && !DetectBarrier(gameObject) && (!gameObject.Map.IsOut(ObjTryMove(gameObject)) || isCanOutMap))
                    ObjDirectMove(gameObject);
                ManualMoveP?.Invoke(button, gameObject);
                if (isPlace) gameObject.Place();
            }
        }
        public static void ManualMoveS(T gameObject, ConsoleKey? button = null, bool isCanOutMap = false)//手动移动手动放置物体
        {
            if (button == null) button = ISystem.KeyPress();
            if (button != null)
            {
                ForeManualMoveP?.Invoke(button, gameObject);
                if (TrytoDire(gameObject, button.Value) && !DetectBarrier(gameObject) && (!gameObject.Map.IsOut(ObjTryMove(gameObject)) || isCanOutMap))
                    ObjDirectMove(gameObject);
                ManualMoveP?.Invoke(button, gameObject);
            }
        }

        public static void TurnDirection(T gameObject, ConsoleKey? button = null)//改变物体方向
        {
            if (button == null) button = ISystem.KeyPress();

            if (button != null)
            {
                ForeTurnDireP?.Invoke(button, gameObject);
                TrytoDire(gameObject, button.Value);
                TurnDireP?.Invoke(button, gameObject);
            }
        }

        public static (int, int) DirectMove((int x, int y) Pos, Direction direct)//朝某方向移动
        {
            (int x, int y) = Pos;
            switch (direct)
            {
                case Direction.Up: y--; break;
                case Direction.Down: y++; break;
                case Direction.Left: x--; break;
                case Direction.Right: x++; break;
                case Direction.UpLeft: x--; y--; break;
                case Direction.UpRight: x++; y--; break;
                case Direction.DownLeft: x--; y++; break;
                case Direction.DownRight: x++; y++; break;
            }
            return (x, y);
        }

        protected static (int, int) ObjTryMove(T gameObject)
        {
            return DirectMove(gameObject.Position, gameObject.Direction);
        }
        public static void ObjDirectMove(T gameObject)//对游戏物体进行移动
        {
            gameObject.Position = DirectMove(gameObject.Position, gameObject.Direction);
        }

        public static (int x, int y) DireToPos(Direction dire)
        {
            return dire switch
            {
                Direction.Up => (0, -1),
                Direction.Down => (0, 1),
                Direction.Left => (-1, 0),
                Direction.Right => (1, 0),
                Direction.UpLeft => (-1, -1),
                Direction.UpRight => (1, -1),
                Direction.DownLeft => (-1, 1),
                Direction.DownRight => (1, 1),
                _ => (0, 0),
            };
        }
        protected static Direction KeyToDire(ConsoleKey button)
        {
            if (button == Keys[0]) return Direction.Up;
            if (button == Keys[1]) return Direction.Down;
            if (button == Keys[2]) return Direction.Left;
            if (button == Keys[3]) return Direction.Right;
            if (button == Keys[4]) return Direction.UpLeft;
            if (button == Keys[5]) return Direction.UpRight;
            if (button == Keys[6]) return Direction.DownLeft;
            if (button == Keys[7]) return Direction.DownRight;
            return Direction.Center;
        }
        protected static bool TrytoDire(T gameObject, ConsoleKey button)
        {
            var dire = KeyToDire(button);
            if (dire != Direction.Center) { gameObject.Direction = dire; return true; }
            return false;
        }
    }

    public class Movement//用于挂载特定对象的移动属性
    {
        /// <summary>
        /// 移动后发生
        /// </summary>
        public event PressEvent Moved;
        /// <summary>
        /// 转向后发生
        /// </summary>
        public event PressEvent Turned;
        /// <summary>
        /// 移动前发生
        /// </summary>
        public event PressEvent Moving;
        /// <summary>
        /// 转向前发生
        /// </summary>
        public event PressEvent Turning;
        /// <summary>
        /// 当碰撞时触发
        /// </summary>
        public event TouchEvent OnTouch;

        /// <summary>
        /// 游戏物体障碍物
        /// </summary>
        public List<char> Barriers { get; set; } = new List<char>();
        /// <summary>
        /// 按键
        /// </summary>
        ConsoleKey?[] Keys { get; } = new ConsoleKey?[] { ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D, null, null, null, null };//键盘映射方向
        /// <summary>
        /// 是否清除
        /// </summary>
        public bool IsClear { get; set; }
        /// <summary>
        /// 是否放置
        /// </summary>
        public bool IsPlace { get; set; }
        /// <summary>
        /// 能否超出地图边界
        /// </summary>
        public bool CanOutMap { get; set; }
        /// <summary>
        /// 屏幕
        /// </summary>
        public Screen Screen { get; set; }
        /// <summary>
        /// 挂载的游戏物体
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="gameObject">挂载的游戏物体</param>
        public void Init(GameObject gameObject)
        {
            Moved = null;
            Turned = null;
            Turning = null;
            Turning = null;
            Barriers.Clear();
            GameObject = gameObject;
        }
        /// <summary>
        /// 设置移动参数
        /// </summary>
        /// <param name="clear">是否清除</param>
        /// <param name="place">是否放置</param>
        /// <param name="canOutMap">是否可以离开地图</param>
        public void SetParam(bool? clear = null, bool? place = null, bool? canOutMap = null)
        {
            IsClear = clear ?? IsClear;
            IsPlace = place ?? IsPlace;
            CanOutMap = canOutMap ?? CanOutMap;
        }

        /// <summary>
        /// 判断游戏物体前方是否有障碍物
        /// </summary>
        /// <returns></returns>
        public bool DetectBarrier()
        {
            foreach (var b in Barriers)
            {
                if (Screen == null && GameObject.Map[GameObject.Position + GameObject.Direction] == b)
                {
                    OnTouch?.Invoke(b, GameObject.Position + GameObject.Direction);
                    return true;
                }
                if (Screen != null && Screen[Screen.GetRelativePos(GameObject.Position + GameObject.Direction)] == b)
                {
                    OnTouch?.Invoke(b, GameObject.Position + GameObject.Direction);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 根据物体方向自动移动
        /// </summary>
        public void AutoMove()
        {
            if (IsClear) GameObject.Place(true);
            AutoMoveS();
            if (IsPlace) GameObject.Place();
        }
        /// <summary>
        /// 手动放置物体
        /// </summary>
        public void AutoMoveS()
        {
            if (!DetectBarrier() && (!GameObject.Map.IsOut(GameObject.Position + GameObject.Direction) || CanOutMap))
                GameObject.Position += GameObject.Direction;
        }
        /// <summary>
        /// 手动移动
        /// </summary>
        /// <param name="button"></param>
        public void ManualMove(ConsoleKey? button = null)
        {
            if (button == null) button = ISystem.KeyPress();
            if (button == null) return;
            if (IsClear) GameObject.Place(true);
            ManualMoveS(button);
            if (IsPlace) GameObject.Place();

        }
        /// <summary>
        /// 手动移动手动放置物体
        /// </summary>
        /// <param name="button"></param>
        public void ManualMoveS(ConsoleKey? button = null)
        {
            if (button == null) button = ISystem.KeyPress();
            if (button == null) return;
            Moving?.Invoke(button, GameObject);
            if (TryTurnDirect(button.Value) && !DetectBarrier() && (!GameObject.Map.IsOut(GameObject.Position + GameObject.Direction) || CanOutMap))
                GameObject.Position += GameObject.Direction;
            Moved?.Invoke(button, GameObject);
        }
        /// <summary>
        /// 改变物体方向
        /// </summary>
        /// <param name="button"></param>
        public void TurnDirection(ConsoleKey? button = null)
        {
            if (button == null) button = ISystem.KeyPress();
            if (button == null) return;
            Turning?.Invoke(button, GameObject);
            TryTurnDirect(button.Value);
            Turned?.Invoke(button, GameObject);

        }

        /// <summary>
        /// 设置键位
        /// </summary>
        /// <param name="direct">指定方向</param>
        /// <param name="key">值</param>
        public void SetKey(Direction direct, ConsoleKey? key)
        {
            if (direct == Direction.Center) return;
            Keys[(int)direct - 1] = key;
        }
        /// <summary>
        /// 获取按键对应的方向
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        Direction KeyToDirect(ConsoleKey button)
        {
            return (Direction)Array.IndexOf(Keys, button) + 1;
        }
        /// <summary>
        /// 根据按键转向，如果按键不对应方向则不改变
        /// </summary>
        /// <param name="button">按键</param>
        /// <returns></returns>
        bool TryTurnDirect(ConsoleKey button)
        {
            var dire = KeyToDirect(button);
            if (dire != Direction.Center)
            {
                GameObject.Direction = dire;
                return true;
            }
            return false;
        }
    }
}
