using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Attribute
{
    public class UpdateAttributeViewModel
    {
        public string? AttributeSetId { get; set; }
        public List<UpdateAttributeAPIViewModel>? Updates { get; set; } = new List<UpdateAttributeAPIViewModel>() { new UpdateAttributeAPIViewModel() };
    }
}
