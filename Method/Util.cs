using System.Net.Mail;
using System.Text.RegularExpressions;

namespace rentalcar_backend.Method
{
    public static class Util
    {
        public static object NullSafe(object data ,object defaultvalue)
        {
            return data == DBNull.Value? defaultvalue : data;   
        }

        public static bool isValidEmail(string emailaddress)
        {
            if(String.IsNullOrEmpty(emailaddress))
            {
                return false;
            }

            try
            {
                MailAddress mail = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool isValidAlphanumeric(string Text)
        {
            if (String.IsNullOrEmpty(Text))
            {
                return false;
            }

            try
            {
                Regex rex = new Regex(" ^[a - zA - Z0 - 9] * $");

                if(rex.IsMatch(Text))
                {
                    return false;

                }else
                {
                    return true;
                }
                
            }
            catch (FormatException)
            {
                return false;
            }


        }

      
    }
}
