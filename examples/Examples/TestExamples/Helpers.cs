namespace SolutionChecker
{
    using System.IO;
    using System.Text;

    class Helpers
    {
        static public string ReadOneWordFromStream(StreamReader reader) {
            var builder = new StringBuilder();
            var current = (char) 0;

            // Skip all whitespace characters
            while (!reader.EndOfStream && char.IsWhiteSpace(current = (char) reader.Read())) {}
            if (current == 0 || char.IsWhiteSpace(current)) return null;
            
            // Read all non-whitespace characters
            do { builder.Append(current); }
            while (!reader.EndOfStream && !char.IsWhiteSpace(current = (char) reader.Read()));

            return builder.Length != 0 ? builder.ToString() : null;
        }

        static public bool CompareTwoFiles(StreamReader input1, StreamReader input2)
        {
            for (;;) {
                var el1 = ReadOneWordFromStream(input1);
                var el2 = ReadOneWordFromStream(input2);

                // End files
                if (el1 == null && el2 == null)
                    return true;

                // Different length check
                if (el1 == null ^ el2 == null)
                    return false;

                // Different element check
                if (el1 != el2)
                    return false;
            }
        }
    }
}
