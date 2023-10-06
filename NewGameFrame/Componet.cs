namespace NewGameFrame
{
    public class Componet
    {
        public bool Init = false;

        public GameObject GameObject { get; set; }
        public Vector Position
        {
            get => GameObject.Position;
            set => GameObject.Position = value;
        }
        public bool Enable { get; set; }

        public Componet(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public virtual void Update() { }
        public virtual void Start() { }
    }
}
