using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;


/*using System.Data.OleDb;
using System.Drawing;
using System.Text.RegularExpressions;*/

namespace sistem_de_gestionare_a_bibliotecii
{
    public partial class Carti : Form
    {
        private Dictionary<string, bool> panelStates = new Dictionary<string, bool>();

        private string numeUtilizator = "B";
        private string prenumeUtilizator = "B";
        private string emailUtilizator = "B@b.b";
        private string parolaUtilizator = "b";
        private string telefonUtilizator = "9876543210";
        private int utilizatorCurentId = -1;
        private bool utilizatorSters = false;
        private bool admin = false;
        private Color yourCurrentBackgroundColor = Carti.DefaultBackColor;

        public Carti()
        {
            InitializeComponent();
        }

        public Carti( string nume, string prenume, string email, string parola, string telefon )
        {
            InitializeComponent();
            numeUtilizator = nume;
            prenumeUtilizator = prenume;
            parolaUtilizator = parola;
            emailUtilizator = email;
            telefonUtilizator = telefon;
        }

        private void Carti_Load( object sender, EventArgs e )
        {
            LoadUser(numeUtilizator, prenumeUtilizator, emailUtilizator, parolaUtilizator, telefonUtilizator);
        }

        public void LoadUser( string nume, string prenume, string email, string parola, string telefon )
        {
            numeUtilizator = nume;
            prenumeUtilizator = prenume;
            parolaUtilizator = parola;
            emailUtilizator = email;
            telefonUtilizator = telefon;

            cartiPanelCartiDateTimePickerAnAparitie.Format = DateTimePickerFormat.Custom;
            cartiPanelCartiDateTimePickerAnAparitie.CustomFormat = "dd MMMM yyyy";
            cartiPanelCartiDateTimePickerAnAparitie.Value = DateTime.Today;

            List<ComboBox> comboBoxes = new List<ComboBox>
            {
                cartiPanelComboBoxNume, cartiPanelComboBoxPrenume,
                cartiPanelComboBoxGen, cartiPanelComboBoxEditura,
                cartiPanelComboBoxRolAdmin
            };
            foreach (ComboBox comboBox in comboBoxes)
            {
                comboBox.IntegralHeight = false;
                comboBox.MaxDropDownItems = 5;
            }

            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            InitializePanelStates();
            utilizatorCurentId = GetUserID(connectionString, numeUtilizator, prenumeUtilizator, emailUtilizator, parolaUtilizator, "parola");
            admin = VerificaAdmin(utilizatorCurentId, connectionString);

            cartiPanelCartiButtonAdaugasiSterge.Text = "Imprumuta";
            cartiPanelTextBoxNrCopii.Visible = false;
            cartiPanelLabelNrCopii.Visible = false;
            cartiPanelTextBoxISBN.ReadOnly = false;
            cartiPanelCartiListBox.HorizontalScrollbar = true;
            cartiPanelUserApasat.Visible = false;
            cartiPanelInformatiiUserApasat.Visible = false;
            cartiPanelCartiApasat.Visible = true;
            cartiPanelComboBoxRolAdmin.Visible = false;
            cartiPanelLabelRolAdmin.Visible = false;



            cartiPanelTextBoxISBN.Visible = admin;
            cartiPanelLabelISBN.Visible = admin;
            adaugaToolStripMenuItem1.Visible = admin;
            stergeToolStripMenuItem1.Visible = admin;
            modificaToolStripMenuItem1.Visible = admin;
            adminCartiToolStripMenuItem.Visible = admin;
            adaugaToolStripMenuItem2.Visible = admin;
            stergeToolStripMenuItem2.Visible = admin;

            ConfigurarePanelCentral(cartiPanelCartiApasat);
            ConfigurarePanelCentral(cartiPanelUserApasat);
            ConfigurarePanelCentral(cartiPanelInformatiiUserApasat);


            cartiPanelInformatiiUserApasat.BackColor = Color.Coral; // Setăm culoarea panelului la roz
            cartiPanelInformatiiLabelTitlu.Text = "Informații Utilizator"; // Textul titlului
            cartiPanelInformatiiLabelTitlu.Font = new Font("Arial", 16, FontStyle.Bold); // Stilul și dimensiunea textului
            cartiPanelInformatiiLabelTitlu.Dock = DockStyle.Top; // Plasarea titlului în partea de sus a panelului
            cartiPanelInformatiiLabelTitlu.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cartiPanelInformatiiLabelTitlu.TextAlign = ContentAlignment.MiddleCenter;


            utilizatorSters = IsAccountDeleted(utilizatorCurentId, connectionString);
            if (utilizatorSters)
            {
                cartiCartiToolStripMenuItem.Visible = false;
                adminCartiToolStripMenuItem.Visible = false;
                adaugaToolStripMenuItem2.Visible = false;
                stergeToolStripMenuItem2.Visible = false;
                cartiPanelCartiApasat.Visible = false;
                cartiPanelUserApasat.Visible = false;
                cartiPanelInformatiiUserButtonModifica.Visible = false;
                cartiPanelInformatiiUserButtonStergeCont.Text = "Recupereaza Contul";

                cartiPanelInformatiiUserApasat.Visible = true;
            }
        }

        private void ConfigurarePanelCentral( Panel panel )
        {
            panel.Anchor = AnchorStyles.None;
            panel.Dock = DockStyle.None;

            // Calculează coordonatele X și Y pentru centrarea panelului
            int x = (ClientSize.Width - panel.Width) / 2;
            int y = (ClientSize.Height - panel.Height + menuStripCarti.Height) / 2;

            // Setează locația panelului în centrul ferestrei
            panel.Location = new Point(x, y);
        }

        private void InitializePanelStates()
        {
            // Inițializează starea panourilor în funcție de numele lor
            panelStates ["cartiPanelCartiApasat"] = false;
            panelStates ["cartiPanelUserApasat"] = false;
            panelStates ["cartiPanelInformatiiUserApasat"] = false;
        }

        private void Carti_FormClosed( object sender, FormClosedEventArgs e )
        {
            Application.Exit();
        }

        private void cartiCartiToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            if (!admin)
            {
                adaugaToolStripMenuItem1.Visible = false;
                stergeToolStripMenuItem1.Visible = false;
            }
        }

