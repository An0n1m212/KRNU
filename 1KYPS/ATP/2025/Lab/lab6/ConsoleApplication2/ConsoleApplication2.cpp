#include <iostream>
#include <cstring>
#include <cstdlib>
#include <Windows.h>
using namespace std;

const int Size = 10;
const int cols = 2;
const int len = 46;

void translateWord(const char* word, char* translation_buffer, size_t bufferSize);
void toLowerCase(const char* src, char* dst);

char dictionary[Size][cols][len] =
{
    {"Hello", "Привіт"},
    {"World", "Світ"},
    {"Programming", "Програмування"},
    {"Book", "Книга"},
    {"Car", "Машина"},
    {"House", "Дім"},
    {"Water", "Вода"},
    {"I", "Я"},
    {"You", "Ти"},
    {"Want", "Хочу"}
};

void printDictionary()
{
    cout << "Вміст словника (English -> Українська):" << endl;
    cout.width(len); cout << left << "ENGLISH";
    cout.width(len); cout << left << "UKRAINIAN" << endl;

    for (int i = 0; i < Size; ++i)
    {
        cout.width(len); cout << left << dictionary[i][0];
        cout.width(len); cout << left << dictionary[i][1] << endl;
    }
}

int main()
{
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    cout << "English-Ukrainian Translator (char version)" << endl;

    printDictionary();

    while (true)
    {
        char inputWord[len];
        char translation[len];

        cout << "\nEnter a word in English (or 'exit' to quit): ";
        cin.getline(inputWord, len);

        if (_stricmp(inputWord, "exit") == 0)
            break;

        if (inputWord[0] == '\0')
            continue;

        translateWord(inputWord, translation, len);

        cout << "Translation: " << translation << endl;
    }

    return 0;
}

void toLowerCase(const char* src, char* dst)
{
    size_t n = strlen(src);
    for (size_t i = 0; i < n; i++)
        dst[i] = tolower((unsigned char)src[i]);
    dst[n] = '\0';
}

void translateWord(const char* word, char* translation_buffer, size_t bufferSize)
{
    char lowerWord[len];
    toLowerCase(word, lowerWord);

    for (int i = 0; i < Size; i++)
    {
        char dictLower[len];
        toLowerCase(dictionary[i][0], dictLower);

        if (strcmp(dictLower, lowerWord) == 0)
        {
            strcpy_s(translation_buffer, bufferSize, dictionary[i][1]);
            return;
        }
    }

    // Якщо нема — показати слово у квадратних дужках
    strcpy_s(translation_buffer, bufferSize, "[");
    strcat_s(translation_buffer, bufferSize, word);
    strcat_s(translation_buffer, bufferSize, "]");
}
