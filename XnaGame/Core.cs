using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using XnaGame.PEntities;
using XnaGame.PEntities.Content;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.WorldMap;

namespace XnaGame
{
    public static class Core
    {
        public static Sprite[] icons;
        public static Sprite OnInventoryIcon => icons[1];
        public static Sprite OffInventoryIcon => icons[0];

        public static Button.Style buttonStyle;
        public static Window.Style windowStyle;

        public static DynamicSpriteFontScaled font;

        public static (Action draw, Action update) entities = (() => { }, () => { });
        public static Player player;
        public static World world;
        public static IMap map;
        public static Camera camera;
        public static Action extraGuiDraw = () => { };
        public static event Action ExtraGuiDraw
        {
            add => extraGuiDraw += value;
            remove => extraGuiDraw -= value;
        }

        public static T GetEntity<T>() where T : Entity
        {
            foreach (var entity in entities.update.GetInvocationList())
                if (entity.Target is T t)
                    return t;
            return null;
        }

        public static T[] GetEntities<T>() where T : Entity
        {
            Queue<T> queue = new Queue<T>();
            foreach (var entity in entities.update.GetInvocationList())
                if (entity.Target is T t)
                    queue.Enqueue(t);
            T[] res = new T[queue.Count];
            for (int i = queue.Count - 1; i >= 0; i--)
                res[i] = queue.Dequeue();
            return res;
        }

        public static void AddEntity(Action draw, Action update)
        {
            entities.update += update;
            entities.draw += draw;
        }
        public static void RemoveEntity(Action draw, Action update)
        {
            entities.update -= update;
            entities.draw -= draw;
        }
    }
}
