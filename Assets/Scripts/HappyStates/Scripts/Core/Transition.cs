using System;
using System.Linq.Expressions;

namespace HappyStates.Scripts.Core
{
    public class Transition {
        public IState To { get; }
        public Func<bool> Condition { get; }
        public string LogicString { get; }
        public string FilePath { get; }
        public int LineNumber { get; }

        public Transition(IState to, Expression<Func<bool>> expr, string path, int line) {
            To = to;
            Condition = expr.Compile();
            LogicString = expr.Body.ToString();
            FilePath = path;
            LineNumber = line;
        }
    }
}