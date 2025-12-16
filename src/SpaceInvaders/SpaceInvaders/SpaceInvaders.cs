using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceInvaders;

public class SpaceInvaders : Game
{
    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _invaderSprites;
    private Texture2D _shipSprite;
    private Texture2D _bulletSprite;

    private readonly List<Invader> _invaders = [];

    private float _invaderDirection = 1;

    private int _shipX = 570;

    private readonly List<Point> _bullets = [];

    private int _frame;
    private bool _altAnimation;

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
        var type = 0;

        for (var y = 0; y < 5; y++)
        {
            for (var x = 0; x < 10; x++)
            {
                _invaders.Add(new Invader
                {
                    Type = type,
                    Y = 50 + y * 40,
                    X = 100 + x * 100
                });
            }

            type = y switch
            {
                0 => 1,
                1 => 2,
                2 => 1,
                3 => 0,
                _ => type
            };
        }

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
            invader.X += _invaderDirection;
        }

        var directionChanged = false;

        foreach (var invader in _invaders)
        {
            if (invader.X is > 1200 - 48 or < 0)
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
                invader.Y += 2;
            }
        }

        for (var i = _bullets.Count - 1; i >= 0; i--)
        {
            if (_bullets[i].Y < 700)
            {
                _bullets[i] = _bullets[i] with { Y = _bullets[i].Y - 7 };

                if (_bullets[i].Y < -8)
                {
                    _bullets.RemoveAt(i);
                }
            }
        }

        for (var i = _invaders.Count - 1; i >= 0; i--)
        {
            var invader = _invaders[i];

            for (var b = _bullets.Count - 1; b >= 0; b--)
            {
                var bullet = _bullets[b];
                    
                if (bullet.X > invader.X && bullet.X < invader.X + 48 && bullet.Y > invader.Y && bullet.Y < invader.Y + 32)
                {
                    _invaders.RemoveAt(i);
                        
                    _bullets.RemoveAt(b);
                }
            }
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Left) && _shipX > 0)
        {
            _shipX -= 5;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Right) && _shipX < 1140)
        {
            _shipX += 5;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            _bullets.Add(new Point(_shipX + 30, 640));
        }

        _frame++;

        if (_frame % 24 == 0)
        {
            _altAnimation = ! _altAnimation;
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
                new Vector2(invader.X, invader.Y),
                new Rectangle(invader.Type * 96 + (_altAnimation ? 48 : 0), 0, 48, 32),
                new Color(0, 255, 0));
        }

        _spriteBatch.Draw(
            _shipSprite,
            new Vector2(_shipX, 650),
            new Rectangle(0, 0, 60, 32),
            new Color(255, 255, 0));

        foreach (var bullet in _bullets)
        {
            if (bullet.Y < 700)
            {
                _spriteBatch.Draw(
                    _bulletSprite,
                    new Vector2(bullet.X, bullet.Y),
                    new Rectangle(0, 0, 2, 8),
                    new Color(255, 255, 255));
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}