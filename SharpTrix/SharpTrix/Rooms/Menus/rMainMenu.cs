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

namespace AHD.SharpTrix
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class rMainMenu : Microsoft.Xna.Framework.GameComponent
    {
        Texture2D tFrame;
        SpriteFont Font_large;
        SpriteFont Font_small;
        Texture2D tBackground;
        int MenuIndex = 0;
        bool Pressed = false;
        int countdown = 0;
        bool FirstOpen = true;
        //items coordinates
        int y = 260;
        int x = 20;
        int vscpace = 40;
        bool ShowExitMessage = false;

        SoundEffect seClick;

        public rMainMenu(Game game)
            : base(game)
        {
            tFrame = base.Game.Content.Load<Texture2D>(@"Backgrounds\frame");
            Font_large = base.Game.Content.Load<SpriteFont>(@"Fonts\FontLarge");
            Font_small = base.Game.Content.Load<SpriteFont>(@"Fonts\FontSmall");
            tBackground = base.Game.Content.Load<Texture2D>(@"Backgrounds\MainMenuGreenTableBackground");
            seClick = base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
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
                for (int i = 0; i < 5; i++)
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
                    if (MenuIndex > 4)
                        MenuIndex = 4;
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
                    base.Game.Exit();
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
                    case 0://Single Player
                        ((TrixCore)base.Game).PlaySound(seClick);
                        ((TrixCore)base.Game).Room = CurrentRoom.NewGameMenu;
                        ((TrixCore)base.Game).rNewGame.FirstOpen = true;
                        break;
                    case 1://Settings
                        ((TrixCore)base.Game).PlaySound(seClick);
                        ((TrixCore)base.Game).rSettings.LoadSettings();
                        ((TrixCore)base.Game).rSettings.ComeFromInGameMenu = false;
                        ((TrixCore)base.Game).rSettings.FirstOpen = true;
                        ((TrixCore)base.Game).Room = CurrentRoom.Settings;
                        break;
                    case 2://Rules
                        ((TrixCore)base.Game).PlaySound(seClick);
                        ((TrixCore)base.Game).Room = CurrentRoom.Rules;
                        break;
                    case 3://Credits
                        ((TrixCore)base.Game).PlaySound(seClick);
                        ((TrixCore)base.Game).Room = CurrentRoom.Credits;
                        break;
                    case 4://Exit
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

            spriteBatch.DrawString(Font_small, "version " + Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                new Vector2(base.Game.GraphicsDevice.Viewport.Width-Font_small.MeasureString("version " +
                    Assembly.GetExecutingAssembly().GetName().Version.ToString()).X - 5, base.Game.GraphicsDevice.Viewport.Height - 15), Color.White);
            if (!ShowExitMessage)
            {
                #region Draw menu items
                int sy = y;
                spriteBatch.DrawString(Font_large, "Single Player", new Vector2(x, sy), MenuIndex == 0 ? Color.Yellow : Color.White);
                sy += vscpace;
                spriteBatch.DrawString(Font_large, "Options", new Vector2(x, sy), MenuIndex == 1 ? Color.Yellow : Color.White);
                sy += vscpace;
                spriteBatch.DrawString(Font_large, "Rules", new Vector2(x, sy), MenuIndex == 2 ? Color.Yellow : Color.White);
                sy += vscpace;
                spriteBatch.DrawString(Font_large, "About", new Vector2(x, sy), MenuIndex == 3 ? Color.Yellow : Color.White);
                sy += vscpace;
                spriteBatch.DrawString(Font_large, "Exit", new Vector2(x, sy), MenuIndex == 4 ? Color.Yellow : Color.White);
                #endregion
            }
            else
            {
                //draw message
                spriteBatch.DrawString(Font_large, "ARE YOU SURE ? \nPress Y to exit, N to cancel.", new Vector2(x, y + 50), Color.White);
            }
        }
    }
}
