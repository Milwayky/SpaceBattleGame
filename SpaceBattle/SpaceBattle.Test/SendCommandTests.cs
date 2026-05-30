namespace SpaceBattle.Tests;
using Moq;
using SpaceBattle.Lib;
using Xunit;

public class SendCommandTests
{
    [Fact]
    public void TestSendCommandPassesCommandToReceiver()
    {
        // Arrange
        var commandToSendMessage = new Mock<ICommand>();
        var receiverMock = new Mock<ICommandReceiver>();
        
        var sendCommand = new SendCommand(commandToSendMessage.Object, receiverMock.Object);
        sendCommand.Execute();

        receiverMock.Verify(r => r.Receive(commandToSendMessage.Object), Times.Once());
    }


    [Fact]
    public void TestSendCommandThrowsExceptionWhenReceiverFails()
    {
        var commandToSendMessage = new Mock<ICommand>();
        var receiverMock = new Mock<ICommandReceiver>();
        
        receiverMock.Setup(r => r.Receive(It.IsAny<ICommand>())).Throws<System.Exception>();
        var sendCommand = new SendCommand(commandToSendMessage.Object, receiverMock.Object);

        Assert.ThrowsAny<System.Exception>(() => sendCommand.Execute());
    }
}

