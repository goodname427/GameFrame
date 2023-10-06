using System;
using System.Collections.Generic;

namespace GameFrame
{
    public class Screen : Map//屏幕类地图
    {
        List<Fog> fogs { get; } = new List<Fog>();//迷雾,会在屏幕上覆盖一层指定的字符
        Map tempMap = null;//暂停时用作记录当前屏幕显示的信息

        List<GameObject>[] ReRenderObjs { get; set; } = new List<GameObject>[10];   //在屏幕上重新渲染的物体，按照顺序渲染
        public char?[,] ReRenderChar { get; private set; } = new char?[21, 21];
        public Camera Camera { get; set; }//相机
        public Map MappingMap => Camera.Map;//映射的地图
        public Vector RelativePos { get => Camera.Position; }//原点相对映射地图的位置

        public Screen(Camera camera, int width = 21, int height = 21) : base(width, height)
        {
            Camera = camera;
        }
        public Screen(int width = 21, int height = 21) : base(width, height) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="isShowMap">是否显示地图</param>
        /// <param name="isCreatMap">是否创建地图</param>
        /// <param name="isShowFrame">是否显示边界</param>
        new public void Init(int width = 21, int height = 21, bool isShowMap = true, bool isCreatMap = true, bool isShowFrame = true)
        {
            if (isCreatMap)
            {
                ReRenderObjs = new List<GameObject>[10];
                ResetReRendChar();
            }
            base.Init(width, height, isShowMap, isCreatMap, isShowFrame);
        }
        /// <summary>
        /// 重置渲染地图
        /// </summary>
        public void ResetReRendChar()
        {
            ReRenderChar = new char?[MappingMap.Width, MappingMap.Height];
        }
        /// <summary>
        /// 重置渲染物体
        /// </summary>
        /// <param name="sort"></param>
        public void ResetRendObj(uint sort)
        {
            ReRenderObjs[(int)sort] = new List<GameObject>();
        }

