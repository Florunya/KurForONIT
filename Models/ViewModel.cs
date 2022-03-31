using System;
using System.Collections.Generic;

namespace Kur.Models
{
    public class ViewModel
    {
        public List<Food> Foods { get; set; } = null;
        public List<EndPoint> EndPoints { get; set; } = null;
        public List<PortInfo> PortInfos { get; set; } = null;
    }
}
