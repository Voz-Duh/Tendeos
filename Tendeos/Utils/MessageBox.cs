using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.Utils;

public class MessageBox : Game
{
    private const int width = 600;

    private static readonly Color
        info = new Color(0xFFD6B378u),
        warning = new Color(0xFF69794Fu),
        error = new Color(0xFF6969D8u),
        text = new Color(0xFF2A2327u),
        light = new Color(0xFFE2E3DEu),
        dark = new Color(0xFFC1BAB4u);

    private readonly GraphicsDeviceManager graphics;
    private readonly Type type;
    private string message, title;
    private Font font;
    private Shader defaultShader;
    private Batch batch;
    private SpriteBatch spriteBatch;
    private Assets assets;

    private MessageBox(string title, string message, Type type)
    {
        graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
        Window.Title = title;
        graphics.PreferredBackBufferWidth = width;
        this.title = title;
        this.message = message;
        this.type = type;
    }

    protected override void LoadContent()
    {
        font = new Font(130, .14f);
        assets = new Assets(GraphicsDevice, font,
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), 4096, 4096);
        defaultShader = assets.GetShader("default");
        font.Init();

        StringBuilder resultMessage = new StringBuilder();
        int lines = 1;
        int last = 0;
        int i;
        for (i = 0; i < title.Length; i++)
        {
            if (title[i] == '\n')
            {
                resultMessage.Append('\n').Append(title[last..i]);
                last = i;
                lines += 2;
            }
            else if (font.MeasureString(title[last..i], 1.5f).X >= width - 60)
            {
                resultMessage.Append('\n').Append(title[last..i]);
                last = i;
                lines++;
            }
        }

        resultMessage.Append('\n').Append(title[last..]);
        title = resultMessage.ToString().Trim();

        resultMessage = new StringBuilder();
        lines = 1;
        last = 0;
        for (i = 0; i < message.Length; i++)
        {
            if (message[i] == '\n')
            {
                resultMessage.Append(message[last..i]);
                last = i;
                lines++;
            }
            else if (font.MeasureString(message[last..i]).X >= width - 20)
            {
                resultMessage.Append('\n').Append(message[last..i]);
                last = i;
                lines++;
            }
        }

        resultMessage.Append('\n').Append(message[last..]);

        graphics.PreferredBackBufferHeight = (int) Math.Ceiling(lines * font.LineHeight) + 100;
        message = resultMessage.ToString().Trim();

        batch = new Batch(GraphicsDevice);
        spriteBatch = new SpriteBatch(GraphicsDevice, assets.atlas);

        graphics.ApplyChanges();
    }

    protected override void Draw(GameTime gameTime)
    {
        Mouse.Update();
        GraphicsDevice.Clear(dark);

        batch.Begin(PrimitiveType.TriangleStrip);
        batch.Color = light;
        batch.Vertex3(0, 0, 0);
        batch.Vertex3(width, 0, 0);
        batch.Vertex3(0, 80, 0);
        batch.Vertex3(width, 80, 0);
        batch.End();

        batch.Begin(PrimitiveType.TriangleStrip);
        switch (type)
        {
            case Type.Info:
                batch.Color = info;
                batch.DrawCircle(40, 40, 35, 20);
                break;
            case Type.Warning:
                batch.Color = warning;
                batch.Vertex3(5, 75, 0);
                batch.Vertex3(75, 75, 0);
                batch.Vertex3(40, 5, 0);
                break;
            case Type.Error:
                batch.Color = error;
                batch.DrawCircle(40, 40, 35, 6);
                break;
        }

        ;
        batch.End();

        spriteBatch.Begin(defaultShader, assets.atlas.texture, 8172);
        spriteBatch.Text(text, font, type switch
        {
            Type.Info => "...",
            Type.Warning => "!",
            Type.Error => "X",
        }, type switch
        {
            Type.Info => new Vec2(35.5f, 17),
            Type.Warning => new Vec2(38.5f, 30.5f),
            Type.Error => new Vec2(36.5f, 27),
        }, 3);
        spriteBatch.Text(text, font, title, new Vec2(80, 30), 1.5f, 0, 0, 0);
        spriteBatch.Text(text, font, message, new Vec2(10, 90), 1, 0, 0, 0);
        spriteBatch.End();
    }

    public static void Show(string title, string message, Type type)
    {
        using MessageBox dialog = new MessageBox(title, message, type);
        dialog.Run();
    }

    public enum Type : byte
    {
        Info,
        Warning,
        Error
    }
}