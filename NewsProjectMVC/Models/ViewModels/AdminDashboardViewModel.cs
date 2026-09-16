namespace NewsProjectMVC.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // --- Aggregate Counts for Statistics Cards ---
        public int NewsCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;
        public int CategoriesCount { get; set; } = 0;
        public int TagsCount { get; set; } = 0;

        // --- Chart Data ---

        /// <summary>
        /// Holds the labels for the chart's X-axis (e.g., "Jan", "Feb", "Mar", etc.).
        /// This list is shared by all datasets in the chart.
        /// </summary>
        public List<string> ChartLabels { get; set; } = new List<string>();

        /// <summary>
        /// Holds the data points for the "Published News" line on the chart.
        /// Each integer corresponds to a label in ChartLabels.
        /// </summary>
        public List<int> ChartDataNews { get; set; } = new List<int>();

        /// <summary>
        /// Holds the data points for the "Published Comments" line on the chart.
        /// Each integer corresponds to a label in ChartLabels.
        /// </summary>
        public List<int> ChartDataComments { get; set; } = new List<int>();
    }
}
