MultiplicationComputeVisitor
Заменяет умножение констант на их произведение

Для запуска использовать следующий код:

string Text = <текст программы>;
Scanner scanner = new Scanner();
scanner.SetSource(Text, 0);
Parser parser = new Parser(scanner);
var r = parser.root;
var mult = new MultiplicationComputeVisitor();
r.Visit(mult);