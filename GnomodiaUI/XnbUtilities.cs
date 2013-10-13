using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GnomodiaUI
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
            _contentManager = new ContentManager(new ServiceProvider());
            _contentManager.RootDirectory = rootDirectory;
        }

        public T Load<T>(string xnbPath)
        {
            FileInfo xnbFile = new FileInfo(xnbPath);
            if (!xnbFile.Exists)
                return default(T);

            return _contentManager.Load<T>(Path.GetFileNameWithoutExtension(xnbFile.Name));
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
