using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_Consumer
{
    public interface IRunnableService
    {
        void Start();

        void Stop();
    }
}