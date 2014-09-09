using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.Smoove
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class vcs
    {

        [System.Xml.Serialization.XmlElementAttribute("sl", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public strassStation Node
        {
            get;
            set;
        }

    }
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class strassStation
    {
        [System.Xml.Serialization.XmlElementAttribute("si", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public station[] Stations
        {
            get;
            set;
        }


    }
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class station
    {

        private int idField;

        private double latField;

        private double longField;

        private int nbBikesField;

        private int nbEmptyDocksField;


        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("id", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlAttributeAttribute("la", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlAttributeAttribute("lg", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlAttributeAttribute("av", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlAttributeAttribute("fr", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
