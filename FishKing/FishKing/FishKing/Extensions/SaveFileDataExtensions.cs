using FishKing.GameClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

        public static async Task<bool> LoadData(this SaveFileData saveFile, string path)
        {
            if (File.Exists(path))
            {
                await Task.Run(() =>
                {
                    // A FileStream is needed to read the XML document.
                    FileStream fs = new FileStream(path, FileMode.Open);
                    XmlReader reader = XmlReader.Create(fs);

                    // Use the Deserialize method to restore the object's state.
                    saveFile = (SaveFileData)serializer.Deserialize(reader);
                    fs.Close();
                });
                return true;
            }
            else
            {
                throw new ArgumentNullException("File does not exist: " + path);
            }
        }
    }
}
