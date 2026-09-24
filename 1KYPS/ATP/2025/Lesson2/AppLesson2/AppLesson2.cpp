#include <iostream>
using namespace std;
int main()
{
	// if, if-else, switch, break, ?:
	/*
	if(Умова(true)){
		вираз1
		вираз2
	}esle {
	///
	}

	*/
	double x, y;

	cout << "Input number:\n";
	cin >> x;
	cout << "Input number:\n";
	cin >> y;
	if (x > y) {
		x *= 2;
		cout << x << ">" << y << endl;
	}
	else
	{
		cout << x << "<=" << y << endl;
		x += y;
	}

	int value;
	cout << "Input ***:\n";
	cin >> value;

	if (!(abs(value) >= 100 && abs(value) <= 999))
	{
		cerr << "Error: input namber!!!\n";
		system("pause");
		return 0;
	}
	int v1 = value / 100; 
	int v2 = value / 10 % 10;
	int v3 = value % 10;
	if (v1 != v2 && v1 != v3 && v2 != v3) {
		value /= 2;
		cout << value << "  Ok\n";
	}
	system("pause");






		/*if (value <0) {
		cout << "1111\n";
	}
	else{
		if(value > 0 && value < 123) {
			cout << "2222\n";
		}
		else
			{
			if (value >=123 && value <500)
				{
				cout << "3333\n";
				}
			}
			else
			{
			if (value >= 500 && value < 5000)
			{
				cout << "4444\n";
			}
			else
			{
				cout << "ELSE!!!!\n";
			}
			}
}*/
;
}
