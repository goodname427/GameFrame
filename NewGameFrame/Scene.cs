using System.Diagnostics.CodeAnalysis;

namespace NewGameFrame
{
    public class Scene
    {
        public static Scene? Instance { get; private set; }

        public event Action<Scene>? OnSceneLoaded;
        public event Action<Scene>? OnSceneUpdate;

        public required Map Map { get; init; }
        public List<GameObject> GameObjects { get; } = new();

        [SetsRequiredMembers]
        public Scene()
        {
            Map = new();
        }

        public void Start()
        {
            Instance = this;
            OnSceneLoaded?.Invoke(this);
        }
        public void Update()
        {
            //渲染地图
            Map.Clear();
            foreach (var g in GameObjects.OrderBy(g => g.Position.Z))
            {
                Map[g.Position] = g.Image;
            }
            //执行组件
            foreach (var g in GameObjects)
            {
                foreach (var componet in g.Componets)
                {
                    if (!componet.Init)
                    {
                        componet.Init = true;
                        componet.Start();
                    }
                    componet.Update();
                }
            }
            OnSceneUpdate?.Invoke(this);
        }
    }


}
