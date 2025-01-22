using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Linq;

using WebCompiler;

namespace WebCompilerTest.Config
{
    [TestClass]
    public class ConfigFileProcessorTest
    {
        private const string configFileWithExtensions = "../../artifacts/configwithextensions.json";

        [TestMethod, TestCategory("Config")]
        public void IsFileConfigured_WhenSourceFileMatchTheExtension_ShouldReturnConfigForThatFile()
        {
            var configFile = new FileInfo(configFileWithExtensions);
            var configFileFolder = new FileInfo(configFileWithExtensions).DirectoryName;
            var test1FilePath = new FileInfo("../../artifacts/scss/test1.razor.scss");
            var test2FilePath = new FileInfo("../../artifacts/scss/test2.razor.scss");
            var expectedTest1InputFile = test1FilePath.FullName.Replace(configFileFolder, "").Substring(1);
            var expectedTest2InputFile = test2FilePath.FullName.Replace(configFileFolder, "").Substring(1);


            var test1Config = ConfigFileProcessor.IsFileConfigured(configFile.FullName, test1FilePath.FullName).FirstOrDefault(x => x.InputFile == expectedTest1InputFile);
            var test2Config = ConfigFileProcessor.IsFileConfigured(configFile.FullName, test2FilePath.FullName).FirstOrDefault(x => x.InputFile == expectedTest2InputFile);

            Assert.IsNotNull(test1Config);
            Assert.IsNotNull(test2Config);
        }

        [TestMethod, TestCategory("Config")]
        public void IsFileConfigured_WhenSourceFileMatchTheExtensionAndIgnoreExtensionConfigIsTrue_ShouldNotReturnConfigForThatFile()
        {
            var configFile = new FileInfo(configFileWithExtensions);
            var configFileFolder = new FileInfo(configFileWithExtensions).DirectoryName;
            var test1FilePath = new FileInfo("../../artifacts/scss/test1.razor.scss");
            var test2FilePath = new FileInfo("../../artifacts/scss/test2.razor.scss");
            var expectedTest1InputFile = test1FilePath.FullName.Replace(configFileFolder, "").Substring(1);
            var expectedTest2InputFile = test2FilePath.FullName.Replace(configFileFolder, "").Substring(1);


            var test1Config = ConfigFileProcessor.IsFileConfigured(configFile.FullName, test1FilePath.FullName, ignoreExtensionConfig: true).FirstOrDefault(x => x.InputFile == expectedTest1InputFile);
            var test2Config = ConfigFileProcessor.IsFileConfigured(configFile.FullName, test2FilePath.FullName, ignoreExtensionConfig: true).FirstOrDefault(x => x.InputFile == expectedTest2InputFile);

            Assert.IsNull(test1Config);
            Assert.IsNull(test2Config);
        }
    }
}
