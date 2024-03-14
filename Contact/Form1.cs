using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace Contact
{
    public partial class Form_Contact : Form
    {
        ContactClass ContactClass = new ContactClass();
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=159632;Database=Contact";
        public Form_Contact()
        {
            InitializeComponent();
            LoadData();
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Телефон" && e.Value != null)
            {
                e.Value = FormatPhoneNumber(e.Value.ToString());
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                string digitsOnly = Regex.Replace(phoneNumber, @"[^\d]", "");
                if (digitsOnly.Length >= 10)
                {
                    return string.Format("{0} ({1}) {2}-{3}-{4}",
                        digitsOnly.Substring(0, 1),    
                        digitsOnly.Substring(1, 3),    
                        digitsOnly.Substring(4, 3),    
                        digitsOnly.Substring(7, 2),    
                        digitsOnly.Substring(9));      
                }
            }
            return phoneNumber;
        }
        private void LoadData()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string query = "SELECT * FROM Contact";
                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, connection);
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet, "Contact");
                    dataGridView1.DataSource = dataSet.Tables["Contact"];
                    dataGridView1.Columns[0].HeaderText = "ID";
                    dataGridView1.Columns[1].HeaderText = "Имя";
                    dataGridView1.Columns[2].HeaderText = "Телефон";
                    foreach (DataRow row in dataSet.Tables["Contact"].Rows)
                    {
                        string phoneNumber = row[2].ToString();
                        row[2] = FormatPhoneNumber(phoneNumber);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }
        private void Check_phone(int phone)
        {
            if (phone != 11)
            {
                return;
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            DataRowView selectedRow = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            string phone = Convert.ToString(selectedRow["phone"]);
            if (ContactClass.Check_phone_null(phone) && ContactClass.Check_phone_length(phone) && ContactClass.Check_phone_number(phone) && ContactClass.IsPhoneNumberUnique(phone))
            {
                if (selectedRow != null)
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        string text = ("SELECT phone FROM contact where phone = @phone");
                        using (NpgsqlCommand command = new NpgsqlCommand(text, connection))
                        {
                            command.Parameters.AddWithValue("@phone", selectedRow["phone"]);
                        }
                        string request = ("INSERT INTO contact (name,phone) values (@name,@phone)");
                        using (NpgsqlCommand command = new NpgsqlCommand(request, connection))
                        {
                            command.Parameters.AddWithValue("@name", selectedRow["name"]);
                            command.Parameters.AddWithValue("@phone", selectedRow["phone"]);
                            try
                            {
                                command.ExecuteNonQuery();
                                MessageBox.Show("Данные успешно добавлены в базу данных.");
                                LoadData();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка при вставке данных: " + ex.Message);
                            }
                        }

                    }
                }
            }
            else
                {
                MessageBox.Show("Телефон должен содержать 11 символов, а также не иметь в составе специальные символы и буквы, а также быть уникальным");
                }           
        }                    

        private void button_delete_Click(object sender, EventArgs e)
        {
            DataRowView selectedRow = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;

            if (selectedRow != null)
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string request = ("DELETE FROM contact where id = @id");
                    using (NpgsqlCommand command = new NpgsqlCommand(request, connection))
                    {
                        command.Parameters.AddWithValue("@id", selectedRow["id"]);

                        try
                        {
                            command.ExecuteNonQuery();
                            MessageBox.Show("Данные успешно удалены из базы данных.");
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка при удалении данных: " + ex.Message);
                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("Выберите строку для вставки данных.");
            }
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            DataRowView selectedRow = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;

            if (selectedRow != null)
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string request = ("UPDATE contact SET id  = @id, name = @name, phone = @phone WHERE id = @id");
                    using (NpgsqlCommand command = new NpgsqlCommand(request, connection))
                    {
                        command.Parameters.AddWithValue("@id", selectedRow["ID"]);
                        command.Parameters.AddWithValue("@name", selectedRow["name"]);
                        command.Parameters.AddWithValue("@phone", selectedRow["phone"]);

                        try
                        {
                            command.ExecuteNonQuery();
                            MessageBox.Show("Данные успешно изменены в базе данных.");
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка при изменении данных: " + ex.Message);
                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("Выберите строку для вставки данных.");
            }
        }

        public void search_Click(string text )
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string name = text;
                    connection.Open();
                    string query = "SELECT * FROM Contact WHERE name = @name";

                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", name);

                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet);
                    dataGridView1.DataSource = dataSet.Tables[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }
        private void button_search_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string name = textBox_search.Text;
                    connection.Open();
                    string query = "SELECT * FROM Contact WHERE name = @name";

                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", name);

                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet);
                    dataGridView1.DataSource = dataSet.Tables[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }

        

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
