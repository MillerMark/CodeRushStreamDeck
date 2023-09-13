using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StreamDeckLib.Messages;

namespace CodeRushStreamDeck
{
    public class GlobalSettingsModel
    {
        public List<string> Integers { get; set; } = new();
        public List<string> Strings { get; set; } = new();

        public GlobalSettingsModel()
        {
            
        }
    }
    public class GlobalSettingsSaver
    {
        public GlobalSettingsModel settingsModel { get; set; }
        public GlobalSettingsSaver()
        {
            
        }
    }
    public static class Variables
    {
        static Dictionary<string, string> stringVariables = new();
        static Dictionary<string, int> intVariables = new();

        public static event EventHandler<VarEventArgs<int>> IntVarChanged;
        public static event EventHandler<VarEventArgs<string>> StringVarChanged;

        static void UpdateGlobalSettings()
        {
            StreamDeck.SaveGlobalSettings();
            
        }
        static void OnIntVarChanging(string varName, int value)
        {
            UpdateGlobalSettings();
            IntVarChanged?.Invoke(null, new VarEventArgs<int>(varName, value));
        }

        static void OnStringVarChanging(string varName, string value)
        {
            UpdateGlobalSettings();
            StringVarChanged?.Invoke(null, new VarEventArgs<string>(varName, value));
        }

        public static void SetInt(string varName, int value = 0)
        {
            if (string.IsNullOrEmpty(varName))
                return;

            if (intVariables.ContainsKey(varName))
                if (intVariables[varName] != value)
                    OnIntVarChanging(varName, value);
            intVariables[varName] = value;
        }

        public static void SetString(string varName, string value = "")
        {
            if (string.IsNullOrEmpty(varName))
                return;

            if (stringVariables.ContainsKey(varName))
                if (stringVariables[varName] != value)
                    OnStringVarChanging(varName, value);
            stringVariables[varName] = value;
        }

        public static void ChangeInt(string varName, int delta)
        {
            int currentValue;
            if (intVariables.ContainsKey(varName))
                currentValue = intVariables[varName];
            else
                currentValue = 0;

            int newValue = currentValue + delta;
            SetInt(varName, newValue);
        }

        public static bool ContainsIntVar(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                return false;

            return intVariables.ContainsKey(variableName);
        }

        public static bool ContainsStringVar(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                return false;

            return stringVariables.ContainsKey(variableName);
        }

        public static int GetInt(string variableName)
        {
            if (intVariables.TryGetValue(variableName, out int value))
                return value;
            return 0;
        }

        public static string GetString(string variableName)
        {
            if (stringVariables.TryGetValue(variableName, out string value))
                return value;
            return string.Empty;
        }

        public static dynamic GetGlobalSettings()
        {
            JArray integers = new JArray();
            intVariables.Keys.ToList().ForEach(x => integers.Add($"{x}\t{intVariables[x]}"));

            JArray strings = new JArray();
            stringVariables.Keys.ToList().ForEach(x => strings.Add($"{x}\t{stringVariables[x]}"));

            dynamic settings = new JObject();

            settings.Integers = integers;
            settings.Strings = strings;

            return settings;
        }

        public static void SetFromGlobalSettings(StreamDeckEventPayload.Payload payload)
        {
            if (payload.settings is JObject jobject)
            {
                var globalSettings = jobject.ToObject<GlobalSettingsSaver>();
                if (globalSettings != null)
                {
                    foreach (string strSetting in globalSettings.settingsModel.Strings)
                    {
                        if (string.IsNullOrEmpty(strSetting))
                            continue;
                        string[] sides = strSetting.Split('\t');
                        if (sides.Length == 2)
                            SetString(sides[0], sides[1]);

                    }

                    foreach (string intSetting in globalSettings.settingsModel.Integers)
                    {
                        if (string.IsNullOrEmpty(intSetting))
                            continue;
                        string[] sides = intSetting.Split('\t');
                        if (sides.Length == 2)
                            if (int.TryParse(sides[1], out int value))
                                SetInt(sides[0], value);

                    }
                }
            }
        }

        public static List<string> GetAllStringVariableNames()
        {
            return stringVariables.Keys.ToList();
        }

        public static List<string> GetAllIntVariableNames()
        {
            return intVariables.Keys.ToList();
        }

        public static string Expand(string phrase)
        {
            List<string> allStringVars = GetAllStringVariableNames();
            
            const string escapedSlash = "___Slash___";
            const string escapedDollarSign = "___Dollar___";

            phrase = phrase.Replace("\\\\", escapedSlash);
            phrase = phrase.Replace("\\$", escapedDollarSign);

            allStringVars.ForEach(x => phrase = phrase.Replace($"${x}$", GetString(x)));

            phrase = phrase.Replace(escapedDollarSign, "$");
            phrase = phrase.Replace(escapedSlash, "\\");
            return phrase;
        }
    }
}
