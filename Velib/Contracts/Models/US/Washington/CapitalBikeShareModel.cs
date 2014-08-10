using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.US.Washington
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class stations
    {

        private stationsStation[] stationField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("station", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public stationsStation[] Stations
        {
            get
            {
                return this.stationField;
            }
            set
            {
                this.stationField = value;
            }
        }

    }
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class stationsStation
    {

        private int idField;

        private string nameField;

        private string terminalNameField;

        private string lastCommWithServerField;

        private double latField;

        private double longField;

        private string installedField;

        private string lockedField;

        private string installDateField;

        private string removalDateField;

        private string temporaryField;

        private string publicField;

        private int nbBikesField;

        private int nbEmptyDocksField;

        private string latestUpdateTimeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("id",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("name",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }



        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("lat",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double Latitude
        {
            get
            {
                return this.latField;
            }
            set
            {
                this.latField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("long",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double Longitude
        {
            get
            {
                return this.longField;
            }
            set
            {
                this.longField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string locked
        {
            get
            {
                return this.lockedField;
            }
            set
            {
                this.lockedField = value;
            }
        }
        
        
        
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("nbBikes", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int AvailableBikes
        {
            get
            {
                return this.nbBikesField;
            }
            set
            {
                this.nbBikesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("nbEmptyDocks",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int AvailableDocks
        {
            get
            {
                return this.nbEmptyDocksField;
            }
            set
            {
                this.nbEmptyDocksField = value;
            }
        }

    }

  
}
