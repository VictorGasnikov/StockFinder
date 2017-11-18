using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Jace;

namespace StockFinder
{
    public class StockInfo
    {
        public static readonly CalculationEngine Engine = new CalculationEngine();
        #region declarations
        //     public static readonly List<string> DefaultProperties = new List<string> { "symbol", "name", "price", "eps", "pe_ratio", "book_value", "pb_ratio", "volume" };
        public static readonly Dictionary<string, StockProperty> DefaultProperties = new Dictionary<string, StockProperty>
        {
            { "ask", new StockProperty("ask", "Ask") },
            { "bid", new StockProperty("bid", "Bid") },
            { "prev_close", new StockProperty("prev_close", "Previous Close") },
            { "open", new StockProperty("open", "Open") },
            { "div_yield", new StockProperty("div_yield", "Dividend Yield") },
            { "div_per_share", new StockProperty("div_per_share", "Dividend Per Share") },
            { "div_pay_date", new StockProperty("div_pay_date", "Dividend Pay Date") {AllowFiltering = false} },
            { "ex_div_date", new StockProperty("ex_div_date", "Ex-Dividend Date") {AllowFiltering = false} },
            { "change", new StockProperty("change", "Change") },
            { "change_pct", new StockProperty("change_pct", "Change in Percent") },
            { "last_trade_date", new StockProperty("last_trade_date", "Last Trade Date") {AllowFiltering = false} },
            { "last_trade_time", new StockProperty("last_trade_time", "Last Trade Time") {AllowFiltering = false} },
            { "after_hours_change", new StockProperty("after_hours_change", "After Hours Change") },
            { "commission", new StockProperty("commission", "Commission") },
            { "day_low", new StockProperty("day_low", "Day's Low") },
            { "day_high", new StockProperty("day_high", "Day's High") },
            { "price", new StockProperty("price", "Price") },
            { "1_yr_target_price", new StockProperty("1_yr_target_price", "1 Year Trade Price") },
            { "50_day_moving_avg", new StockProperty("50_day_moving_avg", "50 Day Moving Average") },
            { "200_day_moving_avg", new StockProperty("200_day_moving_avg", "200 Day Moving Average") },
            { "change_200_day_moving_avg", new StockProperty("change_200_day_moving_avg", "Change From 200 Day Moving Average") },
            { "pct_change_200_day_moving_avg", new StockProperty("pct_change_200_day_moving_avg", "Percent Change From 200 Day Moving Average") },
            { "change_50_day_moving_avg", new StockProperty("change_50_day_moving_avg", "Change From 50 Day Moving Average") },
            { "pct_change_50_day_moving_avg", new StockProperty("pct_change_50_day_moving_avg", "Percent Change From 200 Day Moving Average") },
            { "revenue", new StockProperty("revenue", "Revenue") },
            { "52_week_high", new StockProperty("52_week_high", "52 Week High") },
            { "52_week_low", new StockProperty("52_week_low", "52 Week Low") },
            { "change_from_52_week_low", new StockProperty("change_from_52_week_low", "Change From 52 Week Low") },
            { "change_from_52_week_high", new StockProperty("change_from_52_week_high", "Change From 52 Week High") },
            { "pct_change_from_52_week_low", new StockProperty("pct_change_from_52_week_low", "Percent Change From 52 Week High") },
            { "pct_change_from_52_week_high", new StockProperty("pct_change_from_52_week_high", "Percent Change From 52 Week High") },
            { "market_cap", new StockProperty("market_cap", "Market Capitalization") },
            { "float_shares", new StockProperty("float_shares", "Float Shares") },
            { "name", new StockProperty("name", "Name") {AllowFiltering = false} },
            { "symbol", new StockProperty("symbol", "Symbol") {AllowFiltering = false} },
            { "stock_exchange", new StockProperty("stock_exchange", "Stock Exchange") {AllowFiltering = false} },
            { "shares_outstanding", new StockProperty("shares_outstanding", "Shares Outstanding") },
            { "volume", new StockProperty("volume", "Volume") },
            { "ask_size", new StockProperty("ask_size", "Ask Size") },
            { "bid_size", new StockProperty("bid_size", "Bid Size") },
            { "last_trade_size", new StockProperty("last_trade_size", "Last Trade Size") },
            { "avg_daily_volume", new StockProperty("avg_daily_volume", "Average Daily Volume") },
            { "eps", new StockProperty("eps", "EPS") },
            { "eps_estimate_current_year", new StockProperty("eps_estimate_current_year", "EPS Estimate Current Year") },
            { "eps_estimate_next_year", new StockProperty("eps_estimate_next_year", "EPS Estimate Next Year") },
            { "eps_estimate_current_quarter", new StockProperty("eps_estimate_current_quarter", "EPS Estimate Next Quarter") },
            { "book_value", new StockProperty("book_value", "Book Value") },
            { "ebitda", new StockProperty("ebitda", "EBITDA") },
            { "ps_ratio", new StockProperty("ps_ratio", "PS Ratio") },
            { "pb_ratio", new StockProperty("pb_ratio", "PB Ratio") },
            { "pe_ratio", new StockProperty("pe_ratio", "PE Ratio") },
            { "peg_ratio", new StockProperty("peg_ratio", "PEG Ratio") },
            { "price_eps_estimate_current_year", new StockProperty("price_eps_estimate_current_year", "Price/EPS Estimate Current Year") },
            { "price_eps_estimate_next_year", new StockProperty("price_eps_estimate_next_year", "Price/EPS Estimate Next Year") },
            { "short_ratio", new StockProperty("short_ratio", "Short Ratio") },

//            { "symbol", new StockProperty("Symbol") {AllowFiltering = false} },
//            { "name", new StockProperty("Name") {AllowFiltering = false} },
//            { "price", new StockProperty("Price") },
//            { "eps", new StockProperty("EPS") },
//            { "pe_ratio", new StockProperty("P/E Ratio") },
//            { "book_value", new StockProperty("Book Value") },
//            { "pb_ratio", new StockProperty("P/B Ratio") },
//            { "volume", new StockProperty("Volume") }
        };

