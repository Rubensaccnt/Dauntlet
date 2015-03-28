﻿using System;
using Dauntlet.Entities;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.GameScreens
{
    public class GameplayScreen : GameScreen
    {

        private ContentManager _content;
        public PlayerEntity Player;
        public EnemyEntity Enemy;
        public StaticEntity Fountain;
        public StaticEntity Tree;
        SpriteBatch _spriteBatch;
        private static Matrix _view;

        public World World { get; set; }
        public static bool DebugCollision { get; set; }
        public Vector2 DisplayRoomCenter { get { return new Vector2(TileEngine.CurrentRoom.PixelWidth/2f, TileEngine.CurrentRoom.PixelHeight/2f);} }
        public Vector2 SimRoomCenter { get { return ConvertUnits.ToSimUnits(DisplayRoomCenter); } }
        public override Screen ScreenType { get { return Screen.GameplayScreen;} }

        // ------------------------------------

        public GameplayScreen(Dauntlet game) : base(game)
        {

            DebugCollision = false;
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(MainGame.Services, "Content");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize things
            CameraManager.Init(GraphicsDevice);
            TileEngine.LoadContent(this, _content);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(TileEngine.TileSize); // 1 meter = 1 tile

            World = TileEngine.CurrentRoom.World;
            Entity.DebugCircleTexture = _content.Load<Texture2D>("Textures/Circle");
            Entity.Shadow = _content.Load<Texture2D>("Textures/Shadow");
            Player = new PlayerEntity(World, DisplayRoomCenter + new Vector2(40, -40), _content.Load<Texture2D>("Textures/Dante"));
            Enemy = new Guapo(World, DisplayRoomCenter + new Vector2(40, 40), _content.Load<Texture2D>("Textures/Enemies/sprite_enemy_guapo"));
            Fountain = new StaticEntity(World, DisplayRoomCenter, ConvertUnits.ToSimUnits(new Vector2(128, 39)), _content.Load<Texture2D>("Textures/Fountain"));
            Fountain.SetAnimation(0, 0, 128, 59, 3, 1 / 12f, false);
            Tree = new StaticEntity(World, new Vector2(577.5f, 351f), ConvertUnits.ToSimUnits(new Vector2(125, 30)), _content.Load<Texture2D>("Textures/Tree"));


            isLoaded = true;
        }

        public override void UnloadContent()
        {
            isLoaded = false;
            _content.Unload();
        }


        public override void Update(GameTime gameTime)
        {
            // Update the world
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            Vector2 lol = Player.SimPosition;
            Player.Update(gameTime);
            Enemy.Update(gameTime);
            Fountain.Update(gameTime);

            // Update camera
            _view = CameraManager.MoveCamera(Player.DisplayPosition);

            if (MainGame.Input.IsMovement())
                Player.Move(MainGame.Input.CurrentKeyboardState, MainGame.Input.CurrentGamePadState);
            if (MainGame.Input.IsRotate())
                Player.Rotate(MainGame.Input.CurrentGamePadState);
            if (MainGame.Input.IsPauseGame())
                ((MenuScreen)MainGame.GetScreen(Screen.PauseScreen)).OverlayScreen(this);
            if (MainGame.Input.IsToggleDebug())
                DebugCollision = !DebugCollision;
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0, 141, 158));

            // First pass: Draw room
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, _view);
            TileEngine.DrawRoom(_spriteBatch, gameTime);
            _spriteBatch.End();

            // Second pass: Draw room objects
            // TODO

            // Third pass: Draw debug highlights (optional)
            if (DebugCollision)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _view);
                TileEngine.DrawDebug(_spriteBatch, GraphicsDevice);
                _spriteBatch.End();
            }

            // Fourth pass: Draw entities
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, _view);
            Player.Draw(gameTime, _spriteBatch);
            Fountain.Draw(gameTime, _spriteBatch);
            Enemy.Draw(gameTime, _spriteBatch);
            Tree.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            
        }
    }
}
