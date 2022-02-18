using System;
using System.Globalization;
using System.Threading;

using EasyPrototyping.Analysis;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyPrototypingTest
{
    [TestClass]
    public class DiacriticsChar_Test
    {
        [TestInitialize]
        public void TestSetUp()
        {
            CultureInfo culture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        [TestMethod]
        public void RemoveDiacritics()
        {
            string text = "Hallöchen mit Äpfeln";
            string result = text.RemoveDiacritics();
            Assert.IsTrue(result == "Hallochen mit Apfeln");

            text = "Halloechen mit Aepfeln";
            result = text.RemoveDiacritics();
            Assert.IsTrue(result == "Halloechen mit Aepfeln");
        }

        [TestMethod]
        public void DiacriticsGerConverter()
        {
            string result1 = new DiacriticsConverter("Nürnberger Straße").RemoveDiacritics();
            string result2 = new DiacriticsConverter("Nuernberger Strasse").RemoveDiacritics();

            Assert.IsTrue(result1 == result2);
        }

        [DataRow("Università", "Universita")]
        [DataRow("Perché", "Perche")]
        [DataRow("être", "etre")]
        [TestMethod]
        public void DiacriticsXXXConverter(string input, string expected)
        {
            string result = new DiacriticsConverter(input).RemoveDiacritics();

            Assert.IsTrue(result == expected);
        }

        [DataRow("Äpfel", true)]
        [DataRow("Aepfel", false)]
        [DataRow("Università", true)]
        [DataRow("Perché", true)]
        [DataRow("être", true)]
        [TestMethod]
        public void HasDiacriticsChar(string input, bool expected)
        {
            bool result = new DiacriticsConverter(input).HasDiacriticsChar();

            Assert.IsTrue(result == expected);
        }

        [TestMethod]
        public void ConvertDiacriticsWithNormal()
        {
            string text = "Hallöchen mit Äpfeln";
            string result = text.ConvertDiacriticsGER();
            Assert.IsTrue(result == "Halloechen mit Aepfeln");

            string text1 = "Käse Köln Füße Öl Übel Äü Üß ÄÖÜ Ä Ö Ü ÜBUNG und noch eine Übung";
            string result1 = text1.ConvertDiacriticsGER();
            Assert.IsTrue(result1 == "Kaese Koeln Fueße OEl Uebel Aeue Ueß AEOEUE AE OE UE UEBUNG und noch eine Uebung");
        }

        [TestMethod]
        public void ConvertDiacriticsWithToUpper()
        {
            string text = "Hallöchen mit Äpfeln";
            string result = text.ConvertDiacriticsGER(false);
            Assert.IsTrue(result == "HALLOECHEN MIT AEPFELN");

            string text1 = "Käse Köln Füße Öl Übel Äü Üß ÄÖÜ Ä Ö Ü ÜBUNG";
            string result1 = text1.ConvertDiacriticsGER();
            Assert.IsTrue(result1 == "KAESE KOELN FUESSE OEL UEBEL AEUE UESS AEOEUE AE OE UE UEBUNG");
        }
    }
}