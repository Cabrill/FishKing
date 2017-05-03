using FishKing.GameClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FishKing.Extensions
{
    static class SaveFileDataExtensions
    {
        private static XmlSerializer serializer;
        private static XmlSerializer Serializer
        {
            get
            {
                if (serializer == null)
                {
                    serializer = new XmlSerializer(typeof(SaveFileData));
                }
                return serializer;
            }
        }

        public static bool SaveFile(this SaveFileData saveFile, string path)
        {
            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    serializer.Serialize(writer, saveFile);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void LoadData(this SaveFileData saveFile, string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    saveFile = (SaveFileData)serializer.Deserialize(reader);
                }
            }
            else
            {
                throw new ArgumentNullException("File does not exist: " + path);
            }
        }
    }
}
