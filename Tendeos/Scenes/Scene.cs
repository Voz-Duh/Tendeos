using Tendeos.Utils.Graphics;
using Tendeos.UI;

namespace Tendeos.Scenes
{
    public abstract class Scene
    {
        public Core Game { get; init; }
        public GUIElement GUI { get; init; }


        public Scene(Core game)
        {
            GUI = new GUIElement(game.camera);
            Game = game;
        }

        public void Setup()
        {
            Init();
            InitGUI();
        }

        public abstract void Init();
        public abstract void InitGUI();
        public abstract void Clear();
        public abstract void Update();
        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void OnResize()
        {
        }

        public virtual void AfterDraw(SpriteBatch spriteBatch)
        {
        }

        public void SetValue(string name, object value) => GetType().GetField(name).SetValue(this, value);
        public T GetValue<T>(string name) => (T) GetType().GetField(name).GetValue(this);
    }
}