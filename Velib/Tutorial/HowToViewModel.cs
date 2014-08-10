using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Tutorial
{
    public class HowToViewModel
    {
        public ObservableCollection<HowToModel> Items = new ObservableCollection<HowToModel>();

        public HowToViewModel()
        {
            Items.Add(new HowToModel()
            {
                Title = "Download a city",
                Text ="Tap on \"download cities\" on the menu panel. It will show the available cities list",
                ImageUrl = "downloadcities.png",
            });
            Items.Add(new HowToModel()
            {
                Title = "",
                Text = "Then tap on a city to download or remove it.",
                Details = "It will download the stations list from the selected service." +
                "If there is some new stations provided by the public bike service, you can refresh that list by removing and downloading again the city.",
                ImageUrl = "downloadcities2.png",
            });
            Items.Add(new HowToModel()
            {
                Title = "Switch between bike and station mode",
                Text = "Tap on the bike icon or (P) icon on the bottom right corner.",
                ImageUrl = "cyclemode.png",
            });
            Items.Add(new HowToModel()
            {
                Title = "Add some favorites",
                Text = "Tap on a station or a searched location, it will show up a path walk to get there." +
                            "Then tap again to show up a command panel. Press \"Add to favorites\".",
                Details = "If the tapped point is far away from your location, it will directly shows up " +
                            "the command panel as the walk path is not revelant.",
                ImageUrl = "addToFavorites.png",
            });
            Items.Add(new HowToModel()
            {
                Title = "Get the address from a location",
                Text = "Hold a finger on the map. It will shows up the found address and the walk path to get there if it exist.",
                Details = "",
                ImageUrl = "mapHolding.png",
            });
            Items.Add(new HowToModel()
            {
                Title = "Search for an address",
                Text = "Tap the search icon button in the command bar and type an address in the search text box.",
                Details = "",
                ImageUrl = "addressSearch.png",
            });
            


            Items.Add(new HowToModel()
            {
                Title = "Tips : Store your home/car location",
                Text = "Like a \"Dude, where is my car\" app, if you want to easely get back to your home / car / whereever location, just hold a finger on the"+
                " location and add it to your favorites.",
                Details = "It will be really easy to get back there from the favorites list.",
                ImageUrl = "mapHolding.png",
            });
        }
    }
}
