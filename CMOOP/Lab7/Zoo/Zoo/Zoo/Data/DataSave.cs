using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Zoo
{
    public static class DataSave
    {
        public static void ToFile(string filePath, ZooManager zoo)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var entry in zoo.GetModules())
                {
                    writer.WriteLine($"SECTION:{entry.Key}"); 
                    foreach (var animal in entry.Value.GetAllAnimals())
                    {
                        writer.WriteLine($"{animal.Name}|{animal.Age}|{animal.Size}|{animal.IsSocial}|{animal.Food}");
                    }
                }
            }
            Console.WriteLine("[Система] Дані збережено.");
        }
    }
}

