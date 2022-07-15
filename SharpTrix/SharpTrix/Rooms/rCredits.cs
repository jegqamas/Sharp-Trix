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
    public class rCredits : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_small;
        SpriteFont Font_normal;
        SpriteFont Font_large;
        Texture2D tBackground;
        SoundEffect seClick;
        public rCredits(Game game)
            : base(game)
        {
            Font_normal = base.Game.Content.Load<SpriteFont>(@"Fonts\FontNormal");
            Font_small = base.Game.Content.Load<SpriteFont>(@"Fonts\FontSmall");
            Font_large = base.Game.Content.Load<SpriteFont>(@"Fonts\FontLarge");
            tBackground = base.Game.Content.Load<Texture2D>(@"Backgrounds\GreenTableBackground");
            seClick = base.Game.Content.Load<SoundEffect>(@"Sounds\Effects\click_x");
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
           

            spriteBatch.DrawString(Font_large, "TRIX CREDITS", new Vector2((base.Game.GraphicsDevice.Viewport.Width / 2) -
(Font_large.MeasureString("TRIX CREDITS").X / 2), 50), Color.White);
            spriteBatch.DrawString(Font_large, "Press Escape To Back To Main Menu",
                new Vector2((base.Game.GraphicsDevice.Viewport.Width / 2) -
                    (Font_large.MeasureString("Press Escape To Back To Main Menu").X / 2),
                    base.Game.GraphicsDevice.Viewport.Height - 70), Color.White);

            int y = 110;
            int x = 20;
            spriteBatch.DrawString(Font_normal, "Copyright (C) Ala Hadid 2011", new Vector2(x, y), Color.Yellow);
            y += 40;
            spriteBatch.DrawString(Font_normal, "All Right Reserved.", new Vector2(x, y), Color.White);
            y += 50;
            spriteBatch.DrawString(Font_normal, "Ala Hadid is the Author, Programmer and the Game \nDesigner of this game.\nMusic 'Trix Sound Track' composed by Ala Hadid.\nCard images are from: \nhttp://freeware.esoterica.free.fr/html/freecards.html ", new Vector2(x, y), Color.White);
            y += 160;
            spriteBatch.DrawString(Font_normal, "For support, please contact the author at: \nahdsoftwares@hotmail.com", new Vector2(x, y), Color.White);
            y += 80;
            spriteBatch.DrawString(Font_normal, "This game is free open source licensed under \nGNU GENERAL PUBLIC LICENSE 3.0\nhttp://www.gnu.org/licenses/", new Vector2(x, y), Color.LightPink);
        }
    }
}
