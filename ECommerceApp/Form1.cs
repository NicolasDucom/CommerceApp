using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ECommerceApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        { 
            xmlRead();
        }

        /// <summary>
        /// Lis le fichier XML et rempli de datagridview
        /// </summary>
        private void xmlRead()
        {
            try
            {
                XDocument document = XDocument.Load("products.xml");
                var products = from p in document.Descendants("product")
                               select new
                                   {
                                       id = p.Attribute("id").Value,
                                       libelle = p.Element("libelle").Value,
                                       prix = p.Element("prix").Value,
                                       stock = p.Element("stock").Value,
                                       couleur = p.Element("couleur").Value,
                                       idPack = p.Element("idpack").Value,
                                   };
                foreach (var p in products)
                {
                    dataGridView1.Rows.Add(new object[] { p.id, p.libelle, p.prix, p.stock, p.couleur, p.idPack });
                }
                dataGridView1.AutoResizeColumns();
            }
            catch (Exception)
            {
                label2.Text = "Erreur lors de la lecture du fichier !";
            }
        }

        /// <summary>
        /// Cherche les elements d'un pack dans le fichier XML et les rajoute a un pack dans le fichier paniers
        /// </summary>
        private void buyPackById(string id, string pack)
        {
            try
            {
                XDocument document = XDocument.Load("products.xml");
                var products = from p in document.Descendants("product")
                               where p.Element("idpack").Value == pack
                               select new
                               {
                                   id = p.Attribute("id").Value,
                                   libelle = p.Element("libelle").Value,
                                   prix = p.Element("prix").Value,
                                   stock = p.Element("stock").Value,
                                   couleur = p.Element("couleur").Value,
                                   idPack = p.Element("idpack").Value,
                               };
                if (!File.Exists("paniers.xml"))
                {
                    XElement XML_Nouveau = new XElement("Paniers");
                    XML_Nouveau.Save("paniers.xml");
                }
                string nomDuPack = nomPack(pack);
                string prixDuPack = prixPack(pack);

                XDocument document2 = XDocument.Load("paniers.xml");
                XElement newPack = new XElement("Pack",
                    new XElement("Nom", nomDuPack),
                    new XElement("Prix", prixDuPack),
                    new XElement("Produits")
                 );

                foreach (var p in products)
                {
                    newPack.Element("Produits").Add(
                        new XElement("product",
                            new XAttribute("id", p.id),
                            new XElement("libelle", p.libelle),
                            new XElement("prix", p.prix),
                            new XElement("stock", p.stock),
                            new XElement("couleur", p.couleur),
                            new XElement("idpack", p.idPack)
                            )
                        );
                }
                document2.Element("Paniers").Add(newPack);
                try
                {
                    document2.Save("paniers.xml");
                    label2.Text = nomDuPack + " ajouté !";
                }
                catch (Exception)
                {
                    label2.Text = "Erreur lors de l'ajout du pack!";
                }
            }
            catch (Exception)
            {
                label2.Text = "Erreur lors de l'ouverture du fichier XML";
                return;
            }
           
        }

        /// <summary>
        /// Retourne le nom associé à un pack
        /// </summary>
        private string nomPack(string pack)
        {
            string nomDuPack = "";
            switch (pack)
            {
                case "1":
                    nomDuPack = "Pack cuivre";
                    break;
                case "2":
                    nomDuPack = "Pack argent";
                    break;
                case "3":
                    nomDuPack = "Pack or";
                    break;
                case "4":
                    nomDuPack = "Pack platine";
                    break;
                default :
                    nomDuPack =  "";
                    break;
            }
            return nomDuPack;
        }

        /// <summary>
        /// retourne le prix associé à un pack
        /// </summary>
        private string prixPack(string pack)
        {
            string prixDuPack = "";
            switch (pack)
            {
                case "1":
                    prixDuPack = "19 €";
                    break;
                case "2":
                    prixDuPack = "29 €";
                    break;
                case "3":
                    prixDuPack = "39 €";
                    break;
                case "4":
                    prixDuPack = "99 €";
                    break;
                default:
                    prixDuPack = "";
                    break;
            }
            return prixDuPack;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                buyPackById(dataGridView1.SelectedRows[0].Cells[0].Value.ToString(), dataGridView1.SelectedRows[0].Cells[5].Value.ToString());
            }
            catch (Exception)
            {
                label2.Text = "Pas de pack selectionné !";
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string idpack = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
            button1.Text = "Acheter le " + nomPack(idpack) + " à " + prixPack(idpack);
        }
    }
}
