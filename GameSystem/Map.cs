using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSystem
{
    public class Map : IEnumerable<char>
    {
        char[,] imgs = new char[21, 21];//储存地图信息
        char[,] lastImgs = new char[21, 21];//上一帧地图
        string lastInfo = "陈冠霖今天不吃饭思密达";//上一帧的信息
        Vector origin = new(1, 1);//地图原点

        public char this[Vector position]
        {
            get
            {
                if (position.X < Width && position.X > -1 && position.Y < Height && position.Y > -1)
                    return imgs[position.X, position.Y];
                else return (char)0;
            }
            set
            {
                if (position.X < Width && position.X > -1 && position.Y < Height && position.Y > -1)
                    imgs[position.X, position.Y] = value;
            }
        }
        public char this[int X, int Y]
        {
            get
            {
                if (X < Width && X > -1 && Y < Height && Y > -1)
                    return imgs[X, Y];
                else return (char)0;
            }
            set
            {
                if (X < Width && X > -1 && Y < Height && Y > -1)
                    imgs[X, Y] = value;
            }
        }
        /// <summary>
        /// 地图渲染的原点
        /// </summary>
        public Vector Origin
        {
            get => origin;

            set
            {
                if (value.X < 1)
                    value.X = 1;
                if (value.Y < 1)
                    value.Y = 1;
                origin = value;
            }
        }
        /// <summary>
        /// 原点X坐标  
        /// </summary>
        public int OX { get => Origin.X; }
        /// <summary>
        /// 原点Y坐标                                             
        /// </summary>
        public int OY { get => Origin.Y; }
        /// <summary>
        /// 地图宽度
        /// </summary>
        public int Width { get => imgs.GetLength(0); }
        /// <summary>
        /// 地图高度
        /// </summary>
        public int Height { get => imgs.GetLength(1); }
        /// <summary>
        /// 地图尺寸
        /// </summary>
        public Vector Size { get => new(Width, Height); }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public Map(int width = 21, int height = 21)
        {
            imgs = new char[width, height];//初始化地图
            lastImgs = new char[width, height];//初始化上一帧地图   
        }
        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="isShowMap"></param>
        /// <param name="isCreatMap"></param>
        /// <param name="isShowFrame"></param>
        public void Init(int width = 21, int height = 21, bool isShowMap = true, bool isCreatMap = true, bool isShowFrame = true)//初始化地图,默认大小为21X21
        {
            lastInfo = "陈冠霖今天晚上要吃饭思密达";
            if (isCreatMap)
            {
                imgs = new char[width, height];//初始化地图
                lastImgs = new char[width, height];//初始化上一帧地图              
            }
            #region 显示首次地图
            if (isShowMap)
            {
                Console.SetCursorPosition(2 * (OX - 1), OY - 1);//将光标移动到原点
                for (int i = 0; i <= Height + 1; i++)
                {
                    if (isShowFrame) Console.Write("■");//显示左边界
                    for (int j = 0; j < Width; j++)
                    {
                        if (i == 0) { if (isShowFrame) Console.Write("■"); }//显示上边界
                        else if (i == Height + 1) { if (isShowFrame) Console.Write("■"); }//显示下边界
                        else
                        {
                            if (imgs[j, i - 1] < 256) Console.Write("{0,-2}", imgs[j, i - 1] == 0 ? ' ' : imgs[j, i - 1]);//显示地图信息
                            else Console.Write("{0,-1}", imgs[j, i - 1]);// 不在基础ASCII表内的字符只占一格
                        }
                    }
                    if (isShowFrame) Console.WriteLine("■");//显示右边界
                    Console.CursorLeft = 2 * (OX - 1);
                }
            }
            #endregion
        }
        /// <summary>
        /// 更新地图,逐一检查上一帧地图与最新地图的差异,再将不同处更改,并储存入上一帧地图
        /// </summary>
        /// <param name="information"></param>
        public void Update(string information = "")
        {
            Console.CursorVisible = false;//隐藏光标
            for (int i = 0; i < Height; i++)//行
            {
                for (int j = 0; j < Width; j++)//列
                {

                    if (lastImgs[j, i] != imgs[j, i])//检查该点是否更新
                    {
                        Console.SetCursorPosition(2 * (OX + j), OY + i);//将光标移到该点
                        if (imgs[j, i] < 256) Console.Write("{0,-2}", imgs[j, i] == 0 ? ' ' : imgs[j, i]);//显示最新地图信息
                        else Console.Write("{0,-1}", imgs[j, i]);// 不在基础ASCII表内的字符只占一格
                        lastImgs[j, i] = imgs[j, i];//更新上一帧地图                                            
                    }
                }
            }
            Console.SetCursorPosition(2 * (OX - 1), OY + Height + 1);//将光标移到最下方
            if (information != lastInfo)
            {
                Console.WriteLine(information);//显示游戏信息
                lastInfo = information;
            }
            Console.CursorVisible = true;
        }

        /// <summary>
        /// 判断某个位置是否超出地图
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsOut(Vector position)
        {
            if (position.X > -1 && position.X < Width && position.Y > -1 && position.Y < Height) return false;
            else return true;
        }
        /// <summary>
        /// 判断某个位置是否超出地图
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsOut(int x, int y)
        {
            return IsOut(new Vector(x, y));
        }

        /// <summary>
        /// 将本地图地图的信息从某个位置开始复制进入目标地图
        /// </summary>
        /// <param name="tarMap"></param>
        /// <param name="startPos"></param>
        public void CopyTo(Map tarMap, Vector startPos)
        {
            try
            {
                for (int i = 0; i < tarMap.Width; i++)
                {
                    for (int j = 0; j < tarMap.Height; j++)
                    {
                        if (!IsOut(i + startPos.X, j + startPos.Y))//检测本地图是否出界，无出界则复制地图
                        {
                            if (tarMap[i, j] != imgs[i + startPos.X, j + startPos.Y])//两地图元素不同则复制
                                tarMap[i, j] = imgs[i + startPos.X, j + startPos.Y];//复制
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
        /// <param name="image"></param>
        public void FillWith(char image)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    imgs[i, j] = image;
                }
            }
        }

        public IEnumerator<char> GetEnumerator()
        {
            foreach (var i in imgs) yield return i;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}

