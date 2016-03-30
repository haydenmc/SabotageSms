using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SabotageSms.GameControl;
using Microsoft.Data.Entity;
using SabotageSms.Models.DbModels;
using SabotageSms.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Entity.Infrastructure;
using Xunit.Abstractions;
using SabotageSms.Models;

namespace SabotageSms.Tests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dnx.html
    public class ParserTest
    {
        private ITestOutputHelper _output;
        private ParsingProvider _parser { get; set; }
        
        public ParserTest(ITestOutputHelper output)
        {
            _output = output;
            _parser = new ParsingProvider();
        }
        
        [Fact]
        public void TestName()
        {
            // Name command
            var nameCommand = _parser.ParseCommand(null, "name Hayden");
            Assert.Equal(nameCommand.Command, Command.Name);
            Assert.Equal((string) nameCommand.Parameters, "Hayden");
        }
        
        [Fact]
        public void TestNew()
        {
            // New command
            var newCommand = _parser.ParseCommand(null, "New");
            Assert.Equal(newCommand.Command, Command.New);
        }
        
        [Fact]
        public void TestJoin()
        {
            // Join command
            var joinCommand = _parser.ParseCommand(null, "Join ED8FH");
            Assert.Equal(joinCommand.Command, Command.Join);
            Assert.Equal((string) joinCommand.Parameters, "ED8FH");
        }
        
        [Fact]
        public void TestStartGame()
        {
            // Start game command
            var startCommand = _parser.ParseCommand(null, "Start");
            Assert.Equal(startCommand.Command, Command.StartGame);
        }
        
        [Fact]
        public void TestSelectRoster()
        {
            // Select roster command
            var selectCommand = _parser.ParseCommand(null, "select Hayden, KK, Vlzaha1");
            Assert.Equal(selectCommand.Command, Command.SelectRoster);
            var playerNames = selectCommand.Parameters as string[];
            Assert.True(playerNames.Contains("Hayden"));
            Assert.True(playerNames.Contains("KK"));
            Assert.True(playerNames.Contains("Vlzaha1"));
        }
        
        [Fact]
        public void TestConfirmRoster()
        {
            // Confirm roster command
            var confirmCommand = _parser.ParseCommand(null, "confirm");
            Assert.Equal(confirmCommand.Command, Command.ConfirmRoster);
        }
        
        [Fact]
        public void TestRejectRoster()
        {
            // Reject roster command
            var rejectCommand = _parser.ParseCommand(null, "reject");
            Assert.Equal(rejectCommand.Command, Command.RejectRoster);
        }
        
        [Fact]
        public void TestApproveRoster()
        {
            // Approve roster command
            var approveCommand = _parser.ParseCommand(null, "approve");
            Assert.Equal(approveCommand.Command, Command.ApproveRoster);
        }
        
        [Fact]
        public void TestPassMission()
        {
            // Pass mission command
            var passCommand = _parser.ParseCommand(null, "pass");
            Assert.Equal(passCommand.Command, Command.PassMission);
        }
        
        [Fact]
        public void TestFailMission()
        {
            // Fail mission command
            var failCommand = _parser.ParseCommand(null, "fail");
            Assert.Equal(failCommand.Command, Command.FailMission);
        }
    }
}