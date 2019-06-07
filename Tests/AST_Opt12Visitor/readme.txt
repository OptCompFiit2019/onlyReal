Opt12Visitor
Замена оператора if на его ветку else в случае, если условие ложно. 

Запуск:
var r = parser.root;    
r.Visit(new FillParentVisitor()); 
r.Visit(new Opt12Visitor());	
