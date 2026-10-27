#include <iostream>
using namespace std;


int main()
{
    double x, y, z;
    double f=0;

    cout << "Input x y z\n";
    cin >> x >> y >> z;
    double i = pow(x,2) + (pow(z,3) > x ? pow(z,3) : x);
    if (abs(i)< 1E-6) { cout << "Error, debtor = 0" << endl; }

    else {
         f = (pow((x > y ? y : x), 2) - y)/i;
        cout << "F = " << f << endl;
    }
    system("pause");
}

