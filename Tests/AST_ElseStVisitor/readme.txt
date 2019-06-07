ElseStVisitor
12 «амен€ет оператор if на его ветку else в случае, если условие ложно. if (false) st1; else st2; st2
„тобы провести эту оптимизацию нужно построить AST, » выполнить следующий код:
var r = parser.root;    // корень AST
r.Visit(new FillParentVisitor());   // установка ссылок на родителей на AST
r.Visit(new ElseStVisitor());	// выполнение текущей оптимизации
