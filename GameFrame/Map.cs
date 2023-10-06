using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
namespace GameFrame
{
    public class Map : IEnumerable<char>
    {
        /// <summary>
        /// 返回一个恰好装下该读取文件的地图
        /// </summary>
        /// <param name="mapName">地图名</param>
        /// <returns></returns>
        public static Map GetMapFrom(string mapName)
        {
            var readMap = new Map(10000, 10000);
            string path = @"D:\ConsoleGame\Map\" + mapName + ".txt";
            string s;
            int y = 0;//记录当前读取的行数
            int sizeW = 0;//记录最大宽度
            if (File.Exists(path))//检查文件是否存在
            {
                try
                {
                    using (var sr = File.OpenText(path))//读取文件
                    {
                        while ((s = sr.ReadLine()) != null)
                        {
                            if (s.Length > sizeW) sizeW = s.Length;
                            for (int i = 0; i < s.Length; i++)
                            {
                                readMap[i, y] = s[i];
                            }
                            y++;
                        }
                    }
                }
                catch (IndexOutOfRangeException e)//防止序列溢出
                {
                    Console.WriteLine("Read Failure!");
                    Console.WriteLine(e.Message);
                }
            }
            var newMap = new Map(sizeW, y);
            readMap.CopyTo(newMap, (0, 0));
            return newMap;
        }

        char[,] _images = new char[21, 21];//储存地图信息
        char[,] _lastImages = new char[21, 21];//上一帧地图
        string _lastInfo = "陈冠霖今天不吃饭思密达";//上一帧的信息
        Vector _origin = (1, 1);//地图原点

        public char this[Vector position]
        {
            get
            {
                if (position.X < Width && position.X > -1 && position.Y < Height && position.Y > -1)
                    return _images[position.X, position.Y];
                else return (char)0;
            }
            set
            {
                if (position.X < Width && position.X > -1 && position.Y < Height && position.Y > -1)
                    _images[position.X, position.Y] = value;
            }
        }

        public char this[int x, int y]
        {
            get
            {
                if (x < Width && x > -1 && y < Height && y > -1)
                    return _images[x, y];
                else return (char)0;
            }
            set
            {
                if (x < Width && x > -1 && y < Height && y > -1)
                    _images[x, y] = value;
            }
        }
        /// <summary>
        /// 渲染原点
        /// </summary>
        public Vector Origin
        {
            get => _origin;

            set
            {
                if (value.X < 1)
                    value.X = 1;
                if (value.Y < 1)
                    value.Y = 1;
                _origin = value;
            }
        }
        /// <summary>
        /// 原点x坐标
        /// </summary>
        public int OX { get => Origin.X; }
        /// <summary>
        /// 原点Y坐标
        /// </summary>
        public int OY { get => Origin.Y; }
        /// <summary>
        /// 地图宽度
        /// </summary>
        public int Width { get => _images.GetLength(0); }
        /// <summary>
        /// 地图高度
        /// </summary>
        public int Height { get => _images.GetLength(1); }
        /// <summary>
        /// 地图尺寸
        /// </summary>
        public Vector Size { get => (Width, Height); }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public Map(int width = 21, int height = 21)
        {
            _images = new char[width, height];//初始化地图
            _lastImages = new char[width, height];//初始化上一帧地图   
        }

        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="isResetMap">重置地图</param>
        /// <param name="isShowMap">显示地图</param>
        /// <param name="isPrintBoundary">打印边界</param>
        public void Init(int width = 21, int height = 21, bool isResetMap = true, bool isShowMap = true, bool isPrintBoundary=true)
        {
            if (isResetMap)
                Init(width, height);
            if (isShowMap)
                Show(isPrintBoundary);
        }
        /// <summary>
        /// 初始化地图,默认大小为21X21
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void Init(int width = 21, int height = 21)
        {
            _lastInfo = "陈冠霖今天晚上要吃饭思密达";
            _images = new char[width, height];//初始化地图
            _lastImages = new char[width, height];//初始化上一帧地图              
        }
        /// <summary>
        /// 显示地图
        /// </summary>
        /// <param name="isPrintBoundary">是否显示边界</param>
        public void Show(bool isPrintBoundary = true)
        {
            Console.SetCursorPosition(2 * (OX - 1), OY - 1);//将光标移动到原点
            for (int i = 0; i <= Height + 1; i++)
            {
                if (isPrintBoundary) Console.Write("■");//显示左边界
                for (int j = 0; j < Width; j++)
                {
                    if (i == 0) { if (isPrintBoundary) Console.Write("■"); }//显示上边界
                    else if (i == Height + 1) { if (isPrintBoundary) Console.Write("■"); }//显示下边界
                    else
                    {
                        if (_images[j, i - 1] < 256) Console.Write("{0,-2}", _images[j, i - 1] == 0 ? ' ' : _images[j, i - 1]);//显示地图信息
                        else Console.Write("{0,-1}", _images[j, i - 1]);// 不在基础ASCII表内的字符只占一格
                    }
                }
                if (isPrintBoundary) Console.WriteLine("■");//显示右边界
                Console.CursorLeft = 2 * (OX - 1);
            }
        }
        /// <summary>
        /// 更新地图
        /// </summary>
        /// <param name="information">显示信息</param>
        public void Update(string information = "")
        {
            Console.CursorVisible = false;//隐藏光标
            for (int i = 0; i < Height; i++)//行
            {
                for (int j = 0; j < Width; j++)//列
                {

                    if (_lastImages[j, i] != _images[j, i])//检查该点是否更新
                    {
                        Console.SetCursorPosition(2 * (OX + j), OY + i);//将光标移到该点
                        if (_images[j, i] < 256) Console.Write("{0,-2}", _images[j, i] == 0 ? ' ' : _images[j, i]);//显示最新地图信息
                        else Console.Write("{0,-1}", _images[j, i]);// 不在基础ASCII表内的字符只占一格
                        _lastImages[j, i] = _images[j, i];//更新上一帧地图                                            
                    }
                }
            }
            Console.SetCursorPosition(2 * (OX - 1), OY + Height + 1);//将光标移到最下方
            if (information != _lastInfo)
            {
                Console.WriteLine(information);//显示游戏信息
                _lastInfo = information;
            }
            Console.CursorVisible = true;
        }

