#!/bin/bash
echo "Lex"
mono ./gplex.exe /unicode SimpleLex.lex
echo "Yacc:"
mono ./gppg.exe /no-lines /gplex SimpleYacc.y