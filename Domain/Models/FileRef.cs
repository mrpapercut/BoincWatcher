using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoincWatcher.Domain.Models; 
public class FileRef {
    public string FileName { get; set; }
    public bool MainProgram { get; set; }
    public string OpenName { get; set; }
    public bool CopyFile { get; set; }
}
