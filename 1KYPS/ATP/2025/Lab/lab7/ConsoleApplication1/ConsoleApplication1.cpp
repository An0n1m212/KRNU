#include <iostream>
#include <cstring>
#include <iomanip>
#include <Windows.h>
#include <cstdio>

using namespace std;

const int numapps = 16;
const int numfloors = 4;
const int ownermaxlenth = 30;
const int floorresult = 30;

struct Apartment {
    int floor;
    int rooms;
    float S;
    int residents;
    int child;
    char owner[ownermaxlenth];
};

char maxchildfloorresult[floorresult] = "";

int analyzeresidentsinfloor(const Apartment apartments[], int floor);
int analyzeresidentsonnroom(const Apartment apartments[], int n);
char* analyzemaxchildinfloor(const Apartment apartments[], int floors);
void printApartments(const Apartment apartments[], int numapps);


int main() {
    int floor = 0;
    int n = 0;

    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    Apartment apartments[numapps] = {
        {1, 2, 45.0f, 3, 1, "Петренко А.І."},
        {1, 3, 68.5f, 4, 2, "Іванов С.В."},
        {1, 1, 30.0f, 1, 0, "Коваль О.П."},
        {1, 2, 52.0f, 2, 0, "Мельник Л.Н."},
        {2, 3, 75.2f, 5, 3, "Шевченко В.Д."},
        {2, 2, 48.0f, 2, 1, "Захарчук Р.А."},
        {2, 1, 35.0f, 1, 0, "Ткаченко К.Г."},
        {2, 3, 80.0f, 4, 2, "Бойко М.М."},
        {3, 2, 55.0f, 3, 1, "Савчук Т.І."},
        {3, 4, 95.5f, 6, 3, "Олійник А.В."},
        {3, 1, 28.0f, 1, 0, "Кравчук Є.Р."},
        {3, 2, 50.0f, 3, 1, "Лисенко Б.С."},
        {4, 3, 70.0f, 4, 2, "Мороз В.Л."},
        {4, 2, 60.5f, 3, 1, "Павленко Г.Д."},
        {4, 1, 40.0f, 2, 0, "Кузьменко Ж.Ю."},
        {4, 3, 85.0f, 5, 3, "Соловйов І.К."}
    };

    printApartments(apartments, numapps);

    cout << "Input floor for analyze (1-" << numfloors << "): " << endl;;
    cin >> floor;
    int residentson2flor = analyzeresidentsinfloor(apartments, floor);
    cout << "Total residents on " << floor << " floor: " << residentson2flor << endl;
    system("pause");

    cout << "Input room(s) for analyze residents: " << endl;;
    cin >> n;
    int ninroom = analyzeresidentsonnroom(apartments, n);
    cout << "Total residents in " << n << "-rooms: " << ninroom << endl;
    system("pause");

    char* floormaxchild = analyzemaxchildinfloor(apartments, numfloors);
    cout << "\nFloor(s) with max children: " << floormaxchild << endl;
    system("pause");

    return 0;
}

char* analyzemaxchildinfloor(const Apartment apartments[], int floors) {
    int childreninfloor[numfloors] = { 0 };
    int maxchild = -1;

    maxchildfloorresult[0] = '\0';

    for (int i = 0; i < numapps; ++i) {
        if (apartments[i].floor >= 1 && apartments[i].floor <= floors) {
            childreninfloor[apartments[i].floor - 1] += apartments[i].child;
        }
    }

    char temp_floor_str[5];
    size_t current_len = 0;

    for (int flid = 0; flid < floors; ++flid) {
        int nowchildren = childreninfloor[flid];
        int flnumb = flid + 1;

        if (nowchildren > maxchild) {
            maxchild = nowchildren;

            maxchildfloorresult[0] = '\0';
            current_len = 0;

            snprintf(temp_floor_str, sizeof(temp_floor_str), "%d", flnumb);

            strcpy_s(maxchildfloorresult, floorresult, temp_floor_str);
            current_len = strlen(maxchildfloorresult);
        }
        else if (nowchildren == maxchild) {
            const char* separator = " та ";
            size_t separator_len = strlen(separator);

            if (current_len + separator_len < floorresult) {
                strcpy_s(maxchildfloorresult + current_len, floorresult - current_len, separator);
                current_len += separator_len;
            }

            snprintf(temp_floor_str, sizeof(temp_floor_str), "%d", flnumb);
            size_t num_len = strlen(temp_floor_str);

            if (current_len + num_len < floorresult) {
                strcpy_s(maxchildfloorresult + current_len, floorresult - current_len, temp_floor_str);
                current_len += num_len;
            }
        }
    }
    return maxchildfloorresult;
}

int analyzeresidentsinfloor(const Apartment apartments[], int floor) {
    int residentson2flor = 0;
    for (int i = 0; i < numapps; ++i) {
        if (apartments[i].floor == floor) {
            residentson2flor += apartments[i].residents;
        }
    }
    return residentson2flor;
}

int analyzeresidentsonnroom(const Apartment apartments[], int n) {
    int ninroom = 0;
    for (int i = 0; i < numapps; ++i) {
        if (apartments[i].rooms == n) {
            ninroom += apartments[i].residents;
        }
    }
    return ninroom;
}

void printApartments(const Apartment apartments[], int numapps) {
    cout << "Info about apartments\n";
    cout << left << setw(5) << "№"
        << left << setw(7) << "Floor"
        << left << setw(8) << "Room"
        << left << setw(10) << "S"
        << left << setw(13) << "Residents"
        << left << setw(7) << "Children"
        << left << "Owner" << endl;
    for (int i = 0; i < numapps; ++i) {
        cout << left << setw(5) << i + 1
            << left << setw(7) << apartments[i].floor
            << left << setw(8) << apartments[i].rooms
            << left << setw(10) << apartments[i].S
            << left << setw(13) << apartments[i].residents
            << left << setw(7) << apartments[i].child
            << left << apartments[i].owner << endl;
    }
}