using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebCompilerTest.Config
{
    [TestClass]
    public class ConfigTest
    {
        private WebCompiler.Config _config;

        private const string dummyConfigFile = "../../artifacts/config/dummy.json";
        private const string inputFile = "../../artifacts/config/compilationrequired.scss";
        private const string outputFile = "../../artifacts/config/compilationrequired.css";
        private const string firstLevelDependencyFile = "../../artifacts/config/dependencies/foo.scss";
        private const string secondLevelDependencyFile = "../../artifacts/config/dependencies/sub/bar.scss";

        private const string inputFileWildcardExtension = "*.razor.scss";
        private const string outputFileWildcardExtension = "*.razor.css";

        private readonly FileInfo _inputFileInfo = new FileInfo(inputFile);
        private readonly FileInfo _outputFileInfo = new FileInfo(outputFile);
        private readonly FileInfo _firstLevelDependencyFileInfo = new FileInfo(firstLevelDependencyFile);
        private readonly FileInfo _secondLevelDependencyFileInfo = new FileInfo(secondLevelDependencyFile);

        private readonly Dictionary<FileInfo, DateTime> _originalLastWriteTimes = new Dictionary<FileInfo, DateTime>();

        private DateTime _olderWriteTime;
        private DateTime _newerWriteTime;

        [TestInitialize]
        public void Setup()
        {
            var configFileInfo = new FileInfo(dummyConfigFile);

            _config = new WebCompiler.Config
            {
                FileName = configFileInfo.FullName,
                InputFile = _inputFileInfo.FullName,
                OutputFile = _outputFileInfo.FullName
            };

            // Create dummy output file, only last write time is checked
            File.WriteAllText(outputFile, "");

            // Backup last write times for cleanup
            _originalLastWriteTimes.Add(_inputFileInfo, _inputFileInfo.LastWriteTimeUtc);
            _originalLastWriteTimes.Add(_firstLevelDependencyFileInfo, _firstLevelDependencyFileInfo.LastWriteTimeUtc);
            _originalLastWriteTimes.Add(_secondLevelDependencyFileInfo, _secondLevelDependencyFileInfo.LastWriteTimeUtc);

            var utcNow = DateTime.UtcNow;

            _inputFileInfo.LastWriteTimeUtc = utcNow;
            _outputFileInfo.LastWriteTimeUtc = utcNow;
            _firstLevelDependencyFileInfo.LastWriteTimeUtc = utcNow;
            _secondLevelDependencyFileInfo.LastWriteTimeUtc = utcNow;

            _olderWriteTime = utcNow.AddHours(-1);
            _newerWriteTime = utcNow.AddHours(1);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);

            foreach (var entry in _originalLastWriteTimes)
            {
                entry.Key.LastWriteTimeUtc = entry.Value;
            }
        }

        [TestMethod, TestCategory("Config")]
        public void CompilationRequired_OutputNewerThanEverything_DoesNotRequireCompilation()
        {
            _inputFileInfo.LastWriteTimeUtc = _olderWriteTime;
            _firstLevelDependencyFileInfo.LastWriteTimeUtc = _olderWriteTime;
            _secondLevelDependencyFileInfo.LastWriteTimeUtc = _olderWriteTime;

            var compilationRequired = _config.CompilationRequired();

            Assert.AreEqual(false, compilationRequired);
        }

        [TestMethod, TestCategory("Config")]
        public void CompilationRequired_InputNewerThanOutput_RequiresCompilation()
        {
            _inputFileInfo.LastWriteTimeUtc = _newerWriteTime;
            _firstLevelDependencyFileInfo.LastWriteTimeUtc = _olderWriteTime;
            _secondLevelDependencyFileInfo.LastWriteTimeUtc = _olderWriteTime;

            var compilationRequired = _config.CompilationRequired();

            Assert.AreEqual(true, compilationRequired);
        }

        [TestMethod, TestCategory("Config")]
        public void CompilationRequired_FirstLevelDependencyNewerThanOutput_RequiresCompilation()
        {
            _inputFileInfo.LastWriteTimeUtc = _olderWriteTime;
            _firstLevelDependencyFileInfo.LastWriteTimeUtc = _newerWriteTime;
            _secondLevelDependencyFileInfo.LastWriteTimeUtc = _olderWriteTime;

            var compilationRequired = _config.CompilationRequired();

            Assert.AreEqual(true, compilationRequired);
        }

        [TestMethod, TestCategory("Config")]
        public void CompilationRequired_SecondLevelDependencyNewerThanOutput_RequiresCompilation()
        {
            _inputFileInfo.LastWriteTimeUtc = _olderWriteTime;
            _firstLevelDependencyFileInfo.LastWriteTimeUtc = _olderWriteTime;
            _secondLevelDependencyFileInfo.LastWriteTimeUtc = _newerWriteTime;

            var compilationRequired = _config.CompilationRequired();

            Assert.AreEqual(true, compilationRequired);
        }

        [TestMethod, TestCategory("Config")]
        public void InputFileExtension_WhenInputFileIsWildcardExtension_StripsAsterisk()
        {
            var config = new WebCompiler.Config()
            {
                InputFile = "*.razor.scss",
            };

            Assert.AreEqual(".razor.scss", config.InputExtension);
        }

        [TestMethod, TestCategory("Config")]
        public void OutputFileExtension_WhenOutputFileIsWildcardExtension_StripsAsterisk()
        {
            var config = new WebCompiler.Config()
            {
                OutputFile = "*.razor.css",
            };

            Assert.AreEqual(".razor.css", config.OutputExtension);
        }

        [TestMethod, TestCategory("Config")]
        public void IsExtensionPattern_WhenInputFileIsWildcardExtension_ReturnsTrue()
        {
            var config = new WebCompiler.Config()
            {
                InputFile = "*.razor.scss",
            };

            Assert.AreEqual(true, config.IsExtensionPattern);
        }

        [TestMethod, TestCategory("Config")]
        public void IsExtensionPattern_WhenInputFileIsNotWildcardExtension_ReturnsFalse()
        {
            var config = new WebCompiler.Config()
            {
                InputFile = "somefile.razor.scss",
            };

            Assert.AreEqual(false, config.IsExtensionPattern);
        }

        [TestMethod, TestCategory("Config")]
        public void IsExtensionPattern_WhenInputFileIsNotSet_ReturnsFalse()
        {
            var config = new WebCompiler.Config()
            {
            };

            Assert.AreEqual(false, config.IsExtensionPattern);
        }
    }
}
