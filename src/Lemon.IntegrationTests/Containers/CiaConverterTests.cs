// Copyright (c) 2019 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Lemon.IntegrationTests.Containers
{
    using System.IO;
    using Lemon.Containers.Converters;
    using NUnit.Framework;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    [TestFixtureSource(typeof(TestData), nameof(TestData.CiaParams))]
    public class CiaConverterTests : Binary2ContainerTests
    {
        readonly string yamlPath;
        readonly string binaryPath;

        public CiaConverterTests(string yamlPath, string binaryPath)
        {
            this.yamlPath = yamlPath;
            this.binaryPath = binaryPath;

            TestDataBase.IgnoreIfFileDoesNotExist(binaryPath);
            TestDataBase.IgnoreIfFileDoesNotExist(yamlPath);
        }

        protected override BinaryFormat GetBinary()
        {
            TestContext.WriteLine(Path.GetFileName(binaryPath));
            var stream = DataStreamFactory.FromFile(binaryPath, FileOpenMode.Read);
            return new BinaryFormat(stream);
        }

        protected override NodeContainerInfo GetContainerInfo()
        {
            return NodeContainerInfo.FromYaml(yamlPath);
        }

        protected override IConverter<BinaryFormat, NodeContainerFormat> GetToContainerConverter()
        {
            return new BinaryCia2NodeContainer();
        }

        protected override IConverter<NodeContainerFormat, BinaryFormat> GetToBinaryConverter()
        {
            return null;
        }
    }
}
