
namespace CaptainCombat.Server.Collector
{
    class CollectorClass
    {
        private ICollector collector;

        public void BeginCollect()
        {
            collector.Collect();
        }

       
        

        public void SetCollector(ICollector collector)
        {
            this.collector = collector;
        }

        public void PrintUpdateComponents()
        {
            collector.PrintUpdateComponents();
        }

    }
}
