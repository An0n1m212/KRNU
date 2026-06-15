using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Zoo
{
    public static class DataLoad
    {
        public static void FromFile(string filePath, ZooManager zoo)
        {
            if (!File.Exists(filePath)) return;

            string currentSection = "";
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (line.StartsWith("SECTION:"))
                {
                    currentSection = line.Trim();
                    continue;
                }

                string[] parts = line.Split('|');
                if (parts.Length < 5) continue;

                var sections = zoo.GetSections();

                if (sections.ContainsKey(currentSection))
                {
                    sections[currentSection].LoadAnimal(
                        parts[0],
                        int.Parse(parts[1]),
                        double.Parse(parts[2]),
                        bool.Parse(parts[3]),
                        double.Parse(parts[4])
                    );
                }
            }
            Console.WriteLine("[Система] Дані завантажено з TXT.");
        }
    }
}
