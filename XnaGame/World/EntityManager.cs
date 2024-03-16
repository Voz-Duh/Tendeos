using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using XnaGame.Physical;

namespace XnaGame.World
{
    public static class EntityManager
    {
        private static (Action<SpriteBatch> draw, Action update) entities = (b => { }, () => { });

        public static T GetEntity<T>() where T : Entity
        {
            foreach (var entity in entities.update.GetInvocationList())
                if (entity.Target is T t)
                    return t;
            return null;
        }

        public static T[] GetEntities<T>() where T : Entity
        {
            List<T> list = new List<T>();
            foreach (var entity in entities.update.GetInvocationList())
                if (entity.Target is T t)
                    list.Add(t);
            return list.ToArray();
        }

        public static void Add(Entity entity)
        {
            entities.update += entity.Update;
            entities.draw += entity.Draw;
        }
        public static void Remove(Entity entity)
        {
            entities.update -= entity.Update;
            entities.draw -= entity.Draw;
        }

        public static void Draw(SpriteBatch spriteBatch) => entities.draw(spriteBatch);

        public static void Update() => entities.update();

        public static void Clear() => entities = (b => { }, () => { });
    }
}
