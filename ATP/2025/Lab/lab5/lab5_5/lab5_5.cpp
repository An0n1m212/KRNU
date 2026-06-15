#include <iostream>
#include <ctime>

using namespace std;

int** inpMas(int& n, int& m);
int** genMas(int& n, int& m);
void outMas(int** mas, int n, int m);
int* DiagMas(int** mas, int n, int m);
void outDaigMat(int* newmas, int size);
void delMat(int* mas);
void delMas(int** mas, int n);

int main() {
    int n = 0, m = 0, newsize = 0, choice;
    int** mas = nullptr;
    int* newmas = nullptr;


    srand(time(NULL));

    do {
        cout << "Choice type input array:\n";
        cout << "1 - Input manually\n";
        cout << "2 - Generate random\n";
        cout << "0 - Exit!\n";
        cin >> choice;

        switch (choice) {
        case 1:
            cout << "\nInput Size:\n";
            mas = inpMas(n, m);
            break;

        case 2:
            cout << "\nInput Size mas:\n";
            mas = genMas(n, m);
            break;

        case 0:
            return 0;

        default:
            cout << "Incorrect input!\n";
            continue;
        }

        cout << "\nArray:\n";
        outMas(mas, n, m);
        system("pause");

        cout << "\nDiagonal array:\n";
        newmas = DiagMas(mas, n, m);
        newsize = n < m ? n : m;
        outDaigMat(newmas, newsize);
        delMas(mas, n);
        delMat(newmas);

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

    int** mas = new int* [n];
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

    int** mas = new int* [n];
    for (int i = 0; i < n; ++i)
        mas[i] = new int[m];

    for (int i = 0; i < n; ++i)
        for (int j = 0; j < m; ++j) {
            mas[i][j] = rand() % 91 - 45;
        }

    return mas;
}

void outMas(int** mas, int n, int m) {
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            cout << mas[i][j] << "\t";
        }
        cout << endl;
    }
}

int* DiagMas(int** mas, int n, int m) {
    int index = 0;
    int size = n < m ? n : m;
    int* newmas = new int[size];

        for (int i = 0; i < size; ++i) {
            newmas[index++] = mas[i][i];
        }
        return newmas;
}

void outDaigMat(int* newmas, int size) {
    for (int i = 0; i < size; ++i) {
        cout << newmas[i] << " ";
    }
    cout << endl;
}

void delMas(int** mas, int n) {
    for (int i = 0; i < n; i++)
        delete[] mas[i];
    delete[] mas;
}

void delMat(int* mas) {
    delete[] mas;
}


