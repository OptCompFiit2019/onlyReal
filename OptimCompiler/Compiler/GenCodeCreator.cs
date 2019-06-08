using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using SimpleParser;
using System.Security;

[assembly: SecurityTransparent]
namespace SimpleLang.Compiler
{
    public static class GenCodeConsole
    {
        public static StringBuilder stringBuilder = new StringBuilder();

        public static void WriteLine(int value)
        {
            InternalWriteLine(value);
        }

        public static void WriteLine(double value)
        {
            InternalWriteLine(value);
        }

        public static void WriteLine(bool value)
        {
            InternalWriteLine(value);
        }

        private static void InternalWriteLine(object value)
        {
            stringBuilder.AppendLine(value.ToString());
            Console.WriteLine(value);
        }
    }

    public class GenCodeCreator
    {
        private Dictionary<LocalBuilder, string> variables = new Dictionary<LocalBuilder, string>(); 
        private DynamicMethod dyn;
        private ILGenerator gen;
        private bool write_commands = true;
        private static MethodInfo writeLineInt, writeLineDouble, writeLineBool;

        public List<string> commands = new List<string>();

        public GenCodeCreator()
        {
            dyn = new DynamicMethod("My", null, null, typeof(void));

            Type tcons = typeof(GenCodeConsole);
            string wl = "WriteLine";

            writeLineInt = tcons.GetMethod(wl, new Type[] { typeof(int) });
            writeLineDouble = tcons.GetMethod(wl, new Type[] { typeof(double) });
            writeLineBool = tcons.GetMethod(wl, new Type[] { typeof(bool) });

        gen = dyn.GetILGenerator();
        }

        private static MethodInfo WritelnMethod(type t)
        {
            switch (t)
            {
                case type.tbool: return writeLineBool;
                case type.tint: return writeLineInt;
                default: return writeLineDouble;
            }
        }

        public void Emit(OpCode op)
        {
            gen.Emit(op);
            if (write_commands)
                commands.Add(op.ToString());
        }

        public void Emit(OpCode op, int num)
        {
            gen.Emit(op, num);
            if (write_commands)
                commands.Add(op.ToString() + " " + num);
        }

        public void Emit(OpCode op, double num)
        {
            gen.Emit(op, num);
            if (write_commands)
                commands.Add(op.ToString() + " " + num);
        }

        public void Emit(OpCode op, LocalBuilder lb)
        {
            gen.Emit(op, lb);
            if (write_commands)
                commands.Add(op.ToString() + " " +
                    (variables.ContainsKey(lb) ? variables[lb] : "var" + lb.LocalIndex));
        }

        public void Emit(OpCode op, Label l)
        {
            gen.Emit(op, l);
            if (write_commands)
                commands.Add(op.ToString() + " Label" + l.GetHashCode());
        }

        public LocalBuilder DeclareLocal(Type t, string name = null)
        {
            var lb = gen.DeclareLocal(t);
            variables.Add(lb, name);
            if (write_commands)
                commands.Add("DeclareLocal " + (name ?? "var" + lb.LocalIndex) + ": " + t);
            return lb;
        }

        public Label DefineLabel()
        {
            var l = gen.DefineLabel();
            if (write_commands)
                commands.Add("DefineLabel" + " Label" + l.GetHashCode());

            return l;
        }

        public void MarkLabel(Label l)
        {
            gen.MarkLabel(l);
            if (write_commands)
                commands.Add("MarkLabel" + " Label" + l.GetHashCode());
        }

        public void EmitWriteLine(type t = type.tint)
        {
            gen.Emit(OpCodes.Call, WritelnMethod(t));
            if (write_commands)
                commands.Add("WriteLine");
        }

        public void EndProgram()
        {
            gen.Emit(OpCodes.Ret);
        }

        public string RunProgram()
        {
            GenCodeConsole.stringBuilder.Clear();
            dyn.Invoke(null, null);
            return GenCodeConsole.stringBuilder.ToString();
        }

        public void WriteCommandsOn()
        {
            write_commands = true;
        }

        public void WriteCommandsOff()
        {
            write_commands = false;
        }
    }
}