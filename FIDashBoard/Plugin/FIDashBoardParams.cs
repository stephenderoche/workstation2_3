using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linedata.Framework.WidgetFrame.PluginBase;
using Linedata.Framework.WidgetFrame.MvvmFoundation;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Diagnostics;

namespace FIDashBoard.Client.Plugin
{
    public class FIDashBoardParameters : ObservableObject, IWidgetParameters
    {
       
        private string _accountName;
        private string _xml;
        private string _defaultTheme;

        public FIDashBoardParameters()
        {
            this._defaultTheme = "LightGray";
            this._accountName = string.Empty;
            this._xml = "FIDashboard.xaml";
        }

        public string DefaultTheme
        {
            get
            {
                return this._defaultTheme;
            }

            set
            {
                this._defaultTheme = value;
                this.RaisePropertyChanged("DefaultTheme");

            }
        }
        public string XML
        {
            get
            {
                return this._xml;
            }

            set
            {
                this._xml = value;
                this.RaisePropertyChanged("XML");

            }
        }


        public string AccountName
        {
            get
            {
                return this._accountName;
            }

            set
            {
                this._accountName = value;
                this.RaisePropertyChanged("AccountName");

            }
        }

       
        public XElement GetParams()
        {

            if(string.IsNullOrEmpty(XML))
            {XML = "";}
           XElement parameters = new XElement(
                                   "YieldCurveParameters",
                                   new XAttribute("accountName", this._accountName),
                                   new XAttribute("xml", XML),
                                   new XAttribute("defualtTheme", this._defaultTheme)
                                   );

            

            return parameters;
        }

        public void SetParams(XElement param)
        {
            if (null != param)
            {
               
                XAttribute AccountNameAttribute = param.Attribute("accountName");
                XAttribute XMLAttribute = param.Attribute("xml");
                XAttribute DefualtThemeAttribute = param.Attribute("defualtTheme");
                try
                {

                    if (DefualtThemeAttribute != null)
                    {
                        this._defaultTheme = (string)DefualtThemeAttribute;

                    }


                    if (XML != null)
                    {
                        this._xml = (string)XMLAttribute;

                    }

                   
                 
                  

                    if (AccountNameAttribute != null)
                    {
                        this._accountName = (string)AccountNameAttribute;

                    }




                }
                catch (FormatException ex)
                {
                    Debug.WriteLine(string.Format("One of the parameter have wrong format : {0} ", ex.Message));
                }
            }
        }

        public void UpgradeParams(XElement param)
        {
        }

        public XmlSchemaSet GetParamsSchemaSet()
        {
            return null;
        }
    }
}
