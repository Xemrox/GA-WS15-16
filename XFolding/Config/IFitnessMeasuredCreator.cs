using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XGA.Config {

    internal interface IFitnessMeasuredCreator<T> {

        IFitnessMeasured<T> CreateNew();
    }
}