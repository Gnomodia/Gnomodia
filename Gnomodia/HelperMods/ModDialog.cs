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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using Game;
using Game.GUI;
using Game.GUI.Controls;
using Gnomodia.Annotations;
using Gnomodia.Attributes;
using Gnomodia.Events;
using Gnomodia.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EventArgs = Game.GUI.Controls.EventArgs;

namespace Gnomodia.HelperMods
{
    [Export(typeof(IMod))]
    public class ModDialog : SupportMod
    {
        #region Setup stuff
        public override string Author
        {
            get
            {
                return "alexschrod";
            }
        }
        public override string Description
        {
            get
            {
                return "Shows information about and lets you configure mods in-game";
            }
        }
        public override Version Version
        {
            get { return typeof(ModDialog).Assembly.GetName().Version; }
        }
        #endregion

        [Instance]
        private static ModDialog Instance { get; [UsedImplicitly] set; }

        [Import]
        private IModManager ModManager { get; set; }

        private static HUD s_Hud;
        private static Label s_GnomodiaVersionLabel;
        private static Label s_GnomoriaVersionLabel;

        public override IEnumerable<IModification> Modifications
        {
            get
            {
                yield return new MethodHook(
                    typeof(HUD).GetMethod("c634754f1b7c29a47f4139684f06df05a", BindingFlags.Instance | BindingFlags.NonPublic),
                    Method.Of<HUD>(AddHudModButton));
                yield return new MethodHook(
                    typeof(MainMenuWindow).GetConstructor(new[] { typeof(Manager) }),
                    Method.Of<MainMenuWindow, Manager>(AddMainMenuModButton));
                yield return new MethodHook(
                    typeof(MainMenuWindow).GetConstructor(new[] { typeof(Manager) }),
                    Method.Of<MainMenuWindow, Manager>(SetGnomodiaLogo));
                yield return new MethodHook(
                    typeof(MainMenuWindow).GetMethod("ceb32813158d52d65f7977184d5fc8c24", BindingFlags.Instance | BindingFlags.NonPublic),
                    Method.Of<MainMenuWindow, object, System.EventArgs>(Reset));
            }
        }

        private static readonly FieldInfo HudPanelField = typeof(HUD).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Single(f => f.FieldType == typeof(Panel));
        public static void AddHudModButton(HUD hud)
        {
            s_Hud = hud;

            Panel buttonPanel = (Panel)HudPanelField.GetValue(hud);

            Button helpButton;
            if (!buttonPanel.FindControlRecursive(out helpButton, b => b.Text == "Help"))
                return;

            Button modsButton = new Button(buttonPanel.Manager);
            modsButton.Init();
            SkinLayer skinLayer = modsButton.Skin.Layers["Control"];
            modsButton.Text = "Mods";
            modsButton.Width = (int)skinLayer.Text.Font.Resource.MeasureString(modsButton.Text).X + skinLayer.ContentMargins.Horizontal + 1;
            modsButton.ToolTip.Text = "Show information about and settings for mods";
            modsButton.Margins = new Margins(4, 4, 4, 4);
            modsButton.Click += ModsButtonOnClick;
            modsButton.Left = helpButton.Left;
            buttonPanel.Add(modsButton);

            helpButton.Left = modsButton.Left + modsButton.Width + modsButton.Margins.Right + helpButton.Margins.Left;

            buttonPanel.Width = helpButton.Left + helpButton.Width;
            buttonPanel.Left = (hud.Width - buttonPanel.Width) / 2;
        }

