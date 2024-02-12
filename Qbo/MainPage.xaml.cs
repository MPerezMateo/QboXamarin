using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using Xamarin.Forms;

namespace Qbo
{
  public partial class MainPage : ContentPage
  {

    private static string ip = "192.168.1.18";
    
    private static string dbName = "QueVeo";
    private static string uId = "adminQBO";
    private static string password = "admin123";
    public static string constring = $"Data Source={ip};Initial Catalog={dbName};User ID={uId};Password={password};MultipleActiveResultSets=True;Encrypt=False;Persist Security Info=False;";
    public static SqlConnection con;
    public static Usuario user;
    public static List<Plataforma> plataformas = new List<Plataforma>();

    public MainPage()
    {
      InitializeComponent();
      SQLConnect();
    }
    public void registro(object sender, EventArgs args)
    {
      Navigation.PushAsync(new Registro());
    }

    void login(object sender, EventArgs args)
    {
      var query = $"SELECT * FROM dbo.Users WHERE (username = '{entryUser.Text}' OR email = '{entryUser.Text}') AND password = '{entryPass.Text}'";
      using (con = new SqlConnection(constring))
      {
        try
        {
          con.Open();
          using (SqlCommand command = new SqlCommand(query, con))
          {
            using (SqlDataReader reader = command.ExecuteReader())
            {
              reader.Read();
              if ((bool)reader["isActive"])
              {
                user = new Usuario((int)reader["id"], (string)reader["email"], (string)reader["username"], (string)reader["password"], (DateTime)reader["LastAccess"], (DateTime)reader["CreatedAt"]);
                // Se debería llamar aqui a un método de actualizar el último acceso del usuario
                Application.Current.Properties["user"] = entryUser.Text;
                Application.Current.Properties["password"] = entryPass.Text;
                Navigation.PushAsync(new HomeTabs());
              }
              else if (!(bool)reader["isActive"])
              {
                DisplayAlert("Usuario no activo", "El usuario ha sido desactivado, consulte con soporte", "Ok");
              }
            }
          }
        }
        catch (InvalidOperationException ioe)
        {
          Debug.WriteLine(ioe.ToString());
          if (ioe.Message.Contains("data is present"))
          {
            DisplayAlert("Usuario no encontrado", "Revise que su correo electrónico y contraseña sean correctos.", "Ok");
          }
        }
      }
    }

    private void SQLConnect()
    {
      var query = "SELECT * FROM dbo.Plataformas";

      using (con = new SqlConnection(constring))
      {
        try
        {
          con.Open();
          using (SqlCommand command = new SqlCommand(query, con))
          {
            using (SqlDataReader reader = command.ExecuteReader())
            {
              while (reader.Read())
              {
                plataformas.Add(new Plataforma(
                    (int)reader["IDplataforma"],
                    (string)reader["platNombre"],
                    0.0f,
                    //float.Parse((string)reader["platPrecio"], CultureInfo.InvariantCulture.NumberFormat),
                    (string)reader["IDplatPelicula"],
                    (string)reader["IDplatSerie"],
                    (string)reader["siteUrl"]
                ));
              }
            }
          }
          msgSQL.Text = "Conectado a la BD";
          msgSQL.TextColor = Color.Green;
        }
        catch (Exception ex)
        {
          Debug.WriteLine(ex.ToString());
          msgSQL.Text = "Error al conectar a la BD";
          msgSQL.TextColor = Color.Red;
        }
      }
    }
  }
}
