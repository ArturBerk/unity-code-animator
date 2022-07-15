using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CodeAnimator
{
    public class CodeAnimationProgramParser
    {
        private string workingPath;
        
        private static readonly Regex name = new Regex(@"^\s*name\s+([^\s]+)");
        private static readonly Regex changeWorkingPath = new Regex(@"^\s*path\s+([^\s]+)");
        private static readonly Regex loadFromFileRegex = new Regex(@"^\s*load\s+([^\s]+)");
        private static readonly Regex loadFromFileSetRegex = new Regex(@"^\s*loadset\s+([^\s]+)");
        private static readonly Regex scrollToLineRegex = new Regex(@"^\s*scroll\s+([\d]+)");
        private static readonly Regex delayRegex = new Regex(@"^\s*delay\s+(\d+(\.\d+){0,1})");

        public CodeAnimationProgramParser()
        {
            workingPath = Application.dataPath;
        }
    
        public string WorkingPath
        {
            get => workingPath;
            set => workingPath = value;
        }

        public IList<IAnimatorCommand> Parse(string program)
        {
            var commands = new List<IAnimatorCommand>();
            foreach (var line in program.Split(new [] {'\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                var match = changeWorkingPath.Match(line);
                if (match.Success)
                {
                    workingPath = match.Groups[1].Value;
                    continue;
                }
                match = loadFromFileRegex.Match(line);
                if (match.Success)
                {
                    commands.Add(new LoadFromFileCommand(Path.Combine(workingPath, match.Groups[1].Value), true));
                    continue;
                }
                match = name.Match(line);
                if (match.Success)
                {
                    commands.Add(new SetNameCommand(match.Groups[1].Value));
                    continue;
                }
                
                match = loadFromFileSetRegex.Match(line);
                if (match.Success)
                {
                    commands.Add(new LoadFromFileCommand(Path.Combine(workingPath, match.Groups[1].Value), false));
                    continue;
                }
                match = scrollToLineRegex.Match(line);
                if (match.Success)
                {
                    commands.Add(new ScrollToLineCommand(int.Parse(match.Groups[1].Value)));
                    continue;
                }
                match = delayRegex.Match(line);
                if (match.Success)
                {
                    commands.Add(new DelayCommand(float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture)));
                    continue;
                }
                Debug.LogError("Unsupported command: " + line);
            }

            return commands;
        }
    }
}