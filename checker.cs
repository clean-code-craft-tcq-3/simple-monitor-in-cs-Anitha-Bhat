
using System;
using System.Diagnostics;
using System.Globalization;
using Properties;

public class Checker
{
  static double TEMPERATURE_UPPER_LIMIT = 45;
  static double TEMPERATURE_LOWER_LIMIT = 0;
  static double SOC_UPPER_LIMIT = 80;
  static double SOC_LOWER_LIMIT = 20;
  static double CHARGERATE_LOWER_LIMIT = 0.8;


  static bool batteryIsOk(double temperature, double stateOfCharge, double chargeRate)
  {
    if (monitorChargeRateWarningRange(chargeRate) && monitorSOCWarningRange(stateOfCharge) &&
        monitorTemperatureWarningRange(temperature))
    {
      return false;
    }

    return isTemperatureInValidRange(temperature) && isStateOfChargeInValidRange(stateOfCharge) && isChargeRateInValidRange(chargeRate);

  }



  private static bool isTemperatureInValidRange(double temperature)
  {
    if (temperature < TEMPERATURE_LOWER_LIMIT || temperature > TEMPERATURE_UPPER_LIMIT)
    {
      printText(Resource.TempOutOfRange);
      return false;
    }

    return true;
  }




  private static bool isStateOfChargeInValidRange(double stateOfCahrge)
  {
    if (stateOfCahrge < SOC_LOWER_LIMIT || stateOfCahrge > SOC_UPPER_LIMIT)
    {
      printText(Resource.SocOutOfRange);
      return false;
    }
    return true;
  }



  private static bool isChargeRateInValidRange(double chargeRate)
  {
    if (chargeRate > CHARGERATE_LOWER_LIMIT)
    {
      printText(Resource.ChargeOutOfRange);
      return false;
    }

    return true;
  }

  private static void printText(string text)
  {
    Console.WriteLine(text);
  }

  private static double getUpperTolerenceValue(double upperLimit, double value)
  {
    return (double)value - 0.05 * upperLimit;

  }
  private static double getLowerTolerenceValue(double lowerLimt, double value)
  {
    return (double)value + 0.05 * lowerLimt;
  }

  private static bool isLowerLimitInWarningRange(double temperature, double lowerLimit, Action printAction)
  {
    if (getLowerTolerenceValue(lowerLimit, temperature) >= temperature)
    {
      printAction();
      return true;
    }
    return false;
  }

  private static bool isUpperLimitInWarningRange(double temperature, double upperLimit, Action printAction)
  {
    if (getUpperTolerenceValue(upperLimit, temperature) <= temperature)
    {
      printAction();
      return true;
    }
    return false;
  }

  private static bool monitorSOCWarningRange(double stateOfCharge)
  {
    if (isLowerLimitInWarningRange(stateOfCharge, SOC_LOWER_LIMIT, () => printText(Resource.WarningDischarge)) || isUpperLimitInWarningRange(stateOfCharge, SOC_UPPER_LIMIT, () => printText(Resource.WarningPeak)))
    {
      return true;

    }
    return false;
  }


  private static bool monitorChargeRateWarningRange(double chargeRate)
  {

    if (isLowerLimitInWarningRange(chargeRate, CHARGERATE_LOWER_LIMIT, () => printText(Resource.WarningDischarge)))
    {
      return true;
    }
    return false;
  }

  private static bool monitorTemperatureWarningRange(double temperature)
  {
    if (isLowerLimitInWarningRange(temperature, TEMPERATURE_LOWER_LIMIT, () => printText(Resource.WarningDischarge)) || isUpperLimitInWarningRange(temperature, TEMPERATURE_UPPER_LIMIT, () => printText(Resource.WarningPeak)))
    {
      return true;
    }
    return false;
  }
  static int Main()
  {

    Resources.Culture = new CultureInfo("de");
    Assert.Equal(batteryIsOk(0, 19, 1), false);
    Assert.Equal(batteryIsOk(1, 21, 1), false);
    Assert.Equal(batteryIsOk(1, 81, 0.7), false);
    Assert.Equal(batteryIsOk(-1, 79, 0.7), false);
    Assert.Equal(batteryIsOk(1, 21, 0.7), false);
    Assert.Equal(batteryIsOk(44, 79, 0.7), false);

    //Check if temperature in range
    Assert.Equal(isTemperatureInValidRange(0), true);
    Assert.Equal(isTemperatureInValidRange(45), true);
    Assert.Equal(isTemperatureInValidRange(-1), false);
    Assert.Equal(isTemperatureInValidRange(46), false);
    Assert.Equal(isTemperatureInValidRange(40), true);


    Assert.Equal(isStateOfChargeInValidRange(20), true);
    Assert.Equal(isStateOfChargeInValidRange(80), true);
    Assert.Equal(isStateOfChargeInValidRange(19), false);
    Assert.Equal(isStateOfChargeInValidRange(81), false);
    Assert.Equal(isStateOfChargeInValidRange(21), true);

    //Check if chargerate in range
    Assert.Equal(isChargeRateInValidRange(0.9), false);
    Assert.Equal(isChargeRateInValidRange(0.7), true);
    Assert.Equal(isChargeRateInValidRange(0), true);

    //check temp warning levels
    Assert.Equal(monitorTemperatureWarningRange(1), true);
    Assert.Equal(monitorTemperatureWarningRange(2), true);
    Assert.Equal(monitorTemperatureWarningRange(1), true);
    Assert.Equal(monitorTemperatureWarningRange(2), true);
    Assert.Equal(monitorTemperatureWarningRange(43), true);
    Assert.Equal(monitorTemperatureWarningRange(44), true);

    //Check soc warning levels
    Assert.Equal(monitorSOCWarningRange(21), true);
    Assert.Equal(monitorSOCWarningRange(22), true);
    Assert.Equal(monitorSOCWarningRange(23), true);
    Assert.Equal(monitorSOCWarningRange(24), true);
    Assert.Equal(monitorSOCWarningRange(77), true);
    Assert.Equal(monitorSOCWarningRange(78), true);
    Assert.Equal(monitorSOCWarningRange(79), true);
    Assert.Equal(monitorSOCWarningRange(78), true);
    Assert.Equal(monitorSOCWarningRange(79), true);

    //Check charge rate warnings
    Assert.Equal(monitorChargeRateWarningRange(0.79), true);
    Assert.Equal(monitorChargeRateWarningRange(0.77), true);
    Assert.Equal(monitorChargeRateWarningRange(0.78), true);
    Assert.Equal(monitorChargeRateWarningRange(0.78), true);

    Console.WriteLine("All ok");
    Console.Read();
    return 0;
  }
}
