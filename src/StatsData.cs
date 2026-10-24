using System.IO;
using System.Text.Json;

namespace PersonalStats
{
    public class StatsData
    {
        public double TotalDistanceWalked { get; set; }
        public int Deaths { get; set; }
        public int BlocksBroken { get; set; }
        public int MobsKilled { get; set; }

        public static StatsData Load(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<StatsData>(json);
                    if (data != null) return data;
                }
            }
            catch
            {
                // Corrupt or unreadable file: fall back to fresh stats rather than crashing the mod.
            }

            return new StatsData();
        }

        public void Save(string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch
            {
                // Best-effort persistence; losing one save tick shouldn't crash the client.
            }
        }
    }
}
