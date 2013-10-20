/*
 *  Gnomodia UI Controls
 *
 *  Copyright © 2013 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GnomodiaUI.Controls
{
    internal sealed class XnbUtilities : IDisposable
    {
        private sealed class ServiceProvider : IDisposable, IServiceProvider
        {
            private sealed class FauxGame : Game { }

            private GraphicsDeviceManager _graphicsDeviceManager;
            private FauxGame _fauxGame;

            public ServiceProvider()
            {
                _fauxGame = new FauxGame();
                _graphicsDeviceManager = new GraphicsDeviceManager(_fauxGame);
                ((IGraphicsDeviceManager)_graphicsDeviceManager).CreateDevice();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IGraphicsDeviceService))
                    return _graphicsDeviceManager;

                return null;
            }

            private bool _disposed;

            public void Dispose()
            {
                if (_disposed)
                    return;

                _fauxGame.Dispose();
                _fauxGame = null;
                ((IDisposable)_graphicsDeviceManager).Dispose();
                _graphicsDeviceManager = null;
                _disposed = true;
            }
        }

        private ContentManager _contentManager;
        public XnbUtilities(string rootDirectory)
        {
            _contentManager = new ContentManager(new ServiceProvider()) { RootDirectory = rootDirectory };
        }

        private ZipFile _zipFile;
        public XnbUtilities(ZipFile zipFile)
        {
            _zipFile = zipFile;
            _contentManager = new ContentManager(new ServiceProvider()) { RootDirectory = Path.GetTempPath() };
        }

        public T Load<T>(string xnbPath)
        {
            if (_zipFile != null)
            {
                ZipEntry entry = _zipFile.GetEntry(xnbPath + ".xnb");
                Stream s = _zipFile.GetInputStream(entry);
                xnbPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xnb");
                using (Stream tmpStream = File.OpenWrite(xnbPath))
                {
                    s.CopyTo(tmpStream);
                }
            }

            FileInfo xnbFile = new FileInfo(xnbPath);
            if (!xnbFile.Exists)
                return default(T);

            try
            {
                return _contentManager.Load<T>(Path.GetFileNameWithoutExtension(xnbFile.Name));
            }
            finally
            {
                if (_zipFile != null)
                    File.Delete(xnbPath);
            }
        }

        private static readonly FieldInfo SpriteFontTextureValueField = typeof(SpriteFont).GetField("textureValue", BindingFlags.Instance | BindingFlags.NonPublic);
        public SpriteFont LoadSpriteFont(string xnbPath, out Texture2D fontTexture)
        {
            fontTexture = null;

            SpriteFont spriteFont = Load<SpriteFont>(xnbPath);
            if (spriteFont == null)
                return null;

            fontTexture = (Texture2D)SpriteFontTextureValueField.GetValue(spriteFont);
            return spriteFont;
        }

        public BitmapImage GetImage(Texture2D texture)
        {
            if (texture == null)
                return null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                texture.SaveAsPng(memoryStream, texture.Width, texture.Height);
                memoryStream.Seek(0, SeekOrigin.Begin);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public BitmapImage LoadTextureAsImage(string xnbPath)
        {
            Texture2D texture = Load<Texture2D>(xnbPath);
            return GetImage(texture);
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _contentManager.Dispose();
            _contentManager = null;
            _disposed = true;
        }
    }
}
