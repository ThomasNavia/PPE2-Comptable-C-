
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms; 

namespace PPE2_Comptable
{
    public partial class Remboursement : Form
    {
        string IpAddr;
        public static string[] Tab = new string[11];
        public static string VisiteurSelected;

        public Remboursement(string ip)
        {
            InitializeComponent();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            IpAddr = ip; 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Menu VueMenu = new Menu(IpAddr);
            VueMenu.Show(); 
        }

        private void Remboursement_Load(object sender, EventArgs e)
        {
            NameValueCollection postValues = new NameValueCollection();
            postValues["action"] = "Recup_Nom_Visiteur_Fiche_Valide";
            WebClient webClient = new WebClient();
            webClient.UploadValuesCompleted += AutocompleteTextBox;
            webClient.Proxy = null;
            webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);

        }

        void AutocompleteTextBox(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {

            string reponse = Encoding.UTF8.GetString(e.Result);
            JavaScriptSerializer ser = new JavaScriptSerializer();

            var Result = ser.Deserialize<dynamic>(reponse); // désérialize les données Json dans un tableau d'objet 
            foreach (var i in Result)
            {
                comboBox1.Items.Add(i["nom"]); //récupération de la variable contenue au point i du tableau : i["Clé_De_La_Donnée"] 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Menu VueMenu = new Menu(IpAddr);
            VueMenu.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            NameValueCollection postValues = new NameValueCollection();
            postValues["action"] = "Recup_Nom_Visiteur";
            WebClient webClient = new WebClient();
            webClient.UploadValuesCompleted += RecupMoisVisiteur;
            webClient.Proxy = null;
            webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
        }
        void RecupMoisVisiteur(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            string reponse = Encoding.UTF8.GetString(e.Result);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var Result = ser.Deserialize<dynamic>(reponse);
            foreach (var i in Result)
            {
                if (comboBox1.Text == i["nom"])
                {
                    VisiteurSelected = i["id"];
                    NameValueCollection postValues = new NameValueCollection();
                    postValues["action"] = "Recup_Mois_Fiche_Valide";
                    postValues["id"] = i["id"];
                    WebClient webClient = new WebClient();
                    webClient.UploadValuesCompleted += GetMoisFiche;
                    webClient.Proxy = null;
                    webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
                }

            }
        }

        void GetMoisFiche(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            string reponse = Encoding.UTF8.GetString(e.Result);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var Result = ser.Deserialize<dynamic>(reponse);
            comboBox2.Items.Clear();
            foreach (var i in Result)
            {
                comboBox2.Items.Add(TraitementDate(i[0]));
            }
        }
        //Transforme la date du format (yyyyMM) au format (MM/yyyy)
        string TraitementDate(string date)
        {
            string annee = date.Substring(0, 4);
            string mois = date.Substring(4, 2);
            string result = mois + "/" + annee;
            return result;
        }
        //Transforme la date du format (MM/yyyy) au format (yyyyMM)
        string InverseTraitementDate(string date)
        {
            string annee = date.Substring(3, 4);
            string mois = date.Substring(0, 2);
            string result = annee + mois;
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ((comboBox1.Text != ""))
            {
                NameValueCollection postValues = new NameValueCollection();
                postValues["action"] = "Recup_Nom_Visiteur";
                WebClient webClient = new WebClient();
                webClient.UploadValuesCompleted += VerifNomVisiteur;
                webClient.Proxy = null;
                webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
            }
        }
        void VerifNomVisiteur(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            string reponse = Encoding.UTF8.GetString(e.Result);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var Result = ser.Deserialize<dynamic>(reponse);
            foreach (var i in Result)
            {
                if (comboBox1.Text == i["nom"])
                {
                    NameValueCollection postValues = new NameValueCollection();
                    postValues["action"] = "Recup_Frais_Fortfait";
                    postValues["id"] = i["id"];
                    postValues["Date"] = InverseTraitementDate(comboBox2.Text);
                    WebClient webClient = new WebClient();
                    webClient.UploadValuesCompleted += GetFraisForfait;
                    webClient.Proxy = null;
                    webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
                }
            }
        }

        void GetFraisForfait(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            string reponse = Encoding.UTF8.GetString(e.Result);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var Result = ser.Deserialize<dynamic>(reponse);
            System.Type value = Result.GetType();

            if (value == typeof(string))
            {
                MessageBox.Show("Aucunes fiche de frais n'a été trouvé pour ce visiteur et cette date.", "Erreure");
            }

            else
            {
                int index = 0;
                foreach (var i in Result)
                {

                    if (index == 1)
                    {
                        Tab[2] = i[0]; // km 
                    }
                    if (index == 2)
                    {
                        Tab[1] = i[0]; // nuit 
                    }
                    if (index == 3)
                    {
                        Tab[0] = i[0]; //repas 
                    }
                    index++;
                }
            }

            //apelle du serveur pour les frais hors forfait
            NameValueCollection postValues = new NameValueCollection();
            postValues["action"] = "Recup_Frais_Hors_Fortfait";
            postValues["id"] = VisiteurSelected;
            postValues["mois"] = InverseTraitementDate(comboBox2.Text);
            WebClient webClient = new WebClient();
            webClient.UploadValuesCompleted += GetFraisHorsForfait;
            webClient.Proxy = null;
            webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
        }

        void GetFraisHorsForfait(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            string reponse = Encoding.UTF8.GetString(e.Result);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var Result = ser.Deserialize<dynamic>(reponse);
            System.Type value = Result.GetType();
            DataTable Table = new DataTable();
            if (value == typeof(string))
            {
                MessageBox.Show("Aucuns frais hors forfait n'a été trouvés pour ce visiteur et cette date.", "Erreure");
            }
            else
            {
                foreach (var i in Result)
                {

                        Tab[3] = i[1]; // libelle
                        Tab[4] = i[0]; // montant 
                        Tab[5] = i[2]; //Date
                    double PrixNui = 80;
                    double PrixRepas = 25;
                    double PrixVehicule = 0.62;
                    CrystalReport3 op = new CrystalReport3();
                    op.SetParameterValue("Nom", comboBox1.Text);
                    op.SetParameterValue("Mois", comboBox2.Text);
                    op.SetParameterValue("Qnuit",Tab[1].Replace('.', ','));
                    op.SetParameterValue("Qrepas",Tab[0].Replace('.', ','));
                    op.SetParameterValue("Qvehicule", Tab[2].Replace('.', ','));
                    op.SetParameterValue("MntVehicule", PrixVehicule);
                    op.SetParameterValue("MntRepas", PrixRepas);
                    op.SetParameterValue("MntNuit", PrixNui);
                    op.SetParameterValue("MontantHF", Tab[4].Replace('.',','));
                    op.SetParameterValue("LibelleHF", Tab[3]);
                    op.SetParameterValue("DateHF", Tab[5]);
                    crystalReportViewer1.ReportSource = op;
                    crystalReportViewer1.Refresh();
                    break;
                }
            }
        }
    }
}
