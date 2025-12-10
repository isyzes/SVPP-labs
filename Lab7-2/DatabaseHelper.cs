using System.Data;
using System.Data.SqlClient;

namespace Lab7_2
{
    public class DatabaseHelper
    {
        private static SqlConnection connection;
        private static SqlDataAdapter adapter;
        private static DataTable servicesTable = new DataTable();
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            if (connection == null)
            {
                connection = new SqlConnection(_connectionString);
            }

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public List<Service> GetAllServices()
        {
            var services = new List<Service>();

            if (adapter == null)
            {
                adapter = new SqlDataAdapter("SELECT * FROM Services ORDER BY Id", connection);
            }

            servicesTable.Clear();
            adapter.Fill(servicesTable);

            foreach (DataRow row in servicesTable.Rows)
            {
                services.Add(MapDataRowToService(row));
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

            adapter = new SqlDataAdapter(query, connection);
            adapter.SelectCommand.Parameters.Clear();
            adapter.SelectCommand.Parameters.AddRange(parameters.ToArray());

            servicesTable.Clear();
            adapter.Fill(servicesTable);

            foreach (DataRow row in servicesTable.Rows)
            {
                services.Add(MapDataRowToService(row));
            }

            return services;
        }

        public List<string> GetAllCategories()
        {
            var categories = new List<string>();
            var categoriesTable = new DataTable();

            using (var command = new SqlCommand("SELECT DISTINCT Category FROM Services ORDER BY Category", connection))
            using (var categoriesAdapter = new SqlDataAdapter(command))
            {
                categoriesAdapter.Fill(categoriesTable);
            }

            foreach (DataRow row in categoriesTable.Rows)
            {
                categories.Add(row["Category"].ToString());
            }

            return categories;
        }

        public Service GetServiceById(int id)
        {
            var tempTable = new DataTable();
            using (var command = new SqlCommand("SELECT * FROM Services WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (var tempAdapter = new SqlDataAdapter(command))
                {
                    tempAdapter.Fill(tempTable);
                }
            }

            if (tempTable.Rows.Count > 0)
            {
                return MapDataRowToService(tempTable.Rows[0]);
            }

            return null;
        }

        public bool AddService(Service service)
        {
            string query = @"
                INSERT INTO Services (Name, Description, Category, Price, Provider, Address, Phone, RegistrationDate, IsActive)
                VALUES (@name, @description, @category, @price, @provider, @address, @phone, @registrationDate, @isActive)";

            using (var command = new SqlCommand(query, connection))
            {
                AddServiceParameters(command, service);

                int result = command.ExecuteNonQuery();

                // Обновляем кэшированную таблицу
                if (result > 0 && adapter != null)
                {
                    servicesTable.Clear();
                    adapter.Fill(servicesTable);
                }

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

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", service.Id);
                AddServiceParameters(command, service);

                int result = command.ExecuteNonQuery();

                // Обновляем кэшированную таблицу
                if (result > 0 && adapter != null)
                {
                    var row = servicesTable.Select($"Id = {service.Id}").FirstOrDefault();
                    if (row != null)
                    {
                        UpdateDataRowFromService(row, service);
                    }
                }

                return result > 0;
            }
        }

        public bool DeleteService(int id)
        {
            using (var command = new SqlCommand("DELETE FROM Services WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);

                int result = command.ExecuteNonQuery();

                // Обновляем кэшированную таблицу
                if (result > 0 && adapter != null)
                {
                    var row = servicesTable.Select($"Id = {id}").FirstOrDefault();
                    if (row != null)
                    {
                        servicesTable.Rows.Remove(row);
                    }
                }

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

        private Service MapDataRowToService(DataRow row)
        {
            return new Service
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString(),
                Description = row["Description"] == DBNull.Value ? string.Empty : row["Description"].ToString(),
                Category = row["Category"].ToString(),
                Price = Convert.ToDecimal(row["Price"]),
                Provider = row["Provider"].ToString(),
                Address = row["Address"] == DBNull.Value ? string.Empty : row["Address"].ToString(),
                Phone = row["Phone"].ToString(),
                RegistrationDate = Convert.ToDateTime(row["RegistrationDate"]),
                IsActive = Convert.ToBoolean(row["IsActive"])
            };
        }

        private void UpdateDataRowFromService(DataRow row, Service service)
        {
            row["Name"] = service.Name;
            row["Description"] = string.IsNullOrWhiteSpace(service.Description) ? (object)DBNull.Value : service.Description;
            row["Category"] = service.Category;
            row["Price"] = service.Price;
            row["Provider"] = service.Provider;
            row["Address"] = string.IsNullOrWhiteSpace(service.Address) ? (object)DBNull.Value : service.Address;
            row["Phone"] = service.Phone;
            row["RegistrationDate"] = service.RegistrationDate;
            row["IsActive"] = service.IsActive;
        }

        // Метод для закрытия соединения (вызывать при завершении работы приложения)
        public static void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }

            if (adapter != null)
            {
                adapter.Dispose();
                adapter = null;
            }

            if (servicesTable != null)
            {
                servicesTable.Clear();
                servicesTable.Dispose();
                servicesTable = null;
            }
        }
    }
}