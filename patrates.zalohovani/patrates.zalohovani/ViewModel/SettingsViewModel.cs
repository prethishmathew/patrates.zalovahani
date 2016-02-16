using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using patrates.zalohovani.interfaces;
using System.Collections.ObjectModel;
using patrates.zalohovani.models;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;
using patrates.zalohovani.repository;
using System.ComponentModel;


namespace patrates.zalohovani.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {

        private ILocalSettings _localsetting;
        public List<string> ListOFCloudsSupported { get; set; }
        private List<ILocalCloudAccounts> _localCloudAccounts;
        private ISettingRepository _unitofWork;
        private NotifyIcon nIcon;
        private Boolean _canRunBackup = true;
        public Boolean canRunBackup
        {
            get { return this._canRunBackup; }
            set
            {
                if (this._canRunBackup == value) return;
                this._canRunBackup = value;
                RaisePropertyChanged("canRunBackup");
            }
        }
        IcloudRepository _cloudRepos;
        public SettingsViewModel(ILocalSettings localsettingService, ISettingRepository repositoryService)
        {

            OpenDialog = new RelayCommand(() => ExecuteOpenFileDialog());
            DeleteFile = new RelayCommand(() => DeleteSelectedFile());
            TakeBackupCommand = new RelayCommand(() => TakeBackup(), () => TakeBackupCanExecute());
            SaveSettingsCommand = new RelayCommand(() => SaveSettings(), () => SaveSettingsCanExecute());


            this._unitofWork = repositoryService;
            this._localsetting = this._unitofWork.getdata();
            this.ListOFCloudsSupported = new List<string>();
            initView();

            if (this._localsetting != null)
            {
                loaddata();

            }
            else
            {
                this._localsetting = localsettingService;
                this._localCloudAccounts = new List<ILocalCloudAccounts>();
                this.foldersToBackup = new ObservableCollection<string>();

            }

        }

        private void loaddata()
        {
            this._customerName = this._localsetting.customerName;
            this._customerKey = this._localsetting.customerKey;
            this._localCloudAccounts = this._localsetting.icloudSettings;
            foreach (var item in this._localCloudAccounts)
            {
                if (item.isactive)
                {
                    this._icloudService = (int)item.storageType;
                    this._cloudKey1 = item.cloudKey1;
                    this._cloudKey2 = item.cloudKey2;
                    this._cloudKey3 = item.cloudKey3;
                    switch ((cloudTypes)this._icloudService)
                    {
                        case cloudTypes.Amazon:
                            this._labelcloud1 = "Enter Amazon Public Key";
                            this._labelcloud2 = "Enter Amazon Private Key";
                            this._labelcloud3 = "Enter the Amazon Bucket";
                            this._cloud3Visible = true;
                            break;
                        case cloudTypes.Azure:
                            this._labelcloud1 = "Azure Public Key";
                            this._labelcloud2 = "Azure Secret Key";
                            this._cloud3Visible = false;
                            break;

                    }
                }

            }
            if (this.foldersToBackup == null)
            { this.foldersToBackup = new ObservableCollection<string>(this._localsetting.foldersToBackUp); }

            this._automate = this._localsetting.automate;
            this._frequeny = this._localsetting.frequeny;
            this._monWeek = this._localsetting.monWeek;
            this._tueWeek = this._localsetting.tueWeek;
            this._wedWeek = this._localsetting.wedWeek;
            this._thuWeek = this._localsetting.thuWeek;
            this._friWeek = this._localsetting.friWeek;
            this._satWeek = this._localsetting.satWeek;
            this._sunWeek = this._localsetting.sunWeek;
            this._timeRun = this._localsetting.timeofRun;


        }

        private void initView()
        {
            //foreach (cloudTypes cloud in Enum.GetValues(typeof(cloudTypes)))
            //{
            //    ListOFCloudsSupported.Add(cloud.ToString());
            //}

            ListOFCloudsSupported.Add(cloudTypes.Amazon.ToString());
            this._customerName = "My_Windows_8";


        }
        string _customerKey;
        public string customerKey
        {
            get
            {
                return _customerKey;
            }
            set
            {
                if (_customerKey == value)
                    return;
                _customerKey = value;
                RaisePropertyChanged("customerKey");
            }
        }

