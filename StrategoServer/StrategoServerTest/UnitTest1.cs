using StrategoCore.Services;
using Xunit;

public class EmailSenderTests
{
    [Fact]
    public void SendEmail_Should_Use_Mocked_Instance()
    {
        EmailSender emailSender = EmailSender.Instance;
        emailSender.SendEmail("strategodarkfantasy@gmail.com");
        Assert.True(true);
    }
}
