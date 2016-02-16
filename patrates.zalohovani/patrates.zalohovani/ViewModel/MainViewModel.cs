using GalaSoft.MvvmLight;
using patrates.zalohovani.Model;
using patrates.zalohovani.interfaces;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Command; 

namespace patrates.zalohovani.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel == value)
                    return;
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }
        private readonly SettingsViewModel _settingsViewModelService;
        private readonly RestoreViewModel _restoreViewModelService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                if (_welcomeTitle == value)
                {
                    return;
                }

                _welcomeTitle = value;
                RaisePropertyChanged(WelcomeTitlePropertyName);
            }
        }
        
        private bool _showPopUp;
        public bool ShowPopUp
        {
            get { return _showPopUp; }
            set
            {
                if (_showPopUp != value)
                {
                    _showPopUp = value;
                    RaisePropertyChanged("PopOpen");
                }
            }
        }

        private void setupView(ViewModelBase view)
        {            
            CurrentViewModel = view;
        }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _settingsViewModelService = ServiceLocator.Current.GetInstance<SettingsViewModel>();
            _restoreViewModelService = ServiceLocator.Current.GetInstance<RestoreViewModel>();

            settingsViewCommand = new RelayCommand(() => setupView(_settingsViewModelService));
            restoreViewCommand = new RelayCommand(() => setupView(_restoreViewModelService));
            
            _currentViewModel = _settingsViewModelService;
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    WelcomeTitle = item.Title;
                });
        }

        public static RelayCommand settingsViewCommand { get; set; }
        public static RelayCommand restoreViewCommand { get; set; }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}