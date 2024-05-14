using NewGameFrame.Core;
using NewGameFrame.MathCore;

namespace NewGameFrame.Render
{
    public class Image
    {
        /// <summary>
        /// 图片数据
        /// </summary>
        public char[,] Data { get; private set; }

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// 图片锚点，默认为中心点
        /// </summary>
        public Vector Pivot { get; set; }

        public char this[int x, int y]
        {
            get
            {
                return Data[x, y];
            }
            set
            {
                Data[x, y] = value;
            }
        }

        public char this[Vector position]
        {
            get
            {
                return this[position.X, position.Y];
            }
            set
            {
                Data[position.X, position.Y] = value;
            }
        }

        public Image() : this(0, 0) { }

        public Image(int width, int height)
        {
            Width = width;
            Height = height;
            Pivot = new Vector(Width / 2, Height / 2);
            Data = new char[Width, Height];
        }

        public Image(char[,] data)
        {
            Width = data.GetLength(0);
            Height = data.GetLength(1);
            Pivot = new Vector(Width / 2, Height / 2);
            Data = data;
        }

        public Image(char image) : this(1, 1)
        {
            Data[0, 0] = image;
        }

        public Image(char image, int width, int height) : this(width, height)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Data[x, y] = image;
                }
            }
        }

        public void Resize(int newWidth, int newHeight)
        {
            Data = Map.GetNewSizeMap(Data, newWidth, newHeight);
            Width = newWidth;
            Height = newHeight;
        }
    }
}
