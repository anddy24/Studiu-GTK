using Gtk;
using MySql.Data.MySqlClient;
using System;

public class MediciNode : Gtk.TreeNode
{
    [Gtk.TreeNodeValue(Column = 0)]
    public int ID_Medic { get; }

    [Gtk.TreeNodeValue(Column = 1)]
    public string Nume { get; }

    [Gtk.TreeNodeValue(Column = 2)]
    public string Prenume { get; }

    [Gtk.TreeNodeValue(Column = 3)]
    public string Specializare { get; }

    [Gtk.TreeNodeValue(Column = 4)]
    public string Telefon { get; }

    [Gtk.TreeNodeValue(Column = 5)]
    public string Email { get; }

    [Gtk.TreeNodeValue(Column = 6)]
    public string DataAngajarii { get; }

    public MediciNode(int idMedic, string nume, string prenume, string specializare, string telefon, string email, string dataAngajarii)
    {
        ID_Medic = idMedic;
        Nume = nume;
        Prenume = prenume;
        Specializare = specializare;
        Telefon = telefon;
        Email = email;
        DataAngajarii = dataAngajarii;
    }
}

public class MediciStore
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
                store = new Gtk.NodeStore(typeof(MediciNode));
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
                    ID_Medic, 
                    Nume, 
                    Prenume, 
                    Specializare, 
                    Telefon, 
                    Email, 
                    DataAngajarii 
                FROM Medici";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    store.AddNode(new MediciNode(
                        reader.GetInt32("ID_Medic"),
                        reader.GetString("Nume"),
                        reader.GetString("Prenume"),
                        reader.GetString("Specializare"),
                        reader["Telefon"] != DBNull.Value ? reader.GetString("Telefon") : "",
                        reader["Email"] != DBNull.Value ? reader.GetString("Email") : "",
                        reader.GetDateTime("DataAngajarii").ToString("yyyy-MM-dd")
                    ));
                }
            }
        }
    }
}

public class ShowDoctors
{
    private readonly MediciStore medStore;

    public ShowDoctors()
    {
        medStore = new MediciStore();
    }

    public void PopulateNodeView(Box targetBox)
    {
        var view = new Gtk.NodeView(medStore.Store);

        view.AppendColumn("ID Medic", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("Nume", new Gtk.CellRendererText(), "text", 1);
        view.AppendColumn("Prenume", new Gtk.CellRendererText(), "text", 2);
        view.AppendColumn("Specializare", new Gtk.CellRendererText(), "text", 3);
        view.AppendColumn("Telefon", new Gtk.CellRendererText(), "text", 4);
        view.AppendColumn("Email", new Gtk.CellRendererText(), "text", 5);
        view.AppendColumn("Data Angajarii", new Gtk.CellRendererText(), "text", 6);

        foreach (var child in targetBox.Children)
        {
            targetBox.Remove(child);
        }

        targetBox.PackStart(view, true, true, 0);
        view.ShowAll();
    }
}
