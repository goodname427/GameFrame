using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGameFrame
{
    public class GameObject
    {
        private readonly List<Componet> _componets = new();

        /// <summary>
        /// 游戏物体的位置
        /// </summary>
        public Vector Position { get; set; } = Vector.Zero;
        /// <summary>
        /// 游戏物体的图像
        /// </summary>
        public char Image { get; set; } = '\0';
        /// <summary>
        /// 游戏物体带有的组件
        /// </summary>
        public ReadOnlyCollection<Componet> Componets => _componets.AsReadOnly();

        public GameObject()
        {
            if (Scene.Instance is null)
                throw new ArgumentNullException(nameof(Scene.Instance));

            Scene.Instance.GameObjects.Add(this);
        }
        public GameObject(Scene scene)
        {
            scene.GameObjects.Add(this);
        }

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
