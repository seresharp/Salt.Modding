using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod;
using Modding.Attributes;

namespace Modding.Patches.ProjectTower.character.draw
{
    [MonoModPatch("ProjectTower.character.draw.CharDrawCosmetics")]
    public class CharDrawCosmetics
    {
        [MonoModIgnore]
        [ReplaceFunction("Microsoft.Xna.Framework.Graphics.SpriteBatch::Draw", "SpriteDrawHook")]
        internal void DrawConsumable(int partIdx, int consumableIdx, bool flip, Vector2 location, Vector2 scaling, float rotation, float b, float alpha) { }

        internal static void SpriteDrawHook(SpriteBatch sprite, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, 
            Vector2 scale, SpriteEffects effects, float layerDepth)
            => ModHooks.Instance.OnIconDraw(sprite, texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }
}
