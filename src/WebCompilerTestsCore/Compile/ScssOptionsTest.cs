using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class ScssOptionsTest
    {
        [TestMethod, TestCategory("SCSSOptions")]
        public void AutoPrefix()
        {
            var configs = ConfigHandler.GetConfigs("../../../../WebCompilerTest/artifacts/options/scss/scssconfigautoprefix.json");
            var result = SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual("test", result.AutoPrefix);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void LoadPaths()
        {
            var configs = ConfigHandler.GetConfigs("../../../../WebCompilerTest/artifacts/options/scss/scssconfigloadpaths.json");
            var result = SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual("/test/test.scss;/test/test2.scss", result.LoadPaths);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void StyleExpanded()
        {
            var configs = ConfigHandler.GetConfigs("../../../../WebCompilerTest/artifacts/options/scss/scssconfigexpanded.json");
            var result = SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual(SassStyle.Expanded.ToString().ToLowerInvariant(), result.Style);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void StyleCompressed()
        {
            var configs = ConfigHandler.GetConfigs("../../../../WebCompilerTest/artifacts/options/scss/scssconfigcompressed.json");
            var result = SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual(SassStyle.Compressed.ToString().ToLowerInvariant(), result.Style);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void SourceMap()
        {
            var configs = ConfigHandler.GetConfigs("../../../../WebCompilerTest/artifacts/options/scss/scssconfigsourcemap.json");
            var result = SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual(true, result.SourceMap);
        }

        //[TestMethod, TestCategory("SCSSOptions")]
        //public void SourceMapUrlsRelative()
        //{
        //    var configs = ConfigHandler.GetConfigs("../../../../WebCompilerTest/artifacts/options/scss/scssconfigsourcemapurlsrelative.json");
        //    var result = SassOptions.FromConfig(configs.ElementAt(0));
        //    Assert.AreEqual(SassSourceMapUrls.Relative.ToString().ToLowerInvariant(), result.SourceMapUrls);
        //}

        //[TestMethod, TestCategory("SCSSOptions")]
        //public void SourceMapUrlsAbsolute()
        //{
        //    var configs = ConfigHandler.GetConfigs("../../../../WebCompilerTest/artifacts/options/scss/scssconfigsourcemapurlsabsolute.json");
        //    var result = SassOptions.FromConfig(configs.ElementAt(0));
        //    Assert.AreEqual(SassSourceMapUrls.Absolute.ToString().ToLowerInvariant(), result.SourceMapUrls);
        //}
    }
}
