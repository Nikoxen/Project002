using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace p002.Models
{
    public class BaseResponse<T>
    {
        public T Value { get; set; }
        public List<string> ErrorList { get; set; }
        public bool Status => ErrorList != null && ErrorList.Count == 0;

        public BaseResponse()
        {
            this.ErrorList = new List<string>();
        }
    }
}