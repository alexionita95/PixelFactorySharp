using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace PixelFactory.Graphics
{
    public class Texture
    {
        public Texture2D Texture2D { get; private set; }
        public Vector2 TileSize { get;private set; }
        public Entities.DrawableEntity.EntityRotation Rotation { get; set; } = Entities.DrawableEntity.EntityRotation.None;
        public int CurrentRow { get=>currentRow; set { currentRow = value; UpdateSourceRectangle(); } }
        public int CurrentColumn { get=>currentColumn; set { currentColumn = value; UpdateSourceRectangle(); } }
        public int Width { get=>Texture2D.Width;}
        public int Height { get=>Texture2D.Height;}
        public Rectangle SourceRenctangle {  get; private set; }
        private int currentRow = 0;
        private int currentColumn = 0;
        public Texture() 
        {
        }
        public Texture(Texture2D texture2D)
        {
            Texture2D = texture2D;
            TileSize = new Vector2(texture2D.Width,texture2D.Height);
            CurrentRow = 0;
            currentColumn = 0;
        }
        public Texture(Texture2D texture2D, Vector2 tileSize)
        {
            Texture2D = texture2D;
            currentRow = 0;
            TileSize = tileSize;
            CurrentRow = 0;
            CurrentColumn = 0;
        }

        private void UpdateSourceRectangle()
        {
            SourceRenctangle = new Rectangle(new Vector2(TileSize.X * CurrentColumn, TileSize.Y*CurrentRow).ToPoint(), TileSize.ToPoint()) ;
        }

    }
}