        private static readonly FieldInfo MainMenuWindowPanelField = typeof(MainMenuWindow).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Single(f => f.FieldType == typeof(Panel));
        public static void AddMainMenuModButton(MainMenuWindow mainMenu, Manager manager)
        {
            Panel buttonPanel = (Panel)MainMenuWindowPanelField.GetValue(mainMenu);

            Button exitButton;
            if (!buttonPanel.FindControlRecursive(out exitButton, b => b.Text == "Exit"))
                return;

            Button modsButton = new Button(manager);
            modsButton.Init();
            modsButton.Width = 200;
            modsButton.Top = exitButton.Top;
            modsButton.Left = (buttonPanel.Width - modsButton.Width) / 2;
            modsButton.Margins = new Margins(0, 2, 0, 2);
            modsButton.Text = "Mods";
            buttonPanel.Height += modsButton.Height + 4;
            modsButton.Click += MainMenuModsButtonClick;
            buttonPanel.Add(modsButton);

            exitButton.Top = modsButton.Top + modsButton.Height + modsButton.Margins.Bottom + exitButton.Margins.Top;

            if (!mainMenu.FindControlRecursive(out s_GnomoriaVersionLabel, l => l.Text.StartsWith("v")))
                return;

            s_GnomoriaVersionLabel.Text = "Gnomoria " + s_GnomoriaVersionLabel.Text;

            s_GnomodiaVersionLabel = new Label(manager);
            s_GnomodiaVersionLabel.Init();
            s_GnomodiaVersionLabel.Alignment = Alignment.MiddleRight;
            s_GnomodiaVersionLabel.Text = "Gnomodia v" + typeof(ModDialog).Assembly.GetInformationalVersion();
            mainMenu.Add(s_GnomodiaVersionLabel);

            Reset(mainMenu, null, null);
        }

        public static void SetGnomodiaLogo(MainMenuWindow mainMenu, Manager manager)
        {
            Panel buttonPanel = (Panel)MainMenuWindowPanelField.GetValue(mainMenu);

            ImageBox logo;
            if (!buttonPanel.FindControlRecursive(out logo))
                return;

            Stream logoStream = typeof(ModDialog).Assembly.GetManifestResourceStream("Gnomodia.Images.Gnomodia.png");
            Texture2D logoTexture = Texture2D.FromStream(GnomanEmpire.Instance.GraphicsDevice, logoStream);
            logo.Image = logoTexture;

            logo.SizeMode = SizeMode.Stretched;
            logo.Width = logoTexture.Width * 3;
            logo.Height = logoTexture.Height * 3;

            logo.Left = (buttonPanel.Width - logo.Width) / 2;

            Button[] buttons;
            buttonPanel.FindControlsRecursive(out buttons, b => !string.IsNullOrEmpty(b.Text));
            foreach (var button in buttons)
            {
                button.Top += 76;
            }
        }

        public static void Reset(MainMenuWindow mainMenu, object sender, System.EventArgs eventArgs)
        {
            s_GnomodiaVersionLabel.CalculateWidth();
            s_GnomodiaVersionLabel.Left = mainMenu.ClientWidth - s_GnomodiaVersionLabel.Width - s_GnomodiaVersionLabel.Margins.Right;
            s_GnomodiaVersionLabel.Top = s_GnomoriaVersionLabel.Top;

            s_GnomoriaVersionLabel.CalculateWidth();
            s_GnomoriaVersionLabel.Left = mainMenu.ClientWidth - s_GnomoriaVersionLabel.Width - s_GnomoriaVersionLabel.Margins.Right;
            s_GnomoriaVersionLabel.Top = s_GnomodiaVersionLabel.Top - s_GnomodiaVersionLabel.Margins.Top - 11 - s_GnomoriaVersionLabel.Margins.Bottom;
        }

        private static void MainMenuModsButtonClick(object sender, EventArgs e)
        {
            GnomanEmpire.Instance.GuiManager.MenuStack.PushWindow(new ModsMenu(GnomanEmpire.Instance.GuiManager.Manager, Instance.ModManager));
        }

        private static void ModsButtonOnClick(object sender, EventArgs eventArgs)
        {
            InGameHUD inGameHud = GnomanEmpire.Instance.GuiManager.HUD;
            bool isOtherWindow = !(inGameHud.ActiveWindow is ModDialogUI);
            inGameHud.CloseWindow();
            if (isOtherWindow)
            {
                inGameHud.ShowWindow(new ModDialogUI(s_Hud.Manager, Instance.ModManager));
            }
        }
    }

    internal sealed class ModsMenu : Panel
    {
        private readonly RaisedPanel _raisedPanel;

