using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.CloudIAP.v1.ProjectsResource;

namespace SharedLib
{
    public class IAP : IIAP
    {
        public IAP() { }
        public void IAPExecute(string name) 
        {
            
        }
    }

    public interface IIAP
    {
        public void IAPExecute(string name);
    }
}
