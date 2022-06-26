namespace AutoScheduler.Models
{
    public class UserStatistic {
        public string Name { get; set; }
        public double Figure { get; set; }

        public UserStatistic(string name, double figure)
        {
            this.Name = name;
            this.Figure = figure;
        }
    }
}