//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StrategoDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Report
    {
        public int IdReport { get; set; }
        public string Reason { get; set; }
        public System.DateTime Date { get; set; }
        public int IdReporter { get; set; }
        public int IdReported { get; set; }
    
        public virtual Player Player { get; set; }
        public virtual Player Player1 { get; set; }
    }
}
