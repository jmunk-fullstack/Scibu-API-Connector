using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScibuAPIConnector.Models
{
    interface IPostData
    {

    }

    public class PostData<T> : IPostData
    {
        public string Key { get; set; }
        public T Value { get; set; }
    }
}
