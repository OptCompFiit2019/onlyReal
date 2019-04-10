# Пример работы генератора трехадресного кода

Как собирать:

	В папку с примером копируем Gplex.exe, Gppg.exe и generateParserScanner.bat.

	Вызываем generateParserScanner.bat

	Собираем проект


Как пользоваться.

Создаем генератор
	
	ThreeAddressCodeVisitor treeCode = new ThreeAddressCodeVisitor();

У синтаксического дерева вызываем функцию Visit
	
	r.Visit(treeCode);

Для получения сгенерированного трехадресного кода вызвать функцию GetCode()

	LinkedList<ThreeCode> code = treeCode.GetCode();

Генерато умеет генерировать код в виде строки. это можно сделать одним из способов

	Console.WriteLine(treeCode.ToString());

	или

	string str = ThreeAddressCodeVisitor.ToString(code);

Интерфейс строки сгенерированного кода представлен классом:

public class ThreeCode    {

        public string label;

        public ThreeOperator operation = ThreeOperator.None;

        public string result;

        public ThreeAddressValueType arg1;

        public ThreeAddressValueType arg2;

}

Где ThreeOperator это одно значение из 

enum ThreeOperator {  None, Assign, Minus, Plus, Mult, Div, Goto, IfGoto,

        Logic_or, Logic_and, Logic_less, Logic_equal, Logic_greater, Logic_geq, Logic_leq,

        Logic_not, Logic_neq };

А в качестве ThreeAddressValueType можно использовать 

    class ThreeAddressStringValue {

        public string Value { get; set; }

    }
    class ThreeAddressIntValue {

        public int Value { get; set; }

    }

    class ThreeAddressLogicValue{

        public bool Value { get; set; }

    }

    class ThreeAddressDoubleValue {

        public double Value { get; set; }

    }