        /// <summary>
        /// 将地图映射到屏幕上
        /// </summary>
        void Copy()
        {
            try
            {
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        (int x, int y) Pos = (i + Camera.Position.X, j + Camera.Position.Y);
                        if (!MappingMap.IsOut(Pos))//检测本地图是否出界，无出界则复制地图
                        {
                            if (ReRenderChar[Pos.x, Pos.y] != null)
                            {
                                if (ReRenderChar[Pos.x, Pos.y] != this[i, j])
                                    this[i, j] = ReRenderChar[Pos.x, Pos.y].Value;
                            }
                            else if (this[i, j] != MappingMap[Pos])//两地图元素不同则复制
                                this[i, j] = MappingMap[Pos];//复制
                        }
                        else
                        {
                            if (this[i, j] != '0')
                                this[i, j] = (char)0;//若原地图出界则出界部分用空格代替
                        }

                    }
                }

            }
            catch//报错
            {

            }
        }
        /// <summary>
        /// 在屏幕上重新渲染游戏物体
        /// </summary>
        void ReRender()
        {
            for (int i = 0; i < ReRenderObjs.Length; i++)
            {
                var sorts = ReRenderObjs[i];
                if (i >= 5)//优先级小于五的物体会被迷雾遮挡
                {
                    if (sorts != null)
                        foreach (var obj in sorts)
                        {
                            if (obj != null && this[obj.GetRelativePos(Camera.Position)] != obj.Image) this[obj.GetRelativePos(Camera.Position)] = obj.Image;//再渲染
                        }
                }
                else
                {
                    if (sorts != null)
                        foreach (var obj in sorts)
                        {
                            if (obj != null && ReRenderChar[obj.Position.X, obj.Position.Y] == null && this[obj.GetRelativePos(Camera.Position)] != obj.Image) this[obj.GetRelativePos(Camera.Position)] = obj.Image;
                        }
                }
            }
        }
        /// <summary>
        /// 映射地图
        /// </summary>
        /// <param name="information"></param>
        public void Mapping(string information = "")
        {
            if (tempMap == null)//判断是否暂停
            {
                Camera.Move();//移动相机
                Copy();//复制地图
                ReRender();//重新渲染
                Update(information);//更新显示
            }
        }

        public void AddReRendObj(GameObject gameObject, uint sort = 0)
        {
            sort = sort > 9 ? 9 : sort;//最大为9
            if (ReRenderObjs[(int)sort] == null) ReRenderObjs[(int)sort] = new List<GameObject>();
            ReRenderObjs[(int)sort].Add(gameObject);
        }
        public void RemoveRendObj(GameObject gameObject, uint sort)
        {
            sort = sort > 9 ? 9 : sort;//最大为9
            if (ReRenderObjs[(int)sort] != null)
                ReRenderObjs[(int)sort].Remove(gameObject);
        }
        public void AddFog(Vector startPos, Vector endPos, char fogImage)
        {
            var fog = new Fog(startPos, endPos, fogImage, this);
            fogs.Add(fog);
            fog.Cover();
        }
        public void AddFog(params Fog[] fogs)
        {
            foreach (var fog in fogs)
            {
                this.fogs.AddRange(fogs);
                fog.Screen = this;
                fog.Cover();
            }
        }
        public void RemoveFog(params Fog[] fogs)
        {
            foreach (var fog in fogs)
            {
                this.fogs.Remove(fog);
                fog.Discover();
            }
        }

        /// <summary>
        /// 判断物体是否离开屏幕
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsOutScreen(Vector pos)
        {
            return IsOut((pos.X - Camera.Position.X, pos.Y - Camera.Position.Y));
        }
        /// <summary>
        /// 判断物体是否离开屏幕
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsOutScreen(int x, int y)
        {
            return IsOut((x - Camera.Position.X, y - Camera.Position.Y));
        }
        /// <summary>
        /// 判断物体是否在屏幕边缘
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsOnBoundary(Vector pos)
        {
            int x = pos.X - RelativePos.X, y = pos.Y - RelativePos.Y;
            return x == 0 || x == Width - 1 || y == 0 || y == Height - 1;
        }
        /// <summary>
        /// 物体在边界的哪个方向
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Direction DirectOnBoundary(Vector pos)
        {
            int x = pos.X - RelativePos.X, y = pos.Y - RelativePos.Y;
            if (x == 0 && y == 0) return Direction.UpLeft;
            if (x == Width - 1 && y == Height - 1) return Direction.DownRight;
            if (x == 0 && y == Height - 1) return Direction.DownLeft;
            if (x == Width - 1 && y == 0) return Direction.UpRight;
            if (y == 0 && x != 0 && x != Width - 1) return Direction.Up;
            if (y == Height - 1 && x != 0 && x != Width - 1) return Direction.Down;
            if (x == 0 && y != 0 && y != Height - 1) return Direction.Left;
            if (x == Width - 1 && y != 0 && y != Height - 1) return Direction.Right;
            return Direction.Center;
        }

        /// <summary>
        /// 返回指定位置在该屏幕的相对位置 
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public Vector GetRelativePos(Vector Pos)
        {
            return (Pos.X - RelativePos.X, Pos.Y - RelativePos.Y);
        }
        /// <summary>
        /// 返回指定位置在该屏幕的相对位置 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector GetRelativePos(int x, int y)
        {
            return (x - RelativePos.X, y - RelativePos.Y);
        }

        public void Pause()//暂停视频显示
        {
            try
            {
                tempMap = new Map(Width, Height);
                tempMap.Origin = Origin;
                ReRender();
                CopyTo(tempMap, (0, 0));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void Continue(bool isShowFrame = true)
        {
            Console.Clear();
            FillWith('\0');
            Init(isCreatMap: false, isShowFrame: isShowFrame);
            tempMap = null;
        }
    }
    /// <summary>
    /// 迷雾,用于遮挡屏幕
    /// </summary>
    public class Fog
    {
        char fogImage;

        public Vector Start { get; private set; }
        public Vector End { get; private set; }
        public List<(int x, int y, char? image)> ExtraFog = new List<(int, int, char?)>();//额外设置覆盖模式
        public Screen Screen { get; set; }

        public Fog(Vector start, Vector end, char fogImage, Screen screen)
        {
            this.Start = start;
            this.End = end;
            this.fogImage = fogImage;
            this.Screen = screen;
        }
        public Fog(Vector start, Vector end, char fogImage)
        {
            this.Start = start;
            this.End = end;
            this.fogImage = fogImage;
        }
        public Fog() { }

        /// <summary>
        /// 覆盖屏幕
        /// </summary>
        public void Cover()
        {
            try
            {
                for (int i = Start.X; i <= End.X; i++)
                {
                    for (int j = Start.Y; j <= End.Y; j++)
                    {
                        Screen.ReRenderChar[i, j] = fogImage;
                    }
                }
                foreach (var e in ExtraFog)
                {
                    Screen.ReRenderChar[e.x, e.y] = e.image;
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// 取消覆盖屏幕
        /// </summary>
        public void Discover()
        {
            try
            {
                for (int i = Start.X; i <= End.X; i++)
                {
                    for (int j = Start.Y; j <= End.Y; j++)
                    {
                        Screen.ReRenderChar[i, j] = null;
                    }
                }
                foreach (var e in ExtraFog)
                {
                    Screen.ReRenderChar[e.x, e.y] = null;
                }
            }
            catch
            {
                //throw;
            }
        }
    }
}
