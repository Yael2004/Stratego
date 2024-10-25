using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.Helpers;
using System;
using System.Globalization;
using System.Windows;

namespace Tests
{
    [TestClass]
    public class InverseBooleanToVisibilityConverterTests
    {
        private InverseBooleanToVisibilityConverter _converter;

        [TestInitialize]
        public void TestInitialize()
        {
            _converter = new InverseBooleanToVisibilityConverter();
        }

        [TestMethod]
        public void Test_Convert_WhenValueIsTrue_ShouldReturnCollapsed()
        {
            bool value = true;

            var result = _converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestMethod]
        public void Test_Convert_WhenValueIsFalse_ShouldReturnVisible()
        {
            bool value = false;

            var result = _converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestMethod]
        public void Test_Convert_WhenValueIsNotBoolean_ShouldReturnVisible()
        {
            var value = "not a boolean";

            var result = _converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_ConvertBack_ShouldThrowNotImplementedException()
        {
            _converter.ConvertBack(null, typeof(bool), null, CultureInfo.InvariantCulture);
        }
    }
}