        private void userCartiToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            if (!admin)
            {
                adaugaToolStripMenuItem2.Visible = false;
                stergeToolStripMenuItem2.Visible = false;
            }
        }

        private void loginToolStripMenuItem_Click( object sender, EventArgs e )
        {
            Biblioteca obj = (Biblioteca)Application.OpenForms ["Biblioteca"];

            if (obj == null)
            {
                obj = new Biblioteca(); // Creează un nou formular Biblioteca dacă nu există deja
            }

            obj.Show();
            GolestePanelCarti();
            Hide();
        }

        private void cartiCartiToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (cartiPanelCartiButtonCauta.Text.Equals("Modifica"))
            {
                cartiPanelTextBoxNrCopii.Visible = true;
                cartiPanelLabelNrCopii.Visible = true;
            }
            /*   else if (clickedItem == stergeToolStripMenuItem1 || (cartiPanelCartiButtonAdaugasiSterge.Visible == true && clickedItem == cartiCartiToolStripMenuItem))
               {
                   cartiPanelCartiButtonAdaugasiSterge.Visible = true;
                   cartiPanelTextBoxNrCopii.Visible = false;
                   cartiPanelLabelNrCopii.Visible = false;

                   if (clickedItem == stergeToolStripMenuItem1)
                   {
                       cartiPanelCartiButtonAdaugasiSterge.Text = "Sterge  Autor";
                       cartiPanelCartiButtonCauta.Text = "Sterge  Carte";
                   }
               }
               else if (((cartiPanelCartiButtonCauta.Text.Equals("Adauga Carte")) && clickedItem == cartiCartiToolStripMenuItem) || clickedItem == adaugaToolStripMenuItem1)
               {
                   cartiPanelCartiButtonAdaugasiSterge.Text = "Adauga  Autor";
                   cartiPanelCartiButtonCauta.Text = "Adauga Carte";
               }
               else
               {
                   cartiPanelCartiButtonAdaugasiSterge.Visible = false;
                   cartiPanelCartiButtonCauta.Text = "Cauta";
                   cartiPanelTextBoxNrCopii.Visible = false;
                   cartiPanelLabelNrCopii.Visible = false;
               }
               */


            /*
            else
            {
                bool isStergeToolStripMenuItem = clickedItem == stergeToolStripMenuItem1;
                bool isAdaugaSiStergeVisible = cartiPanelCartiButtonAdaugasiSterge.Visible == true && clickedItem == cartiCartiToolStripMenuItem;

                cartiPanelCartiButtonAdaugasiSterge.Visible = isAdaugaSiStergeVisible;
                cartiPanelTextBoxNrCopii.Visible = !isAdaugaSiStergeVisible;
                cartiPanelLabelNrCopii.Visible = !isAdaugaSiStergeVisible;

                if (isStergeToolStripMenuItem)
                {
                    cartiPanelCartiButtonAdaugasiSterge.Text = "Sterge  Autor";
                    cartiPanelCartiButtonCauta.Text = "Sterge  Carte";
                }
                else if ((cartiPanelCartiButtonCauta.Text.Equals("Adauga Carte") && clickedItem == cartiCartiToolStripMenuItem) || clickedItem == adaugaToolStripMenuItem1)
                {
                    cartiPanelCartiButtonAdaugasiSterge.Text = "Adauga  Autor";
                    cartiPanelCartiButtonCauta.Text = "Adauga Carte";
                }
                else
                {
                    cartiPanelCartiButtonAdaugasiSterge.Visible = false;
                    cartiPanelCartiButtonCauta.Text = "Cauta";
                    cartiPanelTextBoxNrCopii.Visible = false;
                    cartiPanelLabelNrCopii.Visible = false;
                }
            }*/
        }

        private void optiuniCartiToolStripMenuItem_Click( object sender, EventArgs e )
        {
            /* cartiPanelComboBoxRolAdmin.Visible = false;
             cartiPanelLabelRolAdmin.Visible = false;
             cartiPanelTextBoxNrCopii.Visible = false;
             cartiPanelLabelNrCopii.Visible = false;
             cartiPanelCartiButtonCauta.Text = "Cauta";*/

            //SetCartiPanelState(cartiPanelTextBoxISBN.ReadOnly, cartiPanelComboBoxEditura.DroppedDown, false, cartiPanelCartiButtonAdaugasiSterge.Visible, cartiPanelCartiButtonCauta.Text, cartiPanelCartiButtonAdaugasiSterge.Text);
        }

        private void imprumutaToolStripMenuItem1_Click( object sender, EventArgs e )
        {
            /*cartiPanelUserApasat.Visible = false;
            cartiPanelCartiApasat.Visible = true;
            cartiPanelTextBoxISBN.ReadOnly = false;
            cartiPanelComboBoxEditura.DropDownStyle = ComboBoxStyle.DropDown;

            cartiPanelTextBoxNrCopii.Visible = false;
            cartiPanelLabelNrCopii.Visible = false;

            cartiPanelCartiButtonGoleste.Visible = true;
            cartiPanelCartiButtonAdaugasiSterge.Visible = true;
            cartiPanelCartiButtonCauta.Text = "Cauta";
            cartiPanelCartiButtonAdaugasiSterge.Text = "Imprumuta";*/
            //SetCartiPanelState(false, true, false, true, "Cauta", "Imprumuta");

            PanelConfiguration panelConfiguration = new PanelConfiguration
            {
                IsNrCopiiVisible = false,
                PanelName = "cartiPanelCartiApasat",
                IsISBNReadOnly = false,
                IsEdituraDropDown = true,
                IsAdaugasiStergeVisible = true,
                CautaButtonText = "Cauta",
                AdaugasiStergeButtonText = "Imprumuta"
            };
            SetPanelState(panelConfiguration);

        }

        private void returneazaToolStripMenuItem1_Click( object sender, EventArgs e )
        {
            /* cartiPanelUserApasat.Visible = false;
             cartiPanelCartiApasat.Visible = true;
             cartiPanelTextBoxISBN.ReadOnly = false;
             cartiPanelComboBoxEditura.DropDownStyle = ComboBoxStyle.DropDown;

             cartiPanelTextBoxNrCopii.Visible = false;
             cartiPanelLabelNrCopii.Visible = false;

             cartiPanelCartiButtonGoleste.Visible = true;
             cartiPanelCartiButtonAdaugasiSterge.Visible = true;
             cartiPanelCartiButtonCauta.Text = "Cauta";
             cartiPanelCartiButtonAdaugasiSterge.Text = "Returneaza";*/

            //SetCartiPanelState(false, true, false, true, "Cauta", "Returneaza");


            PanelConfiguration panelConfiguration = new PanelConfiguration
            {
                IsNrCopiiVisible = false,
                PanelName = "cartiPanelCartiApasat",
                IsISBNReadOnly = false,
                IsEdituraDropDown = true,
                IsAdaugasiStergeVisible = true,
                CautaButtonText = "Cauta",
                AdaugasiStergeButtonText = "Returneaza"
            };

            SetPanelState(panelConfiguration);
            AfisareCartiNereturnate();
        }

        private void adaugaToolStripMenuItem1_Click( object sender, EventArgs e )
        {
            /*cartiPanelUserApasat.Visible = false;
            cartiPanelCartiApasat.Visible = true;
            cartiPanelTextBoxISBN.ReadOnly = false;
            cartiPanelComboBoxEditura.DropDownStyle = ComboBoxStyle.DropDown;
            cartiPanelCartiButtonAdaugasiSterge.Visible = false;
            cartiPanelTextBoxNrCopii.Visible = true;
            cartiPanelLabelNrCopii.Visible = true;
            cartiPanelCartiButtonCauta.Text = "Adauga Carte";*/

            //SetCartiPanelState(false, true, true, false, "Adauga Carte", cartiPanelCartiButtonAdaugasiSterge.Text);


            PanelConfiguration panelConfiguration = new PanelConfiguration
            {
                IsNrCopiiVisible = true,
                PanelName = "cartiPanelCartiApasat",
                IsISBNReadOnly = false,
                IsEdituraDropDown = true,
                IsAdaugasiStergeVisible = false,
                CautaButtonText = "Adauga Carte",
                AdaugasiStergeButtonText = cartiPanelCartiButtonAdaugasiSterge.Text
            };

            SetPanelState(panelConfiguration);
            cartiPanelCartiButtonGoleste.Visible = false;
        }

        private void stergeToolStripMenuItem1_Click( object sender, EventArgs e )
        {
            /*cartiPanelUserApasat.Visible = false;
            cartiPanelCartiApasat.Visible = true;
            cartiPanelTextBoxISBN.ReadOnly = false;
            cartiPanelComboBoxEditura.DropDownStyle = ComboBoxStyle.DropDown;
            cartiPanelCartiButtonAdaugasiSterge.Visible = true;
            cartiPanelTextBoxNrCopii.Visible = false;
            cartiPanelLabelNrCopii.Visible = false;
            cartiPanelCartiButtonGoleste.Visible = true;
            cartiPanelCartiButtonAdaugasiSterge.Visible = true;
            cartiPanelCartiButtonAdaugasiSterge.Text = "Sterge  Autor";
            cartiPanelCartiButtonCauta.Text = "Sterge  Carte";*/

            // SetCartiPanelState(false, true, false, true, "Sterge  Carte", "Sterge  Autor");

            PanelConfiguration panelConfiguration = new PanelConfiguration
            {
                IsNrCopiiVisible = true,
                PanelName = "cartiPanelCartiApasat",
                IsISBNReadOnly = false,
                IsEdituraDropDown = true,
                IsAdaugasiStergeVisible = true,
                CautaButtonText = "Sterge  Carte",
                AdaugasiStergeButtonText = "Sterge  Autor"
            };
            SetPanelState(panelConfiguration);
        }

        private void modificaToolStripMenuItem_Click( object sender, EventArgs e )
        {
            /*cartiPanelUserApasat.Visible = false;
             cartiPanelCartiApasat.Visible = true;

             cartiPanelTextBoxISBN.ReadOnly = true;
             cartiPanelComboBoxEditura.DropDownStyle = ComboBoxStyle.DropDownList;
             cartiPanelTextBoxNrCopii.Visible = true;
             cartiPanelLabelNrCopii.Visible = true;
             cartiPanelCartiButtonAdaugasiSterge.Visible = true;
             cartiPanelCartiButtonGoleste.Visible = true;
             cartiPanelCartiButtonCauta.Text = "Modifica";
             cartiPanelCartiButtonAdaugasiSterge.Text = "Adauga Autor pt Carti";*/

            //SetCartiPanelState(true, false, true, true, "Modifica", "Adauga Autor pt Carti");



            PanelConfiguration panelConfiguration = new PanelConfiguration
            {
                IsNrCopiiVisible = true,
                PanelName = "cartiPanelCartiApasat",
                IsISBNReadOnly = true,
                IsEdituraDropDown = false,
                IsAdaugasiStergeVisible = true,
                CautaButtonText = "Modifica",
                AdaugasiStergeButtonText = "Adauga Autor pt Carti"
            };
            SetPanelState(panelConfiguration);
        }

        private void cartiPanelCartiButtonGoleste_Click( object sender, EventArgs e )
        {
            /*cartiPanelComboBoxNume.Text = "";
            cartiPanelComboBoxPrenume.Text = "";
            cartiPanelTextBoxTitlu.Text = "";
            cartiPanelComboBoxGen.Text = "";
            cartiPanelComboBoxEditura.Text = "";
            cartiPanelTextBoxISBN.Text = "";
            cartiPanelTextBoxNrCopii.Text = "";
            cartiPanelCartiDateTimePickerAnAparitie.Value = DateTime.Today;*/

            GolestePanelCarti();
        }

        private void adaugaToolStripMenuItem2_Click( object sender, EventArgs e )
        {
            GolestePanelCarti();
            Visible = false;
            Biblioteca obj = (Biblioteca)Application.OpenForms ["Biblioteca"];
            obj.Visible = true;
            obj.AdaugaUserFromCartiForm();
            imprumutaToolStripMenuItem1_Click(sender, e);
        }

        private void stergeToolStripMenuItem2_Click( object sender, EventArgs e )
        {
            /*cartiPanelComboBoxRolAdmin.Visible = false;
            cartiPanelLabelRolAdmin.Visible = false;

            cartiPanelUserApasat.Visible = true;
            cartiPanelCartiApasat.Visible = false;
            cartiPanelUserButtonSterge.Text = "Sterge Utilizator";*/

            //SetUserPanelState(false, true, false, "Sterge Utilizator");
            //SetUserPanelState4(false, "cartiPanelUserApasat", "Sterge Utilizator");


            PanelConfiguration panelConfiguration = new PanelConfiguration
            {
                IsComboBoxRolAdminVisible = false,
                PanelName = "cartiPanelUserApasat",
                UserButtonStergeText = "Sterge Utilizator"
            };
            SetPanelState(panelConfiguration);
        }

        private void adaugaToolStripMenuItem3_Click( object sender, EventArgs e )
        {
            /* cartiPanelComboBoxRolAdmin.Visible = true;
             cartiPanelLabelRolAdmin.Visible = true;
             cartiPanelUserApasat.Visible = true;
             cartiPanelCartiApasat.Visible = false;
             cartiPanelUserButtonSterge.Text = "Adauga Admin";*/

            //SetUserPanelState(true, true, false, "Adauga Admin");

            //SetUserPanelState4(true, "cartiPanelUserApasat", "Adauga Admin");


            PanelConfiguration panelConfiguration = new PanelConfiguration
            {
                IsComboBoxRolAdminVisible = true,
                PanelName = "cartiPanelUserApasat",
                UserButtonStergeText = "Adauga Admin"
            };
            SetPanelState(panelConfiguration);

            cartiPanelLabelNumeUser.Text = "Nume Admin";
            cartiPanelLabelPrenumeUser.Text = "Prenume Admin";
        }

        private void stergeToolStripMenuItem3_Click( object sender, EventArgs e )
        {
            /* cartiPanelUserApasat.Visible = true;
             cartiPanelCartiApasat.Visible = false;
             cartiPanelComboBoxRolAdmin.Visible = true;
             cartiPanelLabelRolAdmin.Visible = true;
             cartiPanelUserButtonSterge.Text = "Sterge Admin";*/

            //SetUserPanelState(true, true, false, "Sterge Admin");
            //SetUserPanelState4(true, "cartiPanelUserApasat", "Sterge Admin");

            PanelConfiguration panelConfiguration = new PanelConfiguration
            {
                IsComboBoxRolAdminVisible = true,
                PanelName = "cartiPanelUserApasat",
                UserButtonStergeText = "Sterge Admin"
            };
            SetPanelState(panelConfiguration);

            cartiPanelLabelNumeUser.Text = "Nume Admin";
            cartiPanelLabelPrenumeUser.Text = "Prenume Admin";
        }

        private void informatiiToolStripMenuItem2_Click( object sender, EventArgs e )
        {
            //SetUserPanelState(false, false, false, cartiPanelUserButtonSterge.Text);
            //SetUserPanelState4(false, "cartiPanelInformatiiUserApasat", cartiPanelUserButtonSterge.Text);
            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;

            PanelConfiguration panelConfiguration = new PanelConfiguration
            {
                PanelName = "cartiPanelInformatiiUserApasat",
            };
            SetPanelState(panelConfiguration);


            if (utilizatorSters)
            {
                cartiPanelInformatiiUserButtonStergeCont.Text = "Recupereaza Contul";
            }
            else
                cartiPanelInformatiiUserButtonStergeCont.Text = "Șterge Cont";


            cartiPanelInformatiiTextBoxNumeUser.ReadOnly = true;
            cartiPanelInformatiiTextBoxPrenumeUser.ReadOnly = true;
            cartiPanelInformatiiTextBoxEmailUser.ReadOnly = true;
            cartiPanelInformatiiTextBoxParolaUser.ReadOnly = true;
            cartiPanelInformatiiTextBoxTelefonUser.ReadOnly = true;
            cartiPanelInformatiiTextBoxRolUser.ReadOnly = true;

            cartiPanelInformatiiTextBoxRolUser.Visible = admin;
            cartiPanelInformatiiLabelRolUser.Visible = admin;
        }

        private void cartiPanelTextBoxNumeUser_TextChanged( object sender, EventArgs e )
        {
            if (IsStergeUtilizatorButton() || IsAdaugaAdminButton())
                AdauagainListBoxUserii();
            else if (IsStergeAdminButton())
                AdauagainListBoxAdminii();
        }

        private void cartiPanelTextBoxPrenumeUser_TextChanged( object sender, EventArgs e )
        {
            if (IsStergeUtilizatorButton() || IsAdaugaAdminButton())
                AdauagainListBoxUserii();
            else if (IsStergeAdminButton())
                AdauagainListBoxAdminii();
        }

        private void cartiPanelTextBoxTitlu_TextChanged( object sender, EventArgs e )
        {
            if (IsReturneazaButton())
                AfisareCartiNereturnate();
            else
                AfiseazaCarteinListBox();//out int numarCartiGasite);
        }





        private void cartiPanelComboBoxRolAdmin_DropDown( object sender, EventArgs e )
        {
            string rol = FormatName(cartiPanelComboBoxRolAdmin.Text);
            string query = "SELECT DISTINCT rol FROM Admini WHERE rol LIKE @rol AND rol <> 'Boss'";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@rol", SqlDbType.NVarChar) { Value = rol + '%' }
            };
            PopulateComboBox(cartiPanelComboBoxRolAdmin, query, parameters);



            //string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            /* try
             {
                 using (SqlConnection connection = new SqlConnection(connectionString))
                 using (SqlCommand command = new SqlCommand(query, connection))
                 {
                     command.Parameters.AddWithValue("@rol", rol);

                     connection.Open();

                     using (SqlDataReader reader = command.ExecuteReader())
                     {
                         HashSet<string> uniqueRoles = new HashSet<string>();

                         while (reader.Read())
                         {
                             uniqueRoles.Add(reader ["rol"].ToString());
                         }

                         cartiPanelComboBoxNume.Items.Clear();
                         cartiPanelComboBoxNume.Items.AddRange(uniqueRoles.ToArray());
                     }
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show("A apărut o eroare la căutarea numelor de autori: " + ex.Message);
             }*/
        }

        private void cartiPanelComboBoxNume_DropDown( object sender, EventArgs e )
        {
            string nume = FormatName(cartiPanelComboBoxNume.Text);
            string prenume = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cartiPanelComboBoxPrenume.Text.ToLower().Trim());
            //string query = "SELECT DISTINCT nume FROM Autori WHERE nume LIKE @nume";
            string query = "SELECT DISTINCT nume FROM Autori WHERE nume LIKE @nume AND prenume LIKE @prenume";



            /* List<SqlParameter> parameters = new List<SqlParameter>
             {
                 new SqlParameter("@nume", SqlDbType.NVarChar) { Value = nume + '%' }
             };*/


            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@nume", SqlDbType.NVarChar) { Value = nume + '%'},
                new SqlParameter("@prenume", SqlDbType.NVarChar) { Value = "%" + prenume + "%" }
            };
            PopulateComboBox(cartiPanelComboBoxNume, query, parameters);




            //string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            /* try
             {
                 using (SqlConnection connection = new SqlConnection(connectionString))
                 using (SqlCommand command = new SqlCommand(query, connection))
                 {
                     connection.Open();

                     command.Parameters.AddWithValue("@nume", nume + "%");

                     using (SqlDataReader reader = command.ExecuteReader())
                     {
                         cartiPanelComboBoxNume.Items.Clear();
                         while (reader.Read())
                         {
                             cartiPanelComboBoxNume.Items.Add(reader ["nume"].ToString());
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show("A apărut o eroare la căutarea numelor de autori: " + ex.Message);
             }*/
        }

        private void cartiPanelComboBoxPrenume_DropDown( object sender, EventArgs e )
        {
            string nume = FormatName(cartiPanelComboBoxNume.Text);
            string prenume = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cartiPanelComboBoxPrenume.Text.ToLower().Trim());
            string query = "SELECT DISTINCT prenume FROM Autori WHERE nume LIKE @nume AND prenume LIKE @prenume";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@nume", SqlDbType.NVarChar) { Value = nume + '%'},
                new SqlParameter("@prenume", SqlDbType.NVarChar) { Value =   prenume + "%" }
            };
            PopulateComboBox(cartiPanelComboBoxPrenume, query, parameters);

            //string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            /* try
             {
                 using (SqlConnection connection = new SqlConnection(connectionString))
                 using (SqlCommand command = new SqlCommand(query, connection))
                 {
                     connection.Open();

                     command.Parameters.AddWithValue("@nume", nume);
                     command.Parameters.AddWithValue("@prenume", "%" + prenume + "%");

                     using (SqlDataReader reader = command.ExecuteReader())
                     {
                         cartiPanelComboBoxPrenume.Items.Clear();
                         while (reader.Read())
                         {
                             cartiPanelComboBoxPrenume.Items.Add(reader ["prenume"].ToString());
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show("A apărut o eroare la căutarea prenumelor de autori: " + ex.Message);
             }*/
        }

        private void cartiPanelComboBoxGen_DropDown( object sender, EventArgs e )
        {
            string gen = FormatName(cartiPanelComboBoxGen.Text);
            string query = "SELECT DISTINCT gen FROM Carti WHERE gen LIKE @gen ORDER BY gen ASC";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@gen", SqlDbType.NVarChar) {Value = gen + "%" }
            };
            PopulateComboBox(cartiPanelComboBoxGen, query, parameters);
            //UpdateComboBoxItems(cartiPanelComboBoxGen, "gen", "genurile");
        }

        private void cartiPanelComboBoxEditura_DropDown( object sender, EventArgs e )
        {
            string editura = FormatName(cartiPanelComboBoxEditura.Text);
            string query = "SELECT DISTINCT editura FROM Carti WHERE editura LIKE @editura ORDER BY editura ASC";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@editura", SqlDbType.NVarChar) {Value = editura + "%" }
            };
            PopulateComboBox(cartiPanelComboBoxEditura, query, parameters);
            //UpdateComboBoxItems(cartiPanelComboBoxEditura, "editura", "editurile");
        }

        private void PopulateComboBox( ComboBox comboBox, string query, List<SqlParameter> parameters )
        {
            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            try
            {
                HashSet<string> uniqueItems = new HashSet<string>(); // Folosim un HashSet pentru a evita duplicările

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    foreach (SqlParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0)) // Verificăm dacă valoarea citită nu este null
                            {
                                string itemValue = reader [0].ToString();
                                if (!string.IsNullOrWhiteSpace(itemValue))
                                {
                                    uniqueItems.Add(itemValue);
                                }
                            }
                        }
                    }
                }
                comboBox.Items.Clear();
                comboBox.Items.AddRange(uniqueItems.ToArray()); // Adăugăm elementele unice în ComboBox
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la căutarea datelor: " + ex.Message);
            }
        }


        /* private void UpdateComboBoxItems( ComboBox comboBox, string columnName, string queryFilter )
               {
                   string value = FormatName(comboBox.Text);
                   string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
                   string query = $"SELECT DISTINCT {columnName} FROM Carti WHERE {columnName} LIKE @{columnName} ORDER BY {columnName} ASC";

                   try
                   {
                       using (SqlConnection connection = new SqlConnection(connectionString))
                       using (SqlCommand command = new SqlCommand(query, connection))
                       {
                           connection.Open();
                           command.Parameters.AddWithValue($"@{columnName}", value + "%");
                           using (SqlDataReader reader = command.ExecuteReader())
                           {
                               comboBox.Items.Clear();
                               while (reader.Read())
                               {
                                   string itemValue = reader [columnName]?.ToString();
                                   if (!string.IsNullOrWhiteSpace(itemValue))
                                       comboBox.Items.Add(itemValue);
                               }
                           }
                       }
                   }
                   catch (Exception ex)
                   {
                       MessageBox.Show($"A apărut o eroare la obținerea {queryFilter}: {ex.Message}");
                   }
               }*/







        /* private void SetUserPanelState( bool isComboBoxRolAdminVisible, bool isUserApasatVisible, bool isCartiApasatVisible, string userButtonStergeText )
         {
             GolestePanelCarti();
             cartiPanelComboBoxRolAdmin.Visible = isComboBoxRolAdminVisible;
             cartiPanelLabelRolAdmin.Visible = isComboBoxRolAdminVisible;
             cartiPanelUserButtonSterge.Text = userButtonStergeText;
             cartiPanelLabelNumeUser.Text = "Nume Utilizator";
             cartiPanelLabelPrenumeUser.Text = "Prenume Utilizator";

             cartiPanelUserApasat.Visible = isUserApasatVisible;
             cartiPanelCartiApasat.Visible = isCartiApasatVisible;
         }
         */
        private void SetCartiPanelState( bool isISBNReadOnly, bool isEdituraDropDown, bool isNrCopiiVisible, bool isAdaugasiStergeVisible, string cautaButtonText, string adaugasiStergeButtonText )
        {// Setează vizibilitatea panourilor
            cartiPanelUserApasat.Visible = false;
            cartiPanelInformatiiUserApasat.Visible = false;
            cartiPanelCartiApasat.Visible = true;

            // Setează proprietățile controalelor
            cartiPanelTextBoxISBN.ReadOnly = isISBNReadOnly;
            cartiPanelComboBoxEditura.DropDownStyle = isEdituraDropDown ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
            cartiPanelTextBoxNrCopii.Visible = isNrCopiiVisible;
            cartiPanelLabelNrCopii.Visible = isNrCopiiVisible;
            cartiPanelCartiButtonAdaugasiSterge.Visible = isAdaugasiStergeVisible;
            cartiPanelCartiButtonGoleste.Visible = true;

            // Setează textul butoanelor
            cartiPanelCartiButtonCauta.Text = cautaButtonText;
            cartiPanelCartiButtonAdaugasiSterge.Text = adaugasiStergeButtonText;
        }

        private void SetUserPanelState4( bool isComboBoxRolAdminVisible, string panelName, string userButtonStergeText )
        {
            // Facem o copie a cheilor pentru a evita eroarea de modificare în timpul iterației
            List<string> panelKeys = panelStates.Keys.ToList();

            // Ascunde toate panourile
            foreach (string panelKey in panelKeys)
            {
                panelStates [panelKey] = false;
                Control panel = Controls.Find(panelKey, true).FirstOrDefault() as Control;
                if (panel != null)
                {
                    panel.Visible = false;
                }
            }

            // Afisează panoul specificat și setează starea
            if (panelStates.ContainsKey(panelName))
            {
                panelStates [panelName] = true;
                Control panel = Controls.Find(panelName, true).FirstOrDefault() as Control;
                if (panel != null)
                {
                    panel.Visible = true;
                }
            }

            GolestePanelCarti();
            cartiPanelComboBoxRolAdmin.Visible = isComboBoxRolAdminVisible;
            cartiPanelLabelRolAdmin.Visible = isComboBoxRolAdminVisible;
            cartiPanelUserButtonSterge.Text = userButtonStergeText;
            cartiPanelLabelNumeUser.Text = "Nume Utilizator";
            cartiPanelLabelPrenumeUser.Text = "Prenume Utilizator";
        }

        private void SetPanelState( PanelConfiguration config )
        {

            List<string> panelKeys = panelStates.Keys.ToList();

            // Ascunde toate panourile
            foreach (string panelKey in panelKeys)
            {
                panelStates [panelKey] = false;
                Control panel = Controls.Find(panelKey, true).FirstOrDefault() as Control;
                if (panel != null)
                {
                    panel.Visible = false;
                }
            }

            // Afisează panoul specificat și setează starea
            if (panelStates.ContainsKey(config.PanelName))
            {
                panelStates [config.PanelName] = true;
                Control panel = Controls.Find(config.PanelName, true).FirstOrDefault() as Control;
                if (panel != null)
                {
                    panel.Visible = true;
                }
            }

            // Setează vizibilitatea și proprietățile controalelor pentru panoul de carti                    
            cartiPanelTextBoxISBN.ReadOnly = config.IsISBNReadOnly;
            cartiPanelComboBoxEditura.DropDownStyle = config.IsEdituraDropDown ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
            cartiPanelTextBoxNrCopii.Visible = config.IsNrCopiiVisible;
            cartiPanelLabelNrCopii.Visible = config.IsNrCopiiVisible;
            cartiPanelCartiButtonAdaugasiSterge.Visible = config.IsAdaugasiStergeVisible;
            cartiPanelCartiButtonGoleste.Visible = true;
            cartiPanelCartiButtonCauta.Text = config.CautaButtonText;
            cartiPanelCartiButtonAdaugasiSterge.Text = config.AdaugasiStergeButtonText;

            // Setează vizibilitatea și textul pentru panoul de utilizatori
            cartiPanelComboBoxRolAdmin.Visible = config.IsComboBoxRolAdminVisible;
            cartiPanelLabelRolAdmin.Visible = config.IsComboBoxRolAdminVisible;
            cartiPanelUserButtonSterge.Text = config.UserButtonStergeText;
            cartiPanelLabelNumeUser.Text = "Nume Utilizator";
            cartiPanelLabelPrenumeUser.Text = "Prenume Utilizator";
        }














        private void cartiPanelUserListBox_MouseDoubleClick( object sender, MouseEventArgs e )
        {
            if (e.Button == MouseButtons.Left)
            {
                if (cartiPanelUserListBox.SelectedItem == null)
                {
                    MessageBox.Show("Selecteaza persoana din lista.");
                    return;
                }

                string selectedInfo = cartiPanelUserListBox.SelectedItem.ToString();
                int emailIndex = selectedInfo.IndexOf("Email:");
                int telIndex = selectedInfo.IndexOf("Tel:");
                string numePrenume = selectedInfo.Substring(0, emailIndex).Trim();
                string email = selectedInfo.Substring(emailIndex + 7, telIndex - emailIndex - 7).Trim();
                string telefon = selectedInfo.Substring(telIndex + 5).Trim();

                int separatorIndex = numePrenume.LastIndexOf(" ");
                string nume = numePrenume.Substring(0, separatorIndex).Trim();
                string prenume = numePrenume.Substring(separatorIndex).Trim();

                string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
                int id_user = GetUserID(connectionString, nume, prenume, email, telefon, "telefon");

                if (id_user != -1)
                {
                    DialogResult result = DialogResult.No;
                    string message = string.Empty;
                    if (IsStergeUtilizatorButton())
                    {
                        result = ShowConfirmDialog("ștergi", nume, prenume);
                        message = "Utilizatorul " + nume + " " + prenume + " a fost șters.";
                        if (result == DialogResult.Yes)
                        {
                            StergeUser(id_user, connectionString);
                        }
                    }
                    else if (IsAdaugaAdminButton())
                    {
                        result = ShowConfirmDialog("adaugi ca administrator", nume, prenume);
                        string rol = FormatName(cartiPanelComboBoxRolAdmin.Text);
                        message = "Utilizatorul " + nume + " " + prenume + " a fost adăugat ca " + rol + ".";
                        if (string.IsNullOrWhiteSpace(rol))
                        {
                            MessageBox.Show("Trebuie completat și câmpul rol.");
                            return;
                        }
                        if (result == DialogResult.Yes)
                        {
                            AdaugaAdministrator(id_user, rol, connectionString);
                        }
                    }
                    else if (IsStergeAdminButton())
                    {
                        result = ShowConfirmDialog("ștergi administratorul,", nume, prenume);
                        message = "Administratorul " + nume + " " + prenume + " a fost șters.";
                        if (result == DialogResult.Yes)
                        {
                            StergeAdmin(id_user, connectionString);
                        }
                    }
                    else
                        MessageBox.Show("Actiune nerecunoscuta.");

                    if (result == DialogResult.Yes)
                    {
                        MessageBox.Show(message);
                    }
                    else if (result == DialogResult.No)
                    {
                        MessageBox.Show("Acțiunea a fost anulată.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                    MessageBox.Show("Utilizatorul nu există în baza de date.");
            }
        }

        private void cartiPanelUserButtonSterge_Click( object sender, EventArgs e )
        {
            string numeUser = FormatName(cartiPanelTextBoxNumeUser.Text);
            string prenumeUser = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cartiPanelTextBoxPrenumeUser.Text.ToLower().Trim());

            if (string.IsNullOrWhiteSpace(numeUser) || string.IsNullOrWhiteSpace(prenumeUser))
            {
                MessageBox.Show("Trebuie să completați numele și prenumele utilizatorului pe care doriți să-l ștergeți. Sau faceți dublu clic pe utilizator.");
                return;
            }

            MouseEventArgs mouseArgs = new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0);

            if (IsAdaugaAdminButton())
            {
                AdauagainListBoxUserii();

                if (string.IsNullOrWhiteSpace(cartiPanelComboBoxRolAdmin.Text))
                {
                    MessageBox.Show("Trebuie completat și câmpul rol.");
                    return;
                }
            }

            if (IsStergeUtilizatorButton() || IsAdaugaAdminButton() || IsStergeAdminButton())
            {
                cartiPanelUserListBox_MouseDoubleClick(cartiPanelUserListBox, mouseArgs);
            }
            else
                MessageBox.Show("Actiune nerecunoscuta.");
        }





        private void cartiPanelCartiListBox_MouseDoubleClick( object sender, MouseEventArgs e )
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (cartiPanelCartiListBox.SelectedItem == null)
                return;

            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            string input = cartiPanelCartiListBox.SelectedItem.ToString();
            input = input.Replace("Editura: ", "");
            input = input.Replace("Număr copii: ", "");
            int separatorIndex = input.IndexOf(" - ");
            string titlu = input.Substring(0, separatorIndex).Trim();

            int separatorIndex1 = input.IndexOf(" - ", separatorIndex + 3);
            string autoriCarte = input.Substring(separatorIndex + 3, separatorIndex1 - separatorIndex - 3).Trim();

            // Creăm vectorul de autori
            string [] autori = autoriCarte.Split(new string [] { ", " }, StringSplitOptions.None);

            int separatorIndex2 = input.IndexOf(" - ", separatorIndex1 + 3);
            string edituraCarte = input.Substring(separatorIndex1 + 3, separatorIndex2 - separatorIndex1 - 3).Trim();

            string an_aparitieCarte;
            int separatorIndex3;
            if (admin && !IsReturneazaButton())
                separatorIndex3 = input.IndexOf(" - ", separatorIndex2 + 3);
            else
                separatorIndex3 = input.Length;

            an_aparitieCarte = input.Substring(separatorIndex2 + 3, separatorIndex3 - separatorIndex2 - 3).Trim();

            int idCarte = GetIdCarteByTot(titlu, edituraCarte, an_aparitieCarte, autori, connectionString);

            if (idCarte < 0)
            {
                MessageBox.Show("Cartea nu a fost găsită în baza de date.");
                return;
            }

            string [] informatiiCarte = ExtrageTotFromIdCarte(idCarte, connectionString);

            if (informatiiCarte == null)
                return;


            // Exemplu de completare a combobox-urilor cu informațiile cărți
            //  string [i]=           0        1         2        3               4                     5         6
            //                       17,    SUGE-O,    Horror, Paralela 45, 04.08.2023 12:00:00:AM,   1234,       8,  
            //return new string [] {id_carte, titlu,    gen,    editura,     dataAparitie,             isbn,   numarCopii };

            cartiPanelTextBoxTitlu.Text = informatiiCarte [1];
            cartiPanelComboBoxGen.Text = informatiiCarte [2];
            cartiPanelComboBoxEditura.Text = informatiiCarte [3];
            cartiPanelTextBoxISBN.Text = informatiiCarte [5];
            cartiPanelTextBoxNrCopii.Text = informatiiCarte [6];

            string formatData = "dd.MM.yyyy hh:mm:ss:tt";
            if (DateTime.TryParseExact(informatiiCarte [4], formatData, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataAparitie))
                cartiPanelCartiDateTimePickerAnAparitie.Value = dataAparitie;
            else
            {
                MessageBox.Show("Avem probleme cu formatul datei.");
                return;
            }
            AfiseazaCarteinListBox();//out int numarCartiGasite);
        }

        private void cartiPanelCartiButtonCauta_Click( object sender, EventArgs e )
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            string titlu = cartiPanelTextBoxTitlu.Text.ToUpper().Trim();
            string gen = FormatName(cartiPanelComboBoxGen.Text);
            string editura = FormatName(cartiPanelComboBoxEditura.Text);
            string isbn = cartiPanelTextBoxISBN.Text.Trim();
            DateTime an_aparitie = cartiPanelCartiDateTimePickerAnAparitie.Value;

            string nume = FormatName(cartiPanelComboBoxNume.Text);
            string prenume = textInfo.ToTitleCase(cartiPanelComboBoxPrenume.Text.ToLower().Trim());
            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;


            if (IsAdaugaCarteButton())
            {
                //cauta in baza de date cartea daca exista si afiseaza-l in listbox

                if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume) || string.IsNullOrWhiteSpace(titlu) || string.IsNullOrWhiteSpace(gen) || string.IsNullOrWhiteSpace(isbn) || string.IsNullOrWhiteSpace(editura))
                {
                    MessageBox.Show("Trebuie completate toate câmpurile");
                    return;
                }

                string inputText = cartiPanelTextBoxNrCopii.Text;
                if (!IsValidNrCopii(inputText) || !int.TryParse(inputText, out int NrCopiiDisponibile) || NrCopiiDisponibile <= 0)
                {
                    MessageBox.Show("Introduceți un număr valid și mai mare decât zero pentru numărul de copii disponibile.");
                    return;
                }

                if (AdaugaCarte(titlu, gen, editura, isbn, an_aparitie, NrCopiiDisponibile, nume, prenume, connectionString))
                {
                    MessageBox.Show("Cartea a fost adăugată cu succes în baza de date.");
                    cartiPanelCartiButtonAdaugasiSterge.Visible = true;
                    cartiPanelCartiButtonAdaugasiSterge.Text = "Adauga Autor pt Carti";
                }
            }
            else if (IsModificaCarteButton())
            {
                /*string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume) ||*/
                if (string.IsNullOrWhiteSpace(titlu) || string.IsNullOrWhiteSpace(gen) || string.IsNullOrWhiteSpace(isbn) || string.IsNullOrWhiteSpace(editura))
                {
                    MessageBox.Show("Trebuie completate toate câmpurile. Apasati dublu click pe cartea cautata.");
                    return;
                }

                string inputText = cartiPanelTextBoxNrCopii.Text;
                if (!IsValidNrCopii(inputText) || !int.TryParse(inputText, out int NrCopiiDisponibile) || NrCopiiDisponibile <= 0)
                {
                    MessageBox.Show("Introduceți un număr valid și mai mare decât zero pentru numărul de copii disponibile.");
                    return;
                }


                if (ModificaCarte(gen, isbn, an_aparitie, NrCopiiDisponibile, connectionString))
                {
                    MessageBox.Show("Cartea a fost modificată cu succes în baza de date.");
                }
                // Setarea stilului implicit pentru combobox




                /*if (carteExista)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string selectCarteQuery = "SELECT id_carte, titlu, gen, editura, an_aparitie, isbn, NrCopiiDisponibile FROM Carti WHERE isbn = @isbn";

                            using (SqlCommand selectCarteCommand = new SqlCommand(selectCarteQuery, connection))
                            {
                                selectCarteCommand.Parameters.AddWithValue("@isbn", isbn);
                                using (SqlDataReader reader = selectCarteCommand.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        int idCarte = reader.GetInt32(0);
                                        string titluCurent = reader.GetString(1);
                                        string genCurent = reader.GetString(2);
                                        string edituraCurenta = reader.GetString(3);
                                        DateTime anAparitieCurent = reader.GetDateTime(4);
                                        string isbnCurent = reader.GetString(5);
                                        int nrCopiiDisponibileCurent = reader.GetInt32(6);
                                        reader.Close();

                                        // Acum puteți actualiza valorile cartii cu cele noi
                                        // Dacă nu doriți să modificați o anumită valoare, puteți folosi valoarea curentă citită din baza de date
                                        // De exemplu, dacă titlul sau genul nu se modifică, puteți folosi titluCurent și genCurent în loc de titlu și gen în interogare

                                        string updateCarteQuery = "UPDATE Carti SET titlu = @titlu, gen = @gen, editura = @editura, an_aparitie = @an_aparitie, NrCopiiDisponibile = @NrCopiiDisponibile WHERE id_carte = @idCarte";
                                        using (SqlCommand updateCarteCommand = new SqlCommand(updateCarteQuery, connection))
                                        {
                                            updateCarteCommand.Parameters.AddWithValue("@idCarte", idCarte);
                                            updateCarteCommand.Parameters.AddWithValue("@titlu", titluCurent);
                                            updateCarteCommand.Parameters.AddWithValue("@gen", gen);
                                            updateCarteCommand.Parameters.AddWithValue("@editura", edituraCurenta);
                                            updateCarteCommand.Parameters.AddWithValue("@an_aparitie", an_aparitie);
                                            updateCarteCommand.Parameters.AddWithValue("@NrCopiiDisponibile", NrCopiiDisponibile);

                                            updateCarteCommand.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                        MessageBox.Show("Cartea a fost modificata cu succes în baza de date.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        MessageBox.Show("A apărut o eroare la actualizarea cărții în baza de date: " + ex.Message);
                    }
                }
                else
                    MessageBox.Show("Cartea nu există pentru a putea fi actualizată. Poate ați introdus ISBN-ul greșit.");*/

            }
            else if (IsStergeCarteButton())
            {
                //cauta in baza de date cartea daca exista si afiseaza-l in listbox
                //daca adminul a selectat cartea si a apasat pe buton atunci 
                //apare o ferestra daca il intreaba pe admin daca sigur vrea sa stearga cartea.


                if (string.IsNullOrWhiteSpace(titlu) || string.IsNullOrWhiteSpace(gen) || string.IsNullOrWhiteSpace(editura) || string.IsNullOrWhiteSpace(isbn))
                {
                    MessageBox.Show("Trebuie completate toate câmpurile sau fa dublu click pe cartea cautata.");
                    return;
                }

                int idCarte = GetIdCartebyISBN(isbn, connectionString);
                if (idCarte < 0)
                {
                    MessageBox.Show("Cartea nu exista in baza de date.");
                    return;
                }
                DialogResult result = MessageBox.Show("Ești sigur că vrei să ștergi această carte?\nSigur este singura copie a cartii?", "Atenție", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    StergeCarte(idCarte, connectionString);
                }
                else if (result == DialogResult.No)
                {
                    MessageBox.Show("Ștergerea a fost anulată.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            AfiseazaCarteinListBox();//out int numarCartiGasite);
        }

        private void cartiPanelCartiButtonAdaugasiSterge_Click( object sender, EventArgs e )
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            string isbn = cartiPanelTextBoxISBN.Text.Trim();
            string nume = FormatName(cartiPanelComboBoxNume.Text);
            string prenume = textInfo.ToTitleCase(cartiPanelComboBoxPrenume.Text.ToLower().Trim());

            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            int existingAutoriCount = GetAutorID(nume, prenume, connectionString);

            if ((string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume)) && !IsImprumutButton() && !IsReturneazaButton())
            {
                MessageBox.Show("Trebuie completate câmpurile: nume si prenume");
                return;
            }

            if (IsImprumutButton() || IsReturneazaButton())
            {
                if (string.IsNullOrWhiteSpace(isbn))
                {
                    MessageBox.Show("Faceti dublu click pe carte.");
                    return;
                }

                int idCarte = GetIdCartebyISBN(isbn, connectionString);

                if (idCarte < 0)
                {
                    MessageBox.Show("Nu s-a găsit nicio carte cu acest ISBN.");
                    return;
                }

                if (IsImprumutButton())
                {
                    if (VerificaSiImprumutaCarte(idCarte, utilizatorCurentId, connectionString))
                    {
                        MessageBox.Show("Cartea a fost împrumutată cu succes!");
                    }
                }
                else if (IsReturneazaButton())
                {
                    if (EfectueazaReturneazaCarte(idCarte, utilizatorCurentId, connectionString))
                    {
                        MessageBox.Show("Cartea a fost returnată cu succes!");
                    }
                }
            }
            else if (IsAdaugaAutorButton())
            {
                AdaugaAutor(nume, prenume, connectionString);
                MessageBox.Show("Autorul " + nume + " " + prenume + " a fost adăugat cu succes în baza de date.");
            }
            else if (IsStergeAutorButton())
            {
                if (existingAutoriCount < 0)
                {
                    MessageBox.Show("Autorul nu există în baza de date.");
                    return;
                }

                DialogResult result = MessageBox.Show("Esti sigur ca vrei sa stergi acest autor?\n Autorul o sa fie sters permanent din baza de date si de pe cartile pe care le-a scris!", "Atentie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    StergeAutor(nume, prenume, connectionString);
                else if (result == DialogResult.No)
                    MessageBox.Show("Ștergerea a fost anulată.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (IsAdaugaAutorPtCartiButton())
            {
                string titluCarte = cartiPanelTextBoxTitlu.Text.ToUpper().Trim();
                if (string.IsNullOrWhiteSpace(titluCarte) || string.IsNullOrWhiteSpace(isbn))
                {
                    MessageBox.Show("Trebuie să selectezi o carte înainte de a adăuga autorul.");
                    return;
                }

                if (existingAutoriCount < 0)
                {
                    AdaugaAutor(nume, prenume, connectionString);
                    MessageBox.Show("Autorul " + nume + " " + prenume + " a fost adăugat cu succes în baza de date.");
                }

                int idCarte = GetIdCartebyISBN(isbn, connectionString);
                int idAutor = GetAutorID(nume, prenume, connectionString);

                if (idCarte > 0 && idAutor > 0)
                {
                    if (!CheckIfCarteAutorExists(idCarte, idAutor, connectionString))
                    {
                        AdaugaLegaturaCarteAutor(idCarte, idAutor, connectionString);
                        MessageBox.Show("Legătura dintre carte și autor a fost adăugată cu succes.");
                    }
                    else
                    {
                        MessageBox.Show("Această legătură dintre carte și autor există deja în baza de date.");
                    }
                }
                else
                {
                    MessageBox.Show("Eroare la identificarea cărții sau autorului.");
                }
            }
            else
            {
                MessageBox.Show("Acțiunea nu este recunoscută pentru butonul Adauga Autor.");
            }

            AfiseazaCarteinListBox();//out int numarCartiGasite);
        }






        private bool IsAdaugaCarteButton()
        {
            return cartiPanelCartiButtonCauta.Text.Equals("Adauga Carte", StringComparison.OrdinalIgnoreCase);
        }
        private bool IsModificaCarteButton()
        {
            return cartiPanelCartiButtonCauta.Text.Equals("Modifica", StringComparison.OrdinalIgnoreCase);
        }
        private bool IsStergeCarteButton()
        {
            return cartiPanelCartiButtonCauta.Text.Equals("Sterge  Carte", StringComparison.OrdinalIgnoreCase);
        }
        private bool IsImprumutButton()
        {
            return cartiPanelCartiButtonAdaugasiSterge.Text.Equals("Imprumuta", StringComparison.OrdinalIgnoreCase);
        }
        private bool IsReturneazaButton()
        {
            return cartiPanelCartiButtonAdaugasiSterge.Text.Equals("Returneaza", StringComparison.OrdinalIgnoreCase);
        }
        private bool IsAdaugaAutorButton()
        {
            return cartiPanelCartiButtonAdaugasiSterge.Text.Equals("Adauga  Autor", StringComparison.OrdinalIgnoreCase);
        }
        private bool IsStergeAutorButton()
        {
            return cartiPanelCartiButtonAdaugasiSterge.Text.Equals("Sterge  Autor", StringComparison.OrdinalIgnoreCase);
        }
        private bool IsAdaugaAutorPtCartiButton()
        {
            return cartiPanelCartiButtonAdaugasiSterge.Text.Equals("Adauga Autor pt Carti", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsStergeUtilizatorButton()
        {
            return cartiPanelUserButtonSterge.Text.Equals("Sterge Utilizator", StringComparison.OrdinalIgnoreCase);
        }
        private bool IsAdaugaAdminButton()
        {
            return cartiPanelUserButtonSterge.Text.Equals("Adauga Admin", StringComparison.OrdinalIgnoreCase);
        }
        private bool IsStergeAdminButton()
        {
            return cartiPanelUserButtonSterge.Text.Equals("Sterge Admin", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsModificaUserButton()
        {
            return cartiPanelUserButtonSterge.Text.Equals("Modifica", StringComparison.OrdinalIgnoreCase);
        }




        private string FormatName( string input )
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

        private bool IsValidNrCopii( string copii )
        {
            if (int.TryParse(copii, out int nrCopii))
            {
                return nrCopii > 0;
            }
            return false;
        }

        private void GolestePanelCarti()
        {
            cartiPanelComboBoxNume.Text = "";
            cartiPanelComboBoxPrenume.Text = "";
            cartiPanelTextBoxTitlu.Text = "";
            cartiPanelComboBoxGen.Text = "";
            cartiPanelComboBoxEditura.Text = "";
            cartiPanelTextBoxISBN.Text = "";
            cartiPanelTextBoxNrCopii.Text = "";
            cartiPanelCartiDateTimePickerAnAparitie.Value = DateTime.Today;
            cartiPanelCartiListBox.Items.Clear();
        }

        private DialogResult ShowConfirmDialog( string action, string nume, string prenume )
        {
            return MessageBox.Show(
                $"Ești sigur că vrei să {action} pe {nume} {prenume}?\n Această acțiune poate avea consecințe permanente!",
                "Confirmare",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );
        }




        private int ProcessSqlDataReaderForListBox( ListBox listBox, SqlDataReader reader, string connectionString, bool admin )
       {
            int numarCartiGasite = 0;
            listBox.Items.Clear();
            Dictionary<string, string> cartiEdituraDict = new Dictionary<string, string>();

            while (reader.Read())
            {
                string titluCarte = reader ["titlu"].ToString();
                string edituraCarte = reader ["editura"].ToString();
                DateTime an_Aparitie = (DateTime)reader ["an_aparitie"];
                int nrCopiiDisponibile = (int)reader ["NrCopiiDisponibile"];

                int idCarte = (int)reader ["id_carte"];
                string identificatorCarte = $"{titluCarte} - {edituraCarte}";
                string autoriCarte = GetNumeAutoriFromCarte(idCarte, connectionString);

                if (!string.IsNullOrWhiteSpace(autoriCarte))
                    autoriCarte = autoriCarte.Replace("Anonimus Anonimus", "Anonimus");

                if (!cartiEdituraDict.ContainsKey(identificatorCarte))
                {
                    cartiEdituraDict.Add(identificatorCarte, edituraCarte);
                    string carteSiAutori = $"{titluCarte} - {autoriCarte} - Editura: {edituraCarte} - {an_Aparitie.ToShortDateString()}";
                    if (admin)
                    {
                        carteSiAutori += $" - Număr copii: {nrCopiiDisponibile}";
                    }

                    if (nrCopiiDisponibile > 1 && !IsReturneazaButton())
                    {
                        cartiPanelCartiListBox.Items.Add(carteSiAutori);
                    }
                    numarCartiGasite++;
                }
            }
            return numarCartiGasite;
        }

        private void AfiseazaCarteinListBox()// out int numarCartiGasite )
        {
            //numarCartiGasite = 0;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string titlu = cartiPanelTextBoxTitlu.Text.ToUpper().Trim();
            string gen = FormatName(cartiPanelComboBoxGen.Text);
            string editura = FormatName(cartiPanelComboBoxEditura.Text);
            string isbn = cartiPanelTextBoxISBN.Text.Trim();
            string nume = FormatName(cartiPanelComboBoxNume.Text);
            string prenume = textInfo.ToTitleCase(cartiPanelComboBoxPrenume.Text.ToLower().Trim());
            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;

            string queryString = "SELECT DISTINCT c.*, a.nume, a.prenume, " +
                     "c.NrCopiiDisponibile AS NrCopiiDisponibile " +
                     "FROM Carti c " +
                     "LEFT JOIN Carti_Autori ca ON c.id_carte = ca.id_carte " +
                     "LEFT JOIN Autori a ON ca.id_autor = a.id_autor " +
                     "WHERE 1=1";

            List<SqlParameter> parameters = new List<SqlParameter>();
            queryString = GenerateQueryAndParameters(queryString, parameters, titlu + "%", "titlu", "c.titlu");
            queryString = GenerateQueryAndParameters(queryString, parameters, gen, "gen", "c.gen");
            queryString = GenerateQueryAndParameters(queryString, parameters, editura, "editura", "c.editura");
            queryString = GenerateQueryAndParameters(queryString, parameters, isbn, "isbn", "c.isbn");

            if (cartiPanelCartiDateTimePickerAnAparitie.Value != DateTime.Today)
            {
                queryString += " AND c.an_aparitie = @an_aparitie";
                parameters.Add(new SqlParameter("an_aparitie", cartiPanelCartiDateTimePickerAnAparitie.Value));
            }

            queryString = GenerateQueryAndParameters(queryString, parameters, nume, "nume", "a.nume");
            queryString = GenerateQueryAndParameters(queryString, parameters, prenume, "prenume", "a.prenume");
            queryString += " GROUP BY c.id_carte, c.titlu, c.gen, c.editura, c.an_aparitie, c.isbn, c.NrCopiiDisponibile, a.nume, a.prenume ORDER BY c.titlu";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    command.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        /* numarCartiGasite*/

                        int a = ProcessSqlDataReaderForListBox(cartiPanelCartiListBox, reader, connectionString, admin);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la căutare: " + ex.Message);
            }
        }






        private void AfiseazaCarteinListBoxPrimulBun()//out int numarCartiGasite 
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string titlu = cartiPanelTextBoxTitlu.Text.ToUpper().Trim();

            string gen = FormatName(cartiPanelComboBoxGen.Text);
            string editura = FormatName(cartiPanelComboBoxEditura.Text);
            string isbn = cartiPanelTextBoxISBN.Text.Trim();
            string nume = FormatName(cartiPanelComboBoxNume.Text);
            string prenume = textInfo.ToTitleCase(cartiPanelComboBoxPrenume.Text.ToLower().Trim());
            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;

            string queryString = "SELECT DISTINCT c.*, a.nume, a.prenume, " +
                     "c.NrCopiiDisponibile AS NrCopiiDisponibile " +
                     "FROM Carti c " +
                     "LEFT JOIN Carti_Autori ca ON c.id_carte = ca.id_carte " +
                     "LEFT JOIN Autori a ON ca.id_autor = a.id_autor " +
                     "WHERE 1=1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            // Verificăm câmpurile și adăugăm condițiile numai pentru cele care au valori ne-nule
            if (!string.IsNullOrWhiteSpace(titlu))
            {
                queryString += " AND c.titlu LIKE @titlu";
                parameters.Add(new SqlParameter("@titlu",/* "%" +*/ titlu + "%"));
            }

            if (!string.IsNullOrWhiteSpace(gen))
            {
                queryString += " AND c.gen = @gen";
                parameters.Add(new SqlParameter("@gen", gen));
            }

            if (!string.IsNullOrWhiteSpace(editura))
            {
                queryString += " AND c.editura = @editura";
                parameters.Add(new SqlParameter("@editura", editura));
            }

            if (!string.IsNullOrWhiteSpace(isbn))
            {
                queryString += " AND c.isbn = @isbn";
                parameters.Add(new SqlParameter("isbn", isbn));
            }

            if (cartiPanelCartiDateTimePickerAnAparitie.Value != DateTime.Today)
            {
                queryString += " AND c.an_aparitie = @an_aparitie";
                parameters.Add(new SqlParameter("an_aparitie", cartiPanelCartiDateTimePickerAnAparitie.Value));
            }

            if (!string.IsNullOrWhiteSpace(nume))
            {
                queryString += " AND a.nume = @nume";
                parameters.Add(new SqlParameter("@nume", nume));
            }

            if (!string.IsNullOrWhiteSpace(prenume))
            {
                queryString += " AND a.prenume = @prenume";
                parameters.Add(new SqlParameter("@prenume", prenume));
            }

            queryString += " GROUP BY c.id_carte, c.titlu, c.gen, c.editura, c.an_aparitie, c.isbn, c.NrCopiiDisponibile, a.nume, a.prenume ORDER BY c.titlu";

            // numarCartiGasite = 0;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            cartiPanelCartiListBox.Items.Clear();
                            //HashSet<string> autoriSetptCarte = new HashSet<string>();
                            Dictionary<string, string> cartiEdituraDict = new Dictionary<string, string>();

                            while (reader.Read())
                            {
                                string titluCarte = reader ["titlu"].ToString();
                                string edituraCarte = reader ["editura"].ToString();
                                DateTime an_Aparitie = (DateTime)reader ["an_aparitie"];
                                int nrCopiiDisponibile = (int)reader ["NrCopiiDisponibile"];

                                // Obțineți id-ul cărții curente pentru a căuta toți autorii asociați ei
                                int idCarte = (int)reader ["id_carte"];
                                string identificatorCarte = $"{titluCarte} - {edituraCarte}";
                                string autoriCarte = GetNumeAutoriFromCarte(idCarte, connectionString);

                                if (!string.IsNullOrWhiteSpace(autoriCarte))
                                    autoriCarte = autoriCarte.Replace("Anonimus Anonimus", "Anonimus");

                                // Verificați dacă autorul există deja în setul de autori                                                             
                                //if (!autoriSetptCarte.Contains(autoriCarte))
                                if (!cartiEdituraDict.ContainsKey(identificatorCarte))
                                {
                                    //autoriSetptCarte.Add(autoriCarte);
                                    cartiEdituraDict.Add(identificatorCarte, edituraCarte);

                                    string carteSiAutori = $"{titluCarte} - {autoriCarte} - Editura: {edituraCarte} - {an_Aparitie.ToShortDateString()}";

                                    if (admin)
                                    {
                                        carteSiAutori += $" - Număr copii: {nrCopiiDisponibile}";
                                    }

                                    if (nrCopiiDisponibile > 1 && !IsReturneazaButton())
                                    {
                                        cartiPanelCartiListBox.Items.Add(carteSiAutori);
                                    }
                                    //numarCartiGasite++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la căutare: " + ex.Message);
                }
            }
        }

        private void AfisareCartiNereturnate()
        {
            cartiPanelCartiListBox.Items.Clear();
            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT DISTINCT C.*, A.Nume, A.Prenume
                        FROM Imprumuturi I
                        JOIN Carti C ON I.id_carte = C.id_carte
                        LEFT JOIN Carti_Autori ca ON C.id_carte = ca.id_carte 
                        LEFT JOIN Autori A ON ca.id_autor = A.id_autor
                        WHERE I.returnat = 0 and I.id_utilizator=@userId ORDER BY c.titlu";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", utilizatorCurentId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Dictionary<string, string> cartiEdituraDict = new Dictionary<string, string>();

                            while (reader.Read())
                            {
                                string titluCarte = reader ["titlu"].ToString();
                                string edituraCarte = reader ["editura"].ToString();
                                DateTime an_Aparitie = (DateTime)reader ["an_aparitie"];

                                // Obțineți id-ul cărții curente pentru a căuta toți autorii asociați ei
                                int idCarte = (int)reader ["id_carte"];
                                string identificatorCarte = $"{titluCarte} - {edituraCarte}";
                                string autoriCarte = GetNumeAutoriFromCarte(idCarte, connectionString);

                                if (!string.IsNullOrWhiteSpace(autoriCarte))
                                    autoriCarte = autoriCarte.Replace("Anonimus Anonimus", "Anonimus");

                                // Verificați dacă autorul există deja în setul de autori  
                                if (!cartiEdituraDict.ContainsKey(identificatorCarte))
                                {
                                    cartiEdituraDict.Add(identificatorCarte, edituraCarte);
                                    string carteSiAutori = $"{titluCarte} - {autoriCarte} - Editura: {edituraCarte} - {an_Aparitie.ToShortDateString()}";
                                    cartiPanelCartiListBox.Items.Add(carteSiAutori);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la căutare: " + ex.Message);
            }
        }

        private int GetUserID( string connectionString, string nume, string prenume, string email, string value, string column )
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = $"SELECT id_utilizator FROM Utilizatori WHERE nume = @nume AND prenume = @prenume AND email = @email";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nume", nume);
                        command.Parameters.AddWithValue("@prenume", prenume);
                        command.Parameters.AddWithValue("@email", email);

                        if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(value))
                        {
                            query += $" AND {column} = @value";
                            command.Parameters.AddWithValue("@value", value);
                        }

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int idUtilizator))
                        {
                            return idUtilizator;
                        }
                        else
                        {
                            MessageBox.Show("Autentificare eșuată. Verificați datele de autentificare sau nu v-ați conectat încă.");
                            return -1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la autentificare: " + ex.Message);
                    return -1;
                }
            }
        }

        private void StergeUser( int id_User, string connectionString )
        {
            if (id_User < 0)
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //string deleteQuery = "DELETE FROM Utilizatori WHERE nume=@nume AND prenume=@prenume AND email=@email";
                    string deleteQuery = "DELETE FROM Utilizatori WHERE id_utilizator = @id_User AND id_utilizator <> @utilizatorCurentId";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        // deleteCommand.Parameters.AddWithValue("@nume", nume);
                        //deleteCommand.Parameters.AddWithValue("@prenume", prenume);
                        //deleteCommand.Parameters.AddWithValue("@email", email);
                        deleteCommand.Parameters.AddWithValue("@id_User", id_User);
                        deleteCommand.Parameters.AddWithValue("@utilizatorCurentId", utilizatorCurentId);
                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Actualizează lista de utilizatori
                            AdauagainListBoxUserii();
                        }
                        else
                        {
                            MessageBox.Show("Nu s-a putut șterge utilizatorul.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la ștergerea utilizatorului: " + ex.Message + ".Incercati din nou.");
            }
        }

        private bool VerificaAdmin( int userId, string connectionString )
        {
            if (userId < 0)
            {
                MessageBox.Show("User incorect.");
                return false;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT 1 FROM Admini WHERE id_utilizator = @id_utilizator";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_utilizator", userId);

                        return command.ExecuteScalar() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la autentificare: " + ex.Message);
                return false;
            }

        }

        private string GetNumeAutoriFromCarte( int idCarte, string connectionString )
        {
            if (idCarte < 0)
            {
                MessageBox.Show("Cartea sau autor incorect.");
                return null;
            }
            string queryStringAutori = "SELECT nume, prenume FROM Autori WHERE id_autor IN (SELECT id_autor FROM Carti_Autori WHERE id_carte = @id_carte)";
            using (SqlConnection connectionAutori = new SqlConnection(connectionString))
            {
                connectionAutori.Open();
                using (SqlCommand commandAutori = new SqlCommand(queryStringAutori, connectionAutori))
                {
                    commandAutori.Parameters.AddWithValue("@id_carte", idCarte);
                    using (SqlDataReader readerAutori = commandAutori.ExecuteReader())
                    {
                        // Folosim expresii lambda pentru a citi numele autorilor și a-i concatena într-un șir de caractere
                        List<string> autoriList = new List<string>();
                        while (readerAutori.Read())
                        {
                            string numeAutor = readerAutori ["nume"].ToString();
                            string prenumeAutor = readerAutori ["prenume"].ToString();
                            autoriList.Add($"{numeAutor} {prenumeAutor}");
                        }

                        return string.Join(", ", autoriList);
                    }
                }
            }
        }

        private int GetAutorID( string nume, string prenume, string connectionString )
        {
            int idAutor = -1;
            nume = nume.Trim();
            prenume = prenume.Trim();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id_autor FROM Autori WHERE nume = @nume AND prenume = @prenume";
                    //string query = "SELECT TOP 1 id_autor FROM Autori WHERE nume = @nume AND prenume = @prenume";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nume", nume);
                        command.Parameters.AddWithValue("@prenume", prenume);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            idAutor = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la obținerea informațiilor despre autor: " + ex.Message);
            }
            return idAutor;
        }

        private void AdaugaAutor( string nume, string prenume, string connectionString )
        {
            int existingAutorId = GetAutorID(nume, prenume, connectionString);
            if (existingAutorId > 0)
            {
                MessageBox.Show("Autorul există deja în baza de date.");
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO Autori (nume, prenume) VALUES (@nume, @prenume)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@nume", nume);
                        insertCommand.Parameters.AddWithValue("@prenume", prenume);
                        insertCommand.ExecuteNonQuery();
                    }
                }
                //MessageBox.Show("Autorul " + nume + " " + prenume + " a fost adăugat cu succes în baza de date.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la inserarea în baza de date: " + ex.Message);
            }
        }

        private void StergeAutor( string nume, string prenume, string connectionString )
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Ștergem legăturile dintre autor și cărți din tabela "Carti_Autori"
                    string deleteLegaturaQuery = "DELETE FROM Carti_Autori WHERE id_autor IN (SELECT id_autor FROM Autori WHERE nume = @nume AND prenume = @prenume)";
                    using (SqlCommand deleteLegaturaCommand = new SqlCommand(deleteLegaturaQuery, connection))
                    {
                        deleteLegaturaCommand.Parameters.AddWithValue("@nume", nume);
                        deleteLegaturaCommand.Parameters.AddWithValue("@prenume", prenume);
                        int rowsAffected = deleteLegaturaCommand.ExecuteNonQuery();
                        //Console.WriteLine("Rows affected in Carti_Autori: " + rowsAffected); // Adăugați acest mesaj pentru debug
                    }

                    // Ștergem autorul din tabela "Autori"
                    string deleteAutorQuery = "DELETE FROM Autori WHERE nume = @nume AND prenume = @prenume";
                    using (SqlCommand deleteAutorCommand = new SqlCommand(deleteAutorQuery, connection))
                    {
                        deleteAutorCommand.Parameters.AddWithValue("@nume", nume);
                        deleteAutorCommand.Parameters.AddWithValue("@prenume", prenume);
                        int rowsAffected = deleteAutorCommand.ExecuteNonQuery();
                        //Console.WriteLine("Rows affected in Autori: " + rowsAffected); // Adăugați acest mesaj pentru debug
                    }

                    MessageBox.Show("Autorul " + nume + " " + prenume + " a fost șters cu succes din baza de date.");
                    cartiPanelComboBoxNume.Text = "";
                    cartiPanelComboBoxPrenume.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la ștergerea autorului din baza de date: " + ex.Message);
            }
        }

        private bool CheckIfCarteAutorExists( int idCarte, int idAutor, string connectionString )
        {
            if (idCarte < 0 || idAutor < 0)
            {
                MessageBox.Show("Cartea sau autor incorect.");
                return false;
            }

            bool exists = false;
            string checkQuery = "SELECT COUNT(*) FROM Carti_Autori WHERE id_carte = @id_carte AND id_autor = @id_autor";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
            {
                try
                {
                    connection.Open();
                    checkCommand.Parameters.AddWithValue("@id_carte", idCarte);
                    checkCommand.Parameters.AddWithValue("@id_autor", idAutor);
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                    exists = (count > 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la verificarea legăturii dintre carte și autor: " + ex.Message);
                }
            }
            return exists;
        }

        private void AdaugaLegaturaCarteAutor( int idCarte, int idAutor, string connectionString )
        {
            if (idCarte < 0 || idAutor < 0)
            {
                MessageBox.Show("Cartea sau autor incorect.");
                return;
            }
            string insertLegaturaQuery = "INSERT INTO Carti_Autori (id_carte, id_autor) VALUES (@id_carte, @id_autor)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand insertLegaturaCommand = new SqlCommand(insertLegaturaQuery, connection))
            {
                try
                {
                    connection.Open();
                    insertLegaturaCommand.Parameters.AddWithValue("@id_carte", idCarte);
                    insertLegaturaCommand.Parameters.AddWithValue("@id_autor", idAutor);
                    insertLegaturaCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la adăugarea legăturii dintre carte și autor: " + ex.Message);
                }
            }
        }

        private bool AdaugaCarte( string titlu, string gen, string editura, string isbn, DateTime an_aparitie, int NrCopiiDisponibile, string nume, string prenume, string connectionString )
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    bool carteExista = VerificaExistentaCarte(titlu, gen, editura, isbn, connectionString);
                    if (carteExista)
                    {
                        MessageBox.Show("Cartea există deja în baza de date.");
                        transaction.Rollback();
                        return false;
                    }

                    int idAutor = GetAutorID(nume, prenume, connectionString);
                    if (idAutor < 0)
                    {
                        AdaugaAutor(nume, prenume, connectionString);
                        idAutor = GetAutorID(nume, prenume, connectionString);
                    }

                    string insertCarteQuery = "INSERT INTO Carti (titlu, gen, editura, an_aparitie, isbn, NrCopiiDisponibile) VALUES (@titlu, @gen, @editura, @an_aparitie, @isbn, @NrCopiiDisponibile); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand insertCarteCommand = new SqlCommand(insertCarteQuery, connection, transaction))
                    {
                        insertCarteCommand.Parameters.AddWithValue("@titlu", titlu);
                        insertCarteCommand.Parameters.AddWithValue("@gen", gen);
                        insertCarteCommand.Parameters.AddWithValue("@editura", editura);
                        insertCarteCommand.Parameters.AddWithValue("@isbn", isbn);
                        insertCarteCommand.Parameters.AddWithValue("@an_aparitie", an_aparitie);
                        insertCarteCommand.Parameters.AddWithValue("@NrCopiiDisponibile", NrCopiiDisponibile);

                        int idCarte = Convert.ToInt32(insertCarteCommand.ExecuteScalar());

                        AdaugaLegaturaCarteAutor(idCarte, idAutor, connectionString);

                        transaction.Commit();
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("A aparut o eroare la adaugarea cartii:" + ex);
                    return false;
                }
            }
        }

        private bool ModificaCarte( string gen, string isbn, DateTime an_aparitie, int NrCopiiDisponibile, string connectionString )
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string updateCarteQuery = "UPDATE Carti SET gen = @gen, an_aparitie = @an_aparitie, NrCopiiDisponibile = @NrCopiiDisponibile WHERE isbn = @isbn";

                    using (SqlCommand updateCarteCommand = new SqlCommand(updateCarteQuery, connection))
                    {
                        updateCarteCommand.Parameters.AddWithValue("@gen", gen);
                        updateCarteCommand.Parameters.AddWithValue("@an_aparitie", an_aparitie);
                        updateCarteCommand.Parameters.AddWithValue("@NrCopiiDisponibile", NrCopiiDisponibile);
                        updateCarteCommand.Parameters.AddWithValue("@isbn", isbn);

                        int rowsAffected = updateCarteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Cartea nu există pentru a putea fi actualizată.");
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("A aparut o eroare la modificarea cartii: " + ex.Message);
                return false;
            }
        }

        private void StergeCarte( int idCarte, string connectionString )
        {
            if (idCarte < 0)
            {
                MessageBox.Show("Cartea nu exista.");
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string deleteLegaturaQuery = "DELETE FROM Carti_Autori WHERE id_carte = @idCarte";
                    string deleteCarteQuery = "DELETE FROM Carti WHERE id_carte = @idCarte";

                    using (SqlCommand deleteLegaturaCommand = new SqlCommand(deleteLegaturaQuery, connection))
                    using (SqlCommand deleteCarteCommand = new SqlCommand(deleteCarteQuery, connection))
                    {
                        deleteLegaturaCommand.Parameters.AddWithValue("@idCarte", idCarte);
                        deleteCarteCommand.Parameters.AddWithValue("@idCarte", idCarte);

                        // Ștergeți legătura dintre carte și autor în tabelul "Carti_Autori"
                        deleteLegaturaCommand.ExecuteNonQuery();

                        // Ștergeți cartea din tabelul "Carti"
                        deleteCarteCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cartea a fost ștearsă cu succes din baza de date.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la ștergerea cărții din baza de date: " + ex.Message);
            }
        }

        private bool VerificaExistentaCarte( string titlu, string gen, string editura, string isbn, string connectionString )
        {
            bool carteExista = false;

            string queryByISBN = "SELECT COUNT(*) FROM Carti WHERE isbn = @isbn";
            // string queryByTitluGenEditura = "SELECT COUNT(*) FROM Carti WHERE titlu = @titlu AND gen = @gen AND editura = @editura";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand commandISBN = new SqlCommand(queryByISBN, connection))
                    //using (SqlCommand commandTitluGenEditura = new SqlCommand(queryByTitluGenEditura, connection))
                    {
                        commandISBN.Parameters.AddWithValue("@isbn", isbn);
                        //commandTitluGenEditura.Parameters.AddWithValue("@titlu", titlu);
                        //commandTitluGenEditura.Parameters.AddWithValue("@gen", gen);
                        //commandTitluGenEditura.Parameters.AddWithValue("@editura", editura);

                        int numarCartiCuISBN = Convert.ToInt32(commandISBN.ExecuteScalar());
                        //int numarCartiCuTitluGenEditura = Convert.ToInt32(commandTitluGenEditura.ExecuteScalar());


                        if (numarCartiCuISBN > 0)//|| numarCartiCuTitluGenEditura > 0)
                            carteExista = true;
                        else
                            carteExista = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la verificarea cărții în baza de date: " + ex.Message);
                }
            }

            return carteExista;
        }

        private string [] ExtrageTotFromIdCarte( int idCarte, string connectionString )
        {
            if (idCarte < 0)
            {
                MessageBox.Show("Nu s-au putut obține informațiile despre carte.");
                return null;
            }
            string query = "SELECT * FROM Carti WHERE id_carte = @idCarte";
            //try
            //{


            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@idCarte", idCarte);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int numarColoane = reader.FieldCount;
                        string [] informatiiCarte = new string [numarColoane];
                        for (int i = 0; i < numarColoane; i++)
                        {
                            informatiiCarte [i] = reader [i]?.ToString();
                        }
                        return informatiiCarte;
                    }
                }
            }


            /* }
             catch (Exception ex)
             {
                 MessageBox.Show("A apărut o eroare la extragerea informațiilor cărții: " + ex.Message);
             }*/
            return null;
        }

        private int GetIdCarteByTitlu( string titlu, string connectionString )
        {
            titlu = titlu.Trim();
            int idCarte = -1;
            string query = "SELECT id_carte FROM Carti WHERE titlu = @titlu";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@titlu", titlu);
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        idCarte = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la obținerea informațiilor despre carte: " + ex.Message);
                }
            }
            return idCarte;
        }

        private int GetIdCartebyISBN( string isbn, string connectionString )
        {
            int idCarte = -1;
            isbn = isbn.Trim();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id_carte FROM Carti WHERE isbn = @isbn";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@isbn", isbn);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            idCarte = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la obținerea informațiilor despre carte: " + ex.Message);
            }
            return idCarte;
        }

        private int GetIdCarteByTot( string titlu, string edituraCarte, string an_aparitieCarte, string [] autori, string connectionString )
        {
            int id_carte = -1;
            string [] nume = new string [autori.Length];
            string [] prenume = new string [autori.Length];

            //Console.WriteLine(autori.Length);
            for (int i = 0; i < autori.Length; i++)
            {
                int separatorIndex = autori [i].IndexOf(" ");
                if (separatorIndex == -1)
                {
                    prenume [i] = "Anonimus";
                    nume [i] = "Anonimus";
                }
                else
                {
                    nume [i] = autori [i].Substring(0, separatorIndex).Trim();
                    prenume [i] = autori [i].Substring(separatorIndex + 1).Trim();
                }
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectCarteQuery = @"
                        SELECT c.id_carte
                        FROM Carti c
                        INNER JOIN Carti_Autori ca ON c.id_carte = ca.id_carte
                        INNER JOIN Autori a ON ca.id_autor = a.id_autor
                        WHERE c.titlu = @titlu AND c.editura = @editura AND c.an_aparitie = @an_aparitie";

                    for (int i = 0; i < nume.Length; i++)
                    {
                        selectCarteQuery += $" AND EXISTS (SELECT 1 FROM Carti_Autori ca{i} INNER JOIN Autori a{i} ON ca{i}.id_autor = a{i}.id_autor WHERE ca{i}.id_carte = c.id_carte AND a{i}.nume = @nume{i} AND a{i}.prenume = @prenume{i})";
                    }

                    using (SqlCommand selectCarteCommand = new SqlCommand(selectCarteQuery, connection))
                    {
                        selectCarteCommand.Parameters.AddWithValue("@titlu", titlu);
                        selectCarteCommand.Parameters.AddWithValue("@editura", edituraCarte);
                        // if (!DateTime.TryParseExact(an_aparitieCarte, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out anAparitieDate))

                        DateTime anAparitieDate;
                        if (!DateTime.TryParseExact(an_aparitieCarte, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out anAparitieDate))
                        {
                            throw new Exception("Eroare la convertirea datei la dublu click.");
                        }

                        selectCarteCommand.Parameters.AddWithValue("@an_aparitie", anAparitieDate.Date);

                        for (int i = 0; i < nume.Length; i++)
                        {
                            selectCarteCommand.Parameters.AddWithValue($"@nume{i}", nume [i]);
                            selectCarteCommand.Parameters.AddWithValue($"@prenume{i}", prenume [i]);
                        }

                        object result = selectCarteCommand.ExecuteScalar();
                        //Console.WriteLine(result);
                        if (result != null)
                        {
                            id_carte = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la căutarea cărții în baza de date: " + ex.Message);
            }
            return id_carte;
        }

        private bool IsCarteDisponibila( int idCarte, string connectionString )
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectStareQuery = "SELECT COUNT(*) FROM Carti WHERE id_carte = @idCarte AND NrCopiiDisponibile > 1";

                using (SqlCommand selectStareCommand = new SqlCommand(selectStareQuery, connection))
                {
                    selectStareCommand.Parameters.AddWithValue("@idCarte", idCarte);
                    int availableCount = Convert.ToInt32(selectStareCommand.ExecuteScalar());

                    return availableCount > 0;
                }
            }
        }

        private bool EfectueazaImprumutCarte( int idUtilizator, int idCarte, string connectionString )
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        //string verificareQuery = "SELECT COUNT(*) FROM Imprumuturi WHERE id_utilizator = @idUtilizator AND id_carte = @idCarte and( returnat = 0 )";

                        string verificareQuery = "SELECT COUNT(*) FROM Imprumuturi WHERE id_utilizator = @idUtilizator AND id_carte = @idCarte AND (returnat = 0 OR (data_returnare <= GETDATE() AND DATEDIFF(DAY, data_imprumut, GETDATE()) > 0))";


                        using (SqlCommand verificareCommand = new SqlCommand(verificareQuery, connection, transaction))
                        {
                            verificareCommand.Parameters.AddWithValue("@idUtilizator", idUtilizator);
                            verificareCommand.Parameters.AddWithValue("@idCarte", idCarte);
                            int rezultatVerificare = Convert.ToInt32(verificareCommand.ExecuteScalar());

                            if (rezultatVerificare > 0)
                            {
                                if (rezultatVerificare == 1)
                                {
                                    MessageBox.Show("Nu puteți împrumuta o carte până când nu returnați copia anterioară.");
                                }
                                else
                                {
                                    MessageBox.Show("Nu puteți împrumuta o nouă carte până când returnați cărțile cu data de returnare expirată.");
                                }
                                return false;
                            }
                        }
                        string insertImprumutQuery = "INSERT INTO Imprumuturi (id_utilizator, id_carte, data_imprumut, data_returnare, returnat) VALUES (@idUtilizator, @idCarte, GETDATE(), DATEADD(week, 2, GETDATE()), 0)";

                        using (SqlCommand insertImprumutCommand = new SqlCommand(insertImprumutQuery, connection, transaction))
                        {
                            insertImprumutCommand.Parameters.AddWithValue("@idUtilizator", idUtilizator);
                            insertImprumutCommand.Parameters.AddWithValue("@idCarte", idCarte);
                            insertImprumutCommand.ExecuteNonQuery();
                        }

                        string updateStareCarteQuery = "UPDATE Carti SET NrCopiiDisponibile = NrCopiiDisponibile - 1 WHERE id_carte = @idCarte AND NrCopiiDisponibile > 0";
                        using (SqlCommand updateStareCarteCommand = new SqlCommand(updateStareCarteQuery, connection, transaction))
                        {
                            updateStareCarteCommand.Parameters.AddWithValue("@idCarte", idCarte);
                            int rowsAffected = updateStareCarteCommand.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                transaction.Rollback();
                                MessageBox.Show("Cartea nu este disponibilă pentru împrumut sau a apărut o eroare la împrumut.");
                                return false;
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("A apărut o eroare la împrumutul cărții: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        private bool VerificaSiImprumutaCarte( int idCarte, int idUtilizator, string connectionString )
        {
            if (idUtilizator < 0 || idCarte < 0)
            {
                MessageBox.Show("Cartea sau utilizatorul nu exista.");
                return false;
            }
            if (!IsCarteDisponibila(idCarte, connectionString))
            {
                MessageBox.Show("Cartea nu este disponibilă pentru împrumut.");
                return false;
            }

            return EfectueazaImprumutCarte(idUtilizator, idCarte, connectionString);
        }

        private bool EfectueazaReturneazaCarte( int idCarte, int idUtilizator, string connectionString )
        {
            if (idUtilizator < 0 || idCarte < 0)
            {
                MessageBox.Show("Cartea nu a fost împrumutată sau a apărut o eroare la returnare.");
                return false;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string returnareQuery = "UPDATE Imprumuturi SET data_returnare = GETDATE(), returnat = 1 WHERE id_utilizator = @idUtilizator AND id_carte = @idCarte AND returnat = 0";
                        using (SqlCommand returnareCommand = new SqlCommand(returnareQuery, connection, transaction))
                        {
                            returnareCommand.Parameters.AddWithValue("@idUtilizator", idUtilizator);
                            returnareCommand.Parameters.AddWithValue("@idCarte", idCarte);
                            int rowsAffected = returnareCommand.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                MessageBox.Show("Cartea nu a fost împrumutată sau a apărut o eroare la returnare.");
                                return false;
                            }
                        }

                        string updateStareCarteQuery = "UPDATE Carti SET NrCopiiDisponibile = NrCopiiDisponibile + 1 WHERE id_carte = @idCarte";
                        using (SqlCommand updateStareCarteCommand = new SqlCommand(updateStareCarteQuery, connection, transaction))
                        {
                            updateStareCarteCommand.Parameters.AddWithValue("@idCarte", idCarte);
                            int rowsAffected = updateStareCarteCommand.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                transaction.Rollback();
                                MessageBox.Show("Eroare la actualizarea stării cărții sau a apărut o eroare la returnare.");
                                return false;
                            }
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("A apărut o eroare la returnarea cărții: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        private void AdaugaInListBox( SqlConnection connection, string query, SqlParameter [] parameters )
        {
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        cartiPanelUserListBox.Items.Clear();
                        while (reader.Read())
                        {
                            string nume = reader ["nume"].ToString();
                            string prenume = reader ["prenume"].ToString();
                            string email = reader ["email"].ToString();
                            string telefon = reader ["telefon"].ToString();
                            string info = $"{nume} {prenume}     Email: {email} Tel: {telefon}";
                            cartiPanelUserListBox.Items.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare: " + ex.Message);
            }
        }

        private void AdauagainListBoxAdminii()
        {
            string numeUser = FormatName(cartiPanelTextBoxNumeUser.Text);
            string prenumeUser = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cartiPanelTextBoxPrenumeUser.Text.ToLower().Trim());
            string rol = FormatName(cartiPanelComboBoxRolAdmin.Text);

            if (string.IsNullOrWhiteSpace(numeUser) && string.IsNullOrWhiteSpace(prenumeUser) && string.IsNullOrWhiteSpace(rol))
            {
                cartiPanelUserListBox.Items.Clear();
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT U.nume, U.prenume, U.email, U.telefon, A.rol " +
                               "FROM Utilizatori U " +
                               "INNER JOIN Admini A ON U.id_utilizator = A.id_utilizator " +
                               "WHERE 1=1 AND A.id_utilizator <> @userId ";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@userId", utilizatorCurentId)
                };

                if (!string.IsNullOrWhiteSpace(numeUser))
                {
                    query += "AND U.nume LIKE @numeUser ";
                    parameters.Add(new SqlParameter("@numeUser", $"%{numeUser}%"));
                }

                if (!string.IsNullOrWhiteSpace(prenumeUser))
                {
                    query += "AND U.prenume LIKE @prenumeUser ";
                    parameters.Add(new SqlParameter("@prenumeUser", $"%{prenumeUser}%"));
                }

                if (!string.IsNullOrWhiteSpace(rol))
                {
                    query += "AND A.rol LIKE @rol ";
                    parameters.Add(new SqlParameter("@rol", $"%{rol}%"));
                }

                AdaugaInListBox(connection, query, parameters.ToArray());
            }
        }

        private void AdauagainListBoxUserii()
        {
            string numeUser = FormatName(cartiPanelTextBoxNumeUser.Text);
            string prenumeUser = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cartiPanelTextBoxPrenumeUser.Text.ToLower().Trim());

            if (string.IsNullOrWhiteSpace(numeUser) && string.IsNullOrWhiteSpace(prenumeUser))
            {
                cartiPanelUserListBox.Items.Clear();
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT U.nume, U.prenume, U.email, U.telefon " +
                         "FROM Utilizatori U " +
                         "WHERE U.id_utilizator <> @userId " +
                         "AND NOT EXISTS (SELECT 1 FROM Admini A WHERE U.id_utilizator = A.id_utilizator)";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@userId", utilizatorCurentId)
                };

                if (!string.IsNullOrWhiteSpace(numeUser))
                {
                    query += " AND U.nume LIKE @numeUser";
                    parameters.Add(new SqlParameter("@numeUser", $"{numeUser}%"));
                }

                if (!string.IsNullOrWhiteSpace(prenumeUser))
                {
                    query += " AND U.prenume LIKE @prenumeUser";
                    parameters.Add(new SqlParameter("@prenumeUser", $"%{prenumeUser}%"));
                }

                AdaugaInListBox(connection, query, parameters.ToArray());
            }
        }







        //de verificat astea 

        private bool IsAccountDeleted( int id_user, string connectionString )
        {
            if (id_user < 0)
            {
                return false;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT COUNT(*) FROM Utilizatori WHERE id_utilizator = @id_User AND DataStergeriiContului IS NOT NULL";

                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@id_User", id_user);
                        int userCount = (int)selectCommand.ExecuteScalar();
                        return userCount > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Înregistrați orice eroare în loguri sau gestionați-o în alt mod.
                Log.Error($"A apărut o eroare la verificarea utilizatorului: {ex}");
                return false;
            }
        }

        private void AdaugaAdministrator( int id_User, string rol, string connectionString )
        {
            if (id_User < 0)
            {
                MessageBox.Show("Utilizatorul nu a fost gasit.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand insertCommand = connection.CreateCommand())
                {
                    connection.Open();
                    insertCommand.CommandText = "INSERT INTO Admini(id_utilizator, rol) VALUES (@id_User, @rol)";
                    insertCommand.Parameters.AddWithValue("@id_User", id_User);
                    insertCommand.Parameters.AddWithValue("@rol", rol);

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        AdauagainListBoxUserii();
                    }
                    else
                    {
                        MessageBox.Show("Nu s-a putut adăuga administratorul.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la adăugarea administratorului: " + ex.Message);
            }
        }

        private void StergeAdmin( int id_user, string connectionString )
        {
            if (id_user < 0)
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand deleteCommand = connection.CreateCommand())
                {
                    connection.Open();
                    deleteCommand.CommandText = "DELETE FROM Admini WHERE id_utilizator = @id_user";
                    deleteCommand.Parameters.AddWithValue("@id_user", id_user);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        AdauagainListBoxAdminii();
                    }
                    else
                    {
                        MessageBox.Show("Nu s-a putut șterge administratorul.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la ștergerea administratorului: " + ex.Message);
            }
        }

        private void AdaugaDataStergeriiContului( int idUtilizator, DateTime? dateTime, string connectionString )
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE Utilizatori SET DataStergeriiContului = @dataStergeriiContului WHERE id_utilizator = @idUtilizator";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@idUtilizator", idUtilizator);
                        updateCommand.Parameters.AddWithValue("@dataStergeriiContului", dateTime);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Contul a fost marcat pentru ștergere.");
                        }
                        else
                        {
                            MessageBox.Show("Nu s-a putut marca contul pentru ștergere.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // SqlException este mai specifică pentru erorile legate de baza de date.
                MessageBox.Show("A apărut o eroare la marcarea contului pentru ștergere: " + ex.Message + ". Încercați din nou.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la marcarea contului pentru ștergere: " + ex.Message + ". Încercați din nou.");
            }
        }







        private string GenerateQueryAndParameters( string baseQuery, List<SqlParameter> parameters, string condition, string parameterName, string column )
        {
            if (!string.IsNullOrWhiteSpace(condition))
            {
                if (condition.Contains("%"))
                {
                    baseQuery += $" AND {column} LIKE @{parameterName}";
                }
                else
                {
                    baseQuery += $" AND {column} = @{parameterName}";
                }

                parameters.Add(new SqlParameter($"@{parameterName}", condition));
            }

            return baseQuery;
        }

        private bool AddParameterIfNotEmpty( List<SqlParameter> parameters, ref string query, string condition, string value )
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (query.EndsWith("SET "))
                {
                    query += condition;
                }
                else
                {
                    query += $", {condition}";
                }

                parameters.Add(new SqlParameter(condition.Split('=') [1].Trim(), value));
                return true;
            }
            else
                return false;
        }



        private void ModificaUser( int utilizatorCurentId, string connectionString )
        {
            if (utilizatorCurentId < 0)
                return;

            Biblioteca obj = (Biblioteca)Application.OpenForms ["Biblioteca"];
            string numeNou = FormatName(cartiPanelInformatiiTextBoxNumeUser.Text);
            string prenumeNou = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cartiPanelComboBoxPrenume.Text.ToLower().Trim());
            string emailNou = obj.FormatName(cartiPanelInformatiiTextBoxEmailUser.Text);
            string parolaNou = cartiPanelInformatiiTextBoxParolaUser.Text;
            string telNou = cartiPanelInformatiiTextBoxTelefonUser.Text.Trim();

            if (!TrimitereCodVerificare(emailNou, telNou))
                return;

            string updateQuery = "UPDATE Utilizatori SET ";
            List<SqlParameter> parameters = new List<SqlParameter>();

            AddParameterIfNotEmpty(parameters, ref updateQuery, "nume = @numeNou", numeNou);
            AddParameterIfNotEmpty(parameters, ref updateQuery, "prenume = @prenumeNou", prenumeNou);
            AddParameterIfNotEmpty(parameters, ref updateQuery, "email = @emailNou", emailNou);
            AddParameterIfNotEmpty(parameters, ref updateQuery, "parola = @parolaNou", parolaNou);
            AddParameterIfNotEmpty(parameters, ref updateQuery, "telefon = @telNou", telNou);

            updateQuery += " WHERE id_utilizator = @idUtilizator";
            parameters.Add(new SqlParameter("@idUtilizator", utilizatorCurentId));

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddRange(parameters.ToArray());
                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Datele utilizatorului au fost actualizate cu succes.");
                        }
                        else
                        {
                            MessageBox.Show("Nu s-au putut actualiza datele utilizatorului.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Eroare la modificarea utilizatorului: {ex.Message}");
            }
        }

        private void cartiPanelInformatiiUserButtonModifica_ClickAsync( object sender, EventArgs e )
        {
            /*ColorDialog colorDialog = new ColorDialog();

            // Setează culorea implicită pentru dialog
            colorDialog.Color = yourCurrentBackgroundColor; // Înlocuiește cu culoarea ta actuală

            // Afișează dialogul
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Culoarea selectată de utilizator
                Color selectedColor = colorDialog.Color;

                // Aplică culoarea la fundalul aplicației sau la oricare alt element
                BackColor = selectedColor; // Înlocuiește cu controlul sau fundalul pe care dorești să îl schimbi
            }*/

            cartiPanelInformatiiTextBoxNumeUser.ReadOnly = false;
            cartiPanelInformatiiTextBoxPrenumeUser.ReadOnly = false;
            cartiPanelInformatiiTextBoxEmailUser.ReadOnly = false;
            cartiPanelInformatiiTextBoxParolaUser.ReadOnly = false;
            cartiPanelInformatiiTextBoxTelefonUser.ReadOnly = false;

            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;

            if (IsModificaUserButton())
            {
                ModificaUser(utilizatorCurentId, connectionString);
                return;
            }
            MessageBox.Show("Actiune nerecunoscuta.");
        }

        private void cartiPanelInformatiiUserButtonStergeCont_Click( object sender, EventArgs e )
        {
            string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;
            string message = "Sunteți sigur că doriți să ștergeți contul? Contul va fi șters permanent!";
            string title = "Confirmare ștergere cont";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);

            if (utilizatorSters)
            {
                message = "Doriți să recuperați contul?";
                title = "Confirmare recuperare cont";
                buttons = MessageBoxButtons.YesNo;
                result = MessageBox.Show(message, title, buttons);

                if (result == DialogResult.Yes)
                {
                    if (RecupereazaContul(connectionString))
                    {
                        loginToolStripMenuItem_Click(sender, e);
                        return;
                    }
                    MessageBox.Show("Codul introdus este incorect. Contul nu a fost recuperat.");
                }
                return;
            }

            if (result == DialogResult.Yes)
            {
                AdaugaDataStergeriiContului(utilizatorCurentId, DateTime.Now, connectionString);
                loginToolStripMenuItem_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Stergere anulată.");
            }
        }








        private string GetUserEnteredCode()
        {
            // Implementează această funcție pentru a citi și returna codul introdus de utilizator.
            // Poți folosi Console.ReadLine() pentru citirea de la tastatură.
            throw new NotImplementedException();
        }
        private void SendVerificationSMS( string phoneNumber, string verificationCode )
        {
            // Autentificarea la serviciul Twilio
            /* const string accountSid = "YourTwilioAccountSid";
             const string authToken = "YourTwilioAuthToken";
             TwilioClient.Init(accountSid, authToken);

             try
             {
                 // Trimite SMS-ul folosind serviciul Twilio
                 var message = MessageResource.Create(
                     body: $"Codul de verificare este: {verificationCode}",
                     from: new Twilio.Types.PhoneNumber("YourTwilioPhoneNumber"),
                     to: new Twilio.Types.PhoneNumber(phoneNumber)
                 );

                 Console.WriteLine($"SMS trimis cu succes cu ID-ul: {message.Sid}");
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"Eroare la trimiterea SMS-ului: {ex.Message}");
             }*/
            throw new NotImplementedException();
        }


        private string GenerateVerificationCode()
        {
            Random random = new Random();
            int code = random.Next(0, 1000000); // Generează un număr între 0 și 999999.

            // Formatează codul pentru a avea întotdeauna 6 cifre, inclusiv zero-le din față, dacă există.
            return code.ToString("D6");
        }
        private void SendVerificationEmail( string userEmail, string verificationCode )
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient("your-smtp-server.com");

                mail.From = new MailAddress("your-email@gmail.com");
                mail.To.Add(userEmail);
                mail.Subject = "Cod de verificare";
                mail.Body = $"Codul de verificare este: {verificationCode}";

                client.Port = 587;
                client.Credentials = new NetworkCredential("your-email@gmail.com", "your-password");
                client.EnableSsl = true;
                throw new NotImplementedException("Trebuie un email al firmei.");

                client.Send(mail);
            }
            catch (Exception ex)
            {
                // Poți înregistra sau trata eroarea aici.
                Log.Error($"Eroare la trimiterea emailului de verificare: {ex.Message}");
            }
        }
        private bool VerifyCode( string userEnteredCode, string generatedCode )
        {
            return userEnteredCode == generatedCode;
        }
        private bool TrimitereCodVerificare( string emailNou, string telNou )
        {
            Biblioteca obj = (Biblioteca)Application.OpenForms ["Biblioteca"];

            bool isEmailValid = obj.IsValidEmail(emailNou) && !string.IsNullOrWhiteSpace(emailNou);
            bool isTelValid = obj.IsValidTelefon(telNou) && !string.IsNullOrWhiteSpace(telNou);

            if (!isEmailValid && !isTelValid)
            {
                MessageBox.Show("Email sau număr de telefon invalid sau necompletat.");
                return false;
            }

            string generatedCode = GenerateVerificationCode();
            bool isCodeVerified = false;

            if (isEmailValid)
            {
                SendVerificationEmail(emailNou, generatedCode);
                string userEnteredCode = GetUserEnteredCode();

                isCodeVerified = VerifyCode(userEnteredCode, generatedCode);

                if (isCodeVerified)
                {
                    MessageBox.Show("Codul a fost trimis pe adresa de email și a fost confirmat cu succes.");
                }
                else
                {
                    MessageBox.Show("Codul introdus pentru email este incorect. Contul nu a fost recuperat.");
                }
            }

            if (isTelValid && !isCodeVerified)
            {
                SendVerificationSMS(telNou, generatedCode);
                string userEnteredCode = GetUserEnteredCode();

                isCodeVerified = VerifyCode(userEnteredCode, generatedCode);

                if (isCodeVerified)
                {
                    MessageBox.Show("Codul a fost trimis prin SMS și a fost confirmat cu succes.");
                }
                else
                {
                    MessageBox.Show("Codul introdus pentru SMS este incorect. Contul nu a fost recuperat.");
                }
            }

            return isCodeVerified;
        }
        private bool RecupereazaContul( string connectionString )
        {
            string generatedCode = GenerateVerificationCode();
            SendVerificationEmail(emailUtilizator, generatedCode);
            string userEnteredCode = GetUserEnteredCode();

            if (VerifyCode(userEnteredCode, generatedCode))
            {
                AdaugaDataStergeriiContului(utilizatorCurentId, null, connectionString);
                return true;
            }
            return false;
        }






        public class PanelConfiguration
        {
            public string PanelName
            {
                get; set;
            }
            public bool IsComboBoxRolAdminVisible
            {
                get; set;
            }
            public string UserButtonStergeText
            {
                get; set;
            }
            public bool IsISBNReadOnly
            {
                get; set;
            }
            public bool IsEdituraDropDown
            {
                get; set;
            }
            public bool IsNrCopiiVisible
            {
                get; set;
            }
            public string CautaButtonText
            {
                get; set;
            }
            public bool IsAdaugasiStergeVisible
            {
                get; set;
            }
            public string AdaugasiStergeButtonText
            {
                get; set;
            }

            public PanelConfiguration()
            {
                IsComboBoxRolAdminVisible = false;
                PanelName = "cartiPanelCartiApasat";
                UserButtonStergeText = "Sterge Utilizator";
                IsISBNReadOnly = false;
                IsEdituraDropDown = true;
                IsNrCopiiVisible = false;
                IsAdaugasiStergeVisible = true;
                CautaButtonText = "Cauta";
                AdaugasiStergeButtonText = "Imprumuta";
            }
        }


    }
}
