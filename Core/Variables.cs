using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeRushStreamDeck
{
    public static class Variables
    {
        static Dictionary<string, string> stringVariables = new();
        static Dictionary<string, int> intVariables = new();

        public static event EventHandler<VarEventArgs<int>> IntVarChanged;
        public static event EventHandler<VarEventArgs<string>> StringVarChanged;

        static void OnIntVarChanging(string varName, int value)
        {
            IntVarChanged?.Invoke(null, new VarEventArgs<int>(varName, value));
        }

        static void OnStringVarChanging(string varName, string value)
        {
            StringVarChanged?.Invoke(null, new VarEventArgs<string>(varName, value));
        }

        public static void SetInt(string varName, int value = 0)
        {
            if (intVariables.ContainsKey(varName))
                if (intVariables[varName] != value)
                    OnIntVarChanging(varName, value);
            intVariables[varName] = value;
        }

        public static void SetString(string varName, string value = "")
        {
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
    }
}
