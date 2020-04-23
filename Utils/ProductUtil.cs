using System;
namespace  Jurumani.BotBuilder.Utils
{
    public static class ProductUtil
    {
        public static string SUFFIX = "-HW";
        public static string generateSKU(string model)
        {
            string result = string.Empty;
            int idx = model.IndexOf(SUFFIX);
            if (idx >= 0)
            {
                result = model.Remove(idx, SUFFIX.Length);
            }
            return result;

        }
    }
}
