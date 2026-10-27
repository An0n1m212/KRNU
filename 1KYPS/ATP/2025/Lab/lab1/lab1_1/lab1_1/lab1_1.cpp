#include <iostream>	
using namespace std;


int main()
{
	const float a = 1.22, b = 10, c = 4;
	float y;
	y = ((a * sqrt(c * b) + 1) + 0.33) - (c * a) / (a * b * (fabs(a - pow(b, 3))));
	cout << "y= " << y << endl;
	system("pause");
}