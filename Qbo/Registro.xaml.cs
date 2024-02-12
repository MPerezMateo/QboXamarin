using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Qbo
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Registro : ContentPage
	{
        SqlConnection con = MainPage.con;
        int userId;
        public Registro ()
		{
			InitializeComponent ();
		}

		void registrarse(object sender, EventArgs args)
		{
			if (validation())
			{
                crearUsuario();
                crearPerfil();
                Navigation.PushAsync(new MainPage());
            }

        }

		bool validation()
		{
            string EmailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
			if (!Regex.IsMatch(entryEmail.Text, EmailPattern))
			{
                entryErrMsg.IsVisible = true;
                entryErrMsg.Text = "El email proporcionado no es válido";
				return false;
			}
			else if (entryPassword.Text.Length < 5)
			{
                entryErrMsg.IsVisible = true;
                entryErrMsg.Text = "La contraseña es demasiado corta, debe tener al menos 5 caracteres";
                return false;
            }
            return true;
        }

        void crearUsuario()
		{
            var query = $"insert into Users(email, username, password, isActive, CreatedAt, LastAccess) OUTPUT INSERTED.ID " +
                $"VALUES ('{entryEmail.Text}', '{entryUserName.Text}', '{entryPassword.Text}', 1, GETDATE(), GETDATE())";
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        object generatedId = command.ExecuteScalar();
                        if (generatedId != null)
                        {
                            userId = Convert.ToInt32(generatedId);
                            Console.WriteLine($"Newly inserted ID: {userId}");
                        }
                        else
                        {
                            Console.WriteLine("Insert failed or no ID was generated.");
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    DisplayAlert("Error creando tu usuario", "Ha ocurrido un error creando tu usuario, por favor prueba con otras credenciales o inténtalo más tarde", "Ok");
                }
            }
        }

        void crearPerfil()
        { 
            var query = $"insert into Perfiles values({userId},NULL, NULL, \'\')";
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} row(s) inserted.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    DisplayAlert("Error creando tu perfil", "Ha ocurrido un error creando tu perfil, por favor prueba con otras credenciales o inténtalo más tarde", "Ok");
                }
            }
        }
    }
}