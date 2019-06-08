Opt11Visitor
11 Заменяет оператор if на его первую ветку (st1) в случае, если условие истинно.
if (true) st1; else st2; st1

Чтобы провести эту оптимизацию нужно построить AST и выполнить следующий код:
var r = parser.root;    // корень AST
r.Visit(new FillParentVisitor());   // установка ссылок на родителей на AST
r.Visit(new Opt11Visitor());	// выполнение данной оптимизации
