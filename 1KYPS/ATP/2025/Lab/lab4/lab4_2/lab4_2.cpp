//Нехай задано матрицю суміжності графа С розмірності n × n.
//Необхідно реалізувати крок приведення матриці та провести оцінювання нулів у матриці.
//Вивести на екран індекси нуля або нулів матриці вага яких найбільша.

#include <iostream>
#include <ctime>
#define MAX 20
#define inf 1e12

using namespace std;

void inputMat(double C[MAX][MAX], int n);
void genMat(double C[MAX][MAX], int n);
void outMat(double C[MAX][MAX], int n);
void newMat(double C[MAX][MAX], int n);
bool findWal0(double C[MAX][MAX], int n, int &bestI, int &bestJ, double &bestWeight);


int main() {
    int n, choice;
    int ix[MAX * MAX], jx[MAX * MAX];
    double C[MAX][MAX];

    do {
        cout << "Enter matrix size n < " << MAX << " (n=0 -> Exit): ";
        cin >> n;

        if (n == 0) {
            cout << "Exit!\n";
            break;
        }
        if (n > MAX) {
            cout << "Input n > " << MAX << ", try again\n";
            continue;
        }

        cout << "Choice type input array:\n";
        cout << "1 - Input manually\n";
        cout << "2 - Generate random\n";
        cin >> choice;

        switch (choice) {
        case 1:
            inputMat(C, n);
            break;
        case 2:
            genMat(C, n);
            break;
        default:
            cout << "Incorrect input!\n";
            continue;
        }

        cout << "\nOriginal matrix:\n";
        outMat(C, n);
        newMat(C, n);
        system("pause");

        cout << "\nReduced matrix:\n";
        outMat(C, n);
        system("pause");
        int count = findWal0(C, n, ix, jx);
        cout << "Found zeros: " << count << "\n";
        for (int k = 0; k < count; k++)
            cout << "(" << ix[k] << ", " << jx[k] << ")\n";

        system("pause");
    } while (true);

    return 0;
}

void inputMat(double C[MAX][MAX], int n) {
    cout << "Enter adjacency matrix (" << n << "x" << n << "):\n";
    for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            cin >> C[i][j];
}

void genMat(double C[MAX][MAX], int n) {
    srand((unsigned)time(NULL));
    cout << "Generated adjacency matrix (" << n << "x" << n << "):\n";
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            C[i][j] = rand() % 45 + 1;
        }
    }
}

void outMat(double C[MAX][MAX], int n) {
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (C[i][j] == inf)
                cout << "inf\t";
            else
                cout << C[i][j] << "\t";
        }
        cout << endl;
    }
}

void newMat(double C[MAX][MAX], int n) {
    for (int i = 0; i < n; i++) {
        C[i][i] = inf;
    }

    for (int i = 0; i < n; i++) {
        double rowMin = inf;
        for (int j = 0; j < n; j++)
            if (C[i][j] < rowMin)
                rowMin = C[i][j];
        for (int j = 0; j < n; j++)
            C[i][j] -= rowMin;
    }

    for (int j = 0; j < n; j++) {
        double colMin = inf;
        for (int i = 0; i < n; i++)
            if (C[i][j] < colMin)
                colMin = C[i][j];
        for (int i = 0; i < n; i++)
            C[i][j] -= colMin;
    }
}


int findWal0(double C[MAX][MAX], int n, int bestI[], int bestJ[]) {
    double bestWeight = inf;
    int bestCount = 0;

    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (C[i][j] == 0) {

                double minRow = inf, minCol = inf;

                for (int k = 0; k < n; k++)
                    if (k != j && C[i][k] < minRow)
                        minRow = C[i][k];

                for (int k = 0; k < n; k++)
                    if (k != i && C[k][j] < minCol)
                        minCol = C[k][j];

                double weight = minRow + minCol;

                if (weight < bestWeight) {
                    bestWeight = weight;
                    bestCount = 0;
                    bestI[bestCount] = i;
                    bestJ[bestCount] = j;
                    bestCount++;
                }
                else if (weight == bestWeight) {
                    bestI[bestCount] = i;
                    bestJ[bestCount] = j;
                    bestCount++;
                }
            }
        }
    }

    return bestCount;
}

