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
    /// <summary>
    /// Arguments for ate events
    /// </summary>
    public class EatArgs : EventArgs
    {
        Cards[] ateCards;
        string playerName = "";
        public EatArgs(Cards[] ateCards, string playerName)
        {
            this.ateCards = ateCards;
            this.playerName = playerName;
        }
        /// <summary>
        /// Get the cards that ate at this dak
        /// </summary>
        public Cards[] AteCards
        {
            get { return ateCards; }
        }
        /// <summary>
        /// Get the name of the player which ate this cards
        /// </summary>
        public string PlayerName
        {
            get { return playerName; }
        }
    }
}
