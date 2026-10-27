// ============================================================
// Лабораторна робота №9. Варіант 1
// Тема: змінні та робота з типами даних у JavaScript
//
// Завдання: створити змінну "age" з числовим значенням і
// перевести його у рядок; оголосити змінну "isStudent" з логічним
// значенням і перевести її у рядок. Після виконання завдання
// вивести всі змінні та їх типи у консоль.
// ============================================================

// 1. Числове значення -> рядок
var age = 25;
var ageAsString = String(age);

// 2. Логічне значення -> рядок
var isStudent = true;
var isStudentAsString = isStudent.toString();

// 3. Виведення всіх змінних та їх типів у консоль
console.log("age =", age, "| тип:", typeof age);
console.log("ageAsString =", ageAsString, "| тип:", typeof ageAsString);
console.log("isStudent =", isStudent, "| тип:", typeof isStudent);
console.log("isStudentAsString =", isStudentAsString,
    "| тип:", typeof isStudentAsString);
