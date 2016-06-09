using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneBox_WebServices.ViewModels
{
    public class TreeViewFileViewModel
    {
        public DataTreeViewFileViewModel data { get; set; }
        public string text { get; set; }
        public bool children { get; set; }
    }

    public class DataTreeViewFileViewModel
    {
        public string pathOfTheFile { get; set; }
    }
}