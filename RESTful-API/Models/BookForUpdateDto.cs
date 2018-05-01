using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI.Models
{
    public class BookForUpdateDto : BookForManipulationDto {
        [Required(ErrorMessage = "Description should be filled out.")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
