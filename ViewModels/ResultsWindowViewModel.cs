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
    public class ImageResultData
    {
        public string ImageURL { get; set; }
        public string ThumbnailURL { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
    public class ResultsWindowViewModel : ViewModelBase
    {
        public ResultsWindowViewModel()
        {
            ImageResults = new ObservableCollection<ImageResultData>();
        }

        /// <summary>
        /// The <see cref="ResultsHeader" /> property's name.
        /// </summary>
        public const string ResultsHeaderPropertyName = "ResultsHeader";

        private string _resultsHeader = null;

        /// <summary>
        /// Sets and gets the ResultsHeader property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ResultsHeader
        {
            get
            {
                return _resultsHeader;
            }

            set
            {
                if (_resultsHeader == value)
                {
                    return;
                }

                _resultsHeader = value;
                RaisePropertyChanged(ResultsHeaderPropertyName);
            }
        }
        public List<ArtistEntry> ArtistEntries = null;

        /// <summary>
        /// The <see cref="ImageResults" /> property's name.
        /// </summary>
        public const string ImageResultsPropertyName = "ImageResults";

        private ObservableCollection<ImageResultData> _imageResultData = null;

        /// <summary>
        /// Sets and gets the ImageResults property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ImageResultData> ImageResults
        {
            get
            {
                return _imageResultData;
            }

            set
            {
                if (_imageResultData == value)
                {
                    return;
                }

                _imageResultData = value;
                RaisePropertyChanged(ImageResultsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SearchTerm" /> property's name.
        /// </summary>
        public const string SearchTermPropertyName = "SearchTerm";

        private string _searchTerm = null;

        /// <summary>
        /// Sets and gets the SearchTerm property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }

            set
            {
                if (_searchTerm == value)
                {
                    return;
                }

                _searchTerm = value;
                RaisePropertyChanged(SearchTermPropertyName);
            }
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
    }
}
