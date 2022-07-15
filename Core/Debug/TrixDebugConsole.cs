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
    /// The trix debug console controller, rises debug events only, the drawer shoul take care of the rest
    /// </summary>
    public class TrixDebugConsole
    {
        /// <summary>
        /// Write a debug line to show to user
        /// </summary>
        /// <param name="debugLine"></param>
        public static void WriteLine(string debugLine)
        {
            if (DebugRised != null)
                DebugRised(null, new TrixDebugConsoleArgs(debugLine));
        }
        /// <summary>
        /// The debug event which rised when a debug line writen to this class. WARNING: the sender always NULL
        /// </summary>
        public static event EventHandler<TrixDebugConsoleArgs> DebugRised;
    }
    /// <summary>
    /// Trix Debug Console Args for console events
    /// </summary>
    public class TrixDebugConsoleArgs : EventArgs
    {
        string debugLine;
        /// <summary>
        /// Trix Debug Console Args for console events
        /// </summary>
        /// <param name="debugLine">The debug line to show to the user</param>
        public TrixDebugConsoleArgs(string debugLine)
        {
            this.debugLine = debugLine;
        }
        /// <summary>
        /// Get the debug line
        /// </summary>
        public string DebugLine
        { get { return debugLine; } }
    }
}
