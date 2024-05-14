SELECT LocalClinicalCodeValue, ReadCodeValue, hb_extract, SampleName, [ArithmeticComparator], Interpretation, QuantityUnit, RangeHighValue, RangeLowValue,

       COUNT(*) as RecordCount,

       AVG(QuantityValue) AS QVAverage, STDEV(QuantityValue) as QVStandardDev

FROM [dbo].[Labs_Biochem]

GROUP BY LocalClinicalCodeValue, ReadCodeValue, hb_extract, SampleName, QuantityUnit, RangeHighValue, RangeLowValue, [ArithmeticComparator], Interpretation HAVING COUNT(*) > 5

ORDER BY LocalClinicalCodeValue