%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha   [a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
ID {Alpha}{AlphaDigit}* 

%%

{INTNUM} { 
  yylval.iVal = int.Parse(yytext); 
  return (int)Tokens.INUM; 
}

{REALNUM} { 
  yylval.dVal = double.Parse(yytext, CultureInfo.InvariantCulture);
  return (int)Tokens.RNUM;
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext, this);
  if (res == (int)Tokens.ID)
    yylval.sVal = yytext;
  return res;
}

"=" { return (int)Tokens.ASSIGN; }
";" { return (int)Tokens.SEMICOLON; }
"(" {return (int) Tokens.LPAREN; }
")" {return (int) Tokens.RPAREN; }
"," { return (int) Tokens.COLUMN; }
"+" { return (int) Tokens.ADD; }
"-" { return (int )Tokens.SUB; }
"*" {return (int) Tokens.MULT; }
"/" {return (int) Tokens.DIV; }
"{" { return (int) Tokens.BEGIN; }
"}" { return (int) Tokens.END; }
"==" { return (int) Tokens.EQUALS; }
"!=" { return (int) Tokens.NEQ; }
">" { return (int) Tokens.GTHAN; }
"<" { return (int) Tokens.LTHAN; }
">=" { return (int) Tokens.GEQ; }
"<=" { return (int) Tokens.LEQ; }
"&&" { return (int) Tokens.LOGIC_AND; }
"||" { return (int) Tokens.LOGIC_OR; }
"!" { return (int) Tokens.LOGIC_NOT; }

[^ \r\n\t] {
    LexError();
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol);
%}

%%

public override void yyerror(string format, params object[] args) // обработка синтаксических ошибок
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): Met {2}, and expected {3}", yyline, yycol, args[0], string.Join(" èëè ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
  string errorMsg = string.Format("({0},{1}): Unknow symbol {2}", yyline, yycol, yytext);
  throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;
  private static Scanner parrent = null;
  
  static void FillDict() {
    keywords = new Dictionary<string,int>();
    keywords.Add("while", (int) Tokens.WHILE);
    keywords.Add("for", (int) Tokens.FOR);
    keywords.Add("to", (int) Tokens.TO);
    keywords.Add("println", (int) Tokens.PRINTLN);
    keywords.Add("if", (int) Tokens.IF);
    keywords.Add("then", (int) Tokens.THEN);
    keywords.Add("else", (int) Tokens.ELSE);
    keywords.Add("true", (int) Tokens.TRUE);
    keywords.Add("false", (int) Tokens.FALSE);
    keywords.Add("int", (int) Tokens.TINT);
    keywords.Add("real", (int) Tokens.TREAL);
    keywords.Add("bool", (int) Tokens.TBOOL);
  }

  static ScannerHelper() 
  {
    FillDict();
  }
  public static int GetIDToken(string s, Scanner p)
  {
    if (p != parrent) {
        FillDict();
        parrent = p;
    }
    if (keywords.ContainsKey(s.ToLower()))
      return keywords[s];
    else
      return (int)Tokens.ID;
  }
  
}
