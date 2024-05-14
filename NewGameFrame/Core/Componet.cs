using NewGameFrame.MathCore;

namespace NewGameFrame.Core
{
    public class Componet
    {
        public bool Init = false;

        public GameObject GameObject { get; set; }

        public Scene OwnerScene => GameObject.OwnerScene;

        public Vector Position
        {
            get => GameObject.Position;
            set => GameObject.Position = value;
        }

        public bool Enable { get; set; }

        public Componet(GameObject gameObject)
        {
            GameObject = gameObject;
            OwnerScene.OnComponentAdd(this);
        }

        public virtual void Update() { }
        public virtual void Start() { }
    }
}
