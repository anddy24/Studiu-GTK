using Gtk;
using MySql.Data.MySqlClient;
using System;

public class TratamenteNode : Gtk.TreeNode
{
    [Gtk.TreeNodeValue(Column = 0)]
    public int ID_Tratament { get; }

    [Gtk.TreeNodeValue(Column = 1)]
    public int ID_Consultatie { get; }

    [Gtk.TreeNodeValue(Column = 2)]
    public string Denumire { get; }

    [Gtk.TreeNodeValue(Column = 3)]
    public string Dozaj { get; }

    [Gtk.TreeNodeValue(Column = 4)]
    public string DurataTratamentului { get; }

    public TratamenteNode(int idTratament, int idConsultatie, string denumire, string dozaj, string durataTratamentului)
    {
        ID_Tratament = idTratament;
        ID_Consultatie = idConsultatie;
        Denumire = denumire;
        Dozaj = dozaj;
        DurataTratamentului = durataTratamentului;
    }
}

public class TratamenteStore
{
    private Gtk.NodeStore store;

    public Gtk.NodeStore Store
    {
        get
        {
            if (store == null)
            {
                store = new Gtk.NodeStore(typeof(TratamenteNode));
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
                    t.ID_Tratament, 
                    t.ID_Consultatie, 
                    t.Denumire, 
                    t.Dozaj, 
                    t.DurataTratamentului 
                FROM Tratamente t";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    store.AddNode(new TratamenteNode(
                        reader.GetInt32("ID_Tratament"),
                        reader.GetInt32("ID_Consultatie"),
                        reader.GetString("Denumire"),
                        reader["Dozaj"] != DBNull.Value ? reader.GetString("Dozaj") : "",
                        reader["DurataTratamentului"] != DBNull.Value ? reader.GetString("DurataTratamentului") : ""
                    ));
                }
            }
        }
    }
}

public class ShowTreatments
{
    private readonly TratamenteStore tratStore;

    public ShowTreatments()
    {
        tratStore = new TratamenteStore();
    }

    public void PopulateNodeView(Box targetBox)
    {
        var view = new Gtk.NodeView(tratStore.Store);

        view.AppendColumn("ID Tratament", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("ID Consultație", new Gtk.CellRendererText(), "text", 1);
        view.AppendColumn("Denumire", new Gtk.CellRendererText(), "text", 2);
        view.AppendColumn("Dozaj", new Gtk.CellRendererText(), "text", 3);
        view.AppendColumn("Durata Tratamentului", new Gtk.CellRendererText(), "text", 4);

        foreach (var child in targetBox.Children)
        {
            targetBox.Remove(child);
        }

        targetBox.PackStart(view, true, true, 0);
        view.ShowAll();
    }
}
