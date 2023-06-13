using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JupebPortal.Models
{
    public class Programme
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
}
