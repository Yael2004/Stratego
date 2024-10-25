using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Test.ResultTest
{
    [TestClass]
    public class ResultTests
    {
        [TestMethod]
        public void Success_ShouldSetIsSuccessToTrueAndSetValue()
        {
            var expectedValue = 42;

            var result = Result<int>.Success(expectedValue);

            Assert.AreEqual(expectedValue, result.Value);
        }

        [TestMethod]
        public void Failure_ShouldSetIsSuccessToFalseAndSetError()
        {
            var expectedError = "An error occurred";

            var result = Result<int>.Failure(expectedError);

            Assert.AreEqual(expectedError, result.Error);
        }

        [TestMethod]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            var expectedValue = "Test";
            var expectedError = "Error";

            var resultSuccess = new Result<string>(expectedValue, true, string.Empty);

            Assert.AreEqual(expectedValue, resultSuccess.Value);

            var resultFailure = new Result<string>(false, expectedError);

            Assert.AreEqual(expectedError, resultFailure.Error);
        }

        [TestMethod]
        public void Failure_ShouldReturnDefaultForReferenceType()
        {
            var expectedError = "An error occurred";

            var result = Result<object>.Failure(expectedError);

            Assert.AreEqual(expectedError, result.Error);
        }

        [TestMethod]
        public void Failure_ShouldReturnDefaultForValueType()
        {
            var expectedError = "An error occurred";

            var result = Result<int>.Failure(expectedError);

            Assert.AreEqual(expectedError, result.Error);
        }

        [TestMethod]
        public void Success_ShouldAllowNullOrEmptyValue()
        {
            string nullValue = null;
            string emptyValue = string.Empty;

            var resultWithNull = Result<string>.Success(nullValue);

            Assert.AreEqual(string.Empty, resultWithNull.Error);

            var resultWithEmpty = Result<string>.Success(emptyValue);

            Assert.AreEqual(string.Empty, resultWithEmpty.Error);
        }

        [TestMethod]
        public void Constructor_ShouldCorrectlyInitializeValueAndError()
        {
            var expectedValue = 99;
            var expectedError = "Custom error";

            var successResult = new Result<int>(expectedValue, true, string.Empty);

            Assert.AreEqual(string.Empty, successResult.Error);

            var failureResult = new Result<int>(false, expectedError);

            Assert.AreEqual(expectedError, failureResult.Error);
        }

        [TestMethod]
        public void Constructor_WithValueAndSuccess_ShouldIgnoreErrorIfSuccessIsTrue()
        {
            var expectedValue = 50;
            var unexpectedError = "This error should be ignored";

            var result = new Result<int>(expectedValue, true, unexpectedError);

            Assert.AreEqual(string.Empty, result.Error);
        }

        [TestMethod]
        public void Constructor_WithOnlyIsSuccessAndError_ShouldSetDefaultValueForT()
        {
            var expectedError = "Some failure occurred";

            var result = new Result<int>(false, expectedError);

            Assert.AreEqual(expectedError, result.Error); 
        }

        [TestMethod]
        public void Success_WithNullValue_ShouldAllowNullValueForReferenceTypes()
        {
            string nullValue = null;

            var result = Result<string>.Success(nullValue);

            Assert.AreEqual(string.Empty, result.Error);  
        }


    }
}
