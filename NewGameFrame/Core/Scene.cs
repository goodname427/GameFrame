using NewGameFrame.Render;
using System.Diagnostics.CodeAnalysis;

namespace NewGameFrame.Core
{
    public class Scene
    {
        /// <summary>
        /// 当前场景
        /// </summary>
        public static Scene? CurrentScene { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public event Action<Scene>? OnSceneLoaded;
        public event Action<Scene>? OnSceneUpdate;

        public required Map Map { get; init; }
        public List<GameObject> GameObjects { get; } = new();

        private List<Renderer> Renderers { get; } = new();
        private List<Camera> Cameras { get; } = new();
        private List<Componet> OtherComponets { get; } = new();

        [SetsRequiredMembers]
        public Scene()
        {
            Map = new();
        }

        public void Start()
        {
            CurrentScene = this;
            OnSceneLoaded?.Invoke(this);
        }
        public void Update()
        {
            //渲染地图
            Map.Clear();

            // 获取输入
            Input.GetInput();

            //执行组件
            foreach (var renderer in Renderers)
            {
                renderer.Update();
            }
            foreach (var camera in Cameras)
            {
                camera.Update();
            }
            foreach (var componet in OtherComponets)
            {
                componet.Update();
            }

            OnSceneUpdate?.Invoke(this);
        }

        public void OnComponentAdd(Componet newComponent)
        {
            if (newComponent is Renderer renderer)
            {
                int index = 0;

                foreach (var old in Renderers)
                {
                    if (old.RenderLayer > renderer.RenderLayer)
                    {
                        Renderers.Insert(index, renderer);
                        break;
                    }

                    index++;
                }

                if (index == Renderers.Count)
                {
                    Renderers.Add(renderer);
                }
            }
            else if (newComponent is Camera camera)
            {
                Cameras.Add(camera);
            }
            else
            {
                OtherComponets.Add(newComponent);
            }
        }
    }


}
