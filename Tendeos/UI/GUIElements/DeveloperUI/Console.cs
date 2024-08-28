using System;
using System.Collections.Generic;
using System.Linq;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using VozDuh.TextParser;

namespace Tendeos.UI.GUIElements.DeveloperUI;

public class Console : WindowFiller
{
    public static Console Instance { get; private set; }
    
    public static readonly ParserSettings ParserSettings = new(
        new Literals().With((
            "\"", "\"", "\\",
            new SpecialSymbols()
                .With(("n", "\n"))
                .And(("t", "\t"))
                .And(("a", "\a"))
                .And(("r", "\r"))
                .And(("b", "\b"))
                .And(("v", "\v"))
                .And(("f", "\f"))
                .And(("\\", "\\"))
                .Cap
        )).Cap,
        new Groups(),
        new Comments()
    );

    public static (string, Type[], Delegate) CreateCommand(string command, Delegate action) =>
    (
        command,
        action.Method.GetParameters().Select(arg => arg.ParameterType).ToArray(),
        action
    );

    public readonly (string command, Type[] types, Delegate action)[] commands;

    private List<Font.BoxedTextData> messages = new(11);

    public void Log(string message)
    {
        messages.Insert(0, inputField.style.Font.GetBoxedTextData(message, new FRectangle(Vec2.Zero, camera.WorldViewport)));
        if (messages.Count > 20) messages.RemoveAt(20);
    }
    
    private readonly InputField inputField;
    
    public Console(Camera camera, InputField inputField, Sprite style, bool closeOnClick = false,
        GUIElement[] childs = null, params (string command, Type[] types, Delegate action)[] commands) : base(camera, style, closeOnClick, childs)
    {
        Instance = this;
        this.inputField = inputField;
        this.commands = commands;
    }

    public override void OnAdd() => Add(inputField);
    
    public override void OnRemove() => Remove(inputField);

    public override void Update(FRectangle rectangle)
    {
        base.Update(rectangle);

        if ((MouseOn || inputField.MouseOn) && Keyboard.IsPressed(Keys.Enter))
        {
            TokenManager reader = new(new Parser(inputField.Text, ParserSettings));
            reader.RemoveWhitespaces();
            if (!reader.Move || reader.Current != TokenType.Keyword)
                Log("Invalid command.");

            bool valid = true;
            object[] args = new object[reader.Length - 1];
            foreach (var (command, types, action) in commands)
            {
                valid = true;
                if (reader.Current.Value != command) continue;

                if (reader.Length - 1 != types.Length)
                {
                    Log($"Invalid command arguments count. {types.Length} is excepted.");
                    valid = false;
                    continue;
                }

                for (int i = 0; i < types.Length; i++)
                {
                    TokenType type = GetTokenizedType(types[i]);

                    if (type == TokenType.Space)
                    {
                        valid = false;
                        Log($"Invalid command argument type at {i}. Problem in command code.");
                        break;
                    }
                    
                    if (!type.HasFlag(reader.Next.Type))
                    {
                        valid = false;
                        Log($"Invalid command argument type at {i}. {types[i].Name} is excepted.");
                        break;
                    }
                    
                    if (type == TokenType.Keyword)
                        args[i] = reader.Current.Value == "true";
                    else if (type == (TokenType.Literal | TokenType.Keyword))
                        args[i] = reader.Current.Value;
                    else if (type == TokenType.Integer)
                        args[i] = Convert.ChangeType(long.Parse(reader.Current.Value), types[i]);
                    else
                        args[i] = Convert.ChangeType(double.Parse(reader.Current.Value), types[i]);
                }
                
                if (!valid) continue;

                action.DynamicInvoke(args);
                goto DONE;
            }
            
            if (valid) Log("Invalid command.");
            DONE: ;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
    {
        base.Draw(spriteBatch, rectangle);

        float y = inputField.Rectangle.Bottom;
        foreach (Font.BoxedTextData message in messages)
        {
            spriteBatch.Text(message.font, message.text, new Vec2(0, y), message.scale,
                xOrigin: 0,
                yOrigin: 1);
            y += message.height;
        }
    }

    private static TokenType GetTokenizedType(Type type)
    {
        if (type == typeof(double)
            || type == typeof(float)
            || type == typeof(Half))
            return TokenType.Integer | TokenType.Floating;
        if (type == typeof(long)
            || type == typeof(ulong)
            || type == typeof(int)
            || type == typeof(uint)
            || type == typeof(byte)
            || type == typeof(sbyte))
            return TokenType.Integer;
        if (type == typeof(string))
            return TokenType.Literal | TokenType.Keyword;
        if (type == typeof(bool))
            return TokenType.Keyword;

        return TokenType.Space;
    }
}