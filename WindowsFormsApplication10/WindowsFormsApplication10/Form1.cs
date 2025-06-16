using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication10
{
    public partial class Form1 : Form
    {
        SqlConnection cnx;

        public Form1()
        {
            cnx = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=c:\users\gasy\documents\visual studio 2012\Projects\WindowsFormsApplication10\WindowsFormsApplication10\Clients.mdf;Integrated Security=True;Connect Timeout=30");
            InitializeComponent();
            // Test connection
            try
            {
                cnx.Open();
                MessageBox.Show("Connection réussie !");
                cnx.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de connexion : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Charge les données au démarrage
            ChargerDonnees();
        }

        // Fonction pour charger les données dans le DataGridView
        private void ChargerDonnees()
        {
            try
            {
                if (cnx.State != ConnectionState.Closed)
                    cnx.Close();
                cnx.Open();
                string sql = "SELECT * FROM Clients";
                SqlDataAdapter da = new SqlDataAdapter(sql, cnx);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des données : " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (cnx.State != ConnectionState.Closed)
                    cnx.Close();
            }
        }

        // Fonction de validation des champs
        private bool ValiderChamps()
        {
            if (string.IsNullOrEmpty(textBoxID.Text) || string.IsNullOrEmpty(textBoxNOM.Text) ||
                string.IsNullOrEmpty(textBoxPRENOM.Text) || string.IsNullOrEmpty(textBoxADRESSE.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs !", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Regex.IsMatch(textBoxID.Text, @"^\d+$"))
            {
                MessageBox.Show("L'ID doit contenir uniquement des chiffres !", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!Regex.IsMatch(textBoxNOM.Text, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Le Nom doit contenir uniquement des lettres et des espaces !", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!Regex.IsMatch(textBoxPRENOM.Text, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Le Prénom doit contenir uniquement des lettres et des espaces !", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnAjouter_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValiderChamps())
                    return;

                if (cnx.State != ConnectionState.Closed)
                    cnx.Close();
                cnx.Open();
                string sql = "INSERT INTO Clients (id, nom, prenoms, Adresse) VALUES (@id, @nom, @prenoms, @adresse)";
                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@id", textBoxID.Text);
                cmd.Parameters.AddWithValue("@nom", textBoxNOM.Text);
                cmd.Parameters.AddWithValue("@prenoms", textBoxPRENOM.Text);
                cmd.Parameters.AddWithValue("@adresse", textBoxADRESSE.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Client ajouté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerDonnees();
                textBoxID.Clear();
                textBoxNOM.Clear();
                textBoxPRENOM.Clear();
                textBoxADRESSE.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (cnx.State != ConnectionState.Closed)
                    cnx.Close();
            }
        }

        private void btnModifier_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBoxID.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un client à modifier !", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValiderChamps())
                    return;

                if (cnx.State != ConnectionState.Closed)
                    cnx.Close();
                cnx.Open();
                string sql = "UPDATE Clients SET nom = @nom, prenoms = @prenoms, Adresse = @adresse WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@id", textBoxID.Text);
                cmd.Parameters.AddWithValue("@nom", textBoxNOM.Text);
                cmd.Parameters.AddWithValue("@prenoms", textBoxPRENOM.Text);
                cmd.Parameters.AddWithValue("@adresse", textBoxADRESSE.Text);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Client modifié avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ChargerDonnees();
                    textBoxID.Clear();
                    textBoxNOM.Clear();
                    textBoxPRENOM.Clear();
                    textBoxADRESSE.Clear();
                }
                else
                {
                    MessageBox.Show("Aucun client trouvé avec cet ID.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la modification : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (cnx.State != ConnectionState.Closed)
                    cnx.Close();
            }
        }

        private void btnSupprimer_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBoxID.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un client à supprimer !", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Voulez-vous vraiment supprimer ce client ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (cnx.State != ConnectionState.Closed)
                        cnx.Close();
                    cnx.Open();
                    string sql = "DELETE FROM Clients WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(sql, cnx);
                    cmd.Parameters.AddWithValue("@id", textBoxID.Text);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Client supprimé avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ChargerDonnees();
                        textBoxID.Clear();
                        textBoxNOM.Clear();
                        textBoxPRENOM.Clear();
                        textBoxADRESSE.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Aucun client trouvé avec cet ID.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la suppression : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (cnx.State != ConnectionState.Closed)
                    cnx.Close();
            }
        }

        // Événement pour remplir les TextBox lors de la sélection d'une ligne dans le DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Vider les TextBox avant de remplir
            textBoxID.Clear();
            textBoxNOM.Clear();
            textBoxPRENOM.Clear();
            textBoxADRESSE.Clear();

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBoxID.Text = row.Cells["id"].Value != null ? row.Cells["id"].Value.ToString() : "";
                textBoxNOM.Text = row.Cells["nom"].Value != null ? row.Cells["nom"].Value.ToString() : "";
                textBoxPRENOM.Text = row.Cells["prenoms"].Value != null ? row.Cells["prenoms"].Value.ToString() : "";
                textBoxADRESSE.Text = row.Cells["Adresse"].Value != null ? row.Cells["Adresse"].Value.ToString() : "";
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}