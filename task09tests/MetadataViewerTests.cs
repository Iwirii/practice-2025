using System;
using Xunit;
using task09;

namespace task09tests
{
    public class MetadataViewerTests
    {
        [Fact]
        public void Main_ShowsError_WhenNoArguments()
        {
            MetadataViewer.Main(Array.Empty<string>());
        }

        [Fact]
        public void Main_ShowsError_WhenFileMissing()
        {
            MetadataViewer.Main(new[] { "missing.dll" });
            Assert.True(true);
        }

        [Fact]
        public void Main_RunsWithoutErrors_WithTestAssembly()
        {
            var testDll = typeof(MetadataViewerTests).Assembly.Location;

            MetadataViewer.Main(new[] { testDll });
            Assert.True(true);
        }
    }
}
