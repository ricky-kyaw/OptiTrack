using OptiTrack.Data.DBMLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiTrack.Data.DBConnector
{
    public sealed class ViewModelsConnector
    {
        ViewModelsDataContext dataContext;

        public ViewModelsDataContext dataContextCaller
        {
            get
            {
                if (dataContext == null)
                {
                    dataContext = new ViewModelsDataContext(Properties.Resources.connectionString);
                }
                return dataContext;
            }
        }
    }
}