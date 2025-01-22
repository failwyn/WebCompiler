using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Linq;

using WebCompiler;

namespace WebCompilerTest.Config
{
    [TestClass]
    public class ConfigHandlerTest
    {
        private ConfigHandler _handler;

        private const string originalConfigFile = "../../artifacts/config/originalcoffeeconfig.json";
        private const string processingConfigFile = "../../artifacts/config/coffeeconfig.json";

        private const string configFileWithExtensions = "../../artifacts/configwithextensions.json";

        [TestInitialize]
        public void Setup()
        {
            _handler = new ConfigHandler();

            File.Copy(originalConfigFile, processingConfigFile, true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(processingConfigFile))
                File.Delete(processingConfigFile);
        }

        [TestMethod, TestCategory("Config")]
        public void AddConfig()
        {
            var newConfig = new WebCompiler.Config();
            const string newInputFileName = "newInputFile";
            newConfig.InputFile = newInputFileName;

            _handler.AddConfig(processingConfigFile, newConfig);

            var configs = ConfigHandler.GetConfigs(processingConfigFile);
            Assert.AreEqual(2, configs.Count());
            Assert.AreEqual(newInputFileName, configs.ElementAt(1).InputFile);
        }

        [TestMethod, TestCategory("Config")]
        public void NonExistingConfigFileShouldReturnEmptyList()
        {
            var expectedResult = Enumerable.Empty<WebCompiler.Config>();

            var result = ConfigHandler.GetConfigs("../NonExistingFile.config");

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod, TestCategory("Config")]
        public void GetConfig_WhenExpandExtensionsIsNotProvided_ReturnsConfigsIncludingFilesMatchingExtensions()
        {
            var configs = ConfigHandler.GetConfigs(configFileWithExtensions);
            var configFileFolder = new FileInfo(configFileWithExtensions).DirectoryName;
            var test1FilePath = new FileInfo("../../artifacts/scss/test1.razor.scss");
            var test2FilePath = new FileInfo("../../artifacts/scss/test2.razor.scss");
            var expectedTest1InputFile = test1FilePath.FullName.Replace(configFileFolder, "").Substring(1);
            var expectedTest2InputFile = test2FilePath.FullName.Replace(configFileFolder, "").Substring(1);

            var test1Config = configs.SingleOrDefault(x => x.IsFromExtensionPattern && x.InputFile == expectedTest1InputFile);
            var test2Config = configs.SingleOrDefault(x => x.IsFromExtensionPattern && x.InputFile == expectedTest2InputFile);


            Assert.IsNotNull(test1Config);
            Assert.IsNotNull(test2Config);
            
            Assert.IsTrue(test1Config.IsFromExtensionPattern);
            Assert.IsTrue(test2Config.IsFromExtensionPattern);
            
            Assert.AreEqual(test1Config.OutputFile ,expectedTest1InputFile.Replace(".scss",".css"));
            Assert.AreEqual(test2Config.OutputFile , expectedTest2InputFile.Replace(".scss",".css"));
            
            Assert.IsFalse((bool)test1Config.Minify["enabled"]);
            Assert.IsFalse((bool)test1Config.Minify["enabled"]);

            Assert.AreEqual(3, configs.Count());
            Assert.AreEqual(0, configs.Where(x => x.IsExtensionPattern).Count());
        }

        [TestMethod, TestCategory("Config")]
        public void GetConfig_WhenExpandExtensionsIsFalse_ReturnsConfigsWithoutFilesMatchingExtensions()
        {
            var configs = ConfigHandler.GetConfigs(configFileWithExtensions, expandExtensions: false);

            Assert.AreEqual(2, configs.Count());
            Assert.AreEqual(1, configs.Where(x => x.IsExtensionPattern).Count());
        }

        [TestMethod, TestCategory("Config")]
        public void GetConfig_WhenExpandExtensionsIsFalseAndSourceFileProvided_ReturnsConfigsWithoutFilesMatchingExtensions()
        {
            var configs = ConfigHandler.GetConfigs(configFileWithExtensions, sourceFile: "newfile.razor.scss", expandExtensions: false);

            Assert.AreEqual(2, configs.Count());
            Assert.AreEqual(1, configs.Where(x => x.IsExtensionPattern).Count());
        }

        [TestMethod, TestCategory("Config")]
        public void GetConfig_WhenSourceFileWithValidExtensionIsProvidedAndCacheAlreadyFilled_ReturnsConfigsIncludingFile()
        {
            var newFile = "newFile.razor.scss";

            // trigger loading existing files to dictionary cache
            var configs = ConfigHandler.GetConfigs(configFileWithExtensions);

            configs = ConfigHandler.GetConfigs(configFileWithExtensions, newFile);

            Assert.AreEqual(4, configs.Count());
            Assert.AreEqual(1, configs.Where(x => x.InputFile.Contains(newFile)).Count());
        }

    }
}
