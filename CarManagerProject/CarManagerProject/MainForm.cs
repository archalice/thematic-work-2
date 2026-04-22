using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CarManagerProject
{
    public partial class MainForm : Form
    {
        string connectionString =
@"Data Source=(LocalDB)\MSSQLLocalDB;
AttachDbFilename=D:\Prodjects\CarManagerProject\CarManagerProject\CarDatabase.mdf;
Integrated Security=True;";
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = @"INSERT INTO Car_Brands 
    (brand_name, country, foundation_year)
    VALUES (@name, @country, @year)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", textBox1.Text);
                cmd.Parameters.AddWithValue("@country", textBox2.Text);
                cmd.Parameters.AddWithValue("@year", textBox3.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Добавлено полностью!");

        }

        private void button2_Click(object sender, EventArgs e)
        {

            string query = "SELECT * FROM Car_Brands"; // або Car_Models

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string query = "DELETE FROM Car_Brands WHERE brand_name = @name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", textBox1.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Удалено!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM Car_Brands WHERE brand_name LIKE @name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@name", "%" + textBox1.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "SELECT brand_name FROM Car_Brands";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                comboBox1.Items.Clear();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["brand_name"].ToString());
                }
            }
        }
    }
}