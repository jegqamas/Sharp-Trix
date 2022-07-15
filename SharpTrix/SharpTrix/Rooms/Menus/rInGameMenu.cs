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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AHD.SharpTrix
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class rInGameMenu : Microsoft.Xna.Framework.GameComponent
    {
        Texture2D tFrame;
        SpriteFont Font_large;
        Texture2D tBackground;
        int MenuIndex = 0;
        bool Pressed = false;
        int countdown = 0;
        bool FirstOpen = true;
        SoundEffect seClick;
        bool ShowExitMessage = false;
        public rInGameMenu(Game game)
            : base(game)
        {
            tFrame = base.Game.Content.Load<Texture2D>(@"Backgrounds\frame");
            Font_large = base.Game.Content.Load<SpriteFont>(@"Fonts\FontLarge");
            tBackground = base.Game.Content.Load<Texture2D>(@"Backgrounds\GreenTableBackground");
            seClick = base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x");
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
            if (ms.X < 250)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (ms.Y >= 250 + (40 * i) & ms.Y < 250 + (40 * (i + 1)))
                    {
                        MenuIndex = i;
                    }
                }
            }
            #endregion
            if ((Keyboard.GetState().IsKeyDown(Keys.Enter) | (ms.LeftButton == ButtonState.Pressed))& FirstOpen)
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
                    if (MenuIndex > 2)
                        MenuIndex = 2;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    MenuIndex--;
                    if (MenuIndex < 0)
                        MenuIndex = 0;
                    Pressed = true;
                }
                //Action
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) | ((ms.LeftButton == ButtonState.Pressed) & ms.X < 250) & !ShowExitMessage)
                {
                    DoAction();
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Y) & ShowExitMessage)
                {
                    ((TrixCore)base.Game).PlaySound(seClick);
                    ((TrixCore)base.Game).Room = CurrentRoom.MainMenu;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.N) & ShowExitMessage)
                {
                    ShowExitMessage = false;
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
            if (!ShowExitMessage)
            {
                switch (MenuIndex)
                {
                    case 0://Resume
                        ((TrixCore)base.Game).PlaySound(seClick);
                        ((TrixCore)base.Game).Room = CurrentRoom.GamePlay;
                        ((TrixCore)base.Game).rGamePlay.IsPlaying = true;
                        break;
                    case 1://Settings
                        ((TrixCore)base.Game).PlaySound(seClick);
                        ((TrixCore)base.Game).rSettings.LoadSettings();
                        ((TrixCore)base.Game).rSettings.ComeFromInGameMenu = true;
                        ((TrixCore)base.Game).Room = CurrentRoom.Settings;
                        break;
                    case 2://Exit To Main Menu
                        ShowExitMessage = true;
                        break;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draw background
            spriteBatch.Draw(tBackground,
            new Rectangle(0, 0, base.Game.GraphicsDevice.Viewport.Width,
            base.Game.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.DrawString(Font_large, "PAUSE MENU", new Vector2(20, 200), Color.White);
            if (!ShowExitMessage)
            {
                int y = 260;
                int x = 20;
                spriteBatch.DrawString(Font_large, "Resume", new Vector2(x, y), MenuIndex == 0 ? Color.Yellow : Color.White);
                y += 40;
                spriteBatch.DrawString(Font_large, "Options", new Vector2(x, y), MenuIndex == 1 ? Color.Yellow : Color.White);
                y += 40;
                spriteBatch.DrawString(Font_large, "Exit To Main Menu", new Vector2(x, y), MenuIndex == 2 ? Color.Yellow : Color.White);
            }
            else
            {
                int y = 260;
                int x = 20;
                //draw message
                spriteBatch.DrawString(Font_large, "ARE YOU SURE ? \nPress Y to exit, N to cancel.", new Vector2(x, y + 50), Color.White);
            }
        }
    }
}
