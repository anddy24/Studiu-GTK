using Gtk;
using System;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using Dispensar;
public class AdminWindow : Window
{
    private Button buttonLogOut;
    private Button buttonUsers;
    private Button buttonView;
    private Button buttonReport;
    private Button buttonActivity;

    private Frame frameUsers;
    private Frame frameView;
    private Frame frameReport;
    private Frame frameActivity;
    private Window adminWindow;

    private Entry entryUser;
    private Entry entryPwd;
    private Button buttonAddUser;

    private Box showUserBox;
    private Button buttonDisplayUsers;


    private Box showData;
    private Button buttonDisplayConsultations;
    private Button buttonDisplayPatients;
    private Button butttonDisplayApointments;
    private Button buttonDisplayTreatments;
    private Button buttonDisplayDoctors;

    private Button buttonDisplayUserLog;
    private Box boxShowUserLog;

    private Button buttonExport;
    private ComboBox comboBoxTable;

    public AdminWindow() : base("Admin Window")


    {
        SetDefaultSize(1280, 780);
        SetPosition(WindowPosition.Center);

        var builder = new Builder();
        try
        {
            builder.AddFromFile("Frontend/AdminWindow.glade");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading Glade file: " + ex.Message);
            throw;
        }

        adminWindow = (Window)builder.GetObject("AdminWindow");
        if (adminWindow == null)
        {
            Console.WriteLine("AdminWindow not found in the Glade file.");
        }
        else
        {
            adminWindow.DeleteEvent += OnDeleteEvent;
            adminWindow.ShowAll();
        }

        buttonLogOut = (Button)builder.GetObject("button_logout");
        if (buttonLogOut != null)
        {
            buttonLogOut.Clicked += OnButtonLogOutClicked;
        }

        buttonUsers = (Button)builder.GetObject("button_users");
        if (buttonUsers != null)
        {
            buttonUsers.Clicked += OnButtonUsersClicked;
        }

        buttonView = (Button)builder.GetObject("button_view");
        if (buttonView != null)
        {
            buttonView.Clicked += OnButtonViewClicked;
        }

        buttonReport = (Button)builder.GetObject("button_report");
        if (buttonReport != null)
        {
            buttonReport.Clicked += OnButtonReportClicked;
        }

        buttonActivity = (Button)builder.GetObject("button_activity");
        if (buttonActivity != null)
        {
            buttonActivity.Clicked += OnButtonActivityClicked;
        }

        buttonAddUser = (Button)builder.GetObject("button_add_user");
        if (buttonAddUser != null)
        {
            buttonAddUser.Clicked += OnButtonAddUserClicked;
        }

         entryUser = (Entry)builder.GetObject("entry_user");
        entryPwd = (Entry)builder.GetObject("entry_pwd");

        frameUsers = (Frame)builder.GetObject("frame_users");
        frameView = (Frame)builder.GetObject("frame_view");
        frameReport = (Frame)builder.GetObject("frame_report");
        frameActivity = (Frame)builder.GetObject("frame_activity");

        showUserBox = (Box)builder.GetObject("box_users");
        buttonDisplayUsers = (Button)builder.GetObject("display_users");
        buttonDisplayUsers.Clicked += OnButtonDisplayUsersClicked;

        showData = (Box)builder.GetObject("data_box");
        buttonDisplayConsultations = (Button)builder.GetObject("display_consultations");
        buttonDisplayConsultations.Clicked += OnButtonDisplayConsultationsClicked;

        buttonDisplayPatients = (Button)builder.GetObject("display_pacients");
        buttonDisplayPatients.Clicked += OnButtonDisplayPatientsClicked;

        buttonDisplayTreatments = (Button)builder.GetObject("display_treatments");
        buttonDisplayTreatments.Clicked += OnButtonDisplayTreatmentsClicked;

        butttonDisplayApointments = (Button)builder.GetObject("display_apointments");
        butttonDisplayApointments.Clicked += OnBuilderDisplayApointmentsClicked;

        buttonDisplayDoctors = (Button)builder.GetObject("display_doctors");
        buttonDisplayDoctors.Clicked += OnButtonDisplayDoctorsClicked;
        
        boxShowUserLog = (Box)builder.GetObject("box_user_log");

        buttonDisplayUserLog = (Button)builder.GetObject("display_user_log");
        buttonDisplayUserLog.Clicked += OnButtonDisplayUserLogClicked;

        comboBoxTable = (ComboBox)builder.GetObject("combobox_data");

        buttonExport = (Button)builder.GetObject("show_act");
        buttonExport.Clicked += OnButtonExportClicked;

        PopulateComboBoxWithTables();
        HideFrames();
    }

    private void HideFrames()
    {
        frameUsers.Hide();
        frameView.Hide();
        frameReport.Hide();
        frameActivity.Hide();
    }

