using System;
using System.Windows.Forms;

namespace sistem_de_gestionare_a_bibliotecii
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Console.OutputEncoding = Encoding.UTF8; // Setăm codificarea consolei aici
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AutoDeleteUserProcess.Start(); // Apelăm metoda Start din clasa AutoDeleteUserProcess.
            Biblioteca biblioteca = new Biblioteca();

            //Carti carti = new Carti();
            Application.Run(biblioteca);
        }

    }
}
