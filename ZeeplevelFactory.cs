using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace BPX
{
    #region v14CSV
    public class v14LevelCSVHeader
    {
        public string SceneName { get; private set; }
        public string PlayerName { get; private set; }
        public string UUID { get; private set; }
        public float[] CameraProperties { get; private set; }
        public float AuthorTime { get; private set; }
        public string AuthorTimeString { get; private set; }
        public float GoldTime { get; private set; }
        public float SilverTime { get; private set; }
        public float BronzeTime { get; private set; }
        public int Skybox { get; private set; }
        public int Ground { get; private set; }
        public bool Valid { get; private set; }

        public v14LevelCSVHeader()
        {
            SceneName = "LevelEditor2";
            PlayerName = "Bouwerman";
            UUID = GenerateUUID(PlayerName, 0);
            CameraProperties = new float[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            AuthorTime = 0;
            AuthorTimeString = "invalid track";
            GoldTime = 0;
            SilverTime = 0;
            BronzeTime = 0;
            Skybox = 0;
            Ground = -1;
            Valid = true;
        }

        public v14LevelCSVHeader(string[] csvData)
        {
            CameraProperties = new float[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            Read(csvData);
        }

        public void Read(string[] csvData)
        {
            Valid = true;

            if (csvData.Length != 3)
            {
                Valid = false;
                return;
            }

            for (int i = 0; i < csvData.Length; i++)
            {
                string[] values = csvData[i].Split(",");
                if (i == 0)
                {
                    if (values.Length != 3)
                    {
                        Valid = false;
                        break;
                    }

                    SceneName = values[0];
                    PlayerName = values[1];
                    UUID = values[2];
                }
                else if (i == 1)
                {
                    if (values.Length != 8)
                    {
                        Valid = false;
                        break;
                    }

                    for (int j = 0; j < 8; j++)
                    {
                        CameraProperties[j] = ParseFloat(values[j]);
                    }
                }
                else if (i == 2)
                {
                    if (values.Length != 6)
                    {
                        Valid = false;
                        break;
                    }

                    AuthorTime = ParseFloat(values[0]);
                    AuthorTimeString = AuthorTime == 0 ? "invalid track" : "";

                    GoldTime = ParseFloat(values[1]);
                    SilverTime = ParseFloat(values[2]);
                    BronzeTime = ParseFloat(values[3]);
                    Skybox = ParseInt(values[4]);
                    if (Skybox == -1) { Skybox = 0; }
                    Ground = ParseInt(values[5]);
                }
            }
        }

        private int ParseInt(string value)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : -1;
        }

        private float ParseFloat(string value)
        {
            return float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float result) ? result : 0.0f;
        }

        public static string GenerateUUID(string playerName, int objectCount)
        {
            // Get the current date and time
            string date = DateTime.Now.ToString("ddMMyyyy");
            string time = DateTime.Now.ToString("HHmmssfff");

            // Generate a 12-digit random number that does not start with 0
            Random random = new Random();
            string randomNumber = (random.Next(1, 10).ToString() + random.Next(0, 1000000000).ToString("D9"));

            // Combine all parts to form the UUID
            string uuid = $"{date}-{time}-{playerName}-{randomNumber}-{objectCount}";

            return uuid;
        }

        public string[] ToCSV()
        {
            // First line: SceneName, PlayerName, UUID
            string firstLine = $"{SceneName},{PlayerName},{UUID}";

            // Second line: CameraProperties
            string secondLine = string.Join(",", CameraProperties);

            // Third line: AuthorTime (or AuthorTimeString), GoldTime, SilverTime, BronzeTime, Skybox, Ground
            string authorTimeValue = AuthorTimeString == "invalid track" ? AuthorTimeString : AuthorTime.ToString(CultureInfo.InvariantCulture);
            string thirdLine = $"{authorTimeValue},{GoldTime},{SilverTime},{BronzeTime},{Skybox},{Ground}";

            // Return an array of strings
            return new string[] { firstLine, secondLine, thirdLine };
        }
    }

    public class v14LevelCSVBlock
    {
        public int BlockID { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }
        public Vector3 Scale { get; private set; }
        public List<float> Properties { get; private set; }
        public bool Valid { get; private set; }

        public v14LevelCSVBlock()
        {
            BlockID = -1;
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Scale = Vector3.one;
            Properties = new List<float> { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Valid = false;
        }

        public v14LevelCSVBlock(string csvData)
        {
            Read(csvData);
        }

        public void Read(string csvData) 
        { 
            string[] values = csvData.Split(',');

            if (values.Length != 38)
            {
                Valid = false;
                return;
            }

            try
            {
                BlockID = ParseInt(values[0]);
                Position = new Vector3(ParseFloat(values[1]), ParseFloat(values[2]), ParseFloat(values[3]));
                Rotation = new Vector3(ParseFloat(values[4]), ParseFloat(values[5]), ParseFloat(values[6]));
                Scale = new Vector3(ParseFloat(values[7]), ParseFloat(values[8]), ParseFloat(values[9]));

                Properties = new List<float>();
                for (int i = 1; i < values.Length; i++)
                {
                    Properties.Add(ParseFloat(values[i]));
                }

                Valid = true;
            }
            catch
            {
                Valid = false;
            }
        }

        private int ParseInt(string value)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : -1;
        }

        private float ParseFloat(string value)
        {
            return float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float result) ? result : 0.0f;
        }

        public string ToCSV()
        {
            StringBuilder csvBuilder = new StringBuilder();

            // First part: BlockID, Position, Rotation, Scale
            csvBuilder.Append($"{BlockID},{Position.x.ToString(CultureInfo.InvariantCulture)},{Position.y.ToString(CultureInfo.InvariantCulture)},{Position.z.ToString(CultureInfo.InvariantCulture)},");
            csvBuilder.Append($"{Rotation.x.ToString(CultureInfo.InvariantCulture)},{Rotation.y.ToString(CultureInfo.InvariantCulture)},{Rotation.z.ToString(CultureInfo.InvariantCulture)},");
            csvBuilder.Append($"{Scale.x.ToString(CultureInfo.InvariantCulture)},{Scale.y.ToString(CultureInfo.InvariantCulture)},{Scale.z.ToString(CultureInfo.InvariantCulture)},");

            // Properties part
            for (int i = 9; i < Properties.Count; i++)
            {
                csvBuilder.Append(Properties[i].ToString(CultureInfo.InvariantCulture));
                if (i < Properties.Count - 1)
                {
                    csvBuilder.Append(",");
                }
            }

            return csvBuilder.ToString();
        }
    }

    public class v14LevelCSV
    {
        public v14LevelCSVHeader Header { get; private set; }
        public List<v14LevelCSVBlock> Blocks { get; private set; }
        public bool Valid { get; private set; }

        public v14LevelCSV()
        {
            Header = new v14LevelCSVHeader();
            Blocks = new List<v14LevelCSVBlock>();
            Valid = false;
        }

        public v14LevelCSV(string[] allLines)
        {
            Valid = false;

            if (allLines == null || allLines.Length < 3)
                return;

            // First 3 lines: header
            string[] headerLines = allLines.Take(3).ToArray();
            Header = new v14LevelCSVHeader(headerLines);
            if (!Header.Valid)
                return;

            // Remaining lines: blocks
            Blocks = new List<v14LevelCSVBlock>();
            for (int i = 3; i < allLines.Length; i++)
            {
                var block = new v14LevelCSVBlock(allLines[i]);
                if (block.Valid)
                    Blocks.Add(block);
            }

            Valid = true;
        }

        public string[] ToCSV()
        {
            List<string> csvLines = new List<string>();

            // Add the header CSV lines
            csvLines.AddRange(Header.ToCSV());

            // Add each block's CSV representation
            foreach (var block in Blocks)
            {
                var blockCsv = block.ToCSV();
                if (!string.IsNullOrWhiteSpace(blockCsv))
                {
                    csvLines.Add(blockCsv);
                }
            }

            // Remove any empty lines
            csvLines = csvLines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            return csvLines.ToArray();
        }
    }
    #endregion

    public class ZeeplevelData
    {
        public LevelScriptableObject level;
        public v15LevelJSON json;
        public v14LevelCSV csv;
        public bool Valid;
    }

    public static class ZeeplevelFactory
    {
        public static ZeeplevelData FromPath(string path)
        {
            return FromFile(new FileInfo(path));
        }

        public static ZeeplevelData FromFile(FileInfo file)
        {
            if (file == null || !file.Exists)
            {
                Debug.LogError($"FromFile aborted: file does not exist ({file?.FullName})");
                return null;
            }

            string content = File.ReadAllText(ZeepkistFolders.ConvertToLongPath(file.FullName));
            return GeneralLevelLoadStatic.IsThisLevelDataStringV15(content)
                ? FromJSON(content, file)
                : FromCSV(Regex.Split(content, @"\r\n|\r|\n"), file);
        }

        public static ZeeplevelData FromJSON(string json, FileInfo file = null)
        {
            var data = new ZeeplevelData
            {
                json = JsonConvert.DeserializeObject<v15LevelJSON>(json),
                level = ScriptableObject.CreateInstance<LevelScriptableObject>(),
                Valid = true
            };

            var level = data.level;
            var j = data.json;

            level.Author = j.author.name;
            level.Collaborators = j.author.collaborators;
            level.OverrideAuthorName = j.author.nameOverride;
            level.UID = j.level.UID;
            level.IsValidated = j.medals.isLegit;
            level.TimeAuthor = j.medals.author;
            level.TimeGold = j.medals.gold;
            level.TimeSilver = j.medals.silver;
            level.TimeBronze = j.medals.bronze;
            level.useLevelV15Data = true;
            level.LevelDataV15 = json;

            if (file != null)
            {
                level.Name = Path.GetFileNameWithoutExtension(file.Name);
                level.Path = file.FullName;
            }

            SetFallbackTimes(data);
            return data;
        }

        public static ZeeplevelData FromCSV(string[] lines, FileInfo file = null)
        {
            var csv = new v14LevelCSV(lines);
            if (!csv.Valid)
                return new ZeeplevelData { Valid = false };

            var data = new ZeeplevelData
            {
                csv = csv,
                level = ScriptableObject.CreateInstance<LevelScriptableObject>(),
                Valid = true
            };

            var level = data.level;
            var h = csv.Header;

            level.Author = h.PlayerName;
            level.UID = h.UUID;
            level.TimeAuthor = h.AuthorTime;
            level.TimeGold = h.GoldTime;
            level.TimeSilver = h.SilverTime;
            level.TimeBronze = h.BronzeTime;
            level.useLevelV15Data = false;
            level.LevelData = lines;

            if (file != null)
            {
                level.Name = Path.GetFileNameWithoutExtension(file.Name);
                level.Path = file.FullName;
            }

            SetFallbackTimes(data);
            return data;
        }

        public static ZeeplevelData FromEditor(List<BlockProperties> blocks, string levelName, LEV_LevelEditorCentral central, SkyboxManager skybox)
        {
            if (blocks == null || blocks.Count == 0)
            {
                Debug.LogWarning("FromBlockProperties: No blocks provided.");
                return new ZeeplevelData { Valid = false };
            }

            v15LevelJSON zeepLevel = new v15LevelJSON();

            // Level
            zeepLevel.level.name = levelName;
            zeepLevel.level.UID = GenerateUniqueID(central.manager.steamAchiever.GetPlayerName(false));

            // Author
            zeepLevel.author.name = central.manager.steamAchiever.GetPlayerName(false);
            zeepLevel.author.StmID = central.manager.steamAchiever.GetPlayerSteamID();

            // Medals
            if (central.manager.validated)
            {
                zeepLevel.medals.isLegit = true;
                zeepLevel.medals.author = central.manager.validationTime;

                if (central.medalTimes.allGood)
                {
                    zeepLevel.medals.gold = central.medalTimes.goldTime;
                    zeepLevel.medals.silver = central.medalTimes.silverTime;
                    zeepLevel.medals.bronze = central.medalTimes.bronzeTime;
                }
                else
                {
                    float t = central.manager.validationTime;
                    zeepLevel.medals.gold = t * 1.1f;
                    zeepLevel.medals.silver = t * 1.2f;
                    zeepLevel.medals.bronze = t * 1.35f;
                }
            }
            else
            {
                zeepLevel.medals.isLegit = false;
                zeepLevel.medals.author = 0f;
                zeepLevel.medals.gold = 0f;
                zeepLevel.medals.silver = 0f;
                zeepLevel.medals.bronze = 0f;
            }

            // Environment
            zeepLevel.enviro.skybox = skybox.current;
            zeepLevel.enviro.groundMat = central.painter.currentGroundMaterial;
            zeepLevel.enviro.overrideFog_b = skybox.overrideFogBool;
            zeepLevel.enviro.overrideFog_f = skybox.overrideFogFloat;
            if (skybox.GetCurrentCustomSkybox() != null)
                zeepLevel.enviro.skyboxOverride = skybox.GetCurrentCustomSkybox();
            else if (GetSettings.Get().hidden_saveLevelsWithCustomSkyboxTemplate || PlayerManager.Instance.version.saveLevelWithSkyboxTemplate)
                zeepLevel.enviro.skyboxOverride = new SkyboxCreator_DataObject();

            // Camera
            zeepLevel.editcam.pos = new CV3(central.cam.transform.position);
            zeepLevel.editcam.euler = new CV3(central.cam.cameraTransform.eulerAngles);
            zeepLevel.editcam.rotXY = new CV2(new Vector2(central.cam.rotationX, central.cam.rotationY));

            // Blocks
            zeepLevel.blox = blocks.Select(b => b.ConvertBlockToJSON_v15()).ToList();

            // Hash
            zeepLevel.level.zeepHash = GeneralLevelLoadStatic.HashLevel(zeepLevel, blocks);

            // Wrap into ZeeplevelData
            var data = new ZeeplevelData
            {
                json = zeepLevel,
                level = ScriptableObject.CreateInstance<LevelScriptableObject>(),
                Valid = true
            };

            data.level.Name = levelName;
            data.level.Author = zeepLevel.author.name;
            data.level.Collaborators = zeepLevel.author.collaborators;
            data.level.OverrideAuthorName = zeepLevel.author.nameOverride;
            data.level.UID = zeepLevel.level.UID;
            data.level.IsValidated = zeepLevel.medals.isLegit;
            data.level.TimeAuthor = zeepLevel.medals.author;
            data.level.TimeGold = zeepLevel.medals.gold;
            data.level.TimeSilver = zeepLevel.medals.silver;
            data.level.TimeBronze = zeepLevel.medals.bronze;
            data.level.useLevelV15Data = true;
            data.level.LevelDataV15 = JsonConvert.SerializeObject(zeepLevel, Formatting.Indented);

            return data;
        }

        public static void ToCSVFile(ZeeplevelData data, string path)
        {
            if (data.csv == null || !data.Valid)
            {
                Debug.LogError("ToCSVFile aborted: invalid ZeeplevelData");
                return;
            }

            File.WriteAllLines(path, data.csv.ToCSV());
        }

        public static void ToJSONFile(ZeeplevelData data, string path)
        {
            if (data.json == null || !data.Valid)
            {
                Debug.LogError("ToJSONFile aborted: invalid ZeeplevelData");
                return;
            }

            string json = JsonConvert.SerializeObject(data.json, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private static void SetFallbackTimes(ZeeplevelData data)
        {
            var level = data.level;
            bool wasMissing = false;

            if (level.TimeAuthor <= 0f)
            {
                level.TimeAuthor = 3540f;
                level.IsValidated = false;
                wasMissing = true;
            }
            else
            {
                level.IsValidated = true;
            }

            if (level.TimeGold <= 0f)
                level.TimeGold = wasMissing ? 3546f : level.TimeAuthor * 1.1f;
            if (level.TimeSilver <= 0f)
                level.TimeSilver = wasMissing ? 3552f : level.TimeAuthor * 1.2f;
            if (level.TimeBronze <= 0f)
                level.TimeBronze = wasMissing ? 3558f : level.TimeAuthor * 1.35f;
        }

        private static string GenerateUniqueID(string playerName)
        {
            string guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                .Substring(0, 15);

            return guid + "_" + playerName.Replace(",", "");
        }

        public static ZeeplevelData Copy(ZeeplevelData original)
        {
            if (original == null || !original.Valid)
                return new ZeeplevelData { Valid = false };

            ZeeplevelData copy = new ZeeplevelData();
            copy.level = ScriptableObject.CreateInstance<LevelScriptableObject>();
            copy.Valid = original.Valid;

            if (original.json != null)
            {
                // Deep copy v15 JSON
                string jsonString = JsonConvert.SerializeObject(original.json);
                copy.json = JsonConvert.DeserializeObject<v15LevelJSON>(jsonString);
                copy.level.useLevelV15Data = true;
                copy.level.LevelDataV15 = jsonString;
            }
            else if (original.csv != null)
            {
                // Deep copy v14 CSV
                string[] csvLines = original.csv.ToCSV();
                copy.csv = new v14LevelCSV(csvLines);
                copy.level.useLevelV15Data = false;
                copy.level.LevelData = csvLines;
            }
            else
            {
                Debug.LogError("CopyZeeplevelData: No data found to copy.");
                return new ZeeplevelData { Valid = false };
            }

            // Copy level metadata
            copy.level.Name = original.level.Name;
            copy.level.Author = original.level.Author;
            copy.level.Collaborators = original.level.Collaborators;
            copy.level.OverrideAuthorName = original.level.OverrideAuthorName;
            copy.level.UID = original.level.UID;
            copy.level.IsValidated = original.level.IsValidated;
            copy.level.TimeAuthor = original.level.TimeAuthor;
            copy.level.TimeGold = original.level.TimeGold;
            copy.level.TimeSilver = original.level.TimeSilver;
            copy.level.TimeBronze = original.level.TimeBronze;
            copy.level.IsTestLevel = original.level.IsTestLevel;
            copy.level.IsAdventureLevel = original.level.IsAdventureLevel;
            copy.level.UseAvonturenLevel = original.level.UseAvonturenLevel;
            copy.level.Path = original.level.Path;

            return copy;
        }

        public static void SaveToFile(ZeeplevelData data, string path)
        {
            if (data == null || !data.Valid)
            {
                Debug.LogError("SaveToFile: Invalid ZeeplevelData");
                return;
            }

            if (data.json != null)
            {
                string jsonString = JsonConvert.SerializeObject(data.json, Formatting.Indented);
                File.WriteAllText(path, jsonString);
            }
            else if (data.csv != null)
            {
                string[] csvLines = data.csv.ToCSV();
                File.WriteAllLines(path, csvLines);
            }
            else
            {
                Debug.LogError("SaveToFile: No json or csv data to write.");
            }
        }
    }
}
