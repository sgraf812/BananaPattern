using System;
using System.Diagnostics;
using System.Xml.Linq;
using BananaPattern;
using BananaTest;
using BananaXmlOffset;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FurBot.Test
{
    [TestClass]
    [DeploymentItem("Patterns.xsd")]
    public class XmlOffsetProviderTest
    {
        private FakeFileInfoAdapter _infoFake;
        private XmlOffsetProvider _provider;
        private Mock<IMemory> _memoryMock;

        public TestContext TestContext { get; set; }

        public void Setup()
        {
            Setup(@"<?xml version=""1.0"" encoding=""UTF-8""?>
            <Patterns xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
             xsi:noNamespaceSchemaLocation=""Patterns.xsd"">
              <Pattern>
               <Name>TestPattern</Name>
               <Pattern>CC</Pattern>
              </Pattern>
              <Offset>
               <Name>TestOffset</Name>
               <Operations>
                <PatternResult Name=""TestPattern""/>
               </Operations>
              </Offset>
              <Offset>
               <Name>TestOffsetWithIndirection</Name>
               <Operations>
                <OperatorResult Type=""Lea"" Value=""Byte"">
                 <PatternResult Name=""TestPattern""/>
                </OperatorResult>
               </Operations>
              </Offset>
              <Offset>
               <Name>TestOffsetBinary</Name>
               <Operations>
                <BinaryOperatorResult>
                 <Type>Add</Type>
                 <Value>
                  <ConstantResult>0C</ConstantResult>
                 </Value>
                 <Target>
                  <ConstantResult>B0</ConstantResult>
                 </Target>
                </BinaryOperatorResult>
               </Operations>
              </Offset>
            </Patterns>
            ");
        }

        public void Setup(string document)
        {
            var contextMock = SetupContextMock();
            _infoFake = SetupFakeFileInfo(document);
            _provider = new XmlOffsetProvider(contextMock.Object, _infoFake);
        }

        private Mock<IBotProcessContext> SetupContextMock()
        {
            var contextMock = new Mock<IBotProcessContext>();
            _memoryMock = contextMock.ReturnMockMember(x => x.Memory);
            contextMock.Setup(x => x.TargetProcess).Returns(Process.GetCurrentProcess());
            return contextMock;
        }

        private FakeFileInfoAdapter SetupFakeFileInfo(string document)
        {
            FakeFileInfoAdapter infoFake = new FakeFileInfoAdapter(document)
            {
                LastWriteTime = DateTime.Now
            };
            return infoFake;
        }

        [TestMethod]
        public void Constructor_IfPatternDocumentEmpty_BuildsAValidDocument()
        {
            Setup("");

            Assert.IsNotNull(_provider._patternDocument);
            Assert.AreEqual("Patterns", _provider._patternDocument.Root.Name);
        }

        [TestMethod]
        public void Constructor_ValidatesValidDocument_Succeeds()
        {
            XDocument validDocument = new XDocument(new XElement("Patterns"));
            Setup(validDocument.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(OffsetException))]
        public void Constructor_ValidatesInvalidDocument_Throws()
        {
            XDocument invalidDocument = new XDocument(new XElement("Foo", ""));
            Setup(invalidDocument.ToString());
        }

        [TestMethod]
        public void CanResolve_SomeUnresolvableSymbol_ReturnsFalse()
        {
            Setup();

            bool canResolve = _provider.CanResolve("cannot resolve this");

            Assert.IsFalse(canResolve);
        }

        [TestMethod]
        public void CanResolve_TestPattern_ReturnsTrue()
        {
            Setup();

            bool canResolve = _provider.CanResolve("TestOffset");

            Assert.IsTrue(canResolve);
        }

        [TestMethod]
        [ExpectedException(typeof(OffsetException))]
        public void GetAddress_SomeUnresolvableSymbol_Throws()
        {
            Setup();

            _provider.GetAddress("cannot resolve this");
        }

        [TestMethod]
        public unsafe void GetAddress_TestPattern_ReturnsMatch()
        {
            Setup();

            _memoryMock.SetReturnsDefault<byte[]>(new byte[] { 0xBC, 0xCC, 0xDD, 0xFF });

            IntPtr match = _provider.GetAddress("TestOffset");
            match = SubstractMainModuleBase(match);

            Assert.AreEqual(new IntPtr(1), match);
        }

        private static IntPtr SubstractMainModuleBase(IntPtr match)
        {
            match = new IntPtr(match.ToInt64() - Process.GetCurrentProcess().MainModule.BaseAddress.ToInt64());
            return match;
        }

        [TestMethod]
        public unsafe void GetAddress_TestPatternWithIndirection_ReturnsMatch()
        {
            Setup();

            byte expected = 0x12;
            byte[] bytes = new byte[] { 0xBC, 0xCC, 0xDD, 0xFF };
            fixed (byte* b = bytes)
            {
                _memoryMock.SetReturnsDefault<byte[]>(bytes);
                _memoryMock.SetReturnsDefault<byte>(expected);

                IntPtr match = _provider.GetAddress("TestOffsetWithIndirection");

                Assert.AreEqual(new IntPtr(expected), match);
            }
        }

        [TestMethod]
        public void GetAddress_BinaryOperatorResult_ReturnsCorrectResult()
        {
            Setup();

            IntPtr expected = new IntPtr(0xBC); // see XDocument: 0xB0 + 0x0C

            IntPtr actual = _provider.GetAddress("TestOffsetBinary");

            Assert.AreEqual(expected, actual);
        }
    }
}
