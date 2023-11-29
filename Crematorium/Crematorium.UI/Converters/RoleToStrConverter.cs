using Crematorium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Crematorium.UI.Converters
{
    internal class RoleToStrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            value = (Role)value;
            return value switch
            {
                Role.Admin => "Admin",
                Role.Employee => "Employee",
                Role.Customer => "Customer",
                Role.NoName => "No role",
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
