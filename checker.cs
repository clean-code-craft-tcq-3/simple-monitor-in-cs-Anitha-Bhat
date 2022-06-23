
using System;
using Properties;
using System.Diagnostics;
using System.Globalization;

public class Checker
{
  static double TEMPERATURE_UPPER_LIMIT = 45;
  static double TEMPERATURE_LOWER_LIMIT = 0;
  static double SOC_UPPER_LIMIT = 80;
  static double SOC_LOWER_LIMIT = 20;
  static double CHARGERATE_LOWER_LIMIT = 0.8;


  public static bool batteryIsOk(double temperature, double stateOfCharge, double chargeRate)
  {
    return !isBatteryInWarningRange(temperature,stateOfCharge,chargeRate)&&isBatteryInValidRange(temperature,stateOfCharge,chargeRate);

  }
  
  private static bool isBatteryInValidRange(double temperature, double stateOfCharge, double chargeRate){
   return isTemperatureInValidRange(temperature) && isStateOfChargeInValidRange(stateOfCharge) && isChargeRateInValidRange(chargeRate);
  }

  private static bool isBatteryInWarningRange(double temperature, double stateOfCharge, double chargeRate){
   return  monitorChargeRateWarningRange(chargeRate) && monitorSOCWarningRange(stateOfCharge) &&
        monitorTemperatureWarningRange(temperature);
  }


  public static bool isTemperatureInValidRange(double temperature)
  {
    if (temperature < TEMPERATURE_LOWER_LIMIT || temperature > TEMPERATURE_UPPER_LIMIT)
    {
      printText(Resource.TempOutOfRange);
      return false;
    }

    return true;
  }




  public static bool isStateOfChargeInValidRange(double stateOfCahrge)
  {
    if (stateOfCahrge < SOC_LOWER_LIMIT || stateOfCahrge > SOC_UPPER_LIMIT)
    {
      printText(Resource.SocOutOfRange);
      return false;
    }
    return true;
  }



  public static bool isChargeRateInValidRange(double chargeRate)
  {
    if (chargeRate > CHARGERATE_LOWER_LIMIT)
    {
      printText(Resource.ChargeOutOfRange);
      return false;
    }

    return true;
  }

  public static void printText(string text)
  {
    Console.WriteLine(text);
  }

  public static double getUpperTolerenceValue(double upperLimit, double value)
  {
    return (double)value - 0.05 * upperLimit;

  }
  public static double getLowerTolerenceValue(double lowerLimt, double value)
  {
    return (double)value + 0.05 * lowerLimt;
  }

  public static bool isLowerLimitInWarningRange(double temperature, double lowerLimit, Action printAction)
  {
    if (getLowerTolerenceValue(lowerLimit, temperature) >= temperature)
    {
      printAction();
      return true;
    }
    return false;
  }

  public static bool isUpperLimitInWarningRange(double temperature, double upperLimit, Action printAction)
  {
    if (getUpperTolerenceValue(upperLimit, temperature) <= temperature)
    {
      printAction();
      return true;
    }
    return false;
  }

  public static bool monitorSOCWarningRange(double stateOfCharge)
  {
    if (isLowerLimitInWarningRange(stateOfCharge, SOC_LOWER_LIMIT, () => printText(Resource.WarningDischarge)) || isUpperLimitInWarningRange(stateOfCharge, SOC_UPPER_LIMIT, () => printText(Resource.WarningPeak)))
    {
      return true;

    }
    return false;
  }


  public static bool monitorChargeRateWarningRange(double chargeRate)
  {

    if (isLowerLimitInWarningRange(chargeRate, CHARGERATE_LOWER_LIMIT, () => printText(Resource.WarningDischarge)))
    {
      return true;
    }
    return false;
  }

  public static bool monitorTemperatureWarningRange(double temperature)
  {
    if (isLowerLimitInWarningRange(temperature, TEMPERATURE_LOWER_LIMIT, () => printText(Resource.WarningDischarge)) || isUpperLimitInWarningRange(temperature, TEMPERATURE_UPPER_LIMIT, () => printText(Resource.WarningPeak)))
    {
      return true;
    }
    return false;
  }
  public static int Main()
  {

    Resource.Culture = new CultureInfo("de");
    Debug.Assert(batteryIsOk(0, 19, 1) == false);
    Debug.Assert(batteryIsOk(1, 21, 1) == false);
    Debug.Assert(batteryIsOk(1, 81, 0.7) == false);
    Debug.Assert(batteryIsOk(-1, 79, 0.7) == false);
    Debug.Assert(batteryIsOk(1, 21, 0.7) == false);
    Debug.Assert(batteryIsOk(44, 79, 0.7) == false);

    //Check if temperature in range
    Debug.Assert(isTemperatureInValidRange(0) == true);
    Debug.Assert(isTemperatureInValidRange(45) == true);
    Debug.Assert(isTemperatureInValidRange(-1) == false);
    Debug.Assert(isTemperatureInValidRange(46) == false);
    Debug.Assert(isTemperatureInValidRange(40) == true);


    Debug.Assert(isStateOfChargeInValidRange(20) == true);
    Debug.Assert(isStateOfChargeInValidRange(80) == true);
    Debug.Assert(isStateOfChargeInValidRange(19) == false);
    Debug.Assert(isStateOfChargeInValidRange(81) == false);
    Debug.Assert(isStateOfChargeInValidRange(21) == true);

    //Check if chargerate in range
    Debug.Assert(isChargeRateInValidRange(0.9) == false);
    Debug.Assert(isChargeRateInValidRange(0.7) == true);
    Debug.Assert(isChargeRateInValidRange(0) == true);

    //check temp warning levels
    Debug.Assert(monitorTemperatureWarningRange(1) == true);
    Debug.Assert(monitorTemperatureWarningRange(2) == true);
    Debug.Assert(monitorTemperatureWarningRange(1) == true);
    Debug.Assert(monitorTemperatureWarningRange(2) == true);
    Debug.Assert(monitorTemperatureWarningRange(43) == true);
    Debug.Assert(monitorTemperatureWarningRange(44) == true);

    //Check soc warning levels
    Debug.Assert(monitorSOCWarningRange(21) == true);
    Debug.Assert(monitorSOCWarningRange(22) == true);
    Debug.Assert(monitorSOCWarningRange(23) == true);
    Debug.Assert(monitorSOCWarningRange(24) == true);
    Debug.Assert(monitorSOCWarningRange(77) == true);
    Debug.Assert(monitorSOCWarningRange(78) == true);
    Debug.Assert(monitorSOCWarningRange(79) == true);
    Debug.Assert(monitorSOCWarningRange(78) == true);
    Debug.Assert(monitorSOCWarningRange(79) == true);

    //Check charge rate warnings
    Debug.Assert(monitorChargeRateWarningRange(0.79) == true);
    Debug.Assert(monitorChargeRateWarningRange(0.77) == true);
    Debug.Assert(monitorChargeRateWarningRange(0.78) == true);
    Debug.Assert(monitorChargeRateWarningRange(0.78) == true);

    Console.WriteLine("All ok");
    Console.Read();
    return 0;
  }
}
