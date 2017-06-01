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

using WCF.Soap.Client.QuizServiceClient;

namespace ÜbungWCFSoap.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        QuizServiceClient client;
        private QuizFrage aktuelleFrage;

        public MainWindow()
        {
            InitializeComponent();
            client = new QuizServiceClient();
        }

        private void btnGetQuestion_Click(object sender, RoutedEventArgs e)
        {
            aktuelleFrage = client.HoleFrage();
            UpdateView(aktuelleFrage);
        }

        private void btnAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (aktuelleFrage == null)
            {
                return;
            }

            var awIdx = -1;
            if (rbAnswer1.IsChecked ?? false)
            {
                awIdx = 0;
            }
            else if (rbAnswer2.IsChecked ?? false)
            {
                awIdx = 1;
            }
            else if (rbAnswer3.IsChecked ?? false)
            {
                awIdx = 2;
            }

            var result = client.BeantworteFrage(aktuelleFrage.ID, awIdx);
            UpdateView(result);
        }

        private void UpdateView(QuizFrage frage)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Frage " + frage.ID + ": " + frage.Frage);
            sb.AppendLine("------");
            sb.AppendLine("1.) " + frage.Antworten[0]);
            sb.AppendLine("2.) " + frage.Antworten[1]);
            sb.AppendLine("3.) " + frage.Antworten[2]);
            tbQuestion.Text = sb.ToString();

            tbAnswer.Text = "Bitte geben sie ihre Antwort an!";
        }

        private void UpdateView(QuizResultat resultat)
        {
            tbAnswer.Text = resultat.Antwort;
        }

    }
}
