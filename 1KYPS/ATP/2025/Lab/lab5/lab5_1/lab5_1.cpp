#include <iostream>
#include <ctime>

using namespace std;

int countMat(int* mas, int n);
int* inpMat(int& n);
int* genMat(int& n);
void outMat(int* mas, int n);
int* newMat(int* mas, int n, int& count3rd);
void delMat(int* mas);

int main() {
    int n = 0, choice;
    int* mas = nullptr;
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
            mas = inpMat(n);
            break;

        case 2:
            mas = genMat(n);
            break;

        case 0:
            return 0;

        default:
            cout << "Incorrect input!\n";
            continue;
        }

        cout << "\nOriginal array:\n";
        outMat(mas, n);

        int negCount = countMat(mas, n);
        cout << "Count negative elements on odd indices = " << negCount << endl;

        int count3rd = 0;
        newmas = newMat(mas, n, count3rd);

        cout << "\nNew array (3rd element):\n";
        outMat(newmas, count3rd);

        delMat(mas);
        delMat(newmas);

        system("pause");

    } while (true);

    return 0;
}

int* inpMat(int& n) {
    cout << "n = ";
    cin >> n;

    int* mas = new int[n];
    for (int i = 0; i < n; ++i) {
        cout << "mas[" << i << "] = ";
        cin >> mas[i];
    }
    return mas;
}

int* genMat(int& n) {
    cout << "n = ";
    cin >> n;

    int* mas = new int[n];
    for (int i = 0; i < n; ++i) {
        mas[i] = rand() % 91 - 45; 
    }
    return mas;
}

int countMat(int* mas, int n) {
    int count = 0;
    for (int i = 1; i < n; i += 2) {
        if (mas[i] < 0)
            count++;
    }
    return count;
}

int* newMat(int* mas, int n, int& count3rd) {

    count3rd = 0;
    for (int i = 2; i < n; i += 3) {
        count3rd++;
    }

    int* newmas = new int[count3rd];

    int index = 0;
    for (int i = 2; i < n; i += 3) {
        newmas[index++] = mas[i];
    }

    return newmas;
}

void outMat(int* mas, int n) {
    for (int i = 0; i < n; i++) {
        cout << mas[i] << " ";
    }
    cout << endl;
}

void delMat(int* mas) {
    delete[] mas;
}
