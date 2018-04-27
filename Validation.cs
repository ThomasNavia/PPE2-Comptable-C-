
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

namespace PPE2_Comptable
{
    public partial class Validation : Form
    {
        string IpAddr;
        public static string VisiteurSelected; 
        public Validation(string addr)
        {
            InitializeComponent();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            IpAddr = addr; 
            
        }

        private void Validation_Load(object sender, EventArgs e)
        {
            NameValueCollection postValues = new NameValueCollection();
            postValues["action"] = "Recup_Nom_Visiteur";
            WebClient webClient = new WebClient();
            webClient.UploadValuesCompleted += AutocompleteTextBox; 
            webClient.Proxy = null; 
            webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
        }

        // Insert les nom des visiteurs possédant une fiche dans une liste déroulante
        void AutocompleteTextBox(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {

            string reponse = Encoding.UTF8.GetString(e.Result); 
            JavaScriptSerializer ser = new JavaScriptSerializer();

            var Result = ser.Deserialize<dynamic>(reponse); // désérialize les données Json dans un tableau d'objet 
            foreach(var i in Result) 
            {
                comboBox1.Items.Add(i["nom"]); //récupération de la variable contenue au point i du tableau : i["Clé_De_La_Donnée"] 
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            if ((comboBox1.Text != ""))
            {
                NameValueCollection postValues = new NameValueCollection();
                postValues["action"] = "Recup_Nom_Visiteur";
                WebClient webClient = new WebClient();
                webClient.UploadValuesCompleted += VerifNomVisiteur;
                webClient.Proxy = null; 
                webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
                Clean();
            }
        }
        void VerifNomVisiteur(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            string reponse = Encoding.UTF8.GetString(e.Result);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var Result = ser.Deserialize<dynamic>(reponse);
            foreach (var i in Result)  
            {
                if(comboBox1.Text == i["nom"])
                {
                    NameValueCollection postValues = new NameValueCollection();
                    postValues["action"] = "Recup_Frais_Fortfait";
                    postValues["id"] = i["id"] ;
                    postValues["Date"] = InverseTraitementDate(comboBox3.Text); 
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
                    if (index == 0)
                    {
                        textBox4.Text = i[0];
                    }
                    if (index == 1)
                    {
                        textBox5.Text = i[0];
                    }
                    if (index == 2)
                    {
                        textBox3.Text = i[0];
                    }
                    if (index == 3)
                    {
                        textBox2.Text = i[0];
                    }
                    index++;
                }
            }
            
            //apelle du serveur pour les frais hors forfait
            NameValueCollection postValues = new NameValueCollection();
            postValues["action"] = "Recup_Frais_Hors_Fortfait";
            postValues["id"] = VisiteurSelected;
            postValues["mois"] = InverseTraitementDate(comboBox3.Text);
            WebClient webClient = new WebClient();
            webClient.UploadValuesCompleted += GetFraisHorsForfait;
            webClient.Proxy = null;
            webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
        }

        // récupère les données Json et les envoies dans une DataGridView. 
        void GetFraisHorsForfait(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            string reponse = Encoding.UTF8.GetString(e.Result);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var Result = ser.Deserialize<dynamic>(reponse);
            System.Type value = Result.GetType();
            DataTable Table = new DataTable();
            int Val1 = 0; 
            if (value == typeof(string))
            {
                MessageBox.Show("Aucuns frais hors forfait n'a été trouvés pour ce visiteur et cette date.", "Erreure");
            }
            else
            {
                foreach (var i in Result)
                {
                    if(Val1 == 0)
                    {
                        Table.Columns.Add("Libelle", typeof(string));
                        Table.Columns.Add("Montant", typeof(string));
                        Table.Columns.Add("Date", typeof(string));
                        Table.Columns.Add("Validation", typeof(bool)); 
                        Table.Rows.Add(i[1], i[0], i[2], true);
                        dataGridView1.DataSource = Table;
                        Val1++;
                    }
                    else
                    {
                        Table.Rows.Add(i[1], i[0], i[2], true);
                        dataGridView1.DataSource = Table; 
                    }
                }
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; 
            }
        }
        //Apelle le serveur pour obtenir tout les noms des visiteur. 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            NameValueCollection postValues = new NameValueCollection();
            postValues["action"] = "Recup_Nom_Visiteur";
            WebClient webClient = new WebClient();
            webClient.UploadValuesCompleted += RecupMoisVisiteur;
            webClient.Proxy = null;
            webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
            Clean(); 

        }
        // Vide tout les éléments de la form
        private void Clean()
        {
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            dataGridView1.DataSource = null; 
        }
        //Apelle le serveur pour obtenir les mois en fonction de l'utilisateur sélectionné. 
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
                    postValues["action"] = "Recup_Mois_Fiche";
                    postValues["id"] = i["id"];
                    WebClient webClient = new WebClient();
                    webClient.UploadValuesCompleted += GetMoisFiche;
                    webClient.Proxy = null;
                    webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
                }

            }
        }
        //Ajoute les Visiteur possédant une fiche à la combobox visiteur
        void GetMoisFiche(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            string reponse = Encoding.UTF8.GetString(e.Result);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var Result = ser.Deserialize<dynamic>(reponse);
            comboBox3.Items.Clear();
            foreach (var i in Result)
            {
                comboBox3.Items.Add(TraitementDate(i[0])); 
            }
        }

        //Transforme la date du format (yyyyMM) au format (MM/yyyy)
        string TraitementDate(string date)
        {
            string annee =  date.Substring(0, 4);
            string mois = date.Substring(4, 2);
            string result = mois + "/" + annee;
            return result; 
        }
        //Transforme la date du format (MM/yyyy) au format (yyyyMM)
        string InverseTraitementDate(string date)
        {
            string annee = date.Substring(3, 4);
            string mois = date.Substring(0, 2);
            string result =  annee + mois;
            return result;
        }

        //Met à jour toutes les données de la fiche
        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "")
            {
                DialogResult dialogResult =  MessageBox.Show("êtes vous sûre de vouloir modifier ces valeurs ? ", "Confirmation avant modification", MessageBoxButtons.YesNo); 
                if(dialogResult == DialogResult.Yes)
                {
                    NameValueCollection postValues = new NameValueCollection();
                    postValues["action"] = "MAJ_Frais_Forfait";
                    postValues["ETP"] = textBox4.Text;
                    postValues["KM"] = textBox5.Text;
                    postValues["NUI"] = textBox3.Text;
                    postValues["REP"] = textBox2.Text;
                    postValues["mois"] = InverseTraitementDate(comboBox3.Text);
                    postValues["id"] = VisiteurSelected;
                    WebClient webClient = new WebClient();
                    webClient.Proxy = null;
                    webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);
                }
            }   
            else
            {
                MessageBox.Show("Toutes les cases doivent être remplies.", "Erreure de saisie");
            }
        }

        // réinitialisation des élément après changement d'une valeur
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Clean(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("êtes vous sûre de vouloir mettre la fiche à l'état : Validé et mise en paiement ? ", "Confirmation avant validdation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                NameValueCollection postValues1 = new NameValueCollection();
                postValues1["action"] = "MAJ_Etat_Fiche";
                postValues1["total"] = CalculTotal(); 
                postValues1["mois"] = InverseTraitementDate(comboBox3.Text);
                postValues1["id"] = VisiteurSelected;
                WebClient webClient = new WebClient();
                webClient.Proxy = null;
                webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "")
            {
                DialogResult dialogResult = MessageBox.Show("êtes vous sûre de vouloir enregistrer les valeurs ? ", "Confirmation avant modification", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    NameValueCollection postValues = new NameValueCollection();
                    postValues["action"] = "MAJ_Frais_Forfait";
                    postValues["ETP"] = textBox4.Text;
                    postValues["KM"] = textBox5.Text;
                    postValues["NUI"] = textBox3.Text;
                    postValues["REP"] = textBox2.Text;
                    postValues["mois"] = InverseTraitementDate(comboBox3.Text);
                    postValues["id"] = VisiteurSelected;
                    WebClient webClient = new WebClient();
                    webClient.Proxy = null;
                    webClient.UploadValuesAsync(new Uri(IpAddr), "POST", postValues);


                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        bool Verifcheck = Convert.ToBoolean(dataGridView1.Rows[i].Cells[3].Value);
                        if (Verifcheck == false)
                        {
                            string libelle = (string)dataGridView1.Rows[i].Cells[0].Value;
                            NameValueCollection postValues1 = new NameValueCollection();
                            postValues1["action"] = "MAJ_Frais_Hors_Forfait"; 
                            postValues1["libelle"] = libelle; 
                            postValues1["mois"] = InverseTraitementDate(comboBox3.Text);
                            postValues1["id"] = VisiteurSelected;
                            WebClient webClient1 = new WebClient();
                            webClient1.Proxy = null;
                            webClient1.UploadValuesAsync(new Uri(IpAddr), "POST", postValues1);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Toutes les cases doivent être remplies.", "Erreure de saisie");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Menu VueMenu = new Menu(IpAddr);
            VueMenu.Show(); 
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private string CalculTotal()
        {
            double repas =  double.Parse(textBox2.Text);
            double nuit = double.Parse(textBox3.Text);
            double etape = double.Parse(textBox4.Text);
            double km = double.Parse(textBox5.Text);
            double FraisHF = 0; 
            for(int i = 0; i< dataGridView1.RowCount - 1 ; i++)
            {
                string value = dataGridView1.Rows[i].Cells[1].Value.ToString();
                string newvalue =  value.Replace('.', ',');
                FraisHF += double.Parse(newvalue); 
                }
            double total = (repas * 25) + (nuit * 80) + (etape * 110) + (km * 0.62) + FraisHF;
            return total.ToString(); 
        }

    }
}
