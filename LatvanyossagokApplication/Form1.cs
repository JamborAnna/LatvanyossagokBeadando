using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LatvanyossagokApplication
{
    
    public partial class Form1 : Form
    {
        private MySqlConnection conn;

        public Form1()
        {
            InitializeComponent();
            conn = new MySqlConnection("Server=localhost; Port=3307;Database=latvanyossagokdb;Uid=root;Pwd=;");
            conn.Open();
            createTable();
            Varos();
            
        }
        public void createTable()
        {
            listBoxVaros.Items.Clear();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT*FROM varosok";

        }

        private void LatvanyossagKilistazas(int id)
        {
           
            {
                listBoxLatvanyossag.Items.Clear();
                groupBox2.Enabled = false;
                textBoxLatvanyossagMod.Text = "";
                VarosLeirasModTxtB.Text = "";
                NumUppDArMod.Value = 0;
                if (listBoxVaros.SelectedIndex!= -1)
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT id, nev, leiras,ar,id FROM latvanyossagok WHERE varos_id=@varos_id ORDER BY nev";
                    var varos = (Varos)listBoxVaros.SelectedItem;
                    cmd.Parameters.AddWithValue("id", varos.Id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           // var id = reader.GetInt32("id");
                            var nev = reader.GetString("nev");
                            var leiras = reader.GetString("leiras");
                            var ar = reader.GetInt32("ar");
                            var varos_id = reader.GetInt32("varos_id");
                            var latvanyossag = new Latvanyossag(id, nev, leiras, ar, varos_id);

                            listBoxLatvanyossag.Items.Add(latvanyossag);
                        }
                    }
                }
               
            }

        }

      

        void Varos()
        {
            VarosokListB.Items.Clear();
            listBoxVaros.Items.Clear();
            groupBox1.Enabled = false;
            textUjVarosNev.Text = "";
            NumUppDUjLakossag.Value = 0;


            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id,nev,lakossag FROM varos ORDER BY nev";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32("id");
                    var nev = reader.GetString("nev");
                    var lakossag = reader.GetInt32("lakossag");
                    var varos = new Varos(id, nev, lakossag);
                    listBoxVaros.Items.Add(varos);
                }
            }
        }

        private void btnFeltoltesVarosnev_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBoxVarosNev.Text == "" || textBoxVarosNev.Text == null)
                {
                    MessageBox.Show("Nem adott meg város nevet!");
                    return;
                }
                if (lakossagUppD.Value <= 0)
                {
                    MessageBox.Show("Érvénytelen lakosság szám!");
                    return;
                }
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO varosok(nev,lakossag) VALUE(@nev,@lakossag)";
                cmd.Parameters.AddWithValue("@nev", textBoxVarosNev.Text);
                cmd.Parameters.AddWithValue("@lakossag", lakossagUppD.Value);

                cmd.ExecuteNonQuery();

                createTable();
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate entry"))
                {
                    MessageBox.Show("A város már szerepel az adatbázisban!");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void latvanyossagFeltoltbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (latvanyossagText.Text == "" || latvanyossagText.Text == null)
                {
                    MessageBox.Show("Nem adta meg a látványosság nevét!");
                    return;
                }
                if (arNumUppD.Value < 0)
                {
                    MessageBox.Show("Érvénytelen ár!");
                    return;
                }
                if (textBoxLatvanyossagLeiras.Text == "" || textBoxLatvanyossagLeiras.Text == null)
                {
                    MessageBox.Show("Üres leírás!");
                    return;
                }
                if (VarosokListB.SelectedIndex == -1)
                {
                    MessageBox.Show("Nem választott ki várost");
                    return;
                }
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO latvanyossagok 
                                        (nev, leiras, ar, varos_id)
                                  VALUES(@nev, @leiras, @ar, @varos_id)";
                cmd.Parameters.AddWithValue("@nev", latvanyossagText.Text);
                cmd.Parameters.AddWithValue("@leiras", textBoxLatvanyossagLeiras.Text);
                cmd.Parameters.AddWithValue("@ar", arNumUppD.Value);
                cmd.Parameters.AddWithValue("@varos_id", ((Varos)VarosokListB.SelectedItem).Id);
                cmd.ExecuteNonQuery();

                if (listBoxVaros.SelectedIndex != -1
                && VarosokListB.SelectedIndex != -1
                && ((Varos)listBoxVaros.SelectedItem).Id == ((Varos)VarosokListB.SelectedItem).Id)
                    LatvanyossagKilistazas(((Varos)VarosokListB.SelectedItem).Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Váratlan hiba történt!");
            }
        }

        private void varosTorlesBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBoxVaros.SelectedIndex == -1)
                {
                    MessageBox.Show("Nincs kiválasztva város!");
                    return;
                }
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"DELETE FROM varosok WHERE id=@id";
                cmd.Parameters.AddWithValue("@id", ((Varos)listBoxVaros.SelectedItem).Id);

                cmd.ExecuteNonQuery();
                Varos();
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("Hiba történt"))
                {
                    MessageBox.Show("Az elem nem törölhető, mert tartozik hozzá látványosság!");
                    return;
                }
                MessageBox.Show("Hiba!");
            }
        }

        private void VarosModosit_Click(object sender, EventArgs e)
        {
            try
            {
                if (textUjVarosNev.Text == "" || textUjVarosNev.Text == null)
                {
                    MessageBox.Show("Nem adott meg város nevet!");
                    return;
                }
                if (NumUppDUjLakossag.Value <= 0)
                {
                    MessageBox.Show("Érvénytelen lakossági adat!");
                    return;
                }

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"UPDATE varosok 
                                    SET nev = @nev, lakossag = @lakossag
                                    WHERE id = @id";
                cmd.Parameters.AddWithValue("@nev", textUjVarosNev.Text);
                cmd.Parameters.AddWithValue("@lakossag", NumUppDUjLakossag.Value);
                cmd.Parameters.AddWithValue("@id", ((Varos)listBoxVaros.SelectedItem).Id);

                cmd.ExecuteNonQuery();
               Varos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelet_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBoxVaros.SelectedIndex == -1)
                {
                    MessageBox.Show("Nincs kiválasztva város");
                    return;
                }
                if (listBoxLatvanyossag.SelectedIndex == -1)
                {
                    MessageBox.Show("Nincs kiválasztva látványosság!");
                    return;
                }
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"DELETE FROM latvanyossagok WHERE id=@id";
                cmd.Parameters.AddWithValue("@id", ((Latvanyossag)listBoxLatvanyossag.SelectedItem).Id);

                cmd.ExecuteNonQuery();
                LatvanyossagKilistazas(((Varos)listBoxVaros.SelectedItem).Id);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("foreign key constraint fails"))
                {
                    MessageBox.Show("Az elem nem törölhető, mert tartozik hozzá látványosság!");
                    return;
                }
                MessageBox.Show("Váratlan hiba történt!");
            }
        }

        private void listBoxLatvanyossag_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxLatvanyossag.SelectedIndex != -1)
            {
                textBoxLatvanyossagMod.Enabled = true;
                arNumUppD.Enabled = true;
                VarosLeirasModTxtB.Enabled = true;

                textBoxLatvanyossagMod.Text = ((Latvanyossag)listBoxLatvanyossag.SelectedItem).Nev;
                arNumUppD.Value = ((Latvanyossag)listBoxLatvanyossag.SelectedItem).Ar;
                VarosLeirasModTxtB.Text = ((Latvanyossag)listBoxLatvanyossag.SelectedItem).Leiras;
            }
        }

        private void bttnLatMod_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBoxVaros.SelectedIndex == -1)
                {
                    MessageBox.Show("Nincs kiválasztva város!");
                    return;
                }
                if (listBoxLatvanyossag.SelectedIndex == -1)
                {
                    MessageBox.Show("Nincs kiválasztva látványosság!");
                    return;
                }
                if (latvanyossagText.Text == "" || textBoxLatvanyossagMod.Text == null)
                {
                    MessageBox.Show("Nem adott meg látványosság nevet!");
                    return;
                }
                if (arNumUppD.Value <= 0)
                {
                    MessageBox.Show("Érvénytelen ár!");
                    return;
                }

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"UPDATE latvanyossagok
                                    SET nev = @nev, ar = @ar, leiras = @leiras
                                    WHERE id = @id";
                cmd.Parameters.AddWithValue("@nev", textBoxLatvanyossagMod.Text);
                cmd.Parameters.AddWithValue("@ar", arNumUppD.Value);
                cmd.Parameters.AddWithValue("@id", ((Latvanyossag)listBoxLatvanyossag.SelectedItem).Id);
                cmd.Parameters.AddWithValue("@leiras", VarosLeirasModTxtB.Text);

                cmd.ExecuteNonQuery();
                LatvanyossagKilistazas(((Varos)listBoxVaros.SelectedItem).Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listBoxVaros_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxVaros.SelectedIndex != -1)
            {
                textUjVarosNev.Enabled = true;
                NumUppDUjLakossag.Enabled = true;
                LatvanyossagKilistazas(((Varos)listBoxVaros.SelectedItem).Id);

                textUjVarosNev.Text = ((Varos)listBoxVaros.SelectedItem).Nev;
                NumUppDUjLakossag.Value = ((Varos)listBoxVaros.SelectedItem).Lakossag;
            }
        }
    }
    
 }
