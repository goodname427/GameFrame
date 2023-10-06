namespace NewGameFrame
{
    public class Camera : Componet
    {
        /// <summary>
        /// 相机宽度
        /// </summary>
        public int Width { get => Size.X; set => Size = Size with { X = value }; }
        /// <summary>
        /// 相机高度
        /// </summary>
        public int Height { get => Size.Y; set => Size = Size with { Y = value }; }
        /// <summary>
        /// 相机尺寸
        /// </summary>
        public Vector Size { get; set; } = Vector.Zero;

        public Camera(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// 获取映射的地图
        /// </summary>
        public char[,] Map
        {
            get
            {
                if (Screen.Instance is null)
                    throw new ArgumentNullException(nameof(Screen.Instance));

                if (Scene.Instance is null)
                    throw new ArgumentNullException(nameof(Scene.Instance));

                var map = new char[Width, Height];
                var start = new Vector(Position.X - Width / 2, Position.Y + Height / 2);
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        map[i, j] = Scene.Instance.Map[start.X + i, start.Y - j];
                    }
                }
                return map;
            }
        }

        /// <summary>
        /// 更新屏幕
        /// </summary>
        public override void Update()
        {
            if (Screen.Instance is null)
                return;

            Screen.Instance.Update(Map);
        }
    }
}
