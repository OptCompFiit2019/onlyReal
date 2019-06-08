Opt2Visitor
2 Заменяет 0 * expr и expr * 0 на 0.

тобы провести эту оптимизацию нужно построить AST и выполнить следующий код:
var r = parser.root;    // корень AST
r.Visit(new FillParentVisitor());   // установка ссылок на родителей на AST
r.Visit(new Opt2Visitor());	// выполнение данной оптимизации
