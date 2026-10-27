#include <iostream>
using namespace std;

double y_math(double a, double b, double c)
{
    double y;
    y = ((a - c) / (pow(a, 2) + a * c - pow(c, 2))) * ((pow(a, 3) - pow(c, 3)) / (pow(a, 2) * b - b * pow(c, 2))) * (1 - (a / (a - c)) - ((1 + c) / c));
    return y;
}

double maximum()
{
    int x, max = 0; 
    int cont;
    do{
        cout << "Input nuber\n";
        cin >> x;
        if (x > max) max = x;
        cout << "Continue? (0, end)\n";
        cin >> cont;
    } while (cont != 0);
    return max;
}

double function(int N)
{
    double a, b, c;
    double rez;
    if (N % 15 < 8) {
        cout << "Input 3 nubers\n";
        cin >> a >> b >> c;
        rez = y_math(a, b, c);
    }
    else rez = maximum();
    return rez;
}


int main()
{

    int N;
    double y;
    cout << "Input int\n";
    cin >> N;
    y = function(N);
    cout << "Result y(" << N << ") = " << y << endl;
    system("pause");
}


