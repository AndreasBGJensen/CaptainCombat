using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CaptainCombat.Source
{
    public class KeyboardInputValidator
    {
        private static string pattern = @"[A-Z]";

        public static Dictionary<string, string> dict = new Dictionary<string, string>
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
            { "OemMinus", "-" }
        };

        public static bool isValid(string keyData)
        {
            return Regex.IsMatch(keyData, pattern) && keyData.Length <= 1 || dict.ContainsKey(keyData);
        }

    }
}
