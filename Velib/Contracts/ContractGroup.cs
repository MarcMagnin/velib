using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts
{
    public class ContractGroup
    {
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int ItemsCounter { get; set; }
        public ObservableCollection<Contract> Items { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
