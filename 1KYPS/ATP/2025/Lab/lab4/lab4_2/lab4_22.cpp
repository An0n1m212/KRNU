#include <iostream>
#include <ctime>
#define MAX 20
#define inf 1e12


using namespace std;

int main() {
    int n;
    double C[MAX][MAX];
    int choice;
    double minCol = 1e9, minRow = 1e9;
    while (n == 0) {
        cout << "Enter matrix size n < " << MAX << "(n=0, Exit!): ";
        cin >> n;

        if (n > MAX) {
            cout << "Input n > " << MAX << ", try again\n";
        }
        while (choice != 0) {
            cout << "Choice type input array:\n";
            cout << "1 - Input\n";
            cout << "2 - Generate randome\n";
            cout << "0 - Exit!\n";
            cin >> choice;
            switch (choice)
            {
            default: {
                cout << "Input value != 1 || 2" << endl;
            }
            case 1: {
                cout << "Enter adjacency matrix (" << n << "x" << n << "):\n";
                for (int i = 0; i < n; i++)
                    for (int j = 1; j < n; j++)
                        cin >> C[i][j];
            }
            case 2: {

                srand((unsigned)time(NULL));

                cout << "Enter adjacency matrix (" << n << "x" << n << "):\n";
                for (int i = 0; i < n; i++) {
                    for (int j = 0; j < n; j++) {
                        C[i][j] = rand() % 55 + 1;
                        cout << C[i][j] << "\t";
                    } cout << endl;
                }
                for (int i = 0; i < n; i++) {
                    C[i][i] = inf;
                }
            }
            }; system("pause");
            for (int i = 0; i < n; i++) {
                double rowMin = C[i][0];
                for (int j = 0; j < n; j++)
                    if (C[i][j] < rowMin) rowMin = C[i][j];

                for (int j = 0; j < n; j++)
                    C[i][j] -= rowMin;
            }

            for (int j = 0; j < n; j++) {
                double colMin = C[0][j];
                for (int i = 0; i < n; i++)
                    if (C[i][j] < colMin) colMin = C[i][j];

                for (int i = 0; i < n; i++)
                    C[i][j] -= colMin;
            }



            cout << "\nReduced matrix:\n";
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++)
                    cout << C[i][j] << "\t";
                cout << endl;
            }
            system("pause");

            double maxWeight = -1;
            int bestI[MAX], bestJ[MAX], bestCount = 0;

            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    if (C[i][j] == 0) {
                        for (int k = 0; k < n; k++)
                            if (k != j && C[i][k] < minRow)
                                minRow = C[i][k];


                        for (int k = 0; k < n; k++)
                            if (k != i && C[k][j] < minCol)
                                minCol = C[k][j];

                        double weight = minRow + minCol;
                        if (weight == minRow + minCol) {
                            cout << "There is none zeros!";
                            return 0;
                        }

                        if (weight > maxWeight) {
                            maxWeight = weight;
                            bestCount = 0;
                            bestI[bestCount] = i;
                            bestJ[bestCount] = j;
                            bestCount++;
                        }
                        else if (weight == maxWeight) {
                            bestI[bestCount] = i;
                            bestJ[bestCount] = j;
                            bestCount++;
                        }
                    }
                }
            }

            cout << "\nZeros with the highest weight (" << maxWeight << "):\n";
            for (int k = 0; k < bestCount; k++)
                cout << "Zero at position (" << bestI[k] << ", " << bestJ[k] << ")\n";

            return 0;
        }
    };
}
