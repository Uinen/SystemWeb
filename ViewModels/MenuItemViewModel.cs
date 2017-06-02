namespace GestioniDirette.Models
{
    public class MenuItemViewModel
    {
        public string Caption { get; set; }

        public string Url { get; set; }

        public override string ToString()
        {
            return $"Caption: {Caption}, Url: {Url}";
        }
    }
}