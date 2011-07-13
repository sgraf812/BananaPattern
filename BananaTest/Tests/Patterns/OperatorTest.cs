using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using BananaPattern;
using BananaPattern.Operators;

namespace BananaTest.Tests.Patterns
{
    [TestClass]
    public class OperatorTest
    {
        [TestMethod]
        [ExpectedException(typeof(PatternException))]
        public void RegisterOperator_WasntRegisteredBefore_ReturnsNull()
        {
            Operator op = Operator.Create("not registered", () => "some initialization value", () => IntPtr.Zero.ToString("X"));
        }

        [TestMethod]
        public void RegisterOperator_WasRegisteredBefore_ReturnsNewInstanceOfOperator()
        {
            string id = "Stub";
            Operator.RegisterOperator(id, typeof(StubOperator));

            Operator op = Operator.Create(id, () => "some initialization value", () => IntPtr.Zero.ToString("X"));
            Operator.UnregisterOperator(id); //  clean up

            Assert.IsNotNull(op);
        }

        [TestMethod]
        public void GetRegisteredIdentifier_TypeIsRegistered_ReturnsIdentifier()
        {
            string expected = "Stub";
            Type typeOfOperator = typeof(StubOperator);
            Operator.RegisterOperator(expected, typeOfOperator);

            string actual = Operator.GetRegisteredIdentifier(typeOfOperator);
            Operator.UnregisterOperator(expected); //  clean up

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetRegisteredIdentifier_TypeIsNotRegistered_ReturnsIdentifier()
        {
            Type typeOfOperator = typeof(int);

            string id = Operator.GetRegisteredIdentifier(typeOfOperator);

            Assert.IsNull(id);
        }

        [TestMethod]
        public void GetRegisteredIdentifier_AddModifierUnregisteredBefore_RescansForTypes()
        {
            string expected = "Add";
            Type typeOfOperator = typeof(AddOperator);

            Operator.UnregisterOperator(expected);
            string actual = Operator.GetRegisteredIdentifier(typeOfOperator);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(PatternException))]
        public void Create_OperatorNotRegistered_Throws()
        {
            Operator.Create("this identifier doesn't exist", () => "some value", () => IntPtr.Zero.ToString("X"));
        }

        [TestMethod]
        public void Create_LeaOperator_Succeeds()
        {
            Operator op = Operator.Create("Lea", () => "", () => IntPtr.Zero.ToString("X"));

            Assert.IsNotNull(op);
        }

        [TestMethod]
        public void Create_NullTargetValue_InitializesTargetToZero()
        {
            Operator op = Operator.Create("Lea", () => "", () => null);

            Assert.AreEqual(IntPtr.Zero, op.Target);
        }
    }

    [TestClass]
    public class AddOperatorTest
    {
        [TestMethod]
        public void DefaultConstructor_Called_InitializesOffsetToZero()
        {
            var op = new AddOperator();

            Assert.AreEqual(0, op.Offset);
        }

        [TestMethod]
        public void IntegerConstructor_Called_InitializesOffsetToValue()
        {
            var op = new AddOperator(IntPtr.Zero, 50);

            Assert.AreEqual(50, op.Offset);
        }

        [TestMethod]
        public void StringConstructor_NullString_InitializesToDefault()
        {
            var op = new AddOperator(() => null, () => IntPtr.Zero.ToString("X"));

            Assert.AreEqual(0, op.Offset);
        }

        [TestMethod]
        public void StringConstructor_EmptyString_InitializesToDefault()
        {
            var op = new AddOperator(() => "", () => IntPtr.Zero.ToString("X"));

            Assert.AreEqual(0, op.Offset);
        }

        [TestMethod]
        public void StringConstructor_ValidString_InitializesToParsedValue()
        {
            var op = new AddOperator(() => "C0", () => IntPtr.Zero.ToString("X"));

            Assert.AreEqual(0xC0, op.Offset);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StringConstructor_InvalidString_ThrowsWhenEvaluated()
        {
            int offset = new AddOperator(() => "invalid value", () => IntPtr.Zero.ToString("X")).Offset;
        }

        [TestMethod]
        public void Execute_SomePointer_Succeeds()
        {
            var memMock = new Mock<IMemory>();
            IntPtr pointer = new IntPtr(0xbeef);
            IntPtr expected = pointer + 50;
            var op = new AddOperator(pointer, 50);

            IntPtr actual = op.Execute(memMock.Object);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Execute_NullMemory_Succeeds()
        {
            IntPtr pointer = new IntPtr(0xbeef);
            IntPtr expected = pointer + 50;
            var op = new AddOperator(pointer, 50);

            IntPtr actual = op.Execute(null);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsCacheable_Get_ReturnsTrue()
        {
            var op = new AddOperator();

            Assert.IsTrue(op.IsCacheable);
        }
        
    }

    [TestClass]
    public class LeaOperatorTest
    {
        [TestMethod]
        public void DefaultConstructor_Called_InitializesLeaTypeToDword()
        {
            var op = new LeaOperator();

            Assert.AreEqual(LeaType.Dword, op.Type);
        }

        [TestMethod]
        public void LeaTypeConstructor_Called_InitializesLeaTypeToValue()
        {
            var op = new LeaOperator(IntPtr.Zero, LeaType.Pointer);

            Assert.AreEqual(LeaType.Pointer, op.Type);
        }

        [TestMethod]
        public void StringConstructor_NullString_InitializesLeaTypeToDword()
        {
            var op = new LeaOperator(() => null, () => null);

            Assert.AreEqual(LeaType.Dword, op.Type);
        }

        [TestMethod]
        public void StringConstructor_EmptyString_InitializesLeaTypeToDword()
        {
            var op = new LeaOperator(() => "", () => null);

            Assert.AreEqual(LeaType.Dword, op.Type);
        }

        [TestMethod]
        public void StringConstructor_ValidString_InitializesLeaTypeToValue()
        {
            var op = new LeaOperator(() => "Word", () => null);

            Assert.AreEqual(LeaType.Word, op.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(PatternException))]
        public void StringConstructor_InvalidString_ThrowsWhenEvaluated()
        {
            LeaType type = new LeaOperator(() => "invalid value", () => null).Type;
        }

        [TestMethod]
        public void Execute_SomePointer_Succeeds()
        {
            var memMock = new Mock<IMemory>();
            IntPtr pointer = new IntPtr(0xbeef);
            ushort expected = 0xdead;
            var op = new LeaOperator(pointer, LeaType.Word);

            memMock.Setup(x => x.Read<ushort>(pointer)).Returns(expected);

            IntPtr actual = op.Execute(memMock.Object);

            Assert.AreEqual(expected, (ushort)actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Execute_NullMemory_Throws()
        {
            IntPtr pointer = new IntPtr(0xbeef);
            var op = new LeaOperator(pointer);

            op.Execute(null);
        }

        [TestMethod]
        [ExpectedException(typeof(PatternException))]
        public void Execute_UnknownLeaType_Throws()
        {
            IntPtr pointer = new IntPtr(0xbeef);
            var op = new LeaOperator(pointer, (LeaType)15);

            op.Execute(null);
        }

        [TestMethod]
        public void IsCacheable_Get_ReturnsFalse()
        {
            var op = new LeaOperator();

            Assert.IsFalse(op.IsCacheable);
        }
    }
}
