Block
Строит по трехадресному коду список базовых блоков.

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
	var blocks = new Block(treeCode).GenerateBlocks();
	Console.WriteLine(blocks.ToString());

