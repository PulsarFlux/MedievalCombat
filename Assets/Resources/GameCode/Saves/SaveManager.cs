using System;
using Assets.GameCode;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Assets.GameCode.Saves
{
    [Serializable()]
    public struct SaveGame
    {
        public State.GameScene mLoadScene;
        public State.CampaignState mCampaignState;
        public Map.MapState mMapState;
        public Cards.CardGameState mCardGameState;
    }

    public enum SaveSlot
    {
        None,
        One,
        Two,
        Three,
        Max = SaveSlot.Three
    }

    public abstract class SaveManager
    {
        private static int kSaveVersion = 1;
        /*public SaveManager()
        {
        }*/
        private static string GetSavePath(SaveSlot slot)
        {
            string dirPath = System.IO.Directory.GetCurrentDirectory();
            return dirPath + "\\" + "MedComSave" + ((int)slot).ToString() + ".save";
        }

        private static bool GetFileStream(SaveSlot slot, System.IO.FileMode fileMode, ref System.IO.FileStream outFileStream)
        {
            UnityEngine.Debug.Assert(slot != SaveSlot.None);
            UnityEngine.Debug.Assert(outFileStream == null);

            outFileStream = null;

            try
            {
                string fullPath = GetSavePath(slot);
                outFileStream = new System.IO.FileStream(fullPath, fileMode);
            }
            catch (System.IO.IOException e)
            {
                UnityEngine.Debug.Log("Failed to get file stream with mode: " + fileMode.ToString() + 
                    ". Reason: " + e.Message);
                return false;
            }
            catch (ArgumentException e)
            {
                UnityEngine.Debug.Log("Failed to get file stream with mode: " + fileMode.ToString() + 
                    ". Reason: " + e.Message);
                return false;
            }
            catch (NotSupportedException e)
            {
                UnityEngine.Debug.Log("Failed to get file stream with mode: " + fileMode.ToString() + 
                    ". Reason: " + e.Message);
                return false;
            }
            catch (System.Security.SecurityException e)
            {
                UnityEngine.Debug.Log("Failed to get file stream with mode: " + fileMode.ToString() + 
                    ". Reason: " + e.Message);
                return false;
            }

            if (outFileStream == null)
            {
                UnityEngine.Debug.Log("Failed to open save file. Unknown reason.");
                return false;
            }

            return true;
        }

        public static bool SaveGame(SaveSlot slot, string saveName, SaveGame saveState)
        {
            System.IO.FileStream fileStream = null;
            bool result = GetFileStream(slot, System.IO.FileMode.Create, ref fileStream);
            if (result == false)
            {
                return false;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            bool success = true;

            try 
            {
                fileStream.Write(System.BitConverter.GetBytes(kSaveVersion), 0, sizeof(int));
                formatter.Serialize(fileStream, saveName);
                formatter.Serialize(fileStream, saveState);

                UnityEngine.Debug.Log("Saved game, slot: " + slot.ToString() + ", name: " + saveName);
            }
            catch (SerializationException e) 
            {
                UnityEngine.Debug.Log("Failed to serialize to save file. Reason: " + e.Message);
                success = false;
            }
            finally 
            {
                fileStream.Close();
            }

            UnityEngine.Debug.Assert(success);
            return success;
        }

        public static bool ReadSaveHeader(SaveSlot slot, out int outSaveVersion, ref string outSaveName)
        {
            System.IO.FileStream fileStream = null;
            outSaveVersion = 0;

            bool result = GetFileStream(slot, System.IO.FileMode.Open, ref fileStream);
            if (result == false)
            {
                return false;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            bool success = true;

            try 
            {
                byte[] saveVersionBytes = new byte[sizeof(int)];
                int bytesRead = fileStream.Read(saveVersionBytes, 0, sizeof(int));
                UnityEngine.Debug.Assert(bytesRead == sizeof(int));

                outSaveVersion = System.BitConverter.ToInt32(saveVersionBytes, 0);

                outSaveName = (string)formatter.Deserialize(fileStream);
                UnityEngine.Debug.Log("Save game name: " + outSaveName);
            }
            catch (SerializationException e) 
            {
                UnityEngine.Debug.Log("Failed to deserialize save file. Reason: " + e.Message);
                success = false;
            }
            finally 
            {
                fileStream.Close();
            }

            UnityEngine.Debug.Assert(success);
            return success;
        }

        public static bool LoadSave(SaveSlot slot, ref string outSaveName, out SaveGame outSaveGame)
        {
            outSaveGame = new Assets.GameCode.Saves.SaveGame();

            System.IO.FileStream fileStream = null;
            bool result = GetFileStream(slot, System.IO.FileMode.Open, ref fileStream);
            if (result == false)
            {
                return false;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            bool success = true;

            try 
            {
                byte[] saveVersionBytes = new byte[sizeof(int)];
                int bytesRead = fileStream.Read(saveVersionBytes, 0, sizeof(int));
                UnityEngine.Debug.Assert(bytesRead == sizeof(int));

                int saveVersion = System.BitConverter.ToInt32(saveVersionBytes, 0);
                UnityEngine.Debug.Assert(saveVersion == kSaveVersion);

                outSaveName = (string)formatter.Deserialize(fileStream);
                UnityEngine.Debug.Log("Save game name: " + outSaveName);
                outSaveGame = (SaveGame)formatter.Deserialize(fileStream);
            }
            catch (SerializationException e) 
            {
                UnityEngine.Debug.Log("Failed to deserialize save file. Reason: " + e.Message);
                success = false;
            }
            catch (System.IO.IOException e) 
            {
                UnityEngine.Debug.Log("Failed to deserialize save file. Reason: " + e.Message);
                success = false;
            }
            finally 
            {
                fileStream.Close();
            }

            UnityEngine.Debug.Assert(success);
            return success;
        }
    }
}

