using GameFrame;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

namespace GameFrame
{
    public enum PathFindWay { Straight, Flat }
    public enum PathFindDirect { FourDirect = 4, EightDirect = 8 }
    public class Pathfinder//自动寻路
    {
        public event Action BeforeMove;//移动前事件
        public event Action AfterMove;//移动后事件

        Map map;//所在地图
        Screen screen;//所映射的地图
        MLinkedList<Vector> path = new MLinkedList<Vector>();//路径
        MLinkedList<Vector>.Node curNode;//当前到达的点 

        public PathFindWay FindWay { get; set; } = PathFindWay.Straight;//寻路方式 
        public PathFindDirect FindDirect { get; set; } = PathFindDirect.FourDirect;

        public List<char> Barriers { get; } = new List<char>();//障碍物
        public List<char> Preferences { get; } = new List<char>();//偏好物,影响优先级

        public bool IsRun { get; set; } = true;//是否继续执行
        public bool IsMove { get; set; } = true;//是否移动
        /// <summary>
        /// 目标游戏物体
        /// </summary>
        public GameObject TarObj { get; private set; }
        /// <summary>
        /// 目标位置
        /// </summary>
        public Vector TarPos { get => TarObj.Position; private set => TarObj.Position = value; }
        /// <summary>
        /// 当前位置
        /// </summary>
        public Vector CurPos { get; private set; }

        public Pathfinder(Vector tarPos, Vector curPos, Map map, PathFindDirect findDirect, PathFindWay findWay, params char[] barriers)
        {
            TarObj = new GameObject(tarPos, (char)0, map);
            CurPos = curPos;
            this.map = map;
            FindDirect = findDirect;
            FindWay = findWay;
            Barriers.AddRange(barriers);
        }
        public Pathfinder(Vector tarPos, Vector curPos, Screen screen, PathFindDirect findDirect, PathFindWay findWay, params char[] barriers)
        {
            TarObj = new GameObject(tarPos, (char)0, map);
            CurPos = curPos;
            map = screen.MappingMap;
            this.screen = screen;
            FindWay = findWay;
            FindDirect = findDirect;
            Barriers.AddRange(barriers);
        }
        public Pathfinder(GameObject tarObj, Vector curPos, Map map, PathFindDirect findDirect, PathFindWay findWay, params char[] barriers)
        {
            TarObj = tarObj;
            CurPos = curPos;
            this.map = map;
            FindWay = findWay;
            FindDirect = findDirect;
            Barriers.AddRange(barriers);
        }
        public Pathfinder(GameObject tarObj, Vector curPos, Screen screen, PathFindDirect findDirect, PathFindWay findWay, params char[] barriers)
        {
            TarObj = tarObj;
            CurPos = curPos;
            map = screen.MappingMap;
            this.screen = screen;
            FindWay = findWay;
            FindDirect = findDirect;
            Barriers.AddRange(barriers);
        }

