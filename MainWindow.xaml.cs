using ArtistArtDownloader.Models;
using ArtistArtDownloader.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArtistArtDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Got music note icon from: https://www.flaticon.com/premium-icon/music-notes_1895657
        // Got magnifying glass icon from: https://www.freepik.com/free-icon/magnifying-glass_770240.htm

        // Magic strings. Some of these are used for status checks. I should have a status enum or something more elegant, but will get around to that during a refactor.
        private const string MSG_NO_SUBDIRECTORIES_FOUND = "(No subdirectories found.)";
        private const string MSG_LOAD_FOLDER = "Please select a folder...";
        private const string MSG_HAS_ARTWORK = "Contains artwork";
        private const string MSG_NO_ARTWORK = "No artwork";

        private MainWindowViewModel _vm = null;

        public MainWindow()
        {
            InitializeComponent();

            // ViewModel has been set so we'll reference it here
            _vm = this.DataContext as MainWindowViewModel;
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    _vm.SelectedFolder = fbd.SelectedPath;

                    RefreshFromTargetFolder(fbd.SelectedPath);
                }
            }
        }

        void RefreshFromTargetFolder(string folder)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folder);

            if (di.EnumerateDirectories().Count() > 0)
            {
                _vm.ArtistList.Clear();

                foreach (System.IO.DirectoryInfo d in di.GetDirectories())
                {
                    if (System.IO.File.Exists(d.FullName + @"\folder.jpg") || System.IO.File.Exists(d.FullName + @"\folder.png"))
                    {
                        _vm.ArtistList.Add(new ArtistEntry() { Name = d.Name, Status = MSG_HAS_ARTWORK, FullPath = d.FullName });
                    }
                    else
                        _vm.ArtistList.Add(new ArtistEntry() { Name = d.Name, Status = MSG_NO_ARTWORK, FullPath = d.FullName });
                }
            }
            else
            {
                _vm.ArtistList.Clear();
                _vm.ArtistList.Add(new ArtistEntry() { Name = MSG_NO_SUBDIRECTORIES_FOUND });
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            var artistEntries = _vm.ArtistList.Where(x => x.Status == MSG_NO_ARTWORK);

            if (_vm.ArtistList.Count > 0 &&
                _vm.ArtistList[0].Name != MSG_NO_SUBDIRECTORIES_FOUND &&
                _vm.ArtistList[0].Name != MSG_LOAD_FOLDER &&
                artistEntries.Count() > 0)
            {
                SetStatusBarText("Obtaining images...");

                ResultsWindow resultWindow = new ResultsWindow();
                resultWindow.Owner = this;
                resultWindow.ViewModel.ArtistEntries = artistEntries.ToList();

                resultWindow.ShowDialog();

                RefreshFromTargetFolder(_vm.SelectedFolder);

                if (resultWindow.DialogResult == true)
                    SetStatusBarText("Successfully completed.");
                else
                    SetStatusBarText("Not completed, please try again.");
            }
            else if (_vm.ArtistList.Count == 0 ||
                _vm.ArtistList[0].Name == MSG_NO_SUBDIRECTORIES_FOUND ||
                _vm.ArtistList[0].Name == MSG_LOAD_FOLDER)
                SetStatusBarText("Please select a folder with artist subfolders.");
            else if (artistEntries.Count() == 0)
                SetStatusBarText("All folders already have artwork... I'm out of a job!");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetStatusBarText("Ready.");
            _vm.SelectedFolder = MSG_LOAD_FOLDER;
            _vm.ArtistList.Add(new ArtistEntry() { Name = MSG_LOAD_FOLDER });
        }

        private void MenuItemDeleteArtwork_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.ArtistList.Count > 0 && _vm.ArtistList[0].Name != MSG_NO_SUBDIRECTORIES_FOUND && _vm.ArtistList[0].Name != MSG_LOAD_FOLDER)
            {
                var selectedArtists = _vm.ArtistList.Where(x => x.IsSelected);

                if (selectedArtists.Count() > 0)
                {
                    if (System.Windows.MessageBox.Show($"Are you sure you want to delete the artwork for these {selectedArtists.Count()} artists?",
                        "Confirm deletion",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            foreach (var selectedArtist in selectedArtists)
                            {
                                string jpgFile = selectedArtist.FullPath + @"\folder.jpg";
                                string pngFile = selectedArtist.FullPath + @"\folder.png";

                                System.IO.File.Delete(jpgFile);
                                System.IO.File.Delete(pngFile);

                                SetStatusBarText($"Deleted artwork for artist {selectedArtist.Name}.");
                            }

                            SetStatusBarText("Done deleting artwork.");
                            RefreshFromTargetFolder(_vm.SelectedFolder);
                        }
                        catch (Exception ex)
                        {
                            SetStatusBarText($"Unable to delete artwork. {ex.Message}", true);
                        }
                    }
                }
                else
                {
                    SetStatusBarText("No artists were selected for deletion, please try again.");
                }
            }
            else
                SetStatusBarText("Nothing to delete! Load some stuff first.");
            
        }

        private void SetStatusBarText(string message, bool isError = false)
        {
            if (isError)
                StatusBarControl.Foreground = System.Windows.Media.Brushes.Red;
            else
                StatusBarControl.Foreground = System.Windows.Media.Brushes.Black;

            _vm.StatusBarText = message;
        }
    }
}
