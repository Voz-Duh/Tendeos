using XnaGame.UI;

namespace XnaGame.State
{
    public abstract class GameState
    {
        public Game1 Game { get; init; }
        public GUIElement GUI { get; init; }


        public GameState(Game1 game)
        {
            GUI = new GUIElement(Core.camera);
            Game = game;
        }

        public void Setup()
        {
            InitGUI();
            Init();
        }

        public abstract void Init();
        public abstract void Update();
        public abstract void Draw();
        public abstract void InitGUI();

        public void SetValue(string name, object value) => GetType().GetField(name).SetValue(this, value);
        public T GetValue<T>(string name) => (T)GetType().GetField(name).GetValue(this);
    }
}
