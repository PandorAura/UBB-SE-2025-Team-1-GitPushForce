namespace Src.Services
{
    using System;

    public interface ILoanCheckerService
    {
        public event EventHandler LoansUpdated;

        public void Start();
        public void Stop();
    }
}
