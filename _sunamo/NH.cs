namespace SunamoGoogleSheets;





//namespace SunamoGoogleSheets;

////namespace SunamoGoogleSheets;

//internal static class NH
//{
//    //internal static Func<List<double>, string> CalculateMedianAverage;

//    internal static T Average<T>(List<T> list)
//    {
//        return Average<T>(Sum<T>(list), list.Count);
//    }

//    internal static T Sum<T>(List<T> list)
//    {
//        dynamic sum = 0;
//        foreach (var item in list)
//        {
//            sum += item;
//        }
//        return sum;
//    }

//    private static object ReturnZero<T>()
//    {
//        var t = typeof(T);
//        if (t == Types.tDouble)
//        {
//            return NumConsts.zeroDouble;
//        }
//        else if (t == Types.tInt)
//        {
//            return NumConsts.zeroInt;
//        }
//        else if (t == Types.tFloat)
//        {
//            return NumConsts.zeroFloat;
//        }
//        ThrowEx.NotImplementedCase(t.FullName);
//        return null;
//    }

//    internal static T Average<T>(dynamic gridWidth, dynamic columnsCount)
//    {
//        if (EqualityComparer<T>.Default.Equals(columnsCount, (T)NH.ReturnZero<T>()))
//        {
//            return (T)NH.ReturnZero<T>();
//        }

//        if (EqualityComparer<T>.Default.Equals(gridWidth, (T)NH.ReturnZero<T>()))
//        {
//            return (T)NH.ReturnZero<T>();
//        }

//        dynamic result = gridWidth / columnsCount;
//        return result;
//    }

//    internal static string CalculateMedianAverage(List<double> list, out MedianAverage<double> medianAverage)
//    {
//        list.RemoveAll(d => d == 0);

//        ThrowEx.OnlyOneElement("list", list);

//        medianAverage = new MedianAverage<double>();
//        medianAverage.count = list.Count;
//        medianAverage.median = NH.Median<double>(list);
//        medianAverage.average = NH.Average<double>(list);
//        medianAverage.min = list.Min();
//        medianAverage.max = list.Max();

//        return medianAverage.ToString();
//    }

//    internal static T Median<T>(this IList<T> list) where T : IComparable<T>
//    {
//        return list.NthOrderStatistic((list.Count - 1) / 2);
//    }

//    internal static string CalculateMedianAverage(List<double> list)
//    {
//        MedianAverage<double> medianAverage = null;
//        return CalculateMedianAverage(list, out medianAverage);
//    }
//}
