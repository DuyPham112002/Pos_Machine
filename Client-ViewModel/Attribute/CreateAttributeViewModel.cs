using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Attribute
{
    public class CreateAttributeViewModel
    {
        public string? AttributeSetId { get; set; }
        public List<CreateAttributeAPIViewModel>? Attributes { get; set; } = new List<CreateAttributeAPIViewModel>() { new CreateAttributeAPIViewModel() };
    }
}