        /// <summary>
        /// 寻找目标点
        /// </summary>
        /// <param name="isStatic">静态搜索</param>
        /// <returns></returns>
        public async Task Find(bool isStatic = false)
        {
            if (!isStatic)//动态查找
            {
                if (CurPos != TarPos)
                {
                    var node = await Task.Run(() => FindPathAStar());//路径计算
                    while (node.Parent != null && node.Parent.Parent != null)//向上遍历,直到倒数第二个节点
                    {
                        node = node.Parent;
                    }
                    BeforeMove?.Invoke();
                    if (IsMove && !IsTouchBarrier(node.Point.Pos, out _))//执行
                    {
                        CurPos = node.Point.Pos;
                    }
                    AfterMove?.Invoke();
                }
            }
            else//静态
            {
                if (path.Length < 1)//如果未计算路径
                {
                    var node = await Task.Run(() => FindPathAStar());//等待路径计算
                    while (node != null)//添加路径
                    {
                        path.Add(node.Point.Pos);
                        node = node.Parent;
                    }
                    curNode = path.Head;
                }

                BeforeMove?.Invoke();
                if (curNode != null && IsMove)//执行
                {
                    CurPos = curNode.Data;
                    curNode = curNode.Next;
                }
                else
                {
                    IsRun = false;
                }
                AfterMove?.Invoke();
            }

        }
        /// <summary>
        /// 寻找目标点直到当前位置到达目标点
        /// </summary>
        /// <param name="isStatic">静态搜索</param>
        public async void FindLoop(bool isStatic = false)
        {
            while (IsRun)
            {
                await Find(isStatic);
            }
        }
        /// <summary>
        /// A星算法寻找最短路径
        /// </summary>
        /// <returns></returns>
        PathTreeNode FindPathAStar()
        {
            bool tarPosIsBarrier = IsTouchBarrier(TarPos, out _);//如果目标点是障碍物则到达附近时认为寻路结束
            bool[,] boolMap = new bool[map.Width, map.Height];//记录每个点是否走过
            List<PathTreeNode> openList = new List<PathTreeNode>();//待评估的点S
            PathTreeNode rootNode = new PathTreeNode(new PathPoint(CurPos));//根结点
            var curNode = rootNode;//当前节点
            while (curNode.Point.Pos != TarPos && !(tarPosIsBarrier && IsAround(TarPos, curNode.Point.Pos, out Direction? direct) && (int)direct <= 4))
            {
                //添加子节点
                for (Direction d = Direction.Up; (int)d <= (int)FindDirect; d++)//创建子节点
                {
                    var childNode = new PathTreeNode(new PathPoint(curNode.Point.Pos + d));//创建对应位置的子节点
                    if (!IsTouchBarrier(childNode.Point.Pos, out _) && !map.IsOut(childNode.Point.Pos) && !boolMap[childNode.Point.Pos.X, childNode.Point.Pos.Y])//判断该子节点是否需要评估
                    {
                        childNode.Point.G = curNode.Point.G;//设置G值
                        childNode.Point.G += (int)d <= 4 ? 10 : 14;//增加G值，斜线代价为14，直线代价为10
                        childNode.Point.GetH(TarPos);//获取预估代价
                        PathTreeNode sameNode = null;
                        bool hasFound = false;
                        foreach (var o in openList)
                        {
                            if (o.Point.Pos == childNode.Point.Pos)//判断该位置是否已被记录
                            {
                                hasFound = true;
                                if (o.Point.F > childNode.Point.F)//如果f值更小则替换该值
                                {
                                    sameNode = o;//记录要替换的值
                                }
                                break;
                            }
                        }
                        if (sameNode != null || !hasFound)//如果该位置未被发现或者f值小于原先节点则记录该节点
                        {
                            openList.Add(childNode);//记录该字节点
                            openList.Remove(sameNode);//去除相同节点
                            childNode.Parent = curNode;//记录父亲节点
                            curNode.Childs.Add(childNode);//添加孩子节点
                        }
                    }
                }
                if (openList.Count == 0) break;//如果遍历地图未找到路径则跳出
                PathTreeNode minNode = openList[0];
                foreach (var o in openList)
                {
                    if (o.Point.F < minNode.Point.F) minNode = o;//寻找代价最小的点
                }
                curNode = minNode;//移动当前节点
                openList.Remove(minNode);//移出
                boolMap[curNode.Point.Pos.X, curNode.Point.Pos.Y] = true;//记录该点已经走过
            }
            return curNode;//返回当前节点
        }

