OptMulDivOneVisitor
«амен€ет выражени€ вида 1 * ex, ex * 1, ex / 1 на ex на AST.
„тобы провести эту оптимизацию нужно построить AST, » выполнить следующий код:
var r = parser.root;    // корень AST
r.Visit(new FillParentVisitor());   // установка ссылок на родителей на AST
r.Visit(new OptMulDivOneVisitor());	// выполнение текущей оптимизации
