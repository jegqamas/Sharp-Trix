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
    public class TrixBartyiah
    {
        TrixPlayer player1;//YOU
        TrixPlayer player2;//FOE 1
        TrixPlayer player3;//YOUR Partener
        TrixPlayer player4;//FOE 2
        //play
        PlayMode currentPlayMode;//بعد التسماية
        public List<PlayMode> playedModes;//الألعاب اللي نلعبت من المسمي الحالي
        List<Cards> cards;//الأوراق اللي مو مع حدي و اللي لازم تنفت
        List<Cards> onTableCards;//الأوراق اللي عالطاولة
        List<Cards> onTableCards_TrixG_h;
        List<Cards> onTableCards_TrixG_s;
        List<Cards> onTableCards_TrixG_d;
        List<Cards> onTableCards_TrixG_c;
        public List<int> TrixFinishList;
        PlayStatus status = PlayStatus.Distributing;
        int timer = 0;
        public bool HOLD = false;//hold the game to make the player take a choice
        public int FREEZTime = 0;//the time that should freez the game, in frames
        public DrawStatus DrawStatus = DrawStatus.StatusLabel;

        public int playerIndex = 0;//which player has to play now
        public int DakPlayIndex = 0;//which player started this dak
        string statusLabel = "";//this should be paited to the user during playing time
        int NamingIndex = 0;//Which player has to name now ? max = 3
        public int NamingCount = 0;//How many namings have we, max = 5
        public bool PlayerTurn = false;
        bool EndGame = false;
        public bool GAMEOVER = false; 
        bool DoublingPlayed = false;
        //SCORE
        ScoreItem currentModeScore;
        List<ScoreItem> totalScores = new List<ScoreItem>();
        public List<TrixPlayer> GameOverWinningArrange;
  

        void ClearTable()
        {
            TrixFinishList = new List<int>();
            player1.AteCards = new List<Cards>();
            player2.AteCards = new List<Cards>();
            player3.AteCards = new List<Cards>();
            player4.AteCards = new List<Cards>();
            player1.DoublingCards = new List<Cards>();
            player2.DoublingCards = new List<Cards>();
            player3.DoublingCards = new List<Cards>();
            player4.DoublingCards = new List<Cards>();
            onTableCards = new List<Core.Cards>();
            onTableCards_TrixG_h = new List<Cards>();
            onTableCards_TrixG_d = new List<Cards>();
            onTableCards_TrixG_c = new List<Cards>();
            onTableCards_TrixG_s = new List<Cards>();
        }
        void EndGameMode()
        {
            status = Core.PlayStatus.Distributing;
            DrawStatus = DrawStatus.EndGameMode;
            statusLabel = "End of game mode";
            HOLD = true;
            timer = -1;
            playerIndex = 0;
            CardChecker.FillUpCollection(cards);

            CalculateScore();
            ClearTable();

            PlayerTurn = false;
            EndGame = false;
            player1.CardsOnHand = new List<Cards>();
            player2.CardsOnHand = new List<Cards>();
            player3.CardsOnHand = new List<Cards>();
            player4.CardsOnHand = new List<Cards>();

            if (ClearEffectRequest != null)
                ClearEffectRequest(this, new EventArgs());
            if (GameModeEnded != null)
                GameModeEnded(this, new EventArgs());
        }
        void CalculateScore()
        {
            currentModeScore = new ScoreItem();
            currentModeScore.NamingIndex = NamingIndex;
            bool IsDoublingPlayerAteTheCard = false;
            switch (currentPlayMode)
            {
                #region Diamonds
                case Core.PlayMode.Diamonds:
                    currentModeScore.CurrentGameMode = "Diamonds";
                    //Player 1
                    int score = player1.Score - (CardChecker.GetCardCountByType("d", player1.AteCards) * 10);
                    currentModeScore.Player1 = player1.Score + " - (" + CardChecker.GetCardCountByType("d", player1.AteCards) +
                        " x 10) = " + score;
                    currentModeScore.Player1Score = player1.Score = score;
                    //player 2
                    score = player2.Score - (CardChecker.GetCardCountByType("d", player2.AteCards) * 10);
                    currentModeScore.Player2 = player2.Score + " - (" + CardChecker.GetCardCountByType("d", player2.AteCards) +
                        " x 10) = " + score;
                    currentModeScore.Player2Score = player2.Score = score;
                    //player 3
                    score = player3.Score - (CardChecker.GetCardCountByType("d", player3.AteCards) * 10);
                    currentModeScore.Player3 = player3.Score + " - (" + CardChecker.GetCardCountByType("d", player3.AteCards) +
                        " x 10) = " + score;
                    currentModeScore.Player3Score = player3.Score = score;
                    //player 4
                    score = player4.Score - (CardChecker.GetCardCountByType("d", player4.AteCards) * 10);
                    currentModeScore.Player4 = player4.Score + " - (" + CardChecker.GetCardCountByType("d", player4.AteCards) +
                        " x 10) = " + score;
                    currentModeScore.Player4Score = player4.Score = score;
                    break;
                #endregion
                #region KingOfHearts
                case Core.PlayMode.KingOfHearts:
                    currentModeScore.CurrentGameMode = "King Of Hearts";
                    #region Player 1
                    //is this player is the one who doubled this card
                    if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player1.DoublingCards))
                    {
                        //the player didn't eat the card and someone else done, pluse 75 to the score
                        if (!CardChecker.IsCardExistInCollection(Core.Cards.h_k, player1.AteCards))
                        {
                            score = player1.Score + 75;
                            currentModeScore.Player1 = player1.Score + " + 75 (Doubling Won !!) = " + score;
                            currentModeScore.Player1Score = player1.Score = score;
                        }
                        else//The player doubled the card then ate it, 
                        //then the player who played the first card of the hand earns the positive points.
                        {
                            IsDoublingPlayerAteTheCard = true;
                            score = player1.Score - 150;
                            currentModeScore.Player1 = player1.Score + " - 150 (Doubling Lose !!) = " + score;
                            currentModeScore.Player1Score = player1.Score = score;
                        }
                    }
                    //is someone double this card ?
                    else if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player2.DoublingCards) |
                             CardChecker.IsCardExistInCollection(Core.Cards.h_k, player3.DoublingCards) |
                             CardChecker.IsCardExistInCollection(Core.Cards.h_k, player4.DoublingCards))
                    {
                        //the player ate this card
                        if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player1.AteCards))
                        {
                            score = player1.Score - 150;
                            currentModeScore.Player1 = player1.Score + " - 150 (Doubling Lose !!) = " + score;
                            currentModeScore.Player1Score = player1.Score = score;
                        }
                        else//not him
                        {
                            //Player 1
                            score = player1.Score;
                            currentModeScore.Player1 = player1.Score + " - (0 x 75) = " + score;
                            currentModeScore.Player1Score = player1.Score = score;
                        }
                    }
                    else//normal calculate
                    {
                        //Player 1
                        score = player1.Score - (CardChecker.GetCardCount("k", "h", player1.AteCards) * 75);
                        currentModeScore.Player1 = player1.Score + " - (" + CardChecker.GetCardCount("k", "h", player1.AteCards) +
                            " x 75) = " + score;
                        currentModeScore.Player1Score = player1.Score = score;
                    }
                    #endregion
                    #region Player 2
                    //is this player is the one who doubled this card
                    if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player2.DoublingCards))
                    {
                        //the player didn't eat the card and someone else done, pluse 75 to the score
                        if (!CardChecker.IsCardExistInCollection(Core.Cards.h_k, player2.AteCards))
                        {
                            score = player2.Score + 75;
                            currentModeScore.Player2 = player2.Score + " + 75 (Doubling Won !!) = " + score;
                            currentModeScore.Player2Score = player2.Score = score;
                        }
                        else//The player doubled the card then ate it, 
                        //then the player who played the first card of the hand earns the positive points.
                        {
                            IsDoublingPlayerAteTheCard = true;
                            score = player2.Score - 150;
                            currentModeScore.Player2 = player2.Score + " - 150 (Doubling Lose !!) = " + score;
                            currentModeScore.Player2Score = player2.Score = score;
                        }
                    }
                    //is someone double this card ?
                    else if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player1.DoublingCards) |
                             CardChecker.IsCardExistInCollection(Core.Cards.h_k, player3.DoublingCards) |
                             CardChecker.IsCardExistInCollection(Core.Cards.h_k, player4.DoublingCards))
                    {
                        //the player ate this card
                        if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player2.AteCards))
                        {
                            score = player2.Score - 150;
                            currentModeScore.Player2 = player2.Score + " - 150 (Doubling Lose !!) = " + score;
                            currentModeScore.Player2Score = player2.Score = score;
                        }
                        else//not him
                        {
                            score = player2.Score;
                            currentModeScore.Player2 = player2.Score + " - (0 x 75) = " + score;
                            currentModeScore.Player2Score = player2.Score = score;
                        }
                    }
                    else//normal calculate
                    {
                        score = player2.Score - (CardChecker.GetCardCount("k", "h", player2.AteCards) * 75);
                        currentModeScore.Player2 = player2.Score + " - (" + CardChecker.GetCardCount("k", "h", player2.AteCards) +
                            " x 75) = " + score;
                        currentModeScore.Player2Score = player2.Score = score;
                    }
                    #endregion
                    #region Player 3
                    //is this player is the one who doubled this card
                    if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player3.DoublingCards))
                    {
                        //the player didn't eat the card and someone else done, pluse 75 to the score
                        if (!CardChecker.IsCardExistInCollection(Core.Cards.h_k, player3.AteCards))
                        {
                            score = player3.Score + 75;
                            currentModeScore.Player3 = player3.Score + " + 75 (Doubling Won !!) = " + score;
                            currentModeScore.Player3Score = player3.Score = score;
                        }
                        else//The player doubled the card then ate it, 
                        //then the player who played the first card of the hand earns the positive points.
                        {
                            IsDoublingPlayerAteTheCard = true;
                            score = player3.Score - 150;
                            currentModeScore.Player3 = player3.Score + " - 150 (Doubling Lose !!) = " + score;
                            currentModeScore.Player3Score = player3.Score = score;
                        }
                    }
                    //is someone double this card ?
                    else if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player1.DoublingCards) |
                             CardChecker.IsCardExistInCollection(Core.Cards.h_k, player2.DoublingCards) |
                             CardChecker.IsCardExistInCollection(Core.Cards.h_k, player4.DoublingCards))
                    {
                        //the player ate this card
                        if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player3.AteCards))
                        {
                            score = player3.Score - 150;
                            currentModeScore.Player3 = player3.Score + " - 150 (Doubling Lose !!) = " + score;
                            currentModeScore.Player3Score = player3.Score = score;
                        }
                        else//not him
                        {
                            score = player3.Score;
                            currentModeScore.Player3 = player3.Score + " - (0 x 75) = " + score;
                            currentModeScore.Player3Score = player3.Score = score;
                        }
                    }
                    else//normal calculate
                    {
                        score = player3.Score - (CardChecker.GetCardCount("k", "h", player3.AteCards) * 75);
                        currentModeScore.Player3 = player3.Score + " - (" + CardChecker.GetCardCount("k", "h", player3.AteCards) +
                            " x 75) = " + score;
                        currentModeScore.Player3Score = player3.Score = score;
                    }
                    #endregion
                    #region Player 4
                    //is this player is the one who doubled this card
                    if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player4.DoublingCards))
                    {
                        //the player didn't eat the card and someone else done, pluse 75 to the score
                        if (!CardChecker.IsCardExistInCollection(Core.Cards.h_k, player4.AteCards))
                        {
                            score = player4.Score + 75;
                            currentModeScore.Player4 = player4.Score + " + 75 (Doubling Won !!) = " + score;
                            currentModeScore.Player4Score = player4.Score = score;
                        }
                        else//The player doubled the card then ate it, 
                        //then the player who played the first card of the hand earns the positive points.
                        {
                            IsDoublingPlayerAteTheCard = true;
                            score = player4.Score - 150;
                            currentModeScore.Player4 = player4.Score + " - 150 (Doubling Lose !!) = " + score;
                            currentModeScore.Player4Score = player4.Score = score;
                        }
                    }
                    //is someone double this card ?
                    else if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player1.DoublingCards) |
                             CardChecker.IsCardExistInCollection(Core.Cards.h_k, player2.DoublingCards) |
                             CardChecker.IsCardExistInCollection(Core.Cards.h_k, player3.DoublingCards))
                    {
                        //the player ate this card
                        if (CardChecker.IsCardExistInCollection(Core.Cards.h_k, player4.AteCards))
                        {
                            score = player4.Score - 150;
                            currentModeScore.Player4 = player4.Score + " - 150 (Doubling Lose !!) = " + score;
                            currentModeScore.Player4Score = player4.Score = score;
                        }
                        else//not him
                        {
                            score = player4.Score;
                            currentModeScore.Player4 = player4.Score + " - (0 x 75) = " + score;
                            currentModeScore.Player4Score = player4.Score = score;
                        }
                    }
                    else//normal calculate
                    {
                        score = player4.Score - (CardChecker.GetCardCount("k", "h", player4.AteCards) * 75);
                        currentModeScore.Player4 = player4.Score + " - (" + CardChecker.GetCardCount("k", "h", player4.AteCards) +
                            " x 75) = " + score;
                        currentModeScore.Player4Score = player4.Score = score;
                    }
                    #endregion
                    if (IsDoublingPlayerAteTheCard)
                    {
                        //then the player who played the first card of the hand earns the positive points.
                        switch (NamingIndex)
                        {
                            case 0:
                                score = currentModeScore.Player1Score + 75;
                                currentModeScore.Player1 += " + 75 (Doubling Bonus) = " + score;
                                currentModeScore.Player1Score = player1.Score = score;
                                break;
                            case 1:
                                score = currentModeScore.Player2Score + 75;
                                currentModeScore.Player2 += " + 75 (Doubling Bonus) = " + score;
                                currentModeScore.Player2Score = player2.Score = score;
                                break;
                            case 2:
                                score = currentModeScore.Player3Score + 75;
                                currentModeScore.Player3 += " + 75 (Doubling Bonus) = " + score;
                                currentModeScore.Player3Score = player3.Score = score;
                                break;
                            case 3:
                                score = currentModeScore.Player4Score + 75;
                                currentModeScore.Player4 += " + 75 (Doubling Bonus) = " + score;
                                currentModeScore.Player4Score = player4.Score = score;
                                break;
                        }
                    }
                    break;
                #endregion
                #region Queens
                case Core.PlayMode.Queens:
                    currentModeScore.CurrentGameMode = "Queens";
                    Core.Cards[] queens = new Cards[] { Core.Cards.c_q, Core.Cards.d_q, Core.Cards.s_q, Core.Cards.h_q };
                    int doublingBonus = 0;
                    int doublingScore = 0;
                    #region Player 1
                    score = player1.Score;
                    foreach (Core.Cards card in queens)
                    {
                        if (CardChecker.IsCardExistInCollection(card, player1.DoublingCards))
                        {
                            //the player didn't eat the card and someone else done, pluse 25 to the score
                            if (!CardChecker.IsCardExistInCollection(card, player1.AteCards))
                            {
                                doublingScore += 25;
                                score += 25;
                            }
                            else//The player doubled the card then ate it, 
                            //then the player who played the first card of the hand earns the positive points.
                            {
                                IsDoublingPlayerAteTheCard = true;
                                doublingBonus += 25;
                                doublingScore -= 50;
                                score -= 50;
                            }
                        }
                        //is someone double this card ?
                        else if (CardChecker.IsCardExistInCollection(card, player2.DoublingCards) |
                                 CardChecker.IsCardExistInCollection(card, player3.DoublingCards) |
                                 CardChecker.IsCardExistInCollection(card, player4.DoublingCards))
                        {
                            //the player ate this card
                            if (CardChecker.IsCardExistInCollection(card, player1.AteCards))
                            {
                                doublingScore -= 50;
                                score -= 50;
                            }
                            else//not him
                            {

                            }
                        }
                        else//normal calculate
                        {
                            if (CardChecker.IsCardExistInCollection(card, player1.AteCards))
                            {
                                score -= 25;
                            }
                        }
                    }
                    currentModeScore.Player1 = player1.Score + " - (" + CardChecker.GetCardCountByName("q", player1.AteCards) +
                        " x 25) with (" + doublingScore + " Doubling Score) = " + score;
                    currentModeScore.Player1Score = player1.Score = score;
                    #endregion
                    #region Player 2
                    doublingScore = 0;
                    score = player2.Score;
                    foreach (Core.Cards card in queens)
                    {
                        if (CardChecker.IsCardExistInCollection(card, player2.DoublingCards))
                        {
                            //the player didn't eat the card and someone else done, pluse 25 to the score
                            if (!CardChecker.IsCardExistInCollection(card, player2.AteCards))
                            {
                                doublingScore += 25;
                                score += 25;
                            }
                            else//The player doubled the card then ate it, 
                            //then the player who played the first card of the hand earns the positive points.
                            {
                                IsDoublingPlayerAteTheCard = true;
                                doublingBonus += 25;
                                doublingScore -= 50;
                                score -= 50;
                            }
                        }
                        //is someone double this card ?
                        else if (CardChecker.IsCardExistInCollection(card, player1.DoublingCards) |
                                 CardChecker.IsCardExistInCollection(card, player3.DoublingCards) |
                                 CardChecker.IsCardExistInCollection(card, player4.DoublingCards))
                        {
                            //the player ate this card
                            if (CardChecker.IsCardExistInCollection(card, player2.AteCards))
                            {
                                doublingScore -= 50;
                                score -= 50;
                            }
                            else//not him
                            {

                            }
                        }
                        else//normal calculate
                        {
                            if (CardChecker.IsCardExistInCollection(card, player2.AteCards))
                            {
                                score -= 25;
                            }
                        }
                    }
                    currentModeScore.Player2 = player2.Score + " - (" + CardChecker.GetCardCountByName("q", player2.AteCards) +
                        " x 25) with (" + doublingScore + " Doubling Score) = " + score;
                    currentModeScore.Player2Score = player2.Score = score;
                    #endregion
                    #region Player 3
                    doublingScore = 0;
                    score = player3.Score;
                    foreach (Core.Cards card in queens)
                    {
                        if (CardChecker.IsCardExistInCollection(card, player3.DoublingCards))
                        {
                            //the player didn't eat the card and someone else done, pluse 25 to the score
                            if (!CardChecker.IsCardExistInCollection(card, player3.AteCards))
                            {
                                doublingScore += 25;
                                score += 25;
                            }
                            else//The player doubled the card then ate it, 
                            //then the player who played the first card of the hand earns the positive points.
                            {
                                IsDoublingPlayerAteTheCard = true;
                                doublingBonus += 25;
                                doublingScore -= 50;
                                score -= 50;
                            }
                        }
                        //is someone double this card ?
                        else if (CardChecker.IsCardExistInCollection(card, player1.DoublingCards) |
                                 CardChecker.IsCardExistInCollection(card, player2.DoublingCards) |
                                 CardChecker.IsCardExistInCollection(card, player4.DoublingCards))
                        {
                            //the player ate this card
                            if (CardChecker.IsCardExistInCollection(card, player3.AteCards))
                            {
                                doublingScore -= 50;
                                score -= 50;
                            }
                            else//not him
                            {

                            }
                        }
                        else//normal calculate
                        {
                            if (CardChecker.IsCardExistInCollection(card, player3.AteCards))
                            {
                                score -= 25;
                            }
                        }
                    }
                    currentModeScore.Player3 = player3.Score + " - (" + CardChecker.GetCardCountByName("q", player3.AteCards) +
                        " x 25) with (" + doublingScore + " Doubling Score) = " + score;
                    currentModeScore.Player3Score = player3.Score = score;
                    #endregion
                    #region Player 4
                    doublingScore = 0;
                    score = player4.Score;
                    foreach (Core.Cards card in queens)
                    {
                        if (CardChecker.IsCardExistInCollection(card, player4.DoublingCards))
                        {
                            //the player didn't eat the card and someone else done, pluse 25 to the score
                            if (!CardChecker.IsCardExistInCollection(card, player4.AteCards))
                            {
                                doublingScore += 25;
                                score += 25;
                            }
                            else//The player doubled the card then ate it, 
                            //then the player who played the first card of the hand earns the positive points.
                            {
                                IsDoublingPlayerAteTheCard = true;
                                doublingBonus += 25;
                                doublingScore -= 50;
                                score -= 50;
                            }
                        }
                        //is someone double this card ?
                        else if (CardChecker.IsCardExistInCollection(card, player1.DoublingCards) |
                                 CardChecker.IsCardExistInCollection(card, player2.DoublingCards) |
                                 CardChecker.IsCardExistInCollection(card, player3.DoublingCards))
                        {
                            //the player ate this card
                            if (CardChecker.IsCardExistInCollection(card, player4.AteCards))
                            {
                                doublingScore -= 50;
                                score -= 50;
                            }
                            else//not him
                            {

                            }
                        }
                        else//normal calculate
                        {
                            if (CardChecker.IsCardExistInCollection(card, player4.AteCards))
                            {
                                score -= 25;
                            }
                        }
                    }
                    currentModeScore.Player4 = player4.Score + " - (" + CardChecker.GetCardCountByName("q", player4.AteCards) +
                        " x 25) with (" + doublingScore + " Doubling Score) = " + score;
                    currentModeScore.Player4Score = player4.Score = score;
                    #endregion
                    if (IsDoublingPlayerAteTheCard)
                    {
                        //then the player who played the first card of the hand earns the positive points.
                        switch (NamingIndex)
                        {
                            case 0:
                                score = currentModeScore.Player1Score + doublingBonus;
                                currentModeScore.Player1 += "\n +" + doublingBonus + " (Doubling Bonus) = " + score;
                                currentModeScore.Player1Score = player1.Score = score;
                                break;
                            case 1:
                                score = currentModeScore.Player2Score + doublingBonus;
                                currentModeScore.Player2 += "\n +" + doublingBonus + " (Doubling Bonus) = " + score;
                                currentModeScore.Player2Score = player2.Score = score;
                                break;
                            case 2:
                                score = currentModeScore.Player3Score + doublingBonus;
                                currentModeScore.Player3 += "\n +" + doublingBonus + " (Doubling Bonus) = " + score;
                                currentModeScore.Player3Score = player3.Score = score;
                                break;
                            case 3:
                                score = currentModeScore.Player4Score + doublingBonus;
                                currentModeScore.Player4 += "\n +" + doublingBonus + " (Doubling Bonus) = " + score;
                                currentModeScore.Player4Score = player4.Score = score;
                                break;
                        }
                    }
                    break;
                #endregion
                #region Ltoosh
                case Core.PlayMode.Ltoosh:
                    currentModeScore.CurrentGameMode = "Ltoosh";
                    //Player 1
                    score = player1.Score - ((player1.AteCards.Count / 4) * 15);
                    currentModeScore.Player1 = player1.Score + " - (" + (player1.AteCards.Count / 4) +
                        " x 15) = " + score;
                    currentModeScore.Player1Score = player1.Score = score;
                    //player 2
                    score = player2.Score - ((player2.AteCards.Count / 4) * 15);
                    currentModeScore.Player2 = player2.Score + " - (" + (player2.AteCards.Count / 4) +
                        " x 15) = " + score;
                    currentModeScore.Player2Score = player2.Score = score;
                    //player 3
                    score = player3.Score - ((player3.AteCards.Count / 4) * 15);
                    currentModeScore.Player3 = player3.Score + " - (" + (player3.AteCards.Count / 4) +
                        " x 15) = " + score;
                    currentModeScore.Player3Score = player3.Score = score;
                    //player 4
                    score = player4.Score - ((player4.AteCards.Count / 4) * 15);
                    currentModeScore.Player4 = player4.Score + " - (" + (player4.AteCards.Count / 4) +
                        " x 15) = " + score;
                    currentModeScore.Player4Score = player4.Score = score;
                    break;
                #endregion
                #region Trix
                case Core.PlayMode.Trix:
                    currentModeScore.CurrentGameMode = "Trix";

                    for (int i = 0; i < TrixFinishList.Count; i++)
                    {
                        int pluse = 200 - (i * 50);
                        switch (TrixFinishList[i])
                        {
                            case 0:
                                score = player1.Score;
                                score += pluse;
                                currentModeScore.Player1 = player1.Score + " + " + pluse + " = " + score;
                                currentModeScore.Player1Score = player1.Score = score;
                                break;
                            case 1:
                                score = player2.Score;
                                score += pluse;
                                currentModeScore.Player2 = player2.Score + " + " + pluse + " = " + score;
                                currentModeScore.Player2Score = player2.Score = score;
                                break;
                            case 2:
                                score = player3.Score;
                                score += pluse;
                                currentModeScore.Player3 = player3.Score + " + " + pluse + " = " + score;
                                currentModeScore.Player3Score = player3.Score = score;
                                break;
                            case 3:
                                score = player4.Score;
                                score += pluse;
                                currentModeScore.Player4 = player4.Score + " + " + pluse + " = " + score;
                                currentModeScore.Player4Score = player4.Score = score;
                                break;
                        }
                    }
                    break;
                #endregion
            }
            totalScores.Add(currentModeScore);
        }
        void CalculateWinningArrange()
        {
            GameOverWinningArrange = new List<TrixPlayer>();
            GameOverWinningArrange.Add(player1);
            GameOverWinningArrange.Add(player2);
            GameOverWinningArrange.Add(player3);
            GameOverWinningArrange.Add(player4);
            GameOverWinningArrange.Sort(new ScoreComparer());

            if (BartyiahEnded != null)
                BartyiahEnded(this, null);
        }
        /*Called when the user should deal ..*/
        void NormalDealing()
        {
            DrawStatus = Core.DrawStatus.StatusLabel;
            DakPlayIndex = playerIndex = NamingIndex;
            HOLD = true;//hold the game so user player resume
            switch (NamingIndex)
            {
                case 0: player1.Play_Dealing();
                    statusLabel = player1.Name + @" Has chosen """ + currentPlayMode.ToString() + @""" !!"; break;
                case 1: player2.Play_Dealing();
                    statusLabel = player2.Name + @" Has chosen """ + currentPlayMode.ToString() + @""" !!"; break;
                case 2: player3.Play_Dealing();
                    statusLabel = player3.Name + @" Has chosen """ + currentPlayMode.ToString() + @""" !!"; break;
                case 3: player4.Play_Dealing();
                    statusLabel = player4.Name + @" Has chosen """ + currentPlayMode.ToString() + @""" !!"; break;
            }
            switch (currentPlayMode)
            {
                case Core.PlayMode.Diamonds:
                case Core.PlayMode.Ltoosh:
                case Core.PlayMode.Trix: status = Core.PlayStatus.PlayingMode; break;

                case Core.PlayMode.KingOfHearts:
                case Core.PlayMode.Queens: 
                status = Core.PlayStatus.Doubling;
                statusLabel = currentPlayMode == Core.PlayMode.Queens ? @"Doubling ""Queens""" :
              @"Doubling ""King Of Hearts""";
                    DoublingPlayed = false;
                    break;
            }
            FREEZTime = 60;
        }
        /*Called when the user finished the 5 modes*/
        void MoveToNextDealing()
        {
            DrawStatus = Core.DrawStatus.StatusLabel;
            status = Core.PlayStatus.Dealing;//keep in Dealing mode
            NamingCount++;
            NamingIndex = (NamingIndex + 1) % 4;
            if (NamingCount == 5)
            {
                GAMEOVER = true;
                statusLabel = "Game Over";
                HOLD = true;
                CalculateWinningArrange();
            }
            else
            {
                playedModes.Clear();
                FREEZTime = 60;
                switch (NamingIndex)
                {
                    case 0: statusLabel = player1.Name + @" Naming !!"; break;
                    case 1: statusLabel = player2.Name + @" Naming !!"; break;
                    case 2: statusLabel = player3.Name + @" Naming !!"; break;
                    case 3: statusLabel = player4.Name + @" Naming !!"; break;
                }
            }
        }
        /// <summary>
        /// Update game one tick
        /// </summary>
        public void Update()
        {
            if (HOLD)
                return;
            if (FREEZTime > 0)
            { FREEZTime--; return; }

            switch (status)
            {
                #region Distributing Cards
                case Core.PlayStatus.Distributing:
                    Random r = new Random();
                    int cardIndex = 0;
                    //نهاية البرتية
                    if (NamingCount == 4 & playedModes.Count == 5)
                    {
                        GAMEOVER = true;
                        statusLabel = "Game Over";
                        HOLD = true;
                        CalculateWinningArrange();
                        return;
                    }
                    if (timer < 52)
                    {
                        if (timer == 0)
                        {
                            if (FattaEffectRequest != null)
                                FattaEffectRequest(this, new EventArgs());
                        }
                        PlayerTurn = false;
                        statusLabel = "Distributing Cards";
                        switch (playerIndex)
                        {
                            case 0:
                                //randomize index
                                r = new Random();
                                cardIndex = r.Next(0, cards.Count);
                                //add the card to player
                                player1.CardsOnHand.Add(cards[cardIndex]);
                                //remove from Distribute cards
                                cards.RemoveAt(cardIndex);
                                break;
                            case 1:
                                //randomize index
                                r = new Random();
                                cardIndex = r.Next(0, cards.Count);
                                //add the card to player
                                player2.CardsOnHand.Add(cards[cardIndex]);
                                //remove from Distribute cards
                                cards.RemoveAt(cardIndex);
                                break;
                            case 2:
                                //randomize index
                                r = new Random();
                                cardIndex = r.Next(0, cards.Count);
                                //add the card to player
                                player3.CardsOnHand.Add(cards[cardIndex]);
                                //remove from Distribute cards
                                cards.RemoveAt(cardIndex);
                                break;
                            case 3:
                                //randomize index
                                r = new Random();
                                cardIndex = r.Next(0, cards.Count);
                                //add the card to player
                                player4.CardsOnHand.Add(cards[cardIndex]);
                                //remove from Distribute cards
                                cards.RemoveAt(cardIndex);
                                break;
                        }
                        playerIndex++;
                        if (playerIndex > 3)
                            playerIndex = 0;
                    }
                    else
                    {
                        status = Core.PlayStatus.Dealing;
                        timer = -1;
                        playerIndex = 0;
                        statusLabel = "Dealing";
                        //Clear every card on the table
                        ClearTable();
                        //arrange cards by types
                        List<Cards> onMemory = new List<Core.Cards>();
                        foreach (Cards crd in player1.CardsOnHand)
                            onMemory.Add(crd);
                        player1.CardsOnHand.Clear();
                        for (int i = 0; i < onMemory.Count; i++)
                            if (CardChecker.GetCardType(onMemory[i]) == "c")
                            {
                                player1.CardsOnHand.Add(onMemory[i]);
                                onMemory.RemoveAt(i);
                                i = -1;
                            }
                        for (int i = 0; i < onMemory.Count; i++)
                            if (CardChecker.GetCardType(onMemory[i]) == "d")
                            {
                                player1.CardsOnHand.Add(onMemory[i]);
                                onMemory.RemoveAt(i);
                                i = -1;
                            }
                        for (int i = 0; i < onMemory.Count; i++)
                            if (CardChecker.GetCardType(onMemory[i]) == "s")
                            {
                                player1.CardsOnHand.Add(onMemory[i]);
                                onMemory.RemoveAt(i);
                                i = -1;
                            }
                        player1.CardsOnHand.AddRange(onMemory);
                        FREEZTime = 20;
                    }
                    break;
                #endregion
                #region Dealing
                case Core.PlayStatus.Dealing:
                    //أول تسماية
                    if (NamingCount == 0)
                    {
                        //أول تسماية، لازم نحدد مين بدو يسمي باستخدام سبعة الكوبا
                        if (playedModes.Count == 0)
                        {
                            HOLD = true;//hold the game so user player resume
                            FREEZTime = 20;
                            if (player1.IsCardExists(Core.Cards.h_07))
                            {
                                statusLabel = player1.Name + " Has Heart 7 !!";
                                DakPlayIndex = playerIndex = NamingIndex = 0;
                                //the player should naming here
                                player1.Play_Dealing();
                                statusLabel += "\n" + player1.Name + @" Has chosen """ +
                                    currentPlayMode.ToString() + @""" !!";
                            }
                            else if (player2.IsCardExists(Core.Cards.h_07))
                            {
                                statusLabel = player2.Name + " Has Heart 7 !!";
                                DakPlayIndex = playerIndex = NamingIndex = 1;
                                //the player should naming here
                                player2.Play_Dealing();
                                statusLabel += "\n" + player2.Name + @" Has chosen """ +
                       currentPlayMode.ToString() + @""" !!";
                            }
                            else if (player3.IsCardExists(Core.Cards.h_07))
                            {
                                statusLabel = player3.Name + " Has Heart 7 !!";
                                DakPlayIndex = playerIndex = NamingIndex = 2;
                                //the player should naming here
                                player3.Play_Dealing();
                                statusLabel += "\n" + player3.Name + @" Has chosen """ +
                       currentPlayMode.ToString() + @""" !!";
                            }
                            else if (player4.IsCardExists(Core.Cards.h_07))
                            {
                                statusLabel = player4.Name + " Has Heart 7 !!";
                                DakPlayIndex = playerIndex = NamingIndex = 3;
                                //the player should naming here
                                player4.Play_Dealing();
                                statusLabel += "\n" + player4.Name + @" Has chosen """ +
                       currentPlayMode.ToString() + @""" !!";
                            }
                            NamingCount++;
                            FREEZTime = 120;
                            switch (currentPlayMode)
                            { 
                                case Core.PlayMode.Diamonds:
                                case Core.PlayMode.Ltoosh:
                                case Core.PlayMode.Trix: status = Core.PlayStatus.PlayingMode; break;

                                case Core.PlayMode.KingOfHearts:
                                case Core.PlayMode.Queens: 
                                status = Core.PlayStatus.Doubling; 
                                DoublingPlayed = false;
                                statusLabel = currentPlayMode == Core.PlayMode.Queens ? @"Doubling ""Queens""" :
                                    @"Doubling ""King Of Hearts""";
                                break;
                            }
                        }
                        else if (playedModes.Count < 5)//نفس اللاعب بس التسمايات اللي بعدها
                        {
                            NormalDealing();
                        }
                        else if (playedModes.Count == 5)//خلصو كل التسمايات، نقول للاعب التالي
                        {
                            MoveToNextDealing();
                        }
                    }
                    else if (NamingCount < 5)//Player naming
                    {
                        TrixDebugConsole.WriteLine("None first naming");
                        if (playedModes.Count < 5)//نفس اللاعب بس التسمايات اللي بعدها
                        {
                            NormalDealing();
                        }
                        else if (playedModes.Count == 5)//خلصو كل التسمايات، نقول للاعب التالي
                        {
                            MoveToNextDealing();
                        }
                    }
                    //خلصت البرتية !!
                    else if (NamingCount == 5)
                    {
                        GAMEOVER = true;
                        statusLabel = "Game Over";
                        HOLD = true;
                        CalculateWinningArrange();
                        return;
                    }
                    break;
                #endregion
                #region Doubling
                case Core.PlayStatus.Doubling:
                    if (!DoublingPlayed)
                    {
                        DoublingPlayed = true;
                        player1.DoublingCards = new List<Core.Cards>();
                        player2.DoublingCards = new List<Core.Cards>();
                        player3.DoublingCards = new List<Core.Cards>();
                        player4.DoublingCards = new List<Core.Cards>();

                        player2.Play_Doubling();
                        player3.Play_Doubling();
                        player4.Play_Doubling();

                        player1.Play_Doubling();
                    }
                    break;
                #endregion
                #region Playing Mode
                case Core.PlayStatus.PlayingMode:
                    if (currentPlayMode == Core.PlayMode.Trix)
                        DrawStatus = Core.DrawStatus.TrixTable;
                    else
                        DrawStatus = Core.DrawStatus.EatTable;
                    #region Someone must EAT (none trix game)
                    if (onTableCards.Count == 4 & (currentPlayMode != Core.PlayMode.Trix))
                    {
                        //detect which player should eat
                        int EatPlayerIndex = 0;
                        int i = 0;
                        int power = 0;
                        string DakCardType = CardChecker.GetCardType(onTableCards[0]);
                        foreach (Cards crd in onTableCards)
                        {
                            if ((CardChecker.GetCardType(crd) == DakCardType) & CardChecker.GetCardPower(crd) > power)
                            {
                                power = CardChecker.GetCardPower(crd);
                                EatPlayerIndex = (i + DakPlayIndex) % 4;
                            }
                            i++;
                        }
                        //إذا كان ختيار الكوبا مأكول و اللعبة ختيار، طلوب إنهاء الدق بالتكة الجاية
                        if (PlayMode == Core.PlayMode.KingOfHearts)
                        {
                            foreach (Core.Cards crd in onTableCards)
                            {
                                if (CardChecker.GetCardType(crd) == "h" & CardChecker.GetCardName(crd) == "k")
                                {
                                    EndGame = true;
                                }
                            }
                        }
                        //Now we know who must eat, add the latche
             
                        switch (EatPlayerIndex)
                        {
                            case 0:
                                player1.AteCards.AddRange(onTableCards);
                                if (CardsAte != null)
                                    CardsAte(this, new EatArgs(onTableCards.ToArray(), player1.Name));
                                onTableCards.Clear();
                                if (EffectDrawRequest != null)
                                    EffectDrawRequest(this, new EffectArgs(DrawEffect.Player1Eat));
                                break;
                            case 1:
                                player2.AteCards.AddRange(onTableCards);
                                if (CardsAte != null)
                                    CardsAte(this, new EatArgs(onTableCards.ToArray(), player2.Name));
                                onTableCards.Clear();
                                if (EffectDrawRequest != null)
                                    EffectDrawRequest(this, new EffectArgs(DrawEffect.Player2Eat));
                                break;
                            case 2:
                                player3.AteCards.AddRange(onTableCards);
                                if (CardsAte != null)
                                    CardsAte(this, new EatArgs(onTableCards.ToArray(), player3.Name));
                                onTableCards.Clear();
                                if (EffectDrawRequest != null)
                                    EffectDrawRequest(this, new EffectArgs(DrawEffect.Player3Eat));
                                break;
                            case 3:
                                player4.AteCards.AddRange(onTableCards);
                                if (CardsAte != null)
                                    CardsAte(this, new EatArgs(onTableCards.ToArray(), player4.Name));
                                onTableCards.Clear();
                                if (EffectDrawRequest != null)
                                    EffectDrawRequest(this, new EffectArgs(DrawEffect.Player4Eat));
                                break;
                        }
                        FREEZTime = 60;
                        // No card to play
                        //ما حدا معو شي ليلعب أو خلص الدق لسبب ما، حط مود الفتة
                        if (NoCardLeftToPlay() | EndGame)
                        {
                            EndGameMode();
                        }
                        //determine the turn
                        DakPlayIndex = playerIndex = EatPlayerIndex;
                    }
                    //ما حدا معو شي ليلعب
                    else if ((currentPlayMode == Core.PlayMode.Trix) & NoCardLeftToPlay())
                    {

                        EndGameMode();
                    }
                    #endregion
                    else
                    {
                        switch (currentPlayMode)
                        {
                            case Core.PlayMode.Diamonds://The player must not eat dynar ...
                                switch (playerIndex)
                                {
                                    case 0: player1.Play_Dynar(); break;
                                    case 1: player2.Play_Dynar();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player2Play));
                                        break;
                                    case 2: player3.Play_Dynar();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player3Play));
                                        break;
                                    case 3: player4.Play_Dynar();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player4Play));
                                        break;
                                }
                                break;
                            case Core.PlayMode.KingOfHearts://The player must not eat Khetyar ...
                                switch (playerIndex)
                                {
                                    case 0: player1.Play_Khetyar(); break;
                                    case 1: player2.Play_Khetyar();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player2Play));
                                        break;
                                    case 2:
                                        player3.Play_Khetyar();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player3Play));
                                        break;
                                    case 3: player4.Play_Khetyar();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player4Play));
                                        break;
                                }
                                break;
                            case Core.PlayMode.Ltoosh://The player must not eat Ltouch ...
                                switch (playerIndex)
                                {
                                    case 0: player1.Play_Ltouch(); break;
                                    case 1: player2.Play_Ltouch();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player2Play));
                                        break;
                                    case 2: player3.Play_Ltouch();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player3Play));
                                        break;
                                    case 3: player4.Play_Ltouch();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player4Play));
                                        break;
                                }
                                break;
                            case Core.PlayMode.Queens://The player must not eat Queens ...
                                switch (playerIndex)
                                {
                                    case 0: player1.Play_Queens(); break;
                                    case 1: player2.Play_Queens();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player2Play));
                                        break;
                                    case 2: player3.Play_Queens();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player3Play));
                                        break;
                                    case 3: player4.Play_Queens();
                                        if (EffectDrawRequest != null)
                                            EffectDrawRequest(this, new EffectArgs(DrawEffect.Player4Play));
                                        break;
                                }
                                break;
                            case Core.PlayMode.Trix://The player must play Trix ...
                                switch (playerIndex)
                                {
                                    case 0:
                                        if (player1.CardsOnHand.Count > 0)
                                        {
                                            player1.Play_Trix(); /*The finish list must be updated by drawer*/
                                        }
                                        else
                                        {
                                            playerIndex = (playerIndex + 1) % 4;
                                            FREEZTime = 60;
                                        }
                                        break;
                                    case 1:
                                        if (player2.CardsOnHand.Count > 0)
                                        {
                                            bool rise = player2.Play_Trix();
                                            if (player2.CardsOnHand.Count == 0)
                                            { TrixFinishList.Add(1); }
                                            if (EffectDrawRequest != null & rise)
                                                EffectDrawRequest(this, new EffectArgs(DrawEffect.Player2Play));
                                        }
                                        else
                                        { playerIndex = (playerIndex + 1) % 4; FREEZTime = 60; }
                                        break;
                                    case 2:
                                        if (player3.CardsOnHand.Count > 0)
                                        {
                                            bool rise = player3.Play_Trix();
                                            if (player3.CardsOnHand.Count == 0)
                                            { TrixFinishList.Add(2); }
                                            if (EffectDrawRequest != null & rise)
                                                EffectDrawRequest(this, new EffectArgs(DrawEffect.Player3Play));
                                        }
                                        else
                                        { playerIndex = (playerIndex + 1) % 4; FREEZTime = 60; }
                                        break;
                                    case 3:
                                        if (player4.CardsOnHand.Count > 0)
                                        {
                                            bool rise = player4.Play_Trix();
                                            if (player4.CardsOnHand.Count == 0)
                                            { TrixFinishList.Add(3); }
                                            if (EffectDrawRequest != null & rise)
                                                EffectDrawRequest(this, new EffectArgs(DrawEffect.Player4Play));
                                        }
                                        else
                                        { playerIndex = (playerIndex + 1) % 4; FREEZTime = 60; }
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                #endregion
            }
            timer++;
        }

        /// <summary>
        /// Get or set the player 1 (YOU)
        /// </summary>
        public TrixPlayer Player1
        {
            get { return player1; }
            set { player1 = value; }
        }
        /// <summary>
        /// Get or set the player 2 (FOE 1)
        /// </summary>
        public TrixPlayer Player2
        {
            get { return player2; }
            set { player2 = value; }
        }
        /// <summary>
        /// Get or set the player 3 (YOUR Partner)
        /// </summary>
        public TrixPlayer Player3
        {
            get { return player3; }
            set { player3 = value; }
        }
        /// <summary>
        /// Get or set the player 4 (FOE 2)
        /// </summary>
        public TrixPlayer Player4
        {
            get { return player4; }
            set { player4 = value; }
        }
        /// <summary>
        /// Get or set the current play mode
        /// </summary>
        public PlayMode PlayMode
        { get { return currentPlayMode; } set { currentPlayMode = value; } }
        /// <summary>
        /// الأوراق اللي مو مع حدي و اللي لازم تنفت
        /// </summary>
        public List<Cards> Cards
        { get { return cards; } set { cards = value; } }
        /// <summary>
        /// الأوراق اللي عالطاولة
        /// </summary>
        public List<Cards> CardsOnTable
        { get { return onTableCards; } set { onTableCards = value; } }

        /// <summary>
        /// Trix cards H group
        /// </summary>
        public List<Cards> CardsOnTable_Trix_H
        { get { return onTableCards_TrixG_h; } set { onTableCards_TrixG_h = value; } }
        /// <summary>
        /// Trix cards S group
        /// </summary>
        public List<Cards> CardsOnTable_Trix_S
        { get { return onTableCards_TrixG_s; } set { onTableCards_TrixG_s = value; } }
        /// <summary>
        /// Trix cards C group
        /// </summary>
        public List<Cards> CardsOnTable_Trix_C
        { get { return onTableCards_TrixG_c; } set { onTableCards_TrixG_c = value; } }
        /// <summary>
        /// Trix cards D group
        /// </summary>
        public List<Cards> CardsOnTable_Trix_D
        { get { return onTableCards_TrixG_d; } set { onTableCards_TrixG_d = value; } }

        /// <summary>
        /// Get or set the current PlayStatus
        /// </summary>
        public PlayStatus PlayStatus
        { get { return status; } set { status = value; } }
        /// <summary>
        /// Get or set the status label that should be painted on the screen
        /// </summary>
        public string StatusLabel
        { get { return statusLabel; } set { statusLabel = value; } }
        public ScoreItem PlayModeScore
        {
            get { return currentModeScore; }
            set { currentModeScore = value; }
        }
        public List<ScoreItem> TotalScores
        {
            get { return totalScores; }
            set { totalScores = value; }
        }
        public bool IsModePlayed(PlayMode playMode)
        {
            //try
            {
                foreach (PlayMode crd in playedModes)
                    if (crd.ToString() == playMode.ToString())
                        return true;
            }
            //catch { }
            return false;
        }
        public bool NoCardLeftToPlay()
        {
            return ((player1.CardsOnHand.Count == 0) & (player2.CardsOnHand.Count == 0)
                & (player3.CardsOnHand.Count == 0) & (player4.CardsOnHand.Count == 0));
        }

        /// <summary>
        /// Rised when an effect need to be drawn
        /// </summary>
        public event EventHandler<EffectArgs> EffectDrawRequest;
        /// <summary>
        /// Rised when the mode is over and the drawer must clear any effect
        /// </summary>
        public event EventHandler ClearEffectRequest;
        public event EventHandler FattaEffectRequest;
        /// <summary>
        /// Rised when the game mode ended
        /// </summary>
        public event EventHandler GameModeEnded;
        /// <summary>
        /// Rised when the Bartyiah Ended
        /// </summary>
        public event EventHandler BartyiahEnded;
        /// <summary>
        /// Rised when player ate cards
        /// </summary>
        public event EventHandler<EatArgs> CardsAte;

        public void RiseEffect(EffectArgs args)
        {
            if (EffectDrawRequest != null)
                EffectDrawRequest(this, args);
        }
    }
}
