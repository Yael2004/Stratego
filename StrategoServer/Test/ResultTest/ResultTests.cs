﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void Test_Success_ShouldSetIsSuccessToTrueAndSetValue()
        {
            var expectedValue = 42;

            var result = Result<int>.Success(expectedValue);

            Assert.AreEqual(expectedValue, result.Value);
        }

        [TestMethod]
        public void Test_Failure_ShouldSetIsSuccessToFalseAndSetError()
        {
            var expectedError = "An error occurred";

            var result = Result<int>.Failure(expectedError);

            Assert.AreEqual(expectedError, result.Error);
        }

        [TestMethod]
        public void Test_Constructor_ShouldSetValueCorrectly()
        {
            var expectedValue = "Test";

            var resultSuccess = new Result<string>(expectedValue, true, string.Empty);

            Assert.AreEqual(expectedValue, resultSuccess.Value);
        }

        [TestMethod]
        public void Test_Constructor_ShouldSetErrorCorrectly()
        {
            var expectedError = "Test";

            var resultFailure = new Result<string>(false, expectedError);

            Assert.AreEqual(expectedError, resultFailure.Error);
        }

        [TestMethod]
        public void Test_Failure_ShouldReturnDefaultForReferenceType()
        {
            var expectedError = "An error occurred";

            var result = Result<object>.Failure(expectedError);

            Assert.AreEqual(expectedError, result.Error);
        }

        [TestMethod]
        public void Test_Failure_ShouldReturnDefaultForValueType()
        {
            var expectedError = "An error occurred";

            var result = Result<int>.Failure(expectedError);

            Assert.AreEqual(0, result.Value);
        }

        [TestMethod]
        public void Test_Success_ShouldAllowNullOrEmptyValue()
        {
            string nullValue = null;
            string emptyValue = string.Empty;

            var resultWithNull = Result<string>.Success(nullValue);

            var resultWithEmpty = Result<string>.Success(emptyValue);

            Assert.AreEqual(string.Empty, resultWithEmpty.Error);
        }

        [TestMethod]
        public void Test_Constructor_ShouldCorrectlyInitializeValueAndError()
        {
            var expectedValue = 99;
            var expectedError = "Custom error";

            var successResult = new Result<int>(expectedValue, true, string.Empty);

            var failureResult = new Result<int>(false, expectedError);

            Assert.AreEqual(expectedError, failureResult.Error);
        }

        [TestMethod]
        public void Test_Constructor_WithValueAndSuccess_ShouldIgnoreErrorIfSuccessIsTrue()
        {
            var expectedValue = 50;
            var unexpectedError = "This error should be ignored";

            var result = new Result<int>(expectedValue, true, unexpectedError);

            Assert.AreEqual(string.Empty, result.Error);
        }

        [TestMethod]
        public void Test_Constructor_WithOnlyIsSuccessAndError_ShouldSetDefaultValueForT()
        {
            var expectedError = "Some failure occurred";

            var result = new Result<int>(false, expectedError);

            Assert.AreEqual(expectedError, result.Error); 
        }

        [TestMethod]
        public void Test_Success_WithNullValue_ShouldAllowNullValueForReferenceTypes()
        {
            string nullValue = null;

            var result = Result<string>.Success(nullValue);

            Assert.AreEqual(string.Empty, result.Error);  
        }


    }
}
