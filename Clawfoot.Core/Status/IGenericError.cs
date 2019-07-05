using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Core.Status
{
    public interface IGenericError
    {
        string ToString();
        string ToUserString();
    }
}
