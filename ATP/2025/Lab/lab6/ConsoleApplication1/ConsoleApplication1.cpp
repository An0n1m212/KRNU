#include <iostream>
#include <cstring>
#include <cstdlib>
#include <Windows.h>

using namespace std;

const int MAX_INPUT_LEN = 255;
const int MAX_WORDS = 50;

bool is_less(const char* a, const char* b) {
    size_t len_a = strlen(a);
    size_t len_b = strlen(b);

    if (len_a < len_b) {
        return true;
    }
    if (len_a > len_b) {
        return false;
    }

    if (len_a > 0) {
        char last_a = a[len_a - 1];
        char last_b = b[len_b - 1];

        return last_a < last_b;
    }

    return false;
}

void bubblesort(char** a, int n) {
    bool isSort;
    int i;
    do {
        isSort = false;
        for (int j = 0; j < n - i - 1; ++j) {
            if (!is_less(a[j], a[j + 1])) {
                char* temp = a[j];
                a[j] = a[j + 1];
                a[j + 1] = temp;
                isSort = true;
            }
        }
    }while(isSort == true);
}

int main() {
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    system("cls");

    char* input_buffer = new char[MAX_INPUT_LEN];
    char** words_array = new char* [MAX_WORDS];
    int word_count = 0;

    cout << "Введіть рядок слів (не більше " << MAX_INPUT_LEN - 1 << " символів):" << endl;
    cin.getline(input_buffer, MAX_INPUT_LEN);

    char* token = strtok(input_buffer, " ");
    while (token != nullptr && word_count < MAX_WORDS) {
        size_t len = strlen(token);
        words_array[word_count] = new char[len + 1];
        strcpy(words_array[word_count], token);
        word_count++;
        token = strtok(nullptr, " ");
    }

    delete[] input_buffer;

    if (word_count == 0) {
        cout << "\nРядок не містить слів." << endl;
        delete[] words_array;
        system("pause");
        return 0;
    }

    cout << "\n--- Початковий список слів ---" << endl;
    for (int i = 0; i < word_count; ++i) {
        cout << words_array[i] << " ";
    }
    cout << endl;

    bubblesort(words_array, word_count);

    cout << "\n--- Відсортований список слів (Сортування бульбашкою) ---" << endl;
    for (int i = 0; i < word_count; ++i) {
        cout << words_array[i] << " ";
        delete[] words_array[i];
    }
    cout << endl;

    delete[] words_array;

    system("pause");
    return 0;
}