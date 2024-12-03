using Gtk;
using MySql.Data.MySqlClient;
using System;

public class ProgramariNode : Gtk.TreeNode
{
    [Gtk.TreeNodeValue(Column = 0)]
    public int ID_Programare { get; }

    [Gtk.TreeNodeValue(Column = 1)]
    public string NumePacient { get; }

    [Gtk.TreeNodeValue(Column = 2)]
    public string NumeMedic { get; }

    [Gtk.TreeNodeValue(Column = 3)]
    public string DataProgramare { get; }

    [Gtk.TreeNodeValue(Column = 4)]
    public string Status { get; }

    public ProgramariNode(int idProgramare, string numePacient, string numeMedic, string dataProgramare, string status)
    {
        ID_Programare = idProgramare;
        NumePacient = numePacient;
        NumeMedic = numeMedic;
        DataProgramare = dataProgramare;
        Status = status;
    }
}

public class ProgramariStore
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Gtk.NodeStore store;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Gtk.NodeStore Store
    {
        get
        {
            if (store == null)
            {
                store = new Gtk.NodeStore(typeof(ProgramariNode));
                LoadDataFromDatabase();
            }
            return store;
        }
    }

    private void LoadDataFromDatabase()
    {
        string connectionString = "server=localhost;database=dispensar;user=andrei;password=andrei;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = @"
                SELECT 
                    p.ID_Programare, 
                    pac.Nume AS NumePacient, 
                    pac.Prenume AS PrenumePacient, 
                    med.Nume AS NumeMedic, 
                    med.Prenume AS PrenumeMedic, 
                    p.DataProgramare, 
                    p.Status 
                FROM Programari p
                JOIN Pacienti pac ON p.ID_Pacient = pac.ID_Pacient
                JOIN Medici med ON p.ID_Medic = med.ID_Medic";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    store.AddNode(new ProgramariNode(
                        reader.GetInt32("ID_Programare"),
                        $"{reader.GetString("NumePacient")} {reader.GetString("PrenumePacient")}",
                        $"{reader.GetString("NumeMedic")} {reader.GetString("PrenumeMedic")}",
                        reader.GetDateTime("DataProgramare").ToString("yyyy-MM-dd HH:mm"),
                        reader.GetString("Status")
                    ));
                }
            }
        }
    }
}

public class ShowAppointments
{
    private readonly ProgramariStore progStore;

    public ShowAppointments()
    {
        progStore = new ProgramariStore();
    }

    public void PopulateNodeView(Box targetBox)
    {
        var view = new Gtk.NodeView(progStore.Store);

        view.AppendColumn("ID Programare", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("Pacient", new Gtk.CellRendererText(), "text", 1);
        view.AppendColumn("Medic", new Gtk.CellRendererText(), "text", 2);
        view.AppendColumn("Data Programare", new Gtk.CellRendererText(), "text", 3);
        view.AppendColumn("Status", new Gtk.CellRendererText(), "text", 4);

        foreach (var child in targetBox.Children)
        {
            targetBox.Remove(child);
        }

        targetBox.PackStart(view, true, true, 0);
        view.ShowAll();
    }
}
