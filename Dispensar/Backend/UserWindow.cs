using Gdk;
using Gtk;

public class UserWindow : Gtk.Window
{
    private Button buttonAddPacient;
    private Frame frameAddPacient;
    private Frame dashboard;
    private Button buttonDashboard;
    private Button buttonListPacient;
    private Button buttonUpdatePacient;
    private Button buttonLogOut;
    private Frame frameListPacient;
    private Frame frameUpdatePacient;
    private Button buttonShowPatientDB;
    private Box showPatientBox; 

    private Entry entryName;
    private Entry entrySurname;
    private Entry entryDateOfBirth;
    private Entry entryPhone;
    private Entry entryEmail;
    private Entry entryAdress;
    private Button buttonInsertPatient;

    private SearchEntry searchEntryPatientID;
    private Entry entryNewPhone;
    private Entry entryNewEmail;
    private Button buttonUpdate;

    private Label labelNrPacienti;

#pragma warning disable CS8618 
    public UserWindow() : base("User Window")
#pragma warning restore CS8618 
    {
        SetDefaultSize(1280, 780);
        SetPosition(WindowPosition.Center);

        var builder = new Builder();
        builder.AddFromFile("Frontend/UserWindow.glade");

        var miau = (Gtk.Window)builder.GetObject("UserWindow");
        miau.DeleteEvent += OnDeleteEvent;

        miau.ShowAll();
        miau.SetDefaultSize(1280, 780);

        buttonAddPacient = (Button)builder.GetObject("add_pacient");
        buttonAddPacient.Clicked += OnButtonAddPacientClicked;

        buttonDashboard = (Button)builder.GetObject("button_dashboard");
        buttonDashboard.Clicked += OnButtonDashboardClicked;

        buttonListPacient = (Button)builder.GetObject("list_pacient");
        buttonListPacient.Clicked += OnButtonListPacientClicked;

        buttonUpdatePacient = (Button)builder.GetObject("update_pacient");
        buttonUpdatePacient.Clicked += OnButtonUpdatePacientClicked;

        buttonLogOut = (Button)builder.GetObject("log_out");
        buttonLogOut.Clicked += OnButtonLogOutClicked;

        frameAddPacient = (Frame)builder.GetObject("frame_add_pacient");
        dashboard = (Frame)builder.GetObject("Dashboard");
        frameListPacient = (Frame)builder.GetObject("frame_list_pacient");
        frameUpdatePacient = (Frame)builder.GetObject("frame_update_pacient");

        showPatientBox = (Box)builder.GetObject("show_patient");
        if (showPatientBox == null)
        {
            Console.WriteLine("show_patient not found in the Glade file.");
        }

        buttonShowPatientDB = (Button)builder.GetObject("button_show_patients");
        buttonShowPatientDB.Clicked += OnButtonShowPatientDBCliked;

        buttonInsertPatient = (Button)builder.GetObject("button_add_pacient_db");
        buttonInsertPatient.Clicked += OnButtonInsertPatientClicked;

        entryAdress = (Entry)builder.GetObject("adress");
        entryDateOfBirth = (Entry)builder.GetObject("dateofbirth");
        entryEmail = (Entry)builder.GetObject("email");
        entryName = (Entry)builder.GetObject("name");
        entryPhone = (Entry)builder.GetObject("phone");
        entrySurname = (Entry)builder.GetObject("surname");
        
        searchEntryPatientID = (SearchEntry)builder.GetObject("search_id");
        entryNewEmail = (Entry)builder.GetObject("new_email");
        entryNewPhone = (Entry)builder.GetObject("new_phone");

        buttonUpdate = (Button)builder.GetObject("button_update");
        buttonUpdate.Clicked += OnButtonUpdateClicked;

        labelNrPacienti = (Label)builder.GetObject("nr_angajati");

        HideFrames();
    }

    private void HideFrames()
    {
        frameAddPacient.Hide();
        dashboard.Hide();
        frameListPacient.Hide();
        frameUpdatePacient.Hide();
    }

