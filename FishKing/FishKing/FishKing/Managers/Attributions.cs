using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.Managers
{
    public static class Attributions
    {
        public static string Text;

        public static async void LoadText()
        {
            using (var reader = File.OpenText("attributions.txt"))
            {
                Text = await reader.ReadToEndAsync();
            }
        }
    }
}
