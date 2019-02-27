#!/bin/bash
echo "Lex"
mono ./Gplex.exe /unicode SimpleLex.lex
echo "Yacc:"
mono ./Gppg.exe /no-lines /gplex SimpleYacc.y