using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data
{
    [DataContract]
    public class OperationResult
    {
        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public bool IsDataBaseError { get; set; }

        public OperationResult(bool isSuccess, string message, bool isDataBaseError)
        {
            IsSuccess = isSuccess;
            Message = message;
            IsDataBaseError = isDataBaseError;
        }

        public OperationResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
            IsDataBaseError = false;
        }
    }

}
