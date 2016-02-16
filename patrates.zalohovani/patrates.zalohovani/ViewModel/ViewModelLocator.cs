/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:patrates.zalohovani.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using patrates.zalohovani.amazonS3;
using patrates.zalohovani.commons;
using patrates.zalohovani.interfaces;
using patrates.zalohovani.Model;
using patrates.zalohovani.repository;

namespace patrates.zalohovani.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }
            
            //read input file
            SimpleIoc.Default.Register<iCloud, sAmazonS3>();
            SimpleIoc.Default.Register<ILocalSettings, wpfLocalSettings>();
            SimpleIoc.Default.Register<ICryption,AESEncryption>();
            SimpleIoc.Default.Register<ISettingRepository, WPFSettingRepository>();
            SimpleIoc.Default.Register<ILocalCloudAccounts, LocalCloudAccounts>();
            SimpleIoc.Default.Register<IcloudRepository, cloudRepository>(); 
            //determine the cloud type
            //create a new service for the clould

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<RestoreViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}