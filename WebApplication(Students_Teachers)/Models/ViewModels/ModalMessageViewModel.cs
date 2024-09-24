using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication_Students_Teachers_.Models.ViewModels
{
    public class ModalMessageViewModel
    {
        public bool IsError { get; set; }
        public bool Show { get; set; }
        public string Header { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Footer { get; set; } = string.Empty;
    }
}