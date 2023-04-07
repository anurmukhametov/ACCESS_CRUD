using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace CRUD
{
    /// <summary>
    /// Отношение
    /// </summary>
    public struct Relation
    {
        /// <summary>
        /// Ключи отношения
        /// </summary>
        /// <returns>Ключи отношения</returns>
        public List<Attribute> Keys()
        {
            var keys = new List<Attribute>();
            foreach (var attribute in Attributes)
            {
                if (attribute.IsPrimaryKey)
                {
                    keys.Add(attribute);
                }
            }
            return keys;
        }

        /// <summary>
        /// Инициализация отношения
        /// </summary>
        /// <param name="name">Название отношения</param>
        /// <param name="attributes">Атрибуты отношения</param>
        public Relation(string name, List<Attribute> attributes)
        {
            Name = name;
            Attributes = attributes;
        }

        /// <summary>
        /// Инициализация отношения
        /// </summary>
        /// <param name="name">Название отношение</param>
        /// <param name="attributeNames">Названия атрибутов</param>
        /// <param name="numberOfKeys">Колчиество первичных ключей</param>
        public Relation(string name, string[] attributeNames, int numberOfKeys)
        {
            Name = name;
            Attributes = new List<Attribute>();
            foreach (var attributeName in attributeNames)
            {
                Attributes.Add(new Attribute(attributeName, numberOfKeys > 0));
                numberOfKeys--;
            }
        }

        /// <summary>
        /// Название отношения
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Атрибуты отношения
        /// </summary>
        public List<Attribute> Attributes { get; private set; }
    }

    /// <summary>
    /// Атрибут
    /// </summary>
    public struct Attribute
    {
        /// <summary>
        /// Инициализация атриубта
        /// </summary>
        /// <param name="attributeName">Название атриубта</param>
        /// <param name="attributeIsPrimaryKey">Является первичным ключом</param>
        /// <param name="attributeValue">Значение атрибута</param>
        public Attribute(string attributeName, bool attributeIsPrimaryKey, string attributeValue = null)
        {
            Name = attributeName;
            IsPrimaryKey = attributeIsPrimaryKey;
            Value = attributeValue;
        }

        /// <summary>
        /// Название атрибута
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Является первичным ключом
        /// </summary>
        public bool IsPrimaryKey { get; private set; }

        /// <summary>
        /// Значение атрибута
        /// </summary>
        public string Value { get; private set; }
    }

    /// <summary>
    /// Класс создания SQL-запросов
    /// </summary>
    internal static class Query
    {
        /// <summary>
        /// Создание SELECT-запроса
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <param name="condition">Атрибуты условия</param>
        /// <param name="attributes">Атрибуты поиска</param>
        /// <returns>SELECT-запрос</returns>
        public static string Select(Relation relation, List<Attribute> condition, List<Attribute> attributes)
        {
            var query = "SELECT ";

            if (attributes != null && attributes.Count > 0)
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    query += i != 0 ? ", " : "";
                    query += $"[{attributes[i].Name}]";
                }
            }
            else
            {
                for (int i = 0; i < relation.Attributes.Count; i++)
                {
                    query += i != 0 ? ", " : "";
                    query += $"[{relation.Attributes[i].Name}]";
                }
            }

            query += $" FROM [{relation.Name}]";

            if (condition != null && condition.Count > 0)
            {
                for (int i = 0; i < condition.Count; i++)
                {
                    query += i != 0 ? " AND " : " WHERE (";
                    query += $"([{condition[i].Name}] = \"{condition[i].Value}\")";
                }
                query += ")";
            }

            return query;
        }

        /// <summary>
        /// Создание INSERT-запроса
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <param name="attributes">Атрибуты значения</param>
        /// <returns>INSERT-запрос</returns>
        public static string Insert(Relation relation, List<Attribute> attributes)
        {
            if (attributes != null && attributes.Count > 0)
            {
                var query = $"INSERT INTO [{relation.Name}] (";

                for (int i = 0; i < attributes.Count; i++)
                {
                    query += i != 0 ? ", " : "";
                    query += $"[{attributes[i].Name}]";
                }

                query += $") VALUES(";

                for (int i = 0; i < attributes.Count; i++)
                {
                    query += i != 0 ? ", " : "";
                    query += $"\"{attributes[i].Value}\"";
                }
                query += ")";

                return query;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Создание UPDATE-запроса
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <param name="condition">Атрибуты условия</param>
        /// <param name="attributes">Атрибуты значения</param>
        /// <returns>UPDATE-запрос</returns>
        public static string Update(Relation relation, List<Attribute> condition, List<Attribute> attributes)
        {
            if (attributes == null && attributes.Count <= 0)
            {
                return null;
            }

            if (condition == null && condition.Count <= 0)
            {
                return null;
            }

            var query = $"UPDATE [{relation.Name}] SET ";

            for (int i = 0; i < attributes.Count; i++)
            {
                query += i != 0 ? ", " : "";
                query += $"[{attributes[i].Name}] = \"{attributes[i].Value}\"";
            }

            for (int i = 0; i < condition.Count; i++)
            {
                query += i != 0 ? " AND " : " WHERE (";
                query += $"([{condition[i].Name}] = \"{condition[i].Value}\")";
            }
            query += ")";

            return query;
        }

        /// <summary>
        /// Создание DELETE-запроса
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <param name="condition">Атрибуты условия</param>
        /// <returns>DELETE-запрос</returns>
        public static string Delete(Relation relation, List<Attribute> condition)
        {
            if (condition != null && condition.Count > 0)
            {
                var query = $"DELETE FROM [{relation.Name}]";

                for (int i = 0; i < condition.Count; i++)
                {
                    query += i != 0 ? " AND " : " WHERE (";
                    query += $"([{condition[i].Name}] = \"{condition[i].Value}\")";
                }
                query += ")";

                return query;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Мои отношения
    /// </summary>
    internal static class MyRelation
    {
        #region СЛОВАРЬ
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _dictionaryAttributes = new List<Attribute>()
        {
            new Attribute("Код", true),
            new Attribute("Дополнительный код", false),
            new Attribute("Тип", false),
            new Attribute("Наименование", false),
            new Attribute("Описание", false)
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _dictionary = new Relation("Словарь", _dictionaryAttributes);

        /// <summary>
        /// Словарь
        /// </summary>
        public static Relation Dictionary
        {
            get { return _dictionary; }
        }
        #endregion

        #region ТЕЗАУРУС
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _thesaurusAttributes = new List<Attribute>()
        {
            new Attribute("Код основного понятия", true),
            new Attribute("Код подчиненного понятия", true)
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _thesaurus = new Relation("Тезаурус", _thesaurusAttributes);

        /// <summary>
        /// Тезаурус
        /// </summary>
        public static Relation Thesaurus
        {
            get { return _thesaurus; }
        }
        #endregion

        #region ПАЦИЕНТ
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _patientAttributes = new List<Attribute>()
        {
            new Attribute("Номер полиса", true),
            new Attribute("Номер истории беременности", false),
            new Attribute("Фамилия", false),
            new Attribute("Имя", false),
            new Attribute("Отчество", false),
            new Attribute("Дата рождения", false)
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _patient = new Relation("Пациент", _patientAttributes);

        /// <summary>
        /// Пациент
        /// </summary>
        public static Relation Patient
        {
            get { return _patient; }
        }
        #endregion

        #region ТЕКУЩИЙ КОД ПОНЯТИЯ
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _currentCodeAttributes = new List<Attribute>()
        {
            new Attribute("Текущий код понятия", true)
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _currentCode = new Relation("Текущий код понятия", _currentCodeAttributes);

        /// <summary>
        /// Текущий код понятия
        /// </summary>
        public static Relation CurrentCode
        {
            get { return _currentCode; }
        }
        #endregion

        #region ТИПОВАЯ СИТУАЦИЯ
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _typicalSituationAttributes = new List<Attribute>()
        {
            new Attribute("Класс типовой ситуации", true),
            new Attribute("Код лингвистического значения признака", true)
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _typicalSituation = new Relation("Типовая ситуация", _typicalSituationAttributes);

        /// <summary>
        /// Типовая ситуация
        /// </summary>
        public static Relation TypicalSituation
        {
            get { return _typicalSituation; }
        }
        #endregion

        #region РЕЗУЛЬТАТЫ ОБСЛЕДОВАНИЯ
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _surveyResultsAttributes = new List<Attribute>()
        {
            new Attribute("Номер полиса", true),
            new Attribute("Дата определения значения признака", true),
            new Attribute("Код лингвистического значения признака", true),
            new Attribute("Количественное значение признака", false),
            new Attribute("Степень соответствия", false),
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _surveyResults = new Relation("Результаты обследования", _surveyResultsAttributes);

        /// <summary>
        /// Результаты обследования
        /// </summary>
        public static Relation SurveyResults
        {
            get { return _surveyResults; }
        }
        #endregion

        #region ЗНАЧЕНИЕ ПРИЗНАКА
        /// <summary>
        /// Значение признака
        /// </summary>
        public static Relation SymptomValue
        {
            get { return _symptomValue; }
        }
        private static Relation _symptomValue = new Relation("Значение признака", _symptomValueAttributes);
        private static List<Attribute> _symptomValueAttributes = new List<Attribute>()
        {
            new Attribute("Код признака", true),
            new Attribute("Минимальное значение признака", false),
            new Attribute("Максимальное значение признака", false),
            new Attribute("Единицы измерения", false),
        };
        #endregion

        #region ПРАВИЛО
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _ruleAttributes = new List<Attribute>()
        {
            new Attribute("Код группы риска", true),
            new Attribute("Класс типовой ситуации", true),
            new Attribute("Минимальная степень отнесения", false),
            new Attribute("Максимальная степень отнесения", false),
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _rule = new Relation("Правило", _ruleAttributes);

        /// <summary>
        /// Правило
        /// </summary>
        public static Relation Rule
        {
            get { return _rule; }
        }
        #endregion

        #region ФУНКЦИЯ ПРИНАДЛЕЖНОСТИ
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _membershipFunctionAttributes = new List<Attribute>()
        {
            new Attribute("Код лингвистического значения признака", true),
            new Attribute("Тип функции принадлежности (T, Z, S)", false),
            new Attribute("Точка \"A\"", false),
            new Attribute("Точка \"B\"", false),
            new Attribute("Точка \"C\"", false),
            new Attribute("Точка \"D\"", false)
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _membershipFunction = new Relation("Функция принадлежности", _membershipFunctionAttributes);

        /// <summary>
        /// Функция принадлежности
        /// </summary>
        public static Relation MembershipFunction
        {
            get { return _membershipFunction; }
        }
        #endregion

        #region ИНДИВИДУАЛЬНЫЙ ПЛАН ВЕДЕНИЯ
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _individualPlanAttributes = new List<Attribute>()
        {
            new Attribute("Номер полиса", true),
            new Attribute("Неделя беременности", true),
            new Attribute("Код процедуры", true),
            new Attribute("Продолжительность интервала", false),
            new Attribute("Количество процедур в интервале", false),
            new Attribute("Частота выполнения процедуры в неделю", false),
            new Attribute("Условие выполнения, комментарии", false),
            new Attribute("Исполнитель", false),
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _individualPlan = new Relation("Индивидуальный план ведения", _individualPlanAttributes);

        /// <summary>
        /// Индивидуальный план ведения
        /// </summary>
        public static Relation IndividualPlan
        {
            get { return _individualPlan; }
        }
        #endregion

        #region ТИПОВОЙ ПЛАН ВЕДЕНИЯ
        /// <summary>
        /// Атрибуты
        /// </summary>
        private static List<Attribute> _standardPlanAttributes = new List<Attribute>()
        {
            new Attribute("Код группы риска", true),
            new Attribute("Неделя беременности", true),
            new Attribute("Код процедуры", true),
            new Attribute("Продолжительность интервала", false),
            new Attribute("Количество процедур в интервале", false),
            new Attribute("Частота выполнения процедуры в неделю", false),
            new Attribute("Условие выполнения, комментарии", false),
            new Attribute("Исполнитель", false),
        };

        /// <summary>
        /// Отношение
        /// </summary>
        private static Relation _standardPlan = new Relation("Типовой план ведения", _standardPlanAttributes);

        /// <summary>
        /// Типовой план ведения
        /// </summary>
        public static Relation StandardPlan
        {
            get { return _standardPlan; }
        }
        #endregion
    }

    /// <summary>
    /// База данных
    /// </summary>
    internal class Database
    {
        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Инициализация базы данных
        /// </summary>
        /// <param name="databasePath">Пусть к файлу базы данных в формате accdb</param>
        public Database(string databasePath)
        {
            connectionString = $"provider=Microsoft.ACE.OLEDB.12.0;data source={databasePath}";
        }

        /// <summary>
        /// Чтение всего отношения
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <returns>Список записей</returns>
        public List<List<Attribute>> Read(Relation relation)
        {
            return Read(relation, null);
        }

        /// <summary>
        /// Чтение всех атрибутов с заданным условием
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <param name="condition">Атрибуты условия</param>
        /// <returns>Список записей отношения</returns>
        public List<List<Attribute>> Read(Relation relation, List<Attribute> condition)
        {
            return Read(relation, condition, relation.Attributes);
        }

        /// <summary>
        /// Чтение определенных атрибутов с заданным условием
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <param name="condition">Атрибуты условия</param>
        /// <param name="attributes">Атрибуты поиска</param>
        /// <returns>Список записей отношения</returns>
        public List<List<Attribute>> Read(Relation relation, List<Attribute> condition, List<Attribute> attributes)
        {
            var result = new List<List<Attribute>>();
            var query = Query.Select(relation, condition, attributes);
            using (var connection = new OleDbConnection(connectionString))
            {
                var command = new OleDbCommand(query, connection);
                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var record = new List<Attribute>();
                        foreach (var attribute in attributes)
                        {
                            record.Add(new Attribute(attribute.Name, attribute.IsPrimaryKey, reader[attribute.Name].ToString()));
                        }
                        result.Add(record);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    return null;
                }
            }
            return result;
        }

        /// <summary>
        /// Добавление новой записи отношения
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <param name="attributes">Атрибуты значений</param>
        /// <returns>Количество затронутых строк</returns>
        public int Write(Relation relation, List<Attribute> attributes)
        {
            int rowsAffected = 0;
            if (Read(relation, attributes).Count == 0)
            {
                var query = Query.Insert(relation, attributes);
                if (query != null)
                {
                    using (var connection = new OleDbConnection(connectionString))
                    {
                        var command = new OleDbCommand(query, connection);
                        try
                        {
                            connection.Open();
                            rowsAffected = command.ExecuteNonQuery();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }
                    }
                }
            }
            return rowsAffected;
        }

        /// <summary>
        /// Изменение записей отношения
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <param name="condition">Атрибуты условия</param>
        /// <param name="attributes">Атрибуты значений</param>
        /// <returns>Количество затронутых строк</returns>
        public int Rewrite(Relation relation, List<Attribute> condition, List<Attribute> attributes)
        {
            int rowsAffected = 0;
            var query = Query.Update(relation, condition, attributes);
            if (query != null)
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    var command = new OleDbCommand(query, connection);
                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
            }
            return rowsAffected;
        }

        /// <summary>
        /// Удаление записей отношения
        /// </summary>
        /// <param name="relation">Отношение</param>
        /// <param name="condition">Атрибуты условия</param>
        /// <returns>Количество затронутых строк</returns>
        public int Delete(Relation relation, List<Attribute> condition)
        {
            int rowsAffected = 0;
            var query = Query.Delete(relation, condition);
            if (query != null)
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    var command = new OleDbCommand(query, connection);
                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
            }
            return rowsAffected;
        }
    }
}
