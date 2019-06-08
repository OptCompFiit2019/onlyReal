ILCodeGenerator
Генерация IL-кода по трёхадресному коду программы

Как запускать:
SimpleLang.Compiler.ILCodeGenerator gen = new SimpleLang.Compiler.ILCodeGenerator();
gen.Generate(treeCode.GetCode());
gen.PrintCommands();
Console.WriteLine("\nExecute:");
gen.Execute();