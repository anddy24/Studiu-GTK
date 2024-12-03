using Gtk;
using MySql.Data.MySqlClient;
using System;

public class ConsultatiiNode : Gtk.TreeNode
{
    [Gtk.TreeNodeValue(Column = 0)]
    public int ID_Consultatie { get; }

    [Gtk.TreeNodeValue(Column = 1)]
    public int ID_Pacient { get; }

    [Gtk.TreeNodeValue(Column = 2)]
    public int ID_Medic { get; }

    [Gtk.TreeNodeValue(Column = 3)]
    public string DataConsultatiei { get; }

    [Gtk.TreeNodeValue(Column = 4)]
    public string Simptome { get; }

    [Gtk.TreeNodeValue(Column = 5)]
    public string Diagnostic { get; }

    [Gtk.TreeNodeValue(Column = 6)]
    public string Tratament { get; }

    [Gtk.TreeNodeValue(Column = 7)]
    public string Observatii { get; }

    public ConsultatiiNode(
        int idConsultatie, 
        int idPacient, 
        int idMedic, 
        string dataConsultatiei, 
        string simptome, 
        string diagnostic, 
        string tratament, 
        string observatii)
    {
        ID_Consultatie = idConsultatie;
        ID_Pacient = idPacient;
        ID_Medic = idMedic;
        DataConsultatiei = dataConsultatiei;
        Simptome = simptome;
        Diagnostic = diagnostic;
        Tratament = tratament;
        Observatii = observatii;
    }
}

public class ConsultatiiStore
{
#pragma warning disable CS8618
    private Gtk.NodeStore store;
#pragma warning restore CS8618 
    public Gtk.NodeStore Store
    {
        get
        {
            if (store == null)
            {
                store = new Gtk.NodeStore(typeof(ConsultatiiNode));
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
                    ID_Consultatie, 
                    ID_Pacient, 
                    ID_Medic, 
                    DataConsultatiei, 
                    Simptome, 
                    Diagnostic, 
                    Tratament, 
                    Observatii 
                FROM Consultatii";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    store.AddNode(new ConsultatiiNode(
                        reader.GetInt32("ID_Consultatie"),
                        reader.GetInt32("ID_Pacient"),
                        reader.GetInt32("ID_Medic"),
                        reader.GetDateTime("DataConsultatiei").ToString("yyyy-MM-dd HH:mm:ss"),
                        reader["Simptome"] != DBNull.Value ? reader.GetString("Simptome") : "",
                        reader["Diagnostic"] != DBNull.Value ? reader.GetString("Diagnostic") : "",
                        reader["Tratament"] != DBNull.Value ? reader.GetString("Tratament") : "",
                        reader["Observatii"] != DBNull.Value ? reader.GetString("Observatii") : ""
                    ));
                }
            }
        }
    }
}

public class ShowConsultations
{
    private readonly ConsultatiiStore consStore;

    public ShowConsultations()
    {
        consStore = new ConsultatiiStore();
    }

    public void PopulateNodeView(Box targetBox)
    {
        var view = new Gtk.NodeView(consStore.Store);

        view.AppendColumn("ID Consultatie", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("ID Pacient", new Gtk.CellRendererText(), "text", 1);
        view.AppendColumn("ID Medic", new Gtk.CellRendererText(), "text", 2);
        view.AppendColumn("Data Consultatiei", new Gtk.CellRendererText(), "text", 3);
        view.AppendColumn("Simptome", new Gtk.CellRendererText(), "text", 4);
        view.AppendColumn("Diagnostic", new Gtk.CellRendererText(), "text", 5);
        view.AppendColumn("Tratament", new Gtk.CellRendererText(), "text", 6);
        view.AppendColumn("Observatii", new Gtk.CellRendererText(), "text", 7);

        foreach (var child in targetBox.Children)
        {
            targetBox.Remove(child);
        }

        targetBox.PackStart(view, true, true, 0);
        view.ShowAll();
    }
}
