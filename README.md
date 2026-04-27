# Лабораторная работа №3. Разработка синтаксического анализатора (парсера)

## Цель работы
Изучить назначение и принципы работы синтаксического анализатора в структуре компилятора. Спроектировать грамматику, построить схему метода анализа, реализовать парсер с нейтрализацией синтаксических ошибок методом Айронса и интегрировать разработанный модуль в графический интерфейс языкового процессора.

## Сведения об авторе
- **Студент:** Головина А. С.
- **Группа:** АП-327
- **Среда разработки:** Visual Studio 2022
- **Язык реализации:** C# / WPF

## Постановка задачи
Необходимо разработать синтаксический анализатор для конструкции объявления ассоциативного массива с инициализацией на языке C# и встроить его в интерфейс, разработанный ранее. Анализатор должен:
- принимать исходный текст из области редактирования;
- вызывать лексический анализатор;
- выполнять синтаксический разбор по утверждённой грамматике;
- фиксировать ошибки с указанием неверного фрагмента, строки и позиции;
- продолжать анализ после ошибки за счёт нейтрализации методом Айронса;
- выводить результаты в таблицу и обеспечивать переход к месту ошибки при щелчке по строке таблицы.

# Вариант задания

Лексический анализатор должен распознавать конструкцию объявления ассоциативного массива с инициализацией на языке C#.

Пример конструкции:

```csharp
Dictionary<int, string> My_dict1 = new Dictionary<int, string> {
    { 1, "one" },
    { 2, "two" },
    { 3, "three" }
};
```
### Грамматика
```text
Dictionary Declaration = "Dictionary", "<", "int", ",", "string", ">",
                         Dictionary Identifier,
                         "=",
                         "new", "Dictionary", "<", "int", ",", "string", ">",
                         "{",
                         Dictionary Element,
                         {",", Dictionary Element},
                         "}", ";";

Dictionary Element = "{", Number, ",", String, "}";
Dictionary Identifier = letter, {letter | digit | "_"};
Number = digit, {digit};
String = "\"", {symbol}, "\"";
```
---
## Полное определение грамматики

Грамматика задаётся четвёркой:

G = (V_T, V_N, P, S)

где:

- V_T — множество терминалов  
- V_N — множество нетерминалов  
- P — множество правил  
- S — начальный символ  

### Терминалы

Dictionary, int, string, new, <, >, ,, =, {, }, ;, ", letter, digit, symbol

### Нетерминалы

DictionaryDeclaration  
DictionaryElement  
DictionaryElementList  
DictionaryIdentifier  
Number  
String  
letter  
digit  
symbol  

### Начальный символ

S = DictionaryDeclaration

### Продукции

DictionaryDeclaration =
    "Dictionary", "<", "int", ",", "string", ">",
    DictionaryIdentifier,
    "=",
    "new", "Dictionary", "<", "int", ",", "string", ">",
    "{",
    DictionaryElement,
    { ",", DictionaryElement },
    "}", ";";

DictionaryElement =
    "{", Number, ",", String, "}";

DictionaryIdentifier =
    letter, { letter | digit | "_" };

Number =
    digit, { digit };

String =
    "\"", { symbol }, "\"";

letter =
    "A" | "B" | ... | "Z" | "a" | "b" | ... | "z";

digit =
    "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9";

symbol =
    letter | digit | "_" | " " | "." | "," | ":" | ";" | "!" | "?" | "-" | "+";

## Классификация грамматики по Хомскому


## Схема рекурсивного спуска

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/f58d9a83-b3b4-491a-b47f-dd5f672c10b9" />



## Метод Айронса

В анализаторе используется метод Айронса для нейтрализации синтаксических ошибок.

Суть метода заключается в том, что при обнаружении ошибки анализатор не прекращает работу, а пропускает входные лексемы до тех пор, пока не встретит синхронизирующий символ, с которого можно продолжить разбор.

Алгоритм:

1. Обнаруживается ошибка  
2. Записывается в таблицу  
3. Пропускаются лексемы  
4. До синхронизирующего символа  
5. Анализ продолжается  

Синхронизирующие символы:

;   {   }   ,

Это позволяет находить несколько ошибок за один запуск анализатора.

### Грамматика
```text
Dictionary Declaration = "Dictionary", "<", "int", ",", "string", ">",
                         Dictionary Identifier,
                         "=",
                         "new", "Dictionary", "<", "int", ",", "string", ">",
                         "{",
                         Dictionary Element,
                         {",", Dictionary Element},
                         "}", ";";

Dictionary Element = "{", Number, ",", String, "}";
Dictionary Identifier = letter, {letter | digit | "_"};
Number = digit, {digit};
String = "\"", {symbol}, "\"";
```

### Примеры корректных входных строк
```csharp
Dictionary<int, string> dict = new Dictionary<int, string>{ {1,"one"} };
```

```csharp
Dictionary<int, string> my_dict1 = new Dictionary<int, string>{ {1,"one"}, {2,"two"} };
```

```csharp
Dictionary<int, string> data_1 = new Dictionary<int, string>{ {10,"ten"}, {20,"twenty"}, {30,"thirty"} };
```

### Допустимые лексемы
- ключевые слова: `new`, `int`, `string`, `Dictionary`;
- идентификаторы: `letter, {letter | digit | "_"}`;
- целые беззнаковые числа;
- строковые литералы в двойных кавычках;
- разделители и символы: `<`, `>`, `,`, `=`, `{`, `}`, `;`.



## Интеграция в интерфейс
В приложении:
- кнопка **«Пуск»** сначала запускает лексический анализатор, затем синтаксический;
- результаты выводятся в таблицу ошибок;
- при щелчке по ошибке курсор переходит к ошибочному фрагменту в редакторе;
- при отсутствии ошибок выводится сообщение об успешном анализе.

## Тестовые примеры
### Корректная строка
```csharp
Dictionary<int, string> dict = new Dictionary<int, string>{ {1,"one"}, {2,"two"} };
```
**Результат:**
<img width="881" height="637" alt="image" src="https://github.com/user-attachments/assets/93d540a8-5277-4565-ae80-2caabe1ee199" />


### Одна ошибка
```csharp
Dictionary<int, string> = new Dictionary<int, string>{ {1,"one"} };
```
**Результат:**
<img width="879" height="644" alt="image" src="https://github.com/user-attachments/assets/115c5762-6787-4a6f-8b17-e143ec970c59" />


### Несколько ошибок
```csharp
Dictionary<int, string> dict = new Dictionary<int, string>{ {,"one"}; {2,two} };
```
**Результат:**
<img width="878" height="638" alt="image" src="https://github.com/user-attachments/assets/af6bb3d0-de76-4006-8d52-8e7170cdb7d2" />


### Пустая строка
```text

```
**Результат:**
<img width="881" height="639" alt="image" src="https://github.com/user-attachments/assets/52307951-fb25-4e61-ad4c-c8797b0a9ae3" />


### Без первого ключевого слова
```csharp
int dict = new Dictionary<int, string>{ {1,"one"} };
```
**Результат:**
<img width="874" height="636" alt="image" src="https://github.com/user-attachments/assets/473d2bbe-3a36-49db-8140-175cacb9d602" />


## Инструкция по сборке и запуску
1. Открыть решение `Laba1.sln` в Visual Studio 2022.
2. Собрать проект.
3. Запустить приложение.
4. Ввести или открыть текст программы.
5. Нажать кнопку **«Пуск»**.
6. Просмотреть таблицу ошибок или сообщение об успешном анализе.
