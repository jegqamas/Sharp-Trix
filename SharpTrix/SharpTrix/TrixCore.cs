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

namespace AHD.SharpTrix
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TrixCore : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public CurrentRoom Room = CurrentRoom.MainMenu;

        public rMainMenu rMainMenu;
        public rNewGame rNewGame;
        public rGamePlay rGamePlay;
        public rSettings rSettings;
        public rInGameMenu rInGameMenu;
        public rCredits rCredits;
        public rRules rRules;
        //resource
        Texture2D tMouse;
        SpriteFont Font_large;
        //game
        public string PLAYERNAME = "Trix Player";
        //others
        string TextToDraw = "";
        int FramesToDrawText = 0;
        //sounds
        Song soTrixSoundTrack;

        bool soundStopEffect= false;
        int soundStopEffectTimer = 0;

        public TrixCore()
        {
            graphics = new GraphicsDeviceManager(this);
            Program.VideoModes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.ToArray();
            //Apply video settings here for the first time...
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
            graphics.IsFullScreen = Program.Settings.Video_FullScreen;
#if WINDOWS
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();
            Window.Title = "Sharp Trix";
#endif
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.rMainMenu = new rMainMenu(this);
            this.rNewGame = new rNewGame(this);
            this.rGamePlay = new rGamePlay(this);
            this.rSettings = new rSettings(this);
            this.rInGameMenu = new rInGameMenu(this);
            this.rCredits = new rCredits(this);
            this.rRules = new rRules(this);
            soTrixSoundTrack = Content.Load<Song>(@"Sounds\Music\soundTrack");
            PlayMusic(soTrixSoundTrack);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Font_large = Content.Load<SpriteFont>(@"Fonts\FontLarge");
            tMouse = Content.Load<Texture2D>(@"Items\Mouse");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (Room)
            {
                case CurrentRoom.MainMenu: this.rMainMenu.Update(gameTime); break;
                case CurrentRoom.NewGameMenu: this.rNewGame.Update(gameTime); break;
                case CurrentRoom.GamePlay: this.rGamePlay.Update(gameTime); break;
                case CurrentRoom.Settings: this.rSettings.Update(gameTime); break;
                case CurrentRoom.InGameMenu: this.rInGameMenu.Update(gameTime); break;
                case CurrentRoom.Credits: this.rCredits.Update(gameTime); break;
                case CurrentRoom.Rules: this.rRules.Update(gameTime); break;
            }
            if (soundStopEffect)
            {
                if (soundStopEffectTimer > 0)
                { 
                    soundStopEffectTimer--;
                    if (MediaPlayer.Volume > 0.1f)
                        MediaPlayer.Volume -= 0.01f;
                }
                else
                {
                    soundStopEffect = false;
                    MediaPlayer.Stop();
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.ForestGreen);

            spriteBatch.Begin();
            //Draw room objects
            switch (Room)
            {
                case CurrentRoom.MainMenu: this.rMainMenu.Draw(spriteBatch); break;
                case CurrentRoom.NewGameMenu: this.rNewGame.Draw(spriteBatch); break;
                case CurrentRoom.GamePlay: this.rGamePlay.Draw(spriteBatch); break;
                case CurrentRoom.Settings: this.rSettings.Draw(spriteBatch); break;
                case CurrentRoom.InGameMenu: this.rInGameMenu.Draw(spriteBatch); break;
                case CurrentRoom.Credits: this.rCredits.Draw(spriteBatch); break;
                case CurrentRoom.Rules: this.rRules.Draw(spriteBatch); break;
            }
            //Draw mouse
            MouseState ms = Mouse.GetState();
            spriteBatch.Draw(tMouse, new Rectangle(ms.X, ms.Y, 40, 40), Color.White);
            //Draw status
            if (FramesToDrawText > 0)
            {
                FramesToDrawText--;
                spriteBatch.DrawString(Font_large, TextToDraw,
            new Vector2(5,
                GraphicsDevice.Viewport.Height - 33), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawStatusString(string text, int frames)
        {
            TextToDraw = text;
            FramesToDrawText = frames;
        }
        //for default graphics settings
        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            Program.VideoModes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.ToArray();
            if (!Program.IsFirstLaunch())
            {
                Microsoft.Xna.Framework.Graphics.DisplayMode displayMode = Program.VideoModes[Program.Settings.Video_ResIndex];

                e.GraphicsDeviceInformation.PresentationParameters.
                    BackBufferFormat = displayMode.Format;
                e.GraphicsDeviceInformation.PresentationParameters.
                    BackBufferHeight = displayMode.Height;
                e.GraphicsDeviceInformation.PresentationParameters.
                    BackBufferWidth = displayMode.Width;
            }
            else
            {
                int i = 0;
                foreach (Microsoft.Xna.Framework.Graphics.DisplayMode displayMode
                 in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if (displayMode.Width == 800 && displayMode.Height == 600)
                    {
                        e.GraphicsDeviceInformation.PresentationParameters.
                            BackBufferFormat = displayMode.Format;
                        e.GraphicsDeviceInformation.PresentationParameters.
                            BackBufferHeight = displayMode.Height;
                        e.GraphicsDeviceInformation.PresentationParameters.
                            BackBufferWidth = displayMode.Width;
                        Program.Settings.Video_ResIndex = i;
                        break;
                    }
                    i++;
                }
                //sound settings
                Program.Settings.Sound_Enabled = true;
                Program.Settings.Sound_EffectsVolume = 1.0f;
                Program.Settings.Sound_MusicVolume = 1.0f;
                //save
                Program.SaveSettings();
            }
        }

        public void PlaySound(SoundEffect soundEffect)
        {
            if (Program.Settings.Sound_Enabled)
                soundEffect.Play(Program.Settings.Sound_EffectsVolume, 0.0f, 0.0f);
        }
        public void PlayMusic(Song song)
        {
            if (Program.Settings.Sound_Enabled)
            {
                MediaPlayer.Volume = Program.Settings.Sound_MusicVolume;
                MediaPlayer.Play(song);
            }
        }
        public void StopMusicFadeOut()
        {
            soundStopEffect = true;
            soundStopEffectTimer = 120;
        }
    }
    public enum CurrentRoom
    {
        /// <summary>
        /// Single player, Multiplayer, option .... menu
        /// </summary>
        MainMenu,
        /// <summary>
        /// New Game, Load Game menu
        /// </summary>
        NewGameMenu,
        /// <summary>
        /// The game play room
        /// </summary>
        GamePlay,
        /// <summary>
        /// The settings room
        /// </summary>
        Settings,
        /// <summary>
        /// THe menu that appears when the user press escape in the game play
        /// </summary>
        InGameMenu,
        /// <summary>
        /// The credits room
        /// </summary>
        Credits,
        /// <summary>
        /// The rules and help room
        /// </summary>
        Rules
    }
}
