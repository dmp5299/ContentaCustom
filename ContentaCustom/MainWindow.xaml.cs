using System;
using System.Collections.Generic;
using System.Linq;
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
using ContentaCustom.ContentaClasses;
using System.Windows.Forms;
using System.Threading;

namespace ContentaCustom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PCMClientLib.PCMConnection conn;
        PCMClientLib.IPCMcommand cmd;

        public MainWindow()
        {
            InitializeComponent();
            //System.Windows.DataObject.AddPastingHandler(ModuleText, OnPaste);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.UnicodeText, true);
            if (!isText) return;

            string text = e.SourceDataObject.GetData(System.Windows.DataFormats.UnicodeText) as string;
            ModuleText.Text = text;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string host = HostText.Text;
                int socket = Int32.Parse(SocketText.Text);
                string database = DatabaseText.Text;
                conn = new PCMClientLib.PCMConnection();
                cmd = conn.ConnectGetCommand((short)socket, host, database, "sysadmin", "manager", "XyGACTest", 0);

                System.Windows.Forms.MessageBox.Show("Connected!");
                ModuleGrid.Visibility = Visibility.Visible;
                ConnectGrid.Visibility = Visibility.Collapsed;
                //ContentaDelete del = new ContentaDelete("SB-A-12-34-5678-12A-930A-C", conn, cmd);
                //del.Delete();

            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string module = ModuleText.Text;
                if(module == "")
                {
                    throw new ArgumentNullException("Module");
                }
                char[] splitChars = { '|', ',', ' ','\r','\n' };
                string[] modules = module.Split(splitChars);
                
                 ContentaDelete del;
                 foreach (string m in modules)
                 {
                     del = new ContentaDelete(conn, cmd, m);
                     del.Delete();

                 }
                 System.Windows.Forms.MessageBox.Show("Objects deleted");
            }
            catch(ArgumentNullException a)
            {
                System.Windows.Forms.MessageBox.Show(a.Message);
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            ModuleText.Text = "";
        }
    }
}
