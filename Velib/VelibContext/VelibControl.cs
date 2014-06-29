using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Velib.VelibContext
{
    public class VelibControl : ContentControl
    {
        private bool alreadyLoaded;
        public bool Selected { get; set; }

        public VelibControl()
        {
            DefaultStyleKey = typeof(VelibControl);
            this.Loaded += EventControl_Loaded;
            
        }

        void EventControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (alreadyLoaded)
                return;
            VisualStateManager.GoToState(this, "Loaded", false);
            alreadyLoaded = true;

        }

        protected override void OnApplyTemplate()
        {
            VisualStateManager.GoToState(this, "BeforeLoaded", false);
            base.OnApplyTemplate();
          
        }

    }
}
