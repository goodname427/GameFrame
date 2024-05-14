using NewGameFrame.MathCore;
using System.Collections.ObjectModel;

namespace NewGameFrame.Core
{
    public class GameObject
    {
        /// <summary>
        /// 游戏物体所属的场景
        /// </summary>
        public Scene OwnerScene { get; set; }

        /// <summary>
        /// 游戏物体的名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 游戏物体的位置
        /// </summary>
        public Vector Position { get; set; } = Vector.Zero;

        public GameObject() : this(Scene.CurrentScene, string.Empty)
        {

        }
        public GameObject(Scene? scene, string name = "")
        {
            if (scene is null)
                throw new ArgumentNullException(nameof(scene));

            OwnerScene = scene;
            scene.GameObjects.Add(this);

            Name = name;
        }

        /// <summary>
        /// 所有组件
        /// </summary>
        private readonly List<Componet> _componets = new();
        /// <summary>
        /// 游戏物体带有的组件
        /// </summary>
        public ReadOnlyCollection<Componet> Componets => _componets.AsReadOnly();

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public T AddComponet<T>() where T : Componet
        {
            var type = typeof(T);
            if (type.GetConstructor(new Type[] { typeof(GameObject) })?.Invoke(new object[] { this }) is not T component)
                throw new NotImplementedException(nameof(component));
            _componets.Add(component);
            return component;
        }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetComponet<T>() where T : Componet
        {
            return _componets.FirstOrDefault(c => c is T) as T;
        }
    }
}
