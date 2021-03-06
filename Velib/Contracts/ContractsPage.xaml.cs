﻿using Velib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Velib.Contracts;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Diagnostics;

// Pour en savoir plus sur le modèle d'élément Page de base, consultez la page http://go.microsoft.com/fwlink/?LinkID=390556

namespace Velib
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ContractsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private CollectionViewSource contractCollectionViewSource;
        private List<ContractGroup> contractGroup;
        private int cityCounter = 0;
        private Task loadedTask = new Task(() => { });
        public ContractsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.DefaultViewModel["CityCounter"] = cityCounter;
            this.Loaded += ContractsPage_Loaded;
            initialize();
           
        }

        private async void initialize()
        {
            var t = Task.Run(() =>
           {
               contractGroup = new List<ContractGroup>();
               foreach (var contract in ContractsViewModel.Contracts.GroupBy(c => c.Pays).Select(c => c.First()).OrderBy(c => c.Pays).ToList())
               {
                   var group = new ContractGroup() { Title = contract.Pays, ImagePath = contract.PaysImage };
                   group.Items = new ObservableCollection<Contract>();
                   foreach (var c in ContractsViewModel.Contracts.Where(c => c.Pays == contract.Pays).OrderBy(c => c.Name).ToList())
                   {
                       cityCounter++;
                       group.Items.Add(c);
                       group.ItemsCounter++;
                   }
                   contractGroup.Add(group);
               }
           });

            await Task.WhenAll(t, loadedTask);
            contractCollectionViewSource = new CollectionViewSource
            {
                IsSourceGrouped = true,
                Source = contractGroup,
                ItemsPath = new PropertyPath("Items")
            };
            this.DefaultViewModel["Group"] = contractCollectionViewSource.View;
            this.DefaultViewModel["CityCounter"] = cityCounter;

        }

        void ContractsPage_Loaded(object sender, RoutedEventArgs e)
        {
            loadedTask.Start();
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Obtient le <see cref="NavigationHelper"/> associé à ce <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Obtient le modèle d'affichage pour ce <see cref="Page"/>.
        /// Cela peut être remplacé par un modèle d'affichage fortement typé.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Remplit la page à l'aide du contenu passé lors de la navigation. Tout état enregistré est également
        /// fourni lorsqu'une page est recréée à partir d'une session antérieure.
        /// </summary>
        /// <param name="sender">
        /// La source de l'événement ; en général <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Données d'événement qui fournissent le paramètre de navigation transmis à
        /// <see cref="Frame.Navigate(Type, Object)"/> lors de la requête initiale de cette page et
        /// un dictionnaire d'état conservé par cette page durant une session
        /// antérieure.  L'état n'aura pas la valeur Null lors de la première visite de la page.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Conserve l'état associé à cette page en cas de suspension de l'application ou de
        /// suppression de la page du cache de navigation.  Les valeurs doivent être conformes aux
        /// exigences en matière de sérialisation de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">La source de l'événement ; en général <see cref="NavigationHelper"/></param>
        /// <param name="e">Données d'événement qui fournissent un dictionnaire vide à remplir à l'aide de l'
        /// état sérialisable.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        
        #region Inscription de NavigationHelper

        /// <summary>
        /// Les méthodes fournies dans cette section sont utilisées simplement pour permettre
        /// NavigationHelper pour répondre aux méthodes de navigation de la page.
        /// <para>
        /// La logique spécifique à la page doit être placée dans les gestionnaires d'événements pour  
        /// <see cref="NavigationHelper.LoadState"/>
        /// et <see cref="NavigationHelper.SaveState"/>.
        /// Le paramètre de navigation est disponible dans la méthode LoadState 
        /// en plus de l'état de page conservé durant une session antérieure.
        /// </para>
        /// </summary>
        /// <param name="e">Fournit des données pour les méthodes de navigation et
        /// les gestionnaires d'événements qui ne peuvent pas annuler la requête de navigation.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            
          
            

        }
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //VisualStateManager.GoToState(e.OriginalSource as Control, "Disabled", true);
            var contract = e.ClickedItem as Contract;
            if (contract != null)
            {
                if (contract.Downloaded)
                {
                    ContractsViewModel.RemoveContract(contract);
                }
                else
                    ContractsViewModel.DownloadAndSaveContract(contract);
            }
        }
        

        #endregion
    }
}
