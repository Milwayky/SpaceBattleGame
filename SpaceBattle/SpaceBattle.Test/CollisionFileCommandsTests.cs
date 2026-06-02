using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class CollisionFileCommandsTests
{
    [Fact]
    public void WriteAndRead_CollisionFile_SyncsCorrectly()
    {
        var tempFile = Path.Combine(AppContext.BaseDirectory, "temp_collision_test.txt");
        try
        {
            var initialStates = new List<(int, int, int, int)> 
            { 
                (1, 2, 3, 4), 
                (5, 6, 7, 8) 
            };

            var writeCmd = new WriteCollisionInfoFileCommand(tempFile, initialStates);
            writeCmd.Execute();

            var readCmd = new ReadCollisionInfoFileCommand(tempFile);
            readCmd.Execute();

            var fields = typeof(ReadCollisionInfoFileCommand).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            object? internalCollection = null;

            foreach (var f in fields)
            {
                var nameLower = f.Name.ToLower();
                if (nameLower.Contains("quads") || nameLower.Contains("state"))
                {
                    internalCollection = f.GetValue(readCmd);
                    break;
                }
            }

            if (internalCollection == null)
            {
                var props = typeof(ReadCollisionInfoFileCommand).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var p in props)
                {
                    var nameLower = p.Name.ToLower();
                    if (nameLower.Contains("quads") || nameLower.Contains("state"))
                    {
                        internalCollection = p.GetValue(readCmd);
                        break;
                    }
                }
            }

            Assert.NotNull(internalCollection);
            
            var collection = (IEnumerable)internalCollection;
            var resultList = new List<(int, int, int, int)>();
            foreach (var item in collection)
            {
                if (item is ValueTuple<int, int, int, int> quad)
                {
                    resultList.Add(quad);
                }
            }

            Assert.Contains((1, 2, 3, 4), resultList);
            Assert.Contains((5, 6, 7, 8), resultList);
        }
        finally
        {
            if (File.Exists(tempFile)) 
            {
                File.Delete(tempFile);
            }
        }
    }
}

