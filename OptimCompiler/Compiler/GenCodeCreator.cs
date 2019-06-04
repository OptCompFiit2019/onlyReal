using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using SimpleParser;

namespace SimpleLang.Compiler
{
    public class GenCodeCreator
    {
        private DynamicMethod dyn;
        private ILGenerator gen;
        private bool write_commands = true;
        private static MethodInfo writeLineInt, writeLineDouble, writeLineBool;

        public List<string> commands = new List<string>();

        public GenCodeCreator()
        {
            dyn = new DynamicMethod("My", null, null, typeof(void));

            Type tcons = typeof(Console);
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
                commands.Add(op.ToString() + " var" + lb.LocalIndex);
        }

        public void Emit(OpCode op, Label l)
        {
            gen.Emit(op, l);
            if (write_commands)
                commands.Add(op.ToString() + " Label" + l.GetHashCode());
        }

        public LocalBuilder DeclareLocal(Type t)
        {
            var lb = gen.DeclareLocal(t);
            if (write_commands)
                commands.Add("DeclareLocal " + "var" + lb.LocalIndex + ": " + t);
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
            var tw = Console.Out;
            System.IO.MemoryStream stre = new System.IO.MemoryStream();
            System.IO.TextWriter wr = new System.IO.StreamWriter(stre);
            Console.SetOut(wr);
            dyn.Invoke(null, null);
            Console.SetOut(tw);
            wr.Flush();
            stre.Flush();
            return Encoding.UTF8.GetString(stre.ToArray());
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