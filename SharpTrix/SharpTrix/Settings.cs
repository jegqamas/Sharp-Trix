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
namespace AHD.SharpTrix
{
    public class Settings
    {
        //Video
        public int Video_ResIndex = 1;
        public bool Video_FullScreen = false;
        //Sound
        public bool Sound_Enabled = false;
        public float Sound_EffectsVolume = 1.0f;//min=0.0f , max=1.0f
        public float Sound_MusicVolume = 1.0f;//min=0.0f , max=1.0f
        //Game play
        //the cards resource, cards should be named same as "AHD.SharpTrix.Core.Cards" card names
        public string GamePlay_cardsSourceFolder = @"PlayCards\Basic\";
        public string GamePlay_lastPlayerName = "Player 1";
    }
}
