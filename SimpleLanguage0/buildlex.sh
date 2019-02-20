#!/bin/bash
mono ./Gplex.exe /unicode SimpleLex.lex
mono ./Gppg.exe /no-lines /gplex SimpleYacc.y