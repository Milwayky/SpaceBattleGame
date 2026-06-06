using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceBattle.Lib;


public class CreateCollisionTreeCommand : ICommand
{
    private readonly IEnumerable<(int, int, int, int)> _relativeStates;
    public CollisionTree? Tree { get; private set; }

    public CreateCollisionTreeCommand(IEnumerable<(int, int, int, int)> relativeStates) => _relativeStates = relativeStates;
    public void Execute() => Tree = new CollisionTree(_relativeStates);
}

