using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFinder.Technical.Indicators
{
    public class StochasticsIndicator : TechnicalIndicator
    {
        private int _period;

        public StochasticsIndicator(string name, int period) : base(name, period)
        {
            _period = period;
        }

        protected override void CalculationEnd()
        {
            throw new NotImplementedException();
        }

        protected override void CalculateFor(int current)
        {

        }
    }
}
