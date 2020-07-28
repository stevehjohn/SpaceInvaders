using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceInvaders
{
    public class SpaceInvaders : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _invaderSprites;
        private Texture2D _shipSprite;
        private Texture2D _bulletSprite;

        private Dictionary<int, Invader> _invaders;

        private float _invaderDirection = 1;

        private int _shipX = 570;

        private Point _bullet;

        private int _frame = 0;
        private bool _altAnimation = false;

        public SpaceInvaders()
        {
            _graphics = new GraphicsDeviceManager(this)
                        {
                            PreferredBackBufferWidth = 1200,
                            PreferredBackBufferHeight = 700
                        };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _invaders = new Dictionary<int, Invader>();

            var index = 0;

            var type = 0;

            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 10; x++)
                {
                    _invaders.Add(index, new Invader
                                         {
                                             Type = type,
                                             Y = 50 + y * 40,
                                             X = 100 + x * 100
                                         });

                    index++;
                }

                switch (y)
                {
                    case 0:
                        type = 1;
                        break;
                    case 1:
                        type = 2;
                        break;
                    case 2:
                        type = 1;
                        break;
                    case 3:
                        type = 0;
                        break;
                }
            }

            _bullet = new Point(0, 1000);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _invaderSprites = Content.Load<Texture2D>("Invaders");
            _shipSprite = Content.Load<Texture2D>("Ship");
            _bulletSprite = Content.Load<Texture2D>("Bullet");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var invader in _invaders)
            {
                invader.Value.X += _invaderDirection;
            }

            var directionChanged = false;

            foreach (var invader in _invaders)
            {
                if (invader.Value.X > 1200 - 48 || invader.Value.X < 0)
                {
                    _invaderDirection = -_invaderDirection;

                    if (_invaderDirection > 0)
                    {
                        _invaderDirection += 0.1f;
                    }

                    directionChanged = true;

                    break;
                }
            }

            if (directionChanged)
            {
                foreach (var invader in _invaders)
                {
                    invader.Value.Y += 2;
                }
            }

            if (_bullet.Y < 700)
            {
                _bullet.Y -= 7;

                if (_bullet.Y < -8)
                {
                    _bullet.Y = 1000;
                }
            }

            var indexToRemove = -1;

            foreach (var invader in _invaders)
            {
                if (_bullet.X > invader.Value.X && _bullet.X < invader.Value.X + 48 && _bullet.Y > invader.Value.Y && _bullet.Y < invader.Value.Y + 32)
                {
                    _bullet.Y = 1000;

                    indexToRemove = invader.Key;
                }
            }

            if (indexToRemove > 0)
            {
                _invaders.Remove(indexToRemove);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left) && _shipX > 0)
            {
                _shipX -= 5;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right) && _shipX < 1140)
            {
                _shipX += 5;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && _bullet.Y > 700)
            {
                _bullet = new Point(_shipX + 30, 640);
            }

            _frame++;

            if (_frame % 24 == 0)
            {
                _altAnimation = !_altAnimation;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            foreach (var invader in _invaders)
            {
                _spriteBatch.Draw(
                    _invaderSprites,
                    new Vector2(invader.Value.X, invader.Value.Y),
                    new Rectangle(invader.Value.Type * 48 + (_altAnimation ? 48 : 0), 0, 48, 32),
                    new Color(0, 255, 0));
            }

            _spriteBatch.Draw(
                _shipSprite,
                new Vector2(_shipX, 650),
                new Rectangle(0, 0, 60, 32),
                new Color(255, 255, 0));

            if (_bullet.Y < 700)
            {
                _spriteBatch.Draw(
                    _bulletSprite,
                    new Vector2(_bullet.X, _bullet.Y),
                    new Rectangle(0, 0, 2, 8),
                    new Color(255, 255, 255));
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}