        string _customerName;
        public string customerName
        {
            get
            {
                return _customerName;
            }
            set
            {
                if (_customerName == value)
                    return;
                _customerName = value;
                RaisePropertyChanged("customerName");
            }


        }
        string _cloudKey1;
        public string cloudKey1
        {
            get
            {
                return _cloudKey1;
            }
            set
            {
                if (_cloudKey1 == value)
                    return;
                _cloudKey1 = value;
                RaisePropertyChanged("cloudKey1");
                SaveSettingsCommand.RaiseCanExecuteChanged();
                TakeBackupCommand.RaiseCanExecuteChanged();
                //_updateCloudKeyChanges();
            }


        }




        string _cloudKey2;
        public string cloudKey2
        {
            get
            {
                return _cloudKey2;
            }
            set
            {
                if (_cloudKey2 == value)
                    return;
                _cloudKey2 = value;
                RaisePropertyChanged("cloudKey2");
                SaveSettingsCommand.RaiseCanExecuteChanged();
                TakeBackupCommand.RaiseCanExecuteChanged();
                //_updateCloudKeyChanges();
            }


        }

        string _cloudKey3;
        public string cloudKey3
        {
            get
            {
                return _cloudKey3;
            }
            set
            {
                if (_cloudKey3 == value)
                    return;
                _cloudKey3 = value;
                RaisePropertyChanged("cloudKey3");
                SaveSettingsCommand.RaiseCanExecuteChanged();
                TakeBackupCommand.RaiseCanExecuteChanged();
                //_updateCloudKeyChanges();
            }


        }

        public ObservableCollection<string> foldersToBackup
        {
            get;
            set;
        }

        int _icloudService;
        public int icloudService
        {
            get
            {
                return _icloudService;
            }
            set
            {
                if (_icloudService == value)
                    return;
                _icloudService = value;
                RaisePropertyChanged("icloudService");
                _showExistingCloudkey_whenCloudSelectionChanges();
            }


        }

        Boolean _automate;
        public Boolean automate
        {
            get
            {
                return _automate;
            }
            set
            {
                if (_automate == value)
                    return;
                _automate = value;
                RaisePropertyChanged("automate");
            }


        }

        Boolean _monWeek;
        public Boolean monWeek
        {
            get
            {
                return _monWeek;
            }
            set
            {
                if (_monWeek == value)
                    return;
                _monWeek = value;
                RaisePropertyChanged("monWeek");
            }


        }

        Boolean _tueWeek;
        public Boolean tueWeek
        {
            get
            {
                return _tueWeek;
            }
            set
            {
                if (_tueWeek == value)
                    return;
                _tueWeek = value;
                RaisePropertyChanged("tueWeek");
            }
        }

        Boolean _wedWeek;
        public Boolean wedWeek
        {
            get
            {
                return _wedWeek;
            }
            set
            {
                if (_wedWeek == value)
                    return;
                _wedWeek = value;
                RaisePropertyChanged("wedWeek");
            }
        }

        Boolean _thuWeek;
        public Boolean thuWeek
        {
            get
            {
                return _thuWeek;
            }
            set
            {
                if (_thuWeek == value)
                    return;
                _thuWeek = value;
                RaisePropertyChanged("thuWeek");
            }
        }

        Boolean _friWeek;
        public Boolean friWeek
        {
            get
            {
                return _friWeek;
            }
            set
            {
                if (_friWeek == value)
                    return;
                _friWeek = value;
                RaisePropertyChanged("friWeek");
            }
        }

        Boolean _satWeek;
        public Boolean satWeek
        {
            get
            {
                return _satWeek;
            }
            set
            {
                if (_satWeek == value)
                    return;
                _satWeek = value;
                RaisePropertyChanged("satWeek");
            }
        }

        Boolean _sunWeek;
        public Boolean sunWeek
        {
            get
            {
                return _sunWeek;
            }
            set
            {
                if (_sunWeek == value)
                    return;
                _sunWeek = value;
                RaisePropertyChanged("sunWeek");
            }
        }

        DateTime _timeRun;
        public DateTime timeRun
        {
            get
            {
                return _timeRun;
            }
            set
            {
                if (_timeRun == value)
                    return;
                _timeRun = value;
                RaisePropertyChanged("timeRun");
            }
        }

        backupfrequency _frequeny;

        public backupfrequency frequeny
        {
            get
            {
                return _frequeny;
            }
            set
            {
                if (_frequeny == value)
                    return;
                _frequeny = value;
                RaisePropertyChanged("frequeny");
            }
        }


        string _labelcloud1;
        public string LabelCloud1
        {
            get
            {
                return _labelcloud1;
            }
            set
            {
                if (_labelcloud1 == value)
                    return;
                _labelcloud1 = value;
                RaisePropertyChanged("LabelCloud1");
            }

        }


