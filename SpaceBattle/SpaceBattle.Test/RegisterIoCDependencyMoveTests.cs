using Moq;
using SpaceBattle.Lib;
using Xunit;
using App;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyMoveTests
{
    [Fact]
    public void Execute_ShouldRegisterMoveDependency()
    {
        var mockMoving = new Mock<IMoving>();
        mockMoving.SetupGet(m => m.Position).Returns(new Vector(1, 2));
        mockMoving.SetupGet(m => m.Velocity).Returns(new Vector(3, 4));

        var mockStorage = new Dictionary<string, Func<object[], object>>();
        mockStorage["Adapters.IMoving"] = (args) => mockMoving.Object;

        var iocType = typeof(Ioc);
        var strategyField = iocType.GetField("strategy", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) 
                            ?? iocType.GetField("_strategy", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        if (strategyField != null)
        {
            Func<string, object[], object> testStrategy = (dependency, args) =>
            {
                if (dependency == "IoC.Register")
                {
                    var key = (string)args[0];
                    var strat = (Func<object[], object>)args[1];
                    mockStorage[key] = strat;

                    var mockCmd = new Mock<SpaceBattle.Lib.ICommand>();
                    return mockCmd.Object;
                }

                if (mockStorage.ContainsKey(dependency))
                {
                    return mockStorage[dependency](args);
                }

                throw new ArgumentException($"Dependency {dependency} is not found.");
            };

            strategyField.SetValue(null, testStrategy);
        }

        var registerCommand = new RegisterIoCDependencyMoveCommand();
        registerCommand.Execute();

        var dummyObj = new Mock<object>().Object;
        var moveCmd = Ioc.Resolve<object>("Commands.Move", dummyObj);

        if (moveCmd is SpaceBattle.Lib.ICommand spaceCmd)
        {
            spaceCmd.Execute();
        }

        Assert.NotNull(moveCmd);
        Assert.IsType<MoveCommand>(moveCmd);
    }
}


