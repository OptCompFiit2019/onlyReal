Данные тесты проверяют корректность генерации трехадресного кода. В исходниках приведен код до генерации трехадресного кода. В выходных файлах приведен трехадресный код.

Запуск:
	string tt = System.IO.File.ReadAllText(FileName);
	 SimpleScanner.Scanner scan = new SimpleScanner.Scanner();
	scan.SetSource(tt, 0);
	SimpleParser.Parser pars = new SimpleParser.Parser(scan);
	var b = pars.Parse();
	var r = pars.root;
	SimpleLang.Visitors.FillParentVisitor parVisitor = new SimpleLang.Visitors.FillParentVisitor();
	r.Visit(parVisitor);
	SimpleLang.Visitors.ThreeAddressCodeVisitor threeCodeVisitor = new SimpleLang.Visitors.ThreeAddressCodeVisitor();
	r.Visit(threeCodeVisitor);