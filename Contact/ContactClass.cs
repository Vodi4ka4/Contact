using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Contact
{
    public class ContactClass
    {
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=159632;Database=Contact";
        public bool Check_phone_length(string phone)
        {
            if (phone.Length != 11)
            {
                return false;
            }
            return true;
        }
        public bool Check_phone_number(string phone)
        {
            string pattern = @"^(?=(.*\d){11})";
            Regex  regex = new Regex(pattern);
            if (regex.IsMatch(phone))
            {
                return true;
            }
            return false;
        }
        public bool Check_phone_null(string phone)
        {
            if (phone != null)
            {
                return true;
            }
            return false;
        }
        public bool IsPhoneNumberUnique(string phoneNumber)
        {
            List<string> existingPhoneNumbers = GetExistingPhoneNumbersFromDatabase();
            return !existingPhoneNumbers.Contains(phoneNumber);
        }
        private List<string> GetExistingPhoneNumbersFromDatabase()
        {
            List<string> phoneNumbers = new List<string>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT Phone FROM Contact"; 
                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string phoneNumber = reader["Phone"].ToString();
                    phoneNumbers.Add(phoneNumber);
                }

                reader.Close();
            }

            return phoneNumbers;
        }
    }
}

