%{
// Эти объявления добавляются в класс GPPGParser, представляющий собой парсер, генерируемый системой gppg
    public BlockNode root; // Корневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
	private bool InDefSect = false;
%}

%output = SimpleYacc.cs

%union { 
            public double dVal; 
            public int iVal; 
            public string sVal; 
            public Node nVal;
            public ExprNode eVal;
            public StatementNode stVal;
            public LogicExprNode logVal;
            public BlockNode blVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END ASSIGN SEMICOLON WHILE FOR TO PRINTLN PRINTLN LPAREN RPAREN IF THEN ELSE VAR COLUMN ADD SUB MULT DIV LOGIC_AND LOGIC_OR LOGIC_NOT TRUE FALSE EQUALS GTHAN LTHAN GEQ LEQ NEQ
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident T F
%type <logVal> logic_expr logic_T  logic_F logic_E logic_D
%type <stVal> assign statement while for println if var varlist
%type <blVal> stlist block

%%

progr   : block { root = $1; }
        ;

stlist  :
            { 
                $$ = new BlockNode(); 
            }
        | stlist statement 
            { 
                $1.Add($2); 
                $$ = $1; 
            }
        ;

statement: assign SEMICOLON { $$ = $1; }
        | while   { $$ = $1; }
        | for     { $$ = $1; }
        | println SEMICOLON   { $$ = $1; }
        | if     { $$ = $1; }
        | var SEMICOLON     { $$ = $1; }
        | block   { $$ = $1; }
    ;

ident   : ID { $$ = new IdNode($1); }   
        ;

println   : PRINTLN LPAREN expr RPAREN { $$ = new PrintlnNode($3); }
        ;
    
assign  : ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
        ;
        
        
var		: VAR { InDefSect = true; } varlist 
		{ 
			foreach (var v in ($3 as VarDefNode).vars)
				SymbolTable.NewVarDef(v.Name, type.tint);
			InDefSect = false;	
		}
		;

varlist	: ident 
		{ 
			$$ = new VarDefNode($1 as IdNode); 
		}
		| varlist COLUMN ident 
		{ 
			($1 as VarDefNode).Add($3 as IdNode);
			$$ = $1;
		}
		;


//expr  : ident  { $$ = $1 as IdNode; }
//      | INUM { $$ = new IntNumNode($1); }
//      ;

logic_expr  :   logic_T { $$ = $1; }
            |   logic_expr LOGIC_OR logic_T { $$ = new LogicOpNode($1, $3, "||"); }
            ;
            
            //logic_equals  : expr EQUALS expr { $$ = new EqualsNode($1, $3); }
//        ;
logic_T     :   logic_F { $$ = $1 as LogicExprNode; }
            |   logic_T LOGIC_AND logic_F { $$ = new LogicOpNode($1, $3, "&&"); }
            ;
            
logic_F     :   logic_E { $$ = $1 as LogicExprNode; }
            |   logic_E EQUALS logic_E  { $$ = new LogicOpNode($1, $3, "=="); }
            |   logic_E GTHAN logic_E  { $$ = new LogicOpNode($1, $3, ">"); }
            |   logic_E LTHAN logic_E  { $$ = new LogicOpNode($1, $3, "<"); }
            |   logic_E GEQ logic_E  { $$ = new LogicOpNode($1, $3, ">="); }
            |   logic_E LEQ logic_E  { $$ = new LogicOpNode($1, $3, "<="); }
            |   logic_E NEQ logic_E  { $$ = new LogicOpNode($1, $3, "!="); }
            ;

logic_E     :   logic_D { $$ = $1 as LogicExprNode; }
            |   LOGIC_NOT logic_D { $$ = new LogicNotNode($2); }
            ;

logic_D		:   ident {$$ = new LogicIdNode($1 as IdNode); }
            |   TRUE { $$ = new BooleanNode(true); }
            |   FALSE { $$ = new BooleanNode(false); }
            |   LPAREN logic_expr RPAREN { $$ = $2 as LogicExprNode; }
            ;

expr    : T { $$ = $1; }
        | expr ADD T { $$ = new BinOpNode($1, $3, '+'/*SimpleParser.Tokens.ADD*/); }
        | expr SUB T { $$ = new BinOpNode($1, $3, '-'/*SimpleParser.Tokens.SUB*/); }
        ;
        
T       : F { $$ = $1 as ExprNode; }
        | T MULT F { $$ = new BinOpNode ( $1, $3, '*'/*SimpleParser.Tokens.MULT*/); }
        | T DIV F { $$ = new BinOpNode ($1, $3, '/'/*SimpleParser.Tokens.DIV*/); }
        ;
        
F       : ident { $$ = $1 as IdNode; }
        | INUM { $$ = new IntNumNode($1); }
        | RNUM { $$ = new DoubleNumNode($1); }
        | LPAREN expr RPAREN { $$ = $2 as ExprNode; }
        ;
        
//params  : expr { $$ =  new ParamsNode($1); }
  //      | params ZP expr {$$ = new ParamsNode($3, $1 as ParamsNode); }
    //    ;
        
if      : IF LPAREN logic_expr RPAREN statement { $$ = new IfNode($3, $5); }
        | IF LPAREN logic_expr RPAREN statement ELSE statement { $$ = new IfNode($3, $5, $7); }
        ;

block   : BEGIN stlist END { $$ = $2; }
        ;
        
        
while   : WHILE LPAREN logic_expr RPAREN statement { $$ = new WhileNode($3, $5); }
        ;
        
        
for     : FOR LPAREN assign TO expr RPAREN statement { $$ = new ForNode($3 as AssignNode, $5, $7); }
        ;
    
%%

