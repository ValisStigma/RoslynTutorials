using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;

namespace ScriptEngineExample
{
    public class ScriptEngine
    {
        private static ScriptState<object> _scriptState = null;
        public static object Execute(string code)
        {
            _scriptState = _scriptState == null ? CSharpScript.RunAsync(code).Result : _scriptState.ContinueWithAsync(code).Result;
            if (!string.IsNullOrEmpty(_scriptState.ReturnValue?.ToString()))
                return _scriptState.ReturnValue;
            return null;
        }

        public static void Main(string[] args)
        {
            while (true)
            {
                var str = Console.ReadLine();
                Console.WriteLine(Execute(str));
            }

        }
    }
}
