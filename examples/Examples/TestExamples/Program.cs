namespace SolutionChecker
{
    using System;
    using System.IO;

    class Program
    {
        #region # Entry Point #

        static void Main()
        {
            const string path = "../../tests";
            const SolutionChecker.ShowFileFlags showFileMode =
                SolutionChecker.ShowFileFlags.ExceptShow |
                SolutionChecker.ShowFileFlags.OutputShot;

            var test = new SolutionChecker(Example, path, showFileMode) {
                {"checkOnNull.in", "checkOnNull.ex", "checkOnNull.out"},
                "10elements",
                "negativeNumbers",
                "badTest",
                {"bigBadTest", SolutionChecker.ShowFileFlags.None},
            };

            test.Check();

            if (!test.ErrorsOccuried)
            {
                Console.WriteLine("All Tests passed successfully");
            }
            else
            {
                Console.WriteLine("Test has following statuses:");
                test.MessageList.ForEach(Console.WriteLine);
            }
        }

        #endregion # Entry Point #

        #region # Methods #

        private static void Example(StreamReader input, StreamWriter output)
        {
            var sum = 0;
            string current;

            while ((current = Helpers.ReadOneWordFromStream(input)) != null)
                sum += int.Parse(current);

            output.WriteLine(sum);
        } 

        #endregion # Methods #
    }
}
