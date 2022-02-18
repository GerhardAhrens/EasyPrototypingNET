using System.Globalization;
using System.Threading;

using EasyPrototyping.Analysis;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyPrototypingTest
{
    [TestClass]
    public class DoubleMetaphone_Test
    {
        [TestInitialize]
        public void TestSetUp()
        {
            CultureInfo culture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        [DataRow("Ahrens","Ahrens",0)]
        [DataRow("AHRENS", "Ahrens", 5)]
        [DataRow("Arens", "Ahrens", 1)]
        [DataRow("Arenz", "Ahrens", 2)]
        [DataRow("Arendz", "Ahrens", 3)]
        [DataRow("Ahrends", "Ahrens", 1)]
        [DataRow("Arends", "Ahrens", 2)]
        [DataRow("Ares", "Ahrens", 2)]
        [TestMethod]
        public void LevenshteinDistance(string input, string compareText, int expected)
        {
            int result= TextAnalysis.LevenshteinDistance(input, compareText);
            Assert.IsTrue(result == expected);
        }

        [DataRow("Ahrens", "ARNS")]
        [DataRow("AHRENS", "ARNS")]
        [DataRow("Arens", "ARNS")]
        [DataRow("Arenz", "ARNS")]
        [DataRow("Arendz", "ARNT")]
        [DataRow("Ahrends", "ARNT")]
        [DataRow("Arends", "ARNT")]
        [DataRow("Ares", "ARS")]
        [TestMethod]
        public void DoubleMetaphone(string input, string expected)
        {
            string[] result = new string[3] { "", "","" };

            using (DoubleMetaphone searchMphone = new DoubleMetaphone(input))
            {
                result[0] = searchMphone.PrimaryKey;
                result[1] = searchMphone.AlternateKey;
                result[2] = searchMphone.Length.ToString();
            }

            Assert.IsTrue(result[0] == expected);
        }
    }
}