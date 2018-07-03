using Couchbase.Lite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EMonthly
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            Couchbase.Lite.Support.NetDesktop.Activate();
            if (e.Args.Length > 0 && e.Args[0].ToLowerInvariant() == "/clean")
            {
                Database.Delete("todo", null);
            }
        }
    }
}
