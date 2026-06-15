#include <iostream>
using namespace std;

int main() {
    double x, s, u;
    int n, f, choice;
    const double Eps = 1e-4;

    do {
        cout << "Please make a selection:\n";
        cout << "1) Sum to given accuracy\n";
        cout << "2) Sum first N elements\n";
        cout << "3) Show N-th element\n";
        cout << "0) Exit\n";
        cin >> choice;

        switch (choice) {
        case 1: {
            s = 0; n = 0;
            cout << "Input x: ";
            cin >> x;
            u = 1.0 / x;
            while (fabs(u) >= Eps) {
                s += u;
                u *= (double)(2 * n + 1) / (2 * n + 3) / (x * x);
                n++;
            }
            cout << "Sum to accuracy " << Eps << " = " << s << endl;
            cout << "Number of terms = " << n << endl;
            break;
        }

        case 2: {
            s = 0;
            cout << "Input x: ";
            cin >> x;
            cout << "Input number of elements n: ";
            cin >> n;
            u = 1.0 / x;
            for (f = 0; f < n; f++) {
                s += u;
                u *= (double)(2 * f + 1) / (2 * f + 3) / (x * x);
            }
            cout << "Sum of first " << n << " elements = " << s << endl;
            break;
        }

        case 3: {
            cout << "Input x: ";
            cin >> x;
            cout << "Input element number n: ";
            cin >> n;
            u = 1.0 / x;
            for (f = 0; f < n; f++) {
                u *= (double)(2 * f + 1) / (2 * f + 3) / (x * x);
            }
            cout << n << "-th element = " << u << endl;
            break;
        }

        case 0:
            cout << "Exiting..." << endl;
            break;

        default:
            cout << "Invalid selection. Try again." << endl;
        }
        system("pause");
        system("cls");
        
        
    } while (choice != 0); 
}
