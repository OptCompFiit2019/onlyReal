ControlFlowGraph
Строит по базовым блокам граф потока управления.

Запуск:
	string Text = File.ReadAllText(FileName);
	Scanner scanner = new Scanner();
	scanner.SetSource(Text, 0);
	Parser parser = new Parser(scanner);
	var r = parser.root;

	Console.WriteLine("\nGenerate Three address code");
	ThreeAddressCodeVisitor treeCode = new ThreeAddressCodeVisitor();
	r.Visit(treeCode);
	Console.WriteLine(treeCode.ToString());

	Console.WriteLine("\nGenerate Base Blocks");
	var block = new Block(treeCode);
	var blocks = block.GenerateBlocks();
	Console.WriteLine(block.ToString());
	
	Console.WriteLine("\nGenerate Control Flow Graph\n");
	CFG controlFlowGraph = new CFG(blocks);
	Console.WriteLine(controlFlowGraph.ToString());
