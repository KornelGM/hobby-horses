using System;

public static class TextFormater
{
    /// <summary>
    /// 
    ///Depending on the number of grams, it displays the weight in g or kg with an accuracy of two decimal places
    /// </summary>
    /// <param name="weight">float</param>
    /// <returns>Text in format: "0g /0.00kg"</returns>
    public static string FormatWeight(float weight)
    {
        string correctFormat;

        if (weight < 1000)
        {
            correctFormat = $"{weight:0} g";
        }
        else
        {
            float temp = weight / 1000;
            temp = (float)Math.Truncate(100 * temp) / 100;
            correctFormat = temp + " kg";
        }

        return correctFormat;
    }
    
    public static string FormatTemperature(float temperature)
    {
        return $"{temperature:0.0} °C";
    }
}
