# Документация проекта onlyReal

# Введение !heading
В этом проекте перед командами стояла задача разработать грамматику языка и создать дял неё оптимизирующий компилятор.

Проект реализован на языке с#. Также были использованы генератор синтаксического анализатора Yacc и генератор лексического анализатора Lex.

# Структура проекта !heading
<table>
    <thead>
    <tr>
        <th>Раздел</th>
        <th>Задача</th>
        <th>Название команды</th>
    </tr>
    </thead>
    <tbody>
    <tr>
        <td>AST дерево</td>
        <td></td>
        <td></td>
    </tr>
    <tr>
        <td>1</td>
        <td>Оптимизация операции умножения на единицу</td>
        <td>Roll</td>
    </tr>
    <tr>
        <td>2</td>
        <td>Оптимизация операции умножения на ноль</td>
        <td>Roslyn</td>
    </tr>
    <tr>
        <td>3</td>
        <td>Оптимизация перемножения констант</td>
        <td>komanda</td>
    </tr>
    <tr>
        <td>4</td>
        <td>Оптимизация суммирования с нулем</td>
        <td>SouthPark</td>
    </tr>
    <tr>
        <td>5</td>
        <td>Оптимизация вычитания собственного значения</td>
        <td>Nvidia</td>
    </tr>
    <tr>
        <td>6</td>
        <td>Оптимизация операции сравнения <</td>
        <td>GreatBean</td>
    </tr>
    <tr>
        <td>7</td>
        <td>Оптимизация операции сравнения ==</td>
        <td>Intel</td>
    </tr>
    <tr>
        <td>8</td>
        <td>Оптимизация операций сравнения с собой</td>
        <td>qwerty</td>
    </tr>
    <tr>
        <td>9</td>
        <td>Оптимизация операций >, !=</td>
        <td></td>
    </tr>
    <tr>
        <td>10</td>
        <td>Оптимизация присваивания собственного значения</td>
        <td>Nvidia, BOOM</td>
    </tr>
    <tr>
        <td>11</td>
        <td>Оптимизация if (true)</td>
        <td>Roslyn</td>
    </tr>
    <tr>
        <td>12</td>
        <td>Оптимизация if (false)</td>
        <td>GreatBean, komanda, Intel</td>
    </tr>
    <tr>
        <td>13</td>
        <td>Оптимизация if (ex) null else null</td>
        <td>qwerty</td>
    </tr>
    <tr>
        <td>14</td>
        <td>Оптимизация while (false)</td>
        <td>Roll</td>
    </tr>
    <tr>
        <td>15</td>
        <td>Удаление всех null</td>
        <td>BOOM</td>
    </tr>
    <tr>
        <td>16</td>
        <td>Оптимизация вложенных блоков</td>
        <td>SouthPark</td>
    </tr>
    <tr>
        <td>Трехадресный код</td>
        <td></td>
        <td></td>
    </tr>
    <tr>
        <td>1</td>
        <td>Генерация трехадресного кода</td>
        <td>Roslyn</td>
    </tr>
    <tr>
        <td>2</td>
        <td>Логические тождества < > and or true false</td>
        <td>GreatBean</td>
    </tr>
    <tr>
        <td>3</td>
        <td>Свертка const, алгебраические тождества (в т.ч. каскадные) + протяжка const</td>
        <td>Roslyn</td>
    </tr>
    <tr>
        <td>4</td>
        <td>Выделение ББл</td>
        <td>Nvidia</td>
    </tr>
    <tr>
        <td>5</td>
        <td>Живые и мёртвые переменные внутри ББл - анализ удаления мертвого кода (в т.ч. каскадное)</td>
        <td>Roll</td>
    </tr>
    <tr>
        <td>6</td>
        <td>Вычисление Def-Use: Удаление мертвого кода, протяжка const (каскадная)</td>
        <td>komanda</td>
    </tr>
    <tr>
        <td>7</td>
        <td>Оптимизация общих подвыражений (простейшая) + протяжка копий</td>
        <td>qwerty</td>
    </tr>
    <tr>
        <td>8</td>
        <td>Очистка от пустых операторов устранение переходов через переходы</td>
        <td>SouthPark</td>
    </tr>
    <tr>
        <td>9</td>
        <td>Устранение недостижимого кода. Устранение переходов к переходам</td>
        <td>BOOM</td>
    </tr>
    <tr>
        <td>10</td>
        <td>Удаление мертвого кода, протяжка копий</td>
        <td>Intel</td>
    </tr>
    <tr>
        <td>11</td>
        <td>CFG</td>
        <td>Nvidia</td>
    </tr>
    <tr>
        <td>12</td>
        <td>Алгоритм LVN</td>
        <td>Roll</td>
    </tr>
    <tr>
        <td>13</td>
        <td>Устранение локальных общих подвыражений построением ациклического графа</td>
        <td>qwerty</td>
    </tr>
    <tr>
        <td>14</td>
        <td>Для достигающих определений вычислить genB, killB для любого B и разработать структуру для хранения передаточной функции.
            fB = fSn*fSn-1*...*fS1 - вычислить fSi для каждой инструкции ББл и потом найти композицию</td>
        <td>Roslyn</td>
    </tr>
    <tr>
        <td>15</td>
        <td>Хранение IN[B] и OUT[B] для ряда задач</td>
        <td>SouthPark</td>
    </tr>
    <tr>
        <td>16</td>
        <td>Для достигающих определений вычислить genB, killB для любого B и разработать структуру для хранения передаточной функции.
            Вычислить fB по яв. формулам fB(x)=genB U (x - killB) killB = kill1 U kill2 U ... U killn, gebB = ....</td>
        <td>Intel</td>
    </tr>
    <tr>
        <td>17</td>
        <td>Итерационный Алгоритм для достигающих определений</td>
        <td>Nvidia</td>
    </tr>
    <tr>
        <td>18</td>
        <td>Оптимизация на основе ИтА для активных переменных - удаление мертвого кода </td>
        <td>Roll</td>
    </tr>
    <tr>
        <td>19 - нет</td>
        <td>Оптимизация протяжка const на основе инф., полученной в результате применения ИтА для достиг. определений</td>
        <td>Roslyn</td>
    </tr>
    <tr>
        <td>20</td>
        <td>Вычисление множеств DEFb и USEb д/активных переменных</td>
        <td>qwerty</td>
    </tr>
    <tr>
        <td>21</td>
        <td>Итерационный алгоритм для активных переменных</td>
        <td>BOOM</td>
    </tr>
    <tr>
        <td>22</td>
        <td>Итерационный алгоритм для доступных выражений</td>
        <td>GreatBean</td>
    </tr>
    <tr>
        <td>23</td>
        <td>Класс передаточной функции (общий):
            - ф-ии формулой
            - ф-ии алгоритмом
            - суперпозиция ф-ий</td>
        <td>Roll</td>
    </tr>
    <tr>
        <td>24</td>
        <td>Доступные выражения-множества e_genB, e-KILLb. Передаточная функция ББл В.</td>
        <td>SouthPark</td>
    </tr>
    <tr>
        <td>25 - нет</td>
        <td>На основе анализа доступных выражений провести оптимизации</td>
        <td>Roslyn</td>
    </tr>
    <tr>
        <td>26</td>
        <td>Генератор IL-кода</td>
        <td>Roslyn</td>
    </tr>
    <tr>
        <td>27</td>
        <td>Итерационный алгоритм в задаче растпространения констант</td>
        <td>Nvidia</td>
    </tr>
    <tr>
        <td>28</td>
        <td>Обобщенный итерационный алгоритм</td>
        <td>Roll</td>
    </tr>
    <tr>
        <td>29 - нет</td>
    </tr>
    <tr>
        <td>30 - нет</td>
        <td>Поиск решения методом MOP</td>
        <td>Intel</td>
    </tr>
    <tr>
        <td>31</td>
        <td>Передаточная функция в задаче о распространении констант</td>
        <td>Roslyn</td>
    </tr>
    <tr>
        <td>32</td>
        <td>Вычисление доминаторов</td>
        <td>komanda</td>
    </tr>
    <tr>
        <td>33</td>
        <td>ControlFlowGraph перевести в Трехадресный код</td>
        <td>BOOM</td>
    </tr>
    <tr>
        <td>34</td>
        <td>Определение того, является ли ребро обратным и являеется ли CFG приводимым</td>
        <td>GreatBean</td>
    </tr>
    <tr>
        <td>35</td>
        <td>Определение глубины CFG</td>
        <td>Roll</td>
    </tr>
    <tr>
        <td>36</td>
        <td>Классификация ребер в глубинном остовном дереве + Построение глубинного остовного дерева с соответствующей нумерацией вершин</td>
        <td>qwerty SouthPark</td>
    </tr>
    <tr>
        <td>37</td>
        <td>Определение всех естественных циклов в CFG с информацией об их вложенности</td>
        <td>Intel</td>
    </tr>
    </tbody>
