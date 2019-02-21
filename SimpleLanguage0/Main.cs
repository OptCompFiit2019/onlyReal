using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void PrintNode(ProgramTree.Node node, string delimer = "")
        {
            if (node == null)
                return;
            if (node is ProgramTree.BlockNode)
            {
                PrintNodes((node as ProgramTree.BlockNode).StList, delimer + "\t");
                return;
            }
            Console.Write(delimer);
            Console.WriteLine(node);
            if (node is ProgramTree.IfNode)
            {
                ProgramTree.IfNode n = node as ProgramTree.IfNode;

                PrintNode(n._IF, delimer + "\t");
                if (n._ELSE != null)
                {
                    Console.WriteLine(delimer + "ELSE:");
                    PrintNode(n._ELSE, delimer + "\t");
                }
            }
            if (node is ProgramTree.WhileNode)
            {
                ProgramTree.WhileNode n = node as ProgramTree.WhileNode;
                PrintNode(n.Stat, delimer + "\t");
            }
            if (node is ProgramTree.ForNode)
            {
                ProgramTree.ForNode n = node as ProgramTree.ForNode;
                PrintNode(n.Stat, delimer + "\t");
            }
        }
        public static void PrintNodes(List<ProgramTree.StatementNode> list, string delimer = "")
        {
            foreach(var st in list)
            {
                PrintNode(st, delimer);
                if (st is ProgramTree.BlockNode)
                {
                    PrintNodes((st as ProgramTree.BlockNode).StList, delimer + "\t");
                }
            }
        }
        public static void Main()
        {
            string FileName = @"../../a.txt";
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);
            
                Parser parser = new Parser(scanner);
                      
                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("Error");
                else
                {
                    Console.WriteLine("Sintaxis tree is success");
                    PrintNodes(parser.root.StList);
                }
                //if (!b)
		          //  Console.WriteLine("Ошибка");
                //else Console.WriteLine("Программа распознана");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл {0} не найден", FileName);
            }
            catch (LexException e)
            {
                Console.WriteLine("Лексическая ошибка. " + e.Message);
            }
            catch (SyntaxException e)
            {
                Console.WriteLine("Синтаксическая ошибка. " + e.Message);
            }

            Console.ReadLine();
        }

    }
}
