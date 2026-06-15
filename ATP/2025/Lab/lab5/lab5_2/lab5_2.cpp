#include <iostream>
#include <ctime>

using namespace std;

int* inpMas(int& n);
int* genMas(int& n);
void outMas(int* mas, int n);
int* newMas(int* masA, int nA, int* masB, int nB, int& q);
void delMas(int* mas);

int main() {
    int nA = 0, nB = 0, q = 0, choice;
    int* masA = nullptr;
    int* masB = nullptr;
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
            masA = inpMas(nA);
            masB = inpMas(nB);
            break;

        case 2:
            cout << "\nInput Size mas:\n";
            masA = genMas(nA);
            masB = genMas(nB);
            break;

        case 0:
            return 0;

        default:
            cout << "Incorrect input!\n";
            continue;
        }

        cout << "\nOriginal array A:\n";
        outMas(masA, nA);

        cout << "\nOriginal array B:\n";
        outMas(masB, nB);

        cout << "\nNew array:\n";
        newmas = newMas(masA, nA, masB, nB, q);
        outMas(newmas, q);

        delMas(masA);
        delMas(masB);
        delMas(newmas);

        system("pause");

    } while (true);

    return 0;
}

int* inpMas(int& n) {
    cout << "n = ";
    cin >> n;

    int* mas = new int[n];
    for (int i = 0; i < n; ++i) {
        cout << "mas[" << i << "] = ";
        cin >> mas[i];
    }
    return mas;
}

int* genMas(int& n) {
    cout << "n = ";
    cin >> n;

    int* mas = new int[n];
    for (int i = 0; i < n; ++i) {
        mas[i] = rand() % 91 - 45;
    }
    return mas;
}

int* newMas(int* masA, int nA, int* masB, int nB, int& q) {
    int* newmas = new int[nB];
    q = 0;

    for (int i = 0; i < nB; i++) {
        bool found = false;
        for (int j = 0; j < nA; j++) {
            if (masB[i] == masA[j]) {
                found = true;
                break;
            }
        }
        if (!found) {
            newmas[q++] = masB[i];
        }
    }
    return newmas;
}

void outMas(int* mas, int n) {
    for (int i = 0; i < n; i++) {
        cout << mas[i] << " ";
    }
    cout << endl;
}

void delMas(int* mas) {
    delete[] mas;
}
