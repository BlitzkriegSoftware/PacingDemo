using System;
using System.Collections.Generic;
using BlitzkriegSoftware.Pacing.Library.Test.Models;

namespace BlitzkriegSoftware.Pacing.Library.Test.Helpers
{
    public class JobTracker : IObservable<JobInfo>
    {
        private List<IObserver<JobInfo>> observers;
        private Dictionary<int, JobInfo> jobs;

        public JobTracker()
        {
            observers = new List<IObserver<JobInfo>>();
            jobs = new Dictionary<int, JobInfo>();
        }

        public Dictionary<int, JobInfo> Jobs
        {
            get
            {
                return jobs;
            }
        }

        public IDisposable Subscribe(IObserver<JobInfo> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        public void TrackJob(JobInfo job)
        {
            if (!jobs.ContainsKey(job.Id)) jobs.Add(job.Id, job);

            foreach (var observer in observers.ToArray())
            {
                observer.OnNext(job);
            }
        }

        public void EndObserve()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }

        #region "Unsubscriber"

        private class Unsubscriber : IDisposable
        {
            private bool disposed = false;
            private List<IObserver<JobInfo>> _observers;
            private IObserver<JobInfo> _observer;

            public Unsubscriber(List<IObserver<JobInfo>> observers, IObserver<JobInfo> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            #region "Disposable"

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposed)
                {
                    return;
                }

                if (disposing)
                {
                    if (_observer != null && _observers.Contains(_observer))
                        _observers.Remove(_observer);
                }

                disposed = true;
            }

            #endregion

        }

        #endregion

    }
}
