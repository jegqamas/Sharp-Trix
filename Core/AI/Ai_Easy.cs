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
/* Stupid AI, writen by Ala Hadid at Saturday, November 12, 2011
 * The aim of this AI to test the game, should named biggener, however
 * this stupid ai is always think about running away when it's turn
 * and when it has the chance it try it's best to play the most killing card 
 * like the h_k.
 */
namespace AHD.SharpTrix.Core
{
    public class Ai_Easy : AI
    {
        TrixBartyiah bartyiah;
        TrixPlayer player;
        public Ai_Easy(TrixBartyiah bartyiah)
        {
            this.bartyiah = bartyiah;
        }
        public void Play_Dealing()
        {
            //TODO this is a test code, make it pro !!
            Random r = new Random();

            //let's see the modes we can choose
            List<PlayMode> availableModes = new List<PlayMode>();

            if (!bartyiah.IsModePlayed(Core.PlayMode.Diamonds))
                availableModes.Add(Core.PlayMode.Diamonds);
            if (!bartyiah.IsModePlayed(Core.PlayMode.KingOfHearts))
                availableModes.Add(Core.PlayMode.KingOfHearts);
            if (!bartyiah.IsModePlayed(Core.PlayMode.Ltoosh))
                availableModes.Add(Core.PlayMode.Ltoosh);
            if (!bartyiah.IsModePlayed(Core.PlayMode.Queens))
                availableModes.Add(Core.PlayMode.Queens);
            if (!bartyiah.IsModePlayed(Core.PlayMode.Trix))
                availableModes.Add(Core.PlayMode.Trix);

            int chosenIndex = r.Next(0, availableModes.Count);
            //set the play mode in the bartyiah
            bartyiah.PlayMode = availableModes[chosenIndex];
            bartyiah.playedModes.Add(availableModes[chosenIndex]);
            //resume the game
            bartyiah.PlayStatus = PlayStatus.PlayingMode;
            bartyiah.HOLD = false;
        }
        public void Play_Doubling()
        {
            //he's stupid, even he doesn't know what's the doubling !!
        }

