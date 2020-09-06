using System;

namespace BlitzkriegSoftware.Pacing.Library.Test.Models
{
    public class JobInfo
    {
        public int Id { get; set; }
        
        public int JobDuration { get; set; }

        public DateTime QuitTime { get; set; }

        public bool DoneFlag { get; set; }

        private JobInfo() { }

        public JobInfo(int id, int duration)
        {
            this.Id = id;
            this.JobDuration = duration;
            this.QuitTime = DateTime.UtcNow.AddMilliseconds(this.JobDuration);
        }

        public bool JobDone
        {
            get
            {
                this.DoneFlag = (this.QuitTime < DateTime.UtcNow);
                return this.DoneFlag;
            }
        }
    }
}
