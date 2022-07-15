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
    public class EffectArgs : EventArgs
    {
        DrawEffect drawEffect;
        public EffectArgs(DrawEffect drawEffect)
        {
            this.drawEffect = drawEffect;
        }
        /// <summary>
        /// Get the draw effect requested by user
        /// </summary>
        public DrawEffect DrawEffect
        { get { return drawEffect; } }
    }
}
