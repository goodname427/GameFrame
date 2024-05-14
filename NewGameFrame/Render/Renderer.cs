using NewGameFrame.Core;
using NewGameFrame.MathCore;
using System.Net;

namespace NewGameFrame.Render
{
    public class Renderer : Componet
    {
        public Image? Image { get; set; }

        public int RenderLayer;

        public Renderer(GameObject gameObject) : base(gameObject)
        {
            Image = new Image(0, 0);
        }

        public void Render()
        {
            if (Image is null)
            {
                return;
            }

            for (int x = 0; x < Image.Width; x++)
            {
                for (int y = 0; y < Image.Height; y++)
                {
                    if (Image[x, y] == '\0')
                    {
                        continue;
                    }

                    OwnerScene.Map[x + Position.X - Image.Pivot.X, y + Position.Y - Image.Pivot.Y] = Image[x, y];
                }
            }
        }

        public override void Update()
        {
            Render();
        }
    }
}
