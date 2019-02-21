using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using ProgramTree;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static string ToString(ProgramTree.ExprNode node)
        {
            if (node is IdNode)
                return (node as IdNode).Name+ " ";
            if (node is IntNumNode)
                return (node as IntNumNode).Num.ToString() + " ";
            if (node is DoubleNumNode)
                return (node as DoubleNumNode).Num.ToString() + " ";
            if (node is OperationNode)
            {
                OperationNode t = node as OperationNode;
                return ToString(t.Before) + " " + t.Operation.ToString() +" " + ToString(t.After);
            }
            return "UNKNOW_EXPR ";
        }
        public static string ToString(ProgramTree.LogicExprNode node)
        {
            if (node is LogicNumNode)
                return (node as LogicNumNode).Val ? "true " : "false ";
            if (node is LogicIdNode)
                return ToString((node as LogicIdNode).Val) + " ";
            if (node is LogicOperationNode)
            {
                LogicOperationNode t = node as LogicOperationNode;
                return ToString(t.Before) + " " + t.Operation.ToString() + " " + ToString(t.After);
            }
            return "UNKNOW_LOGIC ";

        }
        public static void FormatNode(ProgramTree.Node node, string delimer = "")
        {
            if (node == null)
                return;
            if (node is ProgramTree.BlockNode)
            {
                Console.WriteLine(delimer + "{ ");
                FormatNodes((node as ProgramTree.BlockNode).StList, delimer + "\t");
                Console.WriteLine(delimer + "} ");
                return;
            }

            if (node is ProgramTree.IfNode)
            {
                ProgramTree.IfNode n = node as ProgramTree.IfNode;
                Console.WriteLine(delimer + "if ( {0} )", ToString(n.Expr));

                FormatNode(n._IF, delimer + "\t");
                if (n._ELSE != null)
                {
                    Console.WriteLine(delimer + "else ");
                    FormatNode(n._ELSE, delimer + "\t");
                }
            }
            if (node is ProgramTree.WhileNode)
            {
                WhileNode w = node as WhileNode;
                Console.WriteLine(delimer + "while ( {0} )", ToString(w.Expr));
                FormatNode(w.Stat, delimer + "\t");
            }
            if (node is ProgramTree.ForNode)
            {
                ForNode f = node as ForNode;
                Console.WriteLine(delimer + "for ( {0}={1} to {2} )", ToString(f.ID), ToString(f.StartValue), ToString(f.End));
                FormatNode(f.Stat, delimer + "\t");
            }
            if (node is AssignNode)
            {
                AssignNode n = node as AssignNode;
                Console.WriteLine(delimer + "{0} = {1}", ToString(n.Id), ToString(n.Expr));
            }
        }
        public static void FormatNodes(List<ProgramTree.StatementNode> list, string delimer = "")
        {
            foreach (var st in list)
            {
                FormatNode(st, delimer);
            }
        }

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
                    Console.WriteLine("\n\nFormated sources:");
                    FormatNodes(parser.root.StList);
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
