using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        private World _world;

        
        private Vector2 _screenCenter;

        private PlayerEntity _player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            TileEngine.LoadContent(Content);
            _world = TileEngine.CurrentRoom.World;

            var playerSprite = Content.Load<Texture2D>("Circle");

            ConvertUnits.SetDisplayUnitToSimUnitRatio(32f);

            _player = new PlayerEntity(_world, _screenCenter, playerSprite);

        }

        protected override void Update(GameTime gameTime)
        {
            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            _player.Update(gameTime);
            //float a = ConvertUnits.ToSimUnits(TileEngine.TileSize);

            //List<Vector2> posList = (from b in _world.BodyList where b.IsStatic select ConvertUnits.ToDisplayUnits(b.Position)).ToList();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            TileEngine.DrawRoom(_spriteBatch, gameTime);
            _spriteBatch.End();

            _spriteBatch.Begin();
            var rect = new Texture2D(_graphics.GraphicsDevice, 32, 32);
            var data = new Color[32*32];
            for (int i = 0; i < data.Length; i++) data[i] = new Color(1, 0, 0, 0.1f);
            rect.SetData(data);
            foreach (var body in _world.BodyList)
            {
                if (body.IsStatic)
                    _spriteBatch.Draw(rect, ConvertUnits.ToDisplayUnits(body.Position)-new Vector2(TileEngine.TileSize/2f, TileEngine.TileSize/2f), Color.White);
            }
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
            _player.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
