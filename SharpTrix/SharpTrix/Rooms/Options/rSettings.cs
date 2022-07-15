/*  
     This file is part of SharpTrix
    A card game that famous in the Middle East
   
    Copyright (C) 2011  Ala Hadid

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using AHD.SharpTrix.Core;

namespace AHD.SharpTrix
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class rSettings : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_large; 
        SpriteFont Font_small;
        Texture2D tBackground;
        int MenuIndex = 0;
        bool Pressed = false;
        int countdown = 0;
        public bool FirstOpen = true;

        int y = 140;
        int x = 20;
        int ox = 320;
        int vscpace = 40;
        SoundEffect seClick;
        string[] AvailableCarSources = 
        {
          @"PlayCards\Basic\", 
          @"PlayCards\Default\",
          @"PlayCards\150\"
        };
        int cardsIndex = 0;
        int cardLatchedWidth = 35;
        int cardLatchedHeight = 40;
        string[] cardsToDraw = { "backside", "c_02", "h_03", "d_j", "h_k", "s_a" };
        public bool ComeFromInGameMenu = false;

        public rSettings(Game game)
            : base(game)
        {
            Font_small = base.Game.Content.Load<SpriteFont>(@"Fonts\FontSmall");
            Font_large = base.Game.Content.Load<SpriteFont>(@"Fonts\FontLarge");
            tBackground = base.Game.Content.Load<Texture2D>(@"Backgrounds\GreenTableBackground");
            seClick = base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x");
        }

        public void LoadSettings()
        {
            //card resource
            for (int i = 0; i < AvailableCarSources.Length; i++)
            {
                if (Program.Settings.GamePlay_cardsSourceFolder == AvailableCarSources[i])
                {
                    cardsIndex = i;
                    break;
                }
            }
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
#if WINDOWS
            #region Mouse
            MouseState ms = Mouse.GetState();
            if (ms.X < 350)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (ms.Y >= y + (vscpace * i) & ms.Y < y + (vscpace * (i + 1)))
                    {
                        MenuIndex = i;
                        break;
                    }
                }
            }
            #endregion
            if ((Keyboard.GetState().IsKeyDown(Keys.Enter) | (ms.LeftButton == ButtonState.Pressed)) & FirstOpen)
            {
                Pressed = true;
                return;
            }
            if (Keyboard.GetState().GetPressedKeys().Length == 0 & ms.LeftButton == ButtonState.Released & FirstOpen)
                FirstOpen = false;
            if (!Pressed)
            {
                countdown = 10;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    MenuIndex++; ;
                    if (MenuIndex > 7)
                        MenuIndex = 7;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    MenuIndex--;
                    if (MenuIndex < 0)
                        MenuIndex = 0;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) | Keyboard.GetState().IsKeyDown(Keys.Left)
                    | ((ms.LeftButton == ButtonState.Pressed) & ms.X < 250))
                {
                    ChangeOption(Keyboard.GetState().IsKeyDown(Keys.Right) | ((ms.LeftButton == ButtonState.Pressed) & ms.X < 250));
                    Pressed = true;
                }
                //Action
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) | ((ms.LeftButton == ButtonState.Pressed) & ms.X < 250))
                {
                    DoAction();
                    Pressed = true;
                }
            }
            else
            {
                if (countdown > 0)
                    countdown--;
                else
                    Pressed = false;
            }
#endif
            base.Update(gameTime);
        }
        void DoAction()
        {
            switch (MenuIndex)
            {
                case 6://reset to defaults
                    ((TrixCore)base.Game).PlaySound(seClick);
                    Program.Settings.Video_FullScreen = false;
                    Program.Settings.Video_ResIndex = 0;
                    Program.Settings.Sound_Enabled = true;
                    Program.Settings.Sound_EffectsVolume = 1.0f;
                    Program.Settings.Sound_MusicVolume = 1.0f;
                    Program.Settings.GamePlay_cardsSourceFolder = @"PlayCards\Basic\";
                    cardsIndex = 0;
                    ((TrixCore)base.Game).DrawStatusString("Settings reset to defaults", 60);
                    break;
                case 7://save and back
                    ((TrixCore)base.Game).PlaySound(seClick);
                    Program.SaveSettings();
                    Program.ApplyVideoSettings();
                    ((TrixCore)base.Game).rGamePlay.LoadResources();
                    if (!Program.Settings.Sound_Enabled)
                        ((TrixCore)base.Game).StopMusicFadeOut();
                    if (!ComeFromInGameMenu)
                        ((TrixCore)base.Game).Room = CurrentRoom.MainMenu;
                    else
                        ((TrixCore)base.Game).Room = CurrentRoom.InGameMenu;
                    break;
            }
        }
        void ChangeOption(bool isAdvance)
        {
            switch (MenuIndex)
            {
                case 0://video res
                    if (isAdvance)
                    {
                        Program.Settings.Video_ResIndex++;
                        if (Program.Settings.Video_ResIndex >= Program.VideoModes.Length)
                            Program.Settings.Video_ResIndex = Program.VideoModes.Length - 1;
                    }
                    else
                    {
                        Program.Settings.Video_ResIndex--;
                        if (Program.Settings.Video_ResIndex <= 0)
                            Program.Settings.Video_ResIndex = 0;
                    }
                    ((TrixCore)base.Game).PlaySound(seClick);
                    break;
                case 1://fullscreen
                    ((TrixCore)base.Game).PlaySound(seClick);
                    Program.Settings.Video_FullScreen = !Program.Settings.Video_FullScreen;
                    break;
                case 2://sound
                    ((TrixCore)base.Game).PlaySound(seClick);
                    Program.Settings.Sound_Enabled = !Program.Settings.Sound_Enabled;
                    break;
                case 3://sound sfx
                    if (isAdvance)
                    {
                        Program.Settings.Sound_EffectsVolume += 0.01f;
                        if (Program.Settings.Sound_EffectsVolume >= 1.0f)
                            Program.Settings.Sound_EffectsVolume = 1.0f;
                    }
                    else
                    {
                        Program.Settings.Sound_EffectsVolume -= 0.01f;
                        if (Program.Settings.Sound_EffectsVolume <= 0.0f)
                            Program.Settings.Sound_EffectsVolume = 0.0f;
                    }
                    ((TrixCore)base.Game).PlaySound(seClick);
                    break;
                case 4://sound music
                    if (isAdvance)
                    {
                        Program.Settings.Sound_MusicVolume += 0.01f;
                        if (Program.Settings.Sound_MusicVolume >= 1.0f)
                            Program.Settings.Sound_MusicVolume = 1.0f;
                    }
                    else
                    {
                        Program.Settings.Sound_MusicVolume -= 0.01f;
                        if (Program.Settings.Sound_MusicVolume <= 0.0f)
                            Program.Settings.Sound_MusicVolume = 0.0f;
                    }
                    ((TrixCore)base.Game).PlaySound(seClick);
                    MediaPlayer.Volume = Program.Settings.Sound_MusicVolume;
                    break;
                case 5://play cards
                    if (isAdvance)
                    {
                        cardsIndex++;
                        if (cardsIndex >= AvailableCarSources.Length)
                            cardsIndex = 0;
                    }
                    else
                    {
                        cardsIndex--;
                        if (cardsIndex < 0)
                            cardsIndex = AvailableCarSources.Length - 1;
                    }
                    Program.Settings.GamePlay_cardsSourceFolder = AvailableCarSources[cardsIndex];
                    ((TrixCore)base.Game).PlaySound(seClick);
                    break;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draw background
            spriteBatch.Draw(tBackground,
            new Rectangle(0, 0, base.Game.GraphicsDevice.Viewport.Width,
            base.Game.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.DrawString(Font_small, "version " + Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                          new Vector2(base.Game.GraphicsDevice.Viewport.Width - Font_small.MeasureString("version " +
                              Assembly.GetExecutingAssembly().GetName().Version.ToString()).X - 5, base.Game.GraphicsDevice.Viewport.Height - 15), Color.White);
           
            spriteBatch.DrawString(Font_large, "OPTIONS", new Vector2((base.Game.GraphicsDevice.Viewport.Width / 2) -
(Font_large.MeasureString("OPTIONS").X / 2), 50), Color.White);

            #region Draw menu items
            int sy = y;
            spriteBatch.DrawString(Font_large, "Video Resolution", new Vector2(x, sy), MenuIndex == 0 ? Color.Yellow : Color.White);
            spriteBatch.DrawString(Font_large, Program.GetVideoModeText(Program.Settings.Video_ResIndex),
                new Vector2(ox, sy), MenuIndex == 0 ? Color.Yellow : Color.White);
            sy += vscpace;
            spriteBatch.DrawString(Font_large, "Full Screen", new Vector2(x, sy), MenuIndex == 1 ? Color.Yellow : Color.White);
            spriteBatch.DrawString(Font_large, Program.Settings.Video_FullScreen ? "ON" : "OFF", new Vector2(ox, sy), MenuIndex == 1 ? Color.Yellow : Color.White);
            sy += vscpace;
            spriteBatch.DrawString(Font_large, "Sound", new Vector2(x, sy), MenuIndex == 2 ? Color.Yellow : Color.White);
            spriteBatch.DrawString(Font_large, Program.Settings.Sound_Enabled ? "ON" : "OFF", new Vector2(ox, sy), MenuIndex == 2 ? Color.Yellow : Color.White);
            sy += vscpace;
            spriteBatch.DrawString(Font_large, "SFX Volume", new Vector2(x, sy),Program.Settings.Sound_Enabled?( MenuIndex == 3 ? Color.Yellow : Color.White):Color.Gray);
            spriteBatch.DrawString(Font_large, (Program.Settings.Sound_EffectsVolume * 100).ToString("F0") + " %", new Vector2(ox, sy), Program.Settings.Sound_Enabled ? (MenuIndex == 3 ? Color.Yellow : Color.White) : Color.Gray);
            sy += vscpace;
            spriteBatch.DrawString(Font_large, "Music Volume", new Vector2(x, sy), Program.Settings.Sound_Enabled ? (MenuIndex == 4 ? Color.Yellow : Color.White) : Color.Gray);
            spriteBatch.DrawString(Font_large, (Program.Settings.Sound_MusicVolume * 100).ToString("F0") + " %", new Vector2(ox, sy), Program.Settings.Sound_Enabled ? (MenuIndex == 4 ? Color.Yellow : Color.White) : Color.Gray);
            sy += vscpace;
            spriteBatch.DrawString(Font_large, "Play Cards", new Vector2(x, sy), MenuIndex == 5 ? Color.Yellow : Color.White);
            #region cards
            int cardX = ox;
            for (int i = 0; i < cardsToDraw.Length; i++)
            {
                //calculate x position
                Texture2D tCard = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder + cardsToDraw[i]);
                spriteBatch.Draw(tCard, new Rectangle(cardX, sy, cardLatchedWidth, cardLatchedHeight), Color.White);
                cardX += cardLatchedWidth + 1;
            }
            #endregion
            sy += vscpace;
            spriteBatch.DrawString(Font_large, "Reset To Defaults", new Vector2(x, sy), MenuIndex == 6 ? Color.Yellow : Color.White);
            sy += vscpace;
            spriteBatch.DrawString(Font_large, "Back", new Vector2(x, sy), MenuIndex == 7 ? Color.Yellow : Color.White);
            #endregion
        }
    }
}
