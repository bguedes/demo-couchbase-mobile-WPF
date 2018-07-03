using Couchbase.Lite;
using Couchbase.Lite.DI;
using Couchbase.Lite.Logging;
using Couchbase.Lite.Sync;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EMonthly
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Database Database { get; private set; }

        #region Constants

        private const string DbName = "emonthly";
        private static readonly Uri SyncGatewayUrl = new Uri("ws://localhost:4984/emonthly/");
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MainWindow));

        #endregion

        private static Replicator _replication;

        public MainWindow()
        {
            InitializeComponent();

            log4net.Config.XmlConfigurator.Configure();

            Couchbase.Lite.Support.NetDesktop.Activate();

            var config = new DatabaseConfiguration()
            {
                Directory = AppDomain.CurrentDomain.BaseDirectory
            };

            Database = new Database(DbName, config);

            var configSync = new ReplicatorConfiguration(Database, new URLEndpoint(SyncGatewayUrl))
            {
                ReplicatorType = ReplicatorType.PushAndPull,
                Continuous = true
            };

            configSync.Authenticator = new BasicAuthenticator("emonthly", "password");

            _replication = new Replicator(configSync);
            _replication.AddChangeListener((sender, args) =>
            {
                Console.WriteLine(args.Status.Activity);
            });
            _replication.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region Data Control

            if (!String.IsNullOrEmpty(OperationTextBox.Text) && OperationComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Operation value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(MaintenanceTextBox.Text) && MaintenanceComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Maintenance value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(TechAssistanceTextBox.Text) && TechAssistanceComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Tech Assistance value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(TechPubTextBox.Text) && TechPubComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Tech Pub value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(LogisticsTextBox.Text) && LogisticsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Logistics value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(CSCTextBox.Text) && CSCComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a CSC value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(CommunicationTextBox.Text) && CommunicationComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a CSC value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(CommercialSupportTextBox.Text) && CommercialSupportComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Commercial Support value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(CostOfOperationTextBox.Text) && CostOfOperationComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Cost Of Operation value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(WarrentyAdministrationTextBox.Text) && WarrentyAdministrationComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Administration value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(OEMServiceTextBox.Text) && OEMServiceComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a OEM Service value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!String.IsNullOrEmpty(VendorManagementTextBox.Text) && VendorManagementComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Vendor Management value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CustomerComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Customer for this Report.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            #endregion
                
            string id = null;
            using (var mutableDoc = new MutableDocument())
            {
                if (!string.IsNullOrWhiteSpace(GlobalCommentTextBox.Text))
                {
                    mutableDoc.SetString("commentaireGenerale", GlobalCommentTextBox.Text.Trim().ToLower());
                }

                mutableDoc.SetString("customer", CustomerComboBox.Text.Trim().ToLower());

                mutableDoc.SetString("type", "report");
                mutableDoc.SetString("mois", DateTime.Now.ToString("MMMMMMMMMMMMM").ToLower());

                var rubriques = new MutableArrayObject();
                mutableDoc.SetArray("rubriques", rubriques);

                if(OperationComboBox.SelectedIndex != -1)
                {
                    var operation = new MutableDictionaryObject();
                    operation.SetString("nom", "operation")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)OperationComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(OperationTextBox.Text))
                    {
                        operation.SetString("commentaire", OperationTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(operation);

                }

                if (MaintenanceComboBox.SelectedIndex != -1)
                {
                    var maintenance = new MutableDictionaryObject();
                    maintenance.SetString("nom", "maintenance")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)MaintenanceComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(MaintenanceTextBox.Text))
                    {
                        maintenance.SetString("commentaire", MaintenanceTextBox.Text.ToLower().TrimEnd().TrimStart());
                    } 
                    rubriques.AddDictionary(maintenance);
                }

                if (TechAssistanceComboBox.SelectedIndex != -1)
                {
                    var techAssistance = new MutableDictionaryObject();
                    techAssistance.SetString("nom", "techAssistance")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)TechAssistanceComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(TechAssistanceTextBox.Text))
                    {
                        techAssistance.SetString("commentaire", TechAssistanceTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }    
                    rubriques.AddDictionary(techAssistance);
                }

                if (TechPubComboBox.SelectedIndex != -1)
                {
                    var techPub = new MutableDictionaryObject();
                    techPub.SetString("nom", "techPub")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)TechPubComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(TechPubTextBox.Text))
                    {
                        techPub.SetString("commentaire", TechPubTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(techPub);
                }

                if (LogisticsComboBox.SelectedIndex != -1)
                {
                    var logistics = new MutableDictionaryObject();
                    logistics.SetString("nom", "logistics")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)LogisticsComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(LogisticsTextBox.Text))
                    {
                        logistics.SetString("commentaire", LogisticsTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(logistics);
                }

                if (CSCComboBox.SelectedIndex != -1)
                {
                    var csc = new MutableDictionaryObject();
                    csc.SetString("nom", "csc")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)CSCComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(CSCTextBox.Text))
                    {
                        csc.SetString("commentaire", CSCTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(csc);
                }

                if (CommunicationComboBox.SelectedIndex != -1)
                {
                    var communication = new MutableDictionaryObject();
                    communication.SetString("nom", "communication")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)CommunicationComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(CommunicationTextBox.Text))
                    {
                        communication.SetString("commentaire", CommunicationTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(communication);
                }

                if (CommercialSupportComboBox.SelectedIndex != -1)
                {
                    var commercialSupport = new MutableDictionaryObject();
                    commercialSupport.SetString("nom", "commercialSupport")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)CommercialSupportComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(CommercialSupportTextBox.Text))
                    {
                        commercialSupport.SetString("commentaire", CommercialSupportTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(commercialSupport);
                }

                if (CostOfOperationComboBox.SelectedIndex != -1)
                {
                    var CostOfOperation = new MutableDictionaryObject();
                    CostOfOperation.SetString("nom", "costOfOperation")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)CostOfOperationComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(CostOfOperationTextBox.Text))
                    {
                        CostOfOperation.SetString("commentaire", CostOfOperationTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(CostOfOperation);
                }

                if (WarrentyAdministrationComboBox.SelectedIndex != -1)
                {
                    var WarrentyAdministration = new MutableDictionaryObject();
                    WarrentyAdministration.SetString("nom", "warrentyAdministration")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)WarrentyAdministrationComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(WarrentyAdministrationTextBox.Text))
                    {
                        WarrentyAdministration.SetString("commentaire", WarrentyAdministrationTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(WarrentyAdministration);
                }

                if (OEMServiceComboBox.SelectedIndex != -1)
                {
                    var oemService = new MutableDictionaryObject();
                    oemService.SetString("nom", "oemService")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)OEMServiceComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(OEMServiceTextBox.Text))
                    {
                        oemService.SetString("commentaire", OEMServiceTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(oemService);
                }

                if (VendorManagementComboBox.SelectedIndex != -1)
                {
                    var vendorManagement = new MutableDictionaryObject();
                    vendorManagement.SetString("nom", "vendorManagement")
                        .SetInt("note", Int32.Parse(((ComboBoxItem)VendorManagementComboBox.SelectedItem).Content.ToString()));
                    if (!String.IsNullOrEmpty(VendorManagementTextBox.Text))
                    {
                        vendorManagement.SetString("commentaire", VendorManagementTextBox.Text.ToLower().TrimEnd().TrimStart());
                    }
                    rubriques.AddDictionary(vendorManagement);
                }

                Database.Save(mutableDoc);
                id = mutableDoc.Id;
            }

            Console.WriteLine("Document saved with : " + id);
            Logger.Info("Document saved with : " + id);

            MessageBox.Show("Report saved with : " + id, 
                "Report Saved", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);

        }
    }

}