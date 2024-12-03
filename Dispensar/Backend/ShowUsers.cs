using Gtk;
using MySql.Data.MySqlClient;
using System;

public class UsersNode : Gtk.TreeNode
{
    [Gtk.TreeNodeValue(Column = 0)]
    public int UserID { get; }

    [Gtk.TreeNodeValue(Column = 1)]
    public string Username { get; }

    [Gtk.TreeNodeValue(Column = 2)]
    public string PasswordHash { get; }

    [Gtk.TreeNodeValue(Column = 3)]
    public string Role { get; }

    public UsersNode(int userID, string username, string passwordHash, string role)
    {
        UserID = userID;
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }
}

public class UsersStore
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
                store = new Gtk.NodeStore(typeof(UsersNode));
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
            string query = "SELECT UserID, Username, PasswordHash, Role FROM Users";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    store.AddNode(new UsersNode(
                        reader.GetInt32("UserID"),
                        reader.GetString("Username"),
                        reader.GetString("PasswordHash"),
                        reader.GetString("Role")
                    ));
                }
            }
        }
    }
}

public class ShowUsers
{
    private readonly UsersStore userStore;

    public ShowUsers()
    {
        userStore = new UsersStore();
    }

    public void PopulateNodeView(Box targetBox)
    {
        var view = new Gtk.NodeView(userStore.Store);

        view.AppendColumn("User ID", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("Username", new Gtk.CellRendererText(), "text", 1);
        view.AppendColumn("Password Hash", new Gtk.CellRendererText(), "text", 2);
        view.AppendColumn("Role", new Gtk.CellRendererText(), "text", 3);

        // Clear the target box before adding new content
        foreach (var child in targetBox.Children)
        {
            targetBox.Remove(child);
        }

        targetBox.PackStart(view, true, true, 0);
        view.ShowAll();
    }
}
