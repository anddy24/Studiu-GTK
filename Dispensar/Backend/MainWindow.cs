using System;
using Gtk;
using Dispensar;
using System.IO;

namespace Dispensar
{
    public class MainWindow : Window
    {
        private Button loginButton;
        private Entry usernameEntry;
        private Entry passwordEntry;
        private Window mainWindow;

        #pragma warning disable CS8618 
        public MainWindow() : base("Main Window")
        #pragma warning restore CS8618 
        {
            SetDefaultSize(1280, 780);
            SetPosition(WindowPosition.Center);

            var builder = new Builder();
            try
            {
                builder.AddFromFile("Frontend/MainWindow.glade");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading Glade file: " + ex.Message);
                throw;
            }

            loginButton = (Button)builder.GetObject("button_signin");
            usernameEntry = (Entry)builder.GetObject("user_name");
            passwordEntry = (Entry)builder.GetObject("user_pass");

            if (loginButton == null || usernameEntry == null || passwordEntry == null)
            {
                Console.WriteLine("One or more widgets were not found in the Glade file.");
            }
            else
            {
                loginButton.Clicked += OnLoginButtonClicked;
            }

            mainWindow = (Window)builder.GetObject("MainWindow");
            if (mainWindow == null)
            {
                Console.WriteLine("MainWindow not found in the Glade file.");
            }
            else
            {
                mainWindow.DeleteEvent += OnDeleteEvent;
                mainWindow.ShowAll();
            }

            ApplyStyles();
        }

        private void ApplyStyles()
        {
            try
            {
                var cssProvider = new Gtk.CssProvider();
                string cssFilePath = Environment.CurrentDirectory + "/Frontend/style.css";

                cssProvider.LoadFromPath(cssFilePath);

                Gtk.StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, Gtk.StyleProviderPriority.User);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading CSS: {ex.Message}");
            }
        }

        private void OnLoginButtonClicked(object? sender, EventArgs e)
        {
            if (mainWindow == null)
            {
                Console.WriteLine("mainWindow is null.");
                return;
            }

            string username = usernameEntry.Text.Trim();
            string password = passwordEntry.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageDialog dialog = new MessageDialog(this,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Ok,
                    "Please enter both username and password.");
                dialog.Run();
                dialog.Destroy();
                return;
            }

            try
            {
                string connectionString = "Server=localhost;Database=dispensar;user=andrei;password=andrei;";
                Authentication auth = new Authentication(connectionString);

                Console.WriteLine($"Attempting to authenticate user: {username}");

                User authenticatedUser = auth.AuthenticateUser(username, password);

                if (authenticatedUser != null)
                {
                    Console.WriteLine($"Autentificat cu succes! Rol: {authenticatedUser.Role}");

                    if (authenticatedUser.Role == "admin")
                    {
                        Console.WriteLine("Acces la funcționalități de administrator.");

                        mainWindow.Hide();
                        var adminWindow = new AdminWindow();
                    }
                    else if (authenticatedUser.Role == "user")
                    {
                        Console.WriteLine("Acces limitat la funcționalități de utilizator.");

                        mainWindow.Hide();
                        var userWindow = new UserWindow();
                    }
                    else
                    {
                        Console.WriteLine("Rol necunoscut.");
                        MessageDialog dialog = new MessageDialog(this,
                            DialogFlags.Modal,
                            MessageType.Error,
                            ButtonsType.Ok,
                            "Rol necunoscut.");
                        dialog.Run();
                        dialog.Destroy();
                    }
                }
                else
                {
                    MessageDialog dialog = new MessageDialog(this,
                        DialogFlags.Modal,
                        MessageType.Error,
                        ButtonsType.Ok,
                        "Invalid username or password.");
                    dialog.Run();
                    dialog.Destroy();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
                MessageDialog dialog = new MessageDialog(this,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Ok,
                    $"Authentication error: {ex.Message}");
                dialog.Run();
                dialog.Destroy();
            }
        }

        private void OnDeleteEvent(object sender, DeleteEventArgs e)
        {
            Application.Quit();
            e.RetVal = true;
        }
    }
}
