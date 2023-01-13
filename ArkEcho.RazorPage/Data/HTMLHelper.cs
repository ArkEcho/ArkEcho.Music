namespace ArkEcho.RazorPage.Data
{
    public class HTMLHelper
    {
        public string GetBase64PngImg(string base64String)
        {
            return $"data:image/png;base64,{base64String}";
        }
    }
}
