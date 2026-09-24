#include <iostream>
#include <ctime>
#define inf 1e11

using namespace std;

int** inpMas(int& n, int& m);
int** genMas(int& n, int& m);
int* newMas(int** mas, int n, int m);
void outMas(int** mas, int n, int m);
void outNewMas(int* sumelmas, int n);
void delMas(int** mas, int n);
void delNewMat(int* sumelmas);

int main() {
    int n = 0, m = 0, choice;
    int** mas = nullptr;
    int* sumelmas = nullptr;

    srand(time(NULL));

    do {
        cout << "Choice type input array:\n";
        cout << "1 - Input manually\n";
        cout << "2 - Generate random\n";
        cout << "0 - Exit!\n";
        cin >> choice;

        switch (choice) {
        case 1:
            mas = inpMas(n, m);
            break;

        case 2:
            mas = genMas(n, m);
            break;

        case 0:
            return 0;

        default:
            cout << "Incorrect input!\n";
            continue;
        }

        cout << "\nOriginal array:\n";
        outMas(mas, n, m);

        sumelmas = newMas(mas, n, m);
        cout << "\nNew array:\n";
        outNewMas(sumelmas, m);

        delMas(mas, n);
        delNewMat(sumelmas);

        system("pause");

    } while (true);

    return 0;
}

int** inpMas(int& n, int& m)
{
    cout << "n=";
    cin >> n;
    cout << "m=";
    cin >> m;

    int** mas = new int*[n];
    for (int i = 0; i < n; ++i)
        mas[i] = new int[m];

    for (int i = 0; i < n; ++i)
        for (int j = 0; j < m; ++j) {
            cout << "mas[" << i << "][" << j << "] = ";
            cin >> mas[i][j];
        }

    return mas;
}

int** genMas(int& n, int& m) {
    cout << "n=";
    cin >> n;
    cout << "m=";
    cin >> m;

    int** mas = new int*[n];
    for (int i = 0; i < n; ++i)
        mas[i] = new int[m];

    for (int i = 0; i < n; ++i)
        for (int j = 0; j < m; ++j)
            mas[i][j] = rand() % 91 - 45;

    return mas;
}

int* newMas(int** mas, int n, int m) {                 
    int* sumelmas = new int[n];

    for (int i = 0; i < n; ++i) {
        int min = inf;
        int max = -inf;

        for (int j = 0; j < m; ++j) {
            if (mas[i][j] < min) min = mas[i][j];
            if (mas[i][j] > max) max = mas[i][j];
        }

        sumelmas[i] = min + max;
    }

    return sumelmas;
}

void outMas(int** mas, int n, int m) {
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j)
            cout << mas[i][j] << "\t";
        cout << endl;
    }
}

void outNewMas(int* sumelmas, int n) {
    for (int i = 0; i < n; i++)
        cout << sumelmas[i] << " ";
    cout << endl;
}

void delMas(int** mas, int n) {
    for (int i = 0; i < n; i++)
        delete[] mas[i];
    delete[] mas;
}

void delNewMat(int* sumelmas) {
    delete[] sumelmas;
}
