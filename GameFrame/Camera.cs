using System;
using System.Collections.Generic;
using System.Text;

namespace GameFrame
{
    public enum FllowMode { NoFllow, RealTime, OnlyBoundary };//相机跟随的模式，NoFllow表示无跟随的对象，RealTime表示实时跟随，保持跟随的物体始终保持在中央，OnlyBoundary表示仅在跟随物体到达边界时移动相机
    public class Camera : GameObject//相机
    {
        /// <summary>
        /// 跟随物体
        /// </summary>
        public GameObject FllowedObject { get;private set; }
        /// <summary>
        /// 跟随模式
        /// </summary>
        public FllowMode FllowMode { get; set; }
        /// <summary>
        /// 屏幕地图
        /// </summary>
        public Screen Screen { get;private set; }
        /// <summary>
        /// 屏幕是否只会显示地图内容
        /// </summary>
        public bool IsRestricted { get; set; }

        public Camera(GameObject fllowedObject, FllowMode fllowedMode, bool isRestrict, Map inMap, Screen screenMap) : base(0, 0, '\0', inMap)
        {
            FllowedObject = fllowedObject;
            FllowMode = fllowedMode;
            Screen = screenMap;
            IsRestricted = isRestrict;
            screenMap.Camera = this;
            screenMap.ResetReRendChar();
        }
        public Camera(GameObject fllowedObject, FllowMode fllowedMode, Map inMap) : base(0, 0, '\0', inMap)
        {
            FllowedObject = fllowedObject;
            FllowMode = fllowedMode;
        }
        public Camera(Map map) : base(0, 0, '\0', map)
        {
            FllowedObject = null;
            FllowMode = FllowMode.NoFllow;
        }

        public void FllowObject(GameObject fllowedObject, FllowMode fllowedMode)//跟随某物体
        {
            FllowedObject = fllowedObject;
            FllowMode = fllowedMode;
        }
        public void SetMap(Map map, Screen screen, bool isResetReRendChar = true)
        {
            Map = map;
            Screen = screen;
            screen.Camera = this;
            if (isResetReRendChar) screen.ResetReRendChar();
            Position = FllowedObject.Position;
        }
        
        public void Move()//控制相机移动
        {
            switch (FllowMode)
            {
                case FllowMode.RealTime: RealTimeMove(); break;
                case FllowMode.OnlyBoundary: OnlyBoundaryMove(); break;
            }
        }
        void RealTimeMove()//实时移动
        {
            Position = (FllowedObject.Position.X - Screen.Width / 2, FllowedObject.Position.Y - Screen.Height / 2);
            if (IsRestricted) AdjustPos();
        }
        void OnlyBoundaryMove()//边界移动
        {
            var Dire = Screen.DirectOnBoundary(FllowedObject.Position);//判断物体处于屏幕边界
            if (Dire != Direction.Center)
            {
                Position += Dire;
                #region 
                //switch (Dire)//移动相机位置
                //{
                //    case Direction.Up:
                //        Position.Y--;
                //        break;
                //    case Direction.Down:
                //        Position.Y++;
                //        break;
                //    case Direction.Left:
                //        Position.X--;
                //        break;
                //    case Direction.Right:
                //        Position.X++;
                //        break;
                //    case Direction.DownLeft:
                //        Position.X--;
                //        Position.Y++;
                //        break;
                //    case Direction.DownRight:
                //        Position.X++;
                //        Position.Y++;
                //        break;
                //    case Direction.UpLeft:
                //        Position.X--;
                //        Position.Y--;
                //        break;
                //    case Direction.UpRight:
                //        Position.X++;
                //        Position.Y--;
                //        break;
                //}
                #endregion
                if (IsRestricted) AdjustPos();
            }
        }

        void AdjustPos()//当相机位置超出地图则调整其回到边界位置
        {
            if (Position.X < 0) Position.X = 0;
            if (Position.Y < 0) Position.Y = 0;
            if (Position.X + Screen.Width > Map.Width - 1) Position.X = Map.Width - Screen.Width;
            if (Position.Y + Screen.Height > Map.Height - 1) Position.Y = Map.Height - Screen.Height;
        }
    }

}