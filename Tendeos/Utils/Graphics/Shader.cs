using Microsoft.Xna.Framework.Graphics;

namespace Tendeos.Utils.Graphics
{
    public readonly struct Shader
    {
        private readonly Microsoft.Xna.Framework.Graphics.Effect effect;

        public Shader(GraphicsDevice graphicsDevice, byte[] code)
        {
            effect = new Microsoft.Xna.Framework.Graphics.Effect(graphicsDevice, code);
        }

        public readonly EffectParameterCollection Parameters => effect.Parameters;
        public readonly EffectTechnique CurrentTechnique => effect.CurrentTechnique;


        public static implicit operator Microsoft.Xna.Framework.Graphics.Effect(Shader shader) => shader.effect;
    }
}