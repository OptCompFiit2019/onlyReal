using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SimpleLang.Visitors;
using SimpleLang.ThreeOptimize;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main()
        {
            string FileName = @"../../a6.txt";
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                Parser parser = new Parser(scanner);

                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("Ошибка");
                else
                {
                    Console.WriteLine("Синтаксическое дерево построено");
                    var r = parser.root;

                    FillParentVisitor generateParrent = new FillParentVisitor();
                    r.Visit(generateParrent);


					//Console.WriteLine(r.ToString());

					/*Opt2Visitor opt2 = new Opt2Visitor();
					r.Visit(opt2);

                    PrettyPrintVisitor ppvis = new PrettyPrintVisitor();
                    r.Visit(ppvis);
                    Console.WriteLine(ppvis.Text);

                    Console.WriteLine("\nAssignCountVisitor");
                    AssignCountVisitor vis1 = new AssignCountVisitor();
                    r.Visit(vis1);
                    Console.WriteLine(vis1.Count);

                    Console.WriteLine("\nStatementCountVisitor");
                    StatementCountVisitor vis2 = new StatementCountVisitor();
                    r.Visit(vis2);
                    Console.WriteLine(vis2.Count);

                    Console.WriteLine("\nMaxCountExprOpsVisitor");
					MaxCountExprOpsVisitor vis3 = new MaxCountExprOpsVisitor();
					r.Visit(vis3);
					Console.WriteLine(vis3.Max);

					Console.WriteLine("\nNestedCyclesVisitor");
					NestedCyclesVisitor vis4 = new NestedCyclesVisitor();
					r.Visit(vis4);
					Console.WriteLine(vis4.HasNestedCycles);

					Console.WriteLine("\nCycleNestedToIfVisitor");
					CycleNestedToIfVisitor vis5 = new CycleNestedToIfVisitor();
					r.Visit(vis5);
					Console.WriteLine(vis5.HasCycleNestedToIf);

                    Console.WriteLine("\nIfNestedToCycleVisitor");
                    IfNestedToCycleVisitor vis6 = new IfNestedToCycleVisitor();
                    r.Visit(vis6);
                    Console.WriteLine(vis6.HasIfNestedToCycle);

                    Console.WriteLine("\nMaxDepthOfNestedCyclesVisitor");
                    MaxDepthOfNestedCyclesVisitor vis7 = new MaxDepthOfNestedCyclesVisitor();
                    r.Visit(vis7);
                    Console.WriteLine(vis7.Max);*/

                    Console.WriteLine("\nGenerate Three address code");
                    ThreeAddressCodeVisitor treeCode = new ThreeAddressCodeVisitor();
                    r.Visit(treeCode);
                    Console.WriteLine(treeCode.ToString());

                    Applyer app = new Applyer();
                    app.Add(new ApplyConst());
                    app.Add(new ApplyConstExpr());
                    app.Add(new ApplyAlgebraicIdentities());

                    var blocks = app.Apply(treeCode);
                    Console.WriteLine(ThreeAddressCodeVisitor.ToString(blocks));

                    var code = treeCode.GetCode();
                    app.Apply(code);



                    /*Opt11Visitor opt11vis = new Opt11Visitor();
                    ppvis.Text = "";
                    r.Visit(opt11vis);
                    r.Visit(ppvis);
                    Console.WriteLine(ppvis.Text);*/


                    /*var avis = new AssignCountVisitor();
                    parser.root.Visit(avis);
                    Console.WriteLine("Количество присваиваний = {0}", avis.Count);
                    Console.WriteLine("-------------------------------");

                    var pp = new PrettyPrintVisitor();
                    parser.root.Visit(pp);
                    Console.WriteLine(pp.Text);*/
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл {0} не найден", FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e);
            }

            Console.ReadLine();
        }

    }
}
