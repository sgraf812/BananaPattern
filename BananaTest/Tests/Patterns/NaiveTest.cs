using Microsoft.VisualStudio.TestTools.UnitTesting;
using BananaPattern.Algorithms;

namespace BananaTest.Tests.Patterns
{
    [TestClass]
    public class NaiveTest : AlgorithmTest
    {
        protected override IPatternAlgorithm CreateAlgorithm()
        {
            return NaiveAlgorithm.Instance;
        }
    }
}
