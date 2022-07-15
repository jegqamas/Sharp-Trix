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
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using AHD.SharpTrix.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Graphics;

namespace AHD.SharpTrix
{
#if WINDOWS || XBOX
    static class Program
    {
        static Settings settings = new Settings();
        static Microsoft.Xna.Framework.Graphics.DisplayMode[] videoModes;
        static TrixCore game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            TrixDebugConsole.DebugRised += new EventHandler<TrixDebugConsoleArgs>(TrixDebugConsole_DebugRised);
            LoadSettings(); 
            using (game = new TrixCore())
            {
                game.Run();
            }
        }
        /// <summary>
        /// Get the settings class
        /// </summary>
        public static Settings Settings
        { get { return settings; } }
        public static Microsoft.Xna.Framework.Graphics.DisplayMode[] VideoModes
        {
            get { return videoModes; }
            set { videoModes = value; }
        }
        /// <summary>
        /// Get the video mode string
        /// </summary>
        /// <param name="index">The index of the video mode</param>
        /// <returns></returns>
        public static string GetVideoModeText(int index)
        {
            return videoModes[index].Width + " x " + videoModes[index].Height + " " + videoModes[index].Format.ToString();
        }
        static void TrixDebugConsole_DebugRised(object sender, TrixDebugConsoleArgs e)
        {
#if DEBUG
            Console.WriteLine(e.DebugLine);
#endif
        }

        public static void SaveSettings(bool ShowLog)
        {
            IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            if (result.IsCompleted)
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    result = device.BeginOpenContainer("Settings", null, null);
                    result.AsyncWaitHandle.WaitOne();

                    StorageContainer container = device.EndOpenContainer(result);

                    if (container.FileExists("Settings.st"))
                        // Delete it so that we can create one fresh.
                        container.DeleteFile("Settings.st");

                    Stream stream = container.CreateFile("Settings.st");

                    if (stream.Length < device.FreeSpace)
                    {
                        XmlSerializer ser = new XmlSerializer(settings.GetType());
                        ser.Serialize(stream, settings);
                        stream.Close();
                        container.Dispose();
                        if (game != null & ShowLog)
                            game.DrawStatusString("Settings saved success", 60);
                    }
                    else
                    {
                        //No enough space
                        if (game != null & ShowLog)
                            game.DrawStatusString("CAN'T SAVE, NO ENOUGH SPACE !!", 120);
                    }
                }
                else
                {
                    //no device connected
                    if (game != null & ShowLog)
                        game.DrawStatusString("CAN'T SAVE, DEVICE NOT CONNECTED OR DAMAGED !!", 120);
                }
            }
        }
        public static void SaveSettings()
        {
            SaveSettings(true);
        }
        public static void LoadSettings()
        {
            IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            if (result.IsCompleted)
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    result = device.BeginOpenContainer("Settings", null, null);
                    result.AsyncWaitHandle.WaitOne();

                    StorageContainer container = device.EndOpenContainer(result);

                    if (container.FileExists("Settings.st"))
                    {
                        Stream stream = container.OpenFile("Settings.st", FileMode.Open, FileAccess.Read);

                        XmlSerializer ser = new XmlSerializer(typeof(Settings));
                        settings = (Settings)ser.Deserialize(stream);
                        stream.Close();
                        container.Dispose();
                    }
                    else
                    {
                        //file not exists ...
                    }
                }
                else
                {      
                    //no device connected
                    game.DrawStatusString("CAN'T LOAD SETTINGS, DEVICE NOT CONNECTED OR DAMAGED !!", 120);
                }
            }
        }
        public static bool IsFirstLaunch()
        {
            IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            if (result.IsCompleted)
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    result = device.BeginOpenContainer("Settings", null, null);
                    result.AsyncWaitHandle.WaitOne();

                    StorageContainer container = device.EndOpenContainer(result);

                   return (!container.FileExists("Settings.st"));
                    
                }
            }
            return true;
        }
        public static void ApplyVideoSettings()
        {
            game.graphics.IsFullScreen = Program.Settings.Video_FullScreen;
            game.graphics.SynchronizeWithVerticalRetrace = false;

            DisplayMode displayMode = videoModes[Program.Settings.Video_ResIndex];

            game.graphics.PreferredBackBufferFormat = displayMode.Format;
            game.graphics.PreferredBackBufferHeight= displayMode.Height;
            game.graphics.PreferredBackBufferWidth = displayMode.Width;

            game.graphics.ApplyChanges();
        }
    }
#endif
}

