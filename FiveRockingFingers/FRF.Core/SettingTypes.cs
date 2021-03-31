namespace FRF.Core
{
    public static class SettingTypes
    {
        public const string Decimal = "decimal";
        public const string NaturalNumber = "naturalNumber";

        public static bool IsDecimal(string num)
        {
            decimal parsedNumber;
            if(decimal.TryParse(num, out parsedNumber)) return true;
            return false;
        }

        public static bool IsNaturalNumber(string num)
        {
            int parsedNumber;
            if (int.TryParse(num, out parsedNumber) && parsedNumber >= 0) return true;
            return false;
        }
    }
}
