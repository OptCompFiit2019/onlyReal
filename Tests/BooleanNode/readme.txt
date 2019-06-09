BooleanNode

5 Логические тождества < > and or true false

Не является оптимизацией, расширение грамматики языка:

ThreeAddressCodeVisitor treeCod2e = new ThreeAddressCodeVisitor();
r.Visit(treeCod2e);
AutoThreeCodeOptimiser ap2p = new AutoThreeCodeOptimiser();
ap2p.Add(new SimpleLang.ThreeCodeOptimisations.DistributionOfConstants());
ap2p.Add(new SimpleLang.ThreeCodeOptimisations.EvalConstExpr());
ap2p.Add(new SimpleLang.ThreeCodeOptimisations.DeadOrAliveOptimizationAdapter());
var blockwss = ap2p.Apply(treeCod2e);

Console.WriteLine(ThreeAddressCodeVisitor.ToString(blockwss));
