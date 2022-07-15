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
using AHD.SharpTrix.Core;

namespace AHD.SharpTrix
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class rUserNaming : Microsoft.Xna.Framework.GameComponent
    {
        public rUserNaming(Game game, rGamePlay gameplay)
            : base(game)
        {
            tFrame = base.Game.Content.Load<Texture2D>(@"Backgrounds\frame");
            Font_large = base.Game.Content.Load<SpriteFont>(@"Fonts\FontLarge");
            Font_normal = base.Game.Content.Load<SpriteFont>(@"Fonts\FontNormal");
            this.gameplay = gameplay;
        }
        SpriteFont Font_large;
        SpriteFont Font_normal;
        Texture2D tFrame;
        int MenuIndex = 0;
        bool Pressed = false;
        int countdown = 0;
        bool FirstOpen = true;
        rGamePlay gameplay;

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
            #region Mouse
            MouseState ms = Mouse.GetState();
            if (ms.X > 200 & ms.X < 400)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (ms.Y >= 190 + (40 * i) & ms.Y < 190 + (40 * (i + 1)))
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
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) | ((ms.LeftButton == ButtonState.Pressed)
                    & (ms.X > 200 & ms.X < 400)))
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
            base.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw frame
            spriteBatch.Draw(tFrame,
            new Rectangle(170, 120, 350, 150 + (40 * 4)), Color.White);
            int y = 150;
            int x = 230;
            spriteBatch.DrawString(Font_normal, "Choose your game :", new Vector2(x, y), Color.Black);
            y += 40;
            spriteBatch.DrawString(Font_normal, "Queens", new Vector2(x, y), gameplay.trixBartyiah.IsModePlayed(PlayMode.Queens) ? Color.Gray : (MenuIndex == 0 ? Color.Blue : Color.Black));
            y += 40;
            spriteBatch.DrawString(Font_normal, "Ltoosh", new Vector2(x, y), gameplay.trixBartyiah.IsModePlayed(PlayMode.Ltoosh) ? Color.Gray : (MenuIndex == 1 ? Color.Blue : Color.Black));
            y += 40;
            spriteBatch.DrawString(Font_normal, "King Of Hearts", new Vector2(x, y), gameplay.trixBartyiah.IsModePlayed(PlayMode.KingOfHearts) ? Color.Gray : (MenuIndex == 2 ? Color.Blue : Color.Black));
            y += 40;
            spriteBatch.DrawString(Font_normal, "Diamonds", new Vector2(x, y), gameplay.trixBartyiah.IsModePlayed(PlayMode.Diamonds) ? Color.Gray : (MenuIndex == 3 ? Color.Blue : Color.Black));
            y += 40;
            spriteBatch.DrawString(Font_normal, "Trix", new Vector2(x, y), gameplay.trixBartyiah.IsModePlayed(PlayMode.Trix) ? Color.Gray : (MenuIndex == 4 ? Color.Blue : Color.Black));
        }
        void DoAction()
        {
            switch (MenuIndex)
            {
                case 0://Queens
                    //set the play mode in the bartyiah
                    if (!gameplay.trixBartyiah.IsModePlayed(PlayMode.Queens))
                    {
                        gameplay.trixBartyiah.PlayMode = PlayMode.Queens;
                        gameplay.trixBartyiah.playedModes.Add(PlayMode.Queens);
                        gameplay.trixBartyiah.StatusLabel = gameplay.trixBartyiah.Player1.Name + @" Has chosen """ + PlayMode.Queens.ToString() + " !!";
                        //move to doubling !!
                        gameplay.trixBartyiah.PlayStatus = PlayStatus.Doubling;
                        gameplay.trixBartyiah.HOLD = false;
                        gameplay.trixBartyiah.DrawStatus = DrawStatus.StatusLabel;
                        gameplay.trixBartyiah.StatusLabel = @"Doubling ""Queens""";
                    }
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x"));
                    break;
                case 1://Ltouch
                    //set the play mode in the bartyiah
                    if (!gameplay.trixBartyiah.IsModePlayed(PlayMode.Ltoosh))
                    {
                        gameplay.trixBartyiah.PlayMode = PlayMode.Ltoosh;
                        gameplay.trixBartyiah.playedModes.Add(PlayMode.Ltoosh);
                        gameplay.trixBartyiah.StatusLabel = gameplay.trixBartyiah.Player1.Name + @" Has chosen """ + PlayMode.Ltoosh.ToString() + " !!";
                        //resume the game
                        gameplay.trixBartyiah.PlayStatus = PlayStatus.PlayingMode;
                        gameplay.trixBartyiah.HOLD = false;
                        gameplay.trixBartyiah.DrawStatus = DrawStatus.EatTable;
                    }
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x"));
                    break;
                case 2://Khetyar
                    //set the play mode in the bartyiah
                    if (!gameplay.trixBartyiah.IsModePlayed(PlayMode.KingOfHearts))
                    {
                        gameplay.trixBartyiah.PlayMode = PlayMode.KingOfHearts;
                        gameplay.trixBartyiah.playedModes.Add(PlayMode.KingOfHearts);
                        gameplay.trixBartyiah.StatusLabel = gameplay.trixBartyiah.Player1.Name + @" Has chosen """ + PlayMode.KingOfHearts.ToString() + " !!";
                        //move to doubling !!
                        gameplay.trixBartyiah.PlayStatus = PlayStatus.Doubling;
                        gameplay.trixBartyiah.HOLD = false;
                        gameplay.trixBartyiah.DrawStatus = DrawStatus.StatusLabel;
                        gameplay.trixBartyiah.StatusLabel = @"Doubling ""King Of Hearts""";
                    }
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x"));
                    break;
                case 3://Dynar
                    //set the play mode in the bartyiah
                    if (!gameplay.trixBartyiah.IsModePlayed(PlayMode.Diamonds))
                    {
                        gameplay.trixBartyiah.PlayMode = PlayMode.Diamonds;
                        gameplay.trixBartyiah.playedModes.Add(PlayMode.Diamonds);
                        gameplay.trixBartyiah.StatusLabel = gameplay.trixBartyiah.Player1.Name + @" Has chosen """ + PlayMode.Diamonds.ToString() + " !!";
                        //resume the game
                        gameplay.trixBartyiah.PlayStatus = PlayStatus.PlayingMode;
                        gameplay.trixBartyiah.HOLD = false;
                        gameplay.trixBartyiah.DrawStatus = DrawStatus.EatTable;
                    }
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x"));
                    break;
                case 4://Trix
                    //set the play mode in the bartyiah
                    if (!gameplay.trixBartyiah.IsModePlayed(PlayMode.Trix))
                    {
                        gameplay.trixBartyiah.PlayMode = PlayMode.Trix;
                        gameplay.trixBartyiah.playedModes.Add(PlayMode.Trix);
                        gameplay.trixBartyiah.StatusLabel = gameplay.trixBartyiah.Player1.Name + @" Has chosen """ + PlayMode.Trix.ToString() + " !!";
                        //resume the game
                        gameplay.trixBartyiah.PlayStatus = PlayStatus.PlayingMode;
                        gameplay.trixBartyiah.HOLD = false;
                        gameplay.trixBartyiah.DrawStatus = DrawStatus.EatTable;
                    }
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x"));
                    break;
            }

        }
    }
}