        public ModsMenu(Manager manager, IModManager modManager)
            : base(manager)
        {
            this.Init();
            this.Width = this.Manager.ScreenWidth;
            this.Height = this.Manager.ScreenHeight;
            this.Color = Color.Transparent;
            GnomanEmpire.Instance.GuiManager.Add(this);
            GnomanEmpire.Instance.Graphics.DeviceReset += (sender, args) =>
            {
                Width = manager.ScreenWidth;
                Height = manager.ScreenHeight;

                SetPosition();
            };
            _raisedPanel = new RaisedPanel(manager);
            _raisedPanel.Init();
            Add(_raisedPanel);
            AboutModsPanel aboutModsPanel = new AboutModsPanel(_raisedPanel, modManager);

            Button button = new Button(this.Manager);
            button.Init();
            button.Width = 100;
            button.Left = button.Margins.Left;
            button.Top = aboutModsPanel.ModListBox.Top + aboutModsPanel.ModListBox.Height + aboutModsPanel.ModListBox.Margins.Bottom + button.Margins.Top;
            button.Text = "Back";
            button.Click += (sender, args) => GnomanEmpire.Instance.GuiManager.MenuStack.PopWindow();
            this._raisedPanel.Add(button);

            this._raisedPanel.Width = aboutModsPanel.ModInfoPanel.Left + aboutModsPanel.ModInfoPanel.Width + this._raisedPanel.ClientMargins.Horizontal + aboutModsPanel.ModListBox.Margins.Right;
            this._raisedPanel.Height = button.Top + button.Height + button.Margins.Bottom + this._raisedPanel.ClientMargins.Vertical;
            SetPosition();
        }

        private void SetPosition()
        {
            _raisedPanel.Left = (this.Width - this._raisedPanel.Width) / 2;
            _raisedPanel.Top = (this.Height - this._raisedPanel.Height) / 2;
        }
    }

    internal sealed class ModDialogUI : TabbedWindow
    {
        private static int s_Width = 750;
        private static int s_Height = 500;
        private static Vector2 s_Position = new Vector2(-1f);

        public ModDialogUI(Manager manager, IModManager modManager)
            : base(manager)
        {
            Text = "Mods";
            Width = 750;
            Height = 500;
            Resizable = false;
            Center();
            if (s_Position.X < 0f || s_Position.Y < 0f || s_Position.X + s_Width >= Manager.ScreenWidth || s_Position.Y + s_Height >= Manager.ScreenHeight)
            {
                s_Width = this.Width;
                s_Height = this.Height;
                s_Position.X = this.Left;
                s_Position.Y = this.Top;
            }
            AddPage("About", new GnomodiaPanelUI(this.Manager, modManager));
            Width = s_Width;
            Height = s_Height;
            Left = (int)s_Position.X;
            Top = (int)s_Position.Y;

            Resize += (sender, args) =>
            {
                s_Width = Width;
                s_Height = Height;
                s_Position.X = this.Left;
                s_Position.Y = this.Top;
            };

            Move += (sender, args) =>
            {
                s_Position.X = this.Left;
                s_Position.Y = this.Top;
            };
        }
    }

    internal class AboutModsPanel
    {
        private readonly IModManager _modManager;
        internal LoweredPanel ModInfoPanel;
        internal ListBox ModListBox;

        //private IList<IMod> _mods;

        private readonly Label _titleLabel;
        private readonly Label _versionLabel;
        private readonly Label _authorLabel;
        private readonly MultilineLabel _infoLabel;

        private void ModListBoxOnItemIndexChanged(object sender, EventArgs eventArgs)
        {
            if (ModListBox.ItemIndex == 0)
            {
                _titleLabel.Text = "Gnomodia";

                _infoLabel.Text = "Modding toolkit to add additional features or modify existing features of Gnomoria. Gnomodia is based on a Modding Framework for Gnormoia by Faark (faark.de).";

                _versionLabel.Text = string.Format("Version: {0}", typeof(ModDialog).Assembly.GetName().Version);
                _authorLabel.Text = "Author(s): The Gnomodia Team";
            }
            else
            {
                int modIndex = ModListBox.ItemIndex - 1;
                IMod mod = _modManager.Mods[modIndex];

                _titleLabel.Text = mod.Name;

                _infoLabel.Text = mod.Description;

                _versionLabel.Text = string.Format("Version: {0}", mod.Version);
                _authorLabel.Text = string.Format("Author(s): {0}", mod.Author);
            }

            _infoLabel.Height = _infoLabel.FullHeight;
        }

