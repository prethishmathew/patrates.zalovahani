using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using patrates.zalohovani.interfaces;
using patrates.zalohovani.Model;
using patrates.zalohovani.repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReactiveUI;
using System.Reactive.Linq;

namespace patrates.zalohovani.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class RestoreViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MvvmViewModel1 class.
        /// </summary>
        

        List<IcloudFileInfo> CloudFilesList;
        NotifyIcon nIcon;
        public ObservableCollection<CloudItems> treeViewItems { get; set; }

        public ReactiveCommand<object> Search { get; set; }
        public static RelayCommand RestoreFiles { get; set; }
        public static RelayCommand OpenDialog { get; set; }
        public static RelayCommand RefreshData { get; set; }
        public        
        IcloudRepository repository;
        ISettingRepository unitofwork;
        ILocalSettings localsettings;

        string _filterPattern;
        public string FilterPattern
        {
            get { 
                return _filterPattern; 
            }
            set
            {
                if (_filterPattern == value)
                {
                    return;
                }
                _filterPattern = value;                
                RaisePropertyChanged("FilterPattern");
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


        public void ExecuteRestoreFiles()
        {
            RestoreFile();
        
        }
        public void ExecuteOpenFileDialog()
        {
            var dialog = new FolderBrowserDialog { };
            if (dialog.ShowDialog() == DialogResult.OK)
            {

                SelectedPath = dialog.SelectedPath;
               
            }
            CreateTreeView();
        }
        
        public RestoreViewModel(ILocalSettings tr,IcloudRepository repository)
        {
            var canSearch = this.WhenAny(x => x.FilterPattern, x => !String.IsNullOrWhiteSpace(x.Value));
            Search =  ReactiveCommand.Create(canSearch,System.Reactive.Concurrency.DispatcherScheduler.Current);
            Search.CanExecute(canSearch);
            Search.Subscribe(x => CreateTreeView(this.FilterPattern));
            this.WhenAnyValue(x => x.FilterPattern)
             .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler);
            


            RestoreFiles = new RelayCommand(() => ExecuteRestoreFiles());
            OpenDialog = new RelayCommand(() => ExecuteOpenFileDialog());
            RefreshData = new RelayCommand(() => RefreshCurrentData());
            this.repository = repository;
            this.localsettings = tr;

            
            CreateTreeView();
           

        }



        private  void CreateTreeView(string searchStr=null)
        {
            if (this.unitofwork == null)
            {
                CreateRepository();
            }
            using (repository)
            { CloudFilesList = repository.getFilesList(); 
            }

            var listtoWorkwith = (from x in CloudFilesList
                                  where (x.CloudFileName.ToLower().Contains(searchStr.ToLower()) || x.localfolderName.ToLower().Contains(searchStr.ToLower()))
                                  select x
                                      );

            var services = (from x in CloudFilesList
                                select new { service = x.storageType }
                                   ).Distinct();
            
            if (searchStr != null)
            {
                services = (from x in CloudFilesList
                            select new { service = x.storageType }
                                   );
            
            }

            if (this.treeViewItems !=null)
            {
                this.treeViewItems.Clear();                
            }
            else
            { this.treeViewItems = new ObservableCollection<CloudItems>(); }
            
            
            foreach (var x in services)
            {
                CloudItems item = new CloudItems();
                item.cloudtype = x.service;
                item.devices = new List<Devices>();
                var devices = (from y in CloudFilesList
                               where y.storageType == x.service
                               select new { device = y.deviceName }
                                ).Distinct();
                foreach (var d in devices)
                {
                    Devices deviceItem = new Devices();
                    deviceItem.Name = d.device;
                    deviceItem.cloudfiles = new List<CloudWPFItem>();
                    var files = (from fi in CloudFilesList
                                 where fi.deviceName == d.device && fi.storageType == x.service
                                 select fi);

                    //Add Search Criteria to Linq

                    if (searchStr != null)
                    {
                        files = from fi in files
                                where (fi.CloudFileName.ToLower().Contains(searchStr.ToLower()) || fi.localfolderName.ToLower().Contains(searchStr.ToLower()))
                                select fi;
                    }
                    foreach (var file in files)
                    {
                        deviceItem.cloudfiles.Add(new CloudWPFItem { cloudItems = file, isSelected = false });


                    }

                    item.devices.Add(deviceItem);

                }

                this.treeViewItems.Add(item);
            }   

        }

        private void CreateRepository()
        {

            this.unitofwork = ServiceLocator.Current.GetInstance<ISettingRepository>();
            this.localsettings = unitofwork.getdata();
            this.repository = new cloudRepository(this.localsettings);

        }
        public async void RestoreFile()
        {

            var mo = treeViewItems.SelectMany(x => x.devices)
                    .SelectMany(d => d.cloudfiles)
                    .Where(de => de.isSelected == true)
                    .Select(c => c.cloudItems);
            if (this.unitofwork == null)
            {
                CreateRepository();
            }
            
            if (string.IsNullOrEmpty(SelectedPath) || !Directory.Exists(SelectedPath))
            {
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);            
            }
            
            using(repository)
            { 
                await Task.Run(() => repository.downloadFilesAsync(mo, SelectedPath));
            }
            //List<Devices> result = treeViewItems.ConvertAll(e => e);
            /* Different ways to doing the same query
             * var q = (from items in treeViewItems
                     from device in items.devices
                     from cloud in device.cloudfiles.Where(x => x.isSelected == true)
                     select cloud.cloudItems);
            */

            /* Another Way of Doing the Same Query
             * 
             foreach (var items in treeViewItems)
            {
                foreach (var device in items.devices)
                { 
                    foreach (var cloud in device.cloudfiles)
                    {
                        if (cloud.isSelected)
                        {
                            st.Add(cloud.cloudItems.CloudFileName);
                        }

                    }
                
                }
            
            }
             
             */

            
            
            //var list3 = from items in list2
                        

        }

        public void RefreshCurrentData()
        {

            CreateTreeView();
            notification("Data Refreshed");
        }

        private void notification(string message)
        {

            if (nIcon != null)
            {
                nIcon.Dispose();
                
            }

            this.nIcon = new NotifyIcon();
            this.nIcon.Icon = Properties.Resources.logo1_256px;
            this.nIcon.Visible = true;
            this.nIcon.ShowBalloonTip(3000,"Zalohovani",message,ToolTipIcon.Info);
            
            Task.Delay(30000).ContinueWith(_ =>
            {

                this.nIcon.Dispose();
            });
        }

    }
}