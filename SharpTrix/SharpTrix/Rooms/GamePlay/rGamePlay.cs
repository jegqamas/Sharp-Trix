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
    public class rGamePlay : Microsoft.Xna.Framework.GameComponent
    {
        bool Pressed = false;
        int countdown = 0;
        SpriteFont Font_large;
        SpriteFont Font_small;
        public TrixBartyiah trixBartyiah;
        public bool IsPlaying = false;
        bool IsLoading = false;//set true when the game is loading textures
        //others
        int cardWidth = 50;
        int cardHeight = 90;
        int cardLatchedWidth = 15;
        int cardLatchedHeight = 20;
        int cardOffset = 2;//which part of card you want to show (2= 1/2, 3= 1/3 ...)
        int cardSpace = 1;//the space between cards
        int OnTableCardSpace = 65;//the space between cards on table
        int OffScreen = 30;
        int SelectedCardIndex = 0;

        rUserNaming rUserNaming;
        rScore rScore;
        rGameOverScore rGameOverScore;
        //effects
        bool drawEffect = false;
        int effectTimer = 0;
        DrawEffect draweffect;
        //Sound effects
        SoundEffect seCardDrop;
        SoundEffect seCardLatche;
        SoundEffect seFata;
        #region Textures
        Texture2D tBackground;
        Texture2D tPaper;
        Texture2D tFrame;
        Texture2D tIcon_ltoosh;
        Texture2D tIcon_d;
        Texture2D tIcon_k;
        Texture2D tIcon_q;
        Texture2D tIcon_t;

        Texture2D hand_d;
        Texture2D hand_u;
        Texture2D hand_l;
        Texture2D hand_r;
        Texture2D[] cardTextures;
        #endregion

        public rGamePlay(Game game)
            : base(game)
        {
            rUserNaming = new rUserNaming(game, this);
            rScore = new rScore(game, this);
            rGameOverScore = new rGameOverScore(game, this);
        }
        public void LoadResources()
        {
            IsLoading = true;
            Font_large = base.Game.Content.Load<SpriteFont>(@"Fonts\FontLarge");
            Font_small = base.Game.Content.Load<SpriteFont>(@"Fonts\FontSmall");
            seCardDrop = base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\CardDrop");
            seCardLatche = base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\CardLatche");
            seFata = base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\Fata");
            #region Textures
            tFrame = base.Game.Content.Load<Texture2D>(@"Backgrounds\frame");
            tBackground = base.Game.Content.Load<Texture2D>(@"Backgrounds\GreenTableBackground");
            tPaper = base.Game.Content.Load<Texture2D>(@"Backgrounds\PeiceOfPaper");
            tIcon_ltoosh = base.Game.Content.Load<Texture2D>(@"PlayModeIcons\Latches");
            tIcon_d = base.Game.Content.Load<Texture2D>(@"PlayModeIcons\D");
            tIcon_k = base.Game.Content.Load<Texture2D>(@"PlayModeIcons\KH");
            tIcon_q = base.Game.Content.Load<Texture2D>(@"PlayModeIcons\Q");
            tIcon_t = base.Game.Content.Load<Texture2D>(@"PlayModeIcons\Trix");
            hand_d = base.Game.Content.Load<Texture2D>(@"Items\PointHand_Down");
            hand_u = base.Game.Content.Load<Texture2D>(@"Items\PointHand_Up");
            hand_l = base.Game.Content.Load<Texture2D>(@"Items\PointHand_Left");
            hand_r = base.Game.Content.Load<Texture2D>(@"Items\PointHand_Right");
            cardTextures = new Texture2D[53];
            for (int i = 0; i < 53; i++)
            {
                if (i == 0)
                {
                    cardTextures[i] = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder + "backside");
                    cardTextures[i].Name = "backside";
                }
                else
                {
                    cardTextures[i] = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder + CardChecker.CardNames[i - 1]);
                    cardTextures[i].Name = CardChecker.CardNames[i - 1];
                }
            }
            #endregion
            IsLoading = false;
        }

        Texture2D GetCardTexture(Cards card)
        {
            for (int i = 0; i < 53; i++)
            {
                if (card.ToString() == cardTextures[i].Name)
                    return cardTextures[i];
            }
            return null;
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
            if (IsLoading)
            {
                return;//no update while loading !!
            }
            #region None game buttons
            if (!Pressed)
            {
                countdown = 5;
                //in game menu
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    ((TrixCore)base.Game).Room = CurrentRoom.InGameMenu;
                    IsPlaying = false;
                    Pressed = true;
                }
                //replay trix soundtrack
                if (Keyboard.GetState().IsKeyDown(Keys.F1))
                {
                    ((TrixCore)base.Game).PlayMusic(base.Game.Content.Load<Song>(@"Sounds\Music\soundTrack"));
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
            #endregion
            if (trixBartyiah.GAMEOVER)
            {
                rGameOverScore.Update(gameTime); IsPlaying = false;
            }
            if (IsPlaying)
            {
                trixBartyiah.Update();
                if (trixBartyiah.DrawStatus == DrawStatus.UserNaming)
                {
                    rUserNaming.Update(gameTime);
                }
                else if (trixBartyiah.DrawStatus == DrawStatus.EndGameMode)
                {
                    rScore.Update(gameTime);
                }

                //if player turn, let the user choose a card
                if (trixBartyiah.PlayerTurn)
                {
                    bool canDouble = true;
                    bool AutoPass = true;
                    #region Mouse
                    MouseState ms = Mouse.GetState();
                    int x = base.Game.GraphicsDevice.Viewport.Width / 2 - 15;
                    if (ms.Y > base.Game.GraphicsDevice.Viewport.Height - (cardHeight + 20))
                    {
                        SelectedCardIndex = (ms.X - x + (((trixBartyiah.Player1.CardsOnHand.Count * (cardWidth / cardOffset + cardSpace)) / 2)))
                            / (cardWidth / cardOffset + cardSpace);
                        if (SelectedCardIndex < 0)
                            SelectedCardIndex = 0;
                        if (SelectedCardIndex > trixBartyiah.Player1.CardsOnHand.Count - 1)
                            SelectedCardIndex = trixBartyiah.Player1.CardsOnHand.Count - 1;
                    }
                    #endregion
                    #region AUTO PASS (TRIX MODE ONLY)
                    //‘Ê› ≈–« ·«“„ Ì⁄„· √Ê Ê »«’
                    if (trixBartyiah.PlayMode == PlayMode.Trix)
                    {
                        foreach (Cards crd in trixBartyiah.Player1.CardsOnHand)
                        {
                            switch (CardChecker.GetCardType(crd))
                            {
                                case "h"://ﬂÊ»«
                                    if (trixBartyiah.CardsOnTable_Trix_H.Count > 0)
                                    {
                                        //≈–« ﬂ«‰  √÷⁄› „‰ «Ê· Ê—ﬁ…
                                        if (CardChecker.Trix_GetIndex(crd) <
                                            CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_H[0]))
                                        {
                                            //·«“„  ﬂÊ‰ »«· — Ì»
                                            if (CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_H[0]) -
                                                CardChecker.Trix_GetIndex(crd) == 1)
                                            {
                                                AutoPass = false; break;
                                            }
                                        }
                                        //≈–« ﬂ«‰  √ﬁÊÏ „‰ ¬Œ— Ê—ﬁ…
                                        else if (CardChecker.Trix_GetIndex(crd) >
                                                 CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_H[trixBartyiah.CardsOnTable_Trix_H.Count - 1]))
                                        {
                                            //·«“„  ﬂÊ‰ »«· — Ì»
                                            if (CardChecker.Trix_GetIndex(crd) -
                                            CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_H[trixBartyiah.CardsOnTable_Trix_H.Count - 1]) == 1)
                                            {
                                                AutoPass = false; break;
                                            }
                                        }
                                    }
                                    else//·«“„ Ì·⁄» ‘»
                                    {
                                        if (trixBartyiah.Player1.IsCardExists(Cards.h_j))
                                        {
                                            AutoPass = false; break;
                                        }
                                    }
                                    break;
                                case "d"://œÌ‰«—Ì
                                    if (trixBartyiah.CardsOnTable_Trix_D.Count > 0)
                                    {
                                        //≈–« ﬂ«‰  √÷⁄› „‰ «Ê· Ê—ﬁ…
                                        if (CardChecker.Trix_GetIndex(crd) <
                                            CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_D[0]))
                                        {
                                            //·«“„  ﬂÊ‰ »«· — Ì»
                                            if (CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_D[0]) -
                                                CardChecker.Trix_GetIndex(crd) == 1)
                                            {
                                                AutoPass = false; break;
                                            }
                                        }
                                        //≈–« ﬂ«‰  √ﬁÊÏ „‰ ¬Œ— Ê—ﬁ…
                                        else if (CardChecker.Trix_GetIndex(crd) >
                                                 CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_D[trixBartyiah.CardsOnTable_Trix_D.Count - 1]))
                                        {
                                            //·«“„  ﬂÊ‰ »«· — Ì»
                                            if (CardChecker.Trix_GetIndex(crd) -
                                            CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_D[trixBartyiah.CardsOnTable_Trix_D.Count - 1]) == 1)
                                            {
                                                AutoPass = false; break;
                                            }
                                        }
                                    }
                                    else//·«“„ Ì·⁄» ‘»
                                    {
                                        if (trixBartyiah.Player1.IsCardExists(Cards.d_j))
                                        {
                                            AutoPass = false; break;
                                        }
                                    }
                                    break;
                                case "c"://„€“·«‰Ì
                                    if (trixBartyiah.CardsOnTable_Trix_C.Count > 0)
                                    {
                                        //≈–« ﬂ«‰  √÷⁄› „‰ «Ê· Ê—ﬁ…
                                        if (CardChecker.Trix_GetIndex(crd) <
                                            CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_C[0]))
                                        {
                                            //·«“„  ﬂÊ‰ »«· — Ì»
                                            if (CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_C[0]) -
                                                CardChecker.Trix_GetIndex(crd) == 1)
                                            {
                                                AutoPass = false; break;
                                            }
                                        }
                                        //≈–« ﬂ«‰  √ﬁÊÏ „‰ ¬Œ— Ê—ﬁ…
                                        else if (CardChecker.Trix_GetIndex(crd) >
                                                 CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_C[trixBartyiah.CardsOnTable_Trix_C.Count - 1]))
                                        {
                                            //·«“„  ﬂÊ‰ »«· — Ì»
                                            if (CardChecker.Trix_GetIndex(crd) -
                                            CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_C[trixBartyiah.CardsOnTable_Trix_C.Count - 1]) == 1)
                                            {
                                                AutoPass = false; break;
                                            }
                                        }
                                    }
                                    else//·«“„ Ì·⁄» ‘»
                                    {
                                        if (trixBartyiah.Player1.IsCardExists(Cards.c_j))
                                        {
                                            AutoPass = false; break;
                                        }
                                    }
                                    break;
                                case "s"://”»«œ
                                    if (trixBartyiah.CardsOnTable_Trix_S.Count > 0)
                                    {
                                        //≈–« ﬂ«‰  √÷⁄› „‰ «Ê· Ê—ﬁ…
                                        if (CardChecker.Trix_GetIndex(crd) <
                                            CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_S[0]))
                                        {
                                            //·«“„  ﬂÊ‰ »«· — Ì»
                                            if (CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_S[0]) -
                                                CardChecker.Trix_GetIndex(crd) == 1)
                                            {
                                                AutoPass = false; break;
                                            }
                                        }
                                        //≈–« ﬂ«‰  √ﬁÊÏ „‰ ¬Œ— Ê—ﬁ…
                                        else if (CardChecker.Trix_GetIndex(crd) >
                                                 CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_S[trixBartyiah.CardsOnTable_Trix_S.Count - 1]))
                                        {
                                            //·«“„  ﬂÊ‰ »«· — Ì»
                                            if (CardChecker.Trix_GetIndex(crd) -
                                            CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_S[trixBartyiah.CardsOnTable_Trix_S.Count - 1]) == 1)
                                            {
                                                AutoPass = false; break;
                                            }
                                        }
                                    }
                                    else//·«“„ Ì·⁄» ‘»
                                    {
                                        if (trixBartyiah.Player1.IsCardExists(Cards.s_j))
                                        {
                                            AutoPass = false; break;
                                        }
                                    }
                                    break;
                            }
                            if (!AutoPass)
                                break;
                        }
                        if (AutoPass & trixBartyiah.Player1.CardsOnHand.Count > 0)
                        {
                            ((TrixCore)base.Game).DrawStatusString("PRESS SPACE TO PASS, NO CARD TO PLAY !!", 60);
                        }
                    }
                    #endregion
                    #region Can Double ? (Doubling Mode Only)
                    if (trixBartyiah.PlayStatus == PlayStatus.Doubling)
                    {
                        switch (trixBartyiah.PlayMode)
                        {
                            case PlayMode.KingOfHearts:
                                if (!trixBartyiah.Player1.IsCardExists(Cards.h_k))
                                    canDouble = false;
                                break;
                            case PlayMode.Queens:
                                if (CardChecker.GetCardCountByName("q", trixBartyiah.Player1.CardsOnHand) == 0)
                                    canDouble = false;
                                break;
                        }
                        if (!canDouble)
                        {
                            ((TrixCore)base.Game).DrawStatusString("PRESS SPACE TO PASS, YOU CAN'T DOUBLE ANY CARD", 60);
                        }
                        else
                        {
                            ((TrixCore)base.Game).DrawStatusString("PLEASE CHOOSE DOUBLE CARD, PRESS SPACE WHEN DONE", 60);
                        }
                    }
                    #endregion
                    if (!Pressed)
                    {
                        countdown = 5;
                        if (Keyboard.GetState().IsKeyDown(Keys.Right))
                        {
                            SelectedCardIndex++; ;
                            if (SelectedCardIndex > trixBartyiah.Player1.CardsOnHand.Count - 1)
                                SelectedCardIndex = trixBartyiah.Player1.CardsOnHand.Count - 1;
                            Pressed = true;
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.Left))
                        {
                            SelectedCardIndex--;
                            if (SelectedCardIndex < 0)
                                SelectedCardIndex = 0;
                            Pressed = true;
                        }
                        //Action
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter) | ((ms.LeftButton == ButtonState.Pressed)
                            & ((ms.Y > base.Game.GraphicsDevice.Viewport.Height - (cardHeight + 20)))))
                        {
                            DoAction();
                            Pressed = true;
                        }
                        if ( ((ms.RightButton == ButtonState.Pressed)
                           & ((ms.Y > base.Game.GraphicsDevice.Viewport.Height - (cardHeight + 20)))))
                        {
                            DoublingCancel();
                            Pressed = true;
                        }
                        if (trixBartyiah.PlayMode == PlayMode.Trix)
                        {
                            //AUTO PASS
                            if (trixBartyiah.Player1.CardsOnHand.Count == 0)
                            {
                                //advance index
                                trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                trixBartyiah.PlayerTurn = false;
                                trixBartyiah.FREEZTime = 60;
                                Pressed = true;
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.Space) & AutoPass)
                            {
                                //advance index
                                trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                trixBartyiah.PlayerTurn = false;
                                trixBartyiah.FREEZTime = 60;
                                AutoPass = false;
                                Pressed = true;
                            }
                        }
                        //doubling
                        if (trixBartyiah.PlayStatus == PlayStatus.Doubling)
                        {
                            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                            {
                                trixBartyiah.PlayStatus = PlayStatus.PlayingMode;
                                trixBartyiah.HOLD = false;
                                trixBartyiah.PlayerTurn = false;
                                trixBartyiah.FREEZTime = 60;
                                Pressed = true;
                            }
                        }
                    }
                    else
                    {
                        if (countdown > 0)
                            countdown--;
                        else
                            Pressed = false;
                    }
                }

            }
            base.Update(gameTime);
        }
        //User play done here
        void DoAction()
        {
            switch (trixBartyiah.PlayMode)
            {
                #region Latches & Queens
                case PlayMode.Ltoosh:
                case PlayMode.Queens:
                case PlayMode.KingOfHearts:
                case PlayMode.Diamonds:
                    if (trixBartyiah.PlayStatus != PlayStatus.Doubling)
                    {
                        if (trixBartyiah.CardsOnTable.Count == 0)
                        {
                            //«··«⁄» „” ·„ «·œﬁ ›Ì Ì·⁄» «··Ì »œÊ Ì«Â
                            //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                            trixBartyiah.CardsOnTable.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                            trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                            trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                            //advance index
                            trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                            trixBartyiah.PlayerTurn = false;
                            trixBartyiah.FREEZTime = 60;
                            SelectedCardIndex = 0;
                            trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                        }
                        else
                        {
                            //«··«⁄» „Ê „” ·„ «·œﬁ ·«“„ Ì·⁄» «·‰Ê⁄ «·„ÿ·Ê»
                            if (CardChecker.IsCardTypeExistInCollection(
                                CardChecker.GetCardType(trixBartyiah.CardsOnTable[0]), trixBartyiah.Player1.CardsOnHand))
                            {
                                if (CardChecker.GetCardType(trixBartyiah.CardsOnTable[0]) ==
                                    CardChecker.GetCardType(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]))
                                {
                                    //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                    trixBartyiah.CardsOnTable.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                    trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                    trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                    //advance index
                                    trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                    trixBartyiah.PlayerTurn = false;
                                    trixBartyiah.FREEZTime = 60; SelectedCardIndex = 0;
                                    trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                }
                                else
                                {
                                    ((TrixCore)base.Game).DrawStatusString("You can't play this card", 60);
                                }
                            }
                            else //«··«⁄» „Ê „⁄Ê «·‰Ê⁄ «·„ÿ·Ê» ›Ì Ì·⁄» «··Ì »œÊ Ì«Â
                            {
                                //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                trixBartyiah.CardsOnTable.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                //advance index
                                trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                trixBartyiah.PlayerTurn = false;
                                trixBartyiah.FREEZTime = 60; SelectedCardIndex = 0;
                                trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                            }
                        }
                    }
                    else
                    {
                        switch (trixBartyiah.PlayMode)
                        {
                            case PlayMode.KingOfHearts:
                                if (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.h_k)
                                {
                                    if (!CardChecker.IsCardExistInCollection(Cards.h_k, trixBartyiah.Player1.DoublingCards))
                                        trixBartyiah.Player1.DoublingCards.Add(Cards.h_k);
                                    Pressed = true;
                                }
                                else
                                {
                                    ((TrixCore)base.Game).DrawStatusString("You can't double this card", 60);
                                }
                                break;
                            case PlayMode.Queens:
                                if ((trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.c_q) |
                                    (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.d_q) |
                                    (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.s_q) |
                                    (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.h_q))
                                {
                                    if (!CardChecker.IsCardExistInCollection(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex],
                                            trixBartyiah.Player1.DoublingCards))
                                        trixBartyiah.Player1.DoublingCards.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                    Pressed = true;
                                }
                                else
                                {
                                    ((TrixCore)base.Game).DrawStatusString("You can't double this card", 60);
                                }
                                break;
                        }
                    }
                    break;
                #endregion
                #region TRIX
                case PlayMode.Trix:
                    //‘Ê› ≈–« «·Ê—ﬁ… «··Ì ·⁄»Â« » ‰·⁄» ....
                    string cardType = CardChecker.GetCardType(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                    switch (cardType)
                    {
                        case "h"://ﬂÊ»«
                            if (trixBartyiah.CardsOnTable_Trix_H.Count > 0)
                            {
                                //≈–« ﬂ«‰  √÷⁄› „‰ «Ê· Ê—ﬁ…
                                if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) <
                                    CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_H[0]))
                                {
                                    //·«“„  ﬂÊ‰ »«· — Ì»
                                    if (CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_H[0]) -
                                        CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) == 1)
                                    {
                                        //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                        trixBartyiah.CardsOnTable_Trix_H.Insert(0, trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                        trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                        trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                        //advance index
                                        trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                        trixBartyiah.PlayerTurn = false;
                                        trixBartyiah.FREEZTime = 60;
                                        SelectedCardIndex = 0;
                                        trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                    }
                                }
                                //≈–« ﬂ«‰  √ﬁÊÏ „‰ ¬Œ— Ê—ﬁ…
                                else if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) >
                                         CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_H[trixBartyiah.CardsOnTable_Trix_H.Count - 1]))
                                {
                                    //·«“„  ﬂÊ‰ »«· — Ì»
                                    if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) -
                                    CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_H[trixBartyiah.CardsOnTable_Trix_H.Count - 1]) == 1)
                                    {
                                        //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                        trixBartyiah.CardsOnTable_Trix_H.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                        trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                        trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                        //advance index
                                        trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                        trixBartyiah.PlayerTurn = false;
                                        trixBartyiah.FREEZTime = 60;
                                        SelectedCardIndex = 0;
                                        trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                    }
                                }
                            }
                            else//·«“„  ﬂÊ‰ ‘»
                            {
                                if (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.h_j)
                                {
                                    //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                    trixBartyiah.CardsOnTable_Trix_H.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                    trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                    trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                    //advance index
                                    trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                    trixBartyiah.PlayerTurn = false;
                                    trixBartyiah.FREEZTime = 60;
                                    SelectedCardIndex = 0;
                                    trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                }
                            }
                            break;
                        case "d"://œÌ‰«—Ì
                            if (trixBartyiah.CardsOnTable_Trix_D.Count > 0)
                            {
                                //≈–« ﬂ«‰  √÷⁄› „‰ «Ê· Ê—ﬁ…
                                if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) <
                                    CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_D[0]))
                                {
                                    //·«“„  ﬂÊ‰ »«· — Ì»
                                    if (CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_D[0]) -
                                        CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) == 1)
                                    {
                                        //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                        trixBartyiah.CardsOnTable_Trix_D.Insert(0, trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                        trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                        trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                        //advance index
                                        trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                        trixBartyiah.PlayerTurn = false;
                                        trixBartyiah.FREEZTime = 60;
                                        SelectedCardIndex = 0;
                                        trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                    }
                                }
                                //≈–« ﬂ«‰  √ﬁÊÏ „‰ ¬Œ— Ê—ﬁ…
                                else if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) >
                                         CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_D[trixBartyiah.CardsOnTable_Trix_D.Count - 1]))
                                {
                                    //·«“„  ﬂÊ‰ »«· — Ì»
                                    if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) -
                                    CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_D[trixBartyiah.CardsOnTable_Trix_D.Count - 1]) == 1)
                                    {
                                        //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                        trixBartyiah.CardsOnTable_Trix_D.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                        trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                        trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                        //advance index
                                        trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                        trixBartyiah.PlayerTurn = false;
                                        trixBartyiah.FREEZTime = 60;
                                        SelectedCardIndex = 0;
                                        trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                    }
                                }
                            }
                            else//·«“„  ﬂÊ‰ ‘»
                            {
                                if (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.d_j)
                                {
                                    //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                    trixBartyiah.CardsOnTable_Trix_D.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                    trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                    trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                    //advance index
                                    trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                    trixBartyiah.PlayerTurn = false;
                                    trixBartyiah.FREEZTime = 60;
                                    SelectedCardIndex = 0;
                                    trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                }
                            }
                            break;
                        case "c"://„€“·«‰Ì
                            if (trixBartyiah.CardsOnTable_Trix_C.Count > 0)
                            {
                                //≈–« ﬂ«‰  √÷⁄› „‰ «Ê· Ê—ﬁ…
                                if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) <
                                    CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_C[0]))
                                {
                                    //·«“„  ﬂÊ‰ »«· — Ì»
                                    if (CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_C[0]) -
                                        CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) == 1)
                                    {
                                        //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                        trixBartyiah.CardsOnTable_Trix_C.Insert(0, trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                        trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                        trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                        //advance index
                                        trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                        trixBartyiah.PlayerTurn = false;
                                        trixBartyiah.FREEZTime = 60;
                                        SelectedCardIndex = 0;
                                        trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                    }
                                }
                                //≈–« ﬂ«‰  √ﬁÊÏ „‰ ¬Œ— Ê—ﬁ…
                                else if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) >
                                         CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_C[trixBartyiah.CardsOnTable_Trix_C.Count - 1]))
                                {
                                    //·«“„  ﬂÊ‰ »«· — Ì»
                                    if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) -
                                    CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_C[trixBartyiah.CardsOnTable_Trix_C.Count - 1]) == 1)
                                    {
                                        //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                        trixBartyiah.CardsOnTable_Trix_C.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                        trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                        trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                        //advance index
                                        trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                        trixBartyiah.PlayerTurn = false;
                                        trixBartyiah.FREEZTime = 60;
                                        SelectedCardIndex = 0;
                                        trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                    }
                                }
                            }
                            else//·«“„  ﬂÊ‰ ‘»
                            {
                                if (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.c_j)
                                {
                                    //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                    trixBartyiah.CardsOnTable_Trix_C.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                    trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                    trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                    //advance index
                                    trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                    trixBartyiah.PlayerTurn = false;
                                    trixBartyiah.FREEZTime = 60;
                                    SelectedCardIndex = 0;
                                    trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                }
                            } break;
                        case "s"://”»«œ
                            if (trixBartyiah.CardsOnTable_Trix_S.Count > 0)
                            {
                                //≈–« ﬂ«‰  √÷⁄› „‰ «Ê· Ê—ﬁ…
                                if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) <
                                    CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_S[0]))
                                {
                                    //·«“„  ﬂÊ‰ »«· — Ì»
                                    if (CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_S[0]) -
                                        CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) == 1)
                                    {
                                        //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                        trixBartyiah.CardsOnTable_Trix_S.Insert(0, trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                        trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                        trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                        //advance index
                                        trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                        trixBartyiah.PlayerTurn = false;
                                        trixBartyiah.FREEZTime = 60;
                                        SelectedCardIndex = 0;
                                        trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                    }
                                }
                                //≈–« ﬂ«‰  √ﬁÊÏ „‰ ¬Œ— Ê—ﬁ…
                                else if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) >
                                         CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_S[trixBartyiah.CardsOnTable_Trix_S.Count - 1]))
                                {
                                    //·«“„  ﬂÊ‰ »«· — Ì»
                                    if (CardChecker.Trix_GetIndex(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]) -
                                    CardChecker.Trix_GetIndex(trixBartyiah.CardsOnTable_Trix_S[trixBartyiah.CardsOnTable_Trix_S.Count - 1]) == 1)
                                    {
                                        //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                        trixBartyiah.CardsOnTable_Trix_S.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                        trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                        trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                        //advance index
                                        trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                        trixBartyiah.PlayerTurn = false;
                                        trixBartyiah.FREEZTime = 60;
                                        SelectedCardIndex = 0;
                                        trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                    }
                                }
                            }
                            else//·«“„  ﬂÊ‰ ‘»
                            {
                                if (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.s_j)
                                {
                                    //·⁄«» «·Ê—ﬁ… Ê „‘Ì «·œÊ—
                                    trixBartyiah.CardsOnTable_Trix_S.Add(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                                    trixBartyiah.Player1.LastPlayedCard = trixBartyiah.Player1.CardsOnHand[SelectedCardIndex];
                                    trixBartyiah.Player1.CardsOnHand.RemoveAt(SelectedCardIndex);
                                    //advance index
                                    trixBartyiah.playerIndex = (trixBartyiah.playerIndex + 1) % 4;
                                    trixBartyiah.PlayerTurn = false;
                                    trixBartyiah.FREEZTime = 60;
                                    SelectedCardIndex = 0;
                                    trixBartyiah.RiseEffect(new EffectArgs(DrawEffect.Player1Play));
                                }
                            }
                            break;
                    }
                    if (trixBartyiah.Player1.CardsOnHand.Count == 0)
                    {
                        trixBartyiah.TrixFinishList.Add(0);
                        trixBartyiah.playerIndex = 0;
                    }
                    break;
                #endregion
            }
        }
        void DoublingCancel()
        {
            if (trixBartyiah.PlayStatus == PlayStatus.Doubling)
            {
                switch (trixBartyiah.PlayMode)
                {
                    case PlayMode.KingOfHearts:
                        if (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.h_k)
                        {
                            if (CardChecker.IsCardExistInCollection(Cards.h_k, trixBartyiah.Player1.DoublingCards))
                                trixBartyiah.Player1.DoublingCards.Remove(Cards.h_k);
                            Pressed = true;
                        }
                
                        break;
                    case PlayMode.Queens:
                        if ((trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.c_q) |
                            (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.d_q) |
                            (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.s_q) |
                            (trixBartyiah.Player1.CardsOnHand[SelectedCardIndex] == Cards.h_q))
                        {
                            if (CardChecker.IsCardExistInCollection(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex],
                                    trixBartyiah.Player1.DoublingCards))
                                trixBartyiah.Player1.DoublingCards.Remove(trixBartyiah.Player1.CardsOnHand[SelectedCardIndex]);
                            Pressed = true;
                        }
                        break;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draw background
            spriteBatch.Draw(tBackground,
            new Rectangle(0, 0, base.Game.GraphicsDevice.Viewport.Width,
            base.Game.GraphicsDevice.Viewport.Height), Color.White);
            if (IsLoading)
            {
                spriteBatch.DrawString(Font_large, "LOADING, PLEASE WAIT",
             new Vector2(base.Game.GraphicsDevice.Viewport.Width / 2 -
                 (Font_large.MeasureString("LOADING, PLEASE WAIT").X / 2),
                 base.Game.GraphicsDevice.Viewport.Height / 2 -
                 (Font_large.MeasureString("LOADING, PLEASE WAIT").Y / 2)), Color.White);
                return;
            }
            if (trixBartyiah.GAMEOVER)
            {
                rGameOverScore.Draw(spriteBatch); IsPlaying = false;
            }
            if (IsPlaying)
            {
                //Draw status
                if (trixBartyiah.DrawStatus == DrawStatus.StatusLabel)
                {
                    spriteBatch.DrawString(Font_large, trixBartyiah.StatusLabel,
                        new Vector2(base.Game.GraphicsDevice.Viewport.Width / 2 -
                            (Font_large.MeasureString(trixBartyiah.StatusLabel).X / 2),
                            base.Game.GraphicsDevice.Viewport.Height / 2 -
                            (Font_large.MeasureString(trixBartyiah.StatusLabel).Y / 2)), Color.White);
                }
                //naming
                else if (trixBartyiah.DrawStatus == DrawStatus.UserNaming)
                {
                    rUserNaming.Draw(spriteBatch);
                }
                else if (trixBartyiah.DrawStatus == DrawStatus.EndGameMode)
                {
                    rScore.Draw(spriteBatch);
                }
                else if ((trixBartyiah.DrawStatus == DrawStatus.EatTable) |
                    (trixBartyiah.DrawStatus == DrawStatus.TrixTable))
                {
                    #region Draw scores and other shit
                    //The current Mode ...
                    spriteBatch.Draw(tPaper, new Rectangle(5, 5, 150, 70), Color.White);
                    switch (trixBartyiah.PlayMode)
                    {
                        case PlayMode.Diamonds:
                            spriteBatch.Draw(tIcon_d, new Rectangle(10, 30, 45, 35), Color.White);
                            break;
                        case PlayMode.KingOfHearts:
                            spriteBatch.Draw(tIcon_k, new Rectangle(10, 30, 45, 35), Color.White);
                            break;
                        case PlayMode.Ltoosh:
                            spriteBatch.Draw(tIcon_ltoosh, new Rectangle(10, 30, 45, 35), Color.White);
                            break;
                        case PlayMode.Queens:
                            spriteBatch.Draw(tIcon_q, new Rectangle(10, 30, 45, 35), Color.White);
                            break;
                        case PlayMode.Trix:
                            spriteBatch.Draw(tIcon_t, new Rectangle(10, 30, 45, 35), Color.White);
                            break;
                    }
                    string namingNu = trixBartyiah.playedModes.Count.ToString();
                    switch (trixBartyiah.NamingCount)
                    {
                        case 1: namingNu += "st"; break;
                        case 2: namingNu += "nd"; break;
                        case 3: namingNu += "rd"; break;
                        default: namingNu += "th"; break;
                    }
                    spriteBatch.DrawString(Font_small, "Game:", new Vector2(65, 28), Color.Black);
                    spriteBatch.DrawString(Font_small, trixBartyiah.PlayMode.ToString(), new Vector2(65, 40), Color.Black);
                    spriteBatch.DrawString(Font_small, "(" + namingNu + ")", new Vector2(65, 52), Color.Black);
                    //Draw TURN HAND
                    int pindex = trixBartyiah.playerIndex;
                    if (trixBartyiah.CardsOnTable.Count == 4)
                    {
                        pindex -= 1;
                        if (pindex < 0)
                            pindex += 4;
                        pindex %= 4;
                    }
                    switch (pindex)
                    {
                        case 0://you
                            spriteBatch.Draw(hand_d, new Rectangle((base.Game.GraphicsDevice.Viewport.Width / 2) - (hand_d.Width / 2)
                                , base.Game.GraphicsDevice.Viewport.Height - 180, 40, 40), Color.White);
                            break;
                        case 1://player 2
                            spriteBatch.Draw(hand_r, new Rectangle(base.Game.GraphicsDevice.Viewport.Width - 180
                                , (base.Game.GraphicsDevice.Viewport.Height / 2) - (hand_r.Height / 2), 40, 40), Color.White);
                            break;
                        case 2://player 3
                            spriteBatch.Draw(hand_u, new Rectangle((base.Game.GraphicsDevice.Viewport.Width / 2) - (hand_u.Width / 2)
                                , 140, 40, 40), Color.White);
                            break;
                        case 3://player 4
                            spriteBatch.Draw(hand_l, new Rectangle(140
                                , (base.Game.GraphicsDevice.Viewport.Height / 2) - (hand_l.Height / 2), 40, 40), Color.White);
                            break;
                    }
                    #endregion
                }
                //Draw player cards
                #region player 1 (YOU)
                int x = base.Game.GraphicsDevice.Viewport.Width / 2 - 15;
                int y = base.Game.GraphicsDevice.Viewport.Height - (cardHeight + OffScreen);
                for (int i = 0; i < trixBartyiah.Player1.CardsOnHand.Count; i++)
                {
                    //calculate x position
                    int cardX = x - (((trixBartyiah.Player1.CardsOnHand.Count *
                        (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * i));
                    //draw card (show to user)
                    if (trixBartyiah.PlayStatus != PlayStatus.Doubling)
                    {
                        if (trixBartyiah.PlayerTurn & SelectedCardIndex == i)
                        {
                            spriteBatch.Draw(GetCardTexture(trixBartyiah.Player1.CardsOnHand[i]), new Rectangle(cardX, y - 20, cardWidth, cardHeight), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(GetCardTexture(trixBartyiah.Player1.CardsOnHand[i]), new Rectangle(cardX, y, cardWidth, cardHeight), Color.White);
                        }
                    }
                    else
                    {
                        if (CardChecker.IsCardExistInCollection(trixBartyiah.Player1.CardsOnHand[i], trixBartyiah.Player1.DoublingCards))
                            spriteBatch.Draw(GetCardTexture(trixBartyiah.Player1.CardsOnHand[i]), new Rectangle(cardX, y - 40, cardWidth, cardHeight), Color.White);
                        else
                            spriteBatch.Draw(GetCardTexture(trixBartyiah.Player1.CardsOnHand[i]), new Rectangle(cardX, y, cardWidth, cardHeight), Color.White);
                    }
                }
                //Draw the ate cards
                x = base.Game.GraphicsDevice.Viewport.Width / 2 - 15;
                y = base.Game.GraphicsDevice.Viewport.Height - cardLatchedHeight - 5;
                int sX = 0;
                for (int i = 0; i < trixBartyiah.Player1.AteCards.Count; i++)
                {
                    //calculate x position
                    int cardX = sX = x - (((trixBartyiah.Player1.AteCards.Count *
                        (cardLatchedWidth / 2 + 1)) / 2)) + (((cardLatchedWidth / 2 + 1) * i));
                    //draw card (don't show to user unless specificed casses)
                    Texture2D tCard = cardTextures[0];//"backside"
                    if ((trixBartyiah.PlayMode == PlayMode.Diamonds &
                        CardChecker.GetCardType(trixBartyiah.Player1.AteCards[i]) == "d")//«·œﬁ œÌ‰«—Ì Ê «·Ê—ﬁ… œÌ‰«—Ì
                        | (trixBartyiah.PlayMode == PlayMode.Queens &
                        CardChecker.GetCardName(trixBartyiah.Player1.AteCards[i]) == "q")//«·œﬁ »‰«  Ê «·Ê—ﬁ… »‰ 
                        )
                    {
                        tCard = GetCardTexture(trixBartyiah.Player1.AteCards[i]);
                    }
                    spriteBatch.Draw(tCard, new Rectangle(cardX, y, cardLatchedWidth, cardLatchedHeight), Color.White);
                }
                if (trixBartyiah.Player1.AteCards.Count > 0)
                {
                    string st = trixBartyiah.Player1.AteCards.Count.ToString() + "/" +
                        (trixBartyiah.Player1.AteCards.Count / 4).ToString();
                    spriteBatch.DrawString(Font_small, st, new Vector2(sX + cardLatchedWidth + 2, y + 2), Color.White);
                }
                #endregion
                #region player 2 (FOE 1)
                y = base.Game.GraphicsDevice.Viewport.Height / 2;
                x = base.Game.GraphicsDevice.Viewport.Width - (OffScreen);
                for (int i = 0; i < trixBartyiah.Player2.CardsOnHand.Count; i++)
                {
                    //calculate y position
                    int cardY = y - (((trixBartyiah.Player2.CardsOnHand.Count *
                        (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * i));
                    //draw card (NOT show to user)
                    if (trixBartyiah.PlayStatus != PlayStatus.Doubling)
                    {
                        spriteBatch.Draw(cardTextures[0], new Rectangle(x, cardY - 20, cardWidth, cardHeight),
                            new Rectangle(0, 0, cardTextures[0].Width, cardTextures[0].Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                    }
                    else
                    {
                        if (!CardChecker.IsCardExistInCollection(trixBartyiah.Player2.CardsOnHand[i], trixBartyiah.Player2.DoublingCards))
                        {
                            spriteBatch.Draw(cardTextures[0], new Rectangle(x, cardY - 20, cardWidth, cardHeight),
                      new Rectangle(0, 0, cardTextures[0].Width, cardTextures[0].Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(GetCardTexture(trixBartyiah.Player2.CardsOnHand[i]), new Rectangle(x - 20, cardY - 20, cardWidth, cardHeight),
                          new Rectangle(0, 0, cardTextures[0].Width, cardTextures[0].Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                        }
                    }
                }
                //Draw ate cards
                y = base.Game.GraphicsDevice.Viewport.Height / 2;
                x = base.Game.GraphicsDevice.Viewport.Width - cardLatchedHeight + 14;
                for (int i = 0; i < trixBartyiah.Player2.AteCards.Count; i++)
                {
                    //calculate y position
                    int cardY = sX = y - (((trixBartyiah.Player2.AteCards.Count *
                        (cardLatchedWidth / 2 + 1)) / 2)) + (((cardLatchedWidth / 2 + 1) * i));
                    //draw card (NOT show to user)
                    Texture2D tCard = cardTextures[0];
                    if ((trixBartyiah.PlayMode == PlayMode.Diamonds &
                        CardChecker.GetCardType(trixBartyiah.Player2.AteCards[i]) == "d")//«·œﬁ œÌ‰«—Ì Ê «·Ê—ﬁ… œÌ‰«—Ì
                        | (trixBartyiah.PlayMode == PlayMode.Queens &
                        CardChecker.GetCardName(trixBartyiah.Player2.AteCards[i]) == "q")//«·œﬁ »‰«  Ê «·Ê—ﬁ… »‰ 
                        )
                    {
                        tCard = GetCardTexture(trixBartyiah.Player2.AteCards[i]);
                    }
                    spriteBatch.Draw(tCard, new Rectangle(x, cardY - 20, cardLatchedWidth, cardLatchedHeight),
                        new Rectangle(0, 0, tCard.Width, tCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                }
                if (trixBartyiah.Player2.AteCards.Count > 0)
                {
                    string st = trixBartyiah.Player2.AteCards.Count.ToString() + "\n/\n" +
                        (trixBartyiah.Player2.AteCards.Count / 4).ToString();
                    spriteBatch.DrawString(Font_small, st, new Vector2(base.Game.GraphicsDevice.Viewport.Width - 22, sX), Color.White);
                }
                #endregion
                #region player 3 (YOUR Partner)
                x = base.Game.GraphicsDevice.Viewport.Width / 2 - 15;
                y = OffScreen;
                for (int i = 0; i < trixBartyiah.Player3.CardsOnHand.Count; i++)
                {
                    //calculate x position
                    int cardX = x - (((trixBartyiah.Player3.CardsOnHand.Count *
                        (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * i));
                    //draw card (NOT show to user)
                    if (trixBartyiah.PlayStatus != PlayStatus.Doubling)
                    {
                        spriteBatch.Draw(cardTextures[0], new Rectangle(cardX, y, cardWidth, cardHeight), Color.White);
                    }
                    else
                    {
                        if (!CardChecker.IsCardExistInCollection(trixBartyiah.Player3.CardsOnHand[i], trixBartyiah.Player3.DoublingCards))
                            spriteBatch.Draw(cardTextures[0], new Rectangle(cardX, y, cardWidth, cardHeight), Color.White);
                        else
                            spriteBatch.Draw(GetCardTexture(trixBartyiah.Player3.CardsOnHand[i]),
                                new Rectangle(cardX, y + 20, cardWidth, cardHeight), Color.White);
                    }
                }
                //Draw the ate cards
                x = base.Game.GraphicsDevice.Viewport.Width / 2 - 15;
                y = 4;
                for (int i = 0; i < trixBartyiah.Player3.AteCards.Count; i++)
                {
                    //calculate x position
                    int cardX = sX = x - (((trixBartyiah.Player3.AteCards.Count *
                        (cardLatchedWidth / 2 + 1)) / 2)) + (((cardLatchedWidth / 2 + 1) * i));
                    //draw card (show to user)
                    Texture2D tCard = cardTextures[0];
                    if ((trixBartyiah.PlayMode == PlayMode.Diamonds &
                        CardChecker.GetCardType(trixBartyiah.Player3.AteCards[i]) == "d")//«·œﬁ œÌ‰«—Ì Ê «·Ê—ﬁ… œÌ‰«—Ì
                        | (trixBartyiah.PlayMode == PlayMode.Queens &
                        CardChecker.GetCardName(trixBartyiah.Player3.AteCards[i]) == "q")//«·œﬁ »‰«  Ê «·Ê—ﬁ… »‰ 
                        )
                    {
                        tCard = GetCardTexture(trixBartyiah.Player3.AteCards[i]);
                    }
                    spriteBatch.Draw(tCard, new Rectangle(cardX, y, cardLatchedWidth, cardLatchedHeight), Color.White);
                }
                if (trixBartyiah.Player3.AteCards.Count > 0)
                {
                    string st = trixBartyiah.Player3.AteCards.Count.ToString() + "/" +
                        (trixBartyiah.Player3.AteCards.Count / 4).ToString();
                    spriteBatch.DrawString(Font_small, st, new Vector2(sX + cardLatchedWidth + 2, 8), Color.White);
                }
                #endregion
                #region player 4 (FOE 2)
                y = base.Game.GraphicsDevice.Viewport.Height / 2;
                x = cardHeight + OffScreen;
                for (int i = 0; i < trixBartyiah.Player4.CardsOnHand.Count; i++)
                {
                    //calculate y position
                    int cardY = y - (((trixBartyiah.Player4.CardsOnHand.Count *
                        (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * i));
                    //draw card (NOT show to user)
                    if (trixBartyiah.PlayStatus != PlayStatus.Doubling)
                    {
                        spriteBatch.Draw(cardTextures[0], new Rectangle(x, cardY - 20, cardWidth, cardHeight), new Rectangle(0, 0, cardTextures[0].Width, cardTextures[0].Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                    }
                    else
                    {
                        if (!CardChecker.IsCardExistInCollection(trixBartyiah.Player4.CardsOnHand[i], trixBartyiah.Player4.DoublingCards))
                            spriteBatch.Draw(cardTextures[0], new Rectangle(x, cardY - 20, cardWidth, cardHeight), new Rectangle(0, 0, cardTextures[0].Width, cardTextures[0].Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(GetCardTexture(trixBartyiah.Player4.CardsOnHand[i]), new Rectangle(x + 20, cardY - 20, cardWidth, cardHeight), new Rectangle(0, 0, cardTextures[0].Width, cardTextures[0].Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                    }
                }
                //Draw ate cards
                y = base.Game.GraphicsDevice.Viewport.Height / 2;
                x = 25;
                for (int i = 0; i < trixBartyiah.Player4.AteCards.Count; i++)
                {
                    //calculate y position
                    int cardY = sX = y - (((trixBartyiah.Player4.AteCards.Count *
                        (cardLatchedWidth / 2 + 1)) / 2)) + (((cardLatchedWidth / 2 + 1) * i));
                    //draw card (NOT show to user)
                    Texture2D tCard = cardTextures[0];
                    if ((trixBartyiah.PlayMode == PlayMode.Diamonds &
                        CardChecker.GetCardType(trixBartyiah.Player4.AteCards[i]) == "d")//«·œﬁ œÌ‰«—Ì Ê «·Ê—ﬁ… œÌ‰«—Ì
                        | (trixBartyiah.PlayMode == PlayMode.Queens &
                        CardChecker.GetCardName(trixBartyiah.Player4.AteCards[i]) == "q")//«·œﬁ »‰«  Ê «·Ê—ﬁ… »‰ 
                        )
                    {
                        tCard = GetCardTexture(trixBartyiah.Player4.AteCards[i]);
                    }
                    spriteBatch.Draw(tCard, new Rectangle(x, cardY - 20, cardLatchedWidth, cardLatchedHeight),
                        new Rectangle(0, 0, tCard.Width, tCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                }
                if (trixBartyiah.Player4.AteCards.Count > 0)
                {
                    string st = trixBartyiah.Player4.AteCards.Count.ToString() + "\n/\n" +
                        (trixBartyiah.Player4.AteCards.Count / 4).ToString();
                    spriteBatch.DrawString(Font_small, st, new Vector2(10, sX), Color.White);
                }
                #endregion

                #region Draw table cards
                if (trixBartyiah.DrawStatus == DrawStatus.EatTable)
                {
                    for (int i = 0; i < trixBartyiah.CardsOnTable.Count; i++)
                    {
                        x = (base.Game.GraphicsDevice.Viewport.Width / 2) - (cardWidth / 2);
                        y = (base.Game.GraphicsDevice.Viewport.Height / 2) - (cardHeight / 2);
                        switch ((i + trixBartyiah.DakPlayIndex) % 4)
                        {
                            case 0:
                                if (!(drawEffect & (draweffect == DrawEffect.Player1Play)))
                                    spriteBatch.Draw(GetCardTexture(trixBartyiah.CardsOnTable[i]), new Rectangle(x, y + OnTableCardSpace, cardWidth, cardHeight), Color.White);
                                break;
                            case 1:
                                if (!(drawEffect & (draweffect == DrawEffect.Player2Play)))
                                    spriteBatch.Draw(GetCardTexture(trixBartyiah.CardsOnTable[i]), new Rectangle(x + OnTableCardSpace, y, cardWidth, cardHeight), Color.White);
                                break;
                            case 2:
                                if (!(drawEffect & (draweffect == DrawEffect.Player3Play)))
                                    spriteBatch.Draw(GetCardTexture(trixBartyiah.CardsOnTable[i]), new Rectangle(x, y - OnTableCardSpace, cardWidth, cardHeight), Color.White);
                                break;
                            case 3:
                                if (!(drawEffect & (draweffect == DrawEffect.Player4Play)))
                                    spriteBatch.Draw(GetCardTexture(trixBartyiah.CardsOnTable[i]), new Rectangle(x - OnTableCardSpace, y, cardWidth, cardHeight), Color.White);
                                break;
                        }
                    }
                    #region EAT EFFECTS
                    if (drawEffect & (draweffect == DrawEffect.Player1Eat))
                    {
                        if (effectTimer > 0)
                        {
                            if (effectTimer == 50)
                                ((TrixCore)base.Game).PlaySound(seCardLatche);
                            Texture2D tEffectCard = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder + "backside");
                            x = base.Game.GraphicsDevice.Viewport.Width / 2 - 15;
                            y = (base.Game.GraphicsDevice.Viewport.Height / 2) - (cardHeight / 2) + OnTableCardSpace;

                            //calculate x position
                            int cardX = x - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 0));
                            spriteBatch.Draw(tEffectCard, new Rectangle(cardX, y + OnTableCardSpace - (effectTimer * 2), cardWidth, cardHeight), Color.White);
                            cardX = x - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 1));
                            spriteBatch.Draw(tEffectCard, new Rectangle(cardX, y + OnTableCardSpace - (effectTimer * 2), cardWidth, cardHeight), Color.White);
                            cardX = x - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 2));
                            spriteBatch.Draw(tEffectCard, new Rectangle(cardX, y + OnTableCardSpace - (effectTimer * 2), cardWidth, cardHeight), Color.White);
                            cardX = x - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 3));
                            spriteBatch.Draw(tEffectCard, new Rectangle(cardX, y + OnTableCardSpace - (effectTimer * 2), cardWidth, cardHeight), Color.White);
                            effectTimer -= 5;
                        }
                        else
                            drawEffect = false;
                    }
                    if (drawEffect & (draweffect == DrawEffect.Player2Eat))
                    {
                        if (effectTimer > 0)
                        {
                            if (effectTimer == 50)
                                ((TrixCore)base.Game).PlaySound(seCardLatche);
                            Texture2D tEffectCard = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder + "backside");
                            y = (base.Game.GraphicsDevice.Viewport.Height / 2) - (cardHeight / 2);
                            x = (base.Game.GraphicsDevice.Viewport.Width / 2) - (cardWidth / 2) + OnTableCardSpace;

                            int cardY = y - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 0));
                            spriteBatch.Draw(tEffectCard, new Rectangle(x + OnTableCardSpace + (50 - effectTimer), cardY + 20, cardWidth, cardHeight),
                                          new Rectangle(0, 0, tEffectCard.Width, tEffectCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);

                            cardY = y - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 1));
                            spriteBatch.Draw(tEffectCard, new Rectangle(x + OnTableCardSpace + (50 - effectTimer), cardY + 20, cardWidth, cardHeight),
              new Rectangle(0, 0, tEffectCard.Width, tEffectCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);

                            cardY = y - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 2));
                            spriteBatch.Draw(tEffectCard, new Rectangle(x + OnTableCardSpace + (50 - effectTimer), cardY + 20, cardWidth, cardHeight),
              new Rectangle(0, 0, tEffectCard.Width, tEffectCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);

                            cardY = y - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 3));
                            spriteBatch.Draw(tEffectCard, new Rectangle(x + OnTableCardSpace + (50 - effectTimer), cardY + 20, cardWidth, cardHeight),
             new Rectangle(0, 0, tEffectCard.Width, tEffectCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                            effectTimer -= 5;
                        }
                        else
                            drawEffect = false;
                    }
                    if (drawEffect & (draweffect == DrawEffect.Player3Eat))
                    {
                        if (effectTimer > 0)
                        {
                            if (effectTimer == 50)
                                ((TrixCore)base.Game).PlaySound(seCardLatche);
                            Texture2D tEffectCard = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder + "backside");
                            x = base.Game.GraphicsDevice.Viewport.Width / 2 - 15;
                            y = (base.Game.GraphicsDevice.Viewport.Height / 2) - (cardHeight / 2) - OnTableCardSpace;

                            //calculate x position
                            int cardX = x - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 0));
                            spriteBatch.Draw(tEffectCard, new Rectangle(cardX, y - OnTableCardSpace + (effectTimer * 2), cardWidth, cardHeight), Color.White);
                            cardX = x - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 1));
                            spriteBatch.Draw(tEffectCard, new Rectangle(cardX, y - OnTableCardSpace + (effectTimer * 2), cardWidth, cardHeight), Color.White);
                            cardX = x - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 2));
                            spriteBatch.Draw(tEffectCard, new Rectangle(cardX, y - OnTableCardSpace + (effectTimer * 2), cardWidth, cardHeight), Color.White);
                            cardX = x - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 3));
                            spriteBatch.Draw(tEffectCard, new Rectangle(cardX, y - OnTableCardSpace + (effectTimer * 2), cardWidth, cardHeight), Color.White);
                            effectTimer -= 5;
                        }
                        else
                            drawEffect = false;
                    }
                    if (drawEffect & (draweffect == DrawEffect.Player4Eat))
                    {
                        if (effectTimer > 0)
                        {
                            if (effectTimer == 50)
                                ((TrixCore)base.Game).PlaySound(seCardLatche);
                            Texture2D tEffectCard = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder + "backside");
                            y = (base.Game.GraphicsDevice.Viewport.Height / 2) - (cardHeight / 2);
                            x = (base.Game.GraphicsDevice.Viewport.Width / 2) - (cardWidth / 2) - OnTableCardSpace;

                            int cardY = y - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 0));
                            spriteBatch.Draw(tEffectCard, new Rectangle(x - OnTableCardSpace + (effectTimer * 2), cardY + 20, cardWidth, cardHeight),
                                          new Rectangle(0, 0, tEffectCard.Width, tEffectCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);

                            cardY = y - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 1));
                            spriteBatch.Draw(tEffectCard, new Rectangle(x - OnTableCardSpace + (effectTimer * 2), cardY + 20, cardWidth, cardHeight),
              new Rectangle(0, 0, tEffectCard.Width, tEffectCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);

                            cardY = y - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 2));
                            spriteBatch.Draw(tEffectCard, new Rectangle(x - OnTableCardSpace + (effectTimer * 2), cardY + 20, cardWidth, cardHeight),
              new Rectangle(0, 0, tEffectCard.Width, tEffectCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);

                            cardY = y - (((4 * (cardWidth / cardOffset + cardSpace)) / 2)) + (((cardWidth / cardOffset + cardSpace) * 3));
                            spriteBatch.Draw(tEffectCard, new Rectangle(x - OnTableCardSpace + (effectTimer * 2), cardY + 20, cardWidth, cardHeight),
             new Rectangle(0, 0, tEffectCard.Width, tEffectCard.Height), Color.White, 1.56f, new Vector2(), SpriteEffects.None, 0);
                            effectTimer -= 5;
                        }
                        else
                            drawEffect = false;
                    }
                    #endregion
                }

                else if (trixBartyiah.DrawStatus == DrawStatus.TrixTable)
                {
                    int w = 40;
                    int h = 46;
                    x = base.Game.GraphicsDevice.Viewport.Width / 2 - 15;
                    y = base.Game.GraphicsDevice.Viewport.Height / 2 - 15;
                    int space = 120;
                    int spaceCut = 80;
                    //type H
                    for (int i = 0; i < trixBartyiah.CardsOnTable_Trix_H.Count; i++)
                    {
                        //calculate y position
                        int cardY = y - (((trixBartyiah.CardsOnTable_Trix_H.Count * (h / 2)) / 2)) + (((h / 2) * i));
                        Texture2D tCard = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder +
                            trixBartyiah.CardsOnTable_Trix_H[i].ToString());

                        spriteBatch.Draw(tCard, new Rectangle(x - space, cardY, w, h), Color.White);
                    }
                    space -= spaceCut;
                    //type C
                    for (int i = 0; i < trixBartyiah.CardsOnTable_Trix_C.Count; i++)
                    {
                        //calculate y position
                        int cardY = y - (((trixBartyiah.CardsOnTable_Trix_C.Count * (h / 2)) / 2)) + (((h / 2) * i));
                        Texture2D tCard = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder +
                            trixBartyiah.CardsOnTable_Trix_C[i].ToString());

                        spriteBatch.Draw(tCard, new Rectangle(x - space, cardY, w, h), Color.White);
                    }
                    space -= spaceCut;
                    //type S
                    for (int i = 0; i < trixBartyiah.CardsOnTable_Trix_S.Count; i++)
                    {
                        //calculate y position
                        int cardY = y - (((trixBartyiah.CardsOnTable_Trix_S.Count * (h / 2)) / 2)) + (((h / 2) * i));
                        Texture2D tCard = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder +
                            trixBartyiah.CardsOnTable_Trix_S[i].ToString());

                        spriteBatch.Draw(tCard, new Rectangle(x - space, cardY, w, h), Color.White);
                    }
                    space -= spaceCut;
                    //type D
                    for (int i = 0; i < trixBartyiah.CardsOnTable_Trix_D.Count; i++)
                    {
                        //calculate y position
                        int cardY = y - (((trixBartyiah.CardsOnTable_Trix_D.Count * (h / 2)) / 2)) + (((h / 2) * i));
                        Texture2D tCard = base.Game.Content.Load<Texture2D>(Program.Settings.GamePlay_cardsSourceFolder +
                            trixBartyiah.CardsOnTable_Trix_D[i].ToString());

                        spriteBatch.Draw(tCard, new Rectangle(x - space, cardY, w, h), Color.White);
                    }
                }
                #endregion
                #region Play effects
                x = (base.Game.GraphicsDevice.Viewport.Width / 2) - (cardWidth / 2);
                y = (base.Game.GraphicsDevice.Viewport.Height / 2) - (cardHeight / 2);
                if (drawEffect & (draweffect == DrawEffect.Player1Play))
                {
                    if (effectTimer > 0)
                    {
                        if (effectTimer == 50)
                            ((TrixCore)base.Game).PlaySound(seCardDrop);
                        Texture2D tCardeffect = GetCardTexture(trixBartyiah.Player1.LastPlayedCard);
                        spriteBatch.Draw(tCardeffect, new Rectangle(x, y + OnTableCardSpace + effectTimer, cardWidth, cardHeight), Color.White);
                        effectTimer -= 5;
                    }
                    else
                        drawEffect = false;
                }
                if (drawEffect & (draweffect == DrawEffect.Player2Play))
                {
                    if (effectTimer > 0)
                    {
                        if (effectTimer == 50)
                            ((TrixCore)base.Game).PlaySound(seCardDrop);
                        Texture2D tCardeffect = GetCardTexture(trixBartyiah.Player2.LastPlayedCard);
                        spriteBatch.Draw(tCardeffect, new Rectangle(x + OnTableCardSpace + effectTimer, y, cardWidth, cardHeight), Color.White);
                        effectTimer -= 5;
                    }
                    else
                        drawEffect = false;
                }
                if (drawEffect & (draweffect == DrawEffect.Player3Play))
                {
                    if (effectTimer > 0)
                    {
                        if (effectTimer == 50)
                            ((TrixCore)base.Game).PlaySound(seCardDrop);
                        Texture2D tCardeffect = GetCardTexture(trixBartyiah.Player3.LastPlayedCard);
                        spriteBatch.Draw(tCardeffect, new Rectangle(x, y - OnTableCardSpace - effectTimer, cardWidth, cardHeight), Color.White); effectTimer -= 5;
                    }
                    else
                        drawEffect = false;
                }
                if (drawEffect & (draweffect == DrawEffect.Player4Play))
                {
                    if (effectTimer > 0)
                    {
                        if (effectTimer == 50)
                            ((TrixCore)base.Game).PlaySound(seCardDrop);
                        Texture2D tCardeffect = GetCardTexture(trixBartyiah.Player4.LastPlayedCard);
                        spriteBatch.Draw(tCardeffect, new Rectangle(x - OnTableCardSpace - effectTimer, y, cardWidth, cardHeight), Color.White);
                        effectTimer -= 5;
                    }
                    else
                        drawEffect = false;
                }
                #endregion
            }
        }

        public void CreateNewGame(string difficult, string PlayerName)
        {
            trixBartyiah = new TrixBartyiah();
            //prepare players
            trixBartyiah.Player1 = new TrixPlayer(PlayerName, 0, false, PlayMode.Diamonds, new List<Cards>(),
                                   new List<Cards>(), trixBartyiah, null);
            switch (difficult.ToLower())
            {
                case "easy":
                    trixBartyiah.Player2 = new TrixPlayer("Player 2", 0, true, PlayMode.Diamonds, new List<Cards>(),
                        new List<Cards>(), trixBartyiah, new Ai_Easy(trixBartyiah));
                    trixBartyiah.Player3 = new TrixPlayer("Player 3", 0, true, PlayMode.Diamonds, new List<Cards>(),
                        new List<Cards>(), trixBartyiah, new Ai_Easy(trixBartyiah));
                    trixBartyiah.Player4 = new TrixPlayer("Player 4", 0, true, PlayMode.Diamonds, new List<Cards>(),
                        new List<Cards>(), trixBartyiah, new Ai_Easy(trixBartyiah));
                    break;
                case "normal":
                    trixBartyiah.Player2 = new TrixPlayer("Player 2", 0, true, PlayMode.Diamonds, new List<Cards>(),
                        new List<Cards>(), trixBartyiah, new Ai_Normal(trixBartyiah));
                    trixBartyiah.Player3 = new TrixPlayer("Player 3", 0, true, PlayMode.Diamonds, new List<Cards>(),
                        new List<Cards>(), trixBartyiah, new Ai_Normal(trixBartyiah));
                    trixBartyiah.Player4 = new TrixPlayer("Player 4", 0, true, PlayMode.Diamonds, new List<Cards>(),
                        new List<Cards>(), trixBartyiah, new Ai_Normal(trixBartyiah));
                    break;
            }
            //fill up cards for FATA
            string[] xcards = Enum.GetNames(typeof(Cards));
            trixBartyiah.Cards = new List<Cards>();
            for (int i = 0; i < xcards.Length; i++)
            {
                trixBartyiah.Cards.Add((Cards)Enum.Parse(typeof(Cards), xcards[i]));
            }
            //set status
            trixBartyiah.PlayStatus = PlayStatus.Distributing;
            trixBartyiah.playedModes = new List<PlayMode>();

            trixBartyiah.EffectDrawRequest += new EventHandler<EffectArgs>(trixBartyiah_EffectDrawRequest);
            trixBartyiah.ClearEffectRequest += new EventHandler(trixBartyiah_ClearEffectRequest);
            trixBartyiah.FattaEffectRequest += new EventHandler(trixBartyiah_FattaEffectRequest);
            trixBartyiah.GameModeEnded += new EventHandler(trixBartyiah_GameModeEnded);
            trixBartyiah.BartyiahEnded += new EventHandler(trixBartyiah_BartyiahEnded);
            //launch
            IsPlaying = true;
        }
        void trixBartyiah_BartyiahEnded(object sender, EventArgs e)
        {
            ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\writing-1"));
            ((TrixCore)base.Game).PlayMusic(base.Game.Content.Load<Song>(@"Sounds\Music\soundTrack"));
        }
        void trixBartyiah_GameModeEnded(object sender, EventArgs e)
        {
            ((TrixCore)base.Game).PlaySound(base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\writing-1"));
            rScore.ShowTotalScore = false;
        }
        void trixBartyiah_FattaEffectRequest(object sender, EventArgs e)
        {
            ((TrixCore)base.Game).PlaySound(seFata);
        }
        void trixBartyiah_ClearEffectRequest(object sender, EventArgs e)
        {
            drawEffect = false;
            effectTimer = 0;
        }
        void trixBartyiah_EffectDrawRequest(object sender, EffectArgs e)
        {
            drawEffect = true;
            effectTimer = 50;
            draweffect = e.DrawEffect;
        }
    }
}