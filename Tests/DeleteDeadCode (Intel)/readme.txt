DeleteOfDeadCodeOfThreeAddressCode
Удаление мертвого кода для трехадресного кода в одном блоке.

Если мы при проходе снизу вверх встретили команду x := выражение и x – мёртвая, 
то команда x := выражение является мёртвым кодом и её можно удалить.

Запуск:
  var ddc = new DeleteOfDeadCodeOfThreeAddressCode(treeCode);
  ddc.DeleteDeadCode();
  var p = ddc.Program;