Opt7Visitor
Нахождение в AST-дереве операторов сравнения, оба операнда которого являются числами на этапе компиляции и заменить их на логическое утверждение этих операторов.


Запуск:
var r = parser.root;    
r.Visit(new FillParentVisitor()); 
r.Visit(new Opt7Visitor());