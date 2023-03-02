namespace MuProc
{
    public class Note
    {
        private string noteName;
        private double frequency;
        /// <summary>
        /// 0.25 for 1/4 Note
        /// </summary>
        private double duration;

        public Note(string name, double frequency, double duration=0.5)
        {
            this.noteName = name;
            this.frequency = frequency;
            this.duration = duration;
        }

        public Note(Note note)
        {
            this.noteName = note.Name;
            this.frequency = note.Frequency;
            this.duration = note.Duration;
        }

        public double Frequency => frequency;

        public double Duration => duration;
    
        public string Name => noteName;
    }
}
