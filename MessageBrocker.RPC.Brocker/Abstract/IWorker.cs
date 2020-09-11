using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageBrocker.RPC.Brocker.Abstract
{
    public interface IWorker
    {
        public Task Run();
    }
}
