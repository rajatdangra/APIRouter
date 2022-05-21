using Router.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Models
{
    public class ResponseEntity
    {
        public StatusEnum Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public ResponseEntity()
        {

        }
        public ResponseEntity(StatusEnum status, string msg, object data = null)
        {
            this.Status = status;
            this.Message = msg;
            this.Data = data;
        }
    }
}
