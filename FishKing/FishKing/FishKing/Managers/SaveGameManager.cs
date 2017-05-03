using FishKing.Extensions;
using FishKing.GameClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace FishKing.Managers
{
    public static class SaveGameManager
    {
        private static string AppFolder = "LetsGoFishKing";
        private static string SaveFolder = "Saves";

        public static string SavePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(SpecialFolder.MyDocuments), AppFolder, SaveFolder);                
            }

        }
        public static SaveFileData CurrentSaveData { get; private set; }

        public static List<SaveFileData> GetAllSaves()
        {
            List<SaveFileData> returnList = new List<SaveFileData>();

            if (!File.Exists(SavePath))
            {
                return returnList;
            }

            string[] filePaths = Directory.GetFiles(SavePath, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string filePath in filePaths)
            {
                SaveFileData sfd = new SaveFileData();
                sfd.LoadData(filePath);
                returnList.Add(sfd);
            }

            return returnList;
        }

        public static bool Save()
        {
            if (CurrentSaveData != null && !String.IsNullOrWhiteSpace(CurrentSaveData.UniqueID))
            {
                string savePath = Path.Combine(SavePath, CurrentSaveData.UniqueID + ".xml");

                return CurrentSaveData.SaveFile(savePath);
            }
            else
            {
                return false;
            }
        }

        public static void SetCurrentData(SaveFileData data)
        {
            CurrentSaveData = data;
        }
    }
}