        string _labelcloud2;
        public string LabelCloud2
        {
            get
            {
                return _labelcloud2;
            }
            set
            {
                if (_labelcloud2 == value)
                    return;
                _labelcloud2 = value;
                RaisePropertyChanged("LabelCloud2");
            }

        }

        string _labelcloud3;
        public string LabelCloud3
        {
            get
            {
                return _labelcloud3;
            }
            set
            {
                if (_labelcloud3 == value)
                    return;
                _labelcloud3 = value;
                RaisePropertyChanged("LabelCloud3");
            }

        }

        Boolean _cloud3Visible;
        public Boolean cloud3Visible
        {
            get
            {
                return _cloud3Visible;
            }
            set
            {
                if (_cloud3Visible == value)
                    return;
                _cloud3Visible = value;
                RaisePropertyChanged("cloud3Visible");
            }
        }

        int _folderSelected;
        public int folderSelected
        {
            get
            {
                return _folderSelected;
            }
            set
            {
                if (_folderSelected == value)
                    return;
                _folderSelected = value;
                RaisePropertyChanged("folderSelected");
            }


        }
        private string _selectedPath;
        public string SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                _selectedPath = value;
                RaisePropertyChanged("SelectedPath");
            }
        }

        string _message1;

        public string Message1
        {
            get
            {
                return _message1;
            }
            set
            {
                if (_message1 == value)
                    return;
                _message1 = value;
                RaisePropertyChanged("Message1");
            }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    RaisePropertyChanged("IsOpen");
                }
            }
        }

        public static RelayCommand OpenDialog { get; set; }
        public static RelayCommand DeleteFile { get; set; }
        public static RelayCommand SaveSettingsCommand { get; set; }
        public static RelayCommand TakeBackupCommand { get; set; }

        public void DeleteSelectedFile()
        {
            try
            {


                this.foldersToBackup.RemoveAt(_folderSelected);
                TakeBackupCommand.RaiseCanExecuteChanged();
            }
            catch (ArgumentOutOfRangeException)
            {


            }
        }

        private void ExecuteOpenFileDialog()
        {
            var dialog = new FolderBrowserDialog { };
            if (dialog.ShowDialog() == DialogResult.OK)
            {

                SelectedPath = dialog.SelectedPath;
                if (this.foldersToBackup.Where(x => x == SelectedPath).Count() == 0)
                {
                    this.foldersToBackup.Add(SelectedPath);
                }
                TakeBackupCommand.RaiseCanExecuteChanged();
            }

        }

        private void _showExistingCloudkey_whenCloudSelectionChanges()
        {
            Boolean storageExists = false;
            foreach (var item in this._localCloudAccounts)
            {
                if ((int)item.storageType == this._icloudService)
                {
                    this.cloudKey1 = item.cloudKey1;
                    this.cloudKey2 = item.cloudKey2;
                    this.cloudKey3 = item.cloudKey3;
                    item.isactive = true;
                    storageExists = true;

                }
                else { item.isactive = false; }

            }

            if (!storageExists)
            {
                this.cloudKey1 = "";
                this.cloudKey2 = "";
                this.cloudKey3 = "";
            }

            switch ((cloudTypes)this._icloudService)
            {
                case cloudTypes.Amazon:
                    this.LabelCloud1 = "Enter Amazon Public Key";
                    this.LabelCloud2 = "Enter Amazon Private Key";
                    this.LabelCloud3 = "Enter Your Bucket Name";
                    this.cloud3Visible = true;
                    break;
                case cloudTypes.Azure:
                    this.LabelCloud1 = "Azure Public Key";
                    this.LabelCloud2 = "Azure Secret Key";
                    this.cloud3Visible = false;
                    break;

            }

        }

        private void _updateCloudKeyChanges()
        {
            foreach (var item in this._localCloudAccounts)
            {
                if ((int)item.storageType == this._icloudService)
                {
                    item.cloudKey1 = this.cloudKey1;
                    item.cloudKey2 = this.cloudKey2;
                    item.cloudKey3 = this.cloudKey3;
                    item.isactive = true;
                    switch ((cloudTypes)this._icloudService)
                    {
                        case cloudTypes.Amazon:
                            this.LabelCloud1 = "Enter Amazon Public Key";
                            this.LabelCloud2 = "Enter Amazon Private Key";
                            this.LabelCloud3 = "Enter Your Bucket Name";
                            this.cloud3Visible = true;
                            break;
                        case cloudTypes.Azure:
                            this.LabelCloud1 = "Azure Public Key";
                            this.LabelCloud2 = "Azure Secret Key";
                            this.cloud3Visible = false;
                            break;

                    }
                }


            }

        }

        private Boolean SaveSettingsCanExecute()
        {
            if (string.IsNullOrWhiteSpace(this._cloudKey1) || string.IsNullOrWhiteSpace(this._cloudKey2))
            {
                return false;
            }

            return true;

        }

        private void SaveSettings()
        {
            this._localsetting.customerName = this._customerName;
            this._localsetting.customerKey = this._customerKey;
            Boolean foundCloud = false;
            foreach (var item in this._localCloudAccounts)
            {
                if ((int)item.storageType == this._icloudService)
                {
                    item.cloudKey1 = this.cloudKey1;
                    item.cloudKey2 = this.cloudKey2;
                    item.cloudKey3 = this.cloudKey3;
                    item.isactive = true;
                    foundCloud = true;
                }

            }

            if (!foundCloud && !string.IsNullOrWhiteSpace(this._cloudKey1) && !string.IsNullOrWhiteSpace(this._cloudKey2))
            {
                ILocalCloudAccounts newAccount = ServiceLocator.Current.GetInstance<ILocalCloudAccounts>();
                newAccount.cloudKey1 = this._cloudKey1;
                newAccount.cloudKey2 = this._cloudKey2;
                newAccount.cloudKey3 = null;
                newAccount.cloudKey4 = null;
                newAccount.isactive = true;
                newAccount.storageType = (cloudTypes)this._icloudService;
                this._localCloudAccounts.Add(newAccount);
            }


            this._localsetting.icloudSettings = this._localCloudAccounts;
            this._localsetting.foldersToBackUp = this.foldersToBackup.ToList();
            this._localsetting.automate = this._automate;
            this._localsetting.frequeny = this._frequeny;
            this._localsetting.monWeek = this._monWeek;
            this._localsetting.tueWeek = this._tueWeek;
            this._localsetting.wedWeek = this._wedWeek;
            this._localsetting.thuWeek = this._thuWeek;
            this._localsetting.friWeek = this._friWeek;
            this._localsetting.satWeek = this._satWeek;
            this._localsetting.sunWeek = this._sunWeek;
            this._localsetting.timeofRun = this._timeRun;
            this._unitofWork.saveData(this._localsetting);
            this._localsetting = this._unitofWork.getdata();
            loaddata();
            displayMessage("Settings Saved !!");

        }

        private Boolean TakeBackupCanExecute()
        {
            if (!canRunBackup)
            { return false; }
            switch ((cloudTypes)(_icloudService))
            {
                case cloudTypes.Amazon:
                    if (string.IsNullOrEmpty(cloudKey1) || string.IsNullOrEmpty(cloudKey2) || string.IsNullOrEmpty(cloudKey3))
                    { return false; }
                    break;
                case cloudTypes.Azure:
                    if (string.IsNullOrEmpty(cloudKey1) || string.IsNullOrEmpty(cloudKey2))
                    { return false; }
                    break;

            }

            if (this.foldersToBackup.Count == 0)
            { return false; }
            return true;

        }

        private void TakeBackup()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(BackupStart);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackupFinished);
            worker.RunWorkerAsync();

        }

        private void BackupStart(object sender, DoWorkEventArgs e)
        {
            if (!canRunBackup) return;
            canRunBackup = false;

            using (_cloudRepos = new cloudRepository(_localsetting))
            {
                _cloudRepos.uploadFilesAsync();
            }
        }

        private void BackupFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            canRunBackup = true;
            DisplayMessage("Back up completed !!");
        }

        private void displayMessage(string StrMessage)
        {

            this.Message1 = StrMessage;
            this.IsOpen = true;
            Task.Delay(2000).ContinueWith(_ =>
            {

                this.IsOpen = false;
            }
            );

        }

        private void DisplayMessage(string message)
        {

            if (nIcon != null)
            {
                nIcon.Dispose();

            }

            this.nIcon = new NotifyIcon();
            this.nIcon.Icon = Properties.Resources.logo1_256px;
            this.nIcon.Visible = true;
            this.nIcon.ShowBalloonTip(3000, "Zalohovani", message, ToolTipIcon.Info);

            Task.Delay(30000).ContinueWith(_ =>
            {

                this.nIcon.Dispose();
            });


        }
    }
}
