using NewGameFrame.Physics;
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
        /// 场景加载时调用
        /// </summary>
        public event Action<Scene>? OnSceneLoaded;
        /// <summary>
        /// 场景更新时调用
        /// </summary>
        public event Action<Scene>? OnSceneUpdate;

        /// <summary>
        /// 游戏地图，负责渲染
        /// </summary>
        public required Map Map { get; init; }
        /// <summary>
        /// 物理系统
        /// </summary>
        public required Physics.PhysicsSystem PhysicsSystem { get; init; }

        /// <summary>
        /// 场景中所有游戏物体
        /// </summary>
        public List<GameObject> GameObjects { get; } = new();
        
        /// <summary>
        /// 场景中所有组件
        /// </summary>
        private readonly List<Componet>[] Components = { new(), new(), new() };

        [SetsRequiredMembers]
        public Scene()
        {
            Map = new();
            PhysicsSystem = new();
        }

        public void Start()
        {
            CurrentScene = this;
            OnSceneLoaded?.Invoke(this);
        }
        public void Update()
        {
            // 刷新地图
            Map.Clear();

            // 获取输入
            Input.GetInput();

            // 更新组件
            foreach (var components in Components)
            {
                foreach(var component in components)
                {
                    if (component.Enable)
                    {
                        if (!component.Init)
                        {
                            component.Init = true;
                            component.Start();
                        }

                        component.Update();
                    }
                }
            }

            // 物理系统更新
            PhysicsSystem.Update();

            OnSceneUpdate?.Invoke(this);
        }

        /// <summary>
        /// 组件被添加时调用
        /// </summary>
        /// <param name="newComponent"></param>
        public void OnComponentAdd(Componet newComponent)
        {
            if (newComponent is Renderer renderer)
            {
                int index = 0;

                foreach (var component in Components[0])
                {
                    var _renderer = component as Renderer;
                    if (_renderer?.RenderLayer > renderer.RenderLayer)
                    {
                        Components[0].Insert(index, renderer);
                        break;
                    }

                    index++;
                }

                if (index == Components[0].Count)
                {
                    Components[0].Add(renderer);
                }
            }
            else if (newComponent is Camera camera)
            {
                Components[1].Add(camera);
            }
            else if (newComponent is Collider collider)
            {
                PhysicsSystem.AddCollider(collider);
            }
            else
            {
                Components[2].Add(newComponent);
            }
        }
    }


}
