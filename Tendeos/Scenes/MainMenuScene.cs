using Microsoft.Xna.Framework.Graphics;
using Tendeos.UI;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using System.Collections.Generic;
using System.IO;
using Tendeos.Utils.SaveSystem;
using Tendeos.World.Shadows;

namespace Tendeos.Scenes
{
    public class MainMenuScene : Scene
    {
        public MainMenuScene(Core game) : base(game)
        {
        }

        public override void Init()
        {
        }

        public override void InitGUI()
        {
            List<string> saves = new List<string>();
            string savesPath = Path.Combine(Settings.AppData, "saves");
            if (Directory.Exists(savesPath))
                foreach (string file in Directory.GetFiles(savesPath))
                    if (Path.GetExtension(file) == ".save")
                        saves.Add(Path.GetFileNameWithoutExtension(file));

            #region PLAY>LOAD
            GUIElement loadPlane = new Window(Vec2.Zero, new FRectangle(0, 13, 65, 43), Core.WindowStyle)
                .Add(new ScrollButtons<string>(Vec2.Zero, new FRectangle(2, 2, 60, 39), Core.ScrollButtonsStyle,
                value =>
                {
                    GameplayScene.SaveName = value;
                    Game.Scene = GameScene.Gameplay;
                },
                Icon<string>.From((batch, rect, value) =>
                {
                    batch.Text(Core.Font, value, rect.Center);
                }), 3, saves.ToArray()));
            #endregion
            #region PLAY>NEW
            TextLabel infoText = new TextLabel(Vec2.Zero, new FRectangle(0, 43, 65, 10), "", Core.Font);
            InputField nameField = new InputField(Vec2.Zero, new Vec2(22, 2), 41, Core.InputFieldStyle);
            InputField seedField = new InputField(Vec2.Zero, new Vec2(22, 13), 30, Core.InputFieldStyle, Core.UnsignedNumbers);
            for (int i = 0; i < 9; i++)
                seedField.AddText(URandom.SInt(9));
            GUIElement newPlane = new Window(Vec2.Zero, new FRectangle(0, 13, 65, 43), Core.WindowStyle)
                .Add(infoText)
                .Add(new TextLabel(Vec2.Zero, new FRectangle(2, 2, 20, 10), Localization.Translate("name"), Core.Font))
                .Add(nameField)
                .Add(new TextLabel(Vec2.Zero, new FRectangle(2, 13, 20, 10), Localization.Translate("seed"), Core.Font))
                .Add(seedField)
                .Add(new Button(Vec2.Zero, new FRectangle(53, 13, 10, 10),
                () =>
                {
                    seedField.ClearText();
                    for (int i = 0; i < 9; i++)
                        seedField.AddText(URandom.SInt(9));
                }, Core.ButtonStyle, Core.Icons["random_dice"]))
                .Add(new Button(Vec2.Zero, new FRectangle(2, 28, 61, 10),
                () =>
                {
                    if (string.IsNullOrWhiteSpace(nameField.Text))
                    {
                        infoText.text = Localization.Translate("invalid_world_name");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(seedField.Text))
                    {
                        infoText.text = Localization.Translate("invalid_world_seed");
                        return;
                    }
                    if (saves.Contains(nameField.Text))
                    {
                        infoText.text = Localization.Translate("world_available");
                        return;
                    }
                    GameplayScene.SaveName = nameField.Text;
                    GameplayScene.GameSeed = uint.Parse(seedField.Text);
                    Game.Scene = GameScene.Gameplay;
                }, Core.ButtonStyle, Core.Text2Icon("create_world")));
            #endregion
            #region SETTINGS
            GUIElement settingsPlane = null;
            settingsPlane = new WindowFiller(Game.camera, Core.WindowFillerStyle)
                .Add(new Window(new Vec2(0.5f), new FRectangle(0, 0, 70, 35), Core.LabelWindowStyle, Core.Text2Icon("settings"))
                    .Add(new Button(new Vec2(0.5f, 1), new FRectangle(0, -2, 66, 10),
                    () =>
                    {
                        GUI.Remove(settingsPlane);
                        Settings.Save();
                    }, Core.ButtonStyle, Core.Text2Icon("back")))
                    .Add(new EnumSwitcher<ShadowMatrix.SmoothPower>(new Vec2(0.5f, 1), new FRectangle(0, -13, 66, 10), Core.Font, Core.ButtonStyle,
                    () => (ShadowMatrix.SmoothPower)Settings.GetInt("shadow_smoothing"),
                    v => Settings.Set(Settings.Type.Int, "shadow_smoothing", (int)v)))
                );
            #region BUTTON
            GUI.Add(new Button(Vec2.Zero, new FRectangle(0, 10, 60, 10),
                () =>
                {
                    GUI.Add(settingsPlane);
                },
                Core.ButtonStyle, Core.Text2Icon("settings")));
            #endregion
            #endregion
            #region PLAY
            bool playLoadOn = true;
            GUIElement playPlane = null;
            playPlane = new Window(Vec2.Zero, new FRectangle(60, 0, 65, 56), Core.WindowStyle)
                .Add(new Button(Vec2.Zero, new FRectangle(2, 2, 61, 10),
                () =>
                {
                    if (playLoadOn)
                    {
                        playPlane.Add(newPlane);
                        playPlane.Remove(loadPlane);
                        playLoadOn = false;
                    }
                    else
                    {
                        playPlane.Add(loadPlane);
                        playPlane.Remove(newPlane);
                        playLoadOn = true;
                    }
                },
                Core.ButtonStyle,
                Icon.From((batch, rect) => batch.Text(Core.Font, playLoadOn ? Localization.Translate("game_new") : Localization.Translate("game_load"), rect.Center))
                ))
                .Add(loadPlane);
            #region BUTTON
            bool playOn = false;
            GUI.Add(new Button(Vec2.Zero, new FRectangle(0, 0, 60, 10),
                () =>
                {
                    if (playOn)
                    {
                        GUI.Remove(playPlane);
                        playOn = false;
                    }
                    else
                    {
                        GUI.Add(playPlane);
                        playOn = true;
                    }
                },
                Core.ButtonStyle, Core.Text2Icon("play")));
            #endregion
            #endregion
            #region STRUCTURE EDITOR BUTTON
            GUI.Add(new Button(Vec2.Zero, new FRectangle(0, 20, 60, 10),
                () =>
                {
                    Game.Scene = GameScene.StructureEditor;
                },
                Core.ButtonStyle, Core.Text2Icon("structure_editor")));
            #endregion
        }

        public override void Clear()
        {
            GUI.Clear();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }

        public override void Update()
        {
        }
    }
}
