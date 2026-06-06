using System;
using System.Collections.Generic;

namespace SpaceBattle.Lib;

public class AddToStorageCommand : ICommand
{
    private readonly string _form1;
    private readonly string _form2;
    private readonly CollisionTree _tree;
    private readonly IDictionary<(string, string), CollisionTree> _storage;

    public AddToStorageCommand(string form1, string form2, CollisionTree tree, IDictionary<(string, string), CollisionTree> storage)
    {
        _form1 = form1; 
        _form2 = form2; 
        _tree = tree; 
        _storage = storage;
    }

    public void Execute()
    {
        _storage[(_form1, _form2)] = _tree;
        _storage[(_form2, _form1)] = _tree;
    }
}

