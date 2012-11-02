using com.espertech.esper.client;

namespace DarkLight.Responses
{
    public static class Statements
    {
        public static EPStatement AverageStatement(this EPAdministrator admin, int length)
        {
            var statement = 
                "insert into Average " +
                "select Symbol as symbol, avg(Close) as aveClose " +
                "from Bar.win:length(" + length + ") " +
                "group by symbol";

            return admin.CreateEPL(statement);
        }

        public static EPStatement StandardDeviationStatement(this EPAdministrator admin, int length)
        {
            var statement = 
                "insert into StandardDeviation " +
                "select Symbol as symbol, stddev(Close) as stdClose "+
                "from Bar.win:length(" + length + ") " +
                "group by symbol";

            return admin.CreateEPL(statement);
        }

        public static EPStatement NormalizedPriceStatement(this EPAdministrator admin)
        {
            var statement = 
                "insert into NormalizedPrice " +
                "select t.symbol as symbol, (t.trade - a.aveClose)/sd.stdClose as normPrice " + 
                "from Tick as t unidirectional, Average as a, StandardDeviation as sd " +
                "where t.symbol = a.symbol and t.symbol = sd.symbol";

            return admin.CreateEPL(statement);
        }

        public static EPStatement EqualWeightIndexStatement(this EPAdministrator admin)
        {
            var statement =
                "insert into EqualWeightIndex " +
                "select avg(normPrice) as averageNormPrice " +
                "from NormalizedPrice.std:groupwin(symbol).win:length(1)";
            return admin.CreateEPL(statement);
        }

        public static EPStatement DeviationFromIndexStatement(this EPAdministrator admin)
        {
            var statement =
                "insert into DeviationFromIndex " +
                "select p.symbol as symbol, Math.Abs(i.averageNormPrice - p.normPrice) as deviation " +
                "from EqualWeightIndex as i, NormalizedPrice as p unidirectional";
            return admin.CreateEPL(statement);
        }

        public static EPStatement MaxDeviationFromIndexStatement(this EPAdministrator admin)
        {
            var statement =
                "select symbol, max(deviation) as maxDeviation " +
                "from DeviationFromIndex.std:groupwin(symbol).win:length(1)";
            return admin.CreateEPL(statement);
        }
    }
}
