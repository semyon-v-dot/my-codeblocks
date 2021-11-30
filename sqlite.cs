using System;
using System.Data.SQLite;

namespace SKINEMSA_bot
{
    public class SkinemsaDatabase
    {
        public enum RequisiteType
        {
            Card,
            Phone
        }
        
        public readonly struct Requisite
        {
            public int ChatId { get; }
            public RequisiteType Type { get; }
            public int Number { get; }

            public Requisite(int chatId, RequisiteType type, int number)
            {
                ChatId = chatId;
                Type = type;
                Number = number;
            }
        }

        public readonly struct Product
        {
            public int ChatId { get; }
            public string Name { get; }
            public int Amount { get; }
            public int Price { get; }

            public Product(int chatId, string name, int amount, int price)
            {
                ChatId = chatId;
                Name = name;
                Amount = amount;
                Price = price;
            }
        }
        
        private const string ProductsTableName = "Products";
        private const string RequisitesTableName = "Requisites";
        private const string ChatsTableName = "Chats";
        private const string RequisiteTypesTableName = "RequisiteTypes";
        private readonly SQLiteConnection _databaseConnection;

        public SkinemsaDatabase(string fullFilename)
        {
            _databaseConnection = new SQLiteConnection($"URI=file:{fullFilename}");
        }

        public void InsertChat(int chatId)
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = $"INSERT INTO {ChatsTableName}(ChatID) VALUES({chatId})"
            };
            command.ExecuteNonQuery();
        }

        public void DeleteChat(int chatId)
        {
            DeleteRequisite(chatId);
            DeleteProducts(chatId);
            DeleteChatIdFromTable(chatId);
        }
        
        public void InsertProduct(Product product)
        {
            InsertProduct(product.ChatId, product.Name, product.Amount, product.Price);
        }
        
        public void InsertProduct(int chatId, string name, int amount, int price)
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = string.Join(
                    " ",
                    $"INSERT INTO {ProductsTableName}(ChatID, Name, Amount, Price)",
                    $"VALUES({chatId}, '{name}', {amount}, {price})")
            };
            command.ExecuteNonQuery();
        }

        public void DeleteProduct(int chatId, string name)
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = $"DELETE FROM {ProductsTableName} WHERE ChatID = {chatId} AND Name = {name}"
            };
            command.ExecuteNonQuery();
        }

        public void DeleteProducts(int chatId)
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = $"DELETE FROM {ProductsTableName} WHERE ChatID = {chatId}"
            };
            command.ExecuteNonQuery();
        }

        public void InsertRequisite(Requisite requisite)
        {
            InsertRequisite(requisite.ChatId, requisite.Type, requisite.Number);       
        }
        
        public void InsertRequisite(int chatId, RequisiteType type, int number)
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = string.Join(
                    " ",
                    $"INSERT INTO {RequisitesTableName}(ChatID, RequisiteType, Number)",
                    $"VALUES({chatId}, '{type}', {number})")
            };
            command.ExecuteNonQuery();
        }

        public void DeleteRequisite(int chatId)
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = $"DELETE FROM {RequisitesTableName} WHERE ChatID = {chatId}"
            };
            command.ExecuteNonQuery();          
        }
        
        public Product[] GetProducts(int chatId)
        {
            throw new NotImplementedException();
        }

        public Requisite GetRequisite(int chatId)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            _databaseConnection.Open();

            if (!TablesExist())
                CreateTables();
        }

        private bool TablesExist()
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = string.Join(
                    " ",
                    "SELECT COUNT(*)",
                    "FROM sqlite_master",
                    "WHERE type = 'table'",
                    "AND (name = @name_1 OR name = @name_2 OR name = @name_3)")
            };
            command.Parameters.AddWithValue("@name_1", ProductsTableName);
            command.Parameters.AddWithValue("@name_2", RequisitesTableName);
            command.Parameters.AddWithValue("@name_3", ChatsTableName);

            return Convert.ToInt32(command.ExecuteScalar()) == 3;
        }

        private void CreateTables()
        {
            CreateChatsTable();
            
            CreateProductsTable();

            CreateRequisiteTypesTable();

            InsertValuesIntoRequisiteTypesTable();

            CreateRequisitesTable();
        }

        private void CreateChatsTable()
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = $"CREATE TABLE {ChatsTableName}(ChatID INTEGER PRIMARY KEY)"
            };
            command.ExecuteNonQuery();
        }

        private void CreateProductsTable()
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = string.Join(
                    " ",
                    $"CREATE TABLE {ProductsTableName}",
                    "(ChatID INTEGER,",
                    "Name TEXT,",
                    "Amount INTEGER,",
                    "Price INTEGER,",
                    $"FOREIGN KEY(ChatID) REFERENCES {ChatsTableName}(ChatID))")
            };
            command.ExecuteNonQuery();
        }
        
        private void CreateRequisiteTypesTable()
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = $"CREATE TABLE {RequisiteTypesTableName}(RequisiteType Text)"
            };
            command.ExecuteNonQuery();
        }

        private void InsertValuesIntoRequisiteTypesTable()
        {
            using var command = new SQLiteCommand(_databaseConnection);
            foreach (var requisiteType in Enum.GetValues(typeof(RequisiteType)))
            {
                command.CommandText =
                    $"INSERT INTO {RequisiteTypesTableName}(RequisiteType) VALUES('{requisiteType}')";
                command.ExecuteNonQuery();
            }
        }

        private void CreateRequisitesTable()
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = string.Join(
                    " ",
                    $"CREATE TABLE {RequisitesTableName}",
                    "(ChatID INTEGER PRIMARY KEY,",
                    "RequisiteType TEXT,",
                    "Number INTEGER,",
                    $"FOREIGN KEY(ChatID) REFERENCES {ChatsTableName}(ChatID),",
                    $"FOREIGN KEY(RequisiteType) REFERENCES {RequisiteTypesTableName}(RequisiteType))")
            };
            command.ExecuteNonQuery();
        }

        private void DeleteChatIdFromTable(int chatId)
        {
            using var command = new SQLiteCommand(_databaseConnection)
            {
                CommandText = $"DELETE FROM {ChatsTableName} WHERE ChatID = {chatId}"
            };
            command.ExecuteNonQuery();  
        }
    }
}