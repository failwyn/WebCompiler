using System;
namespace WebCompilerTestsCore
{
     [TestClass]
    public class FileHelpersTest
    {
        [DataTestMethod( )]
        [DataRow("/a", "/a", "./a")]
        [DataRow("/a", "/a/b", "./b")]
        [DataRow("/a", "/a/b/c", "./b/c")]
        public void TestCases( String basePath, String filePath, String expectedResult)
        {
            var result = WebCompiler.FileHelpers.MakeRelative(basePath, filePath);
            Assert.AreEqual( expectedResult, result );
        }
    }
}

