#include <iostream>
using namespace std;


int main()
{
	double x;
	int n, f;
	const double Eps = 0.0001;
	double s = 0, u;
	int choice;
	do {
		cout << "Please make a selection: \n";
		cout << "1) Sum all\n";
		cout << "2) Sum first elements\n";
		cout << "3) n-Element\n";
		cout << "0) Exit\n";
		cin >> choice;
	} while (choice != 1 && choice != 2 && choice != choice != 0);
	switch (choice) {
	case 1:
		cout << "Input x: " << endl;
		cin >> x;
		u = 1 / x;
		n = 0;
		while 
			(fabs(u) >= Eps) {
			s += u;
			u *= 3 * pow(x, 3);
			n++;
		}
		cout << "\nSum elemetns accuracy " << Eps << " = " << s;
		system("pause");

	case 2:
		cout << "Input x: " << endl;
		cin >> x;
		u = 1 / x;
		cout << "Input n: " << endl;
		cin >> n;
		for (f = 0; f < n; f++) {
			u *= 3 * pow(x, 3);
			system("pause");

	case 3:
		cout << "Input x: " << endl;
		cin >> x;
		u = 1 / x;
		cout << "Input n: "<< endl;
		cin >> n;
		for (f = 0; f < n; f++) {
			u *= 3 * pow(x, 3);
			system("pause");

	case 0: break;
		}

		}
	}
}
}