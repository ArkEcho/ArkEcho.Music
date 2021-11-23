namespace ArkEcho
{
    public static class EM_String
    {
        public static string RemoveCRLF(this string inputString)
        {
            return inputString.Replace("\r\n", "").Replace("\n", "");
        }
    }
}
