using OptiTrack.Data.DBMLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiTrack.Data.DBConnector
{
    public class TableModelsConnector
    {
        TableModelsDataContext dataContext;

        public TableModelsDataContext dataContextCaller
        {
            get
            {
                if (dataContext == null)
                {
                    dataContext = new TableModelsDataContext(Properties.Resources.connectionString);
                }
                return dataContext;
            }
        }
    }
}

