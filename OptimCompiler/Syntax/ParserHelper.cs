using System.Collections.Generic;
using System;

namespace SimpleParser
{

    public enum type { tint, tbool, treal };

    public static class SymbolTable // Таблица символов
    {
        private static Parser parent = null;
        public static Dictionary<string, type> vars = new Dictionary<string, type>(); // таблица символов
        public static void NewVarDef(string name, type t, Parser p)
        {
            if (p != parent) {
                parent = p;
                vars = new Dictionary<string, type>();
            }
            if (vars.ContainsKey(name))
                throw new Exception("Переменная " + name + " уже определена");
            else vars.Add(name, t);
        }

        public static string TypeName(type t)
        {
            switch (t)
            {
                case type.tint: return "int";
                case type.treal: return "real";
                case type.tbool: return "bool";
                default: return null;
            }
        }
    }

    public class LexException : Exception
    {
        public LexException(string msg) : base(msg) { }
    }
    
    public class SyntaxException : Exception
    {
        public SyntaxException(string msg) : base(msg) { }
    }

    public static class ParserHelper
    {
    }
}