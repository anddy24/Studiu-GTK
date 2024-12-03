using Gtk;
using MySql.Data.MySqlClient;

public class ShowUserLogs
{
    private readonly string connectionString = "server=localhost;database=dispensar;user=andrei;password=andrei;";

    public void PopulateNodeView(Box targetBox)
    {
        var store = new Gtk.NodeStore(typeof(LogNode));
        LoadDataFromDatabase(store);

        var view = new Gtk.NodeView(store);
        view.AppendColumn("Log ID", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("User ID", new Gtk.CellRendererText(), "text", 1);
        view.AppendColumn("Action", new Gtk.CellRendererText(), "text", 2);
        view.AppendColumn("Timestamp", new Gtk.CellRendererText(), "text", 3);
        view.AppendColumn("IP Address", new Gtk.CellRendererText(), "text", 4);
        view.AppendColumn("User Agent", new Gtk.CellRendererText(), "text", 5);

        foreach (var child in targetBox.Children)
        {
            targetBox.Remove(child);
        }

        targetBox.PackStart(view, true, true, 0);
        view.ShowAll();
    }

    private void LoadDataFromDatabase(Gtk.NodeStore store)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT LogID, UserID, Action, Timestamp, IPAddress, UserAgent FROM UserLog";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    store.AddNode(new LogNode(
                        reader.GetInt32("LogID"),
                        reader.GetInt32("UserID"),
                        reader.GetString("Action"),
                        reader.GetDateTime("Timestamp").ToString("yyyy-MM-dd HH:mm:ss"),
                        reader["IPAddress"] != DBNull.Value ? reader.GetString("IPAddress") : "",
                        reader["UserAgent"] != DBNull.Value ? reader.GetString("UserAgent") : ""
                    ));
                }
            }
        }
    }
}

public class LogNode : Gtk.TreeNode
{
    [Gtk.TreeNodeValue(Column = 0)]
    public int LogID { get; }

    [Gtk.TreeNodeValue(Column = 1)]
    public int UserID { get; }

    [Gtk.TreeNodeValue(Column = 2)]
    public string Action { get; }

    [Gtk.TreeNodeValue(Column = 3)]
    public string Timestamp { get; }

    [Gtk.TreeNodeValue(Column = 4)]
    public string IPAddress { get; }

    [Gtk.TreeNodeValue(Column = 5)]
    public string UserAgent { get; }

    public LogNode(int logId, int userId, string action, string timestamp, string ipAddress, string userAgent)
    {
        LogID = logId;
        UserID = userId;
        Action = action;
        Timestamp = timestamp;
        IPAddress = ipAddress;
        UserAgent = userAgent;
    }
}