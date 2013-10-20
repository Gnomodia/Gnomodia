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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace GnomodiaUI.Controls
{
    public class GnomoriaLabel : UserControl
    {
        private static readonly SpriteFont Font;
        private static readonly BitmapImage FontImage;

        private static readonly FieldInfo TextureValueField = typeof(SpriteFont).GetField("textureValue", BindingFlags.Instance | BindingFlags.NonPublic);

        static GnomoriaLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GnomoriaLabel),
                new FrameworkPropertyMetadata(typeof(GnomoriaLabel)));

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            using (ZipFile skinZip = new ZipFile(Path.Combine(GnomodiaControls.GnomoriaPath, @"Content\UI\Default.skin")))
            {
                using (XnbUtilities xnbUtilities = new XnbUtilities(skinZip))
                {
                    Font = xnbUtilities.Load<SpriteFont>("Fonts/Default");
                    FontImage = xnbUtilities.GetImage((Texture2D)TextureValueField.GetValue(Font));
                }
            }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(GnomoriaLabel), new PropertyMetadata(default(String), TextChanged));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static int GetIndexForCharacter(SpriteFont font, char character)
        {
            int num1 = 0;
            int num2 = font.Characters.Count - 1;
            while (num1 <= num2)
            {
                int index = num1 + (num2 - num1 >> 1);
                char ch = font.Characters[index];
                if (ch == character)
                    return index;
                if (ch < character)
                    num1 = index + 1;
                else
                    num2 = index - 1;
            }
            if (font.DefaultCharacter.HasValue)
            {
                char character1 = font.DefaultCharacter.Value;
                if (character != character1)
                    return GetIndexForCharacter(font, character1);
            }
            else
            {
                if (character != '?')
                    return GetIndexForCharacter(font, '?');
            }
            return -1;
        }

        private static readonly FieldInfo KerningField = typeof(SpriteFont).GetField("kerning", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo SpacingField = typeof(SpriteFont).GetField("spacing", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo GlyphDataField = typeof(SpriteFont).GetField("glyphData", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo CroppingDataField = typeof(SpriteFont).GetField("croppingData", BindingFlags.Instance | BindingFlags.NonPublic);

        private static Image GenerateSpriteTextImage(string text)
        {
            Vector2 textblockPosition = new Vector2(0, 0);
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen())
            {
                List<Vector3> kerning = (List<Vector3>)KerningField.GetValue(Font);
                float spacing = (float)SpacingField.GetValue(Font);
                List<Rectangle> glyphData = (List<Rectangle>)GlyphDataField.GetValue(Font);
                List<Rectangle> croppingData = (List<Rectangle>)CroppingDataField.GetValue(Font);

                Vector2 characterPosition = new Vector2();
                bool firstCharacter = true;
                foreach (char character in text)
                {
                    int indexForCharacter = GetIndexForCharacter(Font, character);

                    Vector3 characterKerning = kerning[indexForCharacter];
                    if (firstCharacter)
                        characterKerning.X = Math.Max(characterKerning.X, 0.0f);
                    else
                        characterPosition.X += spacing;
                    characterPosition.X += characterKerning.X;
                    Rectangle source = glyphData[indexForCharacter];
                    Rectangle cropping = croppingData[indexForCharacter];

                    Vector2 destination = characterPosition;
                    destination.X += cropping.X;
                    destination.Y += cropping.Y;
                    destination += textblockPosition;

                    CroppedBitmap cb = new CroppedBitmap(FontImage, new Int32Rect(source.X, source.Y, source.Width, source.Height));

                    dc.DrawImage(cb, new Rect(destination.X, destination.Y, source.Width, source.Height));
                    firstCharacter = false;
                    characterPosition.X += (characterKerning.Y + characterKerning.Z);
                }
            }

            Vector2 size = Font.MeasureString(text);
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)size.X, (int)size.Y, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(dv);

            return new Image { Source = bitmap, Stretch = Stretch.None };
        }

        public override void OnApplyTemplate()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            string text = Text;

            StackPanel glyphPanel = GetTemplateChild("glyphPanel") as StackPanel;
            if (glyphPanel == null)
                return;

            glyphPanel.Children.Clear();

            Image labelImage = GenerateSpriteTextImage(text);
            glyphPanel.Children.Add(labelImage);
        }

        private static void TextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(source))
                return;

            if (e.Property == TextProperty)
            {
                string text = (string)e.NewValue;

                StackPanel glyphPanel = ((GnomoriaLabel)source).GetTemplateChild("glyphPanel") as StackPanel;
                if (glyphPanel == null)
                    return;

                glyphPanel.Children.Clear();

                Image labelImage = GenerateSpriteTextImage(text);
                glyphPanel.Children.Add(labelImage);
            }
        }
    }
}
