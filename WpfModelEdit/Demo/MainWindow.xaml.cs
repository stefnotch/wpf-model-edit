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
using WpfModelEdit;

namespace DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Cd> _cds;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCds();
            btnNew.Click += BtnNew_Click;
            btnDelete.Click += BtnDelete_Click;
            btnTracks.Click += BtnTracks_Click;
        }

        private void BtnTracks_Click(object sender, RoutedEventArgs e)
        {
            if (lbxCds.SelectedIndex < 0 || lbxCds.SelectedIndex >= _cds.Count) return;

            //var tracksWindow = new TrackEditWindow(_cds[lbxCds.SelectedIndex]);
            //tracksWindow.ShowDialog();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbxCds.SelectedIndex < 0 || lbxCds.SelectedIndex >= _cds.Count) return;
            _cds.RemoveAt(lbxCds.SelectedIndex);
            LoadCds();
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            // AddCdWindow addCdWindow = new AddCdWindow();
            // addCdWindow.ShowDialog();

            ModelWindow addCdWindow = new ModelWindow(
                typeof(Cd),
                () => new Cd(),
                (cd) => _cds.Add((Cd)cd)
            );
            addCdWindow.DisplayText = "Add/Create a new CD";
            addCdWindow.ShowDialog();
            LoadCds();
        }

        private void LoadCds()
        {
            _cds = new List<Cd>()
            {
                new Cd()
                {
                    AlbumTitle = "Forty Years of Solitude",
                    Artist = "Can McGee",
                    Tracks = new List<Track>()
                    {
                        new Track()
                        {
                            LeadVocals = "Can McGee",
                            SongWriter = "Can McGee-Jr",
                            Title = "Year One",
                            Year = 2001
                        },
                        new Track()
                        {
                            LeadVocals = "Can McGee",
                            SongWriter = "Can McGee-Jr",
                            Title = "Year Two",
                            Year = 2002
                        },
                        new Track()
                        {
                            LeadVocals = "Can McGee-Jr",
                            SongWriter = "Can McGee-Jr-Jr",
                            Title = "Year Three",
                            Year = 2003
                        }
                    }
                },
                new Cd()
                {
                    AlbumTitle = "Death Metal Overload",
                    Artist = "Fluffy Unicorn",
                    Tracks = new List<Track>()
                },
                new Cd()
                {
                    AlbumTitle = "An album title",
                    Artist = "An artist",
                    Tracks = new List<Track>()
                }
            };
            lbxCds.ItemsSource = _cds;
        }
    }
}
