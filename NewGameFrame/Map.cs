namespace NewGameFrame
{
    public class Map
    {
        /// <summary>
        /// 尝试获取地图图像
        /// </summary>
        /// <param name="quadrant"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected static char TryGetValue(char[,] quadrant, Vector position)
        {
            position = position.Abs;
            return IsOutSide(quadrant, position) ? '\0' : quadrant[position.X, position.Y];
        }
        /// <summary>
        /// 获取位置所在象限索引
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected static int GetQuadrantIndex(Vector position)
        {
            return position switch
            {
                { X: 0, Y: 0 } => 0,
                { X: >= 0, Y: >= 0 } => 0,
                { X: < 0, Y: > 0 } => 1,
                { X: <= 0, Y: <= 0 } => 2,
                { X: > 0, Y: < 0 } => 3,
            };
        }
        /// <summary>
        /// 获取新尺寸的地图
        /// </summary>
        /// <param name="map"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static char[,] GetNewSizeMap(char[,] map, int newWidth, int newHeight)
        {
            var (width, height) = (map.GetLength(0), map.GetLength(1));
            if (width == newWidth && height == newHeight)
                return map;

            var newMap = new char[newWidth, newHeight];
            for (int i = 0; i < newWidth; i++)
            {
                for (int j = 0; j < newHeight; j++)
                {
                    newMap[i, j] = IsOutSide(map, new(i, j)) ? '\0' : map[i, j];
                }
            }
            return newMap;
        }
        /// <summary>
        /// 判断位置是否越界
        /// </summary>
        /// <param name="quadrant"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool IsOutSide(char[,] quadrant, Vector position)
        {
            position = position.Abs;
            var (width, height) = (quadrant.GetLength(0), quadrant.GetLength(1));
            return position.X >= width || position.Y >= height;
        }

        private readonly char[][,] _quadrant = new char[][,] { new char[0, 0], new char[0, 0], new char[0, 0], new char[0, 0] };

        public char this[Vector position]
        {
            get
            {
                var index = GetQuadrantIndex(position);
                return TryGetValue(_quadrant[index], position);
            }
            set
            {
                var index = GetQuadrantIndex(position);
                position = position.Abs;
                if (IsOutSide(_quadrant[index], position))
                    _quadrant[index] = GetNewSizeMap(_quadrant[index], position.X + 1, position.Y + 1);
                _quadrant[index][position.X, position.Y] = value;
            }
        }
        public char this[int x, int y]
        {
            get => this[new Vector(x, y)];
            set => this[new Vector(x, y)] = value;
        }

        /// <summary>
        /// 获取指定象限
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char[,] GetQuadrant(int index) => _quadrant[index];
        /// <summary>
        /// 清理地面
        /// </summary>
        public void Clear()
        {
            foreach (var quadrant in _quadrant)
            {
                for (int i = 0; i < quadrant.GetLength(0); i++)
                    for (int j = 0; j < quadrant.GetLength(1); j++)
                    {
                        quadrant[i, j] = '\0';
                    }
            }
        }

    }
}
