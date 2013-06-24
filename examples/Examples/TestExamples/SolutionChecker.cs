namespace SolutionChecker
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class SolutionChecker : IEnumerable {

        #region # Types #

        public delegate void Callback(StreamReader input, StreamWriter output);

        [Flags]
        public enum ShowFileFlags {
            None = 0,
            InputShow = 1 << 0,
            ExceptShow = 1 << 1,
            OutputShot = 1 << 2,
            All = InputShow | ExceptShow | OutputShot
        }

        private class TestElement {
            public string InputFile { get; set; }
            public string ExceptFile { get; set; }
            public string OutputFile { get; set; }
            public ShowFileFlags? ShowFileMode { get; set; }
        } 

        #endregion # Types #

        #region # Fields #

        private readonly List<TestElement> _testElements;
        private readonly string _path;
        private readonly ShowFileFlags _showFileMode;
        private readonly Callback _callback;
        
        #endregion # Fields #

        #region # Properties #

        public List<string> MessageList { get; set; }
        public bool ErrorsOccuried { get; set; }

        #endregion # Properties #

        #region # Constructors #

        public SolutionChecker(Callback callback, string path = "./",
            ShowFileFlags showFileMode = ShowFileFlags.None)
        {
            _path = path;
            _showFileMode = showFileMode;
            _callback = callback;
            _testElements = new List<TestElement>();

            ErrorsOccuried = false;
            MessageList = new List<string>();
        }

        #endregion # Constructors #

        #region # Methods #

        public IEnumerator GetEnumerator()
        {
            return _testElements.GetEnumerator();
        }

        public void Add(string input, string except, string output,
            ShowFileFlags? showFileMode = null)
        {
            _testElements.Add(new TestElement
            {
                InputFile = input,
                ExceptFile = except,
                OutputFile = output,
                ShowFileMode = showFileMode
            });
        }

        public void Add(string mask, ShowFileFlags? showFileMode = null)
        {
            Add(mask + ".in", mask + ".ex", mask + ".out", showFileMode);
        }

        public void Check() {
            ErrorsOccuried = false;
            MessageList.Clear();

            foreach (var testElement in _testElements) {
                var inputPath = Path.Combine(_path, testElement.InputFile);
                var exceptPath = Path.Combine(_path, testElement.ExceptFile);
                var outputPath = Path.Combine(_path, testElement.OutputFile);

                // Run test
                using (var input = new StreamReader(inputPath))
                using (var output = new StreamWriter(outputPath))
                    _callback(input, output);

                // Check result
                bool result;
                using (var input1 = new StreamReader(exceptPath))
                using (var input2 = new StreamReader(outputPath))
                    result = Helpers.CompareTwoFiles(input1, input2);

                // Generate passed message
                if (result)
                {
                    MessageList.Add("Test passed: " + testElement.InputFile);
                    continue;
                }

                // Generate error message
                ErrorsOccuried = true;
                var error = new StringBuilder();
                error.AppendLine(
                    "Error occurred on test: " + testElement.InputFile);

                var showFileMode = testElement.ShowFileMode ?? _showFileMode;
                Action<ShowFileFlags, string, string> addFile =
                    (mode, message, path) => {
                        if ((showFileMode & mode) == ShowFileFlags.None) return;
                        error.AppendLine(message);
                        error.AppendLine(File.ReadAllText(path));
                    };

                addFile(ShowFileFlags.InputShow, "Input file:", inputPath);
                addFile(ShowFileFlags.ExceptShow, "Except file:", exceptPath);
                addFile(ShowFileFlags.OutputShot, "Output file:", outputPath);

                MessageList.Add(error.ToString());
            }
        }

        #endregion # Methods #
    }
}
