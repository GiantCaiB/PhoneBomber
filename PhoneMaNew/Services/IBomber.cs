using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Twilio.Types;

namespace PhoneMaNew.Services
{
    public interface IBomber
    {
        Task Bomb(PhoneNumber target);
    }
}
