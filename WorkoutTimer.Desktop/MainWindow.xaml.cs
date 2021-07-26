namespace WorkoutTimer.Desktop
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = PlanningAndTrackingOfWorkoutLoop.Create();
        }
    }
}