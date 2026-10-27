#include <iostream>
#include <ctime>
#define N 20
using namespace std;

int main() {
    int arr[N];
    int choice;
    int i;

    cout << "Choice type input array:\n";
    cout << "1 - Input\n";
    cout << "2 - Generate randome\n";
    cin >> choice;
    switch (choice)
    {
    default: {
        cout << "Error!" << endl;
        return 0;
    }
    case 1: {
        cout << "Input elemet`s of array: ";
        for (i = 0; i < N; i++) cin >> arr[i];
    }
    case 2: {

        int x[N]{ 0 };

        srand((unsigned)time(NULL));
        for (i = 0; i < N; i++) {
            x[i] = rand() % 45 - 25;
            arr[i] = x[i];
            }
        }
    }
    cout << "\nInput array : ";
    for (i = 0; i < N; i++) cout << arr[i] << " ";

    int first = -1, last = -1;
    for (int i = 0; i < N; i++) {
        if (arr[i] % 2 != 0) {
            first = arr[i];
            break;
        }
    }

    int dif = 0;
    if (first == -1) {
        cout << "\nDidn`t find %2 != 0 in array!";
        system("pause");
        return 0;
    }

    for (int i = N; i < 0; i--) {
        if (arr[i] % 2 != 0) {
            last = arr[i];
            break;
        }
    }

    dif = first - last;

    cout << "\nThe difference between the first and last odd numbers: " << dif << endl;

    for (int i = 2; i < N; i += 3) {
        arr[i] += dif;
    }

    cout << "Modify array: ";
    for (int i = 0; i < N; i++) cout << arr[i] << " ";
    cout << endl;

    return 0;
}

