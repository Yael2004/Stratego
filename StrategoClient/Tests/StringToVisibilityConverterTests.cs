using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.Helpers;
using System;
using System.Globalization;
using System.Windows;

namespace StrategoApp.Tests.Helpers
{
    [TestClass]
    public class StringToVisibilityConverterTests
    {
        private StringToVisibilityConverter _converter;

        [TestInitialize]
        public void TestInitialize()
        {
            _converter = new StringToVisibilityConverter();
        }

        [TestMethod]
        public void Test_Convert_StringNotEmpty_ReturnsVisible()
        {
            string input = "NotEmpty";

            var result = _converter.Convert(input, typeof(Visibility), null, CultureInfo.InvariantCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestMethod]
        public void Test_Convert_StringEmpty_ReturnsCollapsed()
        {
            string input = "";

            var result = _converter.Convert(input, typeof(Visibility), null, CultureInfo.InvariantCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBack_ShouldThrowNotImplementedException()
        {
            _converter.ConvertBack(Visibility.Visible, typeof(string), null, CultureInfo.InvariantCulture);
        }
    }
}
