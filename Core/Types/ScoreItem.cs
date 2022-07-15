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
    public struct ScoreItem
    {
        string player1;
        string player2;
        string player3;
        string player4;
        int player1Score ;
        int player2Score ;
        int player3Score ;
        int player4Score ;
        string currentGame;
        int namingIndex;
        /// <summary>
        /// Get or set the player 1 score as string
        /// </summary>
        public string Player1
        { get { return player1; } set { player1 = value; } }
        /// <summary>
        /// Get or set the player 2 score as string
        /// </summary>
        public string Player2
        { get { return player2; } set { player2 = value; } }
        /// <summary>
        /// Get or set the player 3 score as string
        /// </summary>
        public string Player3
        { get { return player3; } set { player3 = value; } }
        /// <summary>
        /// Get or set the player 4 score as string
        /// </summary>
        public string Player4
        { get { return player4; } set { player4 = value; } }
        /// <summary>
        /// Get or set the player 1 score
        /// </summary>
        public int Player1Score
        { get { return player1Score; } set { player1Score = value; } }
        /// <summary>
        /// Get or set the player 2 score
        /// </summary>
        public int Player2Score
        { get { return player2Score; } set { player2Score = value; } }
        /// <summary>
        /// Get or set the player 3 score
        /// </summary>
        public int Player3Score
        { get { return player3Score; } set { player3Score = value; } }
        /// <summary>
        /// Get or set the player 4 score
        /// </summary>
        public int Player4Score
        { get { return player4Score; } set { player4Score = value; } }
        /// <summary>
        /// Get or set the naming player index
        /// </summary>
        public int NamingIndex
        { get { return namingIndex; } set { namingIndex = value; } }
        /// <summary>
        /// Get or set the Current GameMode name
        /// </summary>
        public string CurrentGameMode
        { get { return currentGame; } set { currentGame = value; } }
    }
}
