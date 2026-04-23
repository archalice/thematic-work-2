using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace CarManagerProject
{
    public partial class DetailsForm : Form
    {
        string connectionString;
        public DetailsForm(string brandName)
        {
            InitializeComponent();
            connectionString = DbInitializer.ConnectionString;

            LoadBrandDetails(brandName);
            LoadModels(brandName);
        }

        // -------------------------
        // Бренд
        // -------------------------
        private void LoadBrandDetails(string brandName)
        {
            string query = @"
SELECT brand_name, country, foundation_year
FROM Car_Brands
WHERE brand_name = @name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", brandName);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    textBox1.Text = reader["brand_name"].ToString();
                    textBox2.Text = reader["country"].ToString();
                    textBox3.Text = reader["foundation_year"].ToString();
                }
            }
        }

        // -------------------------
        // Моделі
        // -------------------------
        private void LoadModels(string brandName)
        {
            string query = @"
SELECT 
    m.model_name,
    m.price,
    m.discounted_price,
    m.car_photo_img
FROM Car_Models m
JOIN Car_Brands b ON m.brand_id = b.brand_id
WHERE b.brand_name = @name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@name", brandName);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }

        private void DetailsForm_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null &&
               dataGridView1.CurrentRow.Cells["car_photo_img"].Value != null)
            {
                string fileName = dataGridView1.CurrentRow.Cells["car_photo_img"].Value.ToString();
                string normalizedFileName = fileName.Replace('/', '\\');
                string path = Path.Combine(Application.StartupPath, normalizedFileName);

                if (!File.Exists(path))
                {
                    path = Path.Combine(Application.StartupPath, "Images", Path.GetFileName(normalizedFileName));
                }

                if (File.Exists(path))
                {
                    pictureBox1.Image = Image.FromFile(path);
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
