using System.Collections.Generic;

namespace SimpleTest
{
    using System.Reflection;

    public static class TestMessages
    {
        private static readonly IList<string> messages = new List<string>();

        public static void Clear()
        {
            messages.Clear();
        }

        public static void RecordMethodBody(MethodBase method)
        {
            messages.Add(method.Name);
        }

        public static void Record(string message)
        {
            messages.Add(message);
        }

        public static IList<string> Messages
        {
            get { return messages; }
        }
    }
}