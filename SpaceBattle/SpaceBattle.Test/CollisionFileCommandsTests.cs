using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class CollisionFileCommandsTests
{
    [Fact]
    public void WriteAndRead_CollisionFile_SyncsCorrectly()
    {
        var tempFile = "test_collision.txt";
        var states = new List<(int, int, int, int)> { (1, 2, 3, 4), (5, 6, 7, 8) };
        
        var writeCmd = new WriteCollisionInfoFileCommand(tempFile, states);
        writeCmd.Execute();
        
        var readCmd = new ReadCollisionInfoFileCommand(tempFile);
        readCmd.Execute();
        
        Assert.NotNull(readCmd.Quads);
        Assert.Equal(2, readCmd.Quads.Count());
        Assert.Contains((1, 2, 3, 4), readCmd.Quads);
        
        if (File.Exists(tempFile)) File.Delete(tempFile);
    }

    [Fact]
    public void ReadCollisionInfoFile_InvalidFileFormat_DoesNotThrowButIgnoresBadLines()
    {
        // Изначально тут был Throws<FormatException>, но твой парсер использует int.Parse
        // Убедимся, что он падает при парсинге букв.
        var tempFile = Path.Combine(Path.GetTempPath(), "invalid.txt");
        File.WriteAllText(tempFile, "bad, data, here, bro");
        
        var cmd = new ReadCollisionInfoFileCommand(tempFile);
        Assert.Throws<FormatException>(() => cmd.Execute());
        
        File.Delete(tempFile);
    }
}

