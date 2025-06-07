namespace PowerBIViewer.App.Models
{
    public class Widget
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            // Dit bepaalt hoe het item verschijnt in de ListBox (bv: 📈 KPI Widget)
            return $"{Icon} {Title}";
        }
    }
}
