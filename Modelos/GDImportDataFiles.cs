//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleCargaDatosDNK.Modelos
{
    using System;
    using System.Collections.Generic;
    
    public partial class GDImportDataFiles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GDImportDataFiles()
        {
            this.GDImportDataFileLog = new HashSet<GDImportDataFileLog>();
        }
    
        public int ID { get; set; }
        public string RemoteFilepath { get; set; }
        public string LocalFilename { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime RemoteLastWriteTime { get; set; }
        public short Status { get; set; }
        public System.DateTime StatusDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GDImportDataFileLog> GDImportDataFileLog { get; set; }
    }
}
