#include <iostream>
using namespace std;

int sumaNumbers();
int sumaNumbers(int);
int sumaNumbers(int, int, int&);
void minMax(int, double&, double&);

int main()
{
	srand(time(NULL));
	int s = sumaNumbers();
	cout << "s= " << s << endl;
	int n;
	int count = 3;
	do {
		if (count == 0) {
			cout << "Bida: " << count << endl;
			system("pause");
			return 1;
		}
		cout << "n(n>1, n<50)= ";
		cin >> n;
		if (n <= 1 || n >= 50)
		{
			cout << "Error: (n>1, n<50)!!!\n";
			--count;
			cout << "Atempts " << count <<endl;
			system("pause");
		}
	} while (n <= 1 || n >= 50);
	s = sumaNumbers(n);
	cout << "\ns( " << n << " )= " << s << endl;
	int value = 15;
	s = sumaNumbers(n=10, value, count);
	cout << "\ns( " << n << " )= " << s << endl;
	cout << "\nMinMax:\n";
	double min, max;
	minMax(n, min, max);
	cout << "min: " << min << " max: " << max << endl;
	system("pause");
}

void minMax(int n, double& min, double& max) {
	double number;
	cout << "Input" << n << " number \n";
	cin >> number;
	min = max = number;
	for (int i = 2; i <= n; i += 1) {
		cin >> number;
		if (number < min)
			min = number;
		if (number > max)
			max = number;
	}

}

int sumaNumbers(int n, int val, int& k) {
	int number;
	int s = 0;
	cout << "Input" << n << " number \n";
	for (; n > 0; --n) {
		//cin >> number;3
		number = rand() % 47;
		cout.width(4);
		cout << number;
		if (number >= val) continue;
		s += number;
			++k;
	}

	return s;
}

int sumaNumbers(int n) {
	int number;
	int s = 0;
	cout << "Input"<<n<<" number \n";
		for (; n > 0; --n){
	//cin >> number;3
			number = rand() % 47;
																																																																																																																																																																																																																																																																																																																													s += number;
}

	return s;
}
int sumaNumbers() {
	int number;
	int s = 0;
	cout << "Input number (0-exit)\n";
	cin >> number;
	while (number != 0) {
		s += number;
		cin >> number;
	}

	return s;
}
