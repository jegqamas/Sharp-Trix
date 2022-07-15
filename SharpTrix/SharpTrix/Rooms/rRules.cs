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
    public class rRules : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_small;
        SpriteFont Font_large;
        SpriteFont Font_normal;
        Texture2D tBackground;
        Texture2D hand_l;
        Texture2D hand_r;
        SoundEffect seClick;

        int paperWidth = 550;
        int paperHeight = 420;
        Texture2D tFrame;
        int pageIndex = 0;
        bool Pressed = false;
        int countdown = 0;

        public rRules(Game game)
            : base(game)
        {
            tFrame = base.Game.Content.Load<Texture2D>(@"Backgrounds\frame");
            Font_normal = base.Game.Content.Load<SpriteFont>(@"Fonts\FontNormal");
            Font_small = base.Game.Content.Load<SpriteFont>(@"Fonts\FontSmall");
            Font_large = base.Game.Content.Load<SpriteFont>(@"Fonts\FontLarge");
            tBackground = base.Game.Content.Load<Texture2D>(@"Backgrounds\GreenTableBackground");
            seClick = base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x");
            hand_l = base.Game.Content.Load<Texture2D>(@"Items\PointHand_Left");
            hand_r = base.Game.Content.Load<Texture2D>(@"Items\PointHand_Right");
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
            //Action
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x"));
                ((TrixCore)base.Game).Room = CurrentRoom.MainMenu;
            }
            if (!Pressed)
            {
                countdown = 10;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\CardDrop"));
                    pageIndex--;
                    if (pageIndex < 0)
                        pageIndex = 0;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\CardDrop"));
                    pageIndex++;
                    if (pageIndex > 6)
                        pageIndex = 6;
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

        public void Draw(SpriteBatch spriteBatch)
        {

            //draw background
            spriteBatch.Draw(tBackground,
            new Rectangle(0, 0, base.Game.GraphicsDevice.Viewport.Width,
            base.Game.GraphicsDevice.Viewport.Height), Color.White);

            spriteBatch.DrawString(Font_small, "version " + Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                          new Vector2(base.Game.GraphicsDevice.Viewport.Width - Font_small.MeasureString("version " +
                              Assembly.GetExecutingAssembly().GetName().Version.ToString()).X - 5, base.Game.GraphicsDevice.Viewport.Height - 15), Color.White);


            spriteBatch.DrawString(Font_large, "TRIX RULES", new Vector2((base.Game.GraphicsDevice.Viewport.Width / 2) -
(Font_large.MeasureString("TRIX RULES").X / 2), 50), Color.White);
            spriteBatch.DrawString(Font_large, "Press Escape To Back To Main Menu",
                new Vector2((base.Game.GraphicsDevice.Viewport.Width / 2) -
                    (Font_large.MeasureString("Press Escape To Back To Main Menu").X / 2),
                    base.Game.GraphicsDevice.Viewport.Height - 70), Color.White);



            int y = (base.Game.GraphicsDevice.Viewport.Height / 2) - (paperHeight / 2);
            int x = (base.Game.GraphicsDevice.Viewport.Width / 2) - (paperWidth / 2);

            if (pageIndex < 6)
            {
                spriteBatch.Draw(hand_r, new Rectangle(x + paperWidth + 30
                    , (base.Game.GraphicsDevice.Viewport.Height / 2) - (hand_r.Height / 2), 40, 40), Color.White);
            }
            if (pageIndex > 0)
            {
                spriteBatch.Draw(hand_l, new Rectangle(x - 60
                    , (base.Game.GraphicsDevice.Viewport.Height / 2) - (hand_l.Height / 2), 40, 40), Color.White);
            }

            spriteBatch.Draw(tFrame, new Rectangle(x, y, paperWidth, paperHeight), Color.White);

            int ssy = y + 35;
            int ssx = x + 93;
            int sby = y + paperHeight - 30;
            int sbx = (base.Game.GraphicsDevice.Viewport.Width / 2) -
                ((int)Font_small.MeasureString("9").X / 2);
            spriteBatch.DrawString(Font_small, (pageIndex + 1).ToString(), new Vector2(sbx, sby), Color.DarkBlue);
            switch (pageIndex)
            {
                case 0:
                    spriteBatch.DrawString(Font_normal, "General Rules for Trix", new Vector2(ssx, ssy), Color.DarkBlue);
                    ssy += 30;
                    spriteBatch.DrawString(Font_small, " Trix is a card game that is played with a total of four \nplayers."
                        + " It's played with a complete standard deck of 52 cards \nand no jokers."
                        + " There are no partners in Trix, each player is on \ntheir own."
                        + "\nThere are a total of five sub-games in Trix (see Sub-games \nsection)."
                        + "The sub-game that is to be played is chosen by the \ncurrent dealer after the hand is dealt."
                        + "The first person \nto be the dealer is the person who has the Seven of Hearts \nwhen the first hand is dealt."
                        + " This person will choose when to\nplay each of the five sub-games during the next five hands."
                        + "\nAfter that, the person is no longer the dealer and the player \non their right is now the dealer."
                        + "\nThe game ends when all four players have had their turn as \ndealers and a total of twenty sub-games "
                        + "have been played. \nAll players start with a score of zero. The player with the \nhighest score at the "
                        + "end of the game is the winner.",
                        new Vector2(ssx, ssy), Color.DarkBlue);
                    break;
                case 1:
                    spriteBatch.DrawString(Font_normal, "Playing a Hand", new Vector2(ssx, ssy), Color.DarkBlue);
                    ssy += 30;
                    spriteBatch.DrawString(Font_small, " Upon choosing the sub-game to be played, the dealer is the first \nperson to play a card in the hand."
                        + " The turn then goes to the \nperson directly to the right until all four players have played \na card."
                        + " All players must play cards of the same suit as the first \ncard of the hand. If a player does not have any cards of the same "
                        + "\nsuit as the first card, then they are allowed to play any card of \na different suit."
                        + " The player that plays the highest card of the \nsame suit as the first card of the hand will have 'eaten' the \nhand and all cards in the hand."
                        + " The player that 'eats' the hand \nwill be the person that plays the first card of the next hand."
                        + "\nThe players continue in this way until all the cards have been \nplayed or until the sub-game is over."
                        + "\nThese hand rules apply to all sub-games except Trix (see Trix \nSub-game section)."
                        , new Vector2(ssx, ssy), Color.DarkBlue);
                    break;
                case 2:
                    spriteBatch.DrawString(Font_normal, "Sub-games", new Vector2(ssx, ssy), Color.DarkBlue);
                    ssy += 30;
                    spriteBatch.DrawString(Font_small, " After the cards are dealt, the current dealer must choose the \nsub-game that is to be played."
                        + " Initially, the dealer can choose \nANY of the five sub-games. Once a sub-game has been played, \nthe dealer cannot choose that sub-game again."
                        , new Vector2(ssx, ssy), Color.DarkBlue);
                    ssy += 80;
                    spriteBatch.DrawString(Font_normal, "Below are the sub-games available in \nTrix and the rules of each one:", new Vector2(ssx, ssy), Color.DarkBlue);
                    ssy += 67;
                    spriteBatch.DrawString(Font_small, "1 King of Hearts:\n\n"
                        + "The point of this sub-game is NOT to 'eat' the King of Hearts."
                        + "\nThe player who 'eats' the King of Hearts loses 75 points from \ntheir score."
                        + "\n\n2 Ltoosh:\n\n"
                        + "The point of this sub-game is NOT to 'eat' any hands at all."
                        + "\nEach hand that is 'eaten' will cause the player to lose 15 points \nfrom their score."
                        , new Vector2(ssx, ssy), Color.DarkBlue);
                    break;
                case 3:
                    spriteBatch.DrawString(Font_small, "3 Queens:\n\n"
                     + "The point of this sub-game is NOT to 'eat' any Queen."
                     + "\nEach Queen that is 'eaten' will cause the player to lose 25 \npoints from their score."
                     + "\nWhenever a Queen is 'eaten', it is kept face up in front of the \nplayer that 'ate' it for all players to see."
                     + "\n\n4 Diamonds:\n\n"
                     + "The point of this sub-game is NOT to 'eat' any card that is of \nthe Diamond suit."
                     + "\nEach card of the Diamond suit that is 'eaten' will cause the \nplayer to lost 10 points from their score."
                     + "\n\n5 Trix:\n\n"
                     + "Trix is different from all the other sub-games in that there are \nno hands, and no one 'eats' anything."
                     + "\nLike all the other sub-games, the dealer is the first person to \nstart the sub-game of Trix."
                     , new Vector2(ssx, ssy), Color.DarkBlue);
                    break;
                case 4:
                    spriteBatch.DrawString(Font_small, "The first card to be played must be the Jack of any suit."
                     + "\nThe turn then goes to the player directly to the dealer's right, \nand continues anti-clockwise."
                     + "\nEach player has the option to play only one of their cards. "
                     + "\nPlayers are only allowed to play a card that is of the same suit \nand in direct sequence to a card that is on the table. "
                     + "\nFor example, if the Jack of Hearts is on the table then a player \ncan either play the Queen of Hearts, the Ten of Hearts or a Jack \nof any other suit."
                     + "\nThe card stacks will be built from all four suits, starting with \nthe Jack going up to the Ace and down to the Two."
                     + "\nIf a player does not have any cards to play, then they must pass \nand the turn goes to the next player. A player is not allowed to \npass if they have a card to play. "
                     + "\nThe first person to play all their cards earns 200 points. The \nnext player to play all their cards earns 150 points, then the \nnext player earns 100 points and the last player to play all \ntheir cards earns 50 points."
                     , new Vector2(ssx, ssy), Color.DarkBlue);
                    break;
                case 5:
                    spriteBatch.DrawString(Font_normal, "DOUBLING", new Vector2(ssx, ssy), Color.DarkBlue);
                    ssy += 30;
                    spriteBatch.DrawString(Font_small, " Doubling is allowed in two of the five sub-games, King of Hearts \nand Queens."
                        + "\nDoubling can only occur before the sub-game has started."
                        + "\nIn the King of Hearts sub-game, the player who has the King of \nHearts has to option to show it to the rest of the players in \norder to Double it."
                        + "\nDoubling the King of Hearts means that the player who 'eats' \nthe King of Hearts has a double loss of 150 points, and \nthe player who Doubled the King of Hearts earns 75 points."
                        + "\nIn the Queens sub-game, a player who has any of the four Queens \nhas the option to show any of them to the rest of the players in \norder to Double it."
                        + "\nDoubling a Queen means that the player who 'eats' the Doubled \nQueen has a double loss of 50 points, and the player who Doubled \nthe Queen earns 25 points."
                        + "\nIn the case where the player who Doubled the card is the same \nplayer who 'eats' the Doubled card, then the player who played \nthe first card of the hand earns the positive points."
                         , new Vector2(ssx, ssy), Color.DarkBlue);
                    break;
                case 6:
                    spriteBatch.DrawString(Font_normal, "About", new Vector2(ssx, ssy), Color.DarkBlue);
                    ssy += 70;
                    spriteBatch.DrawString(Font_small, "This document is based on the one at :\n\nhttp://www.jawaker.com/en/rules?game=trix",
                        new Vector2(ssx, ssy), Color.DarkBlue);
                    break;
            }
        }
    }
}
