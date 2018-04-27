
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System;
using System.Text;
using System.Web.Script.Serialization;

namespace PPE2_Comptable
{
    public partial class Connexion : Form
    {
        static string IpAddr;
        public Connexion()
        {
            InitializeComponent();
            IpAddr = RefConnexion();

        }

        private void Connexion_Load(object sender, EventArgs e)
        {

        }
        public string RefConnexion()
        {
            string path = System.Windows.Forms.Application.StartupPath; //chemin du repertoire
            string fn = "Connexion.txt"; //nom du fichier 

            // {#1} Chemin de location du fichier stocké dans la variable filename
            string filename = System.IO.Path.Combine(path, fn);


                using (var sr = new StreamReader(filename, true))
                {
                    // {#3} récupération de la date dans le fichier
                    string LienConnexion = sr.ReadLine();
                    sr.Close();   //Fermeture du lecteur
                    sr.Dispose(); // libération de la mémoire
                    return LienConnexion;
                }
            }
        


        void webClient_UploadValuesCompleted(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            try
            {
                string reponse = Encoding.UTF8.GetString(e.Result);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                var r = ser.Deserialize<dynamic>(reponse);
                if (r == "Ok")
                {
                    SwitchView();
                }
                else
                {
                    MessageBox.Show("Mot de passe ou identifiant incorrect");
                }
            }
            catch
            {
                MessageBox.Show("Connexion à la base de données impossible.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NameValueCollection postValues = new NameValueCollection();
            postValues["action"] = "connect_comptable";
            postValues["Identifiant"] = identifiant.Text;
            postValues["Mot_de_passe"] = Mdp.Text; 
            WebClient webClient = new WebClient();
            webClient.UploadValuesCompleted += webClient_UploadValuesCompleted; 
            webClient.Proxy = null; 
            webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
        }

        void SwitchView()
        {
            this.Hide();
            Menu op = new Menu(IpAddr);
            op.Show();
        }
    }
}
