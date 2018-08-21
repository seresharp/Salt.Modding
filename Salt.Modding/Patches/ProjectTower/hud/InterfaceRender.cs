using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod;
using Modding.Attributes;

namespace Modding.Patches.ProjectTower.hud
{
    [MonoModPatch("ProjectTower.hud.InterfaceRender")]
    public class InterfaceRender
    {
        [MonoModIgnore]
        [ReplaceFunction("Microsoft.Xna.Framework.Graphics.SpriteBatch::Draw", "SpriteDrawHook")]
        internal static void DrawItem(Vector2 loc, int img, float brite, float alpha, float scale) { }

        internal static void SpriteDrawHook(SpriteBatch sprite, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin,
            float scale, SpriteEffects effects, float layerDepth)
            => ModHooks.Instance.OnIconDraw(sprite, texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
    }
}
