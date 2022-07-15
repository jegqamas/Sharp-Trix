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
    public class rNewGame : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_large;
        Texture2D tBackground;
        int MenuIndex = 0;
        bool Pressed = false;
        int countdown = 0;
        public bool FirstOpen = true;
        SoundEffect seClick;
        string[] Difficults = { "Easy", "Normal" };
        int difficultIndex = 1;
        string PlayerName = "Player 1";
        bool Renaming = false;
        bool FlashOr = false;
        int flashTimer = 20;
        bool Caps = false;
        char[] InputKeys = 
        {
          'a','b','c','d','e','f','g','h',
          'i','j','k','l','m','n','o','p',
          'q','r','s','t','y','v','w','x',
          'y','z'
        };
        string[] InputKeysNumbers =
        { 
          "NumPad0", "NumPad1", "NumPad2", "NumPad3", "NumPad4",
          "NumPad5", "NumPad6", "NumPad7", "NumPad8", "NumPad9",
          "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8",
          "D9",
        };

        public rNewGame(Game game)
            : base(game)
        {
            Font_large = base.Game.Content.Load<SpriteFont>(@"Fonts\FontLarge");
            tBackground = base.Game.Content.Load<Texture2D>(@"Backgrounds\MainMenuGreenTableBackground");
            seClick = base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x");
            PlayerName = Program.Settings.GamePlay_lastPlayerName;
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
                for (int i = 0; i < 4; i++)
                {
                    if (ms.Y >= 250 + (40 * i) & ms.Y < 250 + (40 * (i + 1)))
                    {
                        MenuIndex = i;
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
                countdown = 7;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    MenuIndex++;
                    if (MenuIndex > 3)
                        MenuIndex = 3;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    MenuIndex--;
                    if (MenuIndex < 0)
                        MenuIndex = 0;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) & (MenuIndex == 1) & !Renaming)
                {
                    ((TrixCore)base.Game).PlaySound(seClick);
                    difficultIndex++;
                    if (difficultIndex >= Difficults.Length)
                        difficultIndex = 0;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left) & (MenuIndex == 1) & !Renaming)
                {
                    ((TrixCore)base.Game).PlaySound(seClick);
                    difficultIndex--;
                    if (difficultIndex < 0)
                        difficultIndex = Difficults.Length - 1;
                    Pressed = true;
                }
                //Action
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) | ((ms.LeftButton == ButtonState.Pressed) & ms.X < 250))
                {
                    if (!Renaming)
                    {
                        DoAction();
                        Pressed = true;
                        return;//MAKE IT NOT TO REACH THE RENAMING
                    }
                }
                #region Renaming
                if (Renaming)
                {
                    if (Keyboard.GetState().IsKeyUp(Keys.LeftShift) & Keyboard.GetState().IsKeyUp(Keys.RightShift))
                    { Caps = false; }
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        if (PlayerName.Length > 0)
                        {
                            MenuIndex = 2;
                            Renaming = false;
                            Pressed = true;
                            Program.Settings.GamePlay_lastPlayerName = PlayerName;
                            Program.SaveSettings(false);
                            ((TrixCore)base.Game).PlaySound(seClick);
                        }
                        else
                        {
                            ((TrixCore)base.Game).DrawStatusString("You must enter name !!", 60);
                            Pressed = true;
                        }
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Back))
                    {
                        PlayerName = PlayerName.Substring(0, PlayerName.Length - 1);
                        Pressed = true;
                    }
                    else if (Keyboard.GetState().GetPressedKeys().Length > 0)
                    {
                        if (PlayerName.Length < "Player 1".Length)
                        {
                            if (Keyboard.GetState().GetPressedKeys()[0] == Keys.Space)
                            {
                                PlayerName += " ";
                                Pressed = true;
                            }
                            else
                            {
                             
                                for (int i = 0; i < Keyboard.GetState().GetPressedKeys().Length; i++)
                                {
                                    if (Keyboard.GetState().GetPressedKeys()[i] == (Keys.RightShift | Keys.LeftShift))
                                    {
                                        Caps = true;
                                    }
                                    else
                                    {
                                        string innn = GetInputKey(Keyboard.GetState().GetPressedKeys()[i]);
                                        if (innn != "")
                                        {
                                            PlayerName += innn;
                                        }
                                        Pressed = true;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
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
                case 0://New game
                    ((TrixCore)base.Game).PlaySound(seClick);
                    ((TrixCore)base.Game).Room = CurrentRoom.GamePlay;
                    ((TrixCore)base.Game).rGamePlay.LoadResources();//apply textures
                    ((TrixCore)base.Game).rGamePlay.CreateNewGame(Difficults[difficultIndex], this.PlayerName);
                    //((TrixCore)base.Game).StopMusicFadeOut();
                    break;
                case 1://difficult
                    ((TrixCore)base.Game).PlaySound(seClick);
                    difficultIndex++;
                    if (difficultIndex >= Difficults.Length)
                        difficultIndex = 0;
                    break;
                case 2://player name
                    Renaming = true;
                    break;
                case 3://Back
                    ((TrixCore)base.Game).PlaySound(seClick);
                    ((TrixCore)base.Game).Room = CurrentRoom.MainMenu;
                    break;
            }
        }
        string GetInputKey(Keys key)
        {
      
            for (int i = 0; i < InputKeys.Length; i++)
            {
                if (InputKeys[i].ToString().ToLower() == key.ToString().ToLower())
                {
                    if (Caps)
                    {
                        return InputKeys[i].ToString().ToUpper();
                    } 
                    return InputKeys[i].ToString().ToLower();
                }
            }
            //try numbers
            for (int i = 0; i < InputKeysNumbers.Length; i++)
            {
                if (InputKeysNumbers[i].ToString().ToLower() == key.ToString().ToLower())
                {
                    return InputKeysNumbers[i].ToString().Substring(InputKeysNumbers[i].Length - 1, 1);
                }
            } 
            return "";
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draw background
            spriteBatch.Draw(tBackground,
            new Rectangle(0, 0, base.Game.GraphicsDevice.Viewport.Width,
            base.Game.GraphicsDevice.Viewport.Height), Color.White);
            if (!Renaming)
            {
                int y = 210;
                int x = 20;
                spriteBatch.DrawString(Font_large, "SINGLE PLAYER", new Vector2(x, y), Color.White);
                y += 50;
                spriteBatch.DrawString(Font_large, "Play !", new Vector2(x, y), MenuIndex == 0 ? Color.Yellow : Color.White);
                y += 40;
                spriteBatch.DrawString(Font_large, "AI difficulty:", new Vector2(x, y), MenuIndex == 1 ? Color.Yellow : Color.White);
                spriteBatch.DrawString(Font_large, Difficults[difficultIndex], new Vector2(x + 220, y), MenuIndex == 1 ? Color.Yellow : Color.White);
                y += 40;
                spriteBatch.DrawString(Font_large, "Your Name:", new Vector2(x, y), MenuIndex == 2 ? Color.Yellow : Color.White);
                spriteBatch.DrawString(Font_large, PlayerName, new Vector2(x + 220, y), MenuIndex == 2 ? Color.Yellow : Color.White);
                y += 40;
                spriteBatch.DrawString(Font_large, "Main Menu", new Vector2(x, y), MenuIndex == 3 ? Color.Yellow : Color.White);
            }
            else
            {   
                //flash or
                if (flashTimer > 0)
                    flashTimer--;
                else
                {
                    flashTimer = 20;
                    FlashOr = !FlashOr;
                }
                int y = 210;
                int x = 20;
                spriteBatch.DrawString(Font_large, "SINGLE PLAYER", new Vector2(x, y), Color.White);
                y += 50;
                spriteBatch.DrawString(Font_large, "Play !", new Vector2(x, y), Color.White);
                y += 40;
                spriteBatch.DrawString(Font_large, "AI difficulty:", new Vector2(x, y), Color.White);
                spriteBatch.DrawString(Font_large, Difficults[difficultIndex], new Vector2(x + 220, y), Color.White);
                y += 40;
                spriteBatch.DrawString(Font_large, "Your Name:", new Vector2(x, y), Color.White);
                spriteBatch.DrawString(Font_large, PlayerName, new Vector2(x + 220, y), Color.White);
                if (FlashOr)
                    spriteBatch.DrawString(Font_large, "_", new Vector2(x + 220 + Font_large.MeasureString(PlayerName).X + 2,
                        y), Color.White);
                y += 40;
                spriteBatch.DrawString(Font_large, "Main Menu", new Vector2(x, y), Color.White);
            }
        }
    }
}
