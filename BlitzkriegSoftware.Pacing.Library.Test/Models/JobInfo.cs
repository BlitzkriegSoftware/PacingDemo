using System;

namespace BlitzkriegSoftware.Pacing.Library.Test.Models
{
    public class JobInfo
    {
        public int Id { get; set; }
        
        public int JobDuration { get; set; }

        public DateTime QuitTime { get; set; }

        public JobStatus Status { get; set; }

        private JobInfo() { }

        public JobInfo(int id, int duration)
        {
            this.Id = id;
            this.JobDuration = duration;
            this.Status = JobStatus.Active;
            this.QuitTime = DateTime.UtcNow.AddMilliseconds(this.JobDuration);
        }

        public bool JobDone
        {
            get
            {
                return (this.QuitTime < DateTime.UtcNow);
            }
        }

        public string Describe()
        {
            var msg = $"[{this.Id:000}] At: {DateTime.UtcNow:mm-ss.ffff}, Duration: {this.JobDuration} ms, Expires {this.QuitTime:mm-ss.ffff}";

            if (this.JobDone) this.Status = JobStatus.Done;

            switch (this.Status)
            {
                case JobStatus.Active:
                    msg += " (active)";
                    break;
                case JobStatus.Done:
                    msg += " (done)";
                    break;
                case JobStatus.Dead:
                    msg = string.Empty;
                    break;
            }

            if (this.Status == JobStatus.Done) this.Status = JobStatus.Dead;
            return msg;
        }

    }

    public enum JobStatus
    {
        Active = 1,
        Done = 2,
        Dead = 3
    }
}
