using StrategoApp.ViewModel;

namespace StrategoTests
{
    public class SignUpViewModelTests
    {
        [Fact]
        public void TestValidateFieldsInvalidUsername()
        {
            var viewModel = new SignUpViewModel(null);
            viewModel.Username = "";
            viewModel.Email = "validemail@example.com";
            viewModel.Password = "ValidPassword123!";

            viewModel.ValidateFields();

            Assert.Equal(StrategoApp.Properties.Resources.InvalidUsername_Label, viewModel.UsernameError);
        }

        [Fact]
        public void ValidateFields_ValidData_NoErrors()
        {
            var viewModel = new SignUpViewModel(null);
            viewModel.Username = "ValidUser";
            viewModel.Email = "validemail@example.com";
            viewModel.Password = "ValidPassword123!";

            viewModel.ValidateFields();

            Assert.Equal(string.Empty, viewModel.UsernameError);
            Assert.Equal(string.Empty, viewModel.EmailError);
            Assert.Equal(string.Empty, viewModel.PasswordError);
        }
    }

}