using System.Data.SqlClient;

namespace Lab7_1
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Добавляем тестовые данные, если таблица пуста
                    string checkDataQuery = "SELECT COUNT(*) FROM Services";
                    using (var command = new SqlCommand(checkDataQuery, connection))
                    {
                        int count = (int)command.ExecuteScalar();
                        if (count == 0)
                        {
                            InsertSampleData(connection);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при инициализации базы данных: {ex.Message}", ex);
            }
        }

        

        public List<Service> GetAllServices()
        {
            var services = new List<Service>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM Services ORDER BY Id", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(MapReaderToService(reader));
                    }
                }
            }

            return services;
        }

        public List<Service> SearchServices(string searchTerm, string category)
        {
            var services = new List<Service>();
            string query = "SELECT * FROM Services WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += " AND (Name LIKE @search OR Category LIKE @search OR Provider LIKE @search OR Description LIKE @search)";
                parameters.Add(new SqlParameter("@search", $"%{searchTerm}%"));
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "Все категории")
            {
                query += " AND Category = @category";
                parameters.Add(new SqlParameter("@category", category));
            }

            query += " ORDER BY Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters.ToArray());
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(MapReaderToService(reader));
                    }
                }
            }

            return services;
        }

        public List<string> GetAllCategories()
        {
            var categories = new List<string>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT DISTINCT Category FROM Services ORDER BY Category", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(reader["Category"].ToString());
                    }
                }
            }

            return categories;
        }

        public Service GetServiceById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM Services WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToService(reader);
                    }
                }
            }

            return null;
        }

        public bool AddService(Service service)
        {
            string query = @"
                INSERT INTO Services (Name, Description, Category, Price, Provider, Address, Phone, RegistrationDate, IsActive)
                VALUES (@name, @description, @category, @price, @provider, @address, @phone, @registrationDate, @isActive)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                AddServiceParameters(command, service);

                connection.Open();
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }

        public bool UpdateService(Service service)
        {
            string query = @"
                UPDATE Services 
                SET Name = @name, 
                    Description = @description, 
                    Category = @category, 
                    Price = @price, 
                    Provider = @provider, 
                    Address = @address, 
                    Phone = @phone, 
                    RegistrationDate = @registrationDate, 
                    IsActive = @isActive
                WHERE Id = @id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", service.Id);
                AddServiceParameters(command, service);

                connection.Open();
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }

        public bool DeleteService(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("DELETE FROM Services WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }

        private void AddServiceParameters(SqlCommand command, Service service)
        {
            command.Parameters.AddWithValue("@name", service.Name);
            command.Parameters.AddWithValue("@description",
                string.IsNullOrWhiteSpace(service.Description) ? (object)DBNull.Value : service.Description);
            command.Parameters.AddWithValue("@category", service.Category);
            command.Parameters.AddWithValue("@price", service.Price);
            command.Parameters.AddWithValue("@provider", service.Provider);
            command.Parameters.AddWithValue("@address",
                string.IsNullOrWhiteSpace(service.Address) ? (object)DBNull.Value : service.Address);
            command.Parameters.AddWithValue("@phone", service.Phone);
            command.Parameters.AddWithValue("@registrationDate", service.RegistrationDate);
            command.Parameters.AddWithValue("@isActive", service.IsActive);
        }

        private Service MapReaderToService(SqlDataReader reader)
        {
            return new Service
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString(),
                Description = reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString(),
                Category = reader["Category"].ToString(),
                Price = Convert.ToDecimal(reader["Price"]),
                Provider = reader["Provider"].ToString(),
                Address = reader["Address"] == DBNull.Value ? string.Empty : reader["Address"].ToString(),
                Phone = reader["Phone"].ToString(),
                RegistrationDate = Convert.ToDateTime(reader["RegistrationDate"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }

        private void InsertSampleData(SqlConnection connection)
        {
            string insertQuery = @"
        INSERT INTO Services (Name, Description, Category, Price, Provider, Address, Phone, RegistrationDate, IsActive)
        VALUES 
        ('Ремонт холодильников', 'Качественный ремонт холодильников всех марок', 'Ремонт бытовой техники', 1500.00, 'ООО ''ХолодСервис''', 'ул. Ленина, 10', '+7 (495) 123-45-67', @date1, 1),
        ('Химчистка мебели', 'Профессиональная химчистка диванов, кресел, матрасов', 'Уборка и чистка', 3000.00, 'Клининговая компания ''Чистота''', 'пр. Мира, 25', '+7 (495) 234-56-78', @date2, 1),
        ('Установка кондиционеров', 'Установка и обслуживание кондиционеров', 'Установка техники', 5000.00, 'ИП Петров А.С.', 'ул. Гагарина, 15', '+7 (495) 345-67-89', @date3, 1),
        ('Ремонт стиральных машин', 'Ремонт всех марок стиральных машин, замена деталей', 'Ремонт бытовой техники', 2000.00, 'ООО ''МастерТехник''', 'ул. Пушкина, 5', '+7 (495) 456-78-90', @date4, 1),
        ('Услуги сантехника', 'Установка, ремонт и замена сантехнического оборудования', 'Ремонт квартир', 2500.00, 'ИП Сидоров В.П.', 'ул. Чехова, 12', '+7 (495) 567-89-01', @date5, 1),
        ('Электрик на дом', 'Установка розеток, выключателей, монтаж электропроводки', 'Ремонт квартир', 1800.00, 'ООО ''ЭлектроСервис''', 'пр. Победы, 30', '+7 (495) 678-90-12', @date6, 1),
        ('Ремонт компьютеров', 'Настройка, ремонт, установка программного обеспечения', 'Ремонт цифровой техники', 1200.00, 'Компьютерный центр ''ТехноСфера''', 'ул. Мира, 8', '+7 (495) 789-01-23', @date7, 1),
        ('Парикмахерские услуги на дому', 'Стрижка, укладка, окрашивание волос', 'Красота и здоровье', 800.00, 'Салон ''Красота''', 'ул. Садовая, 3', '+7 (495) 890-12-34', @date8, 1),
        ('Установка дверей', 'Установка межкомнатных и входных дверей', 'Ремонт квартир', 4000.00, 'ООО ''Дверной мир''', 'ул. Лесная, 7', '+7 (495) 901-23-45', @date9, 1),
        ('Ремонт телевизоров', 'Ремонт LED, LCD, плазменных телевизоров', 'Ремонт цифровой техники', 2200.00, 'Телевизионный сервис ''ТелеМастер''', 'ул. Центральная, 15', '+7 (495) 012-34-56', @date10, 1),
        ('Генеральная уборка квартиры', 'Комплексная уборка квартир после ремонта', 'Уборка и чистка', 3500.00, 'Клининговая служба ''Чистый дом''', 'ул. Зеленая, 20', '+7 (495) 123-45-67', @date11, 1),
        ('Ремонт посудомоечных машин', 'Диагностика и ремонт посудомоечных машин', 'Ремонт бытовой техники', 1700.00, 'Сервисный центр ''БытТехника''', 'ул. Весенняя, 9', '+7 (495) 234-56-78', @date12, 1),
        ('Установка газовых плит', 'Подключение и наладка газовых плит', 'Установка техники', 2800.00, 'Газовый сервис ''ГазМастер''', 'ул. Трудовая, 11', '+7 (495) 345-67-89', @date13, 1),
        ('Маникюр и педикюр на дому', 'Профессиональные услуги маникюра и педикюра', 'Красота и здоровье', 1500.00, 'Студия ногтевого сервиса ''Ноготки''', 'ул. Цветочная, 6', '+7 (495) 456-78-90', @date14, 1),
        ('Ремонт микроволновых печей', 'Ремонт СВЧ-печей всех производителей', 'Ремонт бытовой техники', 1300.00, 'Сервисный центр ''Микроволновка''', 'ул. Северная, 14', '+7 (495) 567-89-01', @date15, 1),
        ('Монтаж натяжных потолков', 'Установка натяжных потолков любой сложности', 'Ремонт квартир', 6000.00, 'ООО ''ПотолокПрофи''', 'ул. Южная, 18', '+7 (495) 678-90-12', @date16, 1),
        ('Ремонт планшетов', 'Ремонт планшетов, замена экранов, ремонт плат', 'Ремонт цифровой техники', 2500.00, 'Сервисный центр ''ПланшетМастер''', 'ул. Западная, 13', '+7 (495) 789-01-23', @date17, 1),
        ('Химчистка ковров', 'Профессиональная химчистка ковров и ковровых покрытий', 'Уборка и чистка', 2000.00, 'Клининг ''КоверЧист''', 'ул. Восточная, 16', '+7 (495) 890-12-34', @date18, 1),
        ('Установка водонагревателей', 'Монтаж и подключение бойлеров', 'Установка техники', 3200.00, 'ООО ''ТеплоВода''', 'ул. Речная, 22', '+7 (495) 901-23-45', @date19, 1),
        ('Массаж на дому', 'Лечебный и расслабляющий массаж', 'Красота и здоровье', 2000.00, 'Медицинский центр ''Здоровье''', 'ул. Горная, 25', '+7 (495) 012-34-56', @date20, 1),
        ('Ремонт кофемашин', 'Ремонт кофемашин и кофеварок', 'Ремонт бытовой техники', 2400.00, 'Сервис ''КофеМастер''', 'ул. Кофейная, 4', '+7 (495) 123-45-67', @date21, 1),
        ('Укладка ламината', 'Укладка ламината, паркетной доски', 'Ремонт квартир', 4500.00, 'ООО ''ПолПрофи''', 'ул. Паркетная, 19', '+7 (495) 234-56-78', @date22, 1),
        ('Ремонт смартфонов', 'Замена дисплеев, ремонт кнопок, замена аккумуляторов', 'Ремонт цифровой техники', 1800.00, 'Сервисный центр ''СмартфонПрофи''', 'ул. Смартфонная, 10', '+7 (495) 345-67-89', @date23, 1),
        ('Мойка окон', 'Мойка окон, витрин, стеклянных поверхностей', 'Уборка и чистка', 1200.00, 'Клининг ''Чистые окна''', 'ул. Стеклянная, 5', '+7 (495) 456-78-90', @date24, 1),
        ('Установка вытяжек', 'Монтаж кухонных вытяжек', 'Установка техники', 2100.00, 'ООО ''Кухонный сервис''', 'ул. Кухонная, 8', '+7 (495) 567-89-01', @date25, 1),
        ('Услуги няни', 'Присмотр за детьми на дому', 'Домашний персонал', 500.00, 'Агентство ''Семейный очаг''', 'ул. Детская, 12', '+7 (495) 678-90-12', @date26, 1),
        ('Ремонт пылесосов', 'Ремонт пылесосов всех типов', 'Ремонт бытовой техники', 1100.00, 'Сервисный центр ''ПылесосМастер''', 'ул. Чистая, 3', '+7 (495) 789-01-23', @date27, 1),
        ('Покраска стен', 'Покраска стен, потолков, подготовка поверхностей', 'Ремонт квартир', 3800.00, 'ООО ''Малярные работы''', 'ул. Краскина, 7', '+7 (495) 890-12-34', @date28, 1),
        ('Ремонт фотоаппаратов', 'Ремонт цифровых фотоаппаратов и объективов', 'Ремонт цифровой техники', 2900.00, 'Фотосервис ''Объектив''', 'ул. Фотонная, 9', '+7 (495) 901-23-45', @date29, 1),
        ('Уборка после праздника', 'Уборка квартиры после мероприятий и праздников', 'Уборка и чистка', 2800.00, 'Клининг ''Праздничная уборка''', 'ул. Праздничная, 11', '+7 (495) 012-34-56', @date30, 1)";

            using (var command = new SqlCommand(insertQuery, connection))
            {
                
                command.Parameters.AddWithValue("@date1", DateTime.Now.AddDays(-30));
                command.Parameters.AddWithValue("@date2", DateTime.Now.AddDays(-29));
                command.Parameters.AddWithValue("@date3", DateTime.Now.AddDays(-28));
                command.Parameters.AddWithValue("@date4", DateTime.Now.AddDays(-27));
                command.Parameters.AddWithValue("@date5", DateTime.Now.AddDays(-26));
                command.Parameters.AddWithValue("@date6", DateTime.Now.AddDays(-25));
                command.Parameters.AddWithValue("@date7", DateTime.Now.AddDays(-24));
                command.Parameters.AddWithValue("@date8", DateTime.Now.AddDays(-23));
                command.Parameters.AddWithValue("@date9", DateTime.Now.AddDays(-22));
                command.Parameters.AddWithValue("@date10", DateTime.Now.AddDays(-21));
                command.Parameters.AddWithValue("@date11", DateTime.Now.AddDays(-20));
                command.Parameters.AddWithValue("@date12", DateTime.Now.AddDays(-19));
                command.Parameters.AddWithValue("@date13", DateTime.Now.AddDays(-18));
                command.Parameters.AddWithValue("@date14", DateTime.Now.AddDays(-17));
                command.Parameters.AddWithValue("@date15", DateTime.Now.AddDays(-16));
                command.Parameters.AddWithValue("@date16", DateTime.Now.AddDays(-15));
                command.Parameters.AddWithValue("@date17", DateTime.Now.AddDays(-14));
                command.Parameters.AddWithValue("@date18", DateTime.Now.AddDays(-13));
                command.Parameters.AddWithValue("@date19", DateTime.Now.AddDays(-12));
                command.Parameters.AddWithValue("@date20", DateTime.Now.AddDays(-11));
                command.Parameters.AddWithValue("@date21", DateTime.Now.AddDays(-10));
                command.Parameters.AddWithValue("@date22", DateTime.Now.AddDays(-9));
                command.Parameters.AddWithValue("@date23", DateTime.Now.AddDays(-8));
                command.Parameters.AddWithValue("@date24", DateTime.Now.AddDays(-7));
                command.Parameters.AddWithValue("@date25", DateTime.Now.AddDays(-6));
                command.Parameters.AddWithValue("@date26", DateTime.Now.AddDays(-5));
                command.Parameters.AddWithValue("@date27", DateTime.Now.AddDays(-4));
                command.Parameters.AddWithValue("@date28", DateTime.Now.AddDays(-3));
                command.Parameters.AddWithValue("@date29", DateTime.Now.AddDays(-2));
                command.Parameters.AddWithValue("@date30", DateTime.Now.AddDays(-1));

                command.ExecuteNonQuery();
            }
        }
    }
}
