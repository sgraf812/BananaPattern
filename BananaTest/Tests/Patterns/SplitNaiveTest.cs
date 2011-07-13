using Microsoft.VisualStudio.TestTools.UnitTesting;
using BananaPattern.Algorithms;

namespace BananaTest.Tests.Patterns
{
    [TestClass]
    public class SplitNaiveTest : AlgorithmTest
    {
        protected override IPatternAlgorithm CreateAlgorithm()
        {
            return SplitNaiveAlgorithm.Instance;
        }
    }
}
