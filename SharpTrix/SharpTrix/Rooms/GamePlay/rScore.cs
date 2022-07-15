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
    public class rScore : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_large;
        SpriteFont Font_normal;
        SpriteFont Font_small;
        Texture2D tFrame;
        Texture2D hand_l;
        Texture2D hand_r;
        rGamePlay gameplay;
        bool Pressed = false;
        int countdown = 0;

        int paperWidth = 450;
        int paperHeight = 400;
        public bool ShowTotalScore = false;

        public rScore(Game game, rGamePlay gameplay)
            : base(game)
        {
            tFrame = base.Game.Content.Load<Texture2D>(@"Backgrounds\frame");
            Font_large = base.Game.Content.Load<SpriteFont>(@"Fonts\FontLarge");
            Font_normal = base.Game.Content.Load<SpriteFont>(@"Fonts\FontNormal");
            Font_small = base.Game.Content.Load<SpriteFont>(@"Fonts\FontSmall");
            hand_l = base.Game.Content.Load<Texture2D>(@"Items\PointHand_Left");
            hand_r = base.Game.Content.Load<Texture2D>(@"Items\PointHand_Right");
            this.gameplay = gameplay;
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
            if (!Pressed)
            {
                countdown = 10;
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x"));
                    gameplay.trixBartyiah.HOLD = false;
                    gameplay.trixBartyiah.DrawStatus = Core.DrawStatus.StatusLabel;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\CardDrop"));
                    ShowTotalScore = false;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\CardDrop"));
                    ShowTotalScore = true;
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
            spriteBatch.DrawString(Font_large, "End Of Game Mode, Scores :", new Vector2((base.Game.GraphicsDevice.Viewport.Width / 2) -
            (Font_large.MeasureString("End Of Game Mode, Scores :").X / 2), 50), Color.White);
            spriteBatch.DrawString(Font_large, "Press Space To Continue",
            new Vector2((base.Game.GraphicsDevice.Viewport.Width / 2) -
            (Font_large.MeasureString("Press Space To Continue").X / 2),
            base.Game.GraphicsDevice.Viewport.Height - 70), Color.White);

            int y =   (base.Game.GraphicsDevice.Viewport.Height / 2) - (paperHeight / 2);
            int x = (base.Game.GraphicsDevice.Viewport.Width / 2) - (paperWidth / 2);

            spriteBatch.Draw(tFrame,new Rectangle(x, y, paperWidth, paperHeight), Color.White);
            if (!ShowTotalScore)
            {
                spriteBatch.Draw(hand_r, new Rectangle(base.Game.GraphicsDevice.Viewport.Width - 170
                    , (base.Game.GraphicsDevice.Viewport.Height / 2) - (hand_r.Height / 2), 40, 40), Color.White);
            }
            else
            {
                spriteBatch.Draw(hand_l, new Rectangle(140
                    , (base.Game.GraphicsDevice.Viewport.Height / 2) - (hand_l.Height / 2), 40, 40), Color.White);
            }
            if (!ShowTotalScore)//Show current mode score
            {
                int ssy = y + 20;
                int ssx = x + 10;
                int sx = x + 130;
                spriteBatch.DrawString(Font_normal, "Current Mode ("+gameplay.trixBartyiah.PlayModeScore.CurrentGameMode+
                    ") Scores:", new Vector2(ssx, ssy), Color.DarkBlue);
                ssy += 50;
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.Player1.Name + ":", new Vector2(ssx, ssy), Color.DarkBlue);
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.PlayModeScore.Player1, new Vector2(sx, ssy), Color.DarkBlue);
                ssy += 30;
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.Player2.Name + ":", new Vector2(ssx, ssy), Color.DarkBlue);
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.PlayModeScore.Player2, new Vector2(sx, ssy), Color.DarkBlue);
                ssy += 30;
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.Player3.Name + ":", new Vector2(ssx, ssy), Color.DarkBlue);
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.PlayModeScore.Player3, new Vector2(sx, ssy), Color.DarkBlue);
                ssy += 30;
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.Player4.Name + ":", new Vector2(ssx, ssy), Color.DarkBlue);
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.PlayModeScore.Player4, new Vector2(sx, ssy), Color.DarkBlue);
            }
            else
            {
                int ssy = y + 20;
                int ssx = x + 10;
                int sx = x + 130;
                int pluse = 70;
                spriteBatch.DrawString(Font_normal, "Total Scores: ", new Vector2(ssx, ssy), Color.DarkBlue);
                ssy += 25;
                //Player names
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.Player1.Name, new Vector2(ssx, ssy), Color.DarkBlue);
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.Player2.Name, new Vector2(ssx + (pluse * 1), ssy), Color.DarkBlue);
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.Player3.Name, new Vector2(ssx + (pluse * 2), ssy), Color.DarkBlue);
                spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.Player4.Name, new Vector2(ssx + (pluse * 3), ssy), Color.DarkBlue);
                spriteBatch.DrawString(Font_small, "Play Mode", new Vector2(ssx + (pluse * 4), ssy), Color.DarkBlue);
                for (int i = 0; i < gameplay.trixBartyiah.TotalScores.Count; i++)
                {
                    ssy += 15;
                    spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.TotalScores[i].Player1Score.ToString(),
                        new Vector2(ssx, ssy), (gameplay.trixBartyiah.TotalScores[i].NamingIndex == 0) ? Color.Red : Color.DarkBlue);
                    spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.TotalScores[i].Player2Score.ToString(),
                        new Vector2(ssx + (pluse * 1), ssy), (gameplay.trixBartyiah.TotalScores[i].NamingIndex == 1) ? Color.Red : Color.DarkBlue);
                    spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.TotalScores[i].Player3Score.ToString(),
                        new Vector2(ssx + (pluse * 2), ssy), (gameplay.trixBartyiah.TotalScores[i].NamingIndex == 2) ? Color.Red : Color.DarkBlue);
                    spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.TotalScores[i].Player4Score.ToString(),
                        new Vector2(ssx + (pluse * 3), ssy), (gameplay.trixBartyiah.TotalScores[i].NamingIndex == 3) ? Color.Red : Color.DarkBlue);
                    spriteBatch.DrawString(Font_small, gameplay.trixBartyiah.TotalScores[i].CurrentGameMode,
                        new Vector2(ssx + (pluse * 4), ssy), Color.Red);
                }
            }
        }
    }
}
