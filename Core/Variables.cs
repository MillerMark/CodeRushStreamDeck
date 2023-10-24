using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StreamDeckLib.Messages;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using System.Collections.Concurrent;

namespace CodeRushStreamDeck
{
    public static class Variables
    {
        static ConcurrentDictionary<string, string> stringVariables = new();
        static ConcurrentDictionary<string, int> intVariables = new();
        static ConcurrentDictionary<string, bool> boolVariables = new();

        public static event EventHandler<VarEventArgs<int>> IntVarChanged;
        public static event EventHandler<VarEventArgs<bool>> BoolVarChanged;
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
        
        static void OnBoolVarChanging(string varName, bool value)
        {
            UpdateGlobalSettings();
            BoolVarChanged?.Invoke(null, new VarEventArgs<bool>(varName, value));
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

            if (!intVariables.TryGetValue(varName, out int existingValue) || existingValue != value)
                OnIntVarChanging(varName, value);

            intVariables[varName] = value;
        }

        public static void SetBool(string varName, bool value = false)
        {
            if (string.IsNullOrEmpty(varName))
                return;

            if (!boolVariables.TryGetValue(varName, out bool existingValue) || existingValue != value)
                OnBoolVarChanging(varName, value);

            boolVariables[varName] = value;
        }

        public static void SetString(string varName, string value = "")
        {
            if (string.IsNullOrEmpty(varName))
                return;

            if (!stringVariables.TryGetValue(varName, out string existingValue) || existingValue != value)
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

        public static bool ContainsBoolVar(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                return false;

            return boolVariables.ContainsKey(variableName);
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

        public static bool GetBool(string variableName)
        {
            if (boolVariables.TryGetValue(variableName, out bool value))
                return value;
            return false;
        }

        public static string GetIntAsStr(string variableName)
        {
            if (intVariables.TryGetValue(variableName, out int value))
                return value.ToString();
            return "0";
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

            JArray booleans = new JArray();
            boolVariables.Keys.ToList().ForEach(x => booleans.Add($"{x}\t{boolVariables[x]}"));

            JArray strings = new JArray();
            stringVariables.Keys.ToList().ForEach(x => strings.Add($"{x}\t{stringVariables[x]}"));

            dynamic settings = new JObject();

            settings.Integers = integers;
            settings.Strings = strings;
            settings.Booleans = booleans;

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
                    foreach (string boolSetting in globalSettings.settingsModel.Booleans)
                    {
                        if (string.IsNullOrEmpty(boolSetting))
                            continue;
                        string[] sides = boolSetting.Split('\t');
                        if (sides.Length == 2)
                            if (bool.TryParse(sides[1], out bool value))
                                SetBool(sides[0], value);
                    }
                }
            }
        }

        static List<string> GetAllStringVariableNames()
        {
            return stringVariables.Keys.ToList();
        }

        static List<string> GetAllIntVariableNames()
        {
            return intVariables.Keys.ToList();
        }

        /// <summary>
        /// Expands the specified phrase, converting .NET types to mnemonics as needed. See 
        /// <seealso cref="DynamicListEntries"/> for a list of dynamic list elements added.
        /// </summary>
        /// <param name="phrase">The text to expand.</param>
        /// <returns>The expanded string.</returns>
        public static string Expand(string phrase)
        {
            List<string> allStringVars = GetAllStringVariableNames();
            List<string> allIntVars = GetAllIntVariableNames();

            const string escapedSlash = "___Slash___";
            const string escapedDollarSign = "___Dollar___";

            phrase = phrase.Replace("\\\\", escapedSlash);
            phrase = phrase.Replace("\\$", escapedDollarSign);
            
            allStringVars.ForEach(x => Replace(ref phrase, x, GetString(x)));
            allIntVars.ForEach(x => Replace(ref phrase, x, GetIntAsStr(x)));

            phrase = phrase.Replace(escapedDollarSign, "$");
            phrase = phrase.Replace(escapedSlash, "\\");
            return phrase;
        }

        private static void Replace(ref string phrase, string x, string replacement)
        {
            string searchTerm = $"${x}$";
            // phrase = m$genericType$$type$
            // searchTerm = $type$
            if (phrase.Contains(searchTerm))
            {
                if (x == "type" && IsTypeFullName(replacement))
                {
                    const string unlikelyCodeRushTypeMnemonic = "qz098";
                    dynamicListEntries.Add(new DynamicListEntry() { Mnemonic = unlikelyCodeRushTypeMnemonic, ListVarName = "Type", Value = replacement });
                    replacement = unlikelyCodeRushTypeMnemonic;
                }
                if (x == "generic1Type")
                    if (IsGeneric1TypeFullName(replacement))
                    {
                        const string unlikelyCodeRushGenericType1Mnemonic = "yr281";
                        dynamicListEntries.Add(new DynamicListEntry() { Mnemonic = unlikelyCodeRushGenericType1Mnemonic, ListVarName = "Generic1Type", Value = replacement });
                        replacement = unlikelyCodeRushGenericType1Mnemonic + ".";
                    }
                    else if (IsGeneric2TypeFullName(replacement))
                    {
                        const string unlikelyCodeRushGenericType2Mnemonic = "mw937";
                        dynamicListEntries.Add(new DynamicListEntry() { Mnemonic = unlikelyCodeRushGenericType2Mnemonic, ListVarName = "Generic2Type", Value = replacement });
                        replacement = unlikelyCodeRushGenericType2Mnemonic + ".s,";  // Make the string the first type parameter.
                    }
                phrase = phrase.Replace(searchTerm, replacement);
            }
        }

        static bool IsTypeFullName(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Length > 0 && char.IsUpper(value[0]);
        }

        static bool IsGeneric1TypeFullName(string value)
        {
            return IsTypeFullName(value) && value.EndsWith("<>");
        }

        static bool IsGeneric2TypeFullName(string value)
        {
            return IsTypeFullName(value) && value.EndsWith("<,>");
        }

        static List<DynamicListEntry> dynamicListEntries = new List<DynamicListEntry>();

        public static List<DynamicListEntry> DynamicListEntries { get => dynamicListEntries; }

        public static void ClearDynamicListEntries()
        {
            dynamicListEntries.Clear();
        }
    }
}
