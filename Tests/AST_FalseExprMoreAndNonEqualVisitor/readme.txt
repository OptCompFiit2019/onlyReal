FalseExprMoreAndNonEqualVisitor
Заменяет выражения логические выражения x != x, x > x, на false

Для запуска использовать следующий код:

string Text = <текст программы>;
Scanner scanner = new Scanner();
scanner.SetSource(Text, 0);
Parser parser = new Parser(scanner);
var r = parser.root;
var v = new SimpleLang.AstOptimisations.FalseExprMoreAndNonEqualVisitor();
r.Visit(v);