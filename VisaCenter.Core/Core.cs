using System;
using System.Threading.Tasks;

namespace VisaCenter.Core
{
    public static class Core
    {
        public static async Task ValidateInitialData()
        {
            await Task.Delay(2000);
        }

        public static async Task CheckPersonDataFromPolice()
        {
            await Task.Delay(3000);
        }

        public static async Task GeneratePdf()
        {
            await Task.Delay(2000);
        }

        public static async Task CheckPersonFromLocalGoverment()
        {
            await Task.Delay(2000);
        }

        public static async Task CheckPhotoIsNotPhotoshopEdited()
        {
            await Task.Delay(22000);
        }

        public static async Task CalculateCreditRating()
        {
            await Task.Delay(10000);
        }
    }
}