        public AboutModsPanel(Panel containingPanel, IModManager modManager, int desiredHeight = 500, int desiredWidth = 750)
        {
            _modManager = modManager;

            ModListBox = new ListBox(containingPanel.Manager);
            ModListBox.Init();
            ModListBox.Top = ModListBox.Margins.Top;
            ModListBox.Left = ModListBox.Margins.Left;
            ModListBox.Width = 250;
            ModListBox.Height = desiredHeight - containingPanel.ClientMargins.Vertical - ModListBox.Top - ModListBox.Margins.Bottom;
            ModListBox.HideSelection = false;

            ModListBox.c81fb310624c15a101535d14adc9ec383.Add("Gnomodia");
            ModListBox.ItemIndexChanged += ModListBoxOnItemIndexChanged;

            foreach (IMod mod in modManager.Mods)
            {
                ModListBox.c81fb310624c15a101535d14adc9ec383.Add(mod.Name);
            }

            containingPanel.Add(ModListBox);

            ModInfoPanel = new LoweredPanel(containingPanel.Manager);
            ModInfoPanel.Init();
            ModInfoPanel.Left = ModListBox.Left + ModListBox.Width + ModListBox.Margins.Right + ModInfoPanel.Margins.Left;
            ModInfoPanel.Top = ModInfoPanel.Margins.Top;
            ModInfoPanel.Width = desiredWidth - ModInfoPanel.Left - ModInfoPanel.Margins.Right;
            ModInfoPanel.Height = desiredHeight - containingPanel.ClientMargins.Vertical - ModInfoPanel.Top - ModInfoPanel.Margins.Bottom;
            ModInfoPanel.AutoScroll = true;
            ModInfoPanel.HorizontalScrollBarEnabled = false;
            ModInfoPanel.VerticalScrollBarShow = true;
            containingPanel.Add(this.ModInfoPanel);

            _titleLabel = new Label(containingPanel.Manager);
            _titleLabel.Init();
            _titleLabel.Top = _titleLabel.Margins.Top;
            _titleLabel.Left = _titleLabel.Margins.Left;
            _titleLabel.Height = 14;
            _titleLabel.Width = ModInfoPanel.ClientWidth - _titleLabel.Margins.Horizontal;
            _titleLabel.Text = "";
            ModInfoPanel.Add(_titleLabel);

            _versionLabel = new Label(containingPanel.Manager);
            _versionLabel.Init();
            _versionLabel.Left = _versionLabel.Margins.Left;
            _versionLabel.Height = 14;
            _versionLabel.Width = ModInfoPanel.Width - _versionLabel.Margins.Horizontal;
            _versionLabel.Top = _titleLabel.Top + _titleLabel.Height + _titleLabel.Margins.Bottom + _versionLabel.Margins.Top;
            _versionLabel.Text = "";
            ModInfoPanel.Add(_versionLabel);

            _authorLabel = new Label(containingPanel.Manager);
            _authorLabel.Init();
            _authorLabel.Left = _authorLabel.Margins.Left;
            _authorLabel.Height = 14;
            _authorLabel.Width = ModInfoPanel.Width - _authorLabel.Margins.Horizontal;
            _authorLabel.Top = _versionLabel.Top + _versionLabel.Height + _versionLabel.Margins.Bottom + _authorLabel.Margins.Top;
            _authorLabel.Text = "";
            ModInfoPanel.Add(_authorLabel);

            _infoLabel = new MultilineLabel(containingPanel.Manager);
            _infoLabel.Left = _infoLabel.Margins.Left;
            _infoLabel.Height = 0;
            _infoLabel.Top = _authorLabel.Top + _authorLabel.Height + _authorLabel.Margins.Bottom + _infoLabel.Margins.Top;
            _infoLabel.Width = ModInfoPanel.ClientWidth - _infoLabel.Margins.Horizontal;
            _infoLabel.Anchor = Anchors.Top | Anchors.Left | Anchors.Right;
            _infoLabel.Alignment = Alignment.TopLeft;
            ModInfoPanel.Add(_infoLabel);

            ModListBox.ItemIndex = 0;
        }
    }

    internal class GnomodiaPanelUI : TabbedWindowPanel
    {
        private readonly IModManager _modManager;
        [UsedImplicitly]
        private AboutModsPanel _aboutModsPanel;

        public GnomodiaPanelUI(Manager manager, IModManager modManager)
            : base(manager)
        {
            _modManager = modManager;
        }

        public override void SetupPanel()
        {
            _aboutModsPanel = new AboutModsPanel(this, _modManager, this.Height, this.Width);
        }
    }
}
