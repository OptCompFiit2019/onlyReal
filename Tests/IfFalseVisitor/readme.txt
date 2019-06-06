IfFalseVisitor
Удаляет недостижимый блок команд в операторе if если в условии стоит коснтанта false

Для запуска использовать следующий код:

string Text = <текст программы>;
Scanner scanner = new Scanner();
scanner.SetSource(Text, 0);
Parser parser = new Parser(scanner);
var r = parser.root;
var iffalse = new IfFalseVisitor();
r.Visit(iffalse);