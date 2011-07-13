using Microsoft.VisualStudio.TestTools.UnitTesting;
using BananaPattern.Algorithms;

namespace BananaTest.Tests.Patterns
{
    [TestClass]
    public class BHMTest : AlgorithmTest
    {
        protected override IPatternAlgorithm CreateAlgorithm()
        {
            return BoyerMooreHorspoolAlgorithm.Instance;
        }
    }
}
