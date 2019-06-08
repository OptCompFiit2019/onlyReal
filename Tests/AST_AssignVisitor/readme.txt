AssignVisitor
«амен€ет выражени€ вида a = a на null.
„тобы провести эту оптимизацию нужно построить AST, » выполнить следующий код:
var r = parser.root;    // корень AST
r.Visit(new FillParentVisitor());   // установка ссылок на родителей на AST
r.Visit(new AssignVisitor());	// выполнение текущей оптимизации