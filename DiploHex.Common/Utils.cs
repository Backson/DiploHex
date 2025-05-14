using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiploHex.Common
{
    public static class Utils
    {
        public static string GetHexColor(int r, int g, int b)
        {
            return $"#{r:X2}{g:X2}{b:X2}";
        }
    }
}
