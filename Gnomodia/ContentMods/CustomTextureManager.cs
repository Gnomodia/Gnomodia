/*
 *  Gnomodia
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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