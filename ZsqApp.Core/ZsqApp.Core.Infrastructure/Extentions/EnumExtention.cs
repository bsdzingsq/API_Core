using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ZsqApp.Core.Infrastructure.Extentions
{
    public static class EnumExtention
    {
        public static string GetDescription(this Enum @this)
        {
            var type = @this.GetType();
            var name = Enum.GetName(type, @this);
            if (name == null)
            {
                return null;
            }
            var field = type.GetField(name);

            return !(Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) is DescriptionAttribute attribute) ? name : attribute.Description;
        }
    }
}
