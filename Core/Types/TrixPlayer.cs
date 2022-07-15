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
using System.Text;

namespace AHD.SharpTrix.Core
{
    public class TrixPlayer
    {
        public TrixPlayer(string name, int score, bool isAI, PlayMode playMode,
            List<Cards> cards, List<Cards> ateCards, TrixBartyiah bartyiah, AI ai)
        {
            this.name = name;
            this.score = score;
            this.isAI = isAI;
            this.cards = cards;
            this.ateCards = ateCards;
            this.bartyiah = bartyiah;
            this.playMode = playMode;
            this.ai = ai;
            if (this.ai != null)
                this.ai.TrixCurrentPlayer = this;
            doublingCards = new List<Cards>();
        }
        string name;
        int score;
        List<Cards> cards;
        List<Cards> ateCards;
        List<Cards> doublingCards;
        bool isAI;//if true, the ai takes control of playing otherwise real player does
        TrixBartyiah bartyiah;
        PlayMode playMode;//the chosen play mode
        AI ai;
        Cards playedCard;

        /// <summary>
        /// Get or set the name of this player
        /// </summary>
        public string Name
        { get { return name; } set { name = value; } }
        /// <summary>
        /// Get or set the score of this player
        /// </summary>
        public int Score
        { get { return score; } set { score = value; } }
        /// <summary>
        /// Get or set the cards collection on hand
        /// </summary>
        public List<Cards> CardsOnHand
        { get { return cards; } set { cards = value; } }
        /// <summary>
        /// Get or set the cards that ate by this player
        /// </summary>
        public List<Cards> AteCards
        { get { return ateCards; } set { ateCards = value; } }
        /// <summary>
        /// Get or set the cards that chosed as doubling cards for this player
        /// </summary>
        public List<Cards> DoublingCards
        { get { return doublingCards; } set { doublingCards = value; } }
        /// <summary>
        /// Get or set the play mode chosen by player
        /// </summary>
        public PlayMode PlayMode
        { get { return playMode; } set { playMode = value; } }
        /// <summary>
        /// Get or set the current ai
        /// </summary>
        public AI AI
        {
            get { return ai; }
            set { ai = value; }
        }
        /// <summary>
        /// Get or set a value indecates if this player is ai
        /// </summary>
        public bool IsAI
        { get { return isAI; } set { isAI = value; } }
        /// <summary>
        /// Indecate if a given card is exist in player hands
        /// </summary>
        /// <param name="card">The card you looking for</param>
        /// <returns>TRue if found, false if not</returns>
        public bool IsCardExists(Cards card)
        {
            foreach (Cards crd in cards)
                if (crd.ToString() == card.ToString())
                    return true;
            return false;
        }
        public Cards LastPlayedCard
        { get { return playedCard; } set { playedCard = value; } }

        public void Play_Dealing()
        {
            //player choice
            if (!isAI)
            {
                bartyiah.DrawStatus = DrawStatus.UserNaming;
                bartyiah.HOLD = true;
                //The reset is done by drawer
            }
            else//ai choice
            {
                ai.Play_Dealing();
            }
        }

        public void Play_Doubling()
        {
            //player choice
            if (!isAI)
            {
                bartyiah.PlayerTurn = true;
                bartyiah.HOLD = true;
                //The reset is done by drawer
            }
            else//ai choice
            {
                ai.Play_Doubling();
            }
        }

        public void Play_Dynar()
        {
            //player choice
            if (!isAI)
            {
                bartyiah.PlayerTurn = true;
                //The reset is done by drawer
            }
            else//ai choice, the player must not eat dynar
            {
                ai.Play_Dynar();
            }
        }
        public void Play_Khetyar()
        {
            //player choice
            if (!isAI)
            {
                bartyiah.PlayerTurn = true;
                //The reset is done by drawer
            }
            else//ai choice, the player must not eat dynar
            {
                ai.Play_Khetyar();
            }
        }
        public void Play_Ltouch()
        {
            //player choice
            if (!isAI)
            {
                bartyiah.PlayerTurn = true;
                //The reset is done by drawer
            }
            else//ai choice, the player must not eat ltouch
            {
                ai.Play_Ltouch();
            }

        }
        public void Play_Queens()
        {
            //player choice
            if (!isAI)
            {
                bartyiah.PlayerTurn = true;
                //The reset is done by drawer
            }
            else//ai choice, the player must not eat dynar
            {
                ai.Play_Queens();
            }
        }
        public bool Play_Trix()
        {
            //player choice
            if (!isAI)
            {
                bartyiah.PlayerTurn = true;
                return false;
                //The reset is done by drawer
            }
            else//ai choice, the player should play by trix rules, if no card found to play, pass ...
            {
                return ai.Play_Trix();
            }
        }
    }
}