        /// <summary>
        /// 判断某个位置是否超出地图
        /// </summary>
        /// <param name="position">位置</param>
        /// <returns></returns>
        public bool IsOut(Vector position)
        {
            return !(position.X > -1 && position.X < Width && position.Y > -1 && position.Y < Height);
        }
        /// <summary>
        /// 判断某个位置是否超出地图
        /// </summary>
        /// <param name="position">位置</param>
        /// <returns></returns>
        public bool IsOut(int x, int y)
        {
            return IsOut((x, y));
        }

        /// <summary>
        /// 保存地图
        /// </summary>
        /// <param name="mapName">地图名</param>
        public void Save(string mapName = "nonamed_map_")
        {
            string path = @"D:\ConsoleGame\Map";
            if (!File.Exists(path)) Directory.CreateDirectory(path);
            if (mapName == "nonamed_map_") mapName += Path.GetRandomFileName() + DateTime.Now.ToFileTime();//如果未命名则生成默认名字
            path += "\\" + mapName + ".txt";
            using (var sw = File.CreateText(path))
            {
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        sw.Write(_images[i, j]);
                    }
                    sw.WriteLine();
                }
            }

        }
        /// <summary>
        /// 读取地图
        /// </summary>
        /// <param name="mapName">地图名</param>
        public void Read(string mapName)
        {
            string path = @"D:\ConsoleGame\Map\" + mapName + ".txt";
            string s;
            int y = 0;//记录当前读取的行数
            if (File.Exists(path))//检查文件是否存在
            {
                try
                {
                    using (var sr = File.OpenText(path))//读取文件
                    {
                        while ((s = sr.ReadLine()) != null)
                        {
                            for (int i = 0; i < s.Length; i++)
                            {
                                _images[i, y] = s[i];
                            }
                            y++;
                        }
                    }
                }
                catch (IndexOutOfRangeException e)//防止序列溢出
                {
                    Console.WriteLine("Read Failure!");
                    Console.WriteLine(e.Message);
                }
            }
        }
        
        /// <summary>
        /// 将本地图地图的信息从某个位置开始复制进入目标地图
        /// </summary>
        /// <param name="tarMap">目标地图</param>
        /// <param name="startPos">开始位置</param>
        public void CopyTo(Map tarMap, (int x, int y) startPos)
        {
            try
            {
                #region 弃用版
                //for (int i = startPos.x; i < startPos.x + Width && i < Width; i++)//i和j代表本地图的坐标，目标地图的坐标为i-startPos.x和j - startPos.y
                //{
                //    for (int j = startPos.y; j < startPos.y + Height && j < Height; j++)
                //    {
                //        if (!tarMap.IsOutMap((i - startPos.x, j - startPos.y)) && tarMap[i - startPos.x, j - startPos.y] != Maps[i, j])
                //        {
                //            if (!IsOutMap((i, j))) tarMap[i - startPos.x, j - startPos.y] = Maps[i, j];//复制
                //            else tarMap[i - startPos.x, j - startPos.y] = (char)0;
                //        }

                //    }
                //}
                #endregion
                for (int i = 0; i < tarMap.Width; i++)
                {
                    for (int j = 0; j < tarMap.Height; j++)
                    {
                        if (!IsOut(i + startPos.x, j + startPos.y))//检测本地图是否出界，无出界则复制地图
                        {
                            if (tarMap[i, j] != _images[i + startPos.x, j + startPos.y])//两地图元素不同则复制
                                tarMap[i, j] = _images[i + startPos.x, j + startPos.y];//复制
                        }
                        else
                        {
                            tarMap[i, j] = (char)0;//若原地图出界则出界部分用空格代替
                        }

                    }
                }

            }
            catch (IndexOutOfRangeException)//报错
            {

            }
        }
        /// <summary>
        /// 将地图全部填充为某个字符
        /// </summary>
        /// <param name="image">图像</param>
        public void FillWith(char image)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _images[i, j] = image;
                }
            }
        }

        public IEnumerator<char> GetEnumerator()
        {
            foreach (var i in _images) yield return i;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
