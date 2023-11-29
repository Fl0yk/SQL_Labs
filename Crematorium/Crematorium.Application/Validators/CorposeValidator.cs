using Crematorium.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.Application.Validators
{
    public class CorposeValidator : AbstractValidator<Corpose>
    {
        public CorposeValidator()
        {
            RuleFor(u => u.Name).NotNull().NotEmpty().Length(1, 20).WithMessage("Некорректное имя");
            RuleFor(u => u.SurName).NotNull().NotEmpty().Length(1, 20).WithMessage("Некорректная фамилия");
            RuleFor(u => u.NumPassport).NotNull().Matches(@"\d{7}\w\d{3}(PB|BI|BA)\d").WithMessage("Некорректный номер паспорта");
        }
    }
}
