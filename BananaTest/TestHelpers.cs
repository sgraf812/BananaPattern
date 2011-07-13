using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq.Expressions;
using System.Diagnostics;
using BananaPattern;

namespace BananaTest
{
    public static class TestHelpers
    {
        public static Mock<TMock> ReturnMockMember<T, TMock>(this Mock<T> service, Expression<Func<T, TMock>> memberAccess)
            where T : class
            where TMock : class
        {
            Mock<TMock> mock = new Mock<TMock>();
            service.Setup(memberAccess).Returns(mock.Object);

            return mock;
        }

        public static Mock<TMock> DefaultMockFor<T, TMock>(this Mock<T> service)
            where T : class
            where TMock : class
        {
            Mock<TMock> mock = new Mock<TMock>();
            service.SetReturnsDefault(mock.Object);

            return mock;
        }

        public static Mock<IBotProcessContext> TargetProcessIsCurrent(this Mock<IBotProcessContext> mock)
        {
            mock.Setup(x => x.TargetProcess).Returns(Process.GetCurrentProcess());
            return mock;
        }
    }
}
