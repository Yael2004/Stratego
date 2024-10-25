using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.ViewModel;
using System.Windows.Input;
using System;

namespace Tests
{
    [TestClass]
    public class ViewModelCommandTests
    {
        private bool _canExecuteResult;
        private bool _actionExecuted;
        private ViewModelCommand _command;

        [TestInitialize]
        public void Setup()
        {
            _canExecuteResult = true;
            _actionExecuted = false;
        }

        [TestMethod]
        public void Test_CanExecute_ReturnsTrue_WhenCanExecuteIsTrue()
        {
            _command = new ViewModelCommand(_ => { }, _ => _canExecuteResult);

            bool result = _command.CanExecute(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test_CanExecute_ReturnsFalse_WhenCanExecuteIsFalse()
        {
            _canExecuteResult = false;
            _command = new ViewModelCommand(_ => { }, _ => _canExecuteResult);

            bool result = _command.CanExecute(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Test_Execute_ExecutesAction_WhenCalled()
        {
            _command = new ViewModelCommand(_ => _actionExecuted = true);

            _command.Execute(null);

            Assert.IsTrue(_actionExecuted);
        }

        [TestMethod]
        public void Test_CanExecute_ReturnsTrue_WithoutPredicate()
        {
            _command = new ViewModelCommand(_ => { });

            bool result = _command.CanExecute(null);

            Assert.IsTrue(result);
        }
    }
}
