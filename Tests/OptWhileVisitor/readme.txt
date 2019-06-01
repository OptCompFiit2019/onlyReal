OptWhileVisitor
Заменяет циклы while(false) st; на null на AST.
Чтобы провести эту оптимизацию нужно построить AST, И выполнить следующий код:
var r = parser.root;    // корень AST
r.Visit(new FillParentVisitor());   // установка ссылок на родителей на AST
r.Visit(new OptWhileVisitor());	// выполнение текущей оптимизации