        public void Play_Dynar()
        {
            //TODO: this is a stupid test code, make it pro
            if (bartyiah.CardsOnTable.Count == 0)//the player must play first
            {
                //دوّر على أضعف ورقة موجودة بالإيدين و لازم ما تكون دينار
                int power = 12;
                int index = 0;
                int i = 0;
                foreach (Cards card in player.CardsOnHand)
                {
                    if (CardChecker.GetCardPower(card) < power &
                        (CardChecker.GetCardType(card) != "d"))
                    {
                        power = CardChecker.GetCardPower(card);
                        index = i;
                    }
                    i++;
                }
                //لعاب الورقة و مشي الدور
                bartyiah.CardsOnTable.Add(player.CardsOnHand[index]);
                player.LastPlayedCard = player.CardsOnHand[index];
                player.CardsOnHand.RemoveAt(index);
                //advance index
                bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
            }
            else//the player turn is after another, change play style
            {
                string cardType = CardChecker.GetCardType(bartyiah.CardsOnTable[0]);
                //هل مع اللاعب ورق من هذا النوع ؟ إذا كان ذلك فدور على أضعف واحد منها
                if (CardChecker.IsCardTypeExistInCollection(cardType, player.CardsOnHand))
                {
                    //جيب كل الأوراق من هدا النوع
                    List<Cards> availableToPlay = new List<Cards>();
                    foreach (Cards crd in player.CardsOnHand)
                    {
                        if (CardChecker.GetCardType(crd) == cardType)
                        {
                            availableToPlay.Add(crd);
                        }
                    }
                    //دوّر على أضعف ورقة موجودة بالمجموعة و لازم ما تكون دينار
                    int power = 12;
                    int index = 0;
                    int i = 0;
                    foreach (Cards card in availableToPlay)
                    {
                        if (CardChecker.GetCardPower(card) < power &
                           (CardChecker.GetCardType(card) != "d"))
                        {
                            power = CardChecker.GetCardPower(card);
                            index = i;
                        }
                        i++;
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(availableToPlay[index]);
                    player.LastPlayedCard = availableToPlay[index];
                    player.CardsOnHand.Remove(availableToPlay[index]);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                }
                else//ما معو من هالنوع، دور على أقوى ورقة و لعاب عشوائياً
                {
                    //دوّر على أقوى ورقة موجودة بالإيدين أما إذا كانت دينار فلعبها إجباري
                    int power = 0;
                    int index = 0;
                    int i = 0;
                    foreach (Cards card in player.CardsOnHand)
                    {
                        if (CardChecker.GetCardType(card) == "d")
                        {
                            index = i; break;
                        }
                        if (CardChecker.GetCardPower(card) > power)
                        {
                            power = CardChecker.GetCardPower(card);
                            index = i;
                        }
                        i++;
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(player.CardsOnHand[index]);
                    player.LastPlayedCard = player.CardsOnHand[index];
                    player.CardsOnHand.RemoveAt(index);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                }
            }
            bartyiah.FREEZTime = 60;//freez one second to let players see what happened
        }
        public void Play_Khetyar()
        {
            //TODO: this is a stupid test code, make it pro
            if (bartyiah.CardsOnTable.Count == 0)//the player must play first
            {
                //دوّر على أضعف ورقة موجودة بالإيدين و لازم ما تكون ختيار الكوبا
                int power = 12;
                int index = 0;
                int i = 0;
                foreach (Cards card in player.CardsOnHand)
                {
                    if (CardChecker.GetCardPower(card) < power &
                        (CardChecker.GetCardName(card) != "k") & (CardChecker.GetCardType(card) != "h"))
                    {
                        power = CardChecker.GetCardPower(card);
                        index = i;
                    }
                    i++;
                }
                //لعاب الورقة و مشي الدور
                bartyiah.CardsOnTable.Add(player.CardsOnHand[index]);
                player.LastPlayedCard = player.CardsOnHand[index];
                player.CardsOnHand.RemoveAt(index);
                //advance index
                bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
            }
            else//the player turn is after another, change play style
            {
                string cardType = CardChecker.GetCardType(bartyiah.CardsOnTable[0]);
                //هل مع اللاعب ورق من هذا النوع ؟ إذا كان ذلك فدور على أضعف واحد منها
                if (CardChecker.IsCardTypeExistInCollection(cardType, player.CardsOnHand))
                {
                    //جيب كل الأوراق من هدا النوع
                    List<Cards> availableToPlay = new List<Cards>();
                    foreach (Cards crd in player.CardsOnHand)
                    {
                        if (CardChecker.GetCardType(crd) == cardType)
                        {
                            availableToPlay.Add(crd);
                        }
                    }
                    //دوّر على أضعف ورقة موجودة بالمجموعة و لازم ما تكون ختيار الكوبا
                    int power = 12;
                    int index = 0;
                    int i = 0;
                    foreach (Cards card in availableToPlay)
                    {
                        if (CardChecker.GetCardPower(card) < power &
                              ((CardChecker.GetCardName(card) != "k") & (CardChecker.GetCardType(card) != "h")))
                        {
                            power = CardChecker.GetCardPower(card);
                            index = i;
                        }
                        i++;
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(availableToPlay[index]);
                    player.LastPlayedCard = availableToPlay[index];
                    player.CardsOnHand.Remove(availableToPlay[index]);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                }
                else//ما معو من هالنوع، دور على أقوى ورقة و لعاب عشوائياً
                {
                    //دوّر على أقوى ورقة موجودة بالإيدين أما إذا كانت ختيار الكوبا فلعبها إجباري
                    int power = 0;
                    int index = 0;
                    int i = 0;
                    foreach (Cards card in player.CardsOnHand)
                    {
                        if ((CardChecker.GetCardName(card) == "k") &
                            (CardChecker.GetCardType(card) == "h"))
                        {
                            index = i; break;
                        }
                        if (CardChecker.GetCardPower(card) > power)
                        {
                            power = CardChecker.GetCardPower(card);
                            index = i;
                        }
                        i++;
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(player.CardsOnHand[index]);
                    player.LastPlayedCard = player.CardsOnHand[index];
                    player.CardsOnHand.RemoveAt(index);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                }
            }
            bartyiah.FREEZTime = 60;//freez one second to let players see what happened
        }
        public void Play_Ltouch()
        {
            //TODO: this is a stupid test code, make it pro
            if (bartyiah.CardsOnTable.Count == 0)//the player must play first
            {
                //دوّر على أضعف ورقة موجودة بالإيدين
                int power = 12;
                int index = 0;
                int i = 0;
                foreach (Cards card in player.CardsOnHand)
                {
                    if (CardChecker.GetCardPower(card) < power)
                    {
                        power = CardChecker.GetCardPower(card);
                        index = i;
                    }
                    i++;
                }
                //لعاب الورقة و مشي الدور
                bartyiah.CardsOnTable.Add(player.CardsOnHand[index]);
                player.LastPlayedCard = player.CardsOnHand[index];
                player.CardsOnHand.RemoveAt(index);
                //advance index
                bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
            }
            else//the player turn is after another, change play style
            {
                string cardType = CardChecker.GetCardType(bartyiah.CardsOnTable[0]);
                //هل مع اللاعب ورق من هذا النوع ؟ إذا كان ذلك فدور على أضعف واحد منها
                if (CardChecker.IsCardTypeExistInCollection(cardType, player.CardsOnHand))
                {
                    //جيب كل الأوراق من هدا النوع
                    List<Cards> availableToPlay = new List<Cards>();
                    foreach (Cards crd in player.CardsOnHand)
                    {
                        if (CardChecker.GetCardType(crd) == cardType)
                        {
                            availableToPlay.Add(crd);
                        }
                    }
                    //دوّر على أضعف ورقة موجودة بالمجموعة
                    int power = 12;
                    int index = 0;
                    int i = 0;
                    foreach (Cards card in availableToPlay)
                    {
                        if (CardChecker.GetCardPower(card) < power)
                        {
                            power = CardChecker.GetCardPower(card);
                            index = i;
                        }
                        i++;
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(availableToPlay[index]);
                    player.LastPlayedCard = availableToPlay[index];
                    player.CardsOnHand.Remove(availableToPlay[index]);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                }
                else//ما معو من هالنوع، دور على أقوى ورقة و لعاب عشوائياً
                {
                    //دوّر على أقوى ورقة موجودة بالإيدين
                    int power = 0;
                    int index = 0;
                    int i = 0;
                    foreach (Cards card in player.CardsOnHand)
                    {
                        if (CardChecker.GetCardPower(card) > power)
                        {
                            power = CardChecker.GetCardPower(card);
                            index = i;
                        }
                        i++;
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(player.CardsOnHand[index]);
                    player.LastPlayedCard = player.CardsOnHand[index];
                    player.CardsOnHand.RemoveAt(index);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                }
            }
            bartyiah.FREEZTime = 60;//freez one second to let players see what happened
        }
        public void Play_Queens()
        {
            //TODO: this is a stupid test code, make it pro
            if (bartyiah.CardsOnTable.Count == 0)//the player must play first
            {
                //دوّر على أضعف ورقة موجودة بالإيدين و لازم ما تكون بنت
                int power = 12;
                int index = 0;
                int i = 0;
                foreach (Cards card in player.CardsOnHand)
                {
                    if (CardChecker.GetCardPower(card) < power &
                        (CardChecker.GetCardName(card) != "q"))
                    {
                        power = CardChecker.GetCardPower(card);
                        index = i;
                    }
                    i++;
                }
                //لعاب الورقة و مشي الدور
                bartyiah.CardsOnTable.Add(player.CardsOnHand[index]);
                player.LastPlayedCard = player.CardsOnHand[index];
                player.CardsOnHand.RemoveAt(index);
                //advance index
                bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
            }
            else//the player turn is after another, change play style
            {
                string cardType = CardChecker.GetCardType(bartyiah.CardsOnTable[0]);
                //هل مع اللاعب ورق من هذا النوع ؟ إذا كان ذلك فدور على أضعف واحد منها
                if (CardChecker.IsCardTypeExistInCollection(cardType, player.CardsOnHand))
                {
                    //جيب كل الأوراق من هدا النوع
                    List<Cards> availableToPlay = new List<Cards>();
                    foreach (Cards crd in player.CardsOnHand)
                    {
                        if (CardChecker.GetCardType(crd) == cardType)
                        {
                            availableToPlay.Add(crd);
                        }
                    }
                    //دوّر على أضعف ورقة موجودة بالمجموعة و لازم ما تكون بنت
                    int power = 12;
                    int index = 0;
                    int i = 0;
                    foreach (Cards card in availableToPlay)
                    {
                        if (CardChecker.GetCardPower(card) < power &
                        (CardChecker.GetCardName(card) != "q"))
                        {
                            power = CardChecker.GetCardPower(card);
                            index = i;
                        }
                        i++;
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(availableToPlay[index]);
                    player.LastPlayedCard = availableToPlay[index];
                    player.CardsOnHand.Remove(availableToPlay[index]);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                }
                else//ما معو من هالنوع، دور على أقوى ورقة و لعاب عشوائياً
                {
                    //دوّر على أقوى ورقة موجودة بالإيدين أما إذا كانت بنت فلعبها إجباري
                    int power = 0;
                    int index = 0;
                    int i = 0;
                    foreach (Cards card in player.CardsOnHand)
                    {
                        if (CardChecker.GetCardName(card) == "q")
                        {
                            index = i; break;
                        }
                        if (CardChecker.GetCardPower(card) > power)
                        {
                            power = CardChecker.GetCardPower(card);
                            index = i;
                        }
                        i++;
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(player.CardsOnHand[index]);
                    player.LastPlayedCard = player.CardsOnHand[index];
                    player.CardsOnHand.RemoveAt(index);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                }
            }
            bartyiah.FREEZTime = 60;//freez one second to let players see what happened
        }
        public bool Play_Trix()
        {
            #region خلينا نجرب الكوبا H
            //معو كوبا ؟
            if (CardChecker.IsCardTypeExistInCollection("h", player.CardsOnHand))
            {
                //دور عالورقة اللي في يلعبها
                //معو شب ؟
                if (player.IsCardExists(Cards.h_j))
                {
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable_Trix_H.Add(Cards.h_j);
                    player.LastPlayedCard = Cards.h_j;
                    player.CardsOnHand.Remove(Cards.h_j);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                    return true;
                }
                else//ما معو شب
                {
                    //شوف الأوراق اللي عالطاولة 
                    if (bartyiah.CardsOnTable_Trix_H.Count > 0)
                    {
                        //جيب كل الأوراق من هدا النوع
                        List<Cards> availableToPlay = new List<Cards>();
                        foreach (Cards crd in player.CardsOnHand)
                        {
                            if (CardChecker.GetCardType(crd) == "h")
                            {
                                availableToPlay.Add(crd);
                            }
                        }
                        //شوف كل ورقة، إذا كانت أضعف من أول ورقة من ورق الطاولة أو أقوى من آخر ورقة
                        //لعبا و إلا جرب نوع تاني
                        foreach (Cards crd in availableToPlay)
                        {
                            //إذا كانت أضعف من أول ورقة من ورق الطاولة 
                            if (CardChecker.Trix_GetIndex(crd) < CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_H[0]))
                            {
                                //لازم تكون الورقة بعدها بالترتيب
                                if (CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_H[0]) - CardChecker.Trix_GetIndex(crd) == 1)
                                {
                                    //لعاب الورقة و مشي الدور
                                    bartyiah.CardsOnTable_Trix_H.Insert(0, crd);
                                    player.LastPlayedCard = crd;
                                    player.CardsOnHand.Remove(crd);
                                    //advance index
                                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                                    return true;
                                }
                            }
                            //إذا كانت أقوى من آخر ورقة من ورق الطاولة 
                            else if (CardChecker.Trix_GetIndex(crd) >
                                CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_H[bartyiah.CardsOnTable_Trix_H.Count - 1]))
                            {
                                //لازم تكون الورقة بعدها بالترتيب
                                if (CardChecker.Trix_GetIndex(crd) -
                                    CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_H[bartyiah.CardsOnTable_Trix_H.Count - 1]) == 1)
                                {
                                    //لعاب الورقة و مشي الدور
                                    bartyiah.CardsOnTable_Trix_H.Add(crd);
                                    player.LastPlayedCard = crd;
                                    player.CardsOnHand.Remove(crd);
                                    //advance index
                                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region خلينا نجرب الديناري D
            //معو كوبا ؟
            if (CardChecker.IsCardTypeExistInCollection("d", player.CardsOnHand))
            {
                //دور عالورقة اللي في يلعبها
                //معو شب ؟
                if (player.IsCardExists(Cards.d_j))
                {
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable_Trix_D.Add(Cards.d_j);
                    player.CardsOnHand.Remove(Cards.d_j);
                    player.LastPlayedCard = Cards.d_j;
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                    return true;
                }
                else//ما معو شب
                {
                    //شوف الأوراق اللي عالطاولة إذا في شب من هدا النوع منشان يلعب
                    if (bartyiah.CardsOnTable_Trix_D.Count > 0)
                    {
                        //جيب كل الأوراق من هدا النوع
                        List<Cards> availableToPlay = new List<Cards>();
                        foreach (Cards crd in player.CardsOnHand)
                        {
                            if (CardChecker.GetCardType(crd) == "d")
                            {
                                availableToPlay.Add(crd);
                            }
                        }
                        //شوف كل ورقة، إذا كانت أضعف من أول ورقة من ورق الطاولة أو أقوى من آخر ورقة
                        //لعبا و إلا جرب نوع تاني
                        foreach (Cards crd in availableToPlay)
                        {
                            //إذا كانت أضعف من أول ورقة من ورق الطاولة 
                            if (CardChecker.Trix_GetIndex(crd) < CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_D[0]))
                            {
                                //لازم تكون الورقة بعدها بالترتيب
                                if (CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_D[0]) - CardChecker.Trix_GetIndex(crd) == 1)
                                {
                                    //لعاب الورقة و مشي الدور
                                    bartyiah.CardsOnTable_Trix_D.Insert(0, crd);
                                    player.LastPlayedCard = crd;
                                    player.CardsOnHand.Remove(crd);
                                    //advance index
                                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                                    return true;
                                }
                            }
                            //إذا كانت أقوى من آخر ورقة من ورق الطاولة 
                            else if (CardChecker.Trix_GetIndex(crd) >
                                CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_D[bartyiah.CardsOnTable_Trix_D.Count - 1]))
                            {
                                //لازم تكون الورقة بعدها بالترتيب
                                if (CardChecker.Trix_GetIndex(crd) -
                                    CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_D[bartyiah.CardsOnTable_Trix_D.Count - 1]) == 1)
                                {
                                    //لعاب الورقة و مشي الدور
                                    bartyiah.CardsOnTable_Trix_D.Add(crd);
                                    player.LastPlayedCard = crd;
                                    player.CardsOnHand.Remove(crd);
                                    //advance index
                                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region خلينا نجرب المغزلاني C
            //معو كوبا ؟
            if (CardChecker.IsCardTypeExistInCollection("c", player.CardsOnHand))
            {
                //دور عالورقة اللي في يلعبها
                //معو شب ؟
                if (player.IsCardExists(Cards.c_j))
                {
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable_Trix_C.Add(Cards.c_j);
                    player.LastPlayedCard = Cards.c_j;
                    player.CardsOnHand.Remove(Cards.c_j);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                    return true;
                }
                else//ما معو شب
                {
                    //شوف الأوراق اللي عالطاولة إذا في شب من هدا النوع منشان يلعب
                    if (bartyiah.CardsOnTable_Trix_C.Count > 0)
                    {
                        //جيب كل الأوراق من هدا النوع
                        List<Cards> availableToPlay = new List<Cards>();
                        foreach (Cards crd in player.CardsOnHand)
                        {
                            if (CardChecker.GetCardType(crd) == "c")
                            {
                                availableToPlay.Add(crd);
                            }
                        }
                        //شوف كل ورقة، إذا كانت أضعف من أول ورقة من ورق الطاولة أو أقوى من آخر ورقة
                        //لعبا و إلا جرب نوع تاني
                        foreach (Cards crd in availableToPlay)
                        {
                            //إذا كانت أضعف من أول ورقة من ورق الطاولة 
                            if (CardChecker.Trix_GetIndex(crd) < CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_C[0]))
                            {
                                //لازم تكون الورقة بعدها بالترتيب
                                if (CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_C[0]) - CardChecker.Trix_GetIndex(crd) == 1)
                                {
                                    //لعاب الورقة و مشي الدور
                                    bartyiah.CardsOnTable_Trix_C.Insert(0, crd);
                                    player.LastPlayedCard = crd;
                                    player.CardsOnHand.Remove(crd);
                                    //advance index
                                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                                    return true;
                                }
                            }
                            //إذا كانت أقوى من آخر ورقة من ورق الطاولة 
                            else if (CardChecker.Trix_GetIndex(crd) >
                                CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_C[bartyiah.CardsOnTable_Trix_C.Count - 1]))
                            {
                                //لازم تكون الورقة بعدها بالترتيب
                                if (CardChecker.Trix_GetIndex(crd) -
                                    CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_C[bartyiah.CardsOnTable_Trix_C.Count - 1]) == 1)
                                {
                                    //لعاب الورقة و مشي الدور
                                    bartyiah.CardsOnTable_Trix_C.Add(crd);
                                    player.LastPlayedCard = crd;
                                    player.CardsOnHand.Remove(crd);
                                    //advance index
                                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region خلينا نجرب السباد S
            //معو كوبا ؟
            if (CardChecker.IsCardTypeExistInCollection("s", player.CardsOnHand))
            {
                //دور عالورقة اللي في يلعبها
                //معو شب ؟
                if (player.IsCardExists(Cards.s_j))
                {
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable_Trix_S.Add(Cards.s_j);
                    player.LastPlayedCard = Cards.s_j;
                    player.CardsOnHand.Remove(Cards.s_j);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                    return true;
                }
                else//ما معو شب
                {
                    //شوف الأوراق اللي عالطاولة إذا في شب من هدا النوع منشان يلعب
                    if (bartyiah.CardsOnTable_Trix_S.Count > 0)
                    {
                        //جيب كل الأوراق من هدا النوع
                        List<Cards> availableToPlay = new List<Cards>();
                        foreach (Cards crd in player.CardsOnHand)
                        {
                            if (CardChecker.GetCardType(crd) == "s")
                            {
                                availableToPlay.Add(crd);
                            }
                        }
                        //شوف كل ورقة، إذا كانت أضعف من أول ورقة من ورق الطاولة أو أقوى من آخر ورقة
                        //لعبا و إلا جرب نوع تاني
                        foreach (Cards crd in availableToPlay)
                        {
                            //إذا كانت أضعف من أول ورقة من ورق الطاولة 
                            if (CardChecker.Trix_GetIndex(crd) < CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_S[0]))
                            {
                                //لازم تكون الورقة بعدها بالترتيب
                                if (CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_S[0]) - CardChecker.Trix_GetIndex(crd) == 1)
                                {
                                    //لعاب الورقة و مشي الدور
                                    bartyiah.CardsOnTable_Trix_S.Insert(0, crd);
                                    player.LastPlayedCard = crd;
                                    player.CardsOnHand.Remove(crd);
                                    //advance index
                                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                                    return true;
                                }
                            }
                            //إذا كانت أقوى من آخر ورقة من ورق الطاولة 
                            else if (CardChecker.Trix_GetIndex(crd) >
                                CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_S[bartyiah.CardsOnTable_Trix_S.Count - 1]))
                            {
                                //لازم تكون الورقة بعدها بالترتيب
                                if (CardChecker.Trix_GetIndex(crd) -
                                    CardChecker.Trix_GetIndex(bartyiah.CardsOnTable_Trix_S[bartyiah.CardsOnTable_Trix_S.Count - 1]) == 1)
                                {
                                    //لعاب الورقة و مشي الدور
                                    bartyiah.CardsOnTable_Trix_S.Add(crd);
                                    player.LastPlayedCard = crd;
                                    player.CardsOnHand.Remove(crd);
                                    //advance index
                                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                                    bartyiah.FREEZTime = 60;//freez one second to let players see what happened
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            //بالظاهر إنو الأخ ما معو شي يلعبو، باص حبيبي
            //advance index
            bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
            bartyiah.FREEZTime = 60;//freez one second to let players see what happened
            return false;
        }
        TrixPlayer AI.TrixCurrentPlayer
        {
            get
            {
                return player;
            }
            set
            {
                player = value;
            }
        }
    }
}