</table>

#include "ast/1-ast.md"
#include "ast/2-ast.md"
#include "ast/3-ast.md"
#include "ast/4-ast.md"
#include "ast/5-ast.md"
#include "ast/6-ast.md"
#include "ast/7-ast.md"
#include "ast/8-ast.md"
#include "ast/9-ast.md"
#include "ast/10-ast.md"
#include "ast/11-ast.md"
#include "ast/12-ast.md"
#include "ast/13-ast.md"
#include "ast/14-ast.md"
#include "ast/15-ast.md"
#include "ast/16-ast.md"

#include "tac/1-tac.md"
#include "tac/2-tac.md"
#include "tac/3-tac.md"
#include "tac/4-tac.md"
#include "tac/5-tac.md"
#include "tac/6-tac.md"
#include "tac/7-tac.md"
#include "tac/8-tac.md"
#include "tac/9-tac.md"
#include "tac/10-tac.md"
#include "tac/11-tac.md"
#include "tac/12-tac.md"
#include "tac/13-tac.md"
#include "tac/14-tac.md"
#include "tac/15-tac.md"
#include "tac/16-tac.md"
#include "tac/17-tac.md"
#include "tac/18-tac.md"
include "tac/19-tac.md"
#include "tac/20-tac.md"
#include "tac/21-tac.md"
#include "tac/22-tac.md"
#include "tac/23-tac.md"
#include "tac/24-tac.md"
include "tac/25-tac.md"
#include "tac/26-tac.md"
#include "tac/27-tac.md"
#include "tac/28-tac.md"
include "tac/29-tac.md"
include "tac/30-tac.md"
#include "tac/31-tac.md"
#include "tac/32-tac.md"
#include "tac/33-tac.md"
#include "tac/34-tac.md"
#include "tac/35-tac.md"
#include "tac/36-tac.md"
#include "tac/37-tac.md"
#include "tac/100-tac.md"