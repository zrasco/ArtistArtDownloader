using ArtistArtDownloader.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistArtDownloader.ViewModels
{


    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ArtistList = new ObservableCollection<ArtistEntry>();
        }

        /// <summary>
        /// The <see cref="StatusBarText" /> property's name.
        /// </summary>
        public const string StatusBarTextPropertyName = "StatusBarText";

        private string _statusBarText = null;

        /// <summary>
        /// Sets and gets the StatusBarText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string StatusBarText
        {
            get
            {
                return _statusBarText;
            }

            set
            {
                if (_statusBarText == value)
                {
                    return;
                }

                _statusBarText = value;
                RaisePropertyChanged(StatusBarTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedFolder" /> property's name.
        /// </summary>
        public const string SelectedFolderPropertyName = "SelectedFolder";

        private string _selectedFolder = null;

        /// <summary>
        /// Sets and gets the SelectedFolder property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }

            set
            {
                if (_selectedFolder == value)
                {
                    return;
                }

                _selectedFolder = value;
                RaisePropertyChanged(SelectedFolderPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ArtistList" /> property's name.
        /// </summary>
        public const string ArtistListPropertyName = "ArtistList";

        private ObservableCollection<ArtistEntry> _artistList = null;

        /// <summary>
        /// Sets and gets the ArtistList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ArtistEntry> ArtistList
        {
            get
            {
                return _artistList;
            }

            set
            {
                if (_artistList == value)
                {
                    return;
                }

                _artistList = value;
                RaisePropertyChanged(ArtistListPropertyName);
            }
        }
    }
}
