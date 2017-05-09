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
                    Serializer.Serialize(writer, saveFile);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool MeetsRequirements(this SaveFileData saveFile, TournamentStructure tournament)
        {
            if (saveFile == null) return false;

            var trophyType = tournament.TrophyRequirements.Item1;
            var trophyNum = tournament.TrophyRequirements.Item2;

            switch (trophyType)
            {
                case Enums.TrophyTypes.TrophyType.None: return true;
                case Enums.TrophyTypes.TrophyType.Gold: return (trophyNum <= saveFile.NumberOfGoldTrophies);
                case Enums.TrophyTypes.TrophyType.Silver: return (trophyNum <= saveFile.NumberOfGoldTrophies + saveFile.NumberOfSilverTrophies);
                case Enums.TrophyTypes.TrophyType.Bronze: return (trophyNum <= saveFile.NumberOfAllTrophies);
                default: return false;
            }
        }


        public static bool LoadData(this SaveFileData sfd, out SaveFileData saveFile, string path)
        {
            if (File.Exists(path))
            {
                SaveFileData saveData = null;
                // A FileStream is needed to read the XML document.
                FileStream fs = new FileStream(path, FileMode.Open);
                //XmlReader reader = XmlReader.Create(fs);

                // Use the Deserialize method to restore the object's state.
                saveData = Serializer.Deserialize(fs) as SaveFileData;
                fs.Close();

                saveFile = saveData;

                return (saveData != null);
            }
            else
            {
                throw new ArgumentNullException("File does not exist: " + path);
            }
        }
    }
}
