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
namespace SceneGate.Lemon.IntegrationTests.Containers
{
    using System.IO;
    using NUnit.Framework;
    using SceneGate.Lemon.Containers.Formats;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    [TestFixtureSource(typeof(TestData), nameof(TestData.NcchParams))]
    public class NcchConverterTests
    {
        readonly string yamlPath;
        readonly string binaryPath;
        readonly int offset;
        readonly int size;

        Node actualNode;
        Ncch actual;
        NcchTestInfo expected;

        public NcchConverterTests(string yamlPath, string binaryPath, int offset, int size)
        {
            this.yamlPath = yamlPath;
            this.binaryPath = binaryPath;
            this.offset = offset;
            this.size = size;
        }

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            TestDataBase.IgnoreIfFileDoesNotExist(binaryPath);
            TestDataBase.IgnoreIfFileDoesNotExist(yamlPath);

            var stream = DataStreamFactory.FromFile(binaryPath, FileOpenMode.Read, offset, size);
            actualNode = new Node("actual", new BinaryFormat(stream));

            Assert.That(() => actualNode.TransformTo<Ncch>(), Throws.Nothing);
            actual = actualNode.GetFormatAs<Ncch>();

            string yaml = File.ReadAllText(yamlPath);
            expected = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build()
                .Deserialize<NcchTestInfo>(yaml);
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {
            actualNode?.Dispose();
            Assert.That(DataStream.ActiveStreams, Is.EqualTo(0));
        }

        [Test]
        public void ValidateHeader()
        {
            var header = actual.Header;
            Assert.That(header.Signature, Has.Length.EqualTo(expected.SignatureLength));
        }

        [Test]
        public void ValidateRegions()
        {
            Assert.That(
                actual.Root.Children,
                Has.Count.EqualTo(expected.AvailableRegions.Length));

            for (int i = 0; i < actual.Root.Children.Count; i++) {
                var child = actual.Root.Children[i];
                Assert.That(child.Name, Is.EqualTo(expected.AvailableRegions[i]));
                Assert.That(child.Stream.Offset, Is.EqualTo(offset + expected.RegionsOffset[i]));
                Assert.That(child.Stream.Length, Is.EqualTo(expected.RegionsSize[i]));
            }
        }
    }
}
