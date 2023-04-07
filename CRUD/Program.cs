using System;
using System.Collections.Generic;

namespace CRUD
{
    internal class Program
    {
        private const string DatabasePath = @"C:\Arslan\Study\4-course\8-semester\Final-qualifying-work\Database\Женская консультация.accdb";
        static void Main(string[] args)
        {
            ExampleRead();
            Console.WriteLine();

            ExampleWrite();
            Console.WriteLine();

            ExampleRewrite();
            Console.WriteLine();

            ExampleDelete();
            Console.ReadLine();
        }

        /// <summary>
        /// Печать результатов запроса
        /// </summary>
        /// <param name="result"></param>
        static void PrintResult(List<List<Attribute>> result)
        {
            foreach (var record in result) // Перебор записей
            {
                foreach (var attribute in record) // Перебор атрибутов
                {
                    Console.Write($"{attribute.Value} ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Пример чтения
        /// </summary>
        static void ExampleRead()
        {
            Console.WriteLine("========================== ПРИМЕР ЧТЕНИЯ ==========================");

            // Получение структуры отношения
            var dictionary = MyRelation.Dictionary;

            // Объявление атрибутов поиска
            var attributes = new List<Attribute>();
            attributes.Add(new Attribute("Код", true));
            attributes.Add(new Attribute("Тип", false));
            attributes.Add(new Attribute("Наименование", false));

            // Объявление атрибутов условия
            var condition = new List<Attribute>();
            condition.Add(new Attribute("Тип", true, "G"));

            // Печать запроса
            Console.WriteLine(Query.Select(dictionary, condition, attributes));

            // Получение объекта базы данных
            var db = new Database(DatabasePath);

            // Получение результатов запроса
            var result = db.Read(dictionary, condition, attributes);

            // Печать результатов запроса
            PrintResult(result);

            Console.WriteLine("===================================================================");
        }

        /// <summary>
        /// Пример добавления
        /// </summary>
        static void ExampleWrite()
        {
            Console.WriteLine("======================== ПРИМЕР ДОБАВЛЕНИЯ ========================");
            
            // Получение структуры отношения
            var dictionary = MyRelation.Dictionary;

            // Объявление атрибутов значений
            var attributes = new List<Attribute>();
            attributes.Add(new Attribute("Код", true, "000"));
            attributes.Add(new Attribute("Дополнительный код", false, "000"));
            attributes.Add(new Attribute("Тип", false, "0"));
            attributes.Add(new Attribute("Наименование", false, "000"));
            attributes.Add(new Attribute("Описание", false, "000"));

            // Печать запроса
            Console.WriteLine(Query.Insert(dictionary, attributes));

            // Получение объекта базы данных
            var db = new Database(DatabasePath);

            // Выполнение запроса
            var rowsAffected = db.Write(dictionary, attributes);

            // Количество добавленных записей
            Console.WriteLine($"Количество затронутых строк: {rowsAffected}");

            Console.WriteLine("===================================================================");
        }

        /// <summary>
        /// Пример изменения
        /// </summary>
        static void ExampleRewrite()
        {
            Console.WriteLine("======================== ПРИМЕР ИЗМЕНЕНИЯ =========================");

            // Получение структуры отношения
            var dictionary = MyRelation.Dictionary;

            // Объявление атрибутов значений
            var attributes = new List<Attribute>();
            attributes.Add(new Attribute("Дополнительный код", false, "111"));
            attributes.Add(new Attribute("Тип", false, "1"));
            attributes.Add(new Attribute("Наименование", false, "111"));
            attributes.Add(new Attribute("Описание", false, "111"));

            // Объявление атриубутов условия
            var condition = new List<Attribute>();
            condition.Add(new Attribute("Код", true, "000"));

            // Печать запроса
            Console.WriteLine(Query.Update(dictionary, condition, attributes));

            // Получение объекта базы данных
            var db = new Database(DatabasePath);

            // Получение результатов запроса
            var rowsAffected = db.Rewrite(dictionary, condition, attributes);

            // Печать результатов запроса
            Console.WriteLine($"Количество затронутых строк: {rowsAffected}");

            Console.WriteLine("===================================================================");
        }

        /// <summary>
        /// Пример удаления
        /// </summary>
        static void ExampleDelete()
        {
            Console.WriteLine("========================= ПРИМЕР УДАЛЕНИЯ =========================");

            // Получение структуры отношения
            var dictionary = MyRelation.Dictionary;

            // Объявление атриубутов условия
            var condition = new List<Attribute>();
            condition.Add(new Attribute("Код", true, "000"));

            // Печать запроса
            Console.WriteLine(Query.Delete(dictionary, condition));

            // Получение объекта базы данных
            var db = new Database(DatabasePath);

            // Получение результатов запроса
            var rowsAffected = db.Delete(dictionary, condition);

            // Печать результатов запроса
            Console.WriteLine($"Количество затронутых строк: {rowsAffected}");

            Console.WriteLine("===================================================================");
        }
    }
}
