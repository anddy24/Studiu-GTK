using Gtk;
using MySql.Data.MySqlClient;
using System;

public class PacientiNode : Gtk.TreeNode
{
    [Gtk.TreeNodeValue(Column = 0)]
    public int ID_Pacient { get; }

    [Gtk.TreeNodeValue(Column = 1)]
    public string Nume { get; }

    [Gtk.TreeNodeValue(Column = 2)]
    public string Prenume { get; }

    [Gtk.TreeNodeValue(Column = 3)]
    public string DataNasterii { get; }

    [Gtk.TreeNodeValue(Column = 4)]
    public string Telefon { get; }

    [Gtk.TreeNodeValue(Column = 5)]
    public string Email { get; }

    [Gtk.TreeNodeValue(Column = 6)]
    public string Adresa { get; }

    public PacientiNode(int id, string nume, string prenume, string dataNasterii, string telefon, string email, string adresa)
    {
        ID_Pacient = id;
        Nume = nume;
        Prenume = prenume;
        DataNasterii = dataNasterii;
        Telefon = telefon;
        Email = email;
        Adresa = adresa;
    }
}

public class PacientiStore
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
                store = new Gtk.NodeStore(typeof(PacientiNode));
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
            string query = "SELECT ID_Pacient, Nume, Prenume, DataNasterii, Telefon, Email, Adresa FROM Pacienti";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    store.AddNode(new PacientiNode(
                        reader.GetInt32("ID_Pacient"),
                        reader.GetString("Nume"),
                        reader.GetString("Prenume"),
                        reader.GetDateTime("DataNasterii").ToString("yyyy-MM-dd"),
                        reader["Telefon"] != DBNull.Value ? reader.GetString("Telefon") : "",
                        reader["Email"] != DBNull.Value ? reader.GetString("Email") : "",
                        reader["Adresa"] != DBNull.Value ? reader.GetString("Adresa") : ""
                    ));
                }
            }
        }
    }
}

public class ShowPatient
{
    private readonly PacientiStore pacStore;

    public ShowPatient()
    {
        pacStore = new PacientiStore();
    }

    public void PopulateNodeView(Box targetBox)
    {
        var view = new Gtk.NodeView(pacStore.Store);

        view.AppendColumn("ID", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("Nume", new Gtk.CellRendererText(), "text", 1);
        view.AppendColumn("Prenume", new Gtk.CellRendererText(), "text", 2);
        view.AppendColumn("Data Nașterii", new Gtk.CellRendererText(), "text", 3);
        view.AppendColumn("Telefon", new Gtk.CellRendererText(), "text", 4);
        view.AppendColumn("Email", new Gtk.CellRendererText(), "text", 5);
        view.AppendColumn("Adresa", new Gtk.CellRendererText(), "text", 6);

        foreach (var child in targetBox.Children)
        {
            targetBox.Remove(child);
        }

        targetBox.PackStart(view, true, true, 0);
        view.ShowAll();
    }
}

