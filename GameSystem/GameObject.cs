namespace GameSystem
{
    public class GameObject
    {
        public Map Map { get; set; }
        public Vector Position { get; set; }
        public Vector Direction { get; set; }
        public char Image { get; set; }

        public GameObject(Map map)
        {
            Map = map;
        }

        public GameObject(Map map, Vector position, Vector direction, char image)
        {
            Map = map;
            Position = position;
            Direction = direction;
            Image = image;
        }

    }
}