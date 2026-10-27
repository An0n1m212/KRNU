#include <iostream>	
using namespace std;


int main()
{
	int N;
	cout << "Input for M position on X: ";
	float x;
	cin >> x;
	cout << "Input for M position on Y: ";
	float y;
	cin >> y;
	N =  (x * x + y * y < 16) ?( (x * y > 0) ? 1 : (x < 0)?3: 2): 3;

	cout << "\tResult: ";
	cout << "Dot M(" << x << ";" << y << ") behove N = " << N;
	cout << "\nPress Enter to continue.";
	system("pause");
}