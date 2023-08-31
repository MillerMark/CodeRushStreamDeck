using System;
using System.Reflection;
using StreamDeckLib;

namespace CodeRushStreamDeck.Startup
{
    public static class ManagerExtensions
    {
        public static string GetInstanceUuid(this ConnectionManager manager)
        {
            FieldInfo field = manager.GetType().GetField("_uuid", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                var value = field.GetValue(manager);
                if (value != null)
                    return value.ToString();
            }
            return string.Empty;
        }
    }
}
