using NewGameFrame.Core;
using NewGameFrame.MathCore;
using NewGameFrame.Render;

namespace NewGameFrame.Physics
{
    public class BoxCollider : Collider
    {
        private readonly BoxPrimitive _box = new();
        public Vector Min
        { 
            get => _box.Min; 
            set => _box.Min = value;
        }

        public Vector Max
        {
            get => _box.Max;
            set => _box.Max = value;
        }

        public override IPrimitive Primitive => _box;

        public BoxCollider(GameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="renderer"></param>
        public BoxCollider SetBoxToImage(Renderer? renderer = null)
        {
            renderer ??= GameObject.GetComponet<Renderer>();
            if (renderer is null || renderer.Image is null)
            {
                return this;
            }

            Min = -renderer.Image.Pivot;
            Max = new Vector(renderer.Image.Width - 1, renderer.Image.Height - 1) - renderer.Image.Pivot;
        
            return this;
        }
    }
}
