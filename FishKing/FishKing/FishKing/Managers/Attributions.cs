using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FishKing.Managers
{
    public static class Attributions
    {
        public static string Text;

        public static async void LoadText()
        {
            var resourceName = "FishKing.attributions.txt";

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                Text = await reader.ReadToEndAsync();
            }
        }
    }
}
