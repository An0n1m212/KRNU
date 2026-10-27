#include <iostream>
#include <fstream>
#include <cstring>


using namespace std;
#pragma warning(disable:4996)

class Product {
public:
    Product() {
        ++count;
    }

    Product(const char* _shifr, const char* _name, unsigned numb) :
        numberInStock(numb) {
        shifr = new char[strlen(_shifr) + 1];
        name = new char[strlen(_name) + 1];
        ++count; 
        if (name == nullptr || shifr == nullptr) return;
        strcpy(shifr, _shifr);
        strcpy(name, _name);
    }

    Product(const Product& B) : numberInStock(B.numberInStock) {
        shifr = new char[strlen(B.shifr) + 1];
        name = new char[strlen(B.name) + 1];
        ++count; 
        if (name) strcpy(name, B.name);
        if (shifr) strcpy(shifr, B.shifr);
    }

    Product& operator=(const Product& B) {
        if (this == &B) 
            return *this;

        if (shifr) delete[] shifr;
        if (name) delete[] name;

        shifr = new char[strlen(B.shifr) + 1];
        name = new char[strlen(B.name) + 1];

        if (name) strcpy(name, B.name);
        if (shifr) strcpy(shifr, B.shifr);
        numberInStock = B.numberInStock;

        return *this;
    }

    inline const char* getShifr() const {
        return shifr; 
    }

    inline const char* getName() const {
        return name; 
    }

    inline unsigned getNumberInStock() const {
        return numberInStock;
    }

    inline void setShifr(const char* newShif) {
        if (shifr) delete[] shifr;
        shifr = new char[strlen(newShif) + 1];
        if (shifr) strcpy(shifr, newShif);
    }
    inline void setName(const char* _name) {
        if (name) delete[] name;
        name = new char[strlen(_name) + 1];
        if (name) strcpy(name, _name);
    }
    inline void setNumberInStock(unsigned numb) {
        numberInStock = numb;
    }

    ~Product() {
        --count;
        if (shifr) delete[] shifr;
        if (name) delete[] name;
    }

    void input();

    void print() const;

    inline char* ToString() const {
        char buf[550];
        snprintf(buf, sizeof(buf), "%s, %s, %u", shifr, name, numberInStock);

        char* retProductStr = new char[strlen(buf) + 1];
        strcpy(retProductStr, buf);
        return retProductStr;
    }

    static int getCount() { return count; }
private:
    char* shifr{ nullptr };
    char* name{ nullptr };
    unsigned numberInStock{ 0 };
    static unsigned count;
};

void Product::input() {
    char buf[255];
    unsigned numb;

    if (shifr) delete[] shifr;
    cout << "\nShifr: ";
    cin.getline(buf, 255);
    shifr = new char[strlen(buf) + 1];
    if (shifr) strcpy(shifr, buf);

    if (name) delete[] name;
    cout << "Name product: ";
    cin.getline(buf, 255);
    name = new char[strlen(buf) + 1];
    if (name) strcpy(name, buf);

    cout << "Number in stock: ";
    if (cin >> numb) {
        numberInStock = numb;
    }
    cin.ignore();
}

void Product::print() const {
    cout << "\nShifr: " << shifr;
    cout << "\nName Product: " << name;
    cout << "\nIn Stock: " << numberInStock;
}

unsigned Product::count = 0;

int main()
{
    Product product{ "111111111", "Iphone", 25 };

    product.print();

    const int N = 5;
    Product* arrProduct[N]{ nullptr };

    int i;
    arrProduct[0] = new Product("12345678", "Iphone 16 Pro", 10);
    arrProduct[1] = new Product("12233344", "Iphone XR", 5);

    for (i = 2; i < N; ++i) {
        cout << "\nEnter product information\n";
        arrProduct[i] = new Product;
        arrProduct[i]->input();
    }

    cout << "\nProduct Information\n";
    cout << "Total products (static count): " << Product::getCount() << "\n";

    for (i = 0; i < N; ++i) {
        arrProduct[i]->print();
    }



    ofstream fout("product.txt");
    if (fout.is_open()) {
        fout << "Array products\n";
        for (i = 0; i < N; ++i) {
            char* buf2 = arrProduct[i]->ToString();
            fout << buf2 << endl;
            delete[] buf2;
        }
    }
    else {
        cerr << "Error open output file\n";
        system("pause");
        return 1;
    }


    cout << "\nList of 'Iphone' products (by Name)\n";
    int found_count = 0;
    for (i = 0; i < N; ++i) {
        const char* name = arrProduct[i]->getName();
        if (name && strstr(name, "Iphone")) {
            arrProduct[i]->print();
            ++found_count;
        }
    }
    if (found_count == 0) cout << "No Iphone products found\n";

    for (i = 0; i < N; ++i) {
        delete arrProduct[i]; 
    }

    cout << "Remaining products after deletion: " << Product::getCount() << endl;

    cout << "Press ENTER to exit...";
    cin.get();
    return 0;
}