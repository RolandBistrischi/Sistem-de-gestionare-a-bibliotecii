using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace sistem_de_gestionare_a_bibliotecii
{
    public partial class Biblioteca : Form
    {
        int height;
        int width;
        bool signupApasat = false;
        bool backlaCarti = false;
        private Carti cartiForm = null;

        public Biblioteca()
        {
            InitializeComponent();
        }

        private void Biblioteca_Load( object sender, EventArgs e )
        {
            bibliotecaLoginPanel.Hide();

            loginButton.Anchor = AnchorStyles.None; // Debifează orice ancorare existentă pentru butonul de login
            loginButton.Dock = DockStyle.None; // Dezactivează orice ancorare existentă pentru butonul de login

            // Calculează coordonatele X și Y pentru a centra butonul în fereastră
            int x = (ClientSize.Width - loginButton.Width) / 2;
            int y = (ClientSize.Height - loginButton.Height) / 2;
            loginButton.Location = new Point(x - 125, y);

            signUpButton.Location = new Point(loginButton.Right + 100, loginButton.Top);
            signUpButton.Anchor = AnchorStyles.None;


            height = ClientSize.Height;
            width = ClientSize.Width;

            butonuldeBack();
        }

        private void LoginButton_Click( object sender, EventArgs e )
        {
            //Console.WriteLine("Ati apasat Login!");
            MinimumSize = new Size(bibliotecaLoginPanel.Width, MinimumSize.Height);
            loginButton.Visible = false;
            signUpButton.Visible = false;
            bibliotecaLoginPanel.Visible = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            //bibliotecaPanelTextBoxEmail.Hide();
            //bibliotecaPanelLabelEmail.Hide();
            bibliotecaPanelTextBoxTelefon.Hide();
            bibliotecaPanelLabelTelefon.Hide();

        }

        private void SignUpButton_Click( object sender, EventArgs e )
        {
            //Console.WriteLine("Ati apasat SignUp!");
            /*MinimumSize = new Size(bibliotecaLoginPanel.Width, MinimumSize.Height);
            loginButton.Visible = false;
            signUpButton.Visible = false;
            bibliotecaLoginPanel.Visible = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            signupApasat = true;

            //bibliotecaPanelTextBoxEmail.Show();
            //bibliotecaPanelLabelEmail.Show();
            bibliotecaPanelTextBoxTelefon.Show();
            bibliotecaPanelLabelTelefon.Show();*/
            SignUpButton();

        }

        private void bibliotecaLoginPanel_Paint( object sender, PaintEventArgs e )
        {
            bibliotecaLoginPanel.Anchor = AnchorStyles.None;
            bibliotecaLoginPanel.Dock = DockStyle.None;
            bibliotecaLoginPanel.Location = new Point(0, 0);

            Width = bibliotecaLoginPanel.Width;
            Height = bibliotecaLoginPanel.Height + 30;
        }

        private void bibliotecaPanelButtonBack_Click( object sender, EventArgs e )
        {
            if (!backlaCarti)
                butonuldeBack();
            else
                BacklaFormCarti();
        }

        private void bibliotecaPanelButtonLogin_Click( object sender, EventArgs e )
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string prenume = textInfo.ToTitleCase(bibliotecaPanelTextBoxPrenume.Text.ToLower().Trim());
            string nume = FormatName(bibliotecaPanelTextBoxNume.Text);
            string email = FormatName(bibliotecaPanelTextBoxEmail.Text);
            string parola = bibliotecaPanelTextBoxParola.Text;
            string telefon = bibliotecaPanelTextBoxTelefon.Text.Trim();
            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            CloseCurrentFormAndOpenNewFormAsync(nume, prenume, email, parola, telefon);
            return;
            /*email != "" && */
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Introduceți un email valid.");
                return;
            }
            if (signupApasat)
            {//Logica butonului de signUp

                if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume) || string.IsNullOrWhiteSpace(telefon) || string.IsNullOrWhiteSpace(parola))
                {
                    MessageBox.Show("Trebuie completate câmpurile: nume, prenume, parola, telefon.");
                    return;
                }
                if (!IsValidTelefon(telefon))
                {
                    MessageBox.Show("Introduceți un număr de telefon valid.");
                    return;
                }
                if (SignUp(connectionString, nume, prenume, email, parola, telefon))
                {
                    CloseCurrentFormAndOpenNewFormAsync(nume, prenume, email, parola, telefon);
                }
            }
            else
            {// Logica pentru Login
                if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume) || string.IsNullOrWhiteSpace(parola))
                {
                    MessageBox.Show("Trebuie completate câmpurile: nume, prenume, parola");
                    return;
                }
                if (Login(connectionString, nume, prenume, email, parola, telefon))
                {
                    CloseCurrentFormAndOpenNewFormAsync(nume, prenume, email, parola, telefon);
                }
            }
        }

        private bool SignUp( string connectionString, string nume, string prenume, string email, string parola, string telefon )
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Verifică dacă utilizatorul există deja în baza de date
                    string checkQuery = "SELECT COUNT(*) FROM Utilizatori WHERE (nume = @nume AND prenume=@prenume) AND email=@email AND (parola=@parola OR telefon=@telefon )";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@nume", nume);
                        checkCommand.Parameters.AddWithValue("@prenume", prenume);
                        checkCommand.Parameters.AddWithValue("@parola", parola);
                        checkCommand.Parameters.AddWithValue("@telefon", telefon);
                        checkCommand.Parameters.AddWithValue("@email", email);

                        int existingUserCount = (int)checkCommand.ExecuteScalar();

                        // Dacă există deja un utilizator cu același nume sau email, afișează un mesaj de eroare
                        if (existingUserCount > 0)
                        {
                            MessageBox.Show("Utilizatorul există deja în baza de date.");
                            return false;
                        }
                    }

                    // Dacă utilizatorul nu există, efectuează inserarea în baza de date
                    string insertQuery = "INSERT INTO Utilizatori (nume, prenume, email, parola, telefon) VALUES (@nume, @prenume, @email, @parola, @telefon)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@nume", nume);
                        insertCommand.Parameters.AddWithValue("@prenume", prenume);
                        insertCommand.Parameters.AddWithValue("@email", email);
                        insertCommand.Parameters.AddWithValue("@parola", parola);
                        insertCommand.Parameters.AddWithValue("@telefon", telefon);
                        insertCommand.ExecuteNonQuery();
                    }
                    MessageBox.Show("Utilizatorul a fost adăugat cu succes în baza de date.");
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la inserarea în baza de date: " + ex.Message);
                }
            }
            return false;
        }

        private bool Login( string connectionString, string nume, string prenume, string email, string parola, string telefon )
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM Utilizatori WHERE nume = @nume AND prenume = @prenume AND parola = @parola AND email=@email";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nume", nume);
                        command.Parameters.AddWithValue("@prenume", prenume);
                        command.Parameters.AddWithValue("@parola", parola);
                        command.Parameters.AddWithValue("@email", email);

                        int result = (int)command.ExecuteScalar();

                        if (result > 0)
                        {
                            // Utilizatorul există în baza de date
                            return true;
                        }
                        else
                            MessageBox.Show("Autentificare eșuată. Verificați datele de autentificare sau nu v-ati conectat inca.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la autentificare: " + ex.Message);
                }
            }
            return false;
        }

        public bool IsValidEmail( string email )
        {
            // Expresie regulată pentru validarea formatului complet al emailului
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            // Verifică dacă emailul respectă formatul
            return Regex.IsMatch(email, pattern);
        }

        public bool IsValidTelefon( string telefon )
        {
            // Expresie regulată pentru validarea formatului numărului de telefon
            string pattern = @"^\d{10}$";

            // Verifică dacă numărul de telefon respectă formatul
            return Regex.IsMatch(telefon, pattern);
        }

        public string FormatName( string input )
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
            input.Trim();
            // Păstrează prima literă mare și restul caractere mici
            string formattedText = input.Substring(0, 1).ToUpper() + input.Substring(1).ToLowerInvariant();
            return formattedText;
        }

        private void CloseCurrentFormAndOpenNewFormAsync( string nume, string prenume, string email, string parola, string telefon )
        {
            // Creează o instanță a noului form      
            /*
            inaltime panel=451  latime panel=455
            inaltime menu=24  latime menu=476
            min inaltime=535  min latime=492
            inaltime form=294  latime form=375              
            */




            bibliotecaPanelTextBoxNume.Clear();
            bibliotecaPanelTextBoxPrenume.Clear();
            bibliotecaPanelTextBoxEmail.Clear();
            bibliotecaPanelTextBoxParola.Clear();
            bibliotecaPanelTextBoxTelefon.Clear();
            butonuldeBack();

            if (cartiForm == null)
            {
                /*cartiForm = new Carti(nume, prenume, email, parola, telefon)
                {
                    MinimumSize = new Size(490, 535)
                };*/
                cartiForm = new Carti("a", "a", "a@a.a", "a", telefon)
                {
                    MinimumSize = new Size(490, 535)
                };
                //cartiForm.LoadUser(nume, prenume, email, parola, telefon);
                cartiForm.Size = cartiForm.MinimumSize;
                cartiForm.FormClosed += ( sender, e ) => { cartiForm = null; }; // Resetare referință când formularul este închis
            }

            if (!cartiForm.Visible)
            {
                cartiForm.Visible = true;

                if (Application.OpenForms ["Biblioteca"] != null)
                {
                    Application.OpenForms ["Biblioteca"].Hide(); // Ascunde formularul Biblioteca
                }
            }
            //cartiForm.LoadUser(nume, prenume, email, parola, telefon);
            cartiForm.LoadUser("a", "a", "a@a.a", "a", telefon);
        }

        private void butonuldeBack()
        {
            bibliotecaLoginPanel.Hide();
            loginButton.Show();
            signUpButton.Show();
            Width = 681;
            Height = 423;
            FormBorderStyle = FormBorderStyle.Sizable;
            signupApasat = false;
            MinimumSize = new Size(bibliotecaLoginPanel.Width + 50, MinimumSize.Height);
        }

        private void BacklaFormCarti()
        {
            butonuldeBack();
            if (Application.OpenForms ["Biblioteca"] != null)
            {
                Application.OpenForms ["Biblioteca"].Hide();
            }

            if (Application.OpenForms ["Carti"] != null)
            {
                Application.OpenForms ["Carti"].Show();
            }
            backlaCarti = false;
        }

        public void AdaugaUserFromCartiForm()
        {
            backlaCarti = true;
            SignUpButton();
        }

        private void SignUpButton()
        {
            MinimumSize = new Size(bibliotecaLoginPanel.Width, MinimumSize.Height);
            loginButton.Visible = false;
            signUpButton.Visible = false;
            bibliotecaLoginPanel.Visible = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            signupApasat = true;
            bibliotecaPanelTextBoxTelefon.Show();
            bibliotecaPanelLabelTelefon.Show();
        }

    }
}