    private void OnButtonUpdateClicked(object? sender, EventArgs e)
{
    string patientID = searchEntryPatientID.Text.Trim();
    string newEmail = entryNewEmail.Text.Trim();
    string newPhone = entryNewPhone.Text.Trim();

    if (string.IsNullOrEmpty(patientID))
    {
        Console.WriteLine("Please enter the Patient ID.");
        return;
    }
    if (string.IsNullOrEmpty(newEmail) && string.IsNullOrEmpty(newPhone))
    {
        Console.WriteLine("Please enter at least one field to update (Email or Phone).");
        return;
    }

    try
    {
        string connectionString = "Server=localhost;Database=dispensar;User ID=andrei;Password=andrei;";

        string query = "UPDATE Pacienti SET ";
        bool addComma = false;

        if (!string.IsNullOrEmpty(newEmail))
        {
            query += "Email = @NewEmail";
            addComma = true;
        }
        if (!string.IsNullOrEmpty(newPhone))
        {
            if (addComma)
                query += ", ";
            query += "Telefon = @NewPhone";
        }
        query += " WHERE ID_Pacient = @PatientID";

        using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PatientID", patientID);

                if (!string.IsNullOrEmpty(newEmail))
                    command.Parameters.AddWithValue("@NewEmail", newEmail);

                if (!string.IsNullOrEmpty(newPhone))
                    command.Parameters.AddWithValue("@NewPhone", newPhone);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Patient with ID {patientID} updated successfully!");
                }
                else
                {
                    Console.WriteLine($"No patient found with ID {patientID}.");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating patient: {ex.Message}");
    }
}


    private void OnButtonInsertPatientClicked(object? sender, EventArgs e)
{
    string name = entryName.Text.Trim();
    string surname = entrySurname.Text.Trim();
    string dateOfBirth = entryDateOfBirth.Text.Trim();
    string phone = entryPhone.Text.Trim();
    string email = entryEmail.Text.Trim();
    string address = entryAdress.Text.Trim();

    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) ||
        string.IsNullOrEmpty(dateOfBirth) || string.IsNullOrEmpty(phone))
    {
        Console.WriteLine("Please fill in all required fields.");
        return;
    }

    try
    {
        string connectionString = "Server=localhost;Database=dispensar;User ID=andrei;Password=andrei;";

        string query = @"
            INSERT INTO Pacienti (Nume, Prenume, DataNasterii, Telefon, Email, Adresa, DataInregistrarii)
            VALUES (@Name, @Surname, @DateOfBirth, @Phone, @Email, @Address, NOW());
        ";

        using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Surname", surname);
                command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                command.Parameters.AddWithValue("@Phone", phone);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Address", address);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Patient inserted successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to insert patient.");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error inserting patient: {ex.Message}");
    }
}


    private void OnDeleteEvent(object sender, DeleteEventArgs e)
    {
        Application.Quit();
        e.RetVal = true;
    }

    private void OnButtonAddPacientClicked(object? sender, EventArgs e)
    {
        frameAddPacient.Show();
        dashboard.Hide();
        frameListPacient.Hide();
        frameUpdatePacient.Hide();
    }

    private void OnButtonDashboardClicked(object? sender, EventArgs e)
    {
    frameAddPacient.Hide();
    dashboard.Show();
    frameListPacient.Hide();
    frameUpdatePacient.Hide();

    try
    {
        string connectionString = "Server=localhost;Database=dispensar;User ID=andrei;Password=andrei;";

        string query = "SELECT COUNT(*) FROM Pacienti";

        using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
            {
                int totalPatients = Convert.ToInt32(command.ExecuteScalar());

                labelNrPacienti.Text = $"{totalPatients}";
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching total number of patients: {ex.Message}");
        labelNrPacienti.Text = "Error fetching patient count";
    }
    }

    private void OnButtonListPacientClicked(object? sender, EventArgs e)
    {
        frameAddPacient.Hide();
        frameUpdatePacient.Hide();
        dashboard.Hide();
        frameListPacient.Show();
    }

    private void OnButtonUpdatePacientClicked(object? sender, EventArgs e)
    {
        frameAddPacient.Hide();
        dashboard.Hide();
        frameListPacient.Hide();
        frameUpdatePacient.Show();
    }

    private void OnButtonLogOutClicked(object? sender, EventArgs e)
    {
        Application.Quit();
    }

    private void OnButtonShowPatientDBCliked(object? sender, EventArgs e)
    {
        if (showPatientBox == null)
        {
            Console.WriteLine("show_patient GtkBox not initialized.");
            return;
        }

        var showPatient = new ShowPatient();
        showPatient.PopulateNodeView(showPatientBox);
    }
}
