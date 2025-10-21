using System;

namespace FSR.DigitalTwin.Client.Common.Utils.Semantic
{
    public class UriPrefix
    {
        public static readonly UriPrefix SO = new("http://schema.org/");
        public static readonly UriPrefix EX = new("http://www.example.org/");
        public static readonly UriPrefix RDF = new("http://www.w3.org/1999/02/22-rdf-syntax-ns#");
        public static readonly UriPrefix RDFS = new("http://www.w3.org/2000/01/rdf-schema#");
        public static readonly UriPrefix DUL = new("http://www.loa-cnr.it/ontologies/DUL.owl#");
        public static readonly UriPrefix SSN = new("http://purl.oclc.org/NET/ssnx/ssn#");
        public static readonly UriPrefix SOHO = new("http://pst.istc.cnr.it/ontologies/2019/01/soho#");
        public static readonly UriPrefix SOBOTS = new("https://www.uni-augsburg.de/de/fakultaet/fai/informatik/prof/pi/projekte/forschung/forsocialrobots/sobots#");
        public static readonly UriPrefix XSD = new("http://www.w3.org/2001/XMLSchema#");
        public static readonly UriPrefix PI = new("https://www.uni-augsburg.de/de/fakultaet/fai/informatik/prof/pi#");

        private readonly Uri _prefix;
        public Uri Prefix => _prefix;

        public UriPrefix(Uri prefix)
        {
            _prefix = prefix;
        }

        public UriPrefix(string prefix)
        {
            _prefix = new Uri(prefix);
        }

        public static Uri operator +(UriPrefix prefix, Uri postfix) => new("" + prefix.Prefix + postfix);
        public static Uri operator +(UriPrefix prefix, string postfix) => new(prefix.Prefix + postfix);
    }

}