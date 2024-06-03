namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth > today)
            {
                return 0000;
            }

            // Adjust age if the birthday has not occurred yet this year
            if (dateOfBirth.Date > today.AddYears(-age)) age--;

            return age;
        }
    }
}
