#include <iostream>
using namespace std;
int main() {
    float x, y, z;
    int a;
    do {
        cout << "Input a: ";
        cin >> a;
        x = 0.31;
        for (; ; )
        {
            if (x > 0.61) 
                break;
            y = a * pow(x, 5/2) + cos(sqrt(exp (x)));
            cout << " x = " << x << endl << " y = " << y << endl << endl;
            x = x + 0.3;
        }
        system("pause");

        cout << "Continue?  0(no) ";
        cin >> a;
    } while (a);
}
