using LaboratoryWork2_Oleshko.Models;
using LaboratoryWork2_Oleshko.Repository.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace LaboratoryWork2_Oleshko.Repository.Implementation
{
    public class Repository<T> : IRepository<T> where T : EnvironmentObject
    {
        private static Repository<T> instance;
        public string FilePath { get; private set; }
        private List<T> values;

        private Repository(string filePath)
        {
            FilePath = filePath;
            values = GetAllItemsFromFile();
        }

        public static Repository<T> GetRepo(string filePath)
        {
            if (instance == null)
            {
                instance = new Repository<T>(filePath);
            }

            return instance;
        }

        private List<T> GetAllItemsFromFile()
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<T>>(json);
            }
            catch(FileNotFoundException)
            {
                return new List<T>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading json: {ex.Message}");
                return new List<T>();
            }
        }

        private void SaveItemsToFile(List<T> values)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string updatedJson = JsonSerializer.Serialize(values, options);
                File.WriteAllText(FilePath, updatedJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading json: {ex.Message}");
            }
        }

        public void Create(T value)
        {
            values.Add(value);
            SaveItemsToFile(values);
        }

        public void Delete(T value)
        {
            values.RemoveAll(v => v.Id == value.Id);
            SaveItemsToFile(values);
        }

        public List<T> GetAll(Func<T, bool> filter = null)
        {
            return filter != null ? values.Where(filter).ToList() : values;
        }

        public void Update(T value)
        {
            var existingItemIndex = values.FindIndex(v => v.Id == value.Id);
            if (existingItemIndex != -1)
            {
                values[existingItemIndex] = value;
                SaveItemsToFile(values);
            }
        }

        public T GetByFilter(Func<T, bool> filter)
        {
            return values.FirstOrDefault(filter);
        }
    }
}
