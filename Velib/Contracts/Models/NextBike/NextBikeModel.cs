using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.NextBike
{

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class markers
    {

        private markersCountry[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("country", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public markersCountry[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class markersCountry
    {

        private markersCountryCity[] cityField;
        private string nameField;
        private string countryField;

        private string country_nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("city", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public markersCountryCity[] city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }



        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string country_name
        {
            get
            {
                return this.country_nameField;
            }
            set
            {
                this.country_nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class markersCountryCity
    {

        private markersCountryCityPlace[] placeField;

        private int uidField;

        private double latField;

        private double lngField;

        private string breakField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("place", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public markersCountryCityPlace[] place
        {
            get
            {
                return this.placeField;
            }
            set
            {
                this.placeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int uid
        {
            get
            {
                return this.uidField;
            }
            set
            {
                this.uidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double lat
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double lng
        {
            get
            {
                return this.lngField;
            }
            set
            {
                this.lngField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @break
        {
            get
            {
                return this.breakField;
            }
            set
            {
                this.breakField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class markersCountryCityPlace
    {

        private int uidField;

        private double latField;

        private double lngField;

        private string nameField;

        private string numberField;

        private string bikesField;

        private string terminal_typeField;

        private string bike_numbersField;

        private string bike_racksField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int uid
        {
            get
            {
                return this.uidField;
            }
            set
            {
                this.uidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double lat
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double lng
        {
            get
            {
                return this.lngField;
            }
            set
            {
                this.lngField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string bikes
        {
            get
            {
                return this.bikesField;
            }
            set
            {
                this.bikesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string terminal_type
        {
            get
            {
                return this.terminal_typeField;
            }
            set
            {
                this.terminal_typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string bike_numbers
        {
            get
            {
                return this.bike_numbersField;
            }
            set
            {
                this.bike_numbersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string bike_racks
        {
            get
            {
                return this.bike_racksField;
            }
            set
            {
                this.bike_racksField = value;
            }
        }
    }
}
