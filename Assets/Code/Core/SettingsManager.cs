using System;
using System.IO;
using SaintsField.Playa;
using UnityEngine;

namespace Tulip.Core
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] string fileName = "settings.json";

        [ShowInInspector]
        private string FilePath => Path.Join(Application.persistentDataPath, fileName);

        [ShowInInspector, TextArea]
        private static string FullJson => JsonUtility.ToJson(Settings.Instance, prettyPrint: true);

        private void Awake() => LoadFromFile();

        private void OnEnable() => Settings.OnUpdate += Settings_Updated;
        private void OnDisable() => Settings.OnUpdate -= Settings_Updated;

        private void Settings_Updated() => SaveToFile();

        private void SaveToFile() =>
            File.WriteAllText(FilePath, FullJson);

        private void LoadFromFile()
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                Settings storedSettings = JsonUtility.FromJson<Settings>(json);
                Settings.SetInstance(storedSettings ?? new Settings());
            }
            catch (Exception)
            {
                // replace invalid file with default settings
                SaveToFile();
            }
        }
    }
}
