using System.Collections.Generic;
using System.Reflection;

using Game;
using GameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//    Content Mods are meant to be heavy helper classes, used by other mods. This ones are not fully finished (e.G. save related issues, or stockmanager&etc not showing them), with a lot of issues. Use at own risk, and if possible try to complete them.

namespace Gnomodia.ContentMods
{
    public static class CustomTextureManager
    {
        private static Dictionary<Assembly, Dictionary<string, Texture2D>> resourceTextures = null;

        public static Texture2D GetFromAssemblyResource(Assembly sourceAssembly, string manifestResourceName)
        {
            if (resourceTextures == null)
            {
                resourceTextures = new Dictionary<Assembly, Dictionary<string, Texture2D>>();
            }
            Dictionary<string, Texture2D> assembliesTextures;
            if (!resourceTextures.TryGetValue(sourceAssembly, out assembliesTextures))
            {
                assembliesTextures = resourceTextures[sourceAssembly] = new Dictionary<string, Texture2D>();
            }
            Texture2D alreadyLoadedTexture;
            if (assembliesTextures.TryGetValue(manifestResourceName, out alreadyLoadedTexture))
            {
                return alreadyLoadedTexture;
            }
            var gd = GnomanEmpire.Instance.GraphicsDevice;
            var stream = sourceAssembly.GetManifestResourceStream(manifestResourceName);
            //FileStream stream = new FileStream("Content/Tilesheet/default.png", FileMode.Open, FileAccess.Read);
            return assembliesTextures[manifestResourceName] = Texture2D.FromStream(gd, stream);
        }
    }
}