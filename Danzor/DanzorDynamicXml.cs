﻿using Danzor.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace Danzor
{
    public class DanzorDynamicXml : DynamicObject
    {
        private XElement Element { get; set; }
        public string this[string attr]
        {
            get
            {
                return Element.IsNull() ? string.Empty : Element.Attribute(attr).Value;
            }
        }
        public string Value
        {
            get
            {
                return Element.IsNull() ? string.Empty : Element.Value;
            }
        }


        public DanzorDynamicXml(string filename)
        {
            this.Element = XElement.Load(filename);
        }

        private DanzorDynamicXml(XElement el)
        {
            this.Element = el;
        }


        public double ToDouble()
        {
            return this.Element.ToDouble();
        }

        public DateTime? ToDateTime()
        {
            return this.Element.ToDateTime();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var nodes = GetElements(binder.Name);

            if (!nodes.IsEmpty() && nodes.Count() == 1 && binder.Name != "det")
            {
                result = new DanzorDynamicXml(nodes.First());
                return true;
            }
            else if (!nodes.IsEmpty() && nodes.Count() > 1 || binder.Name == "det")
            {
                result = nodes.Select(n => new DanzorDynamicXml(n)).ToList();
                return true;
            }
            else
            {
                result = null;
                return true;
            }
        }

        private IEnumerable<XElement> GetElements(string name)
        {
            if (this.Element.IsNull()) return null;

            XNamespace xnameSpace = "http://www.portalfiscal.inf.br/nfe";
            return this.Element.Elements(xnameSpace + name);
        }
    }
}