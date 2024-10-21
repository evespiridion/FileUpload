using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Domain.Entities
{
    public class FileDetails
    {
        public int Id { get; set; }
        public byte[] FileContent { get; set; } // Storing file content as byte array
        public string FileName { get; set; } // Store the file name for download purposes
        public DateTime StartDate { get; set; } // Accessible start date
        public DateTime EndDate { get; set; } // Accessible end date
    }
}
