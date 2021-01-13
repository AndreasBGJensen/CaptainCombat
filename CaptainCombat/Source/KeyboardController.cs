using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.Source
{
    class KeyboardController 
    {
        private Keys[] lastPressedKeys = new Keys[5];
        private string message = string.Empty;
        private string lastCharacter = string.Empty;

        Dictionary<string, string> dict = new Dictionary<string, string>
        {
            { "D1", "1" },
            { "D2", "2" },
            { "D3", "3" },
            { "D4", "4" },
            { "D5", "5" },
            { "D6", "6" },
            { "D7", "7" },
            { "D8", "8" },
            { "D9", "9" },
            { "D0", "0" },
            { "OemComma", "," },
            { "OemPeriod", "." },
            { "OemMinus", "-" },

        };

        public void GetKeys(string request, Action<string> callback)
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            foreach (Keys key in lastPressedKeys)
            {
                if (!pressedKeys.Contains(key))
                {
                    
                }
            }
            foreach (Keys key in pressedKeys)
            {
                if (!lastPressedKeys.Contains(key))
                {
                   
                }
            }
            lastPressedKeys = pressedKeys;
        }



    }
}