  private void OnButtonExportClicked(object sender, EventArgs e)
{
    TreeIter iter;
    if (comboBoxTable.GetActiveIter(out iter))
    {
        string selectedTable = comboBoxTable.Model.GetValue(iter, 0).ToString(); 

        if (!string.IsNullOrEmpty(selectedTable))
        {
            string folderPath = "/Documents";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = folderPath + "/" + selectedTable + ".xlsx"; 

            var exporter = new ExcelExporter("Server=localhost;Database=dispensar;user=andrei;password=andrei;");
            exporter.ExportDataToExcel(selectedTable, filePath);
        }
        else
        {
            Console.WriteLine("No table selected.");
        }
    }
    else
    {
        Console.WriteLine("Select a table from the ComboBox.");
    }
}




private void PopulateComboBoxWithTables()
{
    try
    {

        ListStore listStore = new ListStore(typeof(string));

        using (var connection = new MySqlConnection("Server=localhost;Database=dispensar;user=andrei;password=andrei;"))
        {
            connection.Open();
            string query = "SHOW TABLES"; 

            using (var cmd = new MySqlCommand(query, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string tableName = reader.GetString(0);
                    listStore.AppendValues(tableName);
            }
        }

        comboBoxTable.Model = listStore;

        CellRendererText cellRenderer = new CellRendererText();
        comboBoxTable.PackStart(cellRenderer, true);
        comboBoxTable.AddAttribute(cellRenderer, "text", 0);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error populating ComboBox: {ex.Message}");
    }
}
    private void OnButtonUsersClicked(object? sender, EventArgs e)
    {
        HideFrames();
        frameUsers.Show();
    }

    private void OnButtonViewClicked(object? sender, EventArgs e)
    {
        HideFrames();
        frameView.Show();
    }

    private void OnButtonReportClicked(object? sender, EventArgs e)
    {
        HideFrames();
        frameReport.Show();
    }

    private void OnButtonActivityClicked(object? sender, EventArgs e)
    {
        HideFrames();
        frameActivity.Show();
    }

    private void OnDeleteEvent(object? sender, DeleteEventArgs e)
    {
        Application.Quit();
        e.RetVal = true;
    }

    private void OnButtonLogOutClicked(object? sender, EventArgs e)
    {
        Console.WriteLine("Logging out...");
        Application.Quit();
    }

    private void OnButtonDisplayUserLogClicked(object? sender, EventArgs e)
    {
        var showUserLogs = new ShowUserLogs();
        showUserLogs.PopulateNodeView(boxShowUserLog);
    }
private void OnButtonDisplayConsultationsClicked(object sender, EventArgs e)
{
    var showConsultations = new ShowConsultations();
    showConsultations.PopulateNodeView(showData);
}

private void OnButtonDisplayPatientsClicked(object sender, EventArgs e)
{
    var showPatients = new ShowPatient();
    showPatients.PopulateNodeView(showData);
}

private void OnButtonDisplayTreatmentsClicked(object sender, EventArgs e)
{
    var showTreatments = new ShowTreatments();
    showTreatments.PopulateNodeView(showData);
}

private void OnBuilderDisplayApointmentsClicked(object sender, EventArgs e)
{
    var showAppointments = new ShowAppointments();
    showAppointments.PopulateNodeView(showData);
}

private void OnButtonDisplayDoctorsClicked(object sender, EventArgs e)
{
    var showDoctors = new ShowDoctors();
    showDoctors.PopulateNodeView(showData);
}


    private void OnButtonAddUserClicked(object? sender, EventArgs e)
    {
        string username = entryUser.Text;
        string plainPassword = entryPwd.Text;
        string role = "user"; 

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(plainPassword))
        {
            Console.WriteLine("Username or password cannot be empty.");
            return;
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

        bool success = InsertUserIntoDatabase(username, hashedPassword, role);

        if (success)
        {
            Console.WriteLine("User added successfully.");
        }
        else
        {
            Console.WriteLine("Error adding user.");
        }
    }

    private void OnButtonDisplayUsersClicked(object? sender, EventArgs e)
{
    if (showUserBox == null)
    {
        Console.WriteLine("show_users_box not found in the Glade file.");
        return;
    }

    var showUsers = new ShowUsers();
    showUsers.PopulateNodeView(showUserBox);
}

    private bool InsertUserIntoDatabase(string username, string hashedPassword, string role)
    {
        try
        {
            string connectionString = "Server=localhost;Database=dispensar;user=andrei;password=andrei;";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@username, @passwordHash, @role)";
                using (var cmd = new MySqlCommand(query, connection))
                {

                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@passwordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@role", role);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting user into database: {ex.Message}");
            return false;
        }
    }
}
