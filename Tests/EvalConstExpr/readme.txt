Данные тесты проверяет корректность работы протяжки констант.

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

	SimpleLang.ThreeCodeOptimisations.EvalConstExpr dist = new SimpleLang.ThreeCodeOptimisations.EvalConstExpr();
	dist.Apply(threeCodeVisitor.GetCode());