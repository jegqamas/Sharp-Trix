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
/*
 Normal AI, writen by Ala Hadid at: Sunday, November 20, 2011
 
 * This player is a Trix player !! play by the rules and knows how
 * to avoid tricks, also how to beat it's enimies ...
 */
namespace AHD.SharpTrix.Core
{
    public class Ai_Normal : AI
    {
        TrixBartyiah bartyiah;
        TrixPlayer player;
        bool MyTurn = false;//set true when the player is the dak handler مستلم الدق
        bool PlayedDiamonds = false;
        List<string> ForbiddenTypes = new List<string>();
        

        public Ai_Normal(TrixBartyiah bartyiah)
        {
            this.bartyiah = bartyiah;
            this.bartyiah.CardsAte += new EventHandler<EatArgs>(bartyiah_CardsAte);
            this.bartyiah.GameModeEnded += new EventHandler(bartyiah_GameModeEnded);
        }
        void bartyiah_GameModeEnded(object sender, EventArgs e)
        {
            MyTurn = false;
            PlayedDiamonds = false;
            ForbiddenTypes = new List<string>();
        }

        public void Play_Dealing()
        {
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

            int chosenIndex = 0;
            if (availableModes.Count > 1)
            {
                for (int i = 0; i < availableModes.Count; i++)
                {
                    #region Diamonds Choose
                    if (availableModes[i] == PlayMode.Diamonds)
                    {
                        //To choose diamonds, the player should have week cards of diamonds
                        //get every dynar card
                        List<Cards> avToPlay = new List<Cards>();
                        foreach (Cards card in player.CardsOnHand)
                        {
                            if (CardChecker.GetCardType(card) == "d")
                            {
                                avToPlay.Add(card);
                            }
                        }
                        if (avToPlay.Count > 0)
                        {
                            //now we have the cards, see what the powerful card
                            int power = 0;
                            foreach (Cards card in avToPlay)
                            {
                                if (CardChecker.GetCardPower(card) > power)
                                {
                                    power = CardChecker.GetCardPower(card);
                                }
                            }
                            //evaluate the power
                            if (power < 9)//power < j
                            {
                                //we can choose diamonds in this case, the player has week cards
                                //but first we should see if the player has a powerful card
                                int pcard = CardChecker.GetPowerfullCard(player.CardsOnHand);
                                if (pcard >= 9)
                                {
                                    //this is it, choose diamonmds
                                    chosenIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //No diamond card !! there's no chance to choose this mode ...
                        }
                    }
                    #endregion
                    #region KingOfHearts choose
                    else if (availableModes[i] == PlayMode.KingOfHearts)
                    {
                        //to choose this mode, the player should has big cound of h type
                        //get every hearts card
                        List<Cards> avToPlay = new List<Cards>();
                        foreach (Cards card in player.CardsOnHand)
                        {
                            if (CardChecker.GetCardType(card) == "h")
                            {
                                avToPlay.Add(card);
                            }
                        }
                        //how many ? these should be more than 3 ...
                        if (avToPlay.Count >= 4)
                        {
                            //this is it
                            chosenIndex = i;
                            break;
                        }
                    }
                    #endregion
                    #region Ltoosh
                    else if (availableModes[i] == PlayMode.Ltoosh)
                    {
                        //to play ltoosh, the player should has too many week cards
                        List<Cards> avToPlay = new List<Cards>();
                        foreach (Cards card in player.CardsOnHand)
                        {
                            if (CardChecker.GetCardPower(card) < 9)
                            {
                                avToPlay.Add(card);
                            }
                        }
                        if (avToPlay.Count >= 7)
                        {
                            //this is it
                            chosenIndex = i;
                            break;
                        }
                    }
                    #endregion
                    #region queens
                    else if (availableModes[i] == PlayMode.Queens)
                    {
                        //he should has queen(s)
                        int queens = CardChecker.GetCardCountByName("q", player.CardsOnHand);
                        if (queens >= 1)
                        {
                            //now the player should look for a small count of type
                            if (CardChecker.GetCardCountByType("h", player.CardsOnHand) < 3)
                            {
                                //this is it
                                if (player.IsCardExists(Cards.h_q))
                                    chosenIndex = i;
                                break;
                            }
                            else if (CardChecker.GetCardCountByType("c", player.CardsOnHand) < 3)
                            {
                                //this is it
                                if (player.IsCardExists(Cards.c_q))
                                    chosenIndex = i;
                                break;
                            }
                            else if (CardChecker.GetCardCountByType("d", player.CardsOnHand) < 3)
                            {
                                //this is it
                                if (player.IsCardExists(Cards.d_q))
                                    chosenIndex = i;
                                break;
                            }
                            else if (CardChecker.GetCardCountByType("s", player.CardsOnHand) < 3)
                            {
                                //this is it
                                if (player.IsCardExists(Cards.s_q))
                                    chosenIndex = i;
                                break;
                            }
                        }
                    }
                    #endregion
                    #region TRIX
                    else if (availableModes[i] == PlayMode.Trix)
                    {
                        //to choose this mode, the player should has a lot of powerfull cards...
                        int pow = CardChecker.GetCardCountByName("j", player.CardsOnHand);
                        pow += CardChecker.GetCardCountByName("q", player.CardsOnHand);
                        pow += CardChecker.GetCardCountByName("k", player.CardsOnHand);
                        pow += CardChecker.GetCardCountByName("a", player.CardsOnHand);
                        int jowizy = CardChecker.GetCardCountByName("02", player.CardsOnHand);
                        jowizy += CardChecker.GetCardCountByName("03", player.CardsOnHand);
                        if (pow >= 5 & jowizy < 4)//عدد كبير من الملاكين و عدد قليل من الجويزات و التريسات
                        {
                            //this is it
                            chosenIndex = i;
                            break;
                        }
                    }
                    #endregion
                }
            }//else { //no need to think, he has no choice but to choose one mode here... } 

            //set the play mode in the bartyiah
            bartyiah.PlayMode = availableModes[chosenIndex];
            bartyiah.playedModes.Add(availableModes[chosenIndex]);
            //resume the game
            bartyiah.PlayStatus = PlayStatus.PlayingMode;
            bartyiah.HOLD = false;
        }
        public void Play_Doubling()
        {
            switch (bartyiah.PlayMode)
            {
                case PlayMode.KingOfHearts:
                    if (player.IsCardExists(Cards.h_k))
                    {
                        //to choose this mode, the player should has big cound of h type
                        //get every hearts card
                        List<Cards> avToPlay = new List<Cards>();
                        foreach (Cards card in player.CardsOnHand)
                        {
                            if (CardChecker.GetCardType(card) == "h")
                            {
                                avToPlay.Add(card);
                            }
                        }
                        //how many ? these should be more than 3 ...
                        if (avToPlay.Count >= 4)
                        {
                            //this is it
                            player.DoublingCards.Add(Cards.h_k);
                            break;
                        }
                    }
                    break;
                case PlayMode.Queens:
                    //he should has queen(s)
                    int queens = CardChecker.GetCardCountByName("q", player.CardsOnHand);
                    if (queens >= 1)
                    {
                        //now the player should look for a small count of type
                        if (CardChecker.GetCardCountByType("h", player.CardsOnHand) < 3)
                        {
                            //this is it
                            if (player.IsCardExists(Cards.h_q))
                                player.DoublingCards.Add(Cards.h_q);
                            break;
                        }
                        else if (CardChecker.GetCardCountByType("c", player.CardsOnHand) < 3)
                        {
                            //this is it
                            if (player.IsCardExists(Cards.c_q))
                                player.DoublingCards.Add(Cards.c_q);
                            break;
                        }
                        else if (CardChecker.GetCardCountByType("d", player.CardsOnHand) < 3)
                        {
                            //this is it
                            if (player.IsCardExists(Cards.d_q))
                                player.DoublingCards.Add(Cards.d_q);
                            break;
                        }
                        else if (CardChecker.GetCardCountByType("s", player.CardsOnHand) < 3)
                        {
                            //this is it
                            if (player.IsCardExists(Cards.s_q))
                                player.DoublingCards.Add(Cards.s_q);
                            break;
                        }
                    }
                    break;
            }
        }

        public void Play_Dynar()
        {
            bartyiah.FREEZTime = 60;//freez one second to let players see what happened
            if (bartyiah.CardsOnTable.Count == 0)//the player must play first
            {
                MyTurn = true;
                //دوّر على نوع مقطش و قطش عليه منشان ينزل الدينار عليه...
                if (CardChecker.GetCardCountByType("h", player.CardsOnHand) < 3 & !IsForbeddinType("h"))
                {
                    //مقطش عالكوبة، لازم يكون معو ورقة أقل شي
                    if (CardChecker.GetCardCountByType("h", player.CardsOnHand) > 0)
                    {
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(CardChecker.GetWeeknessCardOfType("h", player.CardsOnHand));
                        player.LastPlayedCard = CardChecker.GetWeeknessCardOfType("h", player.CardsOnHand);
                        player.CardsOnHand.Remove(CardChecker.GetWeeknessCardOfType("h", player.CardsOnHand));
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                        PlayedDiamonds = false;
                        return;
                    }
                }
                if (CardChecker.GetCardCountByType("c", player.CardsOnHand) < 3 & !IsForbeddinType("c"))
                {
                    //مقطش ، لازم يكون معو ورقة أقل شي
                    if (CardChecker.GetCardCountByType("c", player.CardsOnHand) > 0)
                    {
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(CardChecker.GetWeeknessCardOfType("c", player.CardsOnHand));
                        player.LastPlayedCard = CardChecker.GetWeeknessCardOfType("c", player.CardsOnHand);
                        player.CardsOnHand.Remove(CardChecker.GetWeeknessCardOfType("c", player.CardsOnHand));
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                        PlayedDiamonds = false;
                        return;
                    }
                }
                if (CardChecker.GetCardCountByType("s", player.CardsOnHand) < 3 & !IsForbeddinType("s"))
                {
                    //مقطش ، لازم يكون معو ورقة أقل شي
                    if (CardChecker.GetCardCountByType("s", player.CardsOnHand) > 0)
                    {
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(CardChecker.GetWeeknessCardOfType("s", player.CardsOnHand));
                        player.LastPlayedCard = CardChecker.GetWeeknessCardOfType("s", player.CardsOnHand);
                        player.CardsOnHand.Remove(CardChecker.GetWeeknessCardOfType("s", player.CardsOnHand));
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                        PlayedDiamonds = false;
                        return;
                    }
                }
                //وصل لهون معناها مانو مقطش عشي
                //دوّر على أضعف ورقة موجودة بالإيدين و لازم ما تكون دينار
                int power = 12;
                int index = 0;
                int i = 0;
                List<Cards> available = CardChecker.GetCardsExeptType("d", player.CardsOnHand);
                //get all unforbeddin cards
                foreach (string crdT in ForbiddenTypes)
                {
                    available = CardChecker.GetCardsExeptType(crdT, available);
                }
                if (available.Count > 0)
                {
                    foreach (Cards card in available)
                    {
                        if (CardChecker.GetCardPower(card) < power)
                        {
                            power = CardChecker.GetCardPower(card);
                            index = i;
                        }
                        i++;
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(available[index]);
                    player.LastPlayedCard = available[index];
                    player.CardsOnHand.Remove(available[index]);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    PlayedDiamonds = false;
                }
                else//ما معو غير دينار أو أنواع محرمة
                {
                    //جيب كل شي ماعدا الدينار
                    power = 12;
                    index = 0;
                    i = 0;
                    available = CardChecker.GetCardsExeptType("d", player.CardsOnHand);
                    if (available.Count > 0)
                    {
                        //دور على أضعف ورقة ممكنة
                        foreach (Cards card in available)
                        {
                            if (CardChecker.GetCardPower(card) < power)
                            {
                                power = CardChecker.GetCardPower(card);
                                index = i;
                            }
                            i++;
                        }
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(available[index]);
                        player.LastPlayedCard = available[index];
                        player.CardsOnHand.Remove(available[index]);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                        PlayedDiamonds = false;
                    }
                    else//ديناري حصراً
                    {
                        //دور على أضعف ورقة ممكنة
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
                        player.CardsOnHand.Remove(player.CardsOnHand[index]);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                        PlayedDiamonds = true;
                    }
                }
            }
            else//the player turn is after another, change play style
            {
                MyTurn = false;
                string cardType = CardChecker.GetCardType(bartyiah.CardsOnTable[0]);
                //هل مع اللاعب ورق من هذا النوع ؟ 
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
                    //الأوراق من هذا النوع على الطاولة
                    List<Cards> onTable = new List<Cards>();
                    foreach (Cards crd in bartyiah.CardsOnTable)
                    {
                        if (CardChecker.GetCardType(crd) == cardType)
                        {
                            onTable.Add(crd);
                        }
                    }
                    //أقوى ورقة من المأكولات
                    int powerCard = CardChecker.GetPowerfullCard(onTable);
                    //دوّر على أقوى ورقة موجودة بالمجموعة أضعف من أقوى ورقة من المأكولات منشان نتخلص منها
                    int power = 0;
                    int i = 0;
                    List<Cards> weekList = new List<Cards>();
                    foreach (Cards card in availableToPlay)
                    {
                        if (CardChecker.GetCardPower(card) > power)
                        {
                            power = CardChecker.GetCardPower(card);
                            weekList.Add(card);
                        }
                        i++;
                    }
                    for (int j = 0; j < weekList.Count; j++)
                    {
                        if (CardChecker.GetCardPower(weekList[j]) > powerCard)
                        {
                            weekList.RemoveAt(j);
                            j = -1;
                        }
                    }
                    if (weekList.Count > 0)
                    {
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(weekList[weekList.Count - 1]);
                        player.LastPlayedCard = weekList[weekList.Count - 1];
                        player.CardsOnHand.Remove(weekList[weekList.Count - 1]);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    }
                    else
                    {
                        Cards card = CardChecker.GetWeeknessCardOfType(cardType, availableToPlay);
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(card);
                        player.LastPlayedCard = card;
                        player.CardsOnHand.Remove(card);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    }
                }
                else//ما معو من هالنوع، دور على أقوى ورقة و لعاب عشوائياً
                {
                    //دوّر على أقوى دينار و لعبها إجباري
                    int power = 0;
                    int index = 0;
                    int i = 0;
                    List<Cards> availableToPlay = new List<Cards>();
                    foreach (Cards crd in player.CardsOnHand)
                    {
                        if (CardChecker.GetCardType(crd) == "d")
                        {
                            availableToPlay.Add(crd);
                        }
                    }
                    if (availableToPlay.Count > 0)
                    {
                        foreach (Cards card in availableToPlay)
                        {
                            if (CardChecker.GetCardPower(card) > power)
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
                        return;
                    }
                    else//ما معو ديناري، تخلص من أقوى ورقة
                    {
                        foreach (Cards card in player.CardsOnHand)
                        {
                            if (CardChecker.GetCardPower(card) > power)
                            {
                                power = CardChecker.GetCardPower(card);
                                index = i;
                            }
                            i++;
                        }
                    }
                    //لعاب الورقة و مشي الدور
                    bartyiah.CardsOnTable.Add(player.CardsOnHand[index]);
                    player.LastPlayedCard = player.CardsOnHand[index];
                    player.CardsOnHand.RemoveAt(index);
                    //advance index
                    bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    return;
                }
            }
        }
        public void Play_Khetyar()
        {
            bartyiah.FREEZTime = 60;//freez one second to let players see what happened
            if (bartyiah.CardsOnTable.Count == 0)//the player must play first
            {
                MyTurn = true;
                if (player.IsCardExists(Cards.h_k)) //معو الختيار ؟
                {
                    //دوّر على نوع مقطش و قطش عليه منشان ينزل الختيار عليه...
                    if (CardChecker.GetCardCountByType("d", player.CardsOnHand) < 3)
                    {
                        //مقطش عالكوبة، لازم يكون معو ورقة أقل شي
                        if (CardChecker.GetCardCountByType("d", player.CardsOnHand) > 0)
                        {
                            //لعاب الورقة و مشي الدور
                            bartyiah.CardsOnTable.Add(CardChecker.GetWeeknessCardOfType("d", player.CardsOnHand));
                            player.LastPlayedCard = CardChecker.GetWeeknessCardOfType("d", player.CardsOnHand);
                            player.CardsOnHand.Remove(CardChecker.GetWeeknessCardOfType("d", player.CardsOnHand));
                            //advance index
                            bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                            PlayedDiamonds = false;
                            return;
                        }
                    }
                    if (CardChecker.GetCardCountByType("c", player.CardsOnHand) < 3)
                    {
                        //مقطش ، لازم يكون معو ورقة أقل شي
                        if (CardChecker.GetCardCountByType("c", player.CardsOnHand) > 0)
                        {
                            //لعاب الورقة و مشي الدور
                            bartyiah.CardsOnTable.Add(CardChecker.GetWeeknessCardOfType("c", player.CardsOnHand));
                            player.LastPlayedCard = CardChecker.GetWeeknessCardOfType("c", player.CardsOnHand);
                            player.CardsOnHand.Remove(CardChecker.GetWeeknessCardOfType("c", player.CardsOnHand));
                            //advance index
                            bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                            PlayedDiamonds = false;
                            return;
                        }
                    }
                    if (CardChecker.GetCardCountByType("s", player.CardsOnHand) < 3)
                    {
                        //مقطش ، لازم يكون معو ورقة أقل شي
                        if (CardChecker.GetCardCountByType("s", player.CardsOnHand) > 0)
                        {
                            //لعاب الورقة و مشي الدور
                            bartyiah.CardsOnTable.Add(CardChecker.GetWeeknessCardOfType("s", player.CardsOnHand));
                            player.LastPlayedCard = CardChecker.GetWeeknessCardOfType("s", player.CardsOnHand);
                            player.CardsOnHand.Remove(CardChecker.GetWeeknessCardOfType("s", player.CardsOnHand));
                            //advance index
                            bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                            PlayedDiamonds = false;
                            return;
                        }
                    }
                }
                //وصل لهون معناها مانو مقطش عشي أو مامعو الختيار، لعاب أضعف ورقة
                int power = 12;
                int index = 0;
                int i = 0;
                foreach (Cards card in player.CardsOnHand)
                {
                    if (CardChecker.GetCardPower(card) < power & (card != Cards.h_k))
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
                MyTurn = false;
                string cardType = CardChecker.GetCardType(bartyiah.CardsOnTable[0]);
                //هل مع اللاعب ورق من هذا النوع ؟ 
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
                    //الأوراق من هذا النوع على الطاولة
                    List<Cards> onTable = new List<Cards>();
                    foreach (Cards crd in bartyiah.CardsOnTable)
                    {
                        if (CardChecker.GetCardType(crd) == cardType)
                        {
                            onTable.Add(crd);
                        }
                    }
                    //أقوى ورقة من المأكولات
                    int powerCard = CardChecker.GetPowerfullCard(onTable);
                    //دوّر على أقوى ورقة موجودة بالمجموعة أضعف من أقوى ورقة من المأكولات منشان نتخلص منها
                    int power = 0;
                    int i = 0;
                    List<Cards> weekList = new List<Cards>();
                    foreach (Cards card in availableToPlay)
                    {
                        if (CardChecker.GetCardPower(card) > power)
                        {
                            power = CardChecker.GetCardPower(card);
                            weekList.Add(card);
                        }
                        i++;
                    }
                    for (int j = 0; j < weekList.Count; j++)
                    {
                        if (CardChecker.GetCardPower(weekList[j]) > powerCard)
                        {
                            weekList.RemoveAt(j);
                            j = -1;
                        }
                    }
                    if (weekList.Count > 0)
                    {
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(weekList[weekList.Count - 1]);
                        player.LastPlayedCard = weekList[weekList.Count - 1];
                        player.CardsOnHand.Remove(weekList[weekList.Count - 1]);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    }
                    else
                    {
                        Cards card = CardChecker.GetWeeknessCardOfType(cardType, availableToPlay);
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(card);
                        player.LastPlayedCard = card;
                        player.CardsOnHand.Remove(card);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    }
                }
                else//ما معو من هالنوع، دور على أقوى ورقة و لعاب عشوائياً
                {

                    //دوّر على نوع مقطش و قطش عليه منشان ينزل الختيار عليه...
                    if (CardChecker.GetCardCountByType("d", player.CardsOnHand) < 3)
                    {
                        //مقطش عالكوبة، لازم يكون معو ورقة أقل شي
                        if (CardChecker.GetCardCountByType("d", player.CardsOnHand) > 0)
                        {
                            //لعاب الورقة و مشي الدور
                            bartyiah.CardsOnTable.Add(CardChecker.GetPowerfullCardOfType("d", player.CardsOnHand));
                            player.LastPlayedCard = CardChecker.GetPowerfullCardOfType("d", player.CardsOnHand);
                            player.CardsOnHand.Remove(CardChecker.GetPowerfullCardOfType("d", player.CardsOnHand));
                            //advance index
                            bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                            PlayedDiamonds = false;
                            return;
                        }
                    }
                    if (CardChecker.GetCardCountByType("c", player.CardsOnHand) < 3)
                    {
                        //مقطش ، لازم يكون معو ورقة أقل شي
                        if (CardChecker.GetCardCountByType("c", player.CardsOnHand) > 0)
                        {
                            //لعاب الورقة و مشي الدور
                            bartyiah.CardsOnTable.Add(CardChecker.GetPowerfullCardOfType("c", player.CardsOnHand));
                            player.LastPlayedCard = CardChecker.GetPowerfullCardOfType("c", player.CardsOnHand);
                            player.CardsOnHand.Remove(CardChecker.GetPowerfullCardOfType("c", player.CardsOnHand));
                            //advance index
                            bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                            PlayedDiamonds = false;
                            return;
                        }
                    }
                    if (CardChecker.GetCardCountByType("s", player.CardsOnHand) < 3)
                    {
                        //مقطش ، لازم يكون معو ورقة أقل شي
                        if (CardChecker.GetCardCountByType("s", player.CardsOnHand) > 0)
                        {
                            //لعاب الورقة و مشي الدور
                            bartyiah.CardsOnTable.Add(CardChecker.GetPowerfullCardOfType("s", player.CardsOnHand));
                            player.LastPlayedCard = CardChecker.GetPowerfullCardOfType("s", player.CardsOnHand);
                            player.CardsOnHand.Remove(CardChecker.GetPowerfullCardOfType("s", player.CardsOnHand));
                            //advance index
                            bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                            PlayedDiamonds = false;
                            return;
                        }
                    }

                    //وصل لهون معناها مانو مقطش عشي ، دوّر على أقوى ورقة أما إذا كنت الختيار فلعبها إجباري
                    int power = 12;
                    int index = 0;
                    int i = 0;
                    foreach (Cards card in player.CardsOnHand)
                    {
                        if (card == Cards.h_k)
                        {
                            index = i;
                            break;
                        }
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
            }
        }
        public void Play_Ltouch()
        {
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
                    //الأوراق من هذا النوع على الطاولة
                    List<Cards> onTable = new List<Cards>();
                    foreach (Cards crd in bartyiah.CardsOnTable)
                    {
                        if (CardChecker.GetCardType(crd) == cardType)
                        {
                            onTable.Add(crd);
                        }
                    }
                    //أقوى ورقة من المأكولات
                    int powerCard = CardChecker.GetPowerfullCard(onTable);
                    //دوّر على أقوى ورقة موجودة بالمجموعة أضعف من أقوى ورقة من المأكولات منشان نتخلص منها
                    int power = 0;
                    int i = 0;
                    List<Cards> weekList = new List<Cards>();
                    foreach (Cards card in availableToPlay)
                    {
                        if (CardChecker.GetCardPower(card) > power)
                        {
                            power = CardChecker.GetCardPower(card);
                            weekList.Add(card);
                        }
                        i++;
                    }
                    for (int j = 0; j < weekList.Count; j++)
                    {
                        if (CardChecker.GetCardPower(weekList[j]) > powerCard)
                        {
                            weekList.RemoveAt(j);
                            j = -1;
                        }
                    }
                    if (weekList.Count > 0)
                    {
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(weekList[weekList.Count - 1]);
                        player.LastPlayedCard = weekList[weekList.Count - 1];
                        player.CardsOnHand.Remove(weekList[weekList.Count - 1]);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    }
                    else
                    {
                        Cards card = CardChecker.GetWeeknessCardOfType(cardType, availableToPlay);
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(card);
                        player.LastPlayedCard = card;
                        player.CardsOnHand.Remove(card);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    }
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
            bartyiah.FREEZTime = 60;//freez one second to let players see what happened
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
                    //الأوراق من هذا النوع على الطاولة
                    List<Cards> onTable = new List<Cards>();
                    foreach (Cards crd in bartyiah.CardsOnTable)
                    {
                        if (CardChecker.GetCardType(crd) == cardType)
                        {
                            onTable.Add(crd);
                        }
                    }
                    //أقوى ورقة من المأكولات
                    int powerCard = CardChecker.GetPowerfullCard(onTable);
                    //معو بنت من هدا النوع ؟
                    //فيك ما تحط الكود هون بس هدا بيسرع عملية التفكير شوية
                    if (CardChecker.IsCardExistInCollection("q", availableToPlay))
                    {
                        //لعاب البنت بدون تفكير و مشي الدور بس لازم تكون البنت مو أكيلة
                        if (powerCard > CardChecker.GetCardPower(Cards.c_q))//بنت بشكل عام إلها نفس القوة
                        {
                            bartyiah.CardsOnTable.Add(CardChecker.GetCard("q", availableToPlay));
                            player.LastPlayedCard = CardChecker.GetCard("q", availableToPlay);
                            player.CardsOnHand.Remove(CardChecker.GetCard("q", availableToPlay));
                            //advance index
                            bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                            return;
                        }
                        else
                        {
                            //شيت، البنت أكيلة هون، لازم نغير إسلوب اللعب ...
                        }
                    }
                    //ما معو بنت، طيب، لعاب المعتاد
                    //دوّر على أقوى ورقة موجودة بالمجموعة أضعف من أقوى ورقة من المأكولات منشان نتخلص منها
                    int power = 0;
                    int i = 0;
                    List<Cards> weekList = new List<Cards>();
                    foreach (Cards card in availableToPlay)
                    {
                        if (CardChecker.GetCardPower(card) > power)
                        {
                            power = CardChecker.GetCardPower(card);
                            weekList.Add(card);
                        }
                        i++;
                    }
                    for (int j = 0; j < weekList.Count; j++)
                    {
                        if (CardChecker.GetCardPower(weekList[j]) > powerCard)
                        {
                            weekList.RemoveAt(j);
                            j = -1;
                        }
                    }
                    if (weekList.Count > 0)
                    {
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(weekList[weekList.Count - 1]);
                        player.LastPlayedCard = weekList[weekList.Count - 1];
                        player.CardsOnHand.Remove(weekList[weekList.Count - 1]);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    }
                    else
                    {
                        Cards card = CardChecker.GetWeeknessCardOfType(cardType, availableToPlay);
                        //لعاب الورقة و مشي الدور
                        bartyiah.CardsOnTable.Add(card);
                        player.LastPlayedCard = card;
                        player.CardsOnHand.Remove(card);
                        //advance index
                        bartyiah.playerIndex = (bartyiah.playerIndex + 1) % 4;
                    }
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
        //we will use this event to make forbiden types
        void bartyiah_CardsAte(object sender, EatArgs e)
        {
            switch (bartyiah.PlayMode)
            {
                case PlayMode.Diamonds:
                    if (MyTurn)//هذه الأوراق مأكولة بعدما كان اللاعب مستلم اللعب
                    {
                        if (!PlayedDiamonds)//الزلمة ما لعب ديناري
                        {
                            if (e.PlayerName == player.Name)
                            {
                                //حدا طعماني دينار ؟
                                if (CardChecker.IsCardTypeExistInCollection("d", e.AteCards))
                                {
                                    //معناها هدا النوع  من الورق محرم
                                    bool found = false;
                                    foreach (string card in ForbiddenTypes)
                                    {
                                        if (CardChecker.GetCardType(e.AteCards[0]) == card)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        ForbiddenTypes.Add(CardChecker.GetCardType(e.AteCards[0]));
                                    }
                                }
                            }
                        }
                    }
                    break;
                case PlayMode.KingOfHearts:
                    //ما في داعي لأنو أني نوع محرم رح يطنبرو بالختيار أما إذا الختيار معو فما بيهم
                    break;
                case PlayMode.Ltoosh:
                    //ما في داعي لأنو أني نوع محرم رح ينئكل سواء كان هو أبو اللطش أما غيرو
                    break;
                case PlayMode.Queens:
                    //ما في داعي لأنو البنت رح تنزل شاء أم أبا
                    break;
                case PlayMode.Trix:
                    //ما في لطش هنا
                    break;
            }
        }
        bool IsForbeddinType(string cardType)
        {
            foreach (string card in ForbiddenTypes)
            {
                if (cardType == card)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
