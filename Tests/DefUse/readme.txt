DefUseConstOpt_DefUseDeadCodeOpt
¬ычислить def-use дл€ каждой переменной внутри ЅЅл, на основе этой информации выполнить каскадные прот€жку констант и удаление мертвого кода.

–еализаци€ разбита на 2 класса:
-класс DefUseConstOpt выполн€ет каскадную прот€жку констант;
-класс DefUseDeadCodeOpt выполн€ет каскадное удаление мертвого кода.

”даление мертвого кода выполн€ть только если в исходной программе один ЅЅл, т.к нельз€ удал€ть определение переменной пока мы не знаем, что она не используетс€ в других ЅЅл, а оптимизаци€ работает только внутри одного ЅЅл.

ƒл€ запуска оптимизаций необходимо добавить экземл€р класса DefUseDeadCodeOp/DefUseConstOpt в экземпл€р класса AutoThreeCodeOptimiser и вызывать метод Apply:

AutoThreeCodeOptimiser app = new AutoThreeCodeOptimiser();
app.Add(new DefUseConstOpt());
app.Add(new DefUseDeadCodeOpt());
var blocks = app.Apply(treeCode);

√де treeCode - экземпл€р класса ThreeAddressCodeVisitor.