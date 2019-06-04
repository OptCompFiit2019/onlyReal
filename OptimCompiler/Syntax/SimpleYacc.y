%{
// Ёти объ€влени€ добавл€ютс€ в класс GPPGParser, представл€ющий собой парсер, генерируемый системой gppg
    public BlockNode root; //  орневой узел синтаксического дерева 
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
            public BlockNode blVal;
			public type tVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END ASSIGN SEMICOLON WHILE FOR TO PRINTLN PRINTLN LPAREN RPAREN IF THEN ELSE COLUMN ADD SUB MULT DIV LOGIC_AND LOGIC_OR LOGIC_NOT TRUE FALSE EQUALS GTHAN LTHAN GEQ LEQ NEQ TINT TREAL TBOOL
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident LT LF E T F
%type <stVal> assign statement while for println if var varlist
%type <blVal> stlist block
%type <tVal> type_

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

ident   : ID
            {
				$$ = new IdNode($1);
                if (!InDefSect)
				{
                    if (!SymbolTable.vars.ContainsKey($1))
                        throw new Exception("("+@1.StartLine+","+@1.StartColumn+"): ѕеременна€ "+$1+" не описана");
					$$.Type = SymbolTable.vars[$1];
                }
            }   
        ;

println   : PRINTLN LPAREN expr RPAREN { $$ = new PrintlnNode($3); }
        ;
    
assign  : ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3, @3); }
        ;
        
type_   : TINT { $$ = type.tint; }
        | TREAL { $$ = type.treal; }
		| TBOOL { $$ = type.tbool; }
		;
        
var		: type_ { InDefSect = true; } varlist
		{ 
			foreach (var v in ($3 as VarDefNode).vars)
			{
				SymbolTable.NewVarDef(v.Name, $1, this);
				v.Type = $1;
			}
			$$ = $3;
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

expr    : LT { $$ = $1; }
        | expr EQUALS LT { $$ = new LogicOpNode($1, $3, "==", @2); }
        | expr GTHAN LT { $$ = new LogicOpNode($1, $3, ">", @2); }
        | expr LTHAN LT { $$ = new LogicOpNode($1, $3, "<", @2); }
        | expr GEQ LT { $$ = new LogicOpNode($1, $3, ">=", @2); }
        | expr LEQ LT { $$ = new LogicOpNode($1, $3, "<=", @2); }
        | expr NEQ LT { $$ = new LogicOpNode($1, $3, "!=", @2); }
        ;

LT      : LF { $$ = $1; }
        | LT LOGIC_OR LF { $$ = new BinOpNode($1, $3, "||", @2); }
        ;

LF      : E { $$ = $1; }
        | LF LOGIC_AND E { $$ = new BinOpNode($1, $3, "&&", @2); }
        ;

E       : T { $$ = $1; }
        | E ADD T { $$ = new BinOpNode($1, $3, "+"/*SimpleParser.Tokens.ADD*/, @2); }
        | E SUB T { $$ = new BinOpNode($1, $3, "-"/*SimpleParser.Tokens.SUB*/, @2); }
        ;
        
T       : F { $$ = $1; }
        | T MULT F { $$ = new BinOpNode ( $1, $3, "*"/*SimpleParser.Tokens.MULT*/, @2); }
        | T DIV F { $$ = new BinOpNode ($1, $3, "/"/*SimpleParser.Tokens.DIV*/, @2); }
		| LOGIC_NOT F { $$ = new LogicNotNode($2, @1); }
        ;

F       : ident { $$ = $1 as IdNode; }
        | INUM { $$ = new IntNumNode($1); }
        | RNUM { $$ = new DoubleNumNode($1); }
		| TRUE { $$ = new BooleanNode(true); }
        | FALSE { $$ = new BooleanNode(false); }
        | LPAREN expr RPAREN { $$ = $2 as ExprNode; }
        ;
        
//params  : expr { $$ =  new ParamsNode($1); }
  //      | params ZP expr {$$ = new ParamsNode($3, $1 as ParamsNode); }
    //    ;
        
if      : IF LPAREN expr RPAREN statement { $$ = new IfNode($3, $5); }
        | IF LPAREN expr RPAREN statement ELSE statement { $$ = new IfNode($3, $5, $7); }
        ;

block   : BEGIN stlist END { $$ = $2; }
        ;
        
        
while   : WHILE LPAREN expr RPAREN statement { $$ = new WhileNode($3, $5); }
        ;
        
for     : FOR LPAREN assign TO expr RPAREN statement { $$ = new ForNode($3 as AssignNode, $5, $7); }
        ;
    
%%

