using System;
using BlitzkriegSoftware.Pacing.Library.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlitzkriegSoftware.Pacing.Library.Test.Helpers
{
    public class JobReporter : IObserver<JobInfo>
    {
        private IDisposable unsubscriber;
        private readonly string _name;
        private TestContext _testContext;

        public JobReporter(string name, TestContext context)
        {
            _name = name;
            _testContext = context;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public virtual void Subscribe(IObservable<JobInfo> provider)
        {
            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }

        public virtual void OnCompleted()
        {
            _testContext.WriteLine($"Completed: {this.Name} at {DateTime.UtcNow:mm-ss-ffff}.");
            this.Unsubscribe();
        }

        public virtual void OnError(Exception e)
        {
            _testContext.WriteLine("{0}: Error: {1}", this.Name, e.Message);
        }

        public virtual void OnNext(JobInfo value)
        {
            // Don't show done/done
            if (value.DoneFlag) return;
            
            // show 1st one
            if (value.JobDone)
            {
                _testContext.WriteLine($"{this.Name} - DONE: {value.Id} at {DateTime.UtcNow:mm-ss-ffff}");
                this.Unsubscribe();
            }
            else {
                _testContext.WriteLine($"{this.Name} - WORKING: Id: {value.Id}, Milliseconds: {value.JobDuration} , Until {value.QuitTime:mm-ss-ffff}");
            }
        }

        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

    }
}
