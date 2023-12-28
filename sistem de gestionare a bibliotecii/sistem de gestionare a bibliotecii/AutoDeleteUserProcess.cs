using Serilog;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
public class AutoDeleteUserProcess
{
    private static System.Timers.Timer deletionTimer;
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings ["AdaugareDate"].ConnectionString;

    public static async Task Start()
    {
        using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
        {
            deletionTimer = new System.Timers.Timer();
            deletionTimer.Elapsed += DeletionTimer_ElapsedAsync;
            SetTimerInterval();
            deletionTimer.Start();

            Console.WriteLine("Procesul de ștergere a conturilor a început. Apăsați Enter pentru a opri programul.");
            
            await Task.Run(() =>
            {
                Console.ReadLine();
                Console.WriteLine("Procesul de ștergere a conturilor a fost oprit.");
            }, cancellationTokenSource.Token);

            deletionTimer.Stop();
            deletionTimer.Dispose();
        }
    }

    private static void SetTimerInterval()
    {
        // Obțineți ora actuală.
        DateTime now = DateTime.Now;

        // Calculăm momentul în care vrem să rulăm procesul următoarea dată (aici, mâine dimineață la 2:00 AM).
        DateTime nextRunTime = now.Date.AddDays(1).AddHours(2);

        // Calculăm intervalul de timp până la următoarea rulare.
        double interval = (nextRunTime - now).TotalMilliseconds;

        // Setăm intervalul pentru timer.
        deletionTimer.Interval = interval;

        Console.Write(interval);
    }

    private static async void DeletionTimer_ElapsedAsync( object sender, ElapsedEventArgs e )
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                // Selectați utilizatorii care au fost marcați pentru ștergere cu mai mult de 2 săptămâni în urmă.
                string selectQuery = "SELECT id_utilizator FROM Utilizatori WHERE DataStergeriiContului IS NOT NULL AND DataStergeriiContului < DATEADD(WEEK, -2, GETDATE())";
                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int userId = reader.GetInt32(0);
                            await StergeUser(userId, connectionString);
                        }
                    }
                }
            }

            Console.WriteLine("Procesul de ștergere a conturilor a fost finalizat.");
        }
        catch (Exception ex)
        {
            Log.Error($"A apărut o eroare în timpul procesului de ștergere a conturilor: {ex}");
        }
        finally
        {
            // Setăm următoarea rulare a timerului.
            SetTimerInterval();
        }
    }

    private static async Task StergeUser( int id_User, string connectionString )
    {
        if (id_User < 0)
        {
            return;
        }

        SqlConnection connection = null; // Inițializăm conexiunea în afara blocului try
        try
        {
            connection = new SqlConnection(connectionString);
            await connection.OpenAsync(); // Folosim OpenAsync pentru a permite apeluri asincrone

            //string deleteQuery = "DELETE FROM Utilizatori WHERE nume=@nume AND prenume=@prenume AND email=@email";
            string deleteQuery = "DELETE FROM Utilizatori WHERE id_utilizator = @id_User ";
            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
            {
                // deleteCommand.Parameters.AddWithValue("@nume", nume);
                //deleteCommand.Parameters.AddWithValue("@prenume", prenume);
                //deleteCommand.Parameters.AddWithValue("@email", email);
                deleteCommand.Parameters.AddWithValue("@id_User", id_User);
                int rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    Log.Warning("Nu s-a putut șterge utilizatorul.");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"A apărut o eroare la ștergerea utilizatorului: {ex}");
        }
        finally
        {
            connection?.Close(); // Închidem conexiunea în blocul finally
            connection?.Dispose(); // Eliberăm resursele asociate conexiunii
        }
    }
}
