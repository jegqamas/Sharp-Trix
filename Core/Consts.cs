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
    /// Playing Cards
    /// </summary>
    public enum Cards
    {
        c_02, c_03, c_04, c_05, c_06, c_07, c_08, c_09, c_10, c_j, c_k, c_q, c_a,//مغزلاني
        d_02, d_03, d_04, d_05, d_06, d_07, d_08, d_09, d_10, d_j, d_k, d_q, d_a,//ديناري
        h_02, h_03, h_04, h_05, h_06, h_07, h_08, h_09, h_10, h_j, h_k, h_q, h_a,//كوبا
        s_02, s_03, s_04, s_05, s_06, s_07, s_08, s_09, s_10, s_j, s_k, s_q, s_a,//سبات
    }

    public enum PlayMode
    {
        /// <summary>
        /// بنات
        /// </summary>
        Queens,
        /// <summary>
        /// لطوش
        /// </summary>
        Ltoosh,
        /// <summary>
        /// ختيار الكوبا
        /// </summary>
        KingOfHearts,
        /// <summary>
        /// ديناري
        /// </summary>
        Diamonds,
        /// <summary>
        /// تريكس
        /// </summary>
        Trix
    }
    /// <summary>
    /// The bartyiah status
    /// </summary>
    public enum PlayStatus
    {
        Distributing, Dealing, PlayingMode, Doubling
    }
    /// <summary>
    /// What the programmer should draw now
    /// </summary>
    public enum DrawStatus
    {
        StatusLabel, UserNaming, EatTable, TrixTable, EndGameMode
    }
    /// <summary>
    /// The draw effect that should be drawn
    /// </summary>
    public enum DrawEffect
    { Player1Play, Player1Eat, Player2Play, Player2Eat, Player3Play, Player3Eat, Player4Play, Player4Eat }

    public class ScoreComparer : IComparer<TrixPlayer>
    {
        public int Compare(TrixPlayer x, TrixPlayer y)
        {
            return (x.Score < y.Score) ? 1 : 0;
        }
    }

    /// <summary>
    /// The class which check card status like card power in it's type ...
    /// </summary>
    public class CardChecker
    {
        public static string[] CardNames =
        { 
        // 0       1       2       3       4       5       6       7       8       9      10     11     12
          "c_02", "c_03", "c_04", "c_05", "c_06", "c_07", "c_08", "c_09", "c_10", "c_j", "c_q", "c_k", "c_a",//مغزلاني
          "d_02", "d_03", "d_04", "d_05", "d_06", "d_07", "d_08", "d_09", "d_10", "d_j", "d_q", "d_k", "d_a",//ديناري
          "h_02", "h_03", "h_04", "h_05", "h_06", "h_07", "h_08", "h_09", "h_10", "h_j", "h_q", "h_k", "h_a",//كوبا
          "s_02", "s_03", "s_04", "s_05", "s_06", "s_07", "s_08", "s_09", "s_10", "s_j", "s_q", "s_k", "s_a",//سبات
        };
        static string[] TRIXindexes =
        { 
          "c_a", "c_k", "c_q", "c_j", "c_10", "c_09", "c_08", "c_07", "c_06", "c_05", "c_04", "c_03", "c_02",//مغزلاني
          "d_a", "d_k", "d_q", "d_j", "d_10", "d_09", "d_08", "d_07", "d_06", "d_05", "d_04", "d_03", "d_02",//ديناري
          "h_a", "h_k", "h_q", "h_j", "h_10", "h_09", "h_08", "h_07", "h_06", "h_05", "h_04", "h_03", "h_02",//كوبا
          "s_a", "s_k", "s_q", "s_j", "s_10", "s_09", "s_08", "s_07", "s_06", "s_05", "s_04", "s_03", "s_02",//سبات
        };
        public static int GetCardPower(Cards card)
        {
            int power = 0;
            for (int i = 0; i < CardNames.Length; i++)
            {
                if (card.ToString() == CardNames[i])
                {
                    power = i % 13; break;
                }
            }
            return power;
        }
        public static string GetCardType(Cards card)
        {
            return card.ToString().Substring(0, 1);
        }
        public static string GetCardName(Cards card)
        {
            if (card.ToString().Length == 3)
                return card.ToString().Substring(2, 1);
            else
                return card.ToString().Substring(2, 2);
        }
        public static int GetCardCountOfTypeInCollection(string cardType, List<Cards> cards)
        {
            int count = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ToString().Substring(0, 1) == cardType)
                    count++;
            }
            return count;
        }
        public static Cards GetCard(int index)
        {
            return (Cards)Enum.Parse(typeof(Cards), CardNames[index], false);
        }
        public static bool IsCardTypeExistInCollection(string cardType, List<Cards> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ToString().Substring(0, 1) == cardType)
                    return true;
            }
            return false;
        }
        public static bool IsCardTypeExistInCollection(string cardType, Cards[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].ToString().Substring(0, 1) == cardType)
                    return true;
            }
            return false;
        }
        public static bool IsCardExistInCollection(string cardName, List<Cards> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ToString().Length == 3)
                {
                    if (cards[i].ToString().Substring(2, 1) == cardName)
                        return true;
                }
                else
                {
                    if (cards[i].ToString().Substring(2, 2) == cardName)
                        return true;
                }
            }
            return false;
        }
        public static bool IsCardExistInCollection(Cards card, List<Cards> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ToString() == card.ToString())
                    return true;
            }
            return false;
        }
        public static void FillUpCollection(List<Cards> cards)
        {
            cards.Clear();
            for (int i = 0; i < CardNames.Length; i++)
            {
                cards.Add((Cards)Enum.Parse(typeof(Cards), CardNames[i], true));
            }
        }
        public static int GetCardCountByType(string cardType, List<Cards> cards)
        {
            int count = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (GetCardType(cards[i]) == cardType)
                    count++;
            }
            return count;
        }
        public static int GetCardCountByName(string cardName, List<Cards> cards)
        {
            int count = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (GetCardName(cards[i]) == cardName)
                    count++;
            }
            return count;
        }
        public static int GetCardCount(string cardName, string cardType, List<Cards> cards)
        {
            int count = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (GetCardType(cards[i]) == cardType &
                    GetCardName(cards[i]) == cardName)
                    count++;
            }
            return count;
        }
        public static int GetPowerfullCard(List<Cards> cards)
        {
            int power = 0;
            foreach (Cards card in cards)
            {
                if (CardChecker.GetCardPower(card) > power)
                {
                    power = CardChecker.GetCardPower(card);
                }
            }
            return power;
        }
        public static Cards GetFirstCardOfType(string cardType, List<Cards> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ToString().Substring(0, 1) == cardType)
                    return cards[i];
            }
            return cards[0];
        }
        public static Cards GetWeeknessCardOfType(string cardType, List<Cards> cards)
        {
            int power = 12;
            int index = 0;
            List<Cards> available = new List<Cards>();
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ToString().Substring(0, 1) == cardType)
                    available.Add( cards[i]);
            }
            for (int i = 0; i < available.Count; i++)
            {
                if (GetCardPower(available[i]) < power)
                {
                    index = i; 
                    power = GetCardPower(available[i]);
                }
            }
            return available[index];
        }
        public static Cards GetPowerfullCardOfType(string cardType, List<Cards> cards)
        {
            int power = 0;
            int index = 0;
            List<Cards> available = new List<Cards>();
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ToString().Substring(0, 1) == cardType)
                    available.Add(cards[i]);
            }
            for (int i = 0; i < available.Count; i++)
            {
                if (GetCardPower(available[i]) > power)
                {
                    index = i;
                    power = GetCardPower(available[i]);
                }
            }
            return available[index];
        }
        public static List<Cards> GetCardsExeptType(string cardType, List<Cards> cards)
        {
            List<Cards> available = new List<Cards>();
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ToString().Substring(0, 1) != cardType)
                    available.Add(cards[i]);
            }
            return available;
        }
        public static Cards GetCard(string Name, List<Cards> cards)
        {
            foreach (Cards crd in cards)
            {
                if (GetCardName(crd) == Name)
                    return crd;
            }
            return 0;
        }
        //TRIX
        public static int Trix_GetIndex(Cards card)
        {
            int power = 0;
            for (int i = 0; i < TRIXindexes.Length; i++)
            {
                if (card.ToString() == TRIXindexes[i])
                {
                    power = i % 13; break;
                }
            }
            return power;
        }
    }
}