        /// <summary>
        /// 指定位置是否碰到障碍物
        /// </summary>
        /// <param name="pos">指定位置</param>
        /// <param name="barrier">所碰到的障碍物</param>
        /// <returns></returns>
        public bool IsTouchBarrier(Vector pos, out char? barrier)
        {
            barrier = null;
            if (screen == null || screen.IsOutScreen(pos))
            {
                foreach (var b in Barriers)
                {
                    if (map[pos] == b) { barrier = b; return true; }
                }
            }
            else
            {
                foreach (var b in Barriers)
                {
                    if (screen[screen.GetRelativePos(pos)] == b) { barrier = b; return true; }
                }
            }
            return false;
        }
        /// <summary>
        /// 指定位置是否碰到指定字符
        /// </summary>
        /// <param name="pos">指定位置</param>
        /// <param name="tarChar">指定字符</param>
        /// <returns></returns>
        public bool IsTouch(Vector pos, char tarChar)
        {
            return map[pos] == tarChar;
        }
        /// <summary>
        /// 检测周围是否有指定的字符
        /// </summary>
        /// <param name="tarChar">指定字符</param>
        /// <param name="directs">方向</param>
        /// <returns></returns>
        public bool IsAround(char tarChar, out Direction[] directs)
        {
            List<Direction> dires = new List<Direction>();
            for (Direction d = 0; (int)d <= 8; d++)
            {
                if ((screen == null || screen.IsOutScreen(CurPos + d) && map[CurPos + d] == tarChar)) dires.Add(d);
                else if (screen[screen.GetRelativePos(CurPos + d)] == tarChar) dires.Add(d);
            }
            directs = dires.ToArray();
            return dires.Count > 0;
        }
        /// <summary>
        /// 检测指定目标位置是否在指定位置周围
        /// </summary>
        /// <param name="tarPos">指定目标位置</param>
        /// <param name="pos">指定位置</param>
        /// <param name="direct">指定目标位置在指定位置的方向</param>
        /// <returns></returns>
        public bool IsAround(Vector tarPos, Vector pos, out Direction? direct)
        {
            direct = null;
            for (Direction d = 0; (int)d <= 8; d++)
            {
                if (pos + d == tarPos) { direct = d; break; }
            }
            return direct.HasValue;
        }
        /// <summary>
        /// 检测当前位置周围是否有指定的位置
        /// </summary>
        /// <param name="tarPos">目标位置</param>
        /// <param name="direct">方向</param>
        /// <returns></returns>
        public bool IsAround(Vector tarPos, out Direction? direct)
        {
            return IsAround(tarPos, CurPos, out direct);
        }
        /// <summary>
        /// 检测目标位置是否在当前位置周围
        /// </summary>
        /// <param name="direct">目标位置在当前位置的方向</param>
        /// <returns></returns>
        public bool IsAround(out Direction? direct)
        {
            return IsAround(TarPos, out direct);
        }
        /// <summary>
        /// 改变目标物体
        /// </summary>
        /// <param name="tarObj">指定新物体</param>
        public void ChangeTarget(GameObject tarObj)
        {
            TarObj = tarObj;
            IsRun = true;
            IsMove = true;
        }
        /// <summary>
        /// 改变目标位置
        /// </summary>
        /// <param name="pos">指定新位置</param>
        public void ChangeTarget(Vector pos)
        {
            TarPos = pos;
            path = new MLinkedList<Vector>();//更改目标后需要重新计算A星路径
            curNode = new MLinkedList<Vector>.Node();
            IsRun = true;
            IsMove = true;
        }

        /// <summary>
        /// 路径树节点
        /// </summary>
        class PathTreeNode : IEnumerable<PathTreeNode>
        {
            public PathPoint Point;//储存的路径点
            public PathTreeNode Parent;//父节点
            public List<PathTreeNode> Childs = new List<PathTreeNode>();//子节点
            public PathTreeNode(PathTreeNode parent)
            {
                Parent = parent;
            }
            public PathTreeNode(PathPoint point)
            {
                Point = point;
                Parent = null;
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            public IEnumerator<PathTreeNode> GetEnumerator()//从当前节点向上迭代
            {
                var cur = this;
                while (cur != null)
                {
                    yield return cur;
                    cur = cur.Parent;
                }

            }
        }
        /// <summary>
        /// 路径点
        /// </summary>
        class PathPoint
        {
            int _h = 0;//预估代价

            public Vector Pos;//位置
            public int F { get => G + _h; }//总代价 
            public int G { get; set; } = 0;//移动代价
            
            public PathPoint(Vector Pos) { this.Pos = Pos; }
            
            public void GetH(Vector tarPos)
            {
                _h = Math.Abs(tarPos.X - Pos.X) + Math.Abs(tarPos.Y - Pos.Y);
            }
        }
    }
}



