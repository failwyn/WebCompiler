using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class HandleBarsTest
    {
        private ConfigFileProcessor _processor;

        [TestInitialize]
        public void Setup()
        {
            _processor = new ConfigFileProcessor();
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete("../../../../WebCompilerTest/artifacts/handlebars/test.js");
            File.Delete("../../../../WebCompilerTest/artifacts/handlebars/test.min.js");
            File.Delete("../../../../WebCompilerTest/artifacts/handlebars/test.js.map");
            File.Delete("../../../../WebCompilerTest/artifacts/handlebars/error.js");
            File.Delete("../../../../WebCompilerTest/artifacts/handlebars/error.min.js");
            File.Delete("../../../../WebCompilerTest/artifacts/handlebars/error.js.map");
            File.Delete("../../../../WebCompilerTest/artifacts/handlebars/_partial.js");
            File.Delete("../../../../WebCompilerTest/artifacts/handlebars/_partial.min.js");
            File.Delete("../../../../WebCompilerTest/artifacts/handlebars/_partial.js.map");
        }

        [TestMethod, TestCategory("HandleBars")]
        public void CompileHandleBars()
        {
            var result = _processor.Process("../../../../WebCompilerTest/artifacts/handlebarsconfig.json");
            FileInfo js = new FileInfo("../../../../WebCompilerTest/artifacts/handlebars/test.js");
            FileInfo min = new FileInfo("../../../../WebCompilerTest/artifacts/handlebars/test.min.js");
            FileInfo map = new FileInfo("../../../../WebCompilerTest/artifacts/handlebars/test.js.map");
            Assert.IsTrue(js.Exists, "Output file doesn't exist");
            Assert.IsFalse(min.Exists, "Min file exists");
            Assert.IsTrue(map.Exists, "Map file doesn't exist");
            Assert.IsTrue(js.Length > 5);
            Assert.IsTrue(map.Length > 5);
        }

        [TestMethod, TestCategory("HandleBars")]
        public void CompileHandleBarsPartial()
        {
            var result = _processor.Process("../../../../WebCompilerTest/artifacts/handlebarsconfigPartial.json");
            FileInfo js = new FileInfo("../../../../WebCompilerTest/artifacts/handlebars/_partial.js");
            FileInfo min = new FileInfo("../../../../WebCompilerTest/artifacts/handlebars/_partial.min.js");
            Assert.IsTrue(js.Exists, "Output file doesn't exist");
            Assert.IsTrue(min.Exists, "Min file doesn't exists");
            Assert.IsTrue(js.Length > 5);
            Assert.IsTrue(File.ReadAllText(js.FullName).Contains("Handlebars.partials['partial'] = template("), "Name of partial template is invalid");
        }

        [TestMethod, TestCategory("HandleBars")]
        public void CompileHandleBarsWithError()
        {
            var result = _processor.Process("../../../../WebCompilerTest/artifacts/handlebarsconfigError.json");
            var error = result.First().Errors[0];
            Assert.AreEqual(2, error.LineNumber);
            Assert.AreEqual("Parse error", error.Message);
        }
    }
}
