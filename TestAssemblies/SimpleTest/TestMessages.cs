using System.Collections.Generic;

namespace SimpleTest
{
    using System.Runtime.CompilerServices;
    using System.Text;

    public static class TestMessages
    {
        private static readonly IList<string> messages = new List<string>();

        public static void Clear()
        {
            messages.Clear();
        }

        public static void RecordMethodBody(object[] arguments, [CallerMemberName]string caller = "")
        {
            string message = new StringBuilder()
                .Append(caller)
                .Append("(")
                    .Append(string.Join(", ", arguments))
                .Append(")")
                .ToString();

            messages.Add(message);
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