        private static readonly List<string> DefaultVariableNames = new List<string>
        {
            "ask",
            "bid",
            "prev_close",
            "open",
            "div_yield",
            "div_per_share",
            "div_pay_date",
            "ex_div_date",
            "change",
            "change_pct",
            "last_trade_date",
            "last_trade_time",
            "after_hours_change",
            "commission",
            "day_low",
            "day_high",
            "price",
            "1_yr_target_price",
            "50_day_moving_avg",
            "200_day_moving_avg",
            "change_200_day_moving_avg",
            "pct_change_200_day_moving_avg",
            "change_50_day_moving_avg",
            "pct_change_50_day_moving_avg",
            "revenue",
            "52_week_high",
            "52_week_low",
            "change_from_52_week_low",
            "change_from_52_week_high",
            "pct_change_from_52_week_low",
            "pct_change_from_52_week_high",
            "market_cap",
            "float_shares",
            "name",
            "symbol",
            "stock_exchange",
            "shares_outstanding",
            "volume",
            "ask_size",
            "bid_size",
            "last_trade_size",
            "avg_daily_volume",
            "eps",
            "eps_estimate_current_year",
            "eps_estimate_next_year",
            "eps_estimate_current_quarter",
            "book_value",
            "ebitda",
            "ps_ratio",
            "pb_ratio",
            "pe_ratio",
            "peg_ratio",
            "price_eps_estimate_current_year",
            "price_eps_estimate_next_year",
            "short_ratio",
        };

        public static readonly List<PropertyGroup> Groups = new List<PropertyGroup>
        {
            new PropertyGroup() {
                Name = "Pricing",
                Variables = new List<string>
                {
                    "ask",
                    "bid",
                    "prev_close",
                    "open",
                }
            },
            new PropertyGroup() {
                Name = "Dividends",
                Variables = new List<string>
                {
                    "div_yield",
                    "div_per_share",
                    "div_pay_date",
                    "ex_div_date",
                }
            },
            new PropertyGroup() {
                Name = "Date",
                Variables = new List<string>
                {
                    "change",
                    "change_pct",
                    "last_trade_date",
                    "last_trade_time",
                }
            },new PropertyGroup() {
                Name = "Averages",
                Variables = new List<string>
                {
                    "after_hours_change",
                    "commission",
                    "day_low",
                    "day_high",
                    "price",
                    "1_yr_target_price",
                    "50_day_moving_avg",
                    "200_day_moving_avg",
                    "change_200_day_moving_avg",
                    "pct_change_200_day_moving_avg",
                    "change_50_day_moving_avg",
                    "pct_change_50_day_moving_avg",
                }
            },new PropertyGroup() {
                Name = "Misc",
                Variables = new List<string>
                {
                    "revenue",
                }
            },new PropertyGroup() {
                Name = "52 Week Pricing",
                Variables = new List<string>
                {
                    "52_week_high",
                    "52_week_low",
                    "change_from_52_week_high",
                    "change_from_52_week_low",
                    "pct_change_from_52_week_high",
                    "pct_change_from_52_week_low",
                }
            },new PropertyGroup() {
                Name = "Symbol Info",
                Variables = new List<string>
                {
                    "market_cap",
                    "float_shares",
                    "name",
                    "symbol",
                    "stock_exchange",
                    "shares_outstanding",
                }
            },new PropertyGroup() {
                Name = "Volume",
                Variables = new List<string>
                {
                    "volume",
                    "ask_size",
                    "bid_size",
                    "last_trade_size",
                    "avg_daily_volume",
                }
            },new PropertyGroup() {
                Name = "Ratios",
                Variables = new List<string>
                {
                    "eps",
                    "eps_estimate_current_year",
                    "eps_estimate_next_year",
                    "eps_estimate_current_quarter",
                    "book_value",
                    "ebitda",
                    "ps_ratio",
                    "pb_ratio",
                    "pe_ratio",
                    "peg_ratio",
                    "price_eps_estimate_current_year",
                    "price_eps_estimate_next_year",
                    "short_ratio",
                }
            },

        };
        #endregion

