using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FaceDetection.Models
{
    public class Age
    {
        public string ageRange { get; set; }
        public string score { get; set; }
    }

    public class Gender
    {
        public string gender { get; set; }
        public string score { get; set; }
    }

    public class Disambiguated
    {
        public string dbpedia { get; set; }
        public string freebase { get; set; }
        public string name { get; set; }
        public List<string> subType { get; set; }
        public string website { get; set; }
        public string yago { get; set; }
    }

    public class KnowledgeGraph
    {
        public string typeHierarchy { get; set; }
    }

    public class Identity
    {
        public Disambiguated disambiguated { get; set; }
        public KnowledgeGraph knowledgeGraph { get; set; }
        public string name { get; set; }
        public string score { get; set; }
    }

    public class ImageFace
    {
        public Age age { get; set; }
        public Gender gender { get; set; }
        public string height { get; set; }
        public Identity identity { get; set; }
        public string positionX { get; set; }
        public string positionY { get; set; }
        public string width { get; set; }
    }

    public class FaceDetectionResult
    {
        public string status { get; set; }
        public string usage { get; set; }
        public string totalTransactions { get; set; }
        public List<ImageFace> imageFaces { get; set; }
    }
}