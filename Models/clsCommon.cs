namespace Transport_Management.Models
{
    public class clsCommon
    {
        public static int iSuperAdminId { get; set; } = 1;
        public static int iAdminUserId { get; set; } = 2;
        public static int iDriverUserId { get; set; } = 3;
        public static int iGuestUserId { get; set; } = 4;

        public const string sSuperAdmin = "SuperAdmin";
        public const string sAdmin = "Admin";
        public const string sDriver = "Driver";
        public const string sGuest = "Guest";

        public static string toConvertStringInTestServer(string sFilePath)
        {
            //return sFilePath.ToString();
            return sFilePath.Replace("\\", "/").ToString();
        }
        public static string getUserType(int? userTypeId)
        {
            if (userTypeId == iSuperAdminId)
            {
                return sSuperAdmin;
            }
            else if (userTypeId == iAdminUserId)
            {
                return sAdmin;
            }
            else if (userTypeId == iDriverUserId)
            {
                return sDriver;
            }
            else
            {
                return sGuest;
            }
        }
        public class clsResponse
        {
            public static string sDatafetchedSuccess { get; set; } = "Data Fetched Successfully";
            public static string sDataSaveResult { get; set; } = "Data Saved Successfully";
            public static string sDataSavedFailure { get; set; } = "Data Saveing process  Failed";

        }

        public class CurrencyConversion
        {
            public static string NumberToWords(int number)
            {
                if (number == 0)
                    return "Zero";

                if (number < 0)
                    return "minus " + NumberToWords(Math.Abs(number));

                string words = "";

                if ((number / 1000000) > 0)
                {
                    words += NumberToWords(number / 1000000) + " Million ";
                    number %= 1000000;
                }

                if ((number / 1000) > 0)
                {
                    words += NumberToWords(number / 1000) + " Thousand ";
                    number %= 1000;
                }

                if ((number / 100) > 0)
                {
                    words += NumberToWords(number / 100) + " Hundred ";
                    number %= 100;
                }

                if (number > 0)
                {
                    if (words != "")
                        words += "And ";

                    var unitsMap = new[]
                    {
                    "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
                    "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
                    };

                    var tensMap = new[]
                    {
                    "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
                    };


                    if (number < 20)
                        words += unitsMap[number];
                    else
                    {
                        words += tensMap[number / 10];
                        if ((number % 10) > 0)
                            words += "-" + unitsMap[number % 10];
                    }
                }

                return words;
            }


             
        }

    }
}