        public static Dictionary<string, string> AllPropertiesLabels => DefaultProperties.ToDictionary(prop => prop.Key, prop => prop.Value.Label).Concat(PropertyDefinitions.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value.Label))).ToDictionary(p => p.Key, p => p.Value);
        public static Dictionary<string, StockProperty> AllProperties => DefaultProperties.Concat(PropertyDefinitions.ToDictionary(prop => prop.Key, prop => (StockProperty)prop.Value)).ToDictionary(p => p.Key, p => p.Value);

        public static readonly Dictionary<string, CustomStockProperty> PropertyDefinitions = new Dictionary<string, CustomStockProperty>();

        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public Dictionary<string, Dictionary<string, object>> SubProperties { get; } = new Dictionary<string, Dictionary<string, object>>();

        public Dictionary<string, double> NumericProperties => Properties.Where(e => e.Value is double || e.Value is decimal).ToDictionary(e => e.Key, e => Convert.ToDouble(e.Value));
        //        {
        //            get
        //            {
        //                return Properties.Where(e => e.Value is double || e.Value is decimal).ToDictionary(e => e.Key, e => Convert.ToDouble(e.Value));
        //            }
        //        }

        public string Symbol => (string)Properties["symbol"];


        private void AddNumericProperty(string variable, string value)
        {
            Properties.Add(variable, NumberUtils.ParseDecimal(value));
        }

        public StockInfo(string[] vals)
        {
            for (int i = 0; i < DefaultVariableNames.Count; i++)
            {
                string varName = DefaultVariableNames[i];
                StockProperty property = DefaultProperties[varName];
                if (property.AllowFiltering)
                {
                    AddNumericProperty(varName, vals[i]);
                }
                else
                {
                    Properties.Add(varName, vals[i]);
                }
            }

            foreach (KeyValuePair<string, CustomStockProperty> propertyDefinition in PropertyDefinitions)
            {
                string variable = propertyDefinition.Key;
                Func<Dictionary<string, double>, double> expression = propertyDefinition.Value.Expression;
                Properties.Add(variable, EvaluateExpression(expression, NumericProperties));
            }
        }

        private static double EvaluateExpression(Func<Dictionary<string, double>, double> expression, Dictionary<string, double> numericProperties)
        {
            try
            {
                return expression(numericProperties);
            }
            catch (Exception)
            {
                return -1d;
            }
        }

        public bool IsValid()
        {
            return (decimal)Properties["price"] > -1;
        }
        //public string Symbol { get; set; }
        //public string Name { get; set; }
        //public double Price { get; set; }
        //public double Earnings { get; set; }
        //public double EPS { get; set; }
        //public double PEratio { get; set; }
        //public double BookValue { get; set; }
        //public double PBratio { get; set; }
        //public long Volume { get; set; }
        //public double DividendYield { get; set; }

    }

    public class StockProperty
    {
        public string Label { get; set; }
        public bool AllowFiltering { get; set; } = true;
        public string Variable { get; set; }
        public string DefaultFilterMinValue { get; set; } = "0";
        public string DefaultFilterMaxValue { get; set; } = "Max";
        public string Group { get; set; }

        public StockProperty()
        {

        }

        public StockProperty(string variable, string label)
        {
            Variable = variable;
            Label = label;
        }
    }

    [XmlInclude(typeof(CustomStockProperty))]
    public class CustomStockProperty : StockProperty
    {
        private string _formula;

        public string Formula
        {
            get { return _formula; }
            set
            {
                _formula = value;
                //                Expression = new Expression(value);
                Expression = StockInfo.Engine.Build(value);
            }
        }

        [XmlIgnore]
        public Func<Dictionary<string, double>, double> Expression { get; private set; }

//        public List<> 

        public CustomStockProperty()
        {

        }

        public CustomStockProperty(string variable, string formula, string label) : base(variable, label)
        {
            Formula = formula;
        }
    }

    public class PropertyGroup
    {
        public string Name { get; set; }
        public List<string> Variables { get; set; }
    }